using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Medo.Security.Cryptography.Bimil;
using System.Reflection;
using System.Diagnostics;

namespace Bimil {
    public partial class EditItemForm : Form {

        private readonly BimilItem Item;
        private readonly bool StartsAsEditable;

        public EditItemForm(BimilItem item, bool startsAsEditable) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;

            this.Item = item;
            this.StartsAsEditable = startsAsEditable;
        }

        private void EditItemForm_Load(object sender, EventArgs e) {
            if (this.StartsAsEditable) {
                btnEdit.Visible = false;
                btnOK.Visible = true;
            } else {
                btnEdit.Visible = true;
                btnOK.Visible = false;
                btnCancel.Text = "Close";
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
                case Keys.F2: btnEdit_Click(null, null); break;
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
                titleTextBox = new TextBox() { Font = this.Font, Location = new Point(labelWidth + labelBuffer, 0), Tag = this.Item, Text = this.Item.Title, Width = pnl.ClientSize.Width - labelWidth - labelBuffer, ReadOnly = !this.StartsAsEditable };
                titleTextBox.GotFocus += new EventHandler(delegate(object sender2, EventArgs e2) { ((TextBox)sender2).SelectAll(); });
                titleTextBox.TextChanged += new EventHandler(delegate(object sender2, EventArgs e2) { btnOK.Enabled = (((Control)sender2).Text.Trim().Length > 0); });
                pnl.Controls.Add(titleTextBox);
                var label = new Label() { AutoEllipsis = true, Location = new Point(0, y), Size = new Size(labelWidth, unitHeight), Text = "Name:", TextAlign = ContentAlignment.MiddleLeft, UseMnemonic = false };
                pnl.Controls.Add(label);

                y += label.Height + (label.Height / 4);
            }

            foreach (var record in this.Item.Records) {
                switch (record.Format) {
                    case BimilRecordFormat.Text: {
                            var textBox = new TextBox() { Font = this.Font, Location = new Point(labelWidth + labelBuffer, y), Tag = record, Text = record.Value.Text, Width = pnl.ClientSize.Width - labelWidth - labelBuffer - unitHeight, ReadOnly = !this.StartsAsEditable };
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
                        } break;

                    case BimilRecordFormat.Password: {
                            var textBox = new TextBox() { Font = this.Font, Location = new Point(labelWidth + labelBuffer, y), Tag = record, Text = record.Value.Text, Width = pnl.ClientSize.Width - labelWidth - labelBuffer - unitHeight - unitHeight, UseSystemPasswordChar = true, ReadOnly = !this.StartsAsEditable };
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
                        } break;

                    case BimilRecordFormat.Url: {
                            var textBox = new TextBox() { Font = this.Font, Location = new Point(labelWidth + labelBuffer, y), Tag = record, Text = record.Value.Text, Width = pnl.ClientSize.Width - labelWidth - labelBuffer - unitHeight - unitHeight, ReadOnly = !this.StartsAsEditable };
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
                        } break;

                    case BimilRecordFormat.MultilineText:
                    default: { //behave as multiline text
                            var textBox = new TextBox() { Location = new Point(labelWidth + labelBuffer, y), Tag = record, Text = record.Value.Text, Width = pnl.ClientSize.Width - labelWidth - labelBuffer, Multiline = true, Height = unitHeight * 2, AcceptsReturn = true, ReadOnly = !this.StartsAsEditable };
                            textBox.GotFocus += new EventHandler(delegate(object sender2, EventArgs e2) { ((TextBox)sender2).SelectAll(); });
                            pnl.Controls.Add(textBox);
                        } break;
                }

                var label = new Label() { AutoEllipsis = true, Location = new Point(0, y), Size = new Size(labelWidth, unitHeight), Text = record.Key.Text + ":", TextAlign = ContentAlignment.MiddleLeft, UseMnemonic = false };
                pnl.Controls.Add(label);

                y += label.Height + (label.Height / 4);
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
            }
            btnEdit.Visible = false;
            btnOK.Visible = true;
            btnCancel.Text = "Cancel";
        }

        private void btnOK_Click(object sender, EventArgs e) {
            foreach (Control control in pnl.Controls) {
                var record = control.Tag as BimilRecord;
                if (record != null) {
                    record.Value.Text = control.Text;
                } else {
                    var item = control.Tag as BimilItem;
                    if (item != null) {
                        item.Title = control.Text;
                    }
                }
            }
        }

        private void btnFields_Click(object sender, EventArgs e) {

        }

    }
}
