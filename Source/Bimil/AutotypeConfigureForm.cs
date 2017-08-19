using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Medo.Security.Cryptography.PasswordSafe;

namespace Bimil {
    internal partial class AutotypeConfigureForm : Form {
        public AutotypeConfigureForm(Entry entry, bool isReadOnly) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;

            Medo.Windows.Forms.State.SetupOnLoadAndClose(this, lsvItems);

            this.Entry = entry;
            this.IsReadOnly = isReadOnly;

            foreach (var record in entry.Records) {
                if (record.RecordType == RecordType.Autotype) {
                    var item = new ListViewItem(record.Text);
                    lsvItems.Items.Add(item);
                    UpdateItemDescription(item);
                }
            }

            if (isReadOnly) {
                lsvItems.BackColor = SystemColors.Control;
                lsvItems.LabelEdit = false;
                btnAdd.Enabled = false;
                btnRemove.Enabled = false;
                btnEdit.Visible = false;
                btnView.Visible = true;
                btnOK.Visible = false;
                lblEditInfo.Visible = true;
            }
        }

        private readonly Entry Entry;
        private readonly bool IsReadOnly;


        private void lsvItems_ItemActivate(object sender, EventArgs e) {
            btnEdit_Click(null, null);
        }

        private void lsvItems_SelectedIndexChanged(object sender, EventArgs e) {
            btnEdit.Enabled = !this.IsReadOnly && (lsvItems.SelectedItems.Count == 1);
            btnView.Enabled = (lsvItems.SelectedItems.Count == 1);
            btnRemove.Enabled = !this.IsReadOnly && (lsvItems.SelectedItems.Count > 0);
        }

        private void lsvItems_AfterLabelEdit(object sender, LabelEditEventArgs e) {
            if (string.IsNullOrEmpty(e.Label)) {
                e.CancelEdit = true;
            } else {
                lsvItems.Items[e.Item].SubItems[1].Text = UpdateItemDescription(e.Label);
            }
        }

        private void lsvItems_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyData) {
                case Keys.F2:
                    if (!this.IsReadOnly && (lsvItems.SelectedItems.Count == 1)) {
                        lsvItems.SelectedItems[0].BeginEdit();
                    }
                    e.Handled = true;
                    break;

                case Keys.Delete:
                    if (btnRemove.Enabled) { btnRemove.PerformClick(); }
                    break;
            }
        }


        private void btnAdd_Click(object sender, EventArgs e) {
            using (var frm = new AutotypeHelpForm(null, this.IsReadOnly)) {
                if (frm.ShowDialog(this) == DialogResult.OK) {
                    var item = new ListViewItem(frm.AutotypeText);
                    UpdateItemDescription(item);
                    lsvItems.Items.Add(item);
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e) {
            if (lsvItems.SelectedItems.Count != 1) { return; }
            var item = lsvItems.SelectedItems[0];

            using (var frm = new AutotypeHelpForm(item.Text, this.IsReadOnly)) {
                if (frm.ShowDialog(this) == DialogResult.OK) {
                    if (!this.IsReadOnly) {
                        item.Text = frm.AutotypeText;
                        UpdateItemDescription(item);
                    }
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e) {
            if (lsvItems.SelectedItems.Count < 1) { return; }
            for (var i = lsvItems.SelectedIndices.Count - 1; i >= 0; i--) { //loop just in case multi-select is allowed at later time
                lsvItems.Items.RemoveAt(lsvItems.SelectedIndices[i]);
            }
        }


        private void btnOK_Click(object sender, EventArgs e) {
            //remove all old auto-type records
            for (var i = this.Entry.Records.Count - 1; i >= 0; i--) {
                if (this.Entry.Records[i].RecordType == RecordType.Autotype) {
                    this.Entry.Records.RemoveAt(i);
                }
            }

            //add new records
            foreach (ListViewItem item in lsvItems.Items) {
                var record = new Record(RecordType.Autotype) { Text = item.Text };
                this.Entry.Records.Add(record);
            }
        }


        private void UpdateItemDescription(ListViewItem item) {
            if (item.SubItems.Count < 2) {
                item.SubItems.Add(UpdateItemDescription(item.Text));
            } else {
                item.SubItems[1].Text = UpdateItemDescription(item.Text);
            }
        }

        private string UpdateItemDescription(string text) {
            var tokens = AutotypeToken.GetUnexpandedAutotypeTokens(text);
            var sb = new StringBuilder();
            foreach (var token in tokens) {
                if (sb.Length > 0) { sb.Append(" "); }
                sb.Append(token.ToString());
            }
            return sb.ToString();
        }
    }
}
