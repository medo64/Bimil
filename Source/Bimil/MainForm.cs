using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Medo.Configuration;
using Medo.Security.Cryptography.PasswordSafe;
using LegacyFile = Medo.Security.Cryptography.Bimil;

namespace Bimil {
    internal partial class MainForm : Form {

        private Document Document = null;
        private string DocumentFileName = null;
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


        private bool SuppressMenuKey = false;

        protected override bool ProcessDialogKey(Keys keyData) {
            if (((keyData & Keys.Alt) == Keys.Alt) && (keyData != (Keys.Alt | Keys.Menu))) { this.SuppressMenuKey = true; }

            switch (keyData) {

                case Keys.F10:
                    ToggleMenu();
                    return true;

                case Keys.Escape: {
                        if (Settings.CloseOnEscape) {
                            this.Close();
                            return true;
                        }
                    }
                    break;

                case Keys.Control | Keys.N:
                case Keys.Alt | Keys.N:
                    mnuNew.PerformClick();
                    return true;

                case Keys.Control | Keys.O:
                    mnuOpen.PerformButtonClick();
                    return true;

                case Keys.Alt | Keys.O:
                    mnuOpen.ShowDropDown();
                    return true;

                case Keys.Control | Keys.S:
                    mnuSave.PerformButtonClick();
                    return true;

                case Keys.Alt | Keys.S:
                    mnuSave.ShowDropDown();
                    return true;

            }

            return base.ProcessDialogKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            if (e.KeyData == Keys.Menu) {
                if (this.SuppressMenuKey) { this.SuppressMenuKey = false; return; }
                ToggleMenu();
                e.Handled = true;
                e.SuppressKeyPress = true;
            } else {
                base.OnKeyDown(e);
            }
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            if (e.KeyData == Keys.Menu) {
                if (this.SuppressMenuKey) { this.SuppressMenuKey = false; return; }
                ToggleMenu();
                e.Handled = true;
                e.SuppressKeyPress = true;
            } else {
                base.OnKeyUp(e);
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
                item.Click += new EventHandler(delegate (object sender2, EventArgs e2) {
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
                    }
                    break;

                case Keys.PageUp: {
                        int index = (cmbSearch.SelectedIndex == -1) ? cmbSearch.Items.Count - 1 : cmbSearch.SelectedIndex;
                        cmbSearch.SelectedIndex = Math.Max(index - 1, 0);
                        cmbSearch.SelectAll();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                    break;

                case Keys.Enter:
                    if (lsvPasswords.Items.Count > 0) {
                        lsvPasswords.Select();
                    }
                    break;

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
                    }
                    break;

                case Keys.F4: {
                        mnuEdit_Click(null, null);
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                    break;

                case Keys.Delete: {
                        mnuRemove_Click(null, null);
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                    break;

                case Keys.PageDown: {
                        int index = (cmbSearch.SelectedIndex == -1) ? 0 : cmbSearch.SelectedIndex;
                        cmbSearch.SelectedIndex = Math.Min(index + 1, cmbSearch.Items.Count - 1);
                        cmbSearch.SelectAll();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                    break;

                case Keys.PageUp: {
                        int index = (cmbSearch.SelectedIndex == -1) ? cmbSearch.Items.Count - 1 : cmbSearch.SelectedIndex;
                        cmbSearch.SelectedIndex = Math.Max(index - 1, 0);
                        cmbSearch.SelectAll();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                    break;

            }
        }

        private void lsvPasswords_SelectedIndexChanged(object sender, EventArgs e) {
            UpdateMenu();
        }


        private DialogResult SaveIfNeeded() {
            if (this.Document?.HasChanged ?? false) {
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
                        return (this.Document.HasChanged == false) ? DialogResult.OK : DialogResult.Cancel;
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
            Document doc = null;
            try {
                using (var frm = new NewPasswordForm()) {
                    if (frm.ShowDialog(this) == DialogResult.OK) {
                        doc = new Document(frm.Password);
                    }
                }
            } finally {
                GC.Collect(); //in attempt to kill password string
            }
            if (doc != null) {
                this.Document = doc;
                this.DocumentFileName = null;
            }
            RefreshCategories();
            RefreshItems();
            UpdateMenu();
            cmbSearch.Select();
        }

        private void mnuOpen_ButtonClick(object sender, EventArgs e) {
            if (SaveIfNeeded() != DialogResult.OK) { return; }
            using (var frm = new OpenFileDialog() { AddExtension = true, AutoUpgradeEnabled = true, Filter = "Bimil files|*.bimil|Password Safe files|*.psafe3|All files|*.*", RestoreDirectory = true }) {
                if (frm.ShowDialog(this) == DialogResult.OK) {
                    LoadFile(frm.FileName);
                }
            }
        }

        private void LoadFile(string fileName, string password = null) {
            try {
                if (password == null) {
                    using (var frm = new PasswordForm()) {
                        if (frm.ShowDialog(this) == DialogResult.OK) {
                            password = frm.Password;
                        } else {
                            return;
                        }
                    }
                }

                if (fileName.EndsWith(".bimil", StringComparison.OrdinalIgnoreCase)) {
                    try {
                        LoadBimilFile(fileName, password);
                    } catch (FormatException) { //try password safe if bimil format fails
                        LoadPasswordSafeFile(fileName, password);
                    }
                } else { //Password Safe
                    LoadPasswordSafeFile(fileName, password);
                }
            } finally {
                password = null;
                GC.Collect(); //in attempt to kill password string
            }
        }

        private void LoadPasswordSafeFile(string fileName, string password = null) {
            try {
                using (var fileStream = File.OpenRead(fileName)) {
                    this.Document = Document.Load(fileStream, password);
                    this.Document.TrackAccess = false;
                    this.DocumentFileName = fileName;
                }
            } catch (FormatException) {
                Medo.MessageBox.ShowError(this, "Either password is wrong or file is damaged.");
            }

            this.RecentFiles.Push(fileName);
            RefreshFiles();

            RefreshCategories();
            RefreshItems();
            UpdateMenu();
            cmbSearch.Select();
        }

        private void LoadBimilFile(string fileName, string password = null) {
            LegacyFile.BimilDocument legacyDoc = null;
            try {
                legacyDoc = LegacyFile.BimilDocument.Open(fileName, password);

                this.Document = DocumentConversion.ConvertFromBimil(legacyDoc, password);
                this.DocumentFileName = null;
            } finally {
                password = null;
                GC.Collect(); //in attempt to kill password string
            }

            RefreshCategories();
            RefreshItems();
            UpdateMenu();
            cmbSearch.Select();
        }

        private void mnuSave_ButtonClick(object sender, EventArgs e) {
            if (this.Document == null) { return; }

            if (this.DocumentFileName != null) {
                using (var fileStream = new FileStream(this.DocumentFileName, FileMode.Create, FileAccess.Write)) {
                    this.Document.Save(fileStream);
                }
                UpdateMenu();
            } else {
                mnuSaveAs_Click(null, null);
            }
            cmbSearch.Select();
        }

        private void mnuSaveAs_Click(object sender, EventArgs e) {
            if (this.Document == null) { return; }

            using (var frm = new SaveFileDialog() { AddExtension = true, AutoUpgradeEnabled = true, Filter = "Bimil files|*.bimil|Password Safe files|*.psafe3|All files|*.*", RestoreDirectory = true }) {
                if (this.DocumentFileName != null) { frm.FileName = this.DocumentFileName; }
                if (frm.ShowDialog(this) == DialogResult.OK) {
                    using (var fileStream = new FileStream(frm.FileName, FileMode.Create, FileAccess.Write)) {
                        this.Document.Save(fileStream);
                    }
                    this.DocumentFileName = frm.FileName;
                    this.RecentFiles.Push(this.DocumentFileName);
                    RefreshFiles();
                    UpdateMenu();
                }
            }
            cmbSearch.Select();
        }


        private void mnuChangePassword_Click(object sender, EventArgs e) {
            if (this.Document == null) { return; }

            try {
                using (var frm = new PasswordForm()) {
                    if (frm.ShowDialog(this) == DialogResult.OK) {
                        if (!this.Document.ValidatePassphrase(frm.Password)) {
                            Medo.MessageBox.ShowError(this, "Old password does not match.");
                            return;
                        }
                    }
                }
            } catch (FormatException) {
            } finally {
                GC.Collect(); //in attempt to kill password string
            }

            try {
                using (var frm = new NewPasswordForm()) {
                    if (frm.ShowDialog(this) == DialogResult.OK) {
                        using (var stream = new MemoryStream()) {
                            this.Document.ChangePassphrase(frm.Password);
                        }
                    }
                }
            } finally {
                GC.Collect(); //in attempt to kill password string
            }

            RefreshCategories();
            RefreshItems();
            UpdateMenu();
            cmbSearch.Select();
        }

        private void mnuAdd_Click(object sender, EventArgs e) {
            if (this.Document == null) { return; }

            using (var frm = new SelectTemplateForm()) {
                if (frm.ShowDialog(this) == DialogResult.OK) {
                    var entry = new Entry("New item");
                    entry.Records[RecordType.Password] = null;
                    this.Document.Entries.Add(entry);
                    foreach (var recordType in frm.Template.RecordTypes) {
                        entry.Records.Add(new Record(recordType));
                    }

                    using (var frm2 = new EditItemForm(this.Document, entry, true, this.Categories)) {
                        if (frm2.ShowDialog(this) == DialogResult.OK) {
                            var listItem = new ListViewItem(entry.Title) { Tag = entry };
                            lsvPasswords.Items.Add(listItem);
                            RefreshCategories();
                        } else {
                            this.Document.Entries.Remove(entry);
                        }
                    }

                    UpdateMenu();
                }
            }
            cmbSearch.Select();
        }

        private void mnuEdit_Click(object sender, EventArgs e) {
            if ((this.Document == null) || (lsvPasswords.SelectedItems.Count != 1)) { return; }

            var item = (Entry)(lsvPasswords.SelectedItems[0].Tag);
            using (var frm2 = new EditItemForm(this.Document, item, false, this.Categories)) {
                if (frm2.ShowDialog(this) == DialogResult.OK) {
                    lsvPasswords.SelectedItems[0].Text = item.Title;
                    RefreshCategories();
                    UpdateMenu();
                }
            }
        }

        private void mnuRemove_Click(object sender, EventArgs e) {
            if ((this.Document == null) || (lsvPasswords.SelectedItems.Count == 0)) { return; }

            for (int i = lsvPasswords.SelectedItems.Count - 1; i >= 0; i--) {
                var item = (Entry)(lsvPasswords.SelectedItems[i].Tag);
                this.Document.Entries.Remove(item);
                lsvPasswords.Items.Remove(lsvPasswords.SelectedItems[i]);
            }
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
                foreach (var item in this.Document.Entries) {
                    if (!this.Categories.Contains(item.Group)) {
                        this.Categories.Add(item.Group);
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
                foreach (var item in this.Document.Entries) {
                    if ((string.Equals(cmbSearch.Text, item.Group, StringComparison.CurrentCultureIgnoreCase)) || ((cmbSearch.Text.Length > 0) && (item.Title.IndexOf(cmbSearch.Text, StringComparison.CurrentCultureIgnoreCase) >= 0))) {
                        lsvPasswords.Items.Add(new ListViewItem(item.Title) { Tag = item });
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
                this.Text = (this.Document?.HasChanged ?? false) ? "Bimil*" : "Bimil";
            } else {
                var file = new FileInfo(this.DocumentFileName);
                var title = file.Name.Substring(0, file.Name.Length - file.Extension.Length);

                this.Text = title + (this.Document?.HasChanged ?? false ? "*" : "") + " - Bimil";
            }
        }


        private void ToggleMenu() {
            if (mnu.ContainsFocus) {
                if (lsvPasswords.Visible) {
                    lsvPasswords.Select();
                } else {
                    this.Select();
                }
            } else {
                mnu.Select();
                mnu.Items[0].Select();
            }
        }

    }
}
