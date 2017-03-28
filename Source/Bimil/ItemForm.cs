using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Medo.Security.Cryptography.PasswordSafe;
using System.Collections.Generic;
using System.Web;
using Medo.Security.Cryptography;
using Medo.Windows.Forms;

namespace Bimil {
    internal partial class ItemForm : Form {

        private readonly Document Document;
        private readonly Entry Item;
        private bool Editable;
        private bool IsNew;
        private static Font FixedFont = new Font(FontFamily.GenericMonospace, SystemFonts.MessageBoxFont.SizeInPoints + 0.5F, SystemFonts.MessageBoxFont.Style);
        private static Font UnderlineFont = new Font(SystemFonts.MessageBoxFont.Name, SystemFonts.MessageBoxFont.SizeInPoints, SystemFonts.MessageBoxFont.Style | FontStyle.Underline);
        private readonly IList<string> Categories;
        private readonly string DefaultCategory;

        public ItemForm(Document document, Entry item, IList<string> categories, bool startsAsEditable, bool isNew = false, string defaultCategory = null) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;

            this.Document = document;
            this.Item = item;
            this.Editable = startsAsEditable && !this.Document.IsReadOnly;
            this.IsNew = isNew;
            this.Categories = categories;
            this.DefaultCategory = categories.Contains(defaultCategory) ? defaultCategory : null;

            btnEdit.Visible = !this.Document.IsReadOnly;
            btnAutotype.Visible = !Helpers.IsRunningOnMono; //filling form includes operations not supported under Linux - Mono under Windows might have some troubles too but it is disabled there because it enables easy testing :)

            this.Text = this.Document.IsReadOnly ? "View" : "Edit";
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


        private void Form_Load(object sender, EventArgs e) {
            if (this.Editable) {
                btnEdit_Click(null, null);
            }
            FillRecords();
        }

        private void Form_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            switch (e.KeyData) {
                case Keys.F2: e.IsInputKey = true; break;
            }
        }

