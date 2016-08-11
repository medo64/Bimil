using Medo.Security.Cryptography;
using Medo.Security.Cryptography.PasswordSafe;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace Bimil {
    internal static class Helpers {

        #region ToolStripBorderlessProfessionalRenderer

        internal static ToolStripBorderlessProfessionalRenderer ToolStripBorderlessSystemRendererInstance { get { return new ToolStripBorderlessProfessionalRenderer(); } }

        internal class ToolStripBorderlessProfessionalRenderer : ToolStripProfessionalRenderer {

            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) {
            }

        }

        #endregion


        #region Record

        public static string GetRecordCaption(Record record) {
            return GetRecordCaption(record.RecordType);
        }

        public static string GetRecordCaption(RecordType recordType) {
            switch (recordType) {
                case RecordType.UserName: return "User name";
                case RecordType.Password: return "Password";
                case RecordType.Url: return "URL";
                case RecordType.EmailAddress: return "E-mail";
                case RecordType.Notes: return "Notes";

                case RecordType.TwoFactorKey: return "Two-factor key";
                case RecordType.CreditCardNumber: return "Card number";
                case RecordType.CreditCardExpiration: return "Card expiration";
                case RecordType.CreditCardVerificationValue: return "Card security code";
                case RecordType.CreditCardPin: return "Card PIN";

                case RecordType.QRCode: return "QR Code";

                case RecordType.PasswordHistory: return "Password history";

                default: return null; //all other fields are not really supported
            }
        }

        public static IEnumerable<RecordType> GetUsableRecordTypes() {
            yield return RecordType.UserName;
            yield return RecordType.Password;
            yield return RecordType.Url;

            yield return RecordType.EmailAddress;
            yield return RecordType.TwoFactorKey;

            yield return RecordType.CreditCardNumber;
            yield return RecordType.CreditCardExpiration;
            yield return RecordType.CreditCardVerificationValue;
            yield return RecordType.CreditCardPin;

            yield return RecordType.QRCode;

            yield return RecordType.Notes;

            yield return RecordType.PasswordHistory;
        }

        public static bool GetIsHideable(RecordType recordType) {
            switch (recordType) {
                case RecordType.Password:
                case RecordType.TwoFactorKey:
                case RecordType.CreditCardVerificationValue:
                case RecordType.CreditCardPin:
                case RecordType.PasswordHistory:
                    return true;

                default: return false; //all other fields are visible by default
            }
        }

        #endregion

        public static int GetNearestComboIndex(string text, ComboBox.ObjectCollection items, int defaultIndex = -1) {
            if (text.Length > 0) {
                //check for full match
                for (var i = 0; i < items.Count; i++) {
                    if (items[i].ToString().Equals(text, StringComparison.CurrentCultureIgnoreCase)) { return i; }
                }

                //check for prefix match
                for (var i = 0; i < items.Count; i++) {
                    if (items[i].ToString().StartsWith(text, StringComparison.CurrentCultureIgnoreCase)) { return i; }
                }

                //check for any
                for (var i = 0; i < items.Count; i++) {
                    if (items[i].ToString().IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0) { return i; }
                }
            }

            //give up and return default
            return defaultIndex;
        }


        #region Entry search

        public static void PerformEntrySearch(Document document, ListView lsvEntries, String text, Entry entryToSelect = null, bool extendedSearch = false, bool addMatchDescription = false) {
            var sw = Stopwatch.StartNew();

            //search for matches
            var resultList = new List<EntryCache>();
            if (document != null) {
                foreach (var entry in document.Entries) {
                    var item = new EntryCache(entry);
                    if (string.Equals(item.Group, text, StringComparison.CurrentCultureIgnoreCase)) {
                        item.AddMatch("Group");
                    }
                    if ((text.Length > 0) && (item.Title.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0)) {
                        item.AddMatch("Title");
                    }
                    if ((text.Length > 0) && extendedSearch) {
                        foreach (var record in entry.Records) {
                            if (record.RecordType == RecordType.Title) { continue; }
                            if (record.RecordType == RecordType.Group) { continue; }
                            var recordCaption = Helpers.GetRecordCaption(record.RecordType);
                            if (recordCaption != null) { //so we know it is supported
                                if (!Helpers.GetIsHideable(record.RecordType)) { //also check it is not hidden by default (e.g. password field)
                                    var recordText = record.Text;
                                    if (recordText != null) { //we have something searchable
                                        if (recordText.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0) {
                                            item.AddMatch(recordCaption);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(item.MatchedText)) {
                        resultList.Add(item);
                    }
                }
            }
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Items searched at {0:0.0} ms", sw.ElapsedMilliseconds));

            //sort, preferring the same group
            resultList.Sort((item1, item2) => {
                if (string.Equals(item1.Group, text, StringComparison.CurrentCultureIgnoreCase) && !string.Equals(item2.Group, text, StringComparison.CurrentCultureIgnoreCase)) {
                    return -1; //item1 is before item2
                } else if (!string.Equals(item1.Group, text, StringComparison.CurrentCultureIgnoreCase) && string.Equals(item2.Group, text, StringComparison.CurrentCultureIgnoreCase)) {
                    return +1; //item2 is before item1
                } else {
                    var groupCompare = string.Compare(item1.Group, item2.Group, StringComparison.CurrentCultureIgnoreCase);
                    return (groupCompare != 0) ? groupCompare : string.Compare(item1.Title, item2.Title, StringComparison.CurrentCultureIgnoreCase);
                }
            });
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Items sorted at {0:0.0} ms", sw.ElapsedMilliseconds));

            //show items
            lsvEntries.BeginUpdate();
            lsvEntries.Items.Clear();
            lsvEntries.Groups.Clear();

            var groupDictionary = new Dictionary<string, ListViewGroup>();
            foreach (var item in resultList) {
                ListViewGroup group;
                if (!groupDictionary.TryGetValue(item.Group, out group)) {
                    group = new ListViewGroup(string.IsNullOrEmpty(item.Group) ? "(no group)" : item.Group);
                    lsvEntries.Groups.Add(group);
                    groupDictionary.Add(item.Group, group);
                }
                var lvi = new ListViewItem(item.Title, group) { Tag = item.Entry };
                if (addMatchDescription && !string.IsNullOrEmpty(item.MatchedText)) { lvi.ToolTipText = "Matched: " + item.MatchedText; }
                lsvEntries.Items.Add(lvi);
            }

            lsvEntries.EndUpdate();
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Items updated at {0:0.0} ms", sw.ElapsedMilliseconds));

            if (lsvEntries.Items.Count > 0) {
                lsvEntries.Enabled = true;
                lsvEntries.ForeColor = SystemColors.WindowText;

                if (entryToSelect != null) {
                    foreach (ListViewItem item in lsvEntries.Items) {
                        var entry = (Entry)(item.Tag);
                        if (entry.Equals(entryToSelect)) {
                            item.Selected = true;
                            item.Focused = true;
                            break;
                        }
                    }
                }

                if (lsvEntries.SelectedItems.Count == 0) {
                    lsvEntries.Items[0].Selected = true;
                    lsvEntries.Items[0].Focused = true;
                }

                lsvEntries.EnsureVisible(lsvEntries.SelectedItems[0].Index);
            } else {
                lsvEntries.Enabled = false;
                lsvEntries.ForeColor = SystemColors.GrayText;

                if ((document == null) || (document.Entries.Count == 0)) {
                    lsvEntries.Items.Add("No items.");
                } else {
                    lsvEntries.Items.Add("No matching items found.");
                }
            }

            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Items refreshed at {0:0.0} ms", sw.ElapsedMilliseconds));
        }

        public static bool HandleSearchKeyDown(KeyEventArgs e, ListView lsvEntries, bool processPageUpDown = false) {
            switch (e.KeyData) {
                case Keys.Down:
                    if (lsvEntries.Items.Count > 0) {
                        if (lsvEntries.SelectedIndices.Count == 0) {
                            lsvEntries.Items[0].Selected = true;
                        } else {
                            int index = Math.Min(lsvEntries.SelectedIndices[lsvEntries.SelectedIndices.Count - 1] + 1, lsvEntries.Items.Count - 1);
                            foreach (ListViewItem item in lsvEntries.Items) { item.Selected = false; }
                            lsvEntries.Items[index].Selected = true;
                            lsvEntries.Items[index].Focused = true;
                        }
                        lsvEntries.EnsureVisible(lsvEntries.SelectedItems[0].Index);
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    return true;

                case Keys.Up:
                    if (lsvEntries.Items.Count > 0) {
                        if (lsvEntries.SelectedIndices.Count == 0) {
                            lsvEntries.Items[lsvEntries.Items.Count - 1].Selected = true;
                        } else {
                            int index = Math.Max(lsvEntries.SelectedIndices[0] - 1, 0);
                            foreach (ListViewItem item in lsvEntries.Items) { item.Selected = false; }
                            lsvEntries.Items[index].Selected = true;
                            lsvEntries.Items[index].Focused = true;
                        }
                        lsvEntries.EnsureVisible(lsvEntries.SelectedItems[0].Index);
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    return true;

                case Keys.PageDown:
                    if (!processPageUpDown) { return false; }
                    if (lsvEntries.Items.Count > 0) {
                        var delta = lsvEntries.ClientSize.Height / (lsvEntries.Items[0].Bounds.Height * 2);
                        if (lsvEntries.SelectedIndices.Count == 0) {
                            lsvEntries.Items[0].Selected = true;
                        } else {
                            int index = Math.Min(lsvEntries.SelectedIndices[lsvEntries.SelectedIndices.Count - 1] + delta, lsvEntries.Items.Count - 1);
                            foreach (ListViewItem item in lsvEntries.Items) { item.Selected = false; }
                            lsvEntries.Items[index].Selected = true;
                            lsvEntries.Items[index].Focused = true;
                        }
                        lsvEntries.EnsureVisible(lsvEntries.SelectedItems[0].Index);
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    return true;

                case Keys.PageUp:
                    if (!processPageUpDown) { return false; }
                    if (lsvEntries.Items.Count > 0) {
                        var delta = lsvEntries.ClientSize.Height / (lsvEntries.Items[0].Bounds.Height * 2);
                        if (lsvEntries.SelectedIndices.Count == 0) {
                            lsvEntries.Items[lsvEntries.Items.Count - 1].Selected = true;
                        } else {
                            int index = Math.Max(lsvEntries.SelectedIndices[0] - delta, 0);
                            foreach (ListViewItem item in lsvEntries.Items) { item.Selected = false; }
                            lsvEntries.Items[index].Selected = true;
                            lsvEntries.Items[index].Focused = true;
                        }
                        lsvEntries.EnsureVisible(lsvEntries.SelectedItems[0].Index);
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    return true;

                case Keys.Enter:
                    if (lsvEntries.Items.Count > 0) {
                        lsvEntries.Select();
                        e.SuppressKeyPress = true;
                    }
                    return true;
            }

            return false;
        }

        #endregion


        #region Toolstrip images

        internal static void ScaleToolstrip(params ToolStrip[] toolstrips) {
            var sizeAndSet = GetSizeAndSet(toolstrips);
            var size = sizeAndSet.Key;
            var set = sizeAndSet.Value;

            var resources = Bimil.Properties.Resources.ResourceManager;
            foreach (var toolstrip in toolstrips) {
                toolstrip.ImageScalingSize = new Size(size, size);
                foreach (ToolStripItem item in toolstrip.Items) {
                    item.ImageScaling = ToolStripItemImageScaling.None;
                    if (item.Image != null) { //update only those already having image
                        Bitmap bitmap = null;
                        if (!string.IsNullOrEmpty(item.Name)) {
                            bitmap = resources.GetObject(item.Name + set) as Bitmap;
                        }
                        if ((bitmap == null) && !string.IsNullOrEmpty(item.Tag as string)) {
                            bitmap = resources.GetObject(item.Tag + set) as Bitmap;
                        }

                        item.ImageScaling = ToolStripItemImageScaling.None;
#if DEBUG
                        item.Image = (bitmap != null) ? new Bitmap(bitmap, size, size) : new Bitmap(size, size, PixelFormat.Format8bppIndexed);
#else
                        if (bitmap != null) { item.Image = new Bitmap(bitmap, size, size); }
#endif
                    }

                    var toolstripSplitButton = item as ToolStripSplitButton;
                    if (toolstripSplitButton != null) { ScaleToolstrip(toolstripSplitButton.DropDown); }
                }
            }
        }

        internal static void ScaleToolstripItem(ToolStripItem item, string name) {
            var sizeAndSet = GetSizeAndSet(item.GetCurrentParent());
            var size = sizeAndSet.Key;
            var set = sizeAndSet.Value;

            var resources = Bimil.Properties.Resources.ResourceManager;
            Bitmap bitmap = resources.GetObject(name + set) as Bitmap;
            item.ImageScaling = ToolStripItemImageScaling.None;
#if DEBUG
            item.Image = (bitmap != null) ? new Bitmap(bitmap, size, size) : new Bitmap(size, size, PixelFormat.Format8bppIndexed);
#else
            if (bitmap != null) { item.Image = new Bitmap(bitmap, size, size); }
#endif
        }

        internal static void ScaleButton(Button item) {
            var sizeAndSet = GetSizeAndSet(item);
            var size = sizeAndSet.Key;
            var set = sizeAndSet.Value;

            var resources = Bimil.Properties.Resources.ResourceManager;
            var bitmap = resources.GetObject(item.Name + set) as Bitmap;
#if DEBUG
            item.Image = (bitmap != null) ? new Bitmap(bitmap, size, size) : new Bitmap(size, size, PixelFormat.Format8bppIndexed);
#else
            item.Image = (bitmap != null) ? new Bitmap(bitmap, size, size) : null;
#endif
        }

        internal static void ScaleImage(PictureBox pictureBox, string nameRoot, double scaleBoost = 1) {
            var sizeAndSet = GetSizeAndSet(scaleBoost, pictureBox);
            var size = sizeAndSet.Key;
            var set = sizeAndSet.Value;

            var resources = Bimil.Properties.Resources.ResourceManager;
            var bitmap = resources.GetObject(nameRoot + set) as Bitmap;
#if DEBUG
            pictureBox.Image = (bitmap != null) ? new Bitmap(bitmap, size, size) : new Bitmap(size, size, PixelFormat.Format8bppIndexed);
#else
            pictureBox.Image = (bitmap != null) ? new Bitmap(bitmap, size, size) : null;
#endif
        }

        internal static ImageList GetImageList(Form form, params string[] names) {
            var sizeAndSet = GetSizeAndSet(form);
            var size = sizeAndSet.Key;
            var set = sizeAndSet.Value;

            var imageList = new ImageList() { ColorDepth = ColorDepth.Depth32Bit, ImageSize = new Size(size, size) };

            var resources = Bimil.Properties.Resources.ResourceManager;
            foreach (var name in names) {
                var bitmap = resources.GetObject(name + set) as Bitmap;
                imageList.Images.Add(bitmap);
            }

            return imageList;
        }

        private static KeyValuePair<int, string> GetSizeAndSet(params Control[] controls) {
            return GetSizeAndSet(Settings.ScaleBoost, controls);
        }

        private static KeyValuePair<int, string> GetSizeAndSet(double scaleBoost, params Control[] controls) {
            using (var g = controls[0].CreateGraphics()) {
                var scale = Math.Max(Math.Max(g.DpiX, g.DpiY), 96.0) / 96.0;
                scale += scaleBoost;

                if (scale < 1.5) {
                    return new KeyValuePair<int, string>(16, "_16");
                } else if (scale < 2) {
                    return new KeyValuePair<int, string>(24, "_24");
                } else if (scale < 3) {
                    return new KeyValuePair<int, string>(32, "_32");
                } else {
                    var base32 = 16 * scale / 32;
                    var base48 = 16 * scale / 48;
                    if ((base48 - (int)base48) < (base32 - (int)base32)) {
                        return new KeyValuePair<int, string>(48 * (int)base48, "_48");
                    } else {
                        return new KeyValuePair<int, string>(32 * (int)base32, "_32");
                    }
                }
            }
        }

        #endregion


        public static readonly char[] Base32Characters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '2', '3', '4', '5', '6', '7' };
        public static readonly char[] NumberCharacters = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        public static string FilterText(string text, char[] allowedCopyCharacters) {
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

        public static string GetTwoFactorCode(string text, bool space = false) {
            var key = Helpers.FilterText(text.ToUpperInvariant(), Helpers.Base32Characters);
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


        #region AutoType

        public enum AutoTypeTokenType {
            DoExpand,
            DoText,
            Command,
            Type,
        }

        public class AutoTypeToken {
            internal AutoTypeToken(string text, AutoTypeTokenType type = AutoTypeTokenType.DoText, string argument = null) {
                if (type == AutoTypeTokenType.DoText) {
                    var sb = new StringBuilder();
                    foreach (var ch in text) {
                        switch (ch) {
                            case '+':
                            case '^':
                            case '%':
                            case '~':
                            case '(':
                            case ')':
                            case '{':
                            case '}':
                            case '[':
                            case ']':
                                sb.Append("{" + ch + "}");
                                break;

                            case '\b':
                                sb.Append("{Backspace}");
                                break;

                            case '\n':
                            case '\r':
                                sb.Append("{Enter}");
                                break;

                            case '\t':
                                sb.Append("{Tab}");
                                break;

                            default:
                                sb.Append(ch);
                                break;
                        }
                    }
                    this.Type = AutoTypeTokenType.Type;
                    this.Text = sb.ToString();
                } else {
                    this.Type = type;
                    this.Text = text;
                }

                this.Argument = argument;
            }

            internal AutoTypeToken(char text, AutoTypeTokenType type = AutoTypeTokenType.DoText, string argument = null) : this(text.ToString(), type, argument) { }

            public AutoTypeTokenType Type { get; }
            public string Text { get; }
            public string Argument { get; }

            public override string ToString() {
                switch (this.Type) {
                    case AutoTypeTokenType.DoText: return this.Text;
                    case AutoTypeTokenType.DoExpand: return @"\" + this.Text + ((this.Argument != null) ? this.Argument : "");
                    case AutoTypeTokenType.Command: return @"*" + this.Text + ((this.Argument != null) ? ":" + this.Argument : "") + "*";
                    case AutoTypeTokenType.Type: return this.Text;
                }
                return "";
            }
        }

        public static ReadOnlyCollection<AutoTypeToken> GetAutoTypeTokens(string autotypeText, Entry entry = null) {
            var tokens = new List<AutoTypeToken>();

            if (string.IsNullOrEmpty(autotypeText)) { //Default: User-name <Tab> Password <Tab> <Enter>

                tokens.Add(new AutoTypeToken("u", AutoTypeTokenType.DoExpand));
                tokens.Add(new AutoTypeToken("t", AutoTypeTokenType.DoExpand));
                tokens.Add(new AutoTypeToken("p", AutoTypeTokenType.DoExpand));
                tokens.Add(new AutoTypeToken("t", AutoTypeTokenType.DoExpand));
                tokens.Add(new AutoTypeToken("n", AutoTypeTokenType.DoExpand));

            } else {

                var state = AutoTypeState.Default;
                var sbText = new StringBuilder();
                char? command = null;
                var sbCommandArguments = new StringBuilder();
                foreach (var ch in autotypeText) {
                    switch (state) {
                        case AutoTypeState.Default:
                            if (ch == '\\') {
                                state = AutoTypeState.Escape;
                            } else {
                                sbText.Append(ch);
                            }
                            break;

                        case AutoTypeState.Escape:
                            switch (ch) {
                                case 'u':
                                case 'p':
                                case '2':
                                case 'g':
                                case 'i':
                                case 'l':
                                case 'm':
                                case 'b':
                                case 't':
                                case 's':
                                case 'n':
                                case 'z': //single character escape
                                    if (sbText.Length > 0) { tokens.Add(new AutoTypeToken(sbText.ToString())); sbText.Length = 0; }
                                    tokens.Add(new AutoTypeToken(ch, AutoTypeTokenType.DoExpand));
                                    state = AutoTypeState.Default;
                                    break;

                                case 'c': //double character escape
                                    if (sbText.Length > 0) { tokens.Add(new AutoTypeToken(sbText.ToString())); sbText.Length = 0; }
                                    state = AutoTypeState.EscapeCreditCard;
                                    break;

                                case 'd':
                                case 'w':
                                case 'W': //mandatory number characters
                                    if (sbText.Length > 0) { tokens.Add(new AutoTypeToken(sbText.ToString())); sbText.Length = 0; }
                                    command = ch;
                                    state = AutoTypeState.EscapeMandatoryNumber;
                                    break;

                                case 'o': //optional number characters
                                    if (sbText.Length > 0) { tokens.Add(new AutoTypeToken(sbText.ToString())); sbText.Length = 0; }
                                    command = ch;
                                    state = AutoTypeState.EscapeOptionalNumber;
                                    break;

                                default: //if escape doesn't exist
                                    sbText.Append(ch);
                                    state = AutoTypeState.Default;
                                    break;
                            }
                            break;

                        case AutoTypeState.EscapeCreditCard:
                            switch (ch) {
                                case 'n':
                                case 'e':
                                case 'v':
                                case 'p': //double character escapes
                                    if (sbText.Length > 0) { tokens.Add(new AutoTypeToken(sbText.ToString())); sbText.Length = 0; }
                                    tokens.Add(new AutoTypeToken("c" + ch, AutoTypeTokenType.DoExpand));
                                    state = AutoTypeState.Default;
                                    break;

                                default: //if escape doesn't exist
                                    sbText.Append("c" + ch);
                                    state = AutoTypeState.Default;
                                    break;
                            }
                            break;

                        case AutoTypeState.EscapeMandatoryNumber:
                            if (char.IsDigit(ch)) {
                                sbCommandArguments.Append(ch);
                                state = AutoTypeState.EscapeOptionalNumber;
                            } else {
                                sbText.Append(command + sbCommandArguments.ToString() + ch);
                                state = AutoTypeState.Default;
                            }
                            break;


                        case AutoTypeState.EscapeOptionalNumber:
                            if (char.IsDigit(ch) && (sbCommandArguments.Length < 3)) {
                                sbCommandArguments.Append(ch);
                            } else {
                                tokens.Add(new AutoTypeToken(command.Value, AutoTypeTokenType.DoExpand, sbCommandArguments.ToString()));
                                command = null; sbCommandArguments.Length = 0;
                                sbText.Append(ch);
                                state = AutoTypeState.Default;
                            }
                            break;

                        default: throw new NotImplementedException("Unknown state");
                    }
                }

                if (sbText.Length > 0) {
                    tokens.Add(new AutoTypeToken(sbText.ToString()));
                } else if (command.HasValue) {
                    if ((sbCommandArguments.Length == 0) && ((command == 'd') || (command == 'w') || (command == 'W'))) {
                        tokens.Add(new AutoTypeToken(command + sbCommandArguments.ToString()));
                    } else {
                        tokens.Add(new AutoTypeToken(command.Value, AutoTypeTokenType.DoExpand, sbCommandArguments.ToString()));
                    }
                }

            }


            //if record was provided, replace text
            if (entry != null) {
                var newTokens = new List<AutoTypeToken>();

                foreach (var token in tokens) {
                    if (token.Type == AutoTypeTokenType.DoExpand) { //command is here
                        switch (token.Text) { //command character
                            case "u": //Username
                                newTokens.Add(new AutoTypeToken(entry.UserName));
                                break;

                            case "p": //Password
                                newTokens.Add(new AutoTypeToken(entry.Password));
                                break;

                            case "2": //Two-factor code
                                var keyBytes = entry.TwoFactorKey;
                                if (keyBytes.Length > 0) {
                                    var key = OneTimePassword.ToBase32(keyBytes, keyBytes.Length, SecretFormatFlags.Spacing | SecretFormatFlags.Padding);
                                    newTokens.Add(new AutoTypeToken(GetTwoFactorCode(key)));
                                }
                                break;

                            case "cn":
                                newTokens.Add(new AutoTypeToken(entry.CreditCardNumber));
                                break;

                            case "ce":
                                newTokens.Add(new AutoTypeToken(entry.CreditCardExpiration));
                                break;

                            case "cv":
                                newTokens.Add(new AutoTypeToken(entry.CreditCardVerificationValue));
                                break;

                            case "cp":
                                newTokens.Add(new AutoTypeToken(entry.CreditCardPin));
                                break;

                            case "g":
                                newTokens.Add(new AutoTypeToken(entry.Group));
                                break;

                            case "i":
                                newTokens.Add(new AutoTypeToken(entry.Title));
                                break;

                            case "l":
                                newTokens.Add(new AutoTypeToken(entry.Url));
                                break;

                            case "m":
                                newTokens.Add(new AutoTypeToken(entry.Email));
                                break;

                            case "o":
                                var noteLines = entry.Notes.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
                                if (string.IsNullOrEmpty(token.Argument)) {
                                    newTokens.Add(new AutoTypeToken(string.Join("\n", noteLines)));
                                } else {
                                    int lineNumber;
                                    if (int.TryParse(token.Argument, NumberStyles.Integer, CultureInfo.InvariantCulture, out lineNumber)) {
                                        if (lineNumber <= noteLines.Length) {
                                            var lineText = noteLines[lineNumber - 1];
                                            if (lineText.Length > 0) {
                                                newTokens.Add(new AutoTypeToken(lineText));
                                            }
                                        }
                                    }
                                }
                                break;

                            case "b":
                                newTokens.Add(new AutoTypeToken("{Backspace}", AutoTypeTokenType.Type));
                                break;

                            case "t":
                                newTokens.Add(new AutoTypeToken("{Tab}", AutoTypeTokenType.Type));
                                break;

                            case "s":
                                newTokens.Add(new AutoTypeToken("+{Tab}", AutoTypeTokenType.Type));
                                break;

                            case "n":
                                newTokens.Add(new AutoTypeToken("{Enter}", AutoTypeTokenType.Type));
                                break;

                            case "d":
                                newTokens.Add(new AutoTypeToken("Delay", AutoTypeTokenType.Command, token.Argument));
                                break;

                            case "w":
                                newTokens.Add(new AutoTypeToken("Wait", AutoTypeTokenType.Command, token.Argument));
                                break;

                            case "W":
                                newTokens.Add(new AutoTypeToken("Wait", AutoTypeTokenType.Command, token.Argument + "000"));
                                break;

                            case "z":
                                newTokens.Add(new AutoTypeToken("CopyPaste", AutoTypeTokenType.Command));
                                break;
                        }
                    } else {
                        newTokens.Add(token);
                    }
                }

                tokens = newTokens;

#if DEBUG
                foreach (var token in tokens) {
                    Debug.Assert((token.Type != AutoTypeTokenType.DoExpand) && (token.Type != AutoTypeTokenType.DoText));
                }
#endif
            }

            return tokens.AsReadOnly();
        }

        private enum AutoTypeState {
            Default,
            Escape,
            EscapeCreditCard,
            EscapeMandatoryNumber,
            EscapeOptionalNumber,
            EscapeNumber,
        }

        #endregion


        public static bool IsRunningOnMono {
            get { return (Type.GetType("Mono.Runtime") != null); }
        }

    }
}
