using System;
using System.Drawing;
using System.Windows.Forms;
using Medo.Security.Cryptography.PasswordSafe;
using System.Collections.Generic;

namespace Bimil {
    internal partial class NewRecordForm : Form {
        public NewRecordForm(Document document, IEnumerable<Record> recordsInUse) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;

            erp.SetIconAlignment(cmbRecordType, ErrorIconAlignment.MiddleLeft);
            erp.SetIconPadding(cmbRecordType, cmbRecordType.Height / 6);

            this.Document = document;
            this.RecordsInUse = recordsInUse;
        }

        private readonly Document Document;
        private readonly IEnumerable<Record> RecordsInUse;


        private void Form_Load(object sender, EventArgs e) {
            foreach (RecordType recordType in Helpers.GetUsableRecordTypes()) {
                cmbRecordType.Items.Add(new BimilFormatWrapper(Helpers.GetRecordCaption(recordType), recordType));
            }

            if (this.Record != null) {
                cmbRecordType.SelectedItem = this.Record.RecordType;
            } else {
                foreach (BimilFormatWrapper item in cmbRecordType.Items) {
                    var isAlreadyUsed = false;
                    foreach (var record in this.RecordsInUse) {
                        if (item.Format == record.RecordType) {
                            isAlreadyUsed = true;
                            break;
                        }
                    }
                    if (isAlreadyUsed == false) {
                        cmbRecordType.SelectedItem = item;
                        break;
                    }
                }
                if (cmbRecordType.SelectedItem == null) {
                    cmbRecordType.SelectedIndex = 0;
                }
            }
        }


        public Record Record { get; private set; }

        private void btnOK_Click(object sender, EventArgs e) {
            this.Record = new Record(((BimilFormatWrapper)cmbRecordType.SelectedItem).Format);
            if (this.Record.RecordType == RecordType.PasswordHistory) { this.Record.Text = "10300"; } //enable by default
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

        private void cmbRecordType_SelectedIndexChanged(object sender, EventArgs e) {
            if (!Settings.ShowPasswordSafeWarnings) { return; }

            var wrap = (BimilFormatWrapper)cmbRecordType.SelectedItem;
            erp.SetError(cmbRecordType, null);

            switch (wrap.Format) {
                case RecordType.TwoFactorKey:
                case RecordType.CreditCardNumber:
                case RecordType.CreditCardExpiration:
                case RecordType.CreditCardVerificationValue:
                case RecordType.CreditCardPin:
                case RecordType.QRCode:
                    erp.SetError(cmbRecordType, "This record type is not supported by PasswordSafe.");
                    break;

                default:
                    var recordTypeCount = 0;
                    foreach (var item in this.RecordsInUse) {
                        if (item.RecordType == wrap.Format) { recordTypeCount++; }
                    }
                    if (recordTypeCount >= 1) {
                        erp.SetError(cmbRecordType, "Repeating record type is not supported by PasswordSafe.");
                    }
                    break;
            }
        }

    }
}
