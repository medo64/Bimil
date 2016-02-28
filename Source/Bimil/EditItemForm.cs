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
    public partial class EditItemForm : Form {

        private readonly Document Document;
        private readonly Entry Item;
        private bool Editable;
        private static Font FixedFont = new Font(FontFamily.GenericMonospace, SystemFonts.MessageBoxFont.SizeInPoints + 0.5F, SystemFonts.MessageBoxFont.Style);
        private static Font UnderlineFont = new Font(SystemFonts.MessageBoxFont.Name, SystemFonts.MessageBoxFont.SizeInPoints, SystemFonts.MessageBoxFont.Style | FontStyle.Underline);
        private readonly IList<string> Categories;

        public EditItemForm(Document document, Entry item, bool startsAsEditable, IList<string> categories) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;

            this.Document = document;
            this.Item = item;
            this.Editable = startsAsEditable;
            this.Categories = categories;
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
                            if ((record != null) && (record.RecordType == RecordType.Password) && (textBox.UseSystemPasswordChar)) { alreadyHidden = true; break; }
                        }
                    }
                    foreach (var control in this.pnl.Controls) {
                        var textBox = control as TextBox;
                        if (textBox != null) {
                            var record = textBox.Tag as Record;
                            if ((record != null) && (record.RecordType == RecordType.Password)) { textBox.UseSystemPasswordChar = !alreadyHidden; }
                        }
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
            }
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
                    case RecordType.PasswordHistory:
                        continue;

                    case RecordType.UserName:
                    case RecordType.CreditCardExpiration:
                    case RecordType.CreditCardVerificationValue:
                    case RecordType.CreditCardPin: {
                            var textBox = NewTextBox(labelWidth, y, record);
                            pnl.Controls.Add(textBox);

                            pnl.Controls.Add(NewCopyButton(textBox));

                            yH = textBox.Height;
                        }
                        break;

                    case RecordType.Password: {
                            var textBox = NewTextBox(labelWidth, y, record);
                            textBox.UseSystemPasswordChar = true;
                            pnl.Controls.Add(textBox);

                            pnl.Controls.Add(NewCopyButton(textBox));
                            pnl.Controls.Add(NewShowPasswordButton(textBox));

                            yH = textBox.Height;
                        }
                        break;

                    case RecordType.Url: {
                            var textBox = NewTextBox(labelWidth, y, record, urlLookAndFeel: true);
                            pnl.Controls.Add(textBox);

                            pnl.Controls.Add(NewCopyButton(textBox, "http://", "://"));
                            pnl.Controls.Add(NewExecuteUrlButton(textBox, "http://", "://"));

                            yH = textBox.Height;
                        }
                        break;

                    case RecordType.EmailAddress: {
                            var textBox = NewTextBox(labelWidth, y, record, urlLookAndFeel: true);
                            pnl.Controls.Add(textBox);

                            pnl.Controls.Add(NewCopyButton(textBox, "mailto:"));
                            pnl.Controls.Add(NewExecuteUrlButton(textBox, "mailto:"));

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

                            var btnCopy = NewCopyButton(textBox, noClickHandler: true);
                            btnCopy.Click += new EventHandler(delegate (object sender2, EventArgs e2) {
                                var box = (TextBox)(((Control)sender2).Tag);
                                box.Select();
                                var key = box.Text.ToUpperInvariant().Replace(" ", "");
                                Clipboard.Clear();
                                if (key.Length > 0) {
                                    try {
                                        var otp = new OneTimePassword(key);
                                        Clipboard.SetText(otp.GetCode().ToString(new string('0', otp.Digits), CultureInfo.InvariantCulture));
                                    } catch (ArgumentException) { }
                                }
                            });
                            pnl.Controls.Add(btnCopy);

                            pnl.Controls.Add(NewShowPasswordButton(textBox));

                            var btnExecuteUrl = NewExecuteUrlButton(textBox, noClickHandler: true);
                            btnExecuteUrl.Click += new EventHandler(delegate (object sender2, EventArgs e2) {
                                var box = (TextBox)(((Control)sender2).Tag);
                                box.Select();
                                var key = box.Text.ToUpperInvariant().Replace(" ", "");
                                if (key.Length > 0) {
                                    var url = string.Format(CultureInfo.InvariantCulture, "otpauth://totp/{0}?secret={1}", HttpUtility.UrlPathEncode(this.Item.Title), HttpUtility.UrlEncode(key));
                                    Process.Start(string.Format(CultureInfo.InvariantCulture, "https://api.qrserver.com/v1/create-qr-code/?margin=32&data={0}", HttpUtility.UrlEncode(url)));
                                }
                            });
                            pnl.Controls.Add(btnExecuteUrl);

                            yH = textBox.Height;
                        }
                        break;

                    case RecordType.CreditCardNumber: {
                            var textBox = NewTextBox(labelWidth, y, record);
                            pnl.Controls.Add(textBox);

                            pnl.Controls.Add(NewCopyButton(textBox, allowedCopyCharacters: new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }));

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
                        record.Text = control.Text;
                    }
                }
            }
        }

        private void btnFields_Click(object sender, EventArgs e) {
            btnOK_Click(null, null);
            using (var frm = new FieldsEditorForm(this.Document, this.Item)) {
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
                textBox.Font = EditItemForm.UnderlineFont;
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

        private Button NewCopyButton(TextBox parentTextBox, string defaultPrefix = null, string prefixToCheck = null, bool noClickHandler = false, char[] allowedCopyCharacters = null) {
            parentTextBox.Width -= parentTextBox.Height;
            var button = new Button() { Name = "btnCopy", Location = new Point(parentTextBox.Right, parentTextBox.Top), Size = new Size(parentTextBox.Height, parentTextBox.Height), TabStop = false, Tag = parentTextBox, Text = "", Anchor = AnchorStyles.Top | AnchorStyles.Right };
            Helpers.ScaleButton(button);

            if (!noClickHandler) {
                button.Click += new EventHandler(delegate (object sender, EventArgs e) {
                    var textBox = (TextBox)(((Control)sender).Tag);
                    textBox.Select();

                    var text = textBox.Text;
                    if (allowedCopyCharacters != null) {
                        var allowedCharacters = new List<char>(allowedCopyCharacters);
                        var sb = new StringBuilder();
                        foreach (var ch in text) {
                            if (allowedCharacters.Contains(ch)) {
                                sb.Append(ch);
                            }
                        }
                        text = sb.ToString();
                    }

                    Clipboard.Clear();
                    if (text.Length > 0) {
                        if (defaultPrefix == null) {
                            Clipboard.SetText(text);
                        } else if (text.IndexOf((prefixToCheck != null) ? prefixToCheck : defaultPrefix, StringComparison.OrdinalIgnoreCase) >= 0) {
                            Clipboard.SetText(text);
                        } else {
                            Clipboard.SetText(defaultPrefix + text);
                        }
                    }
                });
            }

            return button;
        }

        private Button NewShowPasswordButton(TextBox parentTextBox) {
            parentTextBox.Width -= parentTextBox.Height;
            var button = new Button() { Name = "btnRevealPassword", Location = new Point(parentTextBox.Right, parentTextBox.Top), Size = new Size(parentTextBox.Height, parentTextBox.Height), TabStop = false, Tag = parentTextBox, Text = "", Anchor = AnchorStyles.Top | AnchorStyles.Right };
            Helpers.ScaleButton(button);

            button.Click += new EventHandler(delegate (object sender, EventArgs e) {
                var textBox = (TextBox)(((Control)sender).Tag);
                textBox.Select();

                textBox.UseSystemPasswordChar = !textBox.UseSystemPasswordChar;
            });

            return button;
        }

        private Button NewExecuteUrlButton(TextBox parentTextBox, string defaultPrefix = null, string prefixToCheck = null, bool noClickHandler = false) {
            parentTextBox.Width -= parentTextBox.Height;
            var button = new Button() { Name = "btnExecuteUrl", Location = new Point(parentTextBox.Right, parentTextBox.Top), Size = new Size(parentTextBox.Height, parentTextBox.Height), TabStop = false, Tag = parentTextBox, Text = "", Anchor = AnchorStyles.Top | AnchorStyles.Right };
            Helpers.ScaleButton(button);

            if (!noClickHandler) {
                button.Click += new EventHandler(delegate (object sender, EventArgs e) {
                    var textBox = (TextBox)(((Control)sender).Tag);
                    textBox.Select();

                    var text = textBox.Text;
                    if (text.Length > 0) {
                        if (defaultPrefix == null) {
                            Process.Start(text);
                        } else if (text.IndexOf((prefixToCheck != null) ? prefixToCheck : defaultPrefix, StringComparison.OrdinalIgnoreCase) >= 0) {
                            Process.Start(text);
                        } else {
                            Process.Start(defaultPrefix + text);
                        }
                    }
                });
            }

            return button;
        }

        #endregion

    }
}
