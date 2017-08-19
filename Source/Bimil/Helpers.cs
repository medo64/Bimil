using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Medo.Security.Cryptography;
using Medo.Security.Cryptography.PasswordSafe;

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

                case RecordType.Autotype: return "Auto-type";
                case RecordType.RunCommand: return "Run command";

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

            yield return RecordType.Autotype;
            yield return RecordType.RunCommand;
        }

        public static bool GetIsHideable(RecordType recordType) {
            switch (recordType) {
                case RecordType.Password:
                case RecordType.TwoFactorKey:
                case RecordType.CreditCardVerificationValue:
                case RecordType.CreditCardPin:
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

        public static void PerformEntrySearch(Document document, ListView listView, String searchText, IEnumerable<Entry> entriesToSelect = null, bool extendedSearch = false, bool addMatchDescription = false, bool includeHidden = false) {
            var entriesToSelectList = new List<Entry>(entriesToSelect ?? new Entry[] { });
            var remainingEntriesToSelect = new List<Entry>(entriesToSelectList);

            var sw = Stopwatch.StartNew();

            //search for matches
            var resultList = new List<EntryCache>();
            if (document != null) {
                var words = new List<string>();
                var curWord = new StringBuilder();
                searchText = searchText.Trim();
                foreach (var ch in searchText) {
                    if (char.IsLetterOrDigit(ch) || char.IsPunctuation(ch)) {
                        curWord.Append(ch);
                    } else if (curWord.Length > 0) {
                        words.Add(curWord.ToString());
                        curWord.Length = 0;
                    }
                }
                if ((curWord.Length > 0) || (words.Count == 0)) { words.Add(curWord.ToString()); }

                foreach (var entry in document.Entries) {
                    var item = new EntryCache(entry);

                    var anyFailedChecks = false;
                    if (!searchText.Equals("*")) { //if text is literal *, show all
                        foreach (var text in words) {
                            var successfulCheck = false;

                            if (string.Equals(item.Group, searchText, StringComparison.CurrentCultureIgnoreCase)) { //if group name fully matches
                                item.AddMatch("Group");
                                successfulCheck = true;
                                break;
                            } else {
                                foreach (var groupPart in item.Group.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)) {
                                    if ((text.Length > 0) && groupPart.StartsWith(text, StringComparison.CurrentCultureIgnoreCase)) { //if group name starts with any word
                                        item.AddMatch("Group");
                                        successfulCheck = true;
                                        break;
                                    }
                                }
                            }

                            if ((text.Length > 0) && (item.Title.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0)) { //if text matches any part of title
                                item.AddMatch("Title");
                                successfulCheck = true;
                            }

                            if ((text.Length > 0) && extendedSearch) { //if other text fields are to be checked
                                foreach (var record in entry.Records) {
                                    if (record.RecordType == RecordType.Title) { continue; }
                                    if (record.RecordType == RecordType.Group) { continue; }
                                    var recordCaption = Helpers.GetRecordCaption(record.RecordType);
                                    if (recordCaption != null) { //so we know it is supported
                                        if (includeHidden || !Helpers.GetIsHideable(record.RecordType)) { //also check it is not hidden by default (e.g. password field)
                                            var recordText = record.Text;
                                            if (recordText != null) { //we have something searchable
                                                if (recordText.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0) { //if text matches any part of content
                                                    item.AddMatch(recordCaption);
                                                    successfulCheck = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            anyFailedChecks |= !successfulCheck;
                            if (anyFailedChecks) { break; }
                        }
                    }
                    if (anyFailedChecks) {
                        continue;
                    } else {
                        if (remainingEntriesToSelect.Contains(item.Entry)) { remainingEntriesToSelect.Remove(item.Entry); }
                        resultList.Add(item);
                    }
                }
            }
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Items searched at {0:0.0} ms", sw.ElapsedMilliseconds));

            //add selected entry if not added before
            if ((document != null) && (remainingEntriesToSelect.Count > 0)) {
                foreach (var entry in document.Entries) { //just loop over them to be sure entry to select actually exists currently
                    if (remainingEntriesToSelect.Contains(entry)) {
                        remainingEntriesToSelect.Remove(entry);
                        var item = new EntryCache(entry);
                        item.AddMatch("Selection");
                        resultList.Add(item);
                    }
                }
                Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Found selected entry at {0:0.0} ms", sw.ElapsedMilliseconds));
            }

            //sort, preferring the same group
            resultList.Sort((item1, item2) => {
                if (string.Equals(item1.Group, searchText, StringComparison.CurrentCultureIgnoreCase) && !string.Equals(item2.Group, searchText, StringComparison.CurrentCultureIgnoreCase)) {
                    return -1; //item1 is before item2
                } else if (!string.Equals(item1.Group, searchText, StringComparison.CurrentCultureIgnoreCase) && string.Equals(item2.Group, searchText, StringComparison.CurrentCultureIgnoreCase)) {
                    return +1; //item2 is before item1
                } else {
                    var groupCompare = string.Compare(item1.Group, item2.Group, StringComparison.CurrentCultureIgnoreCase);
                    return (groupCompare != 0) ? groupCompare : string.Compare(item1.Title, item2.Title, StringComparison.CurrentCultureIgnoreCase);
                }
            });
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Items sorted at {0:0.0} ms", sw.ElapsedMilliseconds));

            //show items
            listView.BeginUpdate();
            listView.Items.Clear();
            listView.Groups.Clear();

            var groupDictionary = new Dictionary<string, ListViewGroup>();
            foreach (var item in resultList) {
                if (!groupDictionary.TryGetValue(item.Group, out var group)) {
                    group = new ListViewGroup(string.IsNullOrEmpty(item.Group) ? "(no group)" : item.Group);
                    listView.Groups.Add(group);
                    groupDictionary.Add(item.Group, group);
                }
                var lvi = new ListViewItem(item.Title, group) { Tag = item.Entry };
                if (addMatchDescription && !string.IsNullOrEmpty(item.MatchedText)) { lvi.ToolTipText = "Matched: " + item.MatchedText; }
                listView.Items.Add(lvi);
            }

            listView.EndUpdate();
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Items updated at {0:0.0} ms", sw.ElapsedMilliseconds));

            if (listView.Items.Count > 0) {
                listView.Enabled = true;
                listView.ForeColor = SystemColors.WindowText;

                if (entriesToSelectList.Count > 0) {
                    foreach (ListViewItem item in listView.Items) {
                        var entry = (Entry)(item.Tag);
                        if (entriesToSelectList.Contains(entry)) {
                            item.Selected = true;
                            item.Focused = true;
                        }
                    }
                }

                if (listView.SelectedItems.Count == 0) {
                    listView.Items[0].Selected = true;
                    listView.Items[0].Focused = true;
                }

                listView.EnsureVisible(listView.SelectedItems[0].Index);
            } else {
                listView.Enabled = true;
                listView.ForeColor = SystemColors.GrayText;

                if ((document == null) || (document.Entries.Count == 0)) {
                    listView.Items.Add("No items.");
                } else {
                    listView.Items.Add("No matching items found (out of " + document.Entries.Count.ToString(CultureInfo.InvariantCulture) + ").");
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

                    if (item is ToolStripSplitButton toolstripSplitButton) { ScaleToolstrip(toolstripSplitButton.DropDown); }
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
            if ((bitmap == null) && !string.IsNullOrEmpty(item.Tag as string)) {
                bitmap = resources.GetObject(item.Tag + set) as Bitmap;
            }
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


        #region Read-only support

        internal static bool? GetReadOnly(string fileName) { //null if file cannot be found
            if (fileName == null) { return null; }
            if (!File.Exists(fileName)) { return null; }
            try {
                return (File.GetAttributes(fileName) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
            } catch (SystemException) { }
            return null;
        }

        internal static void SetReadOnly(string fileName, bool newReadOnly) {
            var attrs = (File.GetAttributes(fileName) & ~FileAttributes.ReadOnly);
            if (newReadOnly) { attrs |= FileAttributes.ReadOnly; }
            File.SetAttributes(fileName, attrs);
        }

        #endregion

        #region 2FA Support

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

        #endregion


        public static string ToTitleCase(string text) {
            var parts = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parts.Length; i++) {
                if (parts[i].Length > 0) {
                    parts[i] = parts[i].Substring(0, 1).ToUpper(CultureInfo.InvariantCulture) + parts[i].Substring(1).ToLower(CultureInfo.InvariantCulture);
                }
            }
            return string.Join(" ", parts);
        }


        public static string GetAutotypeDescription(string text) {
            var tokens = AutotypeToken.GetUnexpandedAutotypeTokens(text);
            var sb = new StringBuilder();
            foreach (var token in tokens) {
                if (sb.Length > 0) { sb.Append(" "); }
                sb.Append(token.ToString());
            }
            return sb.ToString();
        }


        public static bool IsRunningOnMono {
            get { return (Type.GetType("Mono.Runtime") != null); }
        }

    }
}
