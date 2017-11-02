using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Medo.Security.Cryptography.PasswordSafe;

namespace Bimil {
    internal partial class SearchWeakForm : Form {
        public SearchWeakForm(Document document, IList<string> categories) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;
            Medo.Windows.Forms.State.SetupOnLoadAndClose(this);
            lsvEntries.SmallImageList = Helpers.GetImageList(this, "picWarning", "picError");

            this.Document = document;
            this.Categories = categories;

            bwSearchWeak.RunWorkerAsync();
        }

        private readonly Document Document;
        private readonly IList<string> Categories;


        private void Form_FormClosing(object sender, FormClosingEventArgs e) {
            if (bwSearchWeak.IsBusy) { bwSearchWeak.CancelAsync(); }
            if (bwSearchHibp.IsBusy) { bwSearchHibp.CancelAsync(); }
        }

        private void Form_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyData == Keys.Escape) {
                this.Close();
            }
        }

        private void Form_Resize(object sender, System.EventArgs e) {
            lsvEntries.Columns[0].Width = lsvEntries.ClientSize.Width;
        }


        private void lsvEntries_ItemActivate(object sender, System.EventArgs e) {
            if ((this.Document == null) || (lsvEntries.SelectedItems.Count != 1)) { return; }

            var entry = (Entry)(lsvEntries.SelectedItems[0].Tag);
            using (var frm = new ItemForm(this.Document, entry, this.Categories, startsAsEditable: Settings.EditableByDefault, hideAutotype: true)) {
                frm.ShowDialog(this);
            }
        }

        private void chbIncludeHibp_CheckedChanged(object sender, System.EventArgs e) {
            if (chbIncludeHibp.Checked) {
                if (bwSearchWeak.IsBusy == false) { bwSearchHibp.RunWorkerAsync(); } //start only if weak password search is not done yet - otherwise start on completion of it
                chbIncludeHibp.Enabled = false;
            }
        }


        private void bwSearchWeak_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            bwSearchHibp.ReportProgress(0, new ProgressState("Searching for weak passwords..."));

            var sw = Stopwatch.StartNew();
            var count = this.Document.Entries.Count;
            for (var i = 0; i < count; i++) {
                var entry = this.Document.Entries[i];
                foreach (var record in entry.Records) {
                    if (record.RecordType == RecordType.Password) {
                        if (BadPasswords.IsCommon(record.Text, out var matchedPassword)) {
                            var lvi = new ListViewItem(entry.Title) {
                                Tag = entry,
                                ImageIndex = 0,
                                ToolTipText = $"Password is similar to a common password ({matchedPassword})."
                            };
                            bwSearchWeak.ReportProgress(i * 100 / count, new ProgressState(lvi));
                            break;
                        }
                    }
                }
                if (bwSearchWeak.CancellationPending) {
                    e.Cancel = true;
                    return;
                }
            }
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Items searched at {0:0.0} ms", sw.ElapsedMilliseconds));
        }

        private void bwSearch_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e) {
            var state = (ProgressState)e.UserState;

            if (state.Item != null) {
                lsvEntries.BeginUpdate();
                lsvEntries.Items.Add(state.Item);
                lsvEntries.EndUpdate();
            }

            if (state.StatusText != null) {
                staStatusText.Text = state.StatusText;
                staStatusText.ToolTipText = state.StatusText;
            }

            if (!sta.IsDisposed) {
                if (!staProgress.Visible) { staProgress.Visible = true; }
                staProgress.Value = e.ProgressPercentage;
            }
        }

        private void bwSearchWeak_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
            if (!staProgress.IsDisposed) {
                staProgress.Visible = false;
                staStatusText.Text = "Completed.";
            }
            if (e.Cancelled == false) {
                if (chbIncludeHibp.Checked) {
                    bwSearchHibp.RunWorkerAsync();
                }
            }
        }


        private void bwSearchHibp_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            bwSearchHibp.ReportProgress(0, new ProgressState("';--have i been pwned?"));

            if (SearchHibpForBreaches() == false) {
                e.Cancel = true;
            } else if (Settings.CheckWeakPasswordAtHibp && (SearchHibpForPasswords() == false)) {
                e.Cancel = true;
            }
        }

        private bool SearchHibpForBreaches() {
            //collect all user-names
            var sw = Stopwatch.StartNew();
            var userEntryDictionary = new Dictionary<string, List<Entry>>();
            foreach (var entry in this.Document.Entries) {
                if (entry.Records.Contains(RecordType.Password)) {
                    foreach (var record in entry.Records) {
                        if ((record.RecordType == RecordType.EmailAddress) || (record.RecordType == RecordType.UserName)) {
                            var text = record.Text;
                            if (!string.IsNullOrWhiteSpace(text)) {
                                if (!userEntryDictionary.TryGetValue(text, out var entries)) {
                                    entries = new List<Entry>();
                                    userEntryDictionary.Add(text, entries);
                                }
                                if (!entries.Contains(entry)) { entries.Add(entry); }
                            }
                        }
                    }
                }
                if (bwSearchHibp.CancellationPending) { return false; }
            }

            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "User names cached at {0:0.0} ms", sw.ElapsedMilliseconds));

            //move dictionary to list so it gets sorted based on frequency
            var userEntryList = new List<AccountAndEntryStorage>();
            foreach (var kvp in userEntryDictionary) {
                userEntryList.Add(new AccountAndEntryStorage(kvp.Key, kvp.Value));
            }
            userEntryList.Sort((kvp1, kvp2) => {
                var count1 = kvp1.Entries.Count;
                var count2 = kvp2.Entries.Count;
                return (count1 > count2) ? -1 : (count1 < count2) ? +1 : 0;
            });

            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "User names sorted at {0:0.0} ms", sw.ElapsedMilliseconds));

            //Search all accounts for breaches
            for (var i = 0; i < userEntryList.Count; i++) {
                var userEntry = userEntryList[i];
                var percentage = i * 100 / userEntryList.Count;
                try {
                    var breaches = Hibp.GetAllBreaches(userEntry.Account);
                    ReportWebStatus(percentage, HttpStatusCode.OK, null, userEntry.Account);
                    foreach (var entry in userEntry.Entries) {
                        var modified = entry.PasswordModificationTime;
                        foreach (var record in entry.Records) {
                            if (record.RecordType == RecordType.Url) {
                                var url = record.Text;
                                foreach (var breach in breaches) {
                                    if (breach.IsApplicable(url, modified, entry.Title)) { //check only if we know the domain
                                        var lvi = new ListViewItem(entry.Title) {
                                            Tag = entry,
                                            ImageIndex = 1,
                                            ToolTipText = $"Account is present in {breach.Title} breach:\n{FilterHtml(breach.Description)}"
                                        };
                                        bwSearchHibp.ReportProgress(percentage, new ProgressState(lvi));
                                        break;
                                    }
                                }
                            }
                        }
                    }
                } catch (WebException ex) {
                    if (ex.Response is HttpWebResponse response) {
                        ReportWebStatus(percentage, response.StatusCode, response.StatusDescription, userEntry.Account);
                    } else {
                        ReportWebStatus(percentage, null, ex.Message, userEntry.Account);
                    }
                }

                Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Account {1} searched at {0:0.0} ms (followed by {2} ms throttling)", sw.ElapsedMilliseconds, userEntry.Account, ThrottleInterval));
                Thread.Sleep(ThrottleInterval);
            }

            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "{1} accounts searched in {0:0.0} ms (albeit with {2} ms throttling)", sw.ElapsedMilliseconds, userEntryList.Count, ThrottleInterval));
            return true;
        }

        private bool SearchHibpForPasswords() {
            var sw = Stopwatch.StartNew();

            var sha1 = new SHA1Managed();

            var userEntryDictionary = new Dictionary<string, List<Entry>>();
            foreach (var entry in this.Document.Entries) {
                if (entry.Records.Contains(RecordType.Password)) {
                    foreach (var record in entry.Records) {
                        if (record.RecordType == RecordType.Password) {
                            var sha1Bytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(record.Text));
                            var passwordHash = BitConverter.ToString(sha1Bytes).Replace("-", ""); //dirty conversion to hex
                            if (!string.IsNullOrWhiteSpace(passwordHash)) {
                                if (!userEntryDictionary.TryGetValue(passwordHash, out var entries)) {
                                    entries = new List<Entry>();
                                    userEntryDictionary.Add(passwordHash, entries);
                                }
                                if (!entries.Contains(entry)) { entries.Add(entry); }
                            }
                        }
                    }
                }
                if (bwSearchHibp.CancellationPending) { return false; }
            }

            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Passwords hashed at {0:0.0} ms", sw.ElapsedMilliseconds));

            //sort all passwords
            var userPasswords = new List<PasswordAndEntryStorage>();
            foreach (var kvp in userEntryDictionary) {
                userPasswords.Add(new PasswordAndEntryStorage(kvp.Key, kvp.Value));
                if (bwSearchHibp.CancellationPending) { return false; }
            }

            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Passwords filled at {0:0.0} ms", sw.ElapsedMilliseconds));

            //sort based on random value
            if (Settings.RandomizeWeakPasswordAtHibp) {
                userPasswords.Sort((item1, item2) => {
                    var count1 = item1.Entries.Count;
                    var count2 = item2.Entries.Count;
                    if (count1 > count2) { //sort based on number of entries with the same password
                        return -1;
                    } else if (count1 < count2) {
                        return +1;
                    } else { //just randomize
                        return (item1.RandomValue < item2.RandomValue) ? -1
                             : (item1.RandomValue > item2.RandomValue) ? +1
                                                                       : 0;
                    }
                });
            } else {
                userPasswords.Sort((item1, item2) => {
                    var count1 = item1.Entries.Count;
                    var count2 = item2.Entries.Count;
                    if (count1 > count2) { //sort based on number of entries with the same password
                        return -1;
                    } else if (count1 < count2) {
                        return +1;
                    } else { //order by password hash
                        return string.CompareOrdinal(item1.PasswordHash, item2.PasswordHash);
                    }
                });
            }

            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Passwords sorted at {0:0.0} ms", sw.ElapsedMilliseconds));

            for (var i = 0; i < userPasswords.Count; i++) {
                var percentage = i * 100 / userPasswords.Count;
                var item = userPasswords[i];

                try {
                    if (Hibp.IsPassworPwned(item.PasswordHash)) {
                        ReportWebStatus(percentage, HttpStatusCode.OK, null, item.Entries[0].Title);
                        foreach (var entry in item.Entries) {
                            var lvi = new ListViewItem(entry.Title) {
                                Tag = entry,
                                ImageIndex = 1,
                                ToolTipText = $"Password is present in breached password collection at Have I been pwned? site."
                            };
                            bwSearchHibp.ReportProgress(percentage, new ProgressState(lvi));
                        }
                    } else {
                        ReportWebStatus(percentage, HttpStatusCode.NotFound, null, item.Entries[0].Title);
                    }
                } catch (WebException ex) {
                    if (ex.Response is HttpWebResponse response) {
                        ReportWebStatus(percentage, response.StatusCode, response.StatusDescription, item.Entries[0].Title);
                    } else {
                        ReportWebStatus(percentage, null, ex.Message, item.Entries[0].Title);
                    }
                }

                Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Password for {1} entries searched at {0:0.0} ms (followed by {2} ms throttling)", sw.ElapsedMilliseconds, item.Entries.Count, ThrottleInterval));
                Thread.Sleep(ThrottleInterval);
            }

            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "{1} accounts searched in {0:0.0} ms (albeit with {2} ms throttling)", sw.ElapsedMilliseconds, userPasswords.Count, ThrottleInterval));
            return true;
        }

        private void ReportWebStatus(int percentage, HttpStatusCode? statusCode, string statusDescription, string itemTitle) {
            switch (statusCode) {
                case null:
                    bwSearchHibp.ReportProgress(percentage, new ProgressState($"Error - {itemTitle}! " + statusDescription));
                    break;

                case HttpStatusCode.OK:
                    bwSearchHibp.ReportProgress(percentage, new ProgressState($"Pwned - {itemTitle}."));
                    break;

                case HttpStatusCode.NotFound:
                    bwSearchHibp.ReportProgress(percentage, new ProgressState($"OK - {itemTitle}."));
                    break;

                case HttpStatusCode.ServiceUnavailable:
                    bwSearchHibp.ReportProgress(percentage, new ProgressState($"Temporarily unavailable - {itemTitle}!"));
                    break;

                case (HttpStatusCode)429:
                    bwSearchHibp.ReportProgress(percentage, new ProgressState($"Throttled - {itemTitle}!"));
                    break;

                default:
                    bwSearchHibp.ReportProgress(percentage, new ProgressState($"{statusCode} {statusDescription} - {itemTitle}!"));
                    break;
            }
        }

        private void bwSearchHibp_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
            if (!staProgress.IsDisposed) {
                staProgress.Visible = false;
                staStatusText.Text = "Completed.";
            }
        }


        #region Helper

        private class ProgressState {
            public ProgressState(ListViewItem item, string statusText) {
                this.Item = item;
                this.StatusText = statusText;
            }
            public ProgressState(ListViewItem item) : this(item, null) { }
            public ProgressState(string statusText) : this(null, statusText) { }

            public ListViewItem Item { get; }
            public string StatusText { get; }
        }

        private class AccountAndEntryStorage {
            public AccountAndEntryStorage(string account, List<Entry> entries) {
                this.Account = account;
                this.Entries = entries.AsReadOnly();
            }
            public string Account { get; }
            public IList<Entry> Entries { get; }
        }

        private class PasswordAndEntryStorage {
            private static Random Random = new Random();
            public PasswordAndEntryStorage(string passwordHash, List<Entry> entries) {
                this.PasswordHash = passwordHash;
                this.Entries = entries.AsReadOnly();
                this.RandomValue = Random.Next();
            }
            public string PasswordHash { get; }
            public IList<Entry> Entries { get; }
            public int RandomValue { get; } //used for sorting so password check is done in different order every time
        }


        private readonly int ThrottleInterval = 1600;

        private static string FilterHtml(string html) {
            var sb = new StringBuilder();
            var isInTag = false;
            foreach (var ch in html) {
                if (isInTag == false) {
                    if (ch == '<') {
                        isInTag = true;
                    } else {
                        sb.Append(ch);
                    }
                } else if (isInTag) {
                    if (ch == '>') { isInTag = false; }
                }
            }
            return sb.ToString();
        }

        #endregion
    }
}
