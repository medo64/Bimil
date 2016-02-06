using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using Medo.Security.Cryptography.PasswordSafe;
using System.Collections.Generic;
using System.Web;

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
                    case RecordType.BimilCreditCardExpiration:
                    case RecordType.BimilCreditCardSecurityCode:
                    case RecordType.BimilCreditCardPin: {
                            var textBox = new TextBox() { Font = this.Font, Location = new Point(labelWidth + labelBuffer, y), Tag = record, Text = record.Text, Width = pnl.ClientSize.Width - labelWidth - labelBuffer - unitHeight, ReadOnly = !this.Editable, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
                            textBox.GotFocus += new EventHandler(delegate (object sender2, EventArgs e2) { ((TextBox)sender2).SelectAll(); });
                            pnl.Controls.Add(textBox);

                            var btnCopy = new Button() { Location = new Point(pnl.ClientSize.Width - unitHeight, y), Size = new Size(unitHeight, unitHeight), TabStop = false, Tag = textBox, Text = "", Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Bimil.Resources.Copy_16.png")), Anchor = AnchorStyles.Top | AnchorStyles.Right };
                            btnCopy.Click += new EventHandler(delegate (object sender2, EventArgs e2) {
                                var box = (TextBox)(((Control)sender2).Tag);
                                box.Select();
                                var text = box.Text;
                                if (text.Length > 0) {
                                    Clipboard.SetText(text);
                                } else {
                                    Clipboard.Clear();
                                }
                            });
                            pnl.Controls.Add(btnCopy);

                            yH = textBox.Height;
                        }
                        break;

                    case RecordType.Password: {
                            var textBox = new TextBox() { Font = this.Font, Location = new Point(labelWidth + labelBuffer, y), Tag = record, Text = record.Text, Width = pnl.ClientSize.Width - labelWidth - labelBuffer - unitHeight - unitHeight, UseSystemPasswordChar = true, ReadOnly = !this.Editable, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
                            textBox.GotFocus += new EventHandler(delegate (object sender2, EventArgs e2) { ((TextBox)sender2).SelectAll(); });
                            pnl.Controls.Add(textBox);

                            var btnShowPass = new Button() { Location = new Point(pnl.ClientSize.Width - unitHeight - unitHeight, y), Size = new Size(unitHeight, unitHeight), TabStop = false, Tag = textBox, Text = "", Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Bimil.Resources.RevealPassword_16.png")), Anchor = AnchorStyles.Top | AnchorStyles.Right };
                            btnShowPass.Click += new EventHandler(delegate (object sender2, EventArgs e2) {
                                var box = (TextBox)(((Control)sender2).Tag);
                                box.Select();
                                box.UseSystemPasswordChar = !box.UseSystemPasswordChar;
                            });
                            pnl.Controls.Add(btnShowPass);

                            var btnCopy = new Button() { Location = new Point(pnl.ClientSize.Width - unitHeight, y), Size = new Size(unitHeight, unitHeight), TabStop = false, Tag = textBox, Text = "", Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Bimil.Resources.Copy_16.png")), Anchor = AnchorStyles.Top | AnchorStyles.Right };
                            btnCopy.Click += new EventHandler(delegate (object sender2, EventArgs e2) {
                                var box = (TextBox)(((Control)sender2).Tag);
                                box.Select();
                                var text = box.Text;
                                if (text.Length > 0) {
                                    Clipboard.SetText(text);
                                } else {
                                    Clipboard.Clear();
                                }
                            });
                            pnl.Controls.Add(btnCopy);

                            yH = textBox.Height;
                        }
                        break;

                    case RecordType.Url: {
                            var textBox = new TextBox() { Font = EditItemForm.UnderlineFont, Location = new Point(labelWidth + labelBuffer, y), Tag = record, Text = record.Text, Width = pnl.ClientSize.Width - labelWidth - labelBuffer - unitHeight - unitHeight, ReadOnly = !this.Editable, ForeColor = SystemColors.HotTrack, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
                            textBox.GotFocus += new EventHandler(delegate (object sender2, EventArgs e2) { ((TextBox)sender2).SelectAll(); });
                            pnl.Controls.Add(textBox);

                            var btnExecuteUrl = new Button() { Location = new Point(pnl.ClientSize.Width - unitHeight - unitHeight, y), Size = new Size(unitHeight, unitHeight), TabStop = false, Tag = textBox, Text = "", Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Bimil.Resources.ExecuteUrl_16.png")), Anchor = AnchorStyles.Top | AnchorStyles.Right };
                            btnExecuteUrl.Click += new EventHandler(delegate (object sender2, EventArgs e2) {
                                var box = (TextBox)(((Control)sender2).Tag);
                                box.Select();
                                var text = box.Text;
                                if (text.Contains("://")) {
                                    Process.Start(text);
                                } else {
                                    if (text.Length > 0) {
                                        Process.Start("http://" + text);
                                    } else {
                                        Clipboard.Clear();
                                    }
                                }
                            });
                            pnl.Controls.Add(btnExecuteUrl);

                            var btnCopy = new Button() { Font = this.Font, Location = new Point(pnl.ClientSize.Width - unitHeight, y), Size = new Size(unitHeight, unitHeight), TabStop = false, Tag = textBox, Text = "", Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Bimil.Resources.Copy_16.png")), Anchor = AnchorStyles.Top | AnchorStyles.Right };
                            btnCopy.Click += new EventHandler(delegate (object sender2, EventArgs e2) {
                                var box = (TextBox)(((Control)sender2).Tag);
                                box.Select();
                                var text = box.Text;
                                if (text.Contains("://")) {
                                    Clipboard.SetText(text);
                                } else {
                                    if (text.Length > 0) {
                                        Clipboard.SetText("http://" + text);
                                    } else {
                                        Clipboard.Clear();
                                    }
                                }
                            });
                            pnl.Controls.Add(btnCopy);

                            yH = textBox.Height;
                        }
                        break;

                    case RecordType.EmailAddress: {
                            var textBox = new TextBox() { Font = EditItemForm.UnderlineFont, Location = new Point(labelWidth + labelBuffer, y), Tag = record, Text = record.Text, Width = pnl.ClientSize.Width - labelWidth - labelBuffer - unitHeight - unitHeight, ReadOnly = !this.Editable, ForeColor = SystemColors.HotTrack, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
                            textBox.GotFocus += new EventHandler(delegate (object sender2, EventArgs e2) { ((TextBox)sender2).SelectAll(); });
                            pnl.Controls.Add(textBox);

                            var btnExecuteUrl = new Button() { Location = new Point(pnl.ClientSize.Width - unitHeight - unitHeight, y), Size = new Size(unitHeight, unitHeight), TabStop = false, Tag = textBox, Text = "", Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Bimil.Resources.ExecuteUrl_16.png")), Anchor = AnchorStyles.Top | AnchorStyles.Right };
                            btnExecuteUrl.Click += new EventHandler(delegate (object sender2, EventArgs e2) {
                                var box = (TextBox)(((Control)sender2).Tag);
                                box.Select();
                                var text = box.Text;
                                if (text.Length > 0) {
                                    if (text.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase)) {
                                        Process.Start(text);
                                    } else {
                                        Process.Start("mailto:" + text);
                                    }
                                }
                            });
                            pnl.Controls.Add(btnExecuteUrl);

                            var btnCopy = new Button() { Font = this.Font, Location = new Point(pnl.ClientSize.Width - unitHeight, y), Size = new Size(unitHeight, unitHeight), TabStop = false, Tag = textBox, Text = "", Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Bimil.Resources.Copy_16.png")), Anchor = AnchorStyles.Top | AnchorStyles.Right };
                            btnCopy.Click += new EventHandler(delegate (object sender2, EventArgs e2) {
                                var box = (TextBox)(((Control)sender2).Tag);
                                box.Select();
                                var text = box.Text;
                                if (text.Contains("://")) {
                                    Clipboard.SetText(text);
                                } else {
                                    if (text.Length > 0) {
                                        Clipboard.SetText("http://" + text);
                                    } else {
                                        Clipboard.Clear();
                                    }
                                }
                            });
                            pnl.Controls.Add(btnCopy);

                            yH = textBox.Height;
                        }
                        break;

                    case RecordType.Notes: {
                            var textBox = new TextBox() { Location = new Point(labelWidth + labelBuffer, y), Tag = record, Text = record.Text, Width = pnl.ClientSize.Width - labelWidth - labelBuffer, Multiline = true, Height = unitHeight * 3, AcceptsReturn = true, ScrollBars = ScrollBars.Vertical, ReadOnly = !this.Editable, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
                            textBox.GotFocus += new EventHandler(delegate (object sender2, EventArgs e2) { ((TextBox)sender2).SelectAll(); });
                            pnl.Controls.Add(textBox);

                            yH = textBox.Height;
                        }
                        break;

                    case RecordType.BimilTwoFactorKey: {
                            var textBox = new TextBox() { Font = EditItemForm.FixedFont, Location = new Point(labelWidth + labelBuffer, y), Tag = record, Text = record.Text, Width = pnl.ClientSize.Width - labelWidth - labelBuffer - unitHeight, ReadOnly = !this.Editable, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
                            textBox.GotFocus += new EventHandler(delegate (object sender2, EventArgs e2) { ((TextBox)sender2).SelectAll(); });
                            pnl.Controls.Add(textBox);
                            var btnCopy = new Button() { Location = new Point(pnl.ClientSize.Width - unitHeight, y), Size = new Size(unitHeight, textBox.Height), TabStop = false, Tag = textBox, Text = "", Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Bimil.Resources.Copy_16.png")), Anchor = AnchorStyles.Top | AnchorStyles.Right };
                            btnCopy.Click += new EventHandler(delegate (object sender2, EventArgs e2) {
                                var box = (TextBox)(((Control)sender2).Tag);
                                box.Select();
                                var key = box.Text.ToUpperInvariant().Replace(" ", "");
                                if (key.Length > 0) {
                                    Clipboard.SetText(string.Format(CultureInfo.InvariantCulture, "otpauth://totp/{0}?secret={1}", HttpUtility.UrlPathEncode(this.Item.Title), HttpUtility.UrlEncode(key)));
                                } else {
                                    Clipboard.Clear();
                                }
                            });
                            pnl.Controls.Add(btnCopy);
                            yH = textBox.Height;
                        }
                        break;

                    case RecordType.BimilCreditCardNumber: {
                            var textBox = new TextBox() { Font = this.Font, Location = new Point(labelWidth + labelBuffer, y), Tag = record, Text = record.Text, Width = pnl.ClientSize.Width - labelWidth - labelBuffer - unitHeight, ReadOnly = !this.Editable, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
                            textBox.GotFocus += new EventHandler(delegate (object sender2, EventArgs e2) { ((TextBox)sender2).SelectAll(); });
                            pnl.Controls.Add(textBox);

                            var btnCopy = new Button() { Location = new Point(pnl.ClientSize.Width - unitHeight, y), Size = new Size(unitHeight, unitHeight), TabStop = false, Tag = textBox, Text = "", Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Bimil.Resources.Copy_16.png")), Anchor = AnchorStyles.Top | AnchorStyles.Right };
                            btnCopy.Click += new EventHandler(delegate (object sender2, EventArgs e2) {
                                var box = (TextBox)(((Control)sender2).Tag);
                                box.Select();
                                var text = box.Text.Replace(" ", "").Replace("-", "");
                                if (text.Length > 0) {
                                    Clipboard.SetText(text);
                                } else {
                                    Clipboard.Clear();
                                }
                            });
                            pnl.Controls.Add(btnCopy);

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
                    record.Text = control.Text;
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

    }
}
