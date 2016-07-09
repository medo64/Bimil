using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Medo.Security.Cryptography.PasswordSafe;
using System.Collections.Generic;
using System.Web;
using Medo.Security.Cryptography;
using System.Text;

namespace Bimil {
    internal partial class ItemForm : Form {

        private readonly Document Document;
        private readonly Entry Item;
        private bool Editable;
        private static Font FixedFont = new Font(FontFamily.GenericMonospace, SystemFonts.MessageBoxFont.SizeInPoints + 0.5F, SystemFonts.MessageBoxFont.Style);
        private static Font UnderlineFont = new Font(SystemFonts.MessageBoxFont.Name, SystemFonts.MessageBoxFont.SizeInPoints, SystemFonts.MessageBoxFont.Style | FontStyle.Underline);
        private readonly IList<string> Categories;

        public ItemForm(Document document, Entry item, IList<string> categories, bool startsAsEditable) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;

            this.Document = document;
            this.Item = item;
            this.Editable = startsAsEditable && !this.Document.IsReadOnly;
            this.Categories = categories;

            btnEdit.Visible = !this.Document.IsReadOnly;

            this.Text = this.Document.IsReadOnly ? "View" : "Edit";
        }

        private void EditItemForm_Load(object sender, EventArgs e) {
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
                        btnEdit_Click(null, null);
                    } else if (btnFields.Visible) {
                        btnFields_Click(null, null);
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
                titleTextBox.GotFocus += new EventHandler(delegate (object sender2, EventArgs e2) { ((TextBox)sender2).SelectAll(); });
                titleTextBox.TextChanged += new EventHandler(delegate (object sender2, EventArgs e2) { btnOK.Enabled = (((Control)sender2).Text.Trim().Length > 0); });
                pnl.Controls.Add(titleTextBox);
                var label = new Label() { AutoEllipsis = true, Location = new Point(0, y), Size = new Size(labelWidth, unitHeight), Text = "Name:", TextAlign = ContentAlignment.MiddleLeft, UseMnemonic = false };
                pnl.Controls.Add(label);

                y += titleTextBox.Height + (label.Height / 4);
            }

