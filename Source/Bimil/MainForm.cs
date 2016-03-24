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
            lsvPasswords.Font = SystemFonts.MessageBoxFont;

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
                    var fileName = item.Tag.ToString();
                    if (!File.Exists(fileName) || !LoadFile(fileName)) {
                        if (Medo.MessageBox.ShowQuestion(this, "File " + Path.GetFileName(fileName) + " could not be open.\nDo you wish to remove it from the recent list?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                            this.RecentFiles.Remove(fileName);
                            this.RefreshFiles();
                        }
                    }
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
                        if (mnuAdd.Enabled) { mnuAdd_Click(null, null); }
                        e.Handled = true;
                        e.SuppressKeyPress = true;
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
            using (var frm = new OpenFileDialog() { AddExtension = true, AutoUpgradeEnabled = true, Filter = "Bimil files|*.bimil|Password Safe files|*.psafe3|All files|*.*", RestoreDirectory = true, ShowReadOnly = true }) {
                if (frm.ShowDialog(this) == DialogResult.OK) {
                    LoadFile(frm.FileName, frm.ReadOnlyChecked);
                }
            }
        }

        private bool LoadFile(string fileName, bool isReadOnly = false) { //return false if file cannot be open
            string password;
            try {
                if (!File.Exists(fileName)) { return false; }

                while (true) {
                    using (var frm = new PasswordForm()) {
                        if (frm.ShowDialog(this) == DialogResult.OK) {
                            password = frm.Password;
                        } else {
                            return true; //don't signal file doesn't exist just because password has been canceled
                        }
                    }

                    if (fileName.EndsWith(".bimil", StringComparison.OrdinalIgnoreCase)) {
                        if (LoadBimilFile(fileName, password)) {
                            return true;
                        } else if (LoadPasswordSafeFile(fileName, password)) {
                            return true;
                        }
                    } else { //Password Safe
                        if (LoadPasswordSafeFile(fileName, password)) {
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

                if (this.Document != null) {
                    this.Document.IsReadOnly = isReadOnly;
                    cmbSearch.BackColor = isReadOnly ? SystemColors.Control : SystemColors.Window;
                    lsvPasswords.BackColor = isReadOnly ? SystemColors.Control : SystemColors.Window;
                    lsvPasswords.LabelEdit = !isReadOnly;

                    mnuEdit.Text = isReadOnly ? "View" : "Edit";
                    mnuEdit.ToolTipText = mnuEdit.Text + " (F4)";
                }

                RefreshCategories();
                RefreshItems();
                UpdateMenu();
                cmbSearch.Select();
            }
        }

        private bool LoadPasswordSafeFile(string fileName, string password = null) {
            try {
                using (var fileStream = File.OpenRead(fileName)) {
                    this.Document = Document.Load(fileStream, password);
                    this.Document.TrackAccess = false;
                    this.DocumentFileName = fileName;

                    if (this.Document.LastSaveApplication.StartsWith("Bimil ")) { //convert temporary password safe fields to permanent ones
                        foreach (var entry in this.Document.Entries) {
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

                return true;
            } catch (FormatException) {
                return false;
            }
        }

        private bool LoadBimilFile(string fileName, string password = null) {
            LegacyFile.BimilDocument legacyDoc = null;
            try {
                legacyDoc = LegacyFile.BimilDocument.Open(fileName, password);

                this.Document = DocumentConversion.ConvertFromBimil(legacyDoc, password);
                this.DocumentFileName = fileName;

                return true;
            } catch (FormatException) {
                return false;
            } finally {
                password = null;
                GC.Collect(); //in attempt to kill password string
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

                    using (var frm2 = new ItemForm(this.Document, entry, true, this.Categories)) {
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
            using (var frm2 = new ItemForm(this.Document, item, false, this.Categories)) {
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
            mnuSave.Enabled = (this.Document != null) && (!this.Document.IsReadOnly);
            mnuChangePassword.Enabled = (this.Document != null) && (!this.Document.IsReadOnly);
            mnuAdd.Enabled = (this.Document != null) && (!this.Document.IsReadOnly);
            mnuEdit.Enabled = (this.Document != null) && (lsvPasswords.SelectedItems.Count == 1);
            mnuRemove.Enabled = (this.Document != null) && (lsvPasswords.SelectedItems.Count > 0) && (!this.Document.IsReadOnly);

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

    }
}
