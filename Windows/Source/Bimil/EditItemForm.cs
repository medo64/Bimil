using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Medo.Security.Cryptography.Bimil;
using System.Collections.Generic;

namespace Bimil {
    public partial class EditItemForm : Form {

        private readonly BimilDocument Document;
        private readonly BimilItem Item;
        private bool Editable;
        private static Font FixedFont = new Font(FontFamily.GenericMonospace, SystemFonts.MessageBoxFont.SizeInPoints + 0.5F, SystemFonts.MessageBoxFont.Style);
        private static Font UnderlineFont = new Font(SystemFonts.MessageBoxFont.Name, SystemFonts.MessageBoxFont.SizeInPoints, SystemFonts.MessageBoxFont.Style | FontStyle.Underline);
        private readonly IList<string> Categories;

        public EditItemForm(BimilDocument document, BimilItem item, bool startsAsEditable, IList<string> categories) {
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
                case Keys.F2:
                    if (btnEdit.Visible) {
                        btnEdit_Click(null, null);
                    } else if (btnFields.Visible) {
                        btnFields_Click(null, null);
                    }
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
                var record = this.Item.NameRecord;
                titleTextBox = new TextBox() { Font = this.Font, Location = new Point(labelWidth + labelBuffer, 0), Tag = record, Text = record.Value.Text, Width = pnl.ClientSize.Width - labelWidth - labelBuffer, ReadOnly = !this.Editable };
                titleTextBox.GotFocus += new EventHandler(delegate(object sender2, EventArgs e2) { ((TextBox)sender2).SelectAll(); });
                titleTextBox.TextChanged += new EventHandler(delegate(object sender2, EventArgs e2) { btnOK.Enabled = (((Control)sender2).Text.Trim().Length > 0); });
                pnl.Controls.Add(titleTextBox);
                var label = new Label() { AutoEllipsis = true, Location = new Point(0, y), Size = new Size(labelWidth, unitHeight), Text = "Name:", TextAlign = ContentAlignment.MiddleLeft, UseMnemonic = false };
                pnl.Controls.Add(label);

                y += titleTextBox.Height + (label.Height / 4);
            }

            ComboBox categoryComboBox;
            {
                var record = this.Item.CategoryRecord;
                categoryComboBox = new ComboBox() { Font = this.Font, Location = new Point(labelWidth + labelBuffer, y), Tag = record, Text = record.Value.Text, Width = pnl.ClientSize.Width - labelWidth - labelBuffer, Enabled = this.Editable };
                categoryComboBox.GotFocus += new EventHandler(delegate(object sender2, EventArgs e2) { ((ComboBox)sender2).SelectAll(); });
                foreach (var category in this.Categories) {
                    categoryComboBox.Items.Add(category);
                }
                pnl.Controls.Add(categoryComboBox);
                var label = new Label() { AutoEllipsis = true, Location = new Point(0, y), Size = new Size(labelWidth, unitHeight), Text = "Category:", TextAlign = ContentAlignment.MiddleLeft, UseMnemonic = false };
                pnl.Controls.Add(label);

                y += titleTextBox.Height + (label.Height / 4);
            }

