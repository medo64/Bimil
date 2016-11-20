using Medo.Security.Cryptography.PasswordSafe;
using System.Windows.Forms;
using System;
using System.Drawing;
using Medo.Security.Cryptography;
using System.Threading;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Bimil {
    internal partial class AutotypeForm : Form {
        public AutotypeForm(Entry entry) {
            InitializeComponent();

            this.Font = SystemFonts.MessageBoxFont;
            Medo.Windows.Forms.State.SetupOnLoadAndClose(this);
            this.Opacity = Settings.AutoTypeWindowOpacity / 100;

            var y = 0;

            foreach (var record in entry.Records) {
                if (record.RecordType == RecordType.Autotype) {
                    y = AddButton(y, "Auto-type", AutotypeToken.GetUnexpandedAutotypeTokens(record.Text), isDefinedAutoType: true).Bottom;
                }
            }

            if (y == 0) { //no auto-type; use default
                y = AddButton(y, "Auto-type", AutotypeToken.GetUnexpandedAutotypeTokens(null), isDefinedAutoType: true).Bottom;
            }

            //Suffix type
            var buttonNoSuffix = new ToolStripButton("None");
            var buttonTabSuffix = new ToolStripButton("Tab");
            var buttonEnterSuffix = new ToolStripButton("Enter");

            buttonNoSuffix.Click += delegate (object sender2, EventArgs e2) {
                this.Suffix = SuffixType.None;
                buttonNoSuffix.Checked = true;
                buttonTabSuffix.Checked = false;
                buttonEnterSuffix.Checked = false;
            };
            buttonTabSuffix.Click += delegate (object sender2, EventArgs e2) {
                this.Suffix = SuffixType.Tab;
                buttonNoSuffix.Checked = false;
                buttonTabSuffix.Checked = true;
                buttonEnterSuffix.Checked = false;
            };
            buttonEnterSuffix.Click += delegate (object sender2, EventArgs e2) {
                this.Suffix = SuffixType.Enter;
                buttonNoSuffix.Checked = false;
                buttonTabSuffix.Checked = false;
                buttonEnterSuffix.Checked = true;
            };

            var toolstrip = new ToolStrip() { Dock = DockStyle.None, Left = this.ClientRectangle.Left, Top = y + SystemInformation.DragSize.Height, GripStyle = ToolStripGripStyle.Hidden, RenderMode = ToolStripRenderMode.System };
            toolstrip.Items.Add(new ToolStripLabel("Suffix:") { ForeColor = SystemColors.GrayText });
            toolstrip.Items.AddRange(new ToolStripItem[] { buttonNoSuffix, buttonTabSuffix, buttonEnterSuffix });
            buttonNoSuffix.PerformClick();

            this.Controls.Add(toolstrip);
            y = toolstrip.Bottom;

            //all textual fields
            foreach (var record in entry.Records) {
                switch (record.RecordType) {
                    case RecordType.UserName:
                    case RecordType.EmailAddress:
                    case RecordType.CreditCardExpiration:
                        y = AddButton(y, Helpers.GetRecordCaption(record), AutotypeToken.GetAutotypeTokensFromText(record.Text),
                            record).Bottom;
                        break;

                    case RecordType.Password:
                    case RecordType.CreditCardPin:
                    case RecordType.CreditCardVerificationValue:
                        y = AddButton(y, Helpers.GetRecordCaption(record),
                            AutotypeToken.GetAutotypeTokensFromText(record.Text), record, isTextHidden: true).Bottom;
                        break;

                    case RecordType.TwoFactorKey:
                        y = AddButton(y, Helpers.GetRecordCaption(record),
                            AutotypeToken.GetUnexpandedAutotypeTokens(@"\2"), record, isTextHidden: true).Bottom;
                        break;

                    case RecordType.CreditCardNumber:
                        y = AddButton(y, Helpers.GetRecordCaption(record),
                            AutotypeToken.GetAutotypeTokensFromText(Helpers.FilterText(record.Text, Helpers.NumberCharacters)), record).Bottom;
                        break;
                }
            }

            var rect = AddButton(y + SystemInformation.DragSize.Height, "Cancel", isCancel: true);
            this.ClientSize = new Size(this.ClientRectangle.Width, rect.Bottom);
            this.MinimumSize = this.Size;
            this.MaximumSize = new Size(this.Size.Width * 2, this.Size.Height);

            this.Entry = entry;
        }

        private readonly Entry Entry;
        private SuffixType Suffix = SuffixType.None;


        protected override bool ProcessDialogKey(Keys keyData) {
            switch (keyData) {
                case Keys.Escape:
                    this.Close();
                    return true;
            }

            return base.ProcessDialogKey(keyData);
        }


        private int Delay;
        private bool UseSendWait;
        private bool CloseAfterType;

        private void bwType_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            var tokens = (IEnumerable<AutotypeToken>)e.Argument;

            this.Delay = Settings.AutoTypeDelay;
            this.UseSendWait = Settings.AutoTypeUseSendWait;

            foreach (var token in tokens) {
                if (token.Kind == AutotypeTokenKind.Command) {
                    var parts = token.Content.Split(':');
                    var command = parts[0];
                    var argument = (parts.Length > 1) ? parts[1] : null;

                    switch (command) {
                        case "Delay": {
                                int ms;
                                if (int.TryParse(argument, NumberStyles.Integer, CultureInfo.InvariantCulture, out ms)) {
                                    this.Delay = ms;
                                }
                            }
                            break;

                        case "Wait": {
                                int ms;
                                if (int.TryParse(argument, NumberStyles.Integer, CultureInfo.InvariantCulture, out ms)) {
                                    Thread.Sleep(ms);
                                }
                            }
                            break;

                        case "Legacy":
                            this.UseSendWait = !this.UseSendWait;
                            break;
                    }
                } else {
                    bwType.ReportProgress(0, token.Content);
                    Thread.Sleep(this.Delay);
                }
            }

            switch (this.Suffix) {
                case SuffixType.Tab: bwType.ReportProgress(100, "{TAB}"); break;
                case SuffixType.Enter: bwType.ReportProgress(100, "{ENTER}"); break;
            }
        }

        private void bwType_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e) {
            string content = (string)e.UserState;

            if (this.UseSendWait) {
                SendKeys.SendWait((Control.IsKeyLocked(Keys.CapsLock) ? "{CAPSLOCK}" : "") + content);
            } else {
                SendKeys.Send((Control.IsKeyLocked(Keys.CapsLock) ? "{CAPSLOCK}" : "") + content);
            }
        }

        private void bwType_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
            if (CloseAfterType) {
                this.Close();
            } else {
                tmrRestore.Enabled = true;
            }
        }

        private void tmrRestore_Tick(object sender, EventArgs e) {
            tmrRestore.Enabled = false;
            this.Visible = true;
            this.Select();
        }


        private Rectangle AddButton(int top, string caption, IEnumerable<AutotypeToken> tokens = null, Record record = null, bool isTextHidden = false, bool isCancel = false, bool isDefinedAutoType = false) {
            var btn = new Button() { Text = caption, Left = this.ClientRectangle.Left, Width = this.ClientRectangle.Width, Top = top, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, AutoEllipsis = true };

            if (tokens == null) {
                btn.Height *= 2;
            } else {
                btn.Height *= 3;
                btn.Text += "\n";
                if (isTextHidden) {
                    btn.Text += "******";
                } else {
                    if (tokens != null) {
                        var sb = new StringBuilder();
                        foreach (var token in tokens) {
                            if ((token.Kind != AutotypeTokenKind.Key) || (token.Content.StartsWith("{", StringComparison.Ordinal) && token.Content.EndsWith("}", StringComparison.Ordinal))) {
                                sb.Append(" ");
                            }
                            sb.Append(token.Content);
                        }
                        btn.Text += sb.ToString();
                    }
                };
            }

            if (isCancel) {
                btn.Click += delegate (object sender, EventArgs e) {
                    this.Close();
                };
            } else {
                btn.Click += delegate (object sender, EventArgs e) {
                    this.Visible = false;

                    var processedTokens = new List<AutotypeToken>();
                    foreach (var token in AutotypeToken.GetAutotypeTokens(tokens, this.Entry)) {
                        if ((token.Kind == AutotypeTokenKind.Command) && token.Content.Equals("TwoFactorCode", StringComparison.Ordinal)) {
                            var bytes = (record != null) ? record.GetBytes() : this.Entry.TwoFactorKey;
                            var key = OneTimePassword.ToBase32(bytes, bytes.Length, SecretFormatFlags.Spacing | SecretFormatFlags.Padding);
                            processedTokens.AddRange(AutotypeToken.GetAutotypeTokensFromText(Helpers.GetTwoFactorCode(key)));
                        } else {
                            processedTokens.Add(token);
                        }
                    }

                    this.CloseAfterType = isDefinedAutoType;
                    bwType.RunWorkerAsync(processedTokens.AsReadOnly());
                };
            }

            this.Controls.Add(btn);

            return new Rectangle(btn.Location, btn.Size);
        }


        private enum SuffixType {
            None,
            Tab,
            Enter
        }
    }
}
