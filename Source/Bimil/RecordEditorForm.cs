using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Medo.Security.Cryptography.PasswordSafe;

namespace Bimil {
    internal partial class RecordEditorForm : Form {

        private readonly Document Document;
        private readonly Entry Item;

        public RecordEditorForm(Document document, Entry item) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;

            this.Document = document;
            this.Item = item;
        }

        private void Form_Load(object sender, EventArgs e) {
            lsvFields.Columns[0].Width = lsvFields.Width - SystemInformation.VerticalScrollBarWidth;

            foreach (var record in this.Item.Records) {
                var caption = Helpers.GetRecordCaption(record);
                if (caption != null) {
                    var lvi = new ListViewItem(caption) { Tag = record };
                    lsvFields.Items.Add(lvi);
                }
            }
        }

        private void lsvFields_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyData) {
                case Keys.Insert: {
                        btnAdd_Click(null, null);
                    }
                    break;
                case Keys.Delete: {
                        btnRemove_Click(null, null);
                    }
                    break;
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
            var records = new List<Record>();
            foreach (ListViewItem item in lsvFields.Items) {
                records.Add((Record)item.Tag);
            }
            using (var frm = new NewRecordForm(this.Document, records.AsReadOnly())) {
                if (frm.ShowDialog(this) == DialogResult.OK) {
                    var lvi = new ListViewItem(Helpers.GetRecordCaption(frm.Record)) { Tag = frm.Record };
                    lsvFields.Items.Add(lvi);
                }
            }
        }

        private List<Record> RecordsToRemove = new List<Record>();

        private void btnRemove_Click(object sender, EventArgs e) {
            if (lsvFields.SelectedItems.Count == 1) {
                this.RecordsToRemove.Add(lsvFields.SelectedItems[0].Tag as Record);
                lsvFields.Items.RemoveAt(lsvFields.SelectedItems[0].Index);
            }
        }

        private void btnOK_Click(object sender, EventArgs e) {
            foreach (var record in this.RecordsToRemove) {
                this.Item.Records.Remove(record);
            }
            foreach (ListViewItem lvi in lsvFields.Items) {
                var record = (Record)lvi.Tag;
                if (this.Item.Records.Contains(record)) {
                    this.Item.Records.Remove(record);
                    this.Item.Records.Add(record);
                } else {
                    this.Item.Records.Add(record);
                }
            }
        }

    }
}
