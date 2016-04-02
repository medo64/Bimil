using System;
using System.Drawing;
using System.Windows.Forms;

namespace Bimil {
    internal partial class SettingsForm : Form {
        public SettingsForm() {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;
        }

        private void SettingsForm_Load(object sender, EventArgs e) {
            chbShowStart.Checked = Settings.ShowStart;
            chbCloseOnEscape.Checked = Settings.CloseOnEscape;
        }

        private void btnOK_Click(object sender, EventArgs e) {
            Settings.ShowStart = chbShowStart.Checked;
            Settings.CloseOnEscape = chbCloseOnEscape.Checked;
        }

    }
}
