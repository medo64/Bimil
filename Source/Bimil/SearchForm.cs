using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Medo.Security.Cryptography.PasswordSafe;

namespace Bimil {
    internal partial class SearchForm : Form {
        public SearchForm(Document document, IList<string> categories, string defaultText) {
            InitializeComponent();
            Font = SystemFonts.MessageBoxFont;
            Medo.Windows.Forms.State.Attach(this);

            Document = document;
            Categories = categories;
            DefaultText = defaultText;

            erp.SetIconAlignment(chbIncludeHiddenFields, ErrorIconAlignment.MiddleRight);
            erp.SetIconPadding(chbIncludeHiddenFields, chbIncludeHiddenFields.Height / 6);
        }

        private readonly Document Document;
        private readonly IList<string> Categories;
        private readonly string DefaultText;


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


        private void Form_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyData == Keys.Escape) {
                Close();
            }
        }

        private void Form_Shown(object sender, EventArgs e) {
            cmbSearch.Text = DefaultText;
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
            Helpers.PerformEntrySearch(Document, lsvEntries, cmbSearch.Text, extendedSearch: true, addMatchDescription: true, includeHidden: chbIncludeHiddenFields.Checked);
            Form_Resize(null, null); //to support both ListView full row with and without scrollbar
        }


        private void lsvEntries_ItemActivate(object sender, EventArgs e) {
            if ((Document == null) || (lsvEntries.SelectedItems.Count != 1)) { return; }

            var item = (Entry)(lsvEntries.SelectedItems[0].Tag);
            using (var frm2 = new ItemForm(Document, item, Categories, startsAsEditable: Settings.EditableByDefault, hideAutotype: true)) {
                if (frm2.ShowDialog(this) == DialogResult.OK) {
                    Helpers.PerformEntrySearch(Document, lsvEntries, cmbSearch.Text, entriesToSelect: new Entry[] { item }, extendedSearch: true, addMatchDescription: true);
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

        private void chbIncludeHiddenFields_CheckedChanged(object sender, EventArgs e) {
            if (chbIncludeHiddenFields.Checked) {
                erp.SetError(chbIncludeHiddenFields, "Search will include password and key fields.");
            } else {
                erp.SetError(chbIncludeHiddenFields, null);
            }
            cmbText_TextChanged(null, null);
        }
    }
}
