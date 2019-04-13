using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using Medo.Security.Cryptography;
using Medo.Security.Cryptography.PasswordSafe;

namespace Bimil {
    internal partial class AutotypeForm : Form {
        public AutotypeForm(Entry entry) {
            InitializeComponent();

            Font = SystemFonts.MessageBoxFont;
            Medo.Windows.Forms.State.Attach(this);
            Opacity = Settings.AutoTypeWindowOpacity / 100;

            var y = 0;

            foreach (var record in entry.Records) {
                if (record.RecordType == RecordType.Autotype) {
                    y = AddTokenButton(y, "Auto-type", AutotypeToken.GetUnexpandedAutotypeTokens(record.Text), isDefinedAutoType: true).Bottom;
                }
            }

            if (y == 0) { //no auto-type; use default
                y = AddTokenButton(y, "Auto-type", AutotypeToken.GetUnexpandedAutotypeTokens(null), isDefinedAutoType: true).Bottom;
            }

            y += SystemInformation.DragSize.Height;

            //all textual fields
            foreach (var record in entry.Records) {
                var recordBytes = record.GetBytesSilently();
                try {
                    if (recordBytes.Length == 0) { continue; } //omit empty entries
                } finally {
                    Array.Clear(recordBytes, 0, recordBytes.Length);
                }

                switch (record.RecordType) {
                    case RecordType.UserName:
                    case RecordType.EmailAddress:
                    case RecordType.CreditCardExpiration: {
                            var tokens = AutotypeToken.GetAutotypeTokensFromText(record.Text);
                            var btn = AddTokenButton(y, Helpers.GetRecordCaption(record),
                                                     tokens, record);
                            y = btn.Bottom;
                            AddSuffixButtons(btn, tokens, record);
                        }
                        break;

                    case RecordType.Password:
                    case RecordType.CreditCardPin:
                    case RecordType.CreditCardVerificationValue: {
                            var tokens = AutotypeToken.GetAutotypeTokensFromText(record.Text);
                            var btn = AddTokenButton(y, Helpers.GetRecordCaption(record),
                                      tokens, record, isTextHidden: true);
                            y = btn.Bottom;
                            AddSuffixButtons(btn, tokens, record);
                        }
                        break;

                    case RecordType.TwoFactorKey: {
                            var tokens = AutotypeToken.GetUnexpandedAutotypeTokens(@"\2");
                            var btn = AddTokenButton(y, Helpers.GetRecordCaption(record),
                                                     tokens, record, isTextHidden: true);
                            y = btn.Bottom;
                            AddSuffixButtons(btn, tokens, record);
                        }
                        break;

                    case RecordType.CreditCardNumber: {
                            var tokens = AutotypeToken.GetAutotypeTokensFromText(Helpers.FilterText(record.Text, Helpers.NumberCharacters));
                            var btn = AddTokenButton(y, Helpers.GetRecordCaption(record),
                                                     tokens, record);
                            y = btn.Bottom;
                            AddSuffixButtons(btn, tokens, record);
                        }
                        break;
                }
            }

            y += SystemInformation.DragSize.Height;

            var btnClose = AddGenericButton(y, "Close");
            btnClose.Click += delegate { Close(); };

            ClientSize = new Size(SystemInformation.VerticalScrollBarWidth * 12, btnClose.Bottom);
            MinimumSize = new Size(SystemInformation.VerticalScrollBarWidth * 10, Height);
            MaximumSize = new Size(SystemInformation.VerticalScrollBarWidth * 30, Height);

            Entry = entry;
        }


        private readonly Entry Entry;
        private readonly SuffixType Suffix = SuffixType.None;


        protected override bool ProcessDialogKey(Keys keyData) {
            switch (keyData) {
                case Keys.Escape:
                    Close();
                    return true;
            }

            return base.ProcessDialogKey(keyData);
        }


        private int Delay;
        private bool UseSendWait;
        private bool CloseAfterType;

        private void bwType_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            var tokens = new List<AutotypeToken>((IEnumerable<AutotypeToken>)e.Argument).AsReadOnly();

            Delay = Settings.AutoTypeDelay;
            UseSendWait = Settings.AutoTypeUseSendWait;

            for (var i = 0; i < tokens.Count; i++) {
                var token = tokens[i];
                if (token.Kind == AutotypeTokenKind.Command) {
                    var parts = token.Content.Split(':');
                    var command = parts[0];
                    var argument = (parts.Length > 1) ? parts[1] : null;

                    switch (command) {
                        case "Delay": {
                                if (int.TryParse(argument, NumberStyles.Integer, CultureInfo.InvariantCulture, out var ms)) {
                                    Delay = ms;
                                }
                            }
                            break;

                        case "Wait": {
                                if (int.TryParse(argument, NumberStyles.Integer, CultureInfo.InvariantCulture, out var ms)) {
                                    Thread.Sleep(ms);
                                }
                            }
                            break;

                        case "Legacy":
                            UseSendWait = !UseSendWait;
                            break;
                    }
                } else {
                    bwType.ReportProgress(i * 100 / tokens.Count, token.Content);
                    Thread.Sleep(Delay);
                }
            }

            switch (Suffix) {
                case SuffixType.Tab: bwType.ReportProgress(100, "{TAB}"); break;
                case SuffixType.Enter: bwType.ReportProgress(100, "{ENTER}"); break;
            }
        }

