using Medo.Security.Cryptography.PasswordSafe;
using System.Windows.Forms;
using System;
using System.Drawing;
using Medo.Security.Cryptography;
using System.Threading;

namespace Bimil {
    internal partial class FillForm : Form {
        public FillForm(Entry entry) {
            InitializeComponent();

            this.Font = SystemFonts.MessageBoxFont;
            Medo.Windows.Forms.State.SetupOnLoadAndClose(this);

            var y = 0;
            foreach (var record in entry.Records) {
                switch (record.RecordType) {
                    case RecordType.UserName:
                    case RecordType.EmailAddress:
                    case RecordType.CreditCardExpiration:
                        y = AddButton(y, Helpers.GetRecordCaption(record), record.Text).Bottom;
                        break;

                    case RecordType.Password:
                    case RecordType.CreditCardPin:
                    case RecordType.CreditCardVerificationValue:
                        y = AddButton(y, Helpers.GetRecordCaption(record), record.Text, isTextHidden: true).Bottom;
                        break;

                    case RecordType.TwoFactorKey:
                        var bytes = record.GetBytes();
                        y = AddButton(y, Helpers.GetRecordCaption(record), OneTimePassword.ToBase32(bytes, bytes.Length, SecretFormatFlags.Spacing | SecretFormatFlags.Padding), isTextHidden: true, isText2FAKey: true).Bottom;
                        Array.Clear(bytes, 0, bytes.Length);
                        break;

                    case RecordType.CreditCardNumber:
                        y = AddButton(y, Helpers.GetRecordCaption(record), record.Text, allowedCharacters: Helpers.NumberCharacters).Bottom;
                        break;
                }
            }

            var rect = AddButton(y + SystemInformation.DragSize.Height, "Cancel", null, isCancel: true);
            this.ClientSize = new Size(this.ClientRectangle.Width, rect.Bottom);
            this.MinimumSize = this.Size;
            this.MaximumSize = new Size(this.Size.Width * 4, this.Size.Height);
        }


        protected override bool ProcessDialogKey(Keys keyData) {
            switch (keyData) {
                case Keys.Escape:
                    this.Close();
                    return true;
            }

            return base.ProcessDialogKey(keyData);
        }


        private void bwType_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            var text = e.Argument.ToString();

            if (!string.IsNullOrEmpty(text)) {
                if (Settings.AutoTypeUseClipboard) {
                    bwType.ReportProgress(100, text);
                } else {
                    foreach (var key in Helpers.GetTextForIndividualSendKeys(text)) {
                        bwType.ReportProgress(0, key);
                        Thread.Sleep(Settings.AutoTypeDelay);
                    }
                    bwType.ReportProgress(100, "");
                }
            }
        }

        private void bwType_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e) {
            var text = e.UserState.ToString();

            if (!string.IsNullOrEmpty(text)) {
                if (Settings.AutoTypeUseClipboard) {
                    Clipboard.Clear();
                    Clipboard.SetText(text);
                    if (Settings.AutoTypeUseSendWait) { SendKeys.SendWait("^V"); } else { SendKeys.Send("^V"); }
                    Clipboard.Clear();
                } else {
                    if (Settings.AutoTypeUseSendWait) { SendKeys.SendWait(text); } else { SendKeys.Send(text); }
                }
            }

            if (e.ProgressPercentage == 100) {
                if (Settings.AutoTypeUseSendWait) { SendKeys.SendWait(Settings.AutoTypeSuffixKeys); } else { SendKeys.Send(Settings.AutoTypeSuffixKeys); }
            }
        }

        private void bwType_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
            tmrRestore.Enabled = true;
        }

        private void tmrRestore_Tick(object sender, EventArgs e) {
            tmrRestore.Enabled = false;
            this.Visible = true;
            this.Select();
        }


        private Rectangle AddButton(int top, string caption, string text, char[] allowedCharacters = null, bool isTextHidden = false, bool isText2FAKey = false, bool isCancel = false) {
            var btn = new Button() { Text = caption, Left = this.ClientRectangle.Left, Width = this.ClientRectangle.Width, Top = top, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            if (string.IsNullOrEmpty(text)) {
                btn.Height *= 2;
            } else {
                btn.Height *= 3;
                btn.Text += "\n" + (isTextHidden ? "******" : text);
            }

            if (isCancel) {
                btn.Click += delegate (object sender, EventArgs e) {
                    this.Close();
                };
            } else {
                btn.Click += delegate (object sender, EventArgs e) {
                    this.Visible = false;

                    var filteredText = (allowedCharacters == null) ? text : Helpers.FilterText(text, allowedCharacters);

                    bwType.RunWorkerAsync(isText2FAKey ? Helpers.GetTwoFactorCode(filteredText) : filteredText);
                };
            }

            this.Controls.Add(btn);

            return new Rectangle(btn.Location, btn.Size);
        }

    }
}
