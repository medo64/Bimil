using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Medo.Configuration;
using Medo.Security.Cryptography.PasswordSafe;
using LegacyFile = Medo.Security.Cryptography.Bimil;
using Medo.Security.Cryptography;
using System.Diagnostics;
using System.Threading;
using System.Reflection;
using System.Security;

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
            lsvEntries.Font = SystemFonts.MessageBoxFont;

            mnu.Renderer = Helpers.ToolStripBorderlessSystemRendererInstance;
            Helpers.ScaleToolstrip(mnu);

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

                case Keys.Control | Keys.F:
                    mnuSearch.PerformClick();
                    return true;

                case Keys.F5:
                    RefreshCategories();
                    RefreshItems();
                    UpdateMenu();
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
            if (fileName != null) {
                LoadFile(fileName);
            } else if (Settings.ShowStart) {
                using (var frm = new StartForm(this.RecentFiles)) {
                    if (frm.ShowDialog(this) == DialogResult.OK) {
                        if (frm.FileName != null) {
                            LoadFile(frm.FileName, isReadOnly: frm.IsReadOnly);
                        } else {
                            mnuNew_Click(null, null);
                        }
                    }
                }
            }

            var version = Assembly.GetExecutingAssembly().GetName().Version; //don't auto-check for development builds
            if ((version.Major != 0) || (version.Minor != 0)) { bwUpgradeCheck.RunWorkerAsync(); }
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e) {
            if (SaveIfNeeded() != DialogResult.OK) {
                e.Cancel = true;
                return;
            }

            bwUpgradeCheck.CancelAsync();

            this.Document = null;
            this.DocumentFileName = null;
            Application.Exit();
        }

        private void Form_Resize(object sender, EventArgs e) {
            lsvEntries.Columns[0].Width = lsvEntries.ClientSize.Width;
        }

        private void Form_Deactivate(object sender, EventArgs e) {
            if (Settings.AutoCloseTimeout > 0) {
                tmrClose.Interval = Settings.AutoCloseTimeout * 1000;
                tmrClose.Enabled = true;
            }
        }

        private void Form_Activated(object sender, EventArgs e) {
            if (this.Focused) {
                tmrClose.Enabled = false;
            } else if (tmrClose.Enabled) { //because damn Activated gets triggered after ShowDialog exists - even if application is not in focus
                tmrClose.Enabled = false;
                tmrClose.Enabled = true;
            }
        }


        private void tmrClose_Tick(object sender, EventArgs e) {
            if (Application.OpenForms.Count <= 1) {
                tmrClose.Enabled = false;

                if (this.Document != null) {
                    if (Settings.AutoCloseSave && this.Document.HasChanged) {
                        if (this.DocumentFileName != null) {
                            mnuSave_ButtonClick(null, null);
                        } else { //if it cannot be saved, cancel close
                            tmrClose.Enabled = true;
                            return;
                        }
                    }

                    var fileName = this.DocumentFileName;
                    var readOnly = this.Document.IsReadOnly;

                    this.Document = null;
                    this.DocumentFileName = null;
                    LoadFile(fileName, readOnly);
                }
            }
        }


        private void RefreshFiles() {
            for (int i = mnuOpen.DropDownItems.Count - 1; i >= 0; i--) {
                if (mnuOpen.DropDownItems[i] is ToolStripMenuItem) {
                    mnuOpen.DropDownItems.RemoveAt(i);
                } else {
                    break;
                }
            }
            foreach (var file in this.RecentFiles) {
                var item = new ToolStripMenuItem(file.Title) { Tag = file.FileName, ToolTipText = file.FileName };
                item.Click += new EventHandler(delegate (object sender2, EventArgs e2) {
                    if (SaveIfNeeded() != DialogResult.OK) { return; }
                    var fileName = item.Tag.ToString();
                    LoadFile(fileName);
                });
                mnuOpen.DropDownItems.Add(item);
            }
        }

        private void lsvEntries_ItemActivate(object sender, EventArgs e) {
            mnuEdit_Click(null, null);
        }


        private void cmbSearch_KeyDown(object sender, KeyEventArgs e) {
            if (!Helpers.HandleSearchKeyDown(e, lsvEntries)) {
                switch (e.KeyData) {
                    case Keys.PageDown: {
                            if (cmbSearch.Items.Count > 0) {
                                int newIndex = (cmbSearch.SelectedIndex > -1) ? cmbSearch.SelectedIndex + 1 : Helpers.GetNearestComboIndex(cmbSearch.Text, cmbSearch.Items, 1);
                                cmbSearch.SelectedIndex = Math.Min(newIndex, cmbSearch.Items.Count - 1);
                                cmbSearch.SelectAll();
                            }
                            e.Handled = true;
                            e.SuppressKeyPress = true;
                        }
                        break;

                    case Keys.PageUp: {
                            if (cmbSearch.Items.Count > 0) {
                                int newIndex = (cmbSearch.SelectedIndex > -1) ? cmbSearch.SelectedIndex - 1 : Helpers.GetNearestComboIndex(cmbSearch.Text, cmbSearch.Items, 0);
                                cmbSearch.SelectedIndex = Math.Max(newIndex, 0);
                                cmbSearch.SelectAll();
                            }
                            e.Handled = true;
                            e.SuppressKeyPress = true;
                        }
                        break;

                    default:
                        lsvEntries_KeyDown(null, e);
                        break;
                }
            }
        }


        private void lsvEntries_BeforeLabelEdit(object sender, LabelEditEventArgs e) {
            if (!Settings.EditableByDefault) { e.CancelEdit = true; }
        }

        private void lsvEntries_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyData) {
                case Keys.Insert: {
                        if (mnuAdd.Enabled) { mnuAdd_Click(null, null); }
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                    break;

                case Keys.F2: {
                        if ((lsvEntries.SelectedItems.Count == 1) && !this.Document.IsReadOnly) {
                            lsvEntries.SelectedItems[0].BeginEdit();
                        }
                    }
                    break;

                case Keys.F4: {
                        if (mnuEdit.Enabled) { mnuEdit_Click(null, null); }
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                    break;

                case Keys.Delete: {
                        if (mnuRemove.Enabled) { mnuRemove_Click(null, null); }
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

        private void lsvEntries_SelectedIndexChanged(object sender, EventArgs e) {
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
            using (var frm = new OpenFileDialog() { AddExtension = true, AutoUpgradeEnabled = true, Filter = "Bimil and PasswordSafe files|*.bimil;*.psafe3|Bimil files|*.bimil|Password Safe files|*.psafe3|All files|*.*", RestoreDirectory = true, ShowReadOnly = true }) {
                if (frm.ShowDialog(this) == DialogResult.OK) {
                    if (LoadFile(frm.FileName, isReadOnly: frm.ReadOnlyChecked)) {
                        if ((this.Document.LastSaveApplication == null) || !this.Document.LastSaveApplication.StartsWith("Bimil ")) {
                            var application = string.IsNullOrEmpty(this.Document.LastSaveApplication) ? "unknown" : this.Document.LastSaveApplication;
                            Medo.MessageBox.ShowWarning(this, "Be careful when saving files not created by Bimil.\n\nWhile such files can be used, not necessarily all the features of the original software will be supported. Likewise, files edited by Bimil won't be necessarily fully readable by other software; e.g. two-factor keys are only supported in Bimil.\n\nLast edited by:\n" + application);
                        }
                    }
                }
            }
        }

        private void mnuOpen_DropDownOpening(object sender, EventArgs e) {
            foreach (ToolStripDropDownItem item in mnuOpen.DropDownItems) {
                var fileName = item.Tag as string;
                if (fileName != null) {
                    if (!File.Exists(fileName)) {
                        Helpers.ScaleToolstripItem(item, "picNonexistent");
                    } else {
                        item.Image = null;
                    }
                }
            }
        }


        private bool LoadFile(string fileName, bool isReadOnly = false) { //return false if file cannot be open
            DocumentResult document = null;
            string password;

            try {
                if (!File.Exists(fileName)) {
                    foreach (var recentFile in this.RecentFiles) {
                        if (string.Equals(fileName, recentFile.FileName, StringComparison.OrdinalIgnoreCase)) {
                            if (Medo.MessageBox.ShowQuestion(this, "File " + Path.GetFileName(fileName) + " could not be open.\nDo you wish to remove it from the recent list?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                                this.RecentFiles.Remove(recentFile.FileName);
                                this.RefreshFiles();
                                break;
                            }
                        }
                    }
                    return false;
                }

                while (true) {
                    using (var frm = new PasswordForm()) {
                        if (frm.ShowDialog(this) == DialogResult.OK) {
                            password = frm.Password;
                        } else {
                            return true; //don't signal file doesn't exist just because password has been canceled
                        }
                    }

                    if (fileName.EndsWith(".bimil", StringComparison.OrdinalIgnoreCase)) {
                        if ((document = LoadPasswordSafeFile(fileName, password)) != null) {
                            return true;
                        } else if ((document = LoadBimilFile(fileName, password)) != null) {
                            return true;
                        }
                    } else { //Password Safe
                        if ((document = LoadPasswordSafeFile(fileName, password)) != null) {
                            return true;
                        }
                    }

                    Medo.MessageBox.ShowError(this, "Either password is wrong or file is damaged.");
                }
            } catch (IOException ex) {
                Medo.MessageBox.ShowError(this, "Cannot open file.\n\n" + ex.Message);
                return false;
            } catch (SecurityException ex) {
                Medo.MessageBox.ShowError(this, "Cannot open file.\n\n" + ex.Message);
                return false;
            } finally {
                password = null;
                GC.Collect(); //in attempt to kill password string

                if (document != null) {
                    this.Document = document.Document;
                    this.DocumentFileName = document.FileName;
                    this.Document.IsReadOnly = isReadOnly;

                    cmbSearch.BackColor = isReadOnly ? SystemColors.Control : SystemColors.Window;
                    lsvEntries.BackColor = isReadOnly ? SystemColors.Control : SystemColors.Window;
                    lsvEntries.LabelEdit = !isReadOnly;

                    cmbSearch.Text = "";
                    mnuEdit.Text = isReadOnly ? "View" : "Edit";
                    mnuEdit.ToolTipText = mnuEdit.Text + " (F4)";
                }

                RefreshCategories();
                RefreshItems();
                UpdateMenu();
                cmbSearch.Select();
            }
        }

        private DocumentResult LoadPasswordSafeFile(string fileName, string password = null) {
            try {
                Document document;
                using (var fileStream = File.OpenRead(fileName)) {
                    document = Document.Load(fileStream, password);
                    document.TrackAccess = false;

                    if (document.LastSaveApplication.StartsWith("Bimil ")) { //convert temporary password safe fields to permanent ones
                        foreach (var entry in document.Entries) {
                            if (entry.Records.Contains(RecordType.TemporaryBimilTwoFactorKey)) {
                                var buffer = new byte[1024];
                                int bytesLength;
                                try {
                                    OneTimePassword.FromBase32(entry[RecordType.TemporaryBimilTwoFactorKey].Text, buffer, out bytesLength);
                                    var bytes = new byte[bytesLength];
                                    try {
                                        Buffer.BlockCopy(buffer, 0, bytes, 0, bytes.Length);
                                        entry.TwoFactorKey = bytes;
                                    } finally {
                                        Array.Clear(bytes, 0, bytes.Length);
                                    }
                                } catch (FormatException) {
                                    Medo.MessageBox.ShowWarning(this, string.Format("Cannot convert 2-factor key {0} in entry {1}", entry[RecordType.TemporaryBimilTwoFactorKey].Text, entry.Title));
                                    if (entry.Notes.Length > 0) { entry.Notes += "\n"; }
                                    entry.Notes += "Two factor key: " + entry[RecordType.TemporaryBimilTwoFactorKey].Text;
                                } finally {
                                    Array.Clear(buffer, 0, buffer.Length);
                                }
                                entry[RecordType.TemporaryBimilTwoFactorKey] = null;
                            }
                            if (entry.Records.Contains(RecordType.TemporaryBimilCreditCardNumber)) {
                                entry.CreditCardNumber = entry[RecordType.TemporaryBimilCreditCardNumber].Text;
                                entry[RecordType.TemporaryBimilCreditCardNumber] = null;
                            }
                            if (entry.Records.Contains(RecordType.TemporaryBimilCreditCardExpiration)) {
                                entry.CreditCardExpiration = entry[RecordType.TemporaryBimilCreditCardExpiration].Text;
                                entry[RecordType.TemporaryBimilCreditCardExpiration] = null;
                            }
                            if (entry.Records.Contains(RecordType.TemporaryBimilCreditCardSecurityCode)) {
                                entry.CreditCardVerificationValue = entry[RecordType.TemporaryBimilCreditCardSecurityCode].Text;
                                entry[RecordType.TemporaryBimilCreditCardSecurityCode] = null;
                            }
                            if (entry.Records.Contains(RecordType.TemporaryBimilCreditCardPin)) {
                                entry.CreditCardPin = entry[RecordType.TemporaryBimilCreditCardPin].Text;
                                entry[RecordType.TemporaryBimilCreditCardPin] = null;
                            }
                        }
                    }
                }

                this.RecentFiles.Push(fileName);
                RefreshFiles();

                return new DocumentResult(document, fileName);
            } catch (FormatException) {
                return null;
            }
        }

        private DocumentResult LoadBimilFile(string fileName, string password = null) {
            try {
                var legacyDoc = LegacyFile.BimilDocument.Open(fileName, password);
                return new DocumentResult(DocumentConversion.ConvertFromBimil(legacyDoc, password), fileName);
            } catch (FormatException) {
                return null;
            }
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
            RefreshItems((lsvEntries.SelectedItems.Count > 0) ? (Entry)(lsvEntries.SelectedItems[0].Tag) : null);
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

                    //determine current category
                    string categoryText = cmbSearch.Text.Trim();
                    foreach (var category in this.Categories) {
                        if (category.Equals(categoryText, StringComparison.CurrentCultureIgnoreCase)) {
                            categoryText = category;
                            break;
                        }
                    }

                    using (var frm2 = new ItemForm(this.Document, entry, this.Categories, startsAsEditable: true, defaultCategory: categoryText)) {
                        if (frm2.ShowDialog(this) == DialogResult.OK) {
                            RefreshItems(entry);
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
            if ((this.Document == null) || (lsvEntries.SelectedItems.Count != 1)) { return; }

            var item = (Entry)(lsvEntries.SelectedItems[0].Tag);
            using (var frm2 = new ItemForm(this.Document, item, this.Categories, startsAsEditable: Settings.EditableByDefault)) {
                if (frm2.ShowDialog(this) == DialogResult.OK) {
                    lsvEntries.SelectedItems[0].Text = item.Title;
                    RefreshCategories();
                    RefreshItems(item);
                }
                UpdateMenu();
            }
        }

        private void mnuRemove_Click(object sender, EventArgs e) {
            if ((this.Document == null) || (lsvEntries.SelectedItems.Count == 0)) { return; }

            for (int i = lsvEntries.SelectedItems.Count - 1; i >= 0; i--) {
                var item = (Entry)(lsvEntries.SelectedItems[i].Tag);
                this.Document.Entries.Remove(item);
                lsvEntries.Items.Remove(lsvEntries.SelectedItems[i]);
            }
            UpdateMenu();
            cmbSearch.Select();
        }


        private void mnuSearch_Click(object sender, EventArgs e) {
            if (this.Document == null) { return; }

            using (var frm = new SearchForm(this.Document, this.Categories, cmbSearch.Text)) {
                frm.ShowDialog(this);
            }
        }

        private void mnuGeneratePassword_Click(object sender, EventArgs e) {
            using (var frm = new PasswordGeneratorForm()) {
                frm.ShowDialog(this);
            }
        }


        private void mnuAppOptions_Click(object sender, EventArgs e) {
            using (var frm = new SettingsForm()) {
                frm.ShowDialog(this);
            }
            cmbSearch.Select();
        }

        private void mnuAppFeedback_Click(object sender, EventArgs e) {
            Medo.Diagnostics.ErrorReport.ShowDialog(this, null, new Uri("https://medo64.com/feesdback/"));
            cmbSearch.Select();
        }

        private void mnuAppUpgrade_Click(object sender, EventArgs e) {
            Medo.Services.Upgrade.ShowDialog(this, new Uri("https://medo64.com/upgrade/"));
        }

        private void mnuAppAbout_Click(object sender, EventArgs e) {
            Medo.Windows.Forms.AboutBox.ShowDialog(this, new Uri("https://www.medo64.com/bimil/"));
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
                foreach (var entry in this.Document.Entries) {
                    var cached = new EntryCache(entry);
                    if (!this.Categories.Contains(cached.Group)) {
                        this.Categories.Add(cached.Group);
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

        private void RefreshItems(Entry entryToSelect = null) {
            Helpers.PerformEntrySearch(this.Document, lsvEntries, cmbSearch.Text, entryToSelect);
            Form_Resize(null, null); //to support both ListView full row with and without scrollbar
        }


        private void UpdateMenu() {
            mnuSave.Enabled = (this.Document != null) && (!this.Document.IsReadOnly);
            mnuChangePassword.Enabled = (this.Document != null) && (!this.Document.IsReadOnly);
            mnuAdd.Enabled = (this.Document != null) && (!this.Document.IsReadOnly);
            mnuEdit.Enabled = (this.Document != null) && (lsvEntries.SelectedItems.Count == 1);
            mnuRemove.Enabled = (this.Document != null) && (lsvEntries.SelectedItems.Count > 0) && (!this.Document.IsReadOnly);
            mnuSearch.Enabled = (this.Document != null);

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
                if (lsvEntries.Visible) {
                    lsvEntries.Select();
                } else {
                    this.Select();
                }
            } else {
                mnu.Select();
                mnu.Items[0].Select();
            }
        }


        #region Upgrade

        private void bwUpgradeCheck_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            e.Cancel = true;

            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < 3000) { //wait for three seconds
                Thread.Sleep(100);
                if (bwUpgradeCheck.CancellationPending) { return; }
            }

            var file = Medo.Services.Upgrade.GetUpgradeFile(new Uri("https://medo64.com/upgrade/"));
            if (file != null) {
                if (bwUpgradeCheck.CancellationPending) { return; }
                e.Cancel = false;
            }
        }

        private void bwUpgradeCheck_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
            if (!e.Cancelled) {
                Helpers.ScaleToolstripItem(mnuApp, "mnuAppUpgrade");
                mnuAppUpgrade.Text = "Upgrade is available";
            }
        }

        #endregion


        private class DocumentResult {

            public DocumentResult(Document document, string fileName) {
                this.Document = document;
                this.FileName = fileName;
            }


            public Document Document { get; }
            public String FileName { get; }

        }

    }
}
