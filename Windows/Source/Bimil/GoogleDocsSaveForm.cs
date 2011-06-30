using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Bimil {
    public partial class GoogleDocsSaveForm : Form {
        public GoogleDocsSaveForm(string authorizationToken) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;

            this.AuthorizationToken = authorizationToken;
        }

        public string AuthorizationToken { get; private set; }


        private void GoogleDocsSaveForm_Load(object sender, EventArgs e) {
            bwFill.RunWorkerAsync();
        }


        private void bwFill_DoWork(object sender, DoWorkEventArgs e) {
            using (var gdata = new Medo.Web.GData.GDataDocs(this.AuthorizationToken)) {
                var xxx = gdata.GetRawFeedContent();
            }
        }

        private void bwFill_ProgressChanged(object sender, ProgressChangedEventArgs e) {

        }

        private void bwFill_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {

        }
    }
}
