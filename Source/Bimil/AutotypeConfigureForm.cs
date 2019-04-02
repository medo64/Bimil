using System;
using System.Drawing;
using System.Windows.Forms;
using Medo.Security.Cryptography.PasswordSafe;

namespace Bimil {
    internal partial class AutotypeConfigureForm : Form {
        public AutotypeConfigureForm(Entry entry, bool isReadOnly) {
            InitializeComponent();
            Font = SystemFonts.MessageBoxFont;

            Medo.Windows.Forms.State.Attach(this, lsvItems);

            Entry = entry;
            IsReadOnly = isReadOnly;

            foreach (var record in entry.Records) {
                if (record.RecordType == RecordType.Autotype) {
                    var item = new ListViewItem(record.Text);
                    lsvItems.Items.Add(item);
                    UpdateItemDescription(item);
                }
            }

            if (isReadOnly) {
                lsvItems.AllowDrop = false;
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
            btnEdit.Enabled = !IsReadOnly && (lsvItems.SelectedItems.Count == 1);
            btnView.Enabled = (lsvItems.SelectedItems.Count == 1);
            btnRemove.Enabled = !IsReadOnly && (lsvItems.SelectedItems.Count > 0);
        }

        private void lsvItems_AfterLabelEdit(object sender, LabelEditEventArgs e) {
            if (string.IsNullOrEmpty(e.Label)) {
                e.CancelEdit = true;
            } else {
                lsvItems.Items[e.Item].SubItems[1].Text = Helpers.GetAutotypeDescription(e.Label);
            }
        }

        private void lsvItems_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyData) {
                case Keys.Insert:
                    if (btnAdd.Enabled) { btnAdd.PerformClick(); }
                    break;

                case Keys.F2:
                    if (!IsReadOnly && (lsvItems.SelectedItems.Count == 1)) {
                        lsvItems.SelectedItems[0].BeginEdit();
                    }
                    e.Handled = true;
                    break;

                case Keys.Delete:
                    if (btnRemove.Enabled) { btnRemove.PerformClick(); }
                    break;
            }
        }

        #region Drag&drop

        private void lsvItems_ItemDrag(object sender, ItemDragEventArgs e) {
            lsvItems.DoDragDrop(lsvItems.SelectedItems, DragDropEffects.Move);
        }

        private void lsvItems_DragEnter(object sender, DragEventArgs e) {
            for (var i = 0; i < e.Data.GetFormats().Length; i++) {
                if (e.Data.GetFormats()[i].Equals("System.Windows.Forms.ListView+SelectedListViewItemCollection")) {
                    e.Effect = DragDropEffects.Move;
                    break;
                }
            }
        }

        private void lsvItems_DragDrop(object sender, DragEventArgs e) {
            if (lsvItems.SelectedItems.Count == 0) { return; }

            var cp = lsvItems.PointToClient(new Point(e.X, e.Y));
            var dragToItem = lsvItems.GetItemAt(cp.X, cp.Y);
            if (dragToItem == null) { return; }

            var dragIndex = dragToItem.Index;
            var sel = new ListViewItem[lsvItems.SelectedItems.Count];
            for (var i = 0; i < lsvItems.SelectedItems.Count; i++) {
                sel[i] = lsvItems.SelectedItems[i];
            }

            for (var i = 0; i < sel.Length; i++) {
                var dragItem = sel[i];
                var itemIndex = dragIndex;
                if (itemIndex == dragItem.Index) { return; }

                if (dragItem.Index < itemIndex) {
                    itemIndex++;
                } else {
                    itemIndex = dragIndex + i;
                }

                var insertItem = (ListViewItem)dragItem.Clone();
                lsvItems.Items.Insert(itemIndex, insertItem);
                lsvItems.Items.Remove(dragItem);
            }
        }

        #endregion Drag&drop


        private void btnAdd_Click(object sender, EventArgs e) {
            using (var frm = new AutotypeHelpForm(null, IsReadOnly)) {
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

            using (var frm = new AutotypeHelpForm(item.Text, IsReadOnly)) {
                if (frm.ShowDialog(this) == DialogResult.OK) {
                    if (!IsReadOnly) {
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
            for (var i = Entry.Records.Count - 1; i >= 0; i--) {
                if (Entry.Records[i].RecordType == RecordType.Autotype) {
                    Entry.Records.RemoveAt(i);
                }
            }

            //add new records
            foreach (ListViewItem item in lsvItems.Items) {
                var record = new Record(RecordType.Autotype) { Text = item.Text };
                Entry.Records.Add(record);
            }
        }


        private void UpdateItemDescription(ListViewItem item) {
            if (item.SubItems.Count < 2) {
                item.SubItems.Add(Helpers.GetAutotypeDescription(item.Text));
            } else {
                item.SubItems[1].Text = Helpers.GetAutotypeDescription(item.Text);
            }
        }
    }
}