        private void Form_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyData) {
                case Keys.F2: //edit, fields
                    if (btnEdit.Visible) {
                        btnEdit.PerformClick();
                    } else if (btnFields.Visible) {
                        btnFields.PerformClick();
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.F7: //show all
                    bool alreadyHidden = false;
                    foreach (var control in this.pnl.Controls) {
                        var textBox = control as TextBox;
                        if (textBox != null) {
                            var record = textBox.Tag as Record;
                            if ((record != null) && Helpers.GetIsHideable(record.RecordType) && (textBox.UseSystemPasswordChar)) { alreadyHidden = true; break; }
                        }
                    }
                    foreach (var control in this.pnl.Controls) {
                        var textBox = control as TextBox;
                        if (textBox != null) {
                            var record = textBox.Tag as Record;
                            if ((record != null) && Helpers.GetIsHideable(record.RecordType)) { textBox.UseSystemPasswordChar = !alreadyHidden; }
                        }
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Control | Keys.T: //fill
                    if (btnAutotype.Visible) { btnAutotype.PerformClick(); }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
            }
        }

        private void Form_Deactivate(object sender, EventArgs e) {
            if (Settings.AutoCloseItemTimeout > 0) {
                tmrClose.Interval = Settings.AutoCloseItemTimeout * 1000;
                tmrClose.Enabled = true;
            }
        }

        private void Form_Activated(object sender, EventArgs e) {
            tmrClose.Enabled = false;
        }


        private void tmrClose_Tick(object sender, EventArgs e) {
            foreach (var form in this.OwnedForms) {
                foreach (var innerForm in form.OwnedForms) {
                    innerForm.Close();
                }
                form.Close();
            }

            if (Settings.AutoCloseSave && btnOK.Visible && btnOK.Enabled) {
                btnOK.PerformClick();
            }
            this.Close();
        }


        private void FillRecords() {
            pnl.Visible = false;
            pnl.Controls.Clear();

            var unitHeight = (new TextBox() { Font = this.Font }).Height;
            var labelWidth = pnl.ClientSize.Width / 4;
            var labelBuffer = SystemInformation.VerticalScrollBarWidth + 1;

            int y = 0;
            TextBox titleTextBox;
            {
                var record = this.Item[RecordType.Title];
                titleTextBox = new TextBox() { Font = this.Font, Location = new Point(labelWidth + labelBuffer, 0), Tag = record, Text = record.ToString(), Width = pnl.ClientSize.Width - labelWidth - labelBuffer, ReadOnly = !this.Editable, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
                titleTextBox.GotFocus += (object sender2, EventArgs e2) => { ((TextBox)sender2).SelectAll(); };
                titleTextBox.TextChanged += (object sender2, EventArgs e2) => { btnOK.Enabled = (((Control)sender2).Text.Trim().Length > 0); };
                pnl.Controls.Add(titleTextBox);
                var label = new Label() { AutoEllipsis = true, Location = new Point(0, y), Size = new Size(labelWidth, unitHeight), Text = "Name:", TextAlign = ContentAlignment.MiddleLeft, UseMnemonic = false };
                pnl.Controls.Add(label);

                y += titleTextBox.Height + (label.Height / 4);
            }

            ComboBox categoryComboBox;
            {
                var record = this.Item[RecordType.Group];
                categoryComboBox = new ComboBox() { Font = this.Font, Location = new Point(labelWidth + labelBuffer, y), Tag = record, Text = record.ToString(), Width = pnl.ClientSize.Width - labelWidth - labelBuffer, Enabled = this.Editable, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
                categoryComboBox.GotFocus += (object sender2, EventArgs e2) => { ((ComboBox)sender2).SelectAll(); };
                foreach (var category in this.Categories) {
                    categoryComboBox.Items.Add(category);
                }
                if (this.DefaultCategory != null) { categoryComboBox.Text = this.DefaultCategory; }
                pnl.Controls.Add(categoryComboBox);
                var label = new Label() { AutoEllipsis = true, Location = new Point(0, y), Size = new Size(labelWidth, unitHeight), Text = "Category:", TextAlign = ContentAlignment.MiddleLeft, UseMnemonic = false };
                pnl.Controls.Add(label);

                if (!string.IsNullOrEmpty(record.Text)) {
                    var renameButton = NewCustomCommandButton(categoryComboBox, delegate {
                        using (var frm = new InputBox($"Rename all categories named \"{record.Text}\" to:", categoryComboBox.Text)) {
                            if (frm.ShowDialog(this) == DialogResult.OK) {
                                var groupFrom = this.Item.Group;
                                var groupTo = frm.Text;
                                foreach (var entry in this.Document.Entries) {
                                    if (string.Equals(groupFrom, entry.Group, StringComparison.CurrentCultureIgnoreCase)) {
                                        entry.Group = groupTo;
                                    }
                                }
                                categoryComboBox.Text = groupTo;
                            }
                        }
                    }, "Rename group", "btnGroupRename");
                    pnl.Controls.Add(renameButton);
                    renameButton.Enabled = categoryComboBox.Enabled;

                    btnEdit.Click += delegate {
                        renameButton.Enabled = categoryComboBox.Enabled;
                    };
                }

                y += titleTextBox.Height + (label.Height / 4);
            }

            y += unitHeight / 2;

            int yH;
            foreach (var record in this.Item.Records) {
                var label = new Label() { AutoEllipsis = true, Location = new Point(0, y), Size = new Size(labelWidth, unitHeight), Text = Helpers.GetRecordCaption(record) + ":", TextAlign = ContentAlignment.MiddleLeft, UseMnemonic = false };

                switch (record.RecordType) {
                    case RecordType.Uuid:
                    case RecordType.Group:
                    case RecordType.Title:
                    case RecordType.CreationTime:
                    case RecordType.LastAccessTime:
                    case RecordType.LastModificationTime:
                    case RecordType.PasswordExpiryTime:
                    case RecordType.PasswordModificationTime:
                    case RecordType.PasswordHistory:
                        continue;

                    case RecordType.UserName:
                    case RecordType.CreditCardExpiration: {
                            var textBox = NewTextBox(labelWidth, y, record);
                            pnl.Controls.Add(textBox);

                            pnl.Controls.Add(NewCopyButton(textBox));

                            yH = textBox.Height;
                        }
                        break;

                    case RecordType.CreditCardVerificationValue:
                    case RecordType.CreditCardPin: {
                            var textBox = NewTextBox(labelWidth, y, record);
                            textBox.UseSystemPasswordChar = true;
                            pnl.Controls.Add(textBox);

                            pnl.Controls.Add(NewCopyButton(textBox));
                            pnl.Controls.Add(NewShowPasswordButton(textBox));

                            yH = textBox.Height;
                        }
                        break;

                    case RecordType.Password: {
                            var textBox = NewTextBox(labelWidth, y, record);
                            textBox.UseSystemPasswordChar = true;
                            pnl.Controls.Add(textBox);

                            pnl.Controls.Add(NewCopyButton(textBox));
                            pnl.Controls.Add(NewShowPasswordButton(textBox));
                            pnl.Controls.Add(NewConfigureButton(textBox, delegate {
                                using (var frm = new PasswordDetailsForm(this.Item, textBox.ReadOnly)) {
                                    frm.ShowDialog(this);
                                }
                            }, "Password policy configuration."));
                            pnl.Controls.Add(NewGeneratePasswordButton(textBox));

                            void showPasswordWarnings()
                            {
                                if (BadPasswords.IsCommon(textBox.Text, out var matchedPassword)) {
                                    erp.SetIconAlignment(textBox, ErrorIconAlignment.MiddleLeft);
                                    erp.SetIconPadding(textBox, SystemInformation.BorderSize.Width);
                                    erp.SetError(textBox, $"Password is similar to a common password ({matchedPassword}).");
                                } else {
                                    erp.SetError(textBox, null);
                                }
                            }

                            showPasswordWarnings();
                            textBox.TextChanged += delegate { showPasswordWarnings(); };

                            yH = textBox.Height;
                        }
                        break;

                    case RecordType.Url: {
                            var textBox = NewTextBox(labelWidth, y, record, urlLookAndFeel: true);
                            pnl.Controls.Add(textBox);

                            pnl.Controls.Add(NewCopyButton(textBox, copyText: delegate { return Execute.NormalizeUrl(textBox.Text); }));
                            pnl.Controls.Add(NewExecuteUrlButton(textBox));
                            pnl.Controls.Add(NewExecuteUrlQRButton(textBox));

                            yH = textBox.Height;
                        }
                        break;

                    case RecordType.EmailAddress: {
                            var textBox = NewTextBox(labelWidth, y, record, urlLookAndFeel: true);
                            pnl.Controls.Add(textBox);

                            pnl.Controls.Add(NewCopyButton(textBox));
                            pnl.Controls.Add(NewExecuteEmailButton(textBox));

                            yH = textBox.Height;
                        }
                        break;

                    case RecordType.Notes: {
                            var textBox = NewTextBox(labelWidth, y, record, multiline: true);
                            pnl.Controls.Add(textBox);

                            yH = textBox.Height;
                        }
                        break;

                    case RecordType.TwoFactorKey: {
                            var bytes = record.GetBytes();
                            var textBox = NewTextBox(labelWidth, y, record, text: OneTimePassword.ToBase32(bytes, bytes.Length, SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
                            textBox.UseSystemPasswordChar = true;
                            pnl.Controls.Add(textBox);
                            Array.Clear(bytes, 0, bytes.Length);

                            pnl.Controls.Add(NewCopyButton(textBox, tipText: "Copy two-factor key to clipboard.", copyText: delegate { return Helpers.GetTwoFactorCode(textBox.Text); }));
                            pnl.Controls.Add(NewViewTwoFactorCode(textBox));
                            pnl.Controls.Add(NewExecuteOAuthQRButton(textBox));
                            pnl.Controls.Add(NewShowPasswordButton(textBox, tipText: "Show two-factor key."));

                            yH = textBox.Height;
                        }
                        break;

                    case RecordType.CreditCardNumber: {
                            var textBox = NewTextBox(labelWidth, y, record);
                            pnl.Controls.Add(textBox);

                            pnl.Controls.Add(NewCopyButton(textBox, allowedCopyCharacters: Helpers.NumberCharacters));

                            yH = textBox.Height;
                        }
                        break;


                    case RecordType.QRCode: {
                            var textBox = NewTextBox(labelWidth, y, record);
                            pnl.Controls.Add(textBox);

                            pnl.Controls.Add(NewExecuteQRButton(textBox));

                            yH = textBox.Height;
                        }
                        break;

                    case RecordType.Autotype: {
                            var textBox = NewTextBox(labelWidth, y, record);
                            pnl.Controls.Add(textBox);

                            pnl.Controls.Add(NewConfigureButton(textBox, delegate {
                                using (var frm = new AutotypeHelpForm(textBox.Text, textBox.ReadOnly)) {
                                    if (frm.ShowDialog(this) == DialogResult.OK) {
                                        if (!textBox.ReadOnly) {
                                            textBox.Text = frm.AutotypeText;
                                        }
                                    }
                                }
                            }, "Auto-type configuration."));

                            yH = textBox.Height;
                        }
                        break;

                    case RecordType.RunCommand: {
                            var textBox = NewTextBox(labelWidth, y, record);
                            pnl.Controls.Add(textBox);

                            pnl.Controls.Add(NewExecuteCommandButton(textBox));

                            yH = textBox.Height;
                        }
                        break;

                    default:
                        yH = label.Height;
                        break;
                }

                pnl.Controls.Add(label);

                y += yH + (label.Height / 4);
            }

            if (pnl.VerticalScroll.Visible == true) {
                foreach (Control control in pnl.Controls) {
                    var label = control as Label;
                    if (label == null) {
                        control.Left -= SystemInformation.VerticalScrollBarWidth;
                    }
                }
            }

            pnl.Visible = true;

            if (this.IsNew) {
                titleTextBox.Select();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e) {
            foreach (Control control in pnl.Controls) {
                var textBox = control as TextBox;
                if (textBox != null) {
                    textBox.ReadOnly = false;
                }
                var comboBox = control as ComboBox;
                if (comboBox != null) {
                    comboBox.Enabled = true;
                }
            }
            btnFields.Visible = true;
            btnEdit.Visible = false;
            btnOK.Visible = true;
            btnCancel.Text = "Cancel";
            this.Editable = true;
        }

        private void btnAutotype_Click(object sender, EventArgs e) {
            var originalState = this.Owner.WindowState;
            var ownerForm = this.Owner;

            if (btnOK.Visible && btnOK.Enabled) {
                btnOK.PerformClick(); //save any changes
            }

            this.Close();

            var frm = new AutotypeForm(this.Item);
            frm.Shown += delegate {
                ownerForm.Visible = false;
            };
            frm.FormClosed += delegate {
                ownerForm.Visible = true;
                ownerForm.Select();
            };
            frm.Show();
        }

        private void btnOK_Click(object sender, EventArgs e) {
            foreach (Control control in pnl.Controls) {
                var record = control.Tag as Record;
                if (record != null) {
                    if (record.RecordType == RecordType.TwoFactorKey) {
                        var buffer = new byte[1024];
                        int bytesLength;
                        try {
                            OneTimePassword.FromBase32(control.Text, buffer, out bytesLength);
                            var bytes = new byte[bytesLength];
                            try {
                                Buffer.BlockCopy(buffer, 0, bytes, 0, bytes.Length);
                                record.SetBytes(bytes);
                            } finally {
                                Array.Clear(bytes, 0, bytes.Length);
                            }
                        } catch (FormatException) {
                            Medo.MessageBox.ShowWarning(this, string.Format("2-factor key {0} is not a valid base-32 string.", control.Text));
                        } finally {
                            Array.Clear(buffer, 0, buffer.Length);
                        }
                    } else {
                        if (!string.Equals(record.Text, control.Text, StringComparison.Ordinal)) {
                            record.Text = control.Text;
                        }
                    }
                }
            }
        }

        private void btnFields_Click(object sender, EventArgs e) {
            btnOK_Click(null, null);
            using (var frm = new RecordEditorForm(this.Document, this.Item)) {
                if (frm.ShowDialog(this) == DialogResult.OK) {
                    FillRecords();
                }
            }
        }


        #region Controls

        private TextBox NewTextBox(int x, int y, Record record, string text = null, bool urlLookAndFeel = false, bool multiline = false) {
            var padding = SystemInformation.VerticalScrollBarWidth + 1;

            var textBox = new TextBoxEx() { Font = this.Font, Location = new Point(x + padding, y), Tag = record, Width = pnl.ClientSize.Width - x - padding, ReadOnly = !this.Editable, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            textBox.Text = (text != null) ? text : record.Text;

            if (urlLookAndFeel) {
                textBox.Font = ItemForm.UnderlineFont;
                textBox.ForeColor = SystemColors.HotTrack;
            }

            if (multiline) {
                textBox.Multiline = true;
                textBox.Height *= 3;
                textBox.AcceptsReturn = true;
                textBox.ScrollBars = ScrollBars.Vertical;
            }

            textBox.GotFocus += (object sender, EventArgs e) => {
                ((TextBox)sender).SelectAll();
            };

            textBox.KeyDown += (object sender, KeyEventArgs e) => {
                switch (e.KeyData) {
                    case Keys.Control | Keys.A: ((TextBox)sender).SelectAll(); break;
                }
            };

            return textBox;
        }

        private ComboBox NewPasswordHistoryComboBox(int x, int y, Entry entry) {
            var padding = SystemInformation.VerticalScrollBarWidth + 1;

            var control = new ComboBox() { BackColor = SystemColors.Control, Font = this.Font, Location = new Point(x + padding, y), DropDownStyle = ComboBoxStyle.DropDownList, Width = pnl.ClientSize.Width - x - padding, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            control.Items.Add("Open dropdown to show password history (" + entry.PasswordHistory.Count.ToString(CultureInfo.CurrentCulture) + ")");
            control.SelectedIndex = 0;

            control.DropDownClosed += delegate {
                control.Items.Clear();
                control.Items.Add("Open dropdown to show password history (" + entry.PasswordHistory.Count.ToString(CultureInfo.CurrentCulture) + ")");
                control.SelectedIndex = 0;
            };

            control.DropDown += delegate {
                control.ForeColor = SystemColors.ControlText;
                control.Items.Clear();
                if (entry.PasswordHistory.Count == 0) {
                    control.Items.Add("No passwords in history");
                } else {
                    foreach (var item in entry.PasswordHistory) {
                        var timeString = item.TimeFirstUsed.ToShortDateString() + " " + item.TimeFirstUsed.ToShortTimeString();
                        control.Items.Insert(0, timeString + ": " + item.HistoricalPassword);
                    }
                }
            };

            control.KeyDown += (object sender, KeyEventArgs e) => {
                switch (e.KeyData) {
                    case Keys.Down: control.DroppedDown = true; break;
                }
            };

            return control;
        }

        private Button NewCopyButton(TextBox parentTextBox, string tipText = null, char[] allowedCopyCharacters = null, GetText copyText = null) {
            parentTextBox.Width -= parentTextBox.Height;
            var button = new Button() {
                Name = "btnCopy",
                Location = new Point(parentTextBox.Right, parentTextBox.Top),
                Size = new Size(parentTextBox.Height, parentTextBox.Height),
                TabStop = false,
                Tag = parentTextBox,
                Text = "",
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            Helpers.ScaleButton(button);

            tip.SetToolTip(button, (tipText != null) ? tipText : "Copy to clipboard.");

            button.Click += (object sender, EventArgs e) => {
                var textBox = (TextBox)(((Control)sender).Tag);
                textBox.Select();

                string text;
                if (copyText != null) {
                    text = copyText.Invoke();
                } else {
                    text = textBox.Text;
                }
                text = Helpers.FilterText(text, allowedCopyCharacters);

                Execute.ClipboardCopyText(text);
            };

            return button;
        }

        private Button NewShowPasswordButton(TextBox parentTextBox, string tipText = null) {
            parentTextBox.Width -= parentTextBox.Height;
            var button = new Button() {
                Name = "btnViewPassword",
                Location = new Point(parentTextBox.Right, parentTextBox.Top),
                Size = new Size(parentTextBox.Height, parentTextBox.Height),
                TabStop = false,
                Tag = parentTextBox,
                Text = "",
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            Helpers.ScaleButton(button);

            tip.SetToolTip(button, (tipText != null) ? tipText : "Show password.");

            button.Click += (object sender, EventArgs e) => {
                var textBox = (TextBox)(((Control)sender).Tag);
                textBox.Select();

                textBox.UseSystemPasswordChar = !textBox.UseSystemPasswordChar;
            };

            return button;
        }

        private Button NewGeneratePasswordButton(TextBox parentTextBox) {
            parentTextBox.Width -= parentTextBox.Height;
            var button = new Button() {
                Name = "mnuGeneratePassword",
                Location = new Point(parentTextBox.Right, parentTextBox.Top),
                Size = new Size(parentTextBox.Height, parentTextBox.Height),
                TabStop = false,
                Tag = parentTextBox,
                Text = "",
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Enabled = this.Editable
            };
            Helpers.ScaleButton(button);

            tip.SetToolTip(button, "Generate password (available only when no password already present).");

            parentTextBox.ReadOnlyChanged += delegate {
                button.Enabled = !parentTextBox.ReadOnly;
            };

            button.Click += (object sender, EventArgs e) => {
                var textBox = (TextBox)(((Control)sender).Tag);
                textBox.Select();

                bool generateNew = true;
                if (textBox.Text.Length > 0) {
                    generateNew = Medo.MessageBox.ShowQuestion(this, "Do you wish to overwrite current password?", MessageBoxButtons.YesNo) == DialogResult.Yes;
                }

                if (generateNew) {
                    using (var frm = new PasswordGeneratorForm(useCopyAsSave: true)) {
                        if (frm.ShowDialog(this) == DialogResult.OK) {
                            textBox.Text = frm.Password;
                        }
                    }
                }
            };

            return button;
        }

        private Button NewExecuteUrlButton(TextBox parentTextBox) {
            parentTextBox.Width -= parentTextBox.Height;
            var button = new Button() {
                Name = "btnExecuteUrl",
                Location = new Point(parentTextBox.Right, parentTextBox.Top),
                Size = new Size(parentTextBox.Height, parentTextBox.Height),
                TabStop = false,
                Tag = parentTextBox,
                Text = "",
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            Helpers.ScaleButton(button);

            tip.SetToolTip(button, "Go to URL.");

            button.Click += (object sender, EventArgs e) => {
                var textBox = (TextBox)(((Control)sender).Tag);
                textBox.Select();

                Execute.StartUrl(textBox.Text);
            };

            return button;
        }

        private Button NewExecuteUrlQRButton(TextBox parentTextBox) {
            parentTextBox.Width -= parentTextBox.Height;
            var button = new Button() {
                Name = "btnExecuteQR",
                Location = new Point(parentTextBox.Right, parentTextBox.Top),
                Size = new Size(parentTextBox.Height, parentTextBox.Height),
                TabStop = false,
                Tag = parentTextBox,
                Text = "",
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            Helpers.ScaleButton(button);

            tip.SetToolTip(button, "Create URL QR code.");

            button.Click += (object sender, EventArgs e) => {
                var textBox = (TextBox)(((Control)sender).Tag);
                textBox.Select();

                var url = Execute.NormalizeUrl(textBox.Text);
                if (url != "") {
                    using (var frm = new QRCodeForm(url)) {
                        frm.ShowDialog(this);
                    }
                }
            };

            return button;
        }

        private Button NewExecuteEmailButton(TextBox parentTextBox) {
            parentTextBox.Width -= parentTextBox.Height;
            var button = new Button() {
                Name = "btnExecuteEmail",
                Location = new Point(parentTextBox.Right, parentTextBox.Top),
                Size = new Size(parentTextBox.Height, parentTextBox.Height),
                TabStop = false,
                Tag = parentTextBox,
                Text = "",
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            Helpers.ScaleButton(button);

            tip.SetToolTip(button, "E-mail.");

            button.Click += (object sender, EventArgs e) => {
                var textBox = (TextBox)(((Control)sender).Tag);
                textBox.Select();

                var email = GetEmailUrl(textBox.Text);
                if (email != "") { Process.Start(email); }
            };

            return button;
        }

        private Button NewExecuteOAuthQRButton(TextBox parentTextBox) {
            parentTextBox.Width -= parentTextBox.Height;
            var button = new Button() {
                Name = "btnExecuteQR",
                Location = new Point(parentTextBox.Right, parentTextBox.Top),
                Size = new Size(parentTextBox.Height, parentTextBox.Height),
                TabStop = false,
                Tag = parentTextBox,
                Text = "",
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            Helpers.ScaleButton(button);

            tip.SetToolTip(button, "Create QR code.");

            button.Click += (object sender, EventArgs e) => {
                var textBox = (TextBox)(((Control)sender).Tag);
                textBox.Select();

                var key = Helpers.FilterText(textBox.Text.ToUpperInvariant(), Helpers.Base32Characters);
                if (key.Length > 0) {
                    var keyUrl = string.Format(CultureInfo.InvariantCulture, "otpauth://totp/{0}?secret={1}", HttpUtility.UrlPathEncode(this.Item.Title), HttpUtility.UrlEncode(key));
                    using (var frm = new QRCodeForm(keyUrl)) {
                        frm.ShowDialog(this);
                    }
                }
            };

            return button;
        }

        private Button NewViewTwoFactorCode(TextBox parentTextBox) {
            parentTextBox.Width -= parentTextBox.Height;
            var button = new Button() {
                Name = "btnViewCode",
                Location = new Point(parentTextBox.Right, parentTextBox.Top),
                Size = new Size(parentTextBox.Height, parentTextBox.Height),
                TabStop = false,
                Tag = parentTextBox,
                Text = "",
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            Helpers.ScaleButton(button);

            tip.SetToolTip(button, "View two-factor code.");

            button.Click += (object sender, EventArgs e) => {
                var textBox = (TextBox)(((Control)sender).Tag);
                textBox.Select();

                var code = Helpers.GetTwoFactorCode(textBox.Text, space: true);
                if (code != "") { Medo.MessageBox.ShowInformation(this, code); }
            };

            return button;
        }

        private Button NewExecuteQRButton(TextBox parentTextBox) {
            parentTextBox.Width -= parentTextBox.Height;
            var button = new Button() {
                Name = "btnExecuteQR",
                Location = new Point(parentTextBox.Right, parentTextBox.Top),
                Size = new Size(parentTextBox.Height, parentTextBox.Height),
                TabStop = false,
                Tag = parentTextBox,
                Text = "",
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            Helpers.ScaleButton(button);

            tip.SetToolTip(button, "Shows QR code.");

            button.Click += (object sender, EventArgs e) => {
                var textBox = (TextBox)(((Control)sender).Tag);
                textBox.Select();

                var content = textBox.Text;
                if (content != "") {
                    using (var frm = new QRCodeForm(content)) {
                        frm.ShowDialog(this);
                    }
                }
            };

            return button;
        }

        private Button NewConfigureButton(TextBox parentTextBox, EventHandler clickHandler, string tipText = null, bool trackReadonly = false) {
            var button = NewCustomCommandButton(parentTextBox, clickHandler, tipText, "btnConfigure");

            if (trackReadonly) {
                button.Enabled = !parentTextBox.ReadOnly;
                parentTextBox.ReadOnlyChanged += delegate {
                    button.Enabled = !parentTextBox.ReadOnly;
                };
            }

            return button;
        }

        private Button NewCustomCommandButton(Control parentControl, EventHandler clickHandler, string tipText = null, string buttonName = "btnCustom") {
            parentControl.Width -= parentControl.Height;
            var button = new Button() {
                Name = buttonName,
                Location = new Point(parentControl.Right, parentControl.Top),
                Size = new Size(parentControl.Height, parentControl.Height),
                TabStop = false,
                Tag = parentControl,
                Text = "",
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            Helpers.ScaleButton(button);

            tip.SetToolTip(button, tipText ?? "Command");

            button.Click += (object sender, EventArgs e) => {
                clickHandler.Invoke(sender, e);
            };

            return button;
        }

        private Button NewExecuteCommandButton(TextBox parentTextBox) {
            parentTextBox.Width -= parentTextBox.Height;
            var button = new Button() {
                Name = "btnExecute",
                Location = new Point(parentTextBox.Right, parentTextBox.Top),
                Size = new Size(parentTextBox.Height, parentTextBox.Height),
                TabStop = false,
                Tag = parentTextBox,
                Text = "",
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            Helpers.ScaleButton(button);

            tip.SetToolTip(button, "Execute command");

            button.Click += (object sender, EventArgs e) => {
                Execute.StartCommand(((TextBox)button.Tag).Text, this);
            };

            return button;
        }

        private string GetEmailUrl(string text) {
            var email = text.Trim();
            if (email.Length > 0) {
                return email.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase) ? email : ("mailto:" + email);
            } else {
                return "";
            }
        }

        private delegate string GetText();

        #endregion

    }
}
