using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Medo.Security.Cryptography.PasswordSafe;

namespace Bimil {
    internal partial class PasswordDetailsForm : Form {
        public PasswordDetailsForm(Entry entry, bool isReadonly) {
            InitializeComponent();

            Entry = entry;
            IsReadonly = isReadonly;
            IsEnabled = entry.Records.Contains(RecordType.PasswordHistory);

            nudHistoryCount.Value = Settings.SavePasswordHistoryDefaultCount;

            var listItems = new Stack<ListViewItem>(); //to get in the reverse

            if (IsEnabled) {
                nudHistoryCount.Value = entry.PasswordHistory.MaximumCount;
                foreach (var password in entry.PasswordHistory) {
                    listItems.Push(NewHistoricalPasswordListViewItem(password.HistoricalPassword, password.TimeFirstUsed));
                }
                chbHistoryEnabled.Checked = entry.PasswordHistory.Enabled;
            }

            foreach (var record in entry.Records) {
                if (record.RecordType == RecordType.Password) {
                    listItems.Push(NewHistoricalPasswordListViewItem(record.Text, entry.PasswordModificationTime, isCurrent: true)); //use the same modification time for all passwords :( - only one PasswordModificationTime record is here
                }
            }

            lsvHistoryPasswords.BeginUpdate();
            foreach (var item in listItems) {
                lsvHistoryPasswords.Items.Add(item);
            }
            lsvHistoryPasswords.EndUpdate();

            SetupHistoryStates();

            btnOK.Visible = !IsReadonly;
            lblEditInfo.Visible = IsReadonly;
        }

        private ListViewItem NewHistoricalPasswordListViewItem(string password, DateTime utcTime, bool isCurrent = false) {
            var localTime = utcTime.ToLocalTime();
            var lvi = new ListViewItem() { Tag = password };
            lvi.Text = (utcTime > DateTime.MinValue) ? localTime.ToShortDateString() : "unknown";
            lvi.SubItems.Add("***");
            lvi.ToolTipText = $"{localTime:d} {localTime:t}" + (isCurrent ? " (current)" : "") + $"\n{password}";
            if (isCurrent) { lvi.Font = new Font(lvi.Font, FontStyle.Bold); }
            return lvi;
        }


        private readonly Entry Entry;
        private readonly bool IsReadonly;
        private readonly bool IsEnabled;
        private bool IsHidden;


        #region Disable minimize

        protected override void WndProc(ref Message m) {
            if ((Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major < 10)
                && (m != null) && (m.Msg == NativeMethods.WM_SYSCOMMAND) && (m.WParam == NativeMethods.SC_MINIMIZE)) {
                m.Result = IntPtr.Zero;
            } else if (m != null) {
                base.WndProc(ref m);
            }
        }


        private class NativeMethods {
            internal const int WM_SYSCOMMAND = 0x0112;
            internal static readonly IntPtr SC_MINIMIZE = new IntPtr(0xF020);
        }

        #endregion


        private void Form_Load(object sender, System.EventArgs e) {
            Form_Resize(null, null);
        }

        private void Form_Resize(object sender, System.EventArgs e) {
            lsvHistoryPasswords_Password.Width = lsvHistoryPasswords.ClientRectangle.Width - lsvHistoryPasswords_Date.Width - SystemInformation.VerticalScrollBarWidth;
        }


        private void chbHistoryEnabled_CheckedChanged(object sender, System.EventArgs e) {
            SetupHistoryStates();
        }

        private void btnHistoryShow_Click(object sender, System.EventArgs e) {
            foreach (ListViewItem item in lsvHistoryPasswords.Items) {
                item.SubItems[1].Text = (string)(item.Tag);
            }
            btnHistoryShow.Enabled = false;
            IsHidden = true;
        }

        private void lsvHistoryPasswords_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e) {
            e.Cancel = true;
            e.NewWidth = lsvHistoryPasswords.Columns[e.ColumnIndex].Width;
        }

        private void mnxHistoricalPassword_Opening(object sender, System.ComponentModel.CancelEventArgs e) {
            mnxHistoricalPasswordCopy.Enabled = (lsvHistoryPasswords.SelectedItems.Count == 1);
        }

        private void mnxHistoricalPasswordCopy_Click(object sender, System.EventArgs e) {
            if (lsvHistoryPasswords.SelectedItems.Count != 1) { return; }

            var password = (string)(lsvHistoryPasswords.SelectedItems[0].Tag);
            ClipboardHelper.SetClipboardText(this, password, sensitiveData: true);
        }


        private void SetupHistoryStates() {
            chbHistoryEnabled.Enabled = !IsReadonly;
            lblHistoryCount.Enabled = chbHistoryEnabled.Checked && !IsReadonly;
            nudHistoryCount.Enabled = chbHistoryEnabled.Checked && !IsReadonly;
            btnHistoryClean.Enabled = chbHistoryEnabled.Checked && !IsReadonly;
            btnHistoryShow.Enabled = IsEnabled && !IsHidden;
            lsvHistoryPasswords.Enabled = IsEnabled;
        }


        private void btnHistoryClean_Click(object sender, System.EventArgs e) {
            lsvHistoryPasswords.Items.Clear();
        }

        private void btnOK_Click(object sender, System.EventArgs e) {
            if (IsEnabled && chbHistoryEnabled.Checked) { //it was enabled and it is still

                var changedMaxCount = (nudHistoryCount.Value != Entry.PasswordHistory.MaximumCount);
                var wasCleared = (Entry.PasswordHistory.Count > 0) && (lsvHistoryPasswords.Items.Count == 0);
                if (changedMaxCount) { Entry.PasswordHistory.MaximumCount = (int)nudHistoryCount.Value; }
                if (wasCleared) { Entry.PasswordHistory.Clear(); }

            } else if (IsEnabled && !chbHistoryEnabled.Checked) { //was enabled - ought to be disabled now

                Entry.Records.Remove(Entry.Records[RecordType.PasswordHistory]);

            } else if (!IsEnabled && chbHistoryEnabled.Checked) { //was not enabled - ought to be enabled now

                Entry.PasswordHistory.Enabled = true;
                Entry.PasswordHistory.MaximumCount = (int)nudHistoryCount.Value;

            }
        }
    }
}