            int yH;
            foreach (var record in this.Item.Records) {
                if (record.Format != BimilRecordFormat.System) {
                    switch (record.Format) {
                        case BimilRecordFormat.Text: {
                                var textBox = new TextBox() { Font = this.Font, Location = new Point(labelWidth + labelBuffer, y), Tag = record, Text = record.Value.Text, Width = pnl.ClientSize.Width - labelWidth - labelBuffer - unitHeight, ReadOnly = !this.Editable };
                                textBox.GotFocus += new EventHandler(delegate(object sender2, EventArgs e2) { ((TextBox)sender2).SelectAll(); });
                                pnl.Controls.Add(textBox);
                                var btnCopy = new Button() { Location = new Point(pnl.ClientSize.Width - unitHeight, y), Size = new Size(unitHeight, unitHeight), TabStop = false, Tag = textBox, Text = "", Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Bimil.Resources.Copy_16.png")) };
                                btnCopy.Click += new EventHandler(delegate(object sender2, EventArgs e2) {
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
                            } break;

                        case BimilRecordFormat.MonospacedText: {
                                var textBox = new TextBox() { Font = EditItemForm.FixedFont, Location = new Point(labelWidth + labelBuffer, y), Tag = record, Text = record.Value.Text, Width = pnl.ClientSize.Width - labelWidth - labelBuffer - unitHeight, ReadOnly = !this.Editable };
                                textBox.GotFocus += new EventHandler(delegate(object sender2, EventArgs e2) { ((TextBox)sender2).SelectAll(); });
                                pnl.Controls.Add(textBox);
                                var btnCopy = new Button() { Location = new Point(pnl.ClientSize.Width - unitHeight, y), Size = new Size(unitHeight, textBox.Height), TabStop = false, Tag = textBox, Text = "", Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Bimil.Resources.Copy_16.png")) };
                                btnCopy.Click += new EventHandler(delegate(object sender2, EventArgs e2) {
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
                            } break;

                        case BimilRecordFormat.Password: {
                                var textBox = new TextBox() { Font = this.Font, Location = new Point(labelWidth + labelBuffer, y), Tag = record, Text = record.Value.Text, Width = pnl.ClientSize.Width - labelWidth - labelBuffer - unitHeight - unitHeight, UseSystemPasswordChar = true, ReadOnly = !this.Editable };
                                textBox.GotFocus += new EventHandler(delegate(object sender2, EventArgs e2) { ((TextBox)sender2).SelectAll(); });
                                pnl.Controls.Add(textBox);
                                var btnShowPass = new Button() { Location = new Point(pnl.ClientSize.Width - unitHeight - unitHeight, y), Size = new Size(unitHeight, unitHeight), TabStop = false, Tag = textBox, Text = "", Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Bimil.Resources.RevealPassword_16.png")) };
                                btnShowPass.Click += new EventHandler(delegate(object sender2, EventArgs e2) {
                                    var box = (TextBox)(((Control)sender2).Tag);
                                    box.Select();
                                    box.UseSystemPasswordChar = !box.UseSystemPasswordChar;
                                });
                                pnl.Controls.Add(btnShowPass);
                                var btnCopy = new Button() { Location = new Point(pnl.ClientSize.Width - unitHeight, y), Size = new Size(unitHeight, unitHeight), TabStop = false, Tag = textBox, Text = "", Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Bimil.Resources.Copy_16.png")) };
                                btnCopy.Click += new EventHandler(delegate(object sender2, EventArgs e2) {
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
                            } break;

                        case BimilRecordFormat.Url: {
                                var textBox = new TextBox() { Font = EditItemForm.UnderlineFont, Location = new Point(labelWidth + labelBuffer, y), Tag = record, Text = record.Value.Text, Width = pnl.ClientSize.Width - labelWidth - labelBuffer - unitHeight - unitHeight, ReadOnly = !this.Editable, ForeColor = SystemColors.HotTrack };
                                textBox.GotFocus += new EventHandler(delegate(object sender2, EventArgs e2) { ((TextBox)sender2).SelectAll(); });
                                pnl.Controls.Add(textBox);
                                var btnExecuteUrl = new Button() { Location = new Point(pnl.ClientSize.Width - unitHeight - unitHeight, y), Size = new Size(unitHeight, unitHeight), TabStop = false, Tag = textBox, Text = "", Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Bimil.Resources.ExecuteUrl_16.png")) };
                                btnExecuteUrl.Click += new EventHandler(delegate(object sender2, EventArgs e2) {
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
                                var btnCopy = new Button() { Font = this.Font, Location = new Point(pnl.ClientSize.Width - unitHeight, y), Size = new Size(unitHeight, unitHeight), TabStop = false, Tag = textBox, Text = "", Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Bimil.Resources.Copy_16.png")) };
                                btnCopy.Click += new EventHandler(delegate(object sender2, EventArgs e2) {
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
                            } break;

                        case BimilRecordFormat.MultilineText:
                        default: { //behave as multiline text
                                var textBox = new TextBox() { Location = new Point(labelWidth + labelBuffer, y), Tag = record, Text = record.Value.Text, Width = pnl.ClientSize.Width - labelWidth - labelBuffer, Multiline = true, Height = unitHeight * 2, AcceptsReturn = true, ScrollBars = ScrollBars.Vertical, ReadOnly = !this.Editable };
                                textBox.GotFocus += new EventHandler(delegate(object sender2, EventArgs e2) { ((TextBox)sender2).SelectAll(); });
                                pnl.Controls.Add(textBox);
                                yH = textBox.Height;
                            } break;
                    }

                    var label = new Label() { AutoEllipsis = true, Location = new Point(0, y), Size = new Size(labelWidth, unitHeight), Text = record.Key.Text + ":", TextAlign = ContentAlignment.MiddleLeft, UseMnemonic = false };
                    pnl.Controls.Add(label);

                    y += yH + (label.Height / 4);
                }
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
                var record = control.Tag as BimilRecord;
                if (record != null) {
                    record.Value.Text = control.Text;
                }
            }
        }

        private void btnFields_Click(object sender, EventArgs e) {
            using (var frm = new FieldsEditorForm(this.Document, this.Item)) {
                if (frm.ShowDialog(this) == DialogResult.OK) {
                    FillRecords();
                }
            }
        }

    }
}
