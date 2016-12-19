using Medo.Configuration;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Bimil {
    internal partial class StartForm : Form {
        public StartForm(RecentFiles recentFiles) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;
            Medo.Windows.Forms.State.SetupOnLoadAndClose(this);

            this.RecentFiles = recentFiles;

            lsvRecent.SmallImageList = Helpers.GetImageList(this, "picNonexistent", "mnuReadOnly");

            foreach (var file in recentFiles) {
                var lvi = new ListViewItem(file.Title) { Tag = file, ToolTipText = file.FileName };
                var readOnly = Helpers.GetReadOnly(file.FileName);
                if (readOnly == null) {
                    lvi.ImageIndex = 0;
                } else if (readOnly.Value) {
                    lvi.ImageIndex = 1;
                }
                lsvRecent.Items.Add(lvi);
            }
            if (lsvRecent.Items.Count > 0) {
                lsvRecent.Items[0].Selected = true;
            } else {
                lsvRecent.Enabled = false;
                lsvRecent.Items.Add("No recently used files.");
                lsvRecent.ForeColor = SystemColors.GrayText;
            }
        }

        private readonly RecentFiles RecentFiles;


        private void Form_Shown(object sender, EventArgs e) {
            Form_Resize(null, null);
        }

        private void Form_Resize(object sender, EventArgs e) {
            lsvRecent_colFile.Width = lsvRecent.ClientSize.Width - SystemInformation.VerticalScrollBarWidth;
        }


        private void lsvRecent_SelectedIndexChanged(object sender, EventArgs e) {
            var fileName = (lsvRecent.SelectedItems.Count == 1) ? ((RecentFile)lsvRecent.SelectedItems[0].Tag).FileName : null;
            var isReadOnly = Helpers.GetReadOnly(fileName);

            btnOpen.Enabled = (isReadOnly == false);
            btnOpenReadOnly.Enabled = (isReadOnly != null);
        }

        private void lsvRecent_ItemActivate(object sender, EventArgs e) {
            var fileName = ((RecentFile)lsvRecent.SelectedItems[0].Tag).FileName;
            var readOnly = Helpers.GetReadOnly(fileName);
            var isReadOnly = (readOnly == true); //treats non-existing files as false (to allow opening them)

            SelectFileName(((RecentFile)lsvRecent.SelectedItems[0].Tag).FileName, isReadOnly);
            this.DialogResult = DialogResult.OK;
        }


        public string FileName { get; private set; }
        public bool IsReadOnly { get; private set; }


        private void btnOpen_Click(object sender, EventArgs e) {
            SelectFileName(((RecentFile)lsvRecent.SelectedItems[0].Tag).FileName);
        }

        private void btnOpenReadOnly_Click(object sender, EventArgs e) {
            SelectFileName(((RecentFile)lsvRecent.SelectedItems[0].Tag).FileName, readOnly: true);
        }

        private void btnNew_Click(object sender, EventArgs e) {
            this.FileName = null;
        }


        private void mnxList_Opening(object sender, System.ComponentModel.CancelEventArgs e) {
            var fileName = (lsvRecent.SelectedItems.Count == 1) ? ((RecentFile)lsvRecent.SelectedItems[0].Tag).FileName : null;
            var isReadOnly = Helpers.GetReadOnly(fileName);

            mnxListOpen.Enabled = (isReadOnly == false);
            mnxListOpenReadOnly.Enabled = (isReadOnly != null);
            mnxListRemove.Enabled = (fileName != null);
            mnxListReadOnly.Enabled = (isReadOnly != null);
            mnxListReadOnly.Checked = (isReadOnly == true);
        }

        private void mnxListOpen_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.OK;
            btnOpen_Click(null, null);
        }

        private void mnxListOpenReadOnly_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.OK;
            btnOpenReadOnly_Click(null, null);
        }

        private void mnxListRemove_Click(object sender, EventArgs e) {
            var fileName = ((RecentFile)lsvRecent.SelectedItems[0].Tag).FileName;
            this.RecentFiles.Remove(fileName);
        }

        private void mnxListReadOnly_Click(object sender, EventArgs e) {
            var newReadOnly = !mnxListReadOnly.Checked;
            var fileName = ((RecentFile)lsvRecent.SelectedItems[0].Tag).FileName;
            try {
                Helpers.SetReadOnly(fileName, newReadOnly);
                lsvRecent.SelectedItems[0].ImageIndex = newReadOnly ? 1 : -1;
                lsvRecent.Refresh();
            } catch (SystemException ex) {
                Medo.MessageBox.ShowError(this, ex.Message);
            }

            lsvRecent_SelectedIndexChanged(null, null);
        }


        private void SelectFileName(string fileName, bool readOnly = false) {
            this.FileName = fileName;
            this.IsReadOnly = readOnly;
        }
    }
}
