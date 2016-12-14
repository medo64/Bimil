using System;
using System.Drawing;
using System.Windows.Forms;

namespace Bimil {
    internal partial class PasswordForm : Form {

        public PasswordForm() {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;

            erp.SetIconAlignment(txtPassword, ErrorIconAlignment.MiddleLeft);
            erp.SetIconPadding(txtPassword, SystemInformation.Border3DSize.Width);
        }


        private void chbShowPassword_CheckedChanged(object sender, EventArgs e) {
            txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;
        }

        private void btnOK_Click(object sender, EventArgs e) {
            this.Password = txtPassword.Text;
        }

        public string Password { get; private set; }

        private void tmrCapsLock_Tick(object sender, EventArgs e) {
            var isCapsLock = Control.IsKeyLocked(Keys.CapsLock);
            var wasCapsLock = !string.IsNullOrEmpty(erp.GetError(txtPassword));
            if (isCapsLock && !wasCapsLock) {
                erp.SetError(txtPassword, "CapsLock is on.");
            } else if (!isCapsLock && wasCapsLock) {
                erp.SetError(txtPassword, null);
            }
        }

    }
}
