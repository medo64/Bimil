using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Bimil {
    internal partial class SettingsForm : Form {
        public SettingsForm() {
            InitializeComponent();
            Font = SystemFonts.MessageBoxFont;
        }

        private void Form_Load(object sender, EventArgs e) {
            chbShowStart.Checked = Settings.ShowStart;
            chbLoadLast.Checked = Settings.LoadLast;
            chbCloseOnEscape.Checked = Settings.CloseOnEscape;
            chbEditableByDefault.Checked = Settings.EditableByDefault;
            chbShowCommonPasswordWarnings.Checked = Settings.ShowCommonPasswordWarnings;
            chbPasswordSafeWarnings.Checked = Settings.ShowPasswordSafeWarnings;
            chbCheckWeakPasswordAtHibp.Checked = Settings.HibpCheckWeakPassword;
            chbSavePasswordHistoryByDefault.Checked = Settings.SavePasswordHistoryByDefault;

            chbClearClipboardTimeout.Checked = (Settings.AutoClearClipboardTimeout > 0);
            txtClearClipboardTimeout.Text = (Settings.AutoClearClipboardTimeout > 0) ? Settings.AutoClearClipboardTimeout.ToString(CultureInfo.CurrentCulture) : "30";
            chbClearOnlySensitveItems.Enabled = chbClearClipboardTimeout.Checked;
            chbClearOnlySensitveItems.Checked = Settings.AutoClearClipboardForSensitiveDataOnly;

            chbItemTimeout.Checked = (Settings.AutoCloseItemTimeout > 0);
            txtItemTimeout.Text = (Settings.AutoCloseItemTimeout > 0) ? Settings.AutoCloseItemTimeout.ToString(CultureInfo.CurrentCulture) : "120";

            chbAppTimeout.Checked = (Settings.AutoCloseTimeout > 0);
            txtAppTimeout.Text = (Settings.AutoCloseTimeout > 0) ? Settings.AutoCloseTimeout.ToString(CultureInfo.CurrentCulture) : "900";

            chbAutoCloseSave.Checked = Settings.AutoCloseSave;
        }

        private void btnOK_Click(object sender, EventArgs e) {
            Settings.ShowStart = chbShowStart.Checked;
            Settings.LoadLast= chbLoadLast.Checked;
            Settings.CloseOnEscape = chbCloseOnEscape.Checked;
            Settings.EditableByDefault = chbEditableByDefault.Checked;
            Settings.ShowCommonPasswordWarnings = chbShowCommonPasswordWarnings.Checked;
            Settings.ShowPasswordSafeWarnings = chbPasswordSafeWarnings.Checked;
            Settings.HibpCheckWeakPassword = chbCheckWeakPasswordAtHibp.Checked;
            Settings.SavePasswordHistoryByDefault = chbSavePasswordHistoryByDefault.Checked;

            if (chbClearClipboardTimeout.Checked) {
                if (int.TryParse(txtClearClipboardTimeout.Text, NumberStyles.Integer, CultureInfo.CurrentCulture, out var seconds)) {
                    Settings.AutoClearClipboardTimeout = seconds;
                }
            } else {
                Settings.AutoClearClipboardTimeout = 0;
            }
            Settings.AutoClearClipboardForSensitiveDataOnly = chbClearOnlySensitveItems.Checked;

            if (chbItemTimeout.Checked) {
                if (int.TryParse(txtItemTimeout.Text, NumberStyles.Integer, CultureInfo.CurrentCulture, out var seconds)) {
                    Settings.AutoCloseItemTimeout = seconds;
                }
            } else {
                Settings.AutoCloseItemTimeout = 0;
            }

            if (chbAppTimeout.Checked) {
                if (int.TryParse(txtAppTimeout.Text, NumberStyles.Integer, CultureInfo.CurrentCulture, out var seconds)) {
                    Settings.AutoCloseTimeout = seconds;
                }
            } else {
                Settings.AutoCloseTimeout = 0;
            }

            Settings.AutoCloseSave = chbAutoCloseSave.Checked;
        }



        private void chbShowStart_CheckedChanged(object sender, EventArgs e) {
            if (chbShowStart.Checked) { chbLoadLast.Checked = false; }
        }

        private void chbLoadLast_CheckedChanged(object sender, EventArgs e) {
            if (chbLoadLast.Checked) { chbShowStart.Checked = false; }
        }

        private void chbClearClipboardTimeout_CheckedChanged(object sender, EventArgs e) {
            txtClearClipboardTimeout.Enabled = chbClearClipboardTimeout.Checked;
            chbClearOnlySensitveItems.Enabled = chbClearClipboardTimeout.Checked;
        }

        private void chbItemTimeout_CheckedChanged(object sender, EventArgs e) {
            txtItemTimeout.Enabled = chbItemTimeout.Checked;
            chbAutoCloseSave.Enabled = chbAppTimeout.Checked | chbItemTimeout.Checked;
        }

        private void chbAppTimeout_CheckedChanged(object sender, EventArgs e) {
            txtAppTimeout.Enabled = chbAppTimeout.Checked;
            chbAutoCloseSave.Enabled = chbAppTimeout.Checked | chbItemTimeout.Checked;
        }

        private void txtTimeout_KeyDown(object sender, KeyEventArgs e) {
            var textBox = (TextBox)sender;

            switch (e.KeyData) {
                case Keys.D0:
                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                    break;

                case Keys.Left:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.Back:
                case Keys.Delete:
                case Keys.Alt | Keys.F4:
                    break;

                case Keys.PageUp:
                    ChangeTimeout(textBox, -10);
                    e.SuppressKeyPress = true;
                    break;

                case Keys.PageDown:
                    ChangeTimeout(textBox, +10);
                    e.SuppressKeyPress = true;
                    break;

                default:
                    e.SuppressKeyPress = true;
                    break;
            }
        }

        private void ChangeTimeout(TextBox textBox, int delta) {
            if (int.TryParse(textBox.Text, NumberStyles.Integer, CultureInfo.CurrentCulture, out var seconds)) {
                var newSeconds = Math.Min(Math.Max(seconds + delta, 10), 3600);
                textBox.Text = newSeconds.ToString(CultureInfo.CurrentCulture);
                textBox.SelectAll();
            }
        }

        private void txtClearClipboardTimeout_Leave(object sender, EventArgs e) {
            if (!int.TryParse(txtClearClipboardTimeout.Text, NumberStyles.Integer, CultureInfo.CurrentCulture, out var seconds)) {
                seconds = (Settings.AutoClearClipboardTimeout > 0) ? Settings.AutoClearClipboardTimeout : 30;
            }
            txtClearClipboardTimeout.Text = Math.Min(Math.Max(seconds, 10), 3600).ToString(CultureInfo.CurrentCulture);
        }

        private void txtItemTimeout_Leave(object sender, EventArgs e) {
            if (!int.TryParse(txtItemTimeout.Text, NumberStyles.Integer, CultureInfo.CurrentCulture, out var seconds)) {
                seconds = (Settings.AutoCloseItemTimeout > 0) ? Settings.AutoCloseItemTimeout : 120;
            }
            txtItemTimeout.Text = Math.Min(Math.Max(seconds, 10), 3600).ToString(CultureInfo.CurrentCulture);
        }

        private void txtAppTimeout_Leave(object sender, EventArgs e) {
            if (!int.TryParse(txtAppTimeout.Text, NumberStyles.Integer, CultureInfo.CurrentCulture, out var seconds)) {
                seconds = (Settings.AutoCloseTimeout > 0) ? Settings.AutoCloseTimeout : 900;
            }
            txtAppTimeout.Text = Math.Min(Math.Max(seconds, 10), 3600).ToString(CultureInfo.CurrentCulture);
        }

    }
}
