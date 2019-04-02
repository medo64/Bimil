using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Medo.Security.Cryptography.PasswordSafe;

namespace Bimil {
    internal partial class RecordEditorForm : Form {

        private readonly Document Document;
        private readonly Entry Item;

        public RecordEditorForm(Document document, Entry item) {
            InitializeComponent();
            Font = SystemFonts.MessageBoxFont;

            Document = document;
            Item = item;
        }


        #region Disable minimize

        protected override void WndProc(ref Message m) {
            if ((Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major < 10)
                && (m != null) && (m.Msg == NativeMethods.WM_SYSCOMMAND) && (m.WParam == NativeMethods.SC_MINIMIZE)) {
                m.Result = IntPtr.Zero;
            } else if (m != null) {
                base.WndProc(ref m);
            }
        }


        private class NativeMethods {
            internal const int WM_SYSCOMMAND = 0x0112;
            internal static readonly IntPtr SC_MINIMIZE = new IntPtr(0xF020);
        }

        #endregion


        private void Form_Load(object sender, EventArgs e) {
            lsvFields.Columns[0].Width = lsvFields.Width - SystemInformation.VerticalScrollBarWidth;

            foreach (var record in Item.Records) {
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
            for (var i = 0; i < e.Data.GetFormats().Length; i++) {
                if (e.Data.GetFormats()[i].Equals("System.Windows.Forms.ListView+SelectedListViewItemCollection")) {
                    e.Effect = DragDropEffects.Move;
                    break;
                }
            }
        }

        private void lsvFields_DragDrop(object sender, DragEventArgs e) {
            if (lsvFields.SelectedItems.Count == 0) { return; }

            var cp = lsvFields.PointToClient(new Point(e.X, e.Y));
            var dragToItem = lsvFields.GetItemAt(cp.X, cp.Y);
            if (dragToItem == null) { return; }

            var dragIndex = dragToItem.Index;
            var sel = new ListViewItem[lsvFields.SelectedItems.Count];
            for (var i = 0; i < lsvFields.SelectedItems.Count; i++) {
                sel[i] = lsvFields.SelectedItems[i];
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
                lsvFields.Items.Insert(itemIndex, insertItem);
                lsvFields.Items.Remove(dragItem);
            }
        }


        private void btnAdd_Click(object sender, EventArgs e) {
            var records = new List<Record>();
            foreach (ListViewItem item in lsvFields.Items) {
                records.Add((Record)item.Tag);
            }
            using (var frm = new NewRecordForm(Document, records.AsReadOnly())) {
                if (frm.ShowDialog(this) == DialogResult.OK) {
                    var lvi = new ListViewItem(Helpers.GetRecordCaption(frm.Record)) { Tag = frm.Record };
                    lsvFields.Items.Add(lvi);
                }
            }
        }

        private readonly List<Record> RecordsToRemove = new List<Record>();

        private void btnRemove_Click(object sender, EventArgs e) {
            if (lsvFields.SelectedItems.Count == 1) {
                RecordsToRemove.Add(lsvFields.SelectedItems[0].Tag as Record);
                lsvFields.Items.RemoveAt(lsvFields.SelectedItems[0].Index);
            }
        }

        private void btnOK_Click(object sender, EventArgs e) {
            foreach (var record in RecordsToRemove) {
                Item.Records.Remove(record);
            }
            foreach (ListViewItem lvi in lsvFields.Items) {
                var record = (Record)lvi.Tag;
                if (Item.Records.Contains(record)) {
                    Item.Records.Remove(record);
                    Item.Records.Add(record);
                } else {
                    Item.Records.Add(record);
                }
            }
        }

    }
}
