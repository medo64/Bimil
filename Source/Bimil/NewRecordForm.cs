using System;
using System.Drawing;
using System.Windows.Forms;
using Medo.Security.Cryptography.PasswordSafe;

namespace Bimil {
    public partial class NewRecordForm : Form {

        private readonly Document Document;

        public NewRecordForm(Document document) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;

            this.Document = document;
        }

        private void Form_Load(object sender, EventArgs e) {
            foreach (RecordType recordType in Enum.GetValues(typeof(RecordType))) {
                var caption = Helpers.GetRecordCaption(recordType);
                if (caption != null) {
                    cmbRecordType.Items.Add(new BimilFormatWrapper(caption, recordType));
                }
            }
            if (this.Record != null) {
                cmbRecordType.SelectedItem = this.Record.RecordType;
            } else {
                cmbRecordType.SelectedIndex = 0;
            }
        }


        public Record Record { get; private set; }

        private void btnOK_Click(object sender, EventArgs e) {
            this.Record = new Record(((BimilFormatWrapper)cmbRecordType.SelectedItem).Format);
        }


        #region Private

        private class BimilFormatWrapper {
            public BimilFormatWrapper(string text, RecordType format) {
                this.Text = text;
                this.Format = format;
            }
            public string Text { get; private set; }
            public RecordType Format { get; private set; }
            public override bool Equals(object obj) {
                if (obj is RecordType) {
                    var otherFormat = (RecordType)obj;
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
