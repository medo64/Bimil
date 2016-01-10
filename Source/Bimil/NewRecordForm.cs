using System;
using System.Drawing;
using System.Windows.Forms;
using Medo.Security.Cryptography.Bimil;

namespace Bimil {
    public partial class NewRecordForm : Form {

        private readonly BimilDocument Document;

        public NewRecordForm(BimilDocument document, BimilRecord record) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;

            this.Document = document;
            this.Record = record;
        }

        private void Form_Load(object sender, EventArgs e) {
            if (this.Record != null) {
                txtName.Text = this.Record.Key.Text;
            }
            cmbRecordType.Items.Add(new BimilFormatWrapper("Text", BimilRecordFormat.Text));
            cmbRecordType.Items.Add(new BimilFormatWrapper("Password", BimilRecordFormat.Password));
            cmbRecordType.Items.Add(new BimilFormatWrapper("Monospaced text", BimilRecordFormat.MonospacedText));
            cmbRecordType.Items.Add(new BimilFormatWrapper("Multiline text", BimilRecordFormat.MultilineText));
            cmbRecordType.Items.Add(new BimilFormatWrapper("URL", BimilRecordFormat.Url));
            if (this.Record != null) {
                cmbRecordType.SelectedItem = this.Record.Format;
            } else {
                cmbRecordType.SelectedIndex = 0;
            }
        }

        private void Form_Activated(object sender, EventArgs e) {
            if (this.Record != null) {
                txtName.SelectAll();
            }
        }


        public BimilRecord Record { get; private set; }

        private void btnOK_Click(object sender, EventArgs e) {
            if (this.Record != null) {
                this.Record = new BimilRecord(this.Document, txtName.Text, this.Record.Value.Text, ((BimilFormatWrapper)cmbRecordType.SelectedItem).Format);
            } else {
                this.Record = new BimilRecord(this.Document, txtName.Text, "", ((BimilFormatWrapper)cmbRecordType.SelectedItem).Format);
            }
        }


        #region Private

        private class BimilFormatWrapper {
            public BimilFormatWrapper(string text, BimilRecordFormat format) {
                this.Text = text;
                this.Format = format;
            }
            public string Text { get; private set; }
            public BimilRecordFormat Format { get; private set; }
            public override bool Equals(object obj) {
                if (obj is BimilRecordFormat) {
                    var otherFormat = (BimilRecordFormat)obj;
                    return (this.Format == otherFormat);
                }
                return base.Equals(obj);
            }
            public override int GetHashCode() {
                return this.Format.GetHashCode();
            }
            public override string ToString() {
                return this.Text;
            }
        }

        #endregion

    }
}
