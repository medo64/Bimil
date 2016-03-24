using Medo.Configuration;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Bimil {
    internal partial class StartForm : Form {
        public StartForm(RecentFiles recentFiles) {
            InitializeComponent();
            Medo.Windows.Forms.State.SetupOnLoadAndClose(this);

            for (int i = 0; i < recentFiles.Count; i++) {
                var file = recentFiles[i];
                var lvi = new ListViewItem(file.Title) { Tag = file };
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


        private void Form_Shown(object sender, EventArgs e) {
            Form_Resize(null, null);
        }

        private void Form_Resize(object sender, EventArgs e) {
            lsvRecent_colFile.Width = lsvRecent.ClientSize.Width - SystemInformation.VerticalScrollBarWidth;
        }

        private void lsvRecent_SelectedIndexChanged(object sender, EventArgs e) {
            btnOpen.Enabled = (lsvRecent.SelectedItems.Count == 1);
        }



        public string FileName { get; private set; }
        public bool IsReadOnly { get; private set; }


        private void btnOpen_Click(object sender, EventArgs e) {
            this.FileName = ((RecentFile)lsvRecent.SelectedItems[0].Tag).FileName;
        }

        private void btnOpenReadOnly_Click(object sender, EventArgs e) {
            this.FileName = ((RecentFile)lsvRecent.SelectedItems[0].Tag).FileName;
            this.IsReadOnly = true;
        }

        private void btnNew_Click(object sender, EventArgs e) {
            this.FileName = null;
        }

    }
}
