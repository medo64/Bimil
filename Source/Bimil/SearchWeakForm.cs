using System;
using System.Collections.Generic;
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
            Font = SystemFonts.MessageBoxFont;
            Medo.Windows.Forms.State.Attach(this);
            lsvEntries.SmallImageList = Helpers.GetImageList(this, "picWarning", "picError");

            Document = document;
            Categories = categories;

            bwSearchWeak.RunWorkerAsync();
        }

        private readonly Document Document;
        private readonly IList<string> Categories;


        private void Form_Load(object sender, EventArgs e) {
#if !DEBUG
            if (Helpers.IsRunningOnMono) {
                chbIncludeHibp.Visible = false; //don't show HIPB when running on mono (TLS 1.2 not working)
            }
#endif
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e) {
            if (bwSearchWeak.IsBusy) { bwSearchWeak.CancelAsync(); }
            if (bwSearchHibp.IsBusy) { bwSearchHibp.CancelAsync(); }
        }

        private void Form_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyData == Keys.Escape) {
                Close();
            }
        }

        private void Form_Resize(object sender, System.EventArgs e) {
            lsvEntries.Columns[0].Width = lsvEntries.ClientSize.Width;
        }


        private void lsvEntries_ItemActivate(object sender, System.EventArgs e) {
            if ((Document == null) || (lsvEntries.SelectedItems.Count != 1)) { return; }

            var entry = (Entry)(lsvEntries.SelectedItems[0].Tag);
            using (var frm = new ItemForm(Document, entry, Categories, startsAsEditable: Settings.EditableByDefault, hideAutotype: true)) {
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
            var count = Document.Entries.Count;
            for (var i = 0; i < count; i++) {
                var entry = Document.Entries[i];
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

                if (state.EstimatedSecondsRemaining.HasValue) {
                    var secondsLeft = state.EstimatedSecondsRemaining.Value;
                    int value;
                    string valueText;
                    if (secondsLeft > 7200) { //more than two hours; show hours
                        value = secondsLeft / 3600;
                        valueText = (value == 1) ? "hour" : "hours";
                    } else if (secondsLeft > 60) { //more than minute but less than two hours; show minutes
                        value = secondsLeft / 60;
                        valueText = (value == 1) ? "minute" : "minutes";
                    } else { //less then minute; show seconds
                        value = (secondsLeft / 5 + 1) * 5; //round to 5 seconds
                        valueText = (value == 1) ? "second" : "seconds";
                    }
                    staProgress.ToolTipText = $"About {value} {valueText} remaining...";
                } else {
                    staProgress.ToolTipText = "";
                }
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

            if (Settings.HibpCheckWeakPassword && (SearchHibpForPasswords() == false)) {
                e.Cancel = true;
            }
        }

        private bool SearchHibpForPasswords() {
            var sw = Stopwatch.StartNew();

            var sha1 = new SHA1Managed();

            var userEntryDictionary = new Dictionary<string, List<Entry>>();
            foreach (var entry in Document.Entries) {
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
            if (Settings.HibpCheckWeakPasswordInRandomOrder) {
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

            //check all hashes
            var userPasswordCount = userPasswords.Count;
            for (var i = 0; i < userPasswordCount; i++) {
                var percentage = i * 100 / userPasswordCount;
                var item = userPasswords[i];

                try {
                    if (Hibp.IsPassworPwned(item.PasswordHash)) {
                        ReportWebStatus(percentage, HttpStatusCode.OK, null, item.Entries[0].Title, (userPasswordCount - i) * Settings.HibpThrottleInterval);
                        foreach (var entry in item.Entries) {
                            var lvi = new ListViewItem(entry.Title) {
                                Tag = entry,
                                ImageIndex = 1,
                                ToolTipText = $"Password is present in breached password collection at Have I been pwned? site."
                            };
                            bwSearchHibp.ReportProgress(percentage, new ProgressState(lvi));
                        }
                    } else {
                        ReportWebStatus(percentage, HttpStatusCode.NotFound, null, item.Entries[0].Title, (userPasswordCount - i) * Settings.HibpThrottleInterval);
                    }
                } catch (WebException ex) {
                    if (ex.Response is HttpWebResponse response) {
                        ReportWebStatus(percentage, response.StatusCode, response.StatusDescription, item.Entries[0].Title, (userPasswordCount - i) * Settings.HibpThrottleInterval);
                    } else {
                        ReportWebStatus(percentage, null, ex.Message, item.Entries[0].Title, (userPasswordCount - i) * Settings.HibpThrottleInterval);
                    }
                }

                Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Password for {1} entries searched at {0:0.0} ms (followed by {2} ms throttling)", sw.ElapsedMilliseconds, item.Entries.Count, Settings.HibpThrottleInterval));
                Thread.Sleep(Settings.HibpThrottleInterval);
            }

            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "{1} accounts searched in {0:0.0} ms (albeit with {2} ms throttling)", sw.ElapsedMilliseconds, userPasswords.Count, Settings.HibpThrottleInterval));
            return true;
        }

        private void ReportWebStatus(int percentage, HttpStatusCode? statusCode, string statusDescription, string itemTitle, int estimatedMillisecondsRemaining) {
            switch (statusCode) {
                case null:
                    bwSearchHibp.ReportProgress(percentage, new ProgressState($"Error - {itemTitle}! " + statusDescription, estimatedMillisecondsRemaining));
                    break;

                case HttpStatusCode.OK:
                    bwSearchHibp.ReportProgress(percentage, new ProgressState($"Pwned - {itemTitle}.", estimatedMillisecondsRemaining));
                    break;

                case HttpStatusCode.NotFound:
                    bwSearchHibp.ReportProgress(percentage, new ProgressState($"OK - {itemTitle}.", estimatedMillisecondsRemaining));
                    break;

                case HttpStatusCode.ServiceUnavailable:
                    bwSearchHibp.ReportProgress(percentage, new ProgressState($"Temporarily unavailable - {itemTitle}!", estimatedMillisecondsRemaining));
                    break;

                case (HttpStatusCode)429:
                    bwSearchHibp.ReportProgress(percentage, new ProgressState($"Throttled - {itemTitle}!", estimatedMillisecondsRemaining));
                    break;

                default:
                    bwSearchHibp.ReportProgress(percentage, new ProgressState($"{statusCode} {statusDescription} - {itemTitle}!", estimatedMillisecondsRemaining));
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
            public ProgressState(ListViewItem item, string statusText, int? estimatedMillisecondsRemaining) {
                Item = item;
                StatusText = statusText;
                EstimatedSecondsRemaining = (estimatedMillisecondsRemaining != null) ? estimatedMillisecondsRemaining / 1000 : null;
            }
            public ProgressState(ListViewItem item) : this(item, null, null) { }
            public ProgressState(string statusText) : this(null, statusText, null) { }
            public ProgressState(string statusText, int? estimatedMillisecondsRemaining) : this(null, statusText, estimatedMillisecondsRemaining) { }

            public ListViewItem Item { get; }
            public string StatusText { get; }
            public int? EstimatedSecondsRemaining { get; }
        }

        [DebuggerDisplay("{Entries.Count} entries for {Account,nq}")]
        private class AccountAndEntryStorage {
            public AccountAndEntryStorage(string account, List<Entry> entries) {
                Account = account;
                Entries = entries.AsReadOnly();
            }
            public string Account { get; }
            public IList<Entry> Entries { get; }
        }

        [DebuggerDisplay("{Entries.Count} entries for {PasswordHash,nq}")]
        private class PasswordAndEntryStorage {
            private static readonly Random Random = new Random();
            public PasswordAndEntryStorage(string passwordHash, List<Entry> entries) {
                PasswordHash = passwordHash;
                Entries = entries.AsReadOnly();
                RandomValue = Random.Next();
            }
            public string PasswordHash { get; }
            public IList<Entry> Entries { get; }
            public int RandomValue { get; } //used for sorting so password check is done in different order every time
        }


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
