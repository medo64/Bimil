using System;
using System.Drawing;
using System.Windows.Forms;

namespace Bimil {
    internal partial class PasswordForm : Form {

        public PasswordForm() {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;
        }


        private void btnOK_Click(object sender, EventArgs e) {
            this.Password = txtPassword.Text;
        }

        public string Password { get; private set; }

    }
}
