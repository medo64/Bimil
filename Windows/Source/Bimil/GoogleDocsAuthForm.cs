using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Bimil {
    internal partial class GoogleDocsAuthForm : Form {
        public GoogleDocsAuthForm() {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;

            txtEmail.Text = Settings.LastGoogleEmail;
        }

        public string AuthorizationToken { get; private set; }


        private void GoogleDocsAuthForm_Shown(object sender, EventArgs e) {
            if (txtEmail.TextLength == 0) {
                txtEmail.Select();
            } else if (txtPassword.TextLength == 0) {
                txtPassword.Select();
            } else {
                btnOK.Select();
            }
        }

        private void GoogleDocsAuthForm_FormClosing(object sender, FormClosingEventArgs e) {
            if (bwLogin.IsBusy) { bwLogin.CancelAsync(); }
        }


        private void btnOK_Click(object sender, EventArgs e) {
            txtEmail.Enabled = false;
            txtPassword.Enabled = false;
            btnOK.Enabled = false;
            this.Cursor = Cursors.WaitCursor;
            bwLogin.RunWorkerAsync(new string[] { txtEmail.Text, txtPassword.Text });
        }

        private void bwLogin_DoWork(object sender, DoWorkEventArgs e) {
            var args = (string[])e.Argument;
            var userName = args[0];
            var password = args[1];

            using (var login = new Medo.Web.GData.GDataClientLogin(userName, password)) {
                try {
                    e.Result = login.GetDocumentsListDataAuthorizationToken();
                } catch (InvalidOperationException) {
                    e.Result = null;
                }
            }
        }

        private void bwLogin_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            this.Cursor = Cursors.Default;
            if (e.Result != null) {
                Settings.LastGoogleEmail = txtEmail.Text;
                this.AuthorizationToken = (string)e.Result;
                this.DialogResult = DialogResult.OK;
            } else {
                Medo.MessageBox.ShowWarning(this, "Login failed.");
                txtEmail.Enabled = true;
                txtPassword.Enabled = true;
                btnOK.Enabled = true;
                txtPassword.Select();
            }
        }

    }
}
