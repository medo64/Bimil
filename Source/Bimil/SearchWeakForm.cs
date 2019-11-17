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


        private void Form_FormClosing(object sender, FormClosingEventArgs e) {
            if (bwSearchWeak.IsBusy) { bwSearchWeak.CancelAsync(); }
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


        private void bwSearchWeak_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
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

#endregion

    }
}
