using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Medo.Security.Cryptography.Bimil;

namespace Bimil {
    public partial class FieldsEditorForm : Form {

        private readonly BimilDocument Document;
        private readonly BimilItem Item;

        public FieldsEditorForm(BimilDocument document, BimilItem item) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;

            this.Document = document;
            this.Item = item;
        }

        private void Form_Load(object sender, EventArgs e) {
            lsvFields.Columns[0].Width = lsvFields.Width - SystemInformation.VerticalScrollBarWidth;

            foreach (var record in this.Item.Records) {
                if (record.Format != BimilRecordFormat.System) {
                    var lvi = new ListViewItem(record.Key.Text) { Tag = record };
                    lsvFields.Items.Add(lvi);
                }
            }
        }

        private void lsvFields_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyData) {
                case Keys.Insert: {
                        btnAdd_Click(null, null);
                    } break;
                case Keys.F2: {
                    lsvFields_ItemActivate(null, null);
                    } break;
                case Keys.Delete: {
                        btnRemove_Click(null, null);
                    } break;
            }
        }

        private void lsvFields_ItemActivate(object sender, EventArgs e) {
            if (lsvFields.FocusedItem != null) {
                var record = (BimilRecord)lsvFields.FocusedItem.Tag;
                using (var frm = new NewRecordForm(this.Document, record)) {
                    if (frm.ShowDialog(this) == DialogResult.OK) {
                        lsvFields.FocusedItem.Tag = frm.Record;
                        lsvFields.FocusedItem.Text = frm.Record.Key.Text;
                    }
                }
            }
        }

        private void lsvFields_SelectedIndexChanged(object sender, EventArgs e) {
            btnRemove.Enabled = (lsvFields.SelectedItems.Count > 0);
        }

        private void lsvFields_ItemDrag(object sender, ItemDragEventArgs e) {
            lsvFields.DoDragDrop(lsvFields.SelectedItems, DragDropEffects.Move);
        }

        private void lsvFields_DragEnter(object sender, DragEventArgs e) {
            for (int i = 0; i < e.Data.GetFormats().Length; i++) {
                if (e.Data.GetFormats()[i].Equals("System.Windows.Forms.ListView+SelectedListViewItemCollection")) {
                    e.Effect = DragDropEffects.Move;
                    break;
                }
            }
        }

        private void lsvFields_DragDrop(object sender, DragEventArgs e) {
            if (lsvFields.SelectedItems.Count == 0) { return; }

            Point cp = lsvFields.PointToClient(new Point(e.X, e.Y));
            ListViewItem dragToItem = lsvFields.GetItemAt(cp.X, cp.Y);
            if (dragToItem == null) { return; }

            int dragIndex = dragToItem.Index;
            var sel = new ListViewItem[lsvFields.SelectedItems.Count];
            for (int i = 0; i < lsvFields.SelectedItems.Count; i++) {
                sel[i] = lsvFields.SelectedItems[i];
            }

            ListViewItem insertItem = null;
            for (int i = 0; i < sel.Length; i++) {
                var dragItem = sel[i];
                int itemIndex = dragIndex;
                if (itemIndex == dragItem.Index) { return; }

                if (dragItem.Index < itemIndex) {
                    itemIndex++;
                } else {
                    itemIndex = dragIndex + i;
                }

                insertItem = (ListViewItem)dragItem.Clone();
                lsvFields.Items.Insert(itemIndex, insertItem);
                lsvFields.Items.Remove(dragItem);
            }
        }


        private void btnAdd_Click(object sender, EventArgs e) {
            using (var frm = new NewRecordForm(this.Document, null)) {
                if (frm.ShowDialog(this) == DialogResult.OK) {
                    var lvi = new ListViewItem(frm.Record.Key.Text) { Tag = frm.Record };
                    lsvFields.Items.Add(lvi);
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e) {
            if (lsvFields.SelectedItems.Count == 1) {
                lsvFields.Items.RemoveAt(lsvFields.SelectedItems[0].Index);
            }
        }

        private void btnOK_Click(object sender, EventArgs e) {
            this.Item.ClearNonSystemRecords();
            foreach (ListViewItem lvi in lsvFields.Items) {
                var record = (BimilRecord)lvi.Tag;
                record.Key.Text = lvi.Text;
                this.Item.Records.Add(record);
            }
        }

    }
}
