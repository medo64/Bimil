using Medo.Security.Cryptography.PasswordSafe;
using System.Windows.Forms;

namespace Bimil {
    internal partial class PasswordDetailsForm : Form {
        public PasswordDetailsForm(Entry entry, bool isReadonly) {
            InitializeComponent();

            this.Entry = entry;
            this.IsReadonly = isReadonly;
            this.IsEnabled = entry.Records.Contains(RecordType.PasswordHistory);

            if (this.IsEnabled) {
                nudHistoryCount.Value = entry.PasswordHistory.MaximumCount;
                foreach (var password in entry.PasswordHistory) {
                    var lvi = new ListViewItem(password.TimeFirstUsed.ToShortDateString()) { Tag = password };
                    lvi.SubItems.Add("***");
                    lsvHistoryPasswords.Items.Add(lvi);
                    lvi.ToolTipText = $"{password.TimeFirstUsed.ToLongDateString()}  {password.TimeFirstUsed.ToShortTimeString()}\n{password.HistoricalPassword}";
                }

                chbHistoryEnabled.Checked = entry.PasswordHistory.Enabled;
            }
            SetupHistoryStates();

            btnOK.Visible = !this.IsReadonly;
            lblEditInfo.Visible = this.IsReadonly;
        }

        private readonly Entry Entry;
        private readonly bool IsReadonly;
        private readonly bool IsEnabled;
        private bool IsHidden;


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
                item.SubItems[1].Text = ((PasswordHistoryItem)(item.Tag)).HistoricalPassword;
            }
            btnHistoryShow.Enabled = false;
            this.IsHidden = true;
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

            var item = (PasswordHistoryItem)(lsvHistoryPasswords.SelectedItems[0].Tag);
            Clipboard.Clear();
            Clipboard.SetText(item.HistoricalPassword);
        }


        private void SetupHistoryStates() {
            chbHistoryEnabled.Enabled = !this.IsReadonly;
            lblHistoryCount.Enabled = chbHistoryEnabled.Checked && !this.IsReadonly;
            nudHistoryCount.Enabled = chbHistoryEnabled.Checked && !this.IsReadonly;
            btnHistoryClean.Enabled = chbHistoryEnabled.Checked && !this.IsReadonly;
            btnHistoryShow.Enabled = this.IsEnabled && !this.IsHidden;
            lsvHistoryPasswords.Enabled = this.IsEnabled;
        }


        private void btnHistoryClean_Click(object sender, System.EventArgs e) {
            lsvHistoryPasswords.Items.Clear();
        }

        private void btnOK_Click(object sender, System.EventArgs e) {
            if (this.IsEnabled && chbHistoryEnabled.Checked) { //it was enabled and it is still

                var changedMaxCount = (nudHistoryCount.Value != this.Entry.PasswordHistory.MaximumCount);
                var wasCleared = (this.Entry.PasswordHistory.Count > 0) && (lsvHistoryPasswords.Items.Count == 0);
                if (changedMaxCount) { this.Entry.PasswordHistory.MaximumCount = (int)nudHistoryCount.Value; }
                if (wasCleared) { this.Entry.PasswordHistory.Clear(); }

            } else if (this.IsEnabled && !chbHistoryEnabled.Checked) { //was enabled - ought to be disabled now

                this.Entry.Records.Remove(this.Entry.Records[RecordType.PasswordHistory]);

            } else if (!this.IsEnabled && chbHistoryEnabled.Checked) { //was not enabled - ought to be enabled now

                this.Entry.PasswordHistory.Enabled = true;
                this.Entry.PasswordHistory.MaximumCount = (int)nudHistoryCount.Value;

            }
        }
    }
}
