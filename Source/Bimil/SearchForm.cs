using Medo.Security.Cryptography.PasswordSafe;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Bimil {
    internal partial class SearchForm : Form {
        public SearchForm(Document document, IList<string> categories, string defaultText) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;
            Medo.Windows.Forms.State.SetupOnLoadAndClose(this);

            this.Document = document;
            this.Categories = categories;
            this.DefaultText = defaultText;
        }

        private readonly Document Document;
        private readonly IList<string> Categories;
        private readonly string DefaultText;

        protected override bool ProcessDialogKey(Keys keyData) {
            switch (keyData) {
                case Keys.Escape:
                    this.Close();
                    return true;
            }

            return base.ProcessDialogKey(keyData);
        }

        #region Disable minimize

        protected override void WndProc(ref Message m) {
            if ((Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major < 10)
                && (m.Msg == NativeMethods.WM_SYSCOMMAND) && (m.WParam == NativeMethods.SC_MINIMIZE)) {
                m.Result = IntPtr.Zero;
            } else {
                base.WndProc(ref m);
            }
        }


        private class NativeMethods {
            internal const Int32 WM_SYSCOMMAND = 0x0112;
            internal readonly static IntPtr SC_MINIMIZE = new IntPtr(0xF020);
        }

        #endregion


        private void Form_Shown(object sender, EventArgs e) {
            cmbSearch.Text = this.DefaultText;
            cmbSearch.SelectAll();
        }

        private void Form_Resize(object sender, EventArgs e) {
            lsvEntries.Columns[0].Width = lsvEntries.ClientSize.Width;
        }


        private void cmbSearch_KeyDown(object sender, KeyEventArgs e) {
            if (!Helpers.HandleSearchKeyDown(e, lsvEntries, processPageUpDown: true)) {
                if (e.KeyData == (Keys.Control | Keys.F)) {
                    cmbSearch.SelectAll();
                    cmbSearch.Select();
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                }
            }
        }

        private void cmbText_TextChanged(object sender, System.EventArgs e) {
            Helpers.PerformEntrySearch(this.Document, lsvEntries, cmbSearch.Text, extendedSearch: true, addMatchDescription: true);
            Form_Resize(null, null); //to support both ListView full row with and without scrollbar
        }


        private void lsvEntries_ItemActivate(object sender, EventArgs e) {
            if ((this.Document == null) || (lsvEntries.SelectedItems.Count != 1)) { return; }

            var item = (Entry)(lsvEntries.SelectedItems[0].Tag);
            using (var frm2 = new ItemForm(this.Document, item, this.Categories, startsAsEditable: Settings.EditableByDefault)) {
                if (frm2.ShowDialog(this) == DialogResult.OK) {
                    Helpers.PerformEntrySearch(this.Document, lsvEntries, cmbSearch.Text, entryToSelect: item, extendedSearch: true, addMatchDescription: true);
                    Form_Resize(null, null); //to support both ListView full row with and without scrollbar
                }
            }
        }

        private void lsvEntries_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyData == (Keys.Control | Keys.F)) {
                cmbSearch.SelectAll();
                cmbSearch.Select();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

    }
}
