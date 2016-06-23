using System;
using System.Drawing;
using System.Windows.Forms;

namespace Bimil {
    internal partial class SettingsForm : Form {
        public SettingsForm() {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;
        }

        private void Form_Load(object sender, EventArgs e) {
            chbShowStart.Checked = Settings.ShowStart;
            chbCloseOnEscape.Checked = Settings.CloseOnEscape;

            chbItemTimeout.Checked = (Settings.AutoCloseItemTimeout > 0);
            if (Settings.AutoCloseItemTimeout > 0) { nudItemTimeout.Value = Settings.AutoCloseItemTimeout; }

            chbAppTimeout.Checked = (Settings.AutoCloseTimeout > 0);
            if (Settings.AutoCloseTimeout > 0) { nudAppTimeout.Value = Settings.AutoCloseTimeout; }

            chbPasswordSafeWarnings.Checked = Settings.ShowPasswordSafeWarnings;
        }

        private void btnOK_Click(object sender, EventArgs e) {
            Settings.ShowStart = chbShowStart.Checked;
            Settings.CloseOnEscape = chbCloseOnEscape.Checked;
            Settings.AutoCloseItemTimeout = chbItemTimeout.Checked ? (int)nudItemTimeout.Value : 0;
            Settings.AutoCloseTimeout = chbAppTimeout.Checked ? (int)nudAppTimeout.Value : 0;
            Settings.ShowPasswordSafeWarnings = chbPasswordSafeWarnings.Checked;
        }

        private void chbItemTimeout_CheckedChanged(object sender, EventArgs e) {
            nudItemTimeout.Enabled = chbItemTimeout.Checked;
        }

        private void chbAppTimeout_CheckedChanged(object sender, EventArgs e) {
            nudAppTimeout.Enabled = chbAppTimeout.Checked;
        }

    }
}
