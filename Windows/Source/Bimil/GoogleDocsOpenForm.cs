using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Medo.Web.GData;

namespace Bimil {
    public partial class GoogleDocsOpenForm : Form {
        public GoogleDocsOpenForm(string authorizationToken) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;

            this.AuthorizationToken = authorizationToken;
        }

        public string AuthorizationToken { get; private set; }
        public byte[] ContentBytes { get; private set; }


        private void GoogleDocsSaveForm_Load(object sender, EventArgs e) {
            bwFill.RunWorkerAsync();
            this.Cursor = Cursors.WaitCursor;
        }

        private void GoogleDocsOpenForm_Resize(object sender, EventArgs e) {
            lsvDocuments.Columns[0].Width = lsvDocuments.ClientRectangle.Width - SystemInformation.VerticalScrollBarWidth;
        }

        private void GoogleDocsOpenForm_FormClosing(object sender, FormClosingEventArgs e) {
            if (bwFill.IsBusy) { bwFill.CancelAsync(); }
            if (bwOpen.IsBusy) { bwOpen.CancelAsync(); }
        }


        private void lsvDocuments_SelectedIndexChanged(object sender, EventArgs e) {
            btnOK.Enabled = (lsvDocuments.SelectedItems.Count == 1);
        }

        private void lsvDocuments_ItemActivate(object sender, EventArgs e) {
            if (lsvDocuments.SelectedItems.Count == 1) {
                btnOK_Click(null, null);
            }
        }


        private void bwFill_DoWork(object sender, DoWorkEventArgs e) {
            var list = new List<GDataDocsEntry>();
            using (var gdata = new Medo.Web.GData.GDataDocs(this.AuthorizationToken)) {
                foreach (var entry in gdata.GetEntries()) {
                    if (bwFill.CancellationPending) { e.Cancel = true; return; }
                    if (entry.ContentType.Equals("application/octet-stream", StringComparison.OrdinalIgnoreCase)) {
                        list.Add(entry);
                    }
                }
            }
            e.Result = list;
        }

        private void bwFill_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (e.Cancelled == false) {
                var list = (List<GDataDocsEntry>)e.Result;
                foreach (var entry in list) {
                    var lvi = new ListViewItem() { Tag = entry };
                    lvi.Text = entry.Title;
                    lsvDocuments.Items.Add(lvi);
                }
                if (lsvDocuments.Items.Count > 0) {
                    lsvDocuments.Items[0].Selected = true;
                    lsvDocuments.Items[0].Focused = true;
                }
            }
            this.Cursor = Cursors.Default;
        }


        private void btnOK_Click(object sender, EventArgs e) {
            if (lsvDocuments.SelectedItems.Count == 1) {
                btnOK.Enabled = false;
                bwOpen.RunWorkerAsync(lsvDocuments.SelectedItems[0].Tag);
            }
        }

        private void bwOpen_DoWork(object sender, DoWorkEventArgs e) {
            var entry = (GDataDocsEntry)e.Argument;
            this.ContentBytes = entry.GetContent();
        }

        private void bwOpen_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            this.DialogResult = (e.Cancelled) ? DialogResult.Cancel : DialogResult.OK;
        }

    }
}
