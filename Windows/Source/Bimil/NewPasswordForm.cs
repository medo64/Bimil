using System;
using System.Drawing;
using System.Windows.Forms;

namespace Bimil {
    internal partial class NewPasswordForm : Form {

        public NewPasswordForm() {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;
        }


        private void txtPassword_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyData == Keys.Enter) {
                txtPassword2.Select();
            }
        }

        private void txtPassword2_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyData == Keys.Enter) {
                if (btnOK.Enabled) {
                    btnOK_Click(null, null);
                    this.DialogResult = DialogResult.OK;
                }
            }
        }

        private void txtPassword_TextChanged(object sender, EventArgs e) {
            btnOK.Enabled = string.Equals(txtPassword.Text, txtPassword2.Text, StringComparison.Ordinal);
            this.AcceptButton = btnOK.Enabled ? btnOK : null;
        }


        private void btnOK_Click(object sender, EventArgs e) {
            this.Password = txtPassword.Text;
        }

        public string Password { get; private set; }

    }
}
