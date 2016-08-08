using Medo.Security.Cryptography.PasswordSafe;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
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

    }
}
