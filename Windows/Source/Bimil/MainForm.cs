using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Medo.Security.Cryptography.Bimil;
using Medo.Configuration;
using System.Collections.Generic;

namespace Bimil {
    internal partial class MainForm : Form {

        private BimilDocument Document = null;
        private string DocumentFileName = null;
        private bool DocumentChanged = false;
        private RecentFiles RecentFiles = new RecentFiles();
        private readonly List<string> Categories = new List<string>();

        public MainForm() {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;
            mnu.Font = SystemFonts.MessageBoxFont;
            lsvPasswords.Font = SystemFonts.MessageBoxFont;

            mnu.Renderer = Helpers.ToolStripBorderlessSystemRendererInstance;

            Medo.Windows.Forms.State.SetupOnLoadAndClose(this);
        }

        private void Form_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyData) {
                case Keys.Alt | Keys.Menu: {
                        mnu.Select();
                        mnuNew.Select();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    } break;

                case Keys.Escape: {
                        if (Settings.CloseOnEscape) {
                            this.Close();
                            e.Handled = true;
                            e.SuppressKeyPress = true;
                        }
                    } break;

                case Keys.Control | Keys.N: {
                        mnuNew_Click(null, null);
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    } break;

                case Keys.Control | Keys.O: {
                        mnuOpen_ButtonClick(null, null);
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    } break;

                case Keys.Control | Keys.S: {
                        mnuSave_ButtonClick(null, null);
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    } break;
            }
        }

        private void Form_Load(object sender, EventArgs e) {
            RefreshFiles();
            RefreshCategories();
            RefreshItems();
            UpdateMenu();
        }

        private void Form_Shown(object sender, EventArgs e) {
            var fileName = Medo.Application.Args.Current.GetValue("");
            if (fileName != null) { LoadFile(fileName); }
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e) {
            if (SaveIfNeeded() != DialogResult.OK) {
                e.Cancel = true;
                return;
            }

            this.Document = null;
            this.DocumentFileName = null;
            this.DocumentChanged = false;
            Application.Exit();
        }

        private void Form_Resize(object sender, EventArgs e) {
            lsvPasswords.Columns[0].Width = lsvPasswords.ClientSize.Width;
        }

        private void RefreshFiles() {
            for (int i = mnuOpen.DropDownItems.Count - 1; i >= 0; i--) {
                if (mnuOpen.DropDownItems[i] is ToolStripMenuItem) {
                    mnuOpen.DropDownItems.RemoveAt(i);
                } else {
                    break;
                }
            }
            for (int i = 0; i < this.RecentFiles.Count; i++) {
                var item = new ToolStripMenuItem(this.RecentFiles[i].Title) { Tag = this.RecentFiles[i].FileName };
                item.Click += new EventHandler(delegate(object sender2, EventArgs e2) {
                    if (SaveIfNeeded() != DialogResult.OK) { return; }
                    LoadFile(item.Tag.ToString());
                });
                mnuOpen.DropDownItems.Add(item);
            }
        }

        private void lsvPasswords_ItemActivate(object sender, EventArgs e) {
            mnuEdit_Click(null, null);
        }


        private void cmbSearch_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyData) {
                case Keys.Down:
                    if (lsvPasswords.Items.Count > 0) {
                        if (lsvPasswords.SelectedIndices.Count == 0) {
                            lsvPasswords.Items[0].Selected = true;
                        } else {
                            int index = Math.Min(lsvPasswords.SelectedIndices[lsvPasswords.SelectedIndices.Count - 1] + 1, lsvPasswords.Items.Count - 1);
                            foreach (ListViewItem item in lsvPasswords.Items) { item.Selected = false; }
                            lsvPasswords.Items[index].Selected = true;
                            lsvPasswords.Items[index].Focused = true;
                        }
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Up:
                    if (lsvPasswords.Items.Count > 0) {
                        if (lsvPasswords.SelectedIndices.Count == 0) {
                            lsvPasswords.Items[lsvPasswords.Items.Count - 1].Selected = true;
                        } else {
                            int index = Math.Max(lsvPasswords.SelectedIndices[0] - 1, 0);
                            foreach (ListViewItem item in lsvPasswords.Items) { item.Selected = false; }
                            lsvPasswords.Items[index].Selected = true;
                            lsvPasswords.Items[index].Focused = true;
                        }
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.PageDown: {
                        int index = (cmbSearch.SelectedIndex == -1) ? 0 : cmbSearch.SelectedIndex;
                        cmbSearch.SelectedIndex = Math.Min(index + 1, cmbSearch.Items.Count - 1);
                        cmbSearch.SelectAll();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    } break;

                case Keys.PageUp: {
                        int index = (cmbSearch.SelectedIndex == -1) ? cmbSearch.Items.Count - 1 : cmbSearch.SelectedIndex;
                        cmbSearch.SelectedIndex = Math.Max(index - 1, 0);
                        cmbSearch.SelectAll();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    } break;

                case Keys.Enter:
                    if (lsvPasswords.Items.Count > 0) {
                        lsvPasswords_ItemActivate(null, null);
                    } break;

                default:
                    lsvPasswords_KeyDown(null, e);
                    break;
            }
        }

        private void lsvPasswords_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyData) {
                case Keys.Insert: {
                        mnuAdd_Click(null, null);
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    } break;

                case Keys.F4: {
                        mnuEdit_Click(null, null);
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    } break;

                case Keys.Delete: {
                        mnuRemove_Click(null, null);
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    } break;

            }
        }

        private void lsvPasswords_SelectedIndexChanged(object sender, EventArgs e) {
            UpdateMenu();
        }


        private DialogResult SaveIfNeeded() {
            if (this.DocumentChanged) {
                string question;
                if (this.DocumentFileName != null) {
                    var file = new FileInfo(this.DocumentFileName);
                    var title = file.Name.Substring(0, file.Name.Length - file.Extension.Length);
                    question = title + " is not saved. Do you wish to save it now?";
                } else {
                    question = "Document is not saved. Do you wish to save it now?";
                }
                switch (Medo.MessageBox.ShowQuestion(this, question, MessageBoxButtons.YesNoCancel)) {
                    case DialogResult.Yes:
                        mnuSave_ButtonClick(null, null);
                        return (this.DocumentChanged == false) ? DialogResult.OK : DialogResult.Cancel;
                    case DialogResult.No: return DialogResult.OK;
                    case DialogResult.Cancel: return DialogResult.Cancel;
                    default: return DialogResult.Cancel;
                }
            } else {
                return DialogResult.OK;
            }
        }


        #region Menu

        private void mnuNew_Click(object sender, EventArgs e) {
            if (SaveIfNeeded() != DialogResult.OK) { return; }
            BimilDocument doc = null;
            try {
                using (var frm = new NewPasswordForm()) {
                    if (frm.ShowDialog(this) == DialogResult.OK) {
                        doc = new BimilDocument(frm.Password);
                    }
                }
            } finally {
                GC.Collect(); //in attempt to kill password string
            }
            if (doc != null) {
                this.Document = doc;
                this.DocumentFileName = null;
                this.DocumentChanged = false;
            }
            RefreshCategories();
            RefreshItems();
            UpdateMenu();
            cmbSearch.Select();
        }

        private void mnuOpen_ButtonClick(object sender, EventArgs e) {
            if (SaveIfNeeded() != DialogResult.OK) { return; }
            using (var frm = new OpenFileDialog() { AddExtension = true, AutoUpgradeEnabled = true, Filter = "Bimil files|*.bimil|All files|*.*", RestoreDirectory = true }) {
                if (frm.ShowDialog(this) == DialogResult.OK) {
                    LoadFile(frm.FileName);
                }
            }
        }

        private void mnuOpenFromGoogleDocs_Click(object sender, EventArgs e) {
            using (var frmAuth = new GoogleDocsAuthForm()) {
                if (frmAuth.ShowDialog(this) == DialogResult.OK) {
                    using (var frmDocs = new GoogleDocsOpenForm(frmAuth.AuthorizationToken)) {
                        if (frmDocs.ShowDialog(this) == DialogResult.OK) {
                            //frmDocs.ContentBytes 
                        }
                    }
                }
            }
        }

        private void LoadFile(string fileName, string password = null) {
            try {
                BimilDocument doc = null;
                try {
                    if (password == null) {
                        using (var frm = new PasswordForm()) {
                            if (frm.ShowDialog(this) == DialogResult.OK) {
                                doc = BimilDocument.Open(fileName, frm.Password);
                            }
                        }
                    } else {
                        doc = BimilDocument.Open(fileName, password);
                    }
                } finally {
                    GC.Collect(); //in attempt to kill password string
                }
                this.Document = doc;
                this.DocumentFileName = fileName;
                this.DocumentChanged = false;
                this.RecentFiles.Push(this.DocumentFileName);
                RefreshFiles();
            } catch (FormatException) {
                Medo.MessageBox.ShowError(this, "Either password is wrong or file is damaged.");
            }
            RefreshCategories();
            RefreshItems();
            UpdateMenu();
            cmbSearch.Select();
        }

        private void mnuSave_ButtonClick(object sender, EventArgs e) {
            if (this.Document == null) { return; }

            if (this.DocumentFileName != null) {
                this.Document.Save(this.DocumentFileName);
                this.DocumentChanged = false;
                UpdateMenu();
            } else {
                mnuSaveAs_Click(null, null);
            }
            cmbSearch.Select();
        }

        private void mnuSaveAs_Click(object sender, EventArgs e) {
            if (this.Document == null) { return; }

            using (var frm = new SaveFileDialog() { AddExtension = true, AutoUpgradeEnabled = true, Filter = "Bimil files|*.bimil|All files|*.*", RestoreDirectory = true }) {
                if (this.DocumentFileName != null) { frm.FileName = this.DocumentFileName; }
                if (frm.ShowDialog(this) == DialogResult.OK) {
                    this.Document.Save(frm.FileName);
                    this.DocumentFileName = frm.FileName;
                    this.DocumentChanged = false;
                    this.RecentFiles.Push(this.DocumentFileName);
                    RefreshFiles();
                    UpdateMenu();
                }
            }
            cmbSearch.Select();
        }

        private void mnuSaveToGoogleDocs_Click(object sender, EventArgs e) {
            using (var frm = new GoogleDocsAuthForm()) {
                if (frm.ShowDialog(this) == DialogResult.OK) {
                    using (var frm2 = new GoogleDocsSaveForm(frm.AuthorizationToken)) {
                        frm2.ShowDialog(this);
                    }
                }
            }
        }


        private void mnuChangePassword_Click(object sender, EventArgs e) {
            if (this.Document == null) { return; }

            BimilDocument currDoc = null;
            try {
                using (var frm = new PasswordForm()) {
                    if (frm.ShowDialog(this) == DialogResult.OK) {
                        using (var stream = new MemoryStream()) {
                            this.Document.Save(stream);
                            stream.Position = 0;
                            currDoc = BimilDocument.Open(stream, frm.Password);
                        }
                    }
                }
            } catch (FormatException) {
                Medo.MessageBox.ShowError(this, "Old password does not match.");
            } finally {
                GC.Collect(); //in attempt to kill password string
            }

            if (currDoc == null) { return; }

            BimilDocument newDoc = null;
            try {
                using (var frm = new NewPasswordForm()) {
                    if (frm.ShowDialog(this) == DialogResult.OK) {
                        using (var stream = new MemoryStream()) {
                            currDoc.Save(stream, frm.Password);
                            stream.Position = 0;
                            newDoc = BimilDocument.Open(stream, frm.Password);
                        }
                    }
                }
            } finally {
                GC.Collect(); //in attempt to kill password string
            }

            this.Document = newDoc;
            this.DocumentChanged = true;
            RefreshCategories();
            RefreshItems();
            UpdateMenu();
            cmbSearch.Select();
        }

        private void mnuAdd_Click(object sender, EventArgs e) {
            if (this.Document == null) { return; }

            using (var frm = new SelectTemplateForm()) {
                if (frm.ShowDialog(this) == DialogResult.OK) {
                    var item = this.Document.AddItem("New item", 0);
                    foreach (var record in frm.Template.Records) {
                        item.AddRecord(record.Key, "", record.Value);
                    }

                    using (var frm2 = new EditItemForm(this.Document, item, true, this.Categories)) {
                        if (frm2.ShowDialog(this) == DialogResult.OK) {
                            var listItem = new ListViewItem(item.Name) { Tag = item };
                            lsvPasswords.Items.Add(listItem);
                            this.DocumentChanged = true;
                            RefreshCategories();
                        } else {
                            this.Document.Items.Remove(item);
                        }
                    }

                    UpdateMenu();
                }
            }
            cmbSearch.Select();
        }

        private void mnuEdit_Click(object sender, EventArgs e) {
            if ((this.Document == null) || (lsvPasswords.SelectedItems.Count != 1)) { return; }

            var item = (BimilItem)(lsvPasswords.SelectedItems[0].Tag);
            using (var frm2 = new EditItemForm(this.Document, item, false, this.Categories)) {
                if (frm2.ShowDialog(this) == DialogResult.OK) {
                    lsvPasswords.SelectedItems[0].Text = item.Name;
                    this.DocumentChanged = true;
                    RefreshCategories();
                    UpdateMenu();
                }
            }
        }

        private void mnuRemove_Click(object sender, EventArgs e) {
            if ((this.Document == null) || (lsvPasswords.SelectedItems.Count == 0)) { return; }

            for (int i = lsvPasswords.SelectedItems.Count - 1; i >= 0; i--) {
                var item = (BimilItem)(lsvPasswords.SelectedItems[i].Tag);
                this.Document.Items.Remove(item);
                lsvPasswords.Items.Remove(lsvPasswords.SelectedItems[i]);
            }
            this.DocumentChanged = true;
            UpdateMenu();
            cmbSearch.Select();
        }

        private void mnuOptions_Click(object sender, EventArgs e) {
            using (var frm = new SettingsForm()) {
                frm.ShowDialog(this);
            }
            cmbSearch.Select();
        }

        private void mnuReportABug_Click(object sender, EventArgs e) {
            Medo.Diagnostics.ErrorReport.ShowDialog(this, null, new Uri("http://jmedved.com/errorreport/"));
            cmbSearch.Select();
        }

        private void mnuAbout_Click(object sender, EventArgs e) {
            Medo.Windows.Forms.AboutBox.ShowDialog(this, new Uri("http://www.jmedved.com/bimil/"));
            cmbSearch.Select();
        }

        #endregion


        private void cmbSearch_SelectedIndexChanged(object sender, EventArgs e) {
            RefreshItems();
        }


        private void RefreshCategories() {
            this.Categories.Clear();
            if (this.Document != null) {
                this.Categories.Add("");
                foreach (var item in this.Document.Items) {
                    if (this.Categories.Contains(item.CategoryRecord.Value.Text) == false) {
                        this.Categories.Add(item.CategoryRecord.Value.Text);
                    }
                }
            }
            this.Categories.Sort();

            cmbSearch.BeginUpdate();
            cmbSearch.Items.Clear();
            foreach (var category in this.Categories) {
                cmbSearch.Items.Add(category);
            }
            cmbSearch.EndUpdate();
        }

        private void RefreshItems() {
            lsvPasswords.BeginUpdate();
            lsvPasswords.Items.Clear();
            if (this.Document != null) {
                foreach (var item in this.Document.Items) {
                    if ((string.Equals(cmbSearch.Text, item.CategoryRecord.Value.Text, StringComparison.CurrentCultureIgnoreCase)) || ((cmbSearch.Text.Length > 0) && (item.Name.IndexOf(cmbSearch.Text, StringComparison.CurrentCultureIgnoreCase) >= 0))) {
                        lsvPasswords.Items.Add(new ListViewItem(item.Name) { Tag = item });
                    }
                }
            }
            lsvPasswords.EndUpdate();
            if (lsvPasswords.Items.Count > 0) {
                lsvPasswords.Items[0].Selected = true;
                lsvPasswords.Items[0].Focused = true;
            }
            Form_Resize(null, null);
        }

        private void UpdateMenu() {
            mnuSave.Enabled = (this.Document != null);
            mnuChangePassword.Enabled = (this.Document != null);
            mnuAdd.Enabled = (this.Document != null);
            mnuEdit.Enabled = (this.Document != null) && (lsvPasswords.SelectedItems.Count == 1);
            mnuRemove.Enabled = (this.Document != null) && (lsvPasswords.SelectedItems.Count > 0);

            pnlDocument.Visible = (this.Document != null);

            if (this.DocumentFileName == null) {
                this.Text = this.DocumentChanged ? "Bimil*" : "Bimil";
            } else {
                var file = new FileInfo(this.DocumentFileName);
                var title = file.Name.Substring(0, file.Name.Length - file.Extension.Length);

                this.Text = title + (this.DocumentChanged ? "*" : "") + " - Bimil";
            }
        }

    }
}