            ComboBox categoryComboBox;
            {
                var record = this.Item[RecordType.Group];
                categoryComboBox = new ComboBox() { Font = this.Font, Location = new Point(labelWidth + labelBuffer, y), Tag = record, Text = record.ToString(), Width = pnl.ClientSize.Width - labelWidth - labelBuffer, Enabled = this.Editable, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
                categoryComboBox.GotFocus += new EventHandler(delegate (object sender2, EventArgs e2) { ((ComboBox)sender2).SelectAll(); });
                foreach (var category in this.Categories) {
                    categoryComboBox.Items.Add(category);
                }
                pnl.Controls.Add(categoryComboBox);
                var label = new Label() { AutoEllipsis = true, Location = new Point(0, y), Size = new Size(labelWidth, unitHeight), Text = "Category:", TextAlign = ContentAlignment.MiddleLeft, UseMnemonic = false };
                pnl.Controls.Add(label);

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
                            pnl.Controls.Add(NewGeneratePasswordButton(textBox));

                            yH = textBox.Height;
                        }
                        break;

                    case RecordType.PasswordHistory: {
                            var control = NewPasswordHistoryComboBox(labelWidth, y, this.Item);
                            pnl.Controls.Add(control);

                            yH = control.Height;
                        }
                        break;

                    case RecordType.Url: {
                            var textBox = NewTextBox(labelWidth, y, record, urlLookAndFeel: true);
                            pnl.Controls.Add(textBox);

                            pnl.Controls.Add(NewCopyButton(textBox, copyText: delegate () { return GetUrl(textBox.Text); }));
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

                            pnl.Controls.Add(NewCopyButton(textBox, tipText: "Copy two-factor key to clipboard.", copyText: delegate () { return GetTwoFactorCode(textBox.Text); }));
                            pnl.Controls.Add(NewViewTwoFactorCode(textBox));
                            pnl.Controls.Add(NewExecuteQRButton(textBox));
                            pnl.Controls.Add(NewShowPasswordButton(textBox, tipText: "Show two-factor key."));

                            yH = textBox.Height;
                        }
                        break;

                    case RecordType.CreditCardNumber: {
                            var textBox = NewTextBox(labelWidth, y, record);
                            pnl.Controls.Add(textBox);

                            pnl.Controls.Add(NewCopyButton(textBox, allowedCopyCharacters: NumberCharacters));

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

            var textBox = new TextBox() { Font = this.Font, Location = new Point(x + padding, y), Tag = record, Width = pnl.ClientSize.Width - x - padding, ReadOnly = !this.Editable, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
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

            textBox.GotFocus += new EventHandler(delegate (object sender, EventArgs e) {
                ((TextBox)sender).SelectAll();
            });

            textBox.KeyDown += new KeyEventHandler(delegate (object sender, KeyEventArgs e) {
                switch (e.KeyData) {
                    case Keys.Control | Keys.A: ((TextBox)sender).SelectAll(); break;
                }
            });

            return textBox;
        }

        private ComboBox NewPasswordHistoryComboBox(int x, int y, Entry entry) {
            var padding = SystemInformation.VerticalScrollBarWidth + 1;

            var control = new ComboBox() { BackColor = SystemColors.Control, Font = this.Font, Location = new Point(x + padding, y), DropDownStyle = ComboBoxStyle.DropDownList, Width = pnl.ClientSize.Width - x - padding, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            control.Items.Add("Open dropdown to show passwords");
            control.SelectedIndex = 0;

            control.DropDownClosed += new EventHandler(delegate (object sender, EventArgs e) {
                control.Items.Clear();
                control.Items.Add("Open dropdown to show password history");
                control.SelectedIndex = 0;
            });

            control.DropDown += new EventHandler(delegate (object sender, EventArgs e) {
                control.ForeColor = SystemColors.ControlText;
                control.Items.Clear();
                if (entry.PasswordHistory.Count == 0) {
                    control.Items.Add("No password history");
                } else {
                    foreach (var item in entry.PasswordHistory) {
                        var timeString = item.TimeFirstUsed.ToShortDateString() + " " + item.TimeFirstUsed.ToShortTimeString();
                        control.Items.Add(timeString + ": " + item.HistoricalPassword);
                    }
                }
            });

            control.KeyDown += new KeyEventHandler(delegate (object sender, KeyEventArgs e) {
                switch (e.KeyData) {
                    case Keys.Down: control.DroppedDown = true; break;
                }
            });

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

            button.Click += new EventHandler(delegate (object sender, EventArgs e) {
                var textBox = (TextBox)(((Control)sender).Tag);
                textBox.Select();

                string text;
                if (copyText != null) {
                    text = copyText.Invoke();
                } else {
                    text = textBox.Text;
                }
                text = FilterText(text, allowedCopyCharacters);

                Clipboard.Clear();
                if (text.Length > 0) {
                    Clipboard.SetText(text);
                }
            });

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

            button.Click += new EventHandler(delegate (object sender, EventArgs e) {
                var textBox = (TextBox)(((Control)sender).Tag);
                textBox.Select();

                textBox.UseSystemPasswordChar = !textBox.UseSystemPasswordChar;
            });

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
                Enabled = (parentTextBox.TextLength == 0)
            };
            Helpers.ScaleButton(button);

            tip.SetToolTip(button, "Generate password (available only when no password already present).");

            parentTextBox.TextChanged += new EventHandler(delegate (object sender, EventArgs e) {
                var textBox = (TextBox)sender;
                button.Enabled = (textBox.TextLength == 0);
            });

            button.Click += new EventHandler(delegate (object sender, EventArgs e) {
                var textBox = (TextBox)(((Control)sender).Tag);
                textBox.Select();

                using (var frm = new PasswordGeneratorForm(useCopyAsSave: true)) {
                    if (frm.ShowDialog(this) == DialogResult.OK) {
                        textBox.Text = frm.Password;
                    }
                }
            });

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

            button.Click += new EventHandler(delegate (object sender, EventArgs e) {
                var textBox = (TextBox)(((Control)sender).Tag);
                textBox.Select();

                var url = GetUrl(textBox.Text);
                if (url != "") { Process.Start(url); }
            });

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

            button.Click += new EventHandler(delegate (object sender, EventArgs e) {
                var textBox = (TextBox)(((Control)sender).Tag);
                textBox.Select();

                var url = GetUrl(textBox.Text);
                if (url != "") {
                    using (var frm = new QRCodeForm(url)) {
                        frm.ShowDialog(this);
                    }
                }
            });

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

            button.Click += new EventHandler(delegate (object sender, EventArgs e) {
                var textBox = (TextBox)(((Control)sender).Tag);
                textBox.Select();

                var email = GetEmailUrl(textBox.Text);
                if (email != "") { Process.Start(email); }
            });


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

            tip.SetToolTip(button, "Create QR code.");

            button.Click += new EventHandler(delegate (object sender, EventArgs e) {
                var textBox = (TextBox)(((Control)sender).Tag);
                textBox.Select();

                var key = FilterText(textBox.Text.ToUpperInvariant(), Base32Characters);
                if (key.Length > 0) {
                    var keyUrl = string.Format(CultureInfo.InvariantCulture, "otpauth://totp/{0}?secret={1}", HttpUtility.UrlPathEncode(this.Item.Title), HttpUtility.UrlEncode(key));
                    using (var frm = new QRCodeForm(keyUrl)) {
                        frm.ShowDialog(this);
                    }
                }
            });

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

            button.Click += new EventHandler(delegate (object sender, EventArgs e) {
                var textBox = (TextBox)(((Control)sender).Tag);
                textBox.Select();

                var code = GetTwoFactorCode(textBox.Text, space: true);
                if (code != "") { Medo.MessageBox.ShowInformation(this, code); }
            });

            return button;
        }


        private string GetUrl(string text) {
            var url = text.Trim();
            if (url.Length > 0) {
                return (url.IndexOf("://", StringComparison.OrdinalIgnoreCase) > 0) ? url : ("http:" + url);
            } else {
                return "";
            }
        }

        private string GetEmailUrl(string text) {
            var email = text.Trim();
            if (email.Length > 0) {
                return email.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase) ? email : ("mailto:" + email);
            } else {
                return "";
            }
        }

        private string GetTwoFactorCode(string text, bool space = false) {
            var key = FilterText(text.ToUpperInvariant(), Base32Characters);
            if (key.Length > 0) {
                try {
                    var otp = new OneTimePassword(key);
                    var code = otp.GetCode().ToString(new string('0', otp.Digits), CultureInfo.InvariantCulture);
                    if (space) {
                        var mid = code.Length / 2;
                        return code.Substring(0, mid) + " " + code.Substring(mid);
                    } else {
                        return code;
                    }
                } catch (ArgumentException) { }
            }
            return "";
        }


        private static string FilterText(string text, char[] allowedCopyCharacters) {
            if (allowedCopyCharacters != null) {
                var allowedCharacters = new List<char>(allowedCopyCharacters);
                var sb = new StringBuilder();
                foreach (var ch in text) {
                    if (allowedCharacters.Contains(ch)) {
                        sb.Append(ch);
                    }
                }
                return sb.ToString();
            } else {
                return text;
            }
        }

        private static readonly char[] Base32Characters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '2', '3', '4', '5', '6', '7' };
        private static readonly char[] NumberCharacters = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };


        private delegate string GetText();

        #endregion

    }
}