        private void bwType_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e) {
            var content = (string)e.UserState;

            if (UseSendWait) {
                SendKeys.SendWait((Control.IsKeyLocked(Keys.CapsLock) ? "{CAPSLOCK}" : "") + content);
            } else {
                SendKeys.Send((Control.IsKeyLocked(Keys.CapsLock) ? "{CAPSLOCK}" : "") + content);
            }

            try {
                var progressIndex = e.ProgressPercentage / 25;
                if ((progressIndex == 1) && (tryProgress.Icon != Bimil.Properties.Resources.icoProgress1)) {
                    tryProgress.Icon = Bimil.Properties.Resources.icoProgress1;
                } else if ((progressIndex == 2) && (tryProgress.Icon != Bimil.Properties.Resources.icoProgress2)) {
                    tryProgress.Icon = Bimil.Properties.Resources.icoProgress2;
                } else if ((progressIndex == 3) && (tryProgress.Icon != Bimil.Properties.Resources.icoProgress3)) {
                    tryProgress.Icon = Bimil.Properties.Resources.icoProgress3;
                }
            } catch (NullReferenceException) { } //race condition causes tray icon internals to be disposed - lazy way out
        }

        private void bwType_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
            tryProgress.Visible = false;
            if (CloseAfterType) {
                Close();
            } else {
                tmrRestore.Enabled = true;
            }
        }

        private void tmrRestore_Tick(object sender, EventArgs e) {
            tmrRestore.Enabled = false;
            Visible = true;
            Select();
        }


        private Button AddGenericButton(int top, string caption, double heightMultiplier = 2) {
            var btn = new Button() { Text = caption, Left = ClientRectangle.Left, Width = ClientRectangle.Width, Top = top, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, AutoEllipsis = true };
            btn.Height = (int)(SystemInformation.HorizontalScrollBarHeight * 1.25 * heightMultiplier);

            Controls.Add(btn);

            return btn;
        }

        private Button AddTokenButton(int top, string caption, IEnumerable<AutotypeToken> tokens = null, Record record = null, bool isTextHidden = false, bool isDefinedAutoType = false) {
            var btn = AddGenericButton(top, caption, 3);

            btn.Text += "\n";
            if (isTextHidden) {
                btn.Text += "******";
            } else {
                if (tokens != null) {
                    btn.Text += " " + Helpers.GetAutotypeDescription(tokens);
                }
            };

            btn.Click += delegate {
                var processedTokens = GetProcessedTokens(record, tokens);
                ExecuteTokens(processedTokens, isDefinedAutoType);
            };

            Controls.Add(btn);

            return btn;
        }

        private IEnumerable<AutotypeToken> GetProcessedTokens(Record record, IEnumerable<AutotypeToken> tokens, AutotypeToken suffixToken = null) {
            var tokenList = new List<AutotypeToken>(AutotypeToken.GetAutotypeTokens(tokens, Entry));
            if (suffixToken != null) { tokenList.Add(suffixToken); }

            var processedTokens = new List<AutotypeToken>();
            foreach (var token in tokenList) {
                if ((token.Kind == AutotypeTokenKind.Command) && token.Content.Equals("TwoFactorCode", StringComparison.Ordinal)) {
                    var bytes = (record != null) ? record.GetBytes() : Entry.TwoFactorKey;
                    var key = OneTimePassword.ToBase32(bytes, bytes.Length, SecretFormatFlags.Spacing | SecretFormatFlags.Padding);
                    processedTokens.AddRange(AutotypeToken.GetAutotypeTokensFromText(Helpers.GetTwoFactorCode(key)));
                } else {
                    processedTokens.Add(token);
                }
            }

            return processedTokens.AsReadOnly();
        }

        private void AddSuffixButtons(Button button, IEnumerable<AutotypeToken> tokens, Record record) {
            var halfHeight = button.Height / 2;

            var tabButtonRectangle = new Rectangle(button.Right - halfHeight, button.Top, halfHeight, button.Height - halfHeight);
            var enterButtonRectangle = new Rectangle(button.Right - halfHeight, button.Bottom - halfHeight, halfHeight, halfHeight);

            var btnTab = AddSuffixButton(tabButtonRectangle, tokens, record, '\t');
            var btnEnter = AddSuffixButton(enterButtonRectangle, tokens, record, '\n');

            button.Parent.Controls.Add(btnTab);
            button.Parent.Controls.Add(btnEnter);

            button.Width -= halfHeight;
        }

        private Button AddSuffixButton(Rectangle rectangle, IEnumerable<AutotypeToken> tokens, Record record, char suffix) {
            var button = new Button() {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = rectangle.Location,
                Size = rectangle.Size,
            };

            AutotypeToken suffixToken = null;
            switch (suffix) {
                case '\t':
                    button.Name = "btnSuffixTab";
                    tip.SetToolTip(button, "Tab character as a suffix.");
                    suffixToken = new AutotypeToken("{Tab}", AutotypeTokenKind.Key);
                    break;

                case '\n':
                    button.Name = "btnSuffixEnter";
                    tip.SetToolTip(button, "Enter character as a suffix.");
                    suffixToken = new AutotypeToken("{Enter}", AutotypeTokenKind.Key);
                    break;
            }

            button.Click += delegate {
                var processedTokens = GetProcessedTokens(record, tokens, suffixToken);
                ExecuteTokens(processedTokens, closeAfterType: false);
            };

            Helpers.ScaleButton(button);
            return button;
        }

        private void ExecuteTokens(IEnumerable<AutotypeToken> tokens, bool closeAfterType) {
            Visible = false;

            CloseAfterType = closeAfterType;
            tryProgress.Icon = Bimil.Properties.Resources.icoProgress0;
            tryProgress.Visible = true;
            bwType.RunWorkerAsync(tokens);
        }


        private enum SuffixType {
            None,
            Tab,
            Enter
        }

    }
}
