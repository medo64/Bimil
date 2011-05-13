using System;
using System.Drawing;
using System.Windows.Forms;
using Medo.Security.Cryptography.Bimil;

namespace Bimil {
    public partial class NewRecordForm : Form {

        private readonly BimilDocument Document;

        public NewRecordForm(BimilDocument document) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;

            this.Document = document;
        }

        private void NewRecordForm_Load(object sender, EventArgs e) {
            cmbRecordType.Items.Add(new BimilFormatWrapper("Text", BimilRecordFormat.Text));
            cmbRecordType.Items.Add(new BimilFormatWrapper("Password", BimilRecordFormat.Password));
            cmbRecordType.Items.Add(new BimilFormatWrapper("URL", BimilRecordFormat.Url));
            cmbRecordType.Items.Add(new BimilFormatWrapper("Multiline text", BimilRecordFormat.MultilineText));
            cmbRecordType.SelectedIndex = 0;
        }


        public BimilRecord Record { get; private set; }

        private void btnOK_Click(object sender, EventArgs e) {
            this.Record = new BimilRecord(this.Document, txtName.Text, "", ((BimilFormatWrapper)cmbRecordType.SelectedItem).Format);
        }


        #region Private

        private class BimilFormatWrapper {
            public BimilFormatWrapper(string text, BimilRecordFormat format) {
                this.Text = text;
                this.Format = format;
            }
            public string Text { get; private set; }
            public BimilRecordFormat Format { get; private set; }
            public override string ToString() {
                return this.Text;
            }
        }

        #endregion

    }
}
