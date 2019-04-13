using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Bimil {
    internal partial class PasswordGeneratorForm : Form {
        public PasswordGeneratorForm(bool noSave = true) {
            InitializeComponent();
            Font = SystemFonts.MessageBoxFont;

            if (noSave) {
                btnGenerate.Location = btnSaveAndCopy.Location;
                btnGenerate.Visible = true;
                btnCopy.Visible = true;
                btnSaveAndCopy.Visible = false;
                btnSave.Visible = false;
            } else {
                btnGenerate.Visible = true;
                btnCopy.Visible = false;
                btnSaveAndCopy.Visible = true;
                btnSave.Visible = true;
            }
        }

        protected override bool ProcessDialogKey(Keys keyData) {
            if (keyData == Keys.Escape) {
                Close();
                return true;
            } else {
                return base.ProcessDialogKey(keyData);
            }
        }

        #region Disable minimize

        protected override void WndProc(ref Message m) {
            if ((Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major < 10)
                && (m != null) && (m.Msg == NativeMethods.WM_SYSCOMMAND) && (m.WParam == NativeMethods.SC_MINIMIZE)) {
                m.Result = IntPtr.Zero;
            } else if (m != null) {
                base.WndProc(ref m);
            }
        }


        private class NativeMethods {
            internal const int WM_SYSCOMMAND = 0x0112;
            internal static readonly IntPtr SC_MINIMIZE = new IntPtr(0xF020);
        }

        #endregion


        private void Form_Load(object sender, EventArgs e) {
            { //Word
                chbWordIncludeUpperCase.Checked = Settings.PasswordGeneratorWordIncludeUpperCase;
                chbWordIncludeNumber.Checked = Settings.PasswordGeneratorWordIncludeNumber;
                chbWordIncludeSpecialCharacter.Checked = Settings.PasswordGeneratorWordIncludeSpecialCharacter;
                chbWordIncludeIncomplete.Checked = Settings.PasswordGeneratorWordIncludeIncomplete;

                chbWordRestrictAddSpace.Checked = Settings.PasswordGeneratorWordRestrictAddSpace;
                chbWordRestrictBreak.Checked = Settings.PasswordGeneratorWordRestrictBreak;
                chbWordRestrictTitleCase.Checked = Settings.PasswordGeneratorWordRestrictTitleCase;
                chbWordRestrictSuffixOnly.Checked = Settings.PasswordGeneratorWordRestrictSuffixOnly;

                txtWordCount.Text = Settings.PasswordGeneratorWordCount.ToString("0", CultureInfo.CurrentCulture);
            }

            { //Triplet
                chbTripletIncludeNumber.Checked = Settings.PasswordGeneratorTripletIncludeNumber;
                chbTripletIncludeSpecialCharacter.Checked = Settings.PasswordGeneratorTripletIncludeSpecialCharacter;
                chbTripletIncludeRandomUpperCase.Checked = Settings.PasswordGeneratorTripletRandomUpperCase;
                chbTripletIncludeRandomLetterDrop.Checked = Settings.PasswordGeneratorTripletRandomLetterDrop;

                chbTripletRestrictTitleCase.Checked = Settings.PasswordGeneratorTripletRestrictTitleCase;
                chbTripletRestrictBreak.Checked = Settings.PasswordGeneratorTripletRestrictBreak;
                chbTripletRestrictSuffixOnly.Checked = Settings.PasswordGeneratorTripletRestrictSuffixOnly;
                chbTripletRestrictAddSpace.Checked = Settings.PasswordGeneratorTripletRestrictAddSpace;

                txtWordCount.Text = Settings.PasswordGeneratorTripletCount.ToString("0", CultureInfo.CurrentCulture);
            }

            { //Classic
                chbIncludeUpperCase.Checked = Settings.PasswordGeneratorIncludeUpperCase;
                chbIncludeLowerCase.Checked = Settings.PasswordGeneratorIncludeLowerCase;
                chbIncludeNumbers.Checked = Settings.PasswordGeneratorIncludeNumbers;
                chbIncludeSpecialCharacters.Checked = Settings.PasswordGeneratorIncludeSpecialCharacters;

                chbRestrictSimilar.Checked = Settings.PasswordGeneratorRestrictSimilar;
                chbRestrictMovable.Checked = Settings.PasswordGeneratorRestrictMovable;
                chbRestrictPronounceable.Checked = Settings.PasswordGeneratorRestrictPronounceable;
                chbRestrictRepeated.Checked = Settings.PasswordGeneratorRestrictRepeated;

                txtLength.Text = Settings.PasswordGeneratorLength.ToString("0", CultureInfo.CurrentCulture);
            }

            switch (Settings.PasswordGeneratorTabSelection) {
                case Settings.PasswordGeneratorTab.Word:
                    tabStyle.SelectedTab = tabStyle_Word;
                    break;
                case Settings.PasswordGeneratorTab.Triplet:
                    tabStyle.SelectedTab = tabStyle_Triplet;
                    break;
                case Settings.PasswordGeneratorTab.Classic:
                    tabStyle.SelectedTab = tabStyle_Classic;
                    break;
            }

            btnGenerate_Click(null, null);
        }

        private void Form_FormClosed(object sender, FormClosedEventArgs e) {
            { //Word
                Settings.PasswordGeneratorWordIncludeUpperCase = chbWordIncludeUpperCase.Checked;
                Settings.PasswordGeneratorWordIncludeNumber = chbWordIncludeNumber.Checked;
                Settings.PasswordGeneratorWordIncludeSpecialCharacter = chbWordIncludeSpecialCharacter.Checked;
                Settings.PasswordGeneratorWordIncludeIncomplete = chbWordIncludeIncomplete.Checked;

                Settings.PasswordGeneratorWordRestrictAddSpace = chbWordRestrictAddSpace.Checked;
                Settings.PasswordGeneratorWordRestrictBreak = chbWordRestrictBreak.Checked;
                Settings.PasswordGeneratorWordRestrictTitleCase = chbWordRestrictTitleCase.Checked;
                Settings.PasswordGeneratorWordRestrictSuffixOnly = chbWordRestrictSuffixOnly.Checked;

                if (int.TryParse(txtWordCount.Text, NumberStyles.Integer, CultureInfo.CurrentCulture, out var count)) {
                    Settings.PasswordGeneratorWordCount = count;
                }
            }

            { //Triplet
                Settings.PasswordGeneratorTripletIncludeNumber = chbTripletIncludeNumber.Checked;
                Settings.PasswordGeneratorTripletIncludeSpecialCharacter = chbTripletIncludeSpecialCharacter.Checked;
                Settings.PasswordGeneratorTripletRandomUpperCase = chbTripletIncludeRandomUpperCase.Checked;
                Settings.PasswordGeneratorTripletRandomLetterDrop = chbTripletIncludeRandomLetterDrop.Checked;

                Settings.PasswordGeneratorTripletRestrictTitleCase = chbTripletRestrictTitleCase.Checked;
                Settings.PasswordGeneratorTripletRestrictBreak = chbTripletRestrictBreak.Checked;
                Settings.PasswordGeneratorTripletRestrictSuffixOnly = chbTripletRestrictSuffixOnly.Checked;
                Settings.PasswordGeneratorTripletRestrictAddSpace = chbTripletRestrictAddSpace.Checked;

                if (int.TryParse(txtTripletCount.Text, NumberStyles.Integer, CultureInfo.CurrentCulture, out var count)) {
                    Settings.PasswordGeneratorTripletCount = count;
                }
            }

            { //Classic
                Settings.PasswordGeneratorIncludeUpperCase = chbIncludeUpperCase.Checked;
                Settings.PasswordGeneratorIncludeLowerCase = chbIncludeLowerCase.Checked;
                Settings.PasswordGeneratorIncludeNumbers = chbIncludeNumbers.Checked;
                Settings.PasswordGeneratorIncludeSpecialCharacters = chbIncludeSpecialCharacters.Checked;

                Settings.PasswordGeneratorRestrictSimilar = chbRestrictSimilar.Checked;
                Settings.PasswordGeneratorRestrictMovable = chbRestrictMovable.Checked;
                Settings.PasswordGeneratorRestrictPronounceable = chbRestrictPronounceable.Checked;
                Settings.PasswordGeneratorRestrictRepeated = chbRestrictRepeated.Checked;

                if (int.TryParse(txtLength.Text, NumberStyles.Integer, CultureInfo.CurrentCulture, out var length)) {
                    Settings.PasswordGeneratorLength = length;
                }
            }

            if (tabStyle.SelectedTab.Equals(tabStyle_Word)) {
                Settings.PasswordGeneratorTabSelection = Settings.PasswordGeneratorTab.Word;
            } else if (tabStyle.SelectedTab.Equals(tabStyle_Triplet)) {
                Settings.PasswordGeneratorTabSelection = Settings.PasswordGeneratorTab.Triplet;
            } else {
                Settings.PasswordGeneratorTabSelection = Settings.PasswordGeneratorTab.Classic;
            }
        }


        private void chbIncludeUpperCase_CheckedChanged(object sender, EventArgs e) {
            if (chbIncludeUpperCase.Checked == false) { chbIncludeLowerCase.Checked = true; }
            btnGenerate_Click(null, null);
        }

        private void chbIncludeLowerCase_CheckedChanged(object sender, EventArgs e) {
            if (chbIncludeLowerCase.Checked == false) { chbIncludeUpperCase.Checked = true; }
            btnGenerate_Click(null, null);
        }


        private void txtNumber_KeyDown(object sender, KeyEventArgs e) {
            var textBox = (TextBox)sender;

            switch (e.KeyData) {
                case Keys.D0:
                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                    break;

                case Keys.Left:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.Back:
                case Keys.Delete:
                case Keys.Alt | Keys.F4:
                    break;

                case Keys.PageUp:
                    ChangeLength(textBox, -10);
                    e.SuppressKeyPress = true;
                    break;

                case Keys.PageDown:
                    ChangeLength(textBox, +10);
                    e.SuppressKeyPress = true;
                    break;

                default:
                    e.SuppressKeyPress = true;
                    break;
            }
        }

        private void txtWordCount_Leave(object sender, EventArgs e) {
            if (!int.TryParse(txtWordCount.Text, NumberStyles.Integer, CultureInfo.CurrentCulture, out var count)) {
                count = Settings.PasswordGeneratorWordCount;
            }
            txtWordCount.Text = Math.Min(Math.Max(count, 1), 9).ToString(CultureInfo.CurrentCulture);
        }

        private void txtTripletCount_Leave(object sender, EventArgs e) {
            if (!int.TryParse(txtTripletCount.Text, NumberStyles.Integer, CultureInfo.CurrentCulture, out var count)) {
                count = Settings.PasswordGeneratorTripletCount;
            }
            txtTripletCount.Text = Math.Min(Math.Max(count, 1), 9).ToString(CultureInfo.CurrentCulture);
        }

        private void txtLength_Leave(object sender, EventArgs e) {
            if (!int.TryParse(txtLength.Text, NumberStyles.Integer, CultureInfo.CurrentCulture, out var length)) {
                length = Settings.PasswordGeneratorLength;
            }
            txtLength.Text = Math.Min(Math.Max(length, 4), 99).ToString(CultureInfo.CurrentCulture);
        }

        private void ChangeLength(TextBox textBox, int delta) {
            if (int.TryParse(textBox.Text, NumberStyles.Integer, CultureInfo.CurrentCulture, out var length)) {
                var newLength = Math.Min(Math.Max(length + delta, 1), (int)Math.Pow(10, textBox.TextLength) - 1);
                textBox.Text = newLength.ToString(CultureInfo.CurrentCulture);
                textBox.SelectAll();
            }
        }


        public string Password { get; private set; }


        private void btnSave_Click(object sender, EventArgs e) {
            Password = txtPassword.Text;
        }

        private void btnSaveAndCopy_Click(object sender, EventArgs e) {
            Password = txtPassword.Text;
            ClipboardHelper.SetClipboardText(this, txtPassword.Text, sensitiveData: true);
        }

        private void btnCopy_Click(object sender, EventArgs e) {
            ClipboardHelper.SetClipboardText(this, txtPassword.Text, sensitiveData: true);
        }


        private readonly char[] UpperCaseConsonants = new char[] { 'B', 'C', 'D', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'X', 'Z' };
        private readonly char[] UpperCaseVowels = new char[] { 'A', 'E', 'I', 'O', 'U', 'Y' };
        private readonly char[] LowerCaseConsonants = new char[] { 'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'z' };
        private readonly char[] LowerCaseVowels = new char[] { 'a', 'e', 'i', 'o', 'u', 'y' };
        private readonly char[] Digits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        private readonly char[] SpecialCharacters = new char[] { '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '=', '+', '[', '{', ']', '}', '\\', '|', ';', ':', '\'', '\"', ',', '<', '.', '>', '/', '?' };

        private readonly char[] RestrictedSimilar = new char[] { 'I', 'O', 'Q', 'l', '0', '`', '-', '_', ';', ':', '\'', '\"', ',', '.' };
        private readonly char[] RestrictedMoveable = new char[] { 'Y', 'Z', 'y', 'z', '`', '~', '#', '$', '%', '-', ',', '.' };

        private readonly double CracksPerSecond = 100_000_000_000_000; //100 trillion

        private static readonly RandomNumberGenerator Rnd = RandomNumberGenerator.Create();

        private void btnGenerate_Click(object sender, EventArgs e) {
            var password = default(string);
            var combinations = double.NaN;

            if (tabStyle.SelectedTab.Equals(tabStyle_Classic)) {
                if (int.TryParse(txtLength.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var length) && (length >= 4) && (length <= 99)) {
                    var includeUpperCase = chbIncludeUpperCase.Checked;
                    var includeLowerCase = chbIncludeLowerCase.Checked;
                    var includeNumbers = chbIncludeNumbers.Checked;
                    var includeSpecial = chbIncludeSpecialCharacters.Checked;
                    var restrictSimilar = chbRestrictSimilar.Checked;
                    var restrictMovable = chbRestrictMovable.Checked;
                    var restrictPronounceable = chbRestrictPronounceable.Checked;
                    var restrictRepeated = chbRestrictRepeated.Checked;

                    password = GenerateClassicPassword(includeUpperCase, includeLowerCase, includeNumbers, includeSpecial, restrictSimilar, restrictMovable, restrictPronounceable, restrictRepeated, length);
                    combinations = CalculateClassicCombinations(includeUpperCase, includeLowerCase, includeNumbers, includeSpecial, restrictSimilar, restrictMovable, restrictPronounceable, restrictRepeated, length);
                }
            } else if (tabStyle.SelectedTab.Equals(tabStyle_Triplet)) {
                if (int.TryParse(txtTripletCount.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var count) && (count >= 1) && (count <= 9)) {
                    var includeNumber = chbTripletIncludeNumber.Checked;
                    var includeSpecial = chbTripletIncludeSpecialCharacter.Checked;
                    var includeRandomUpperCase = chbTripletIncludeRandomUpperCase.Checked;
                    var includeRandomDrop = chbTripletIncludeRandomLetterDrop.Checked;
                    var restrictTitleCase = chbTripletRestrictTitleCase.Checked;
                    var restrictBreak = chbTripletRestrictBreak.Checked;
                    var restrictSuffixOnly = chbTripletRestrictSuffixOnly.Checked;
                    var restrictAddSpace = chbTripletRestrictAddSpace.Checked;

                    password = GenerateTripletPassword(includeNumber, includeSpecial, includeRandomUpperCase, includeRandomDrop, restrictTitleCase, restrictBreak, restrictSuffixOnly, restrictAddSpace, count);
                    combinations = CalculateTripletCombinations(includeNumber, includeSpecial, includeRandomUpperCase, includeRandomDrop, restrictTitleCase, restrictBreak, restrictSuffixOnly, restrictAddSpace, count);
                    txtTripletPasswordLength.Text = password.Length.ToString(CultureInfo.InvariantCulture);
                }
            } else { //Word
                if (int.TryParse(txtWordCount.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var count) && (count >= 1) && (count <= 9)) {
                    var includeUpperCase = chbWordIncludeUpperCase.Checked;
                    var includeNumber = chbWordIncludeNumber.Checked;
                    var includeSpecial = chbWordIncludeSpecialCharacter.Checked;
                    var includeIncomplete = chbWordIncludeIncomplete.Checked;
                    var restrictAddSpace = chbWordRestrictAddSpace.Checked;
                    var restrictBreak = chbWordRestrictBreak.Checked;
                    var restrictTitleCase = chbWordRestrictTitleCase.Checked;
                    var restrictSuffixOnly = chbWordRestrictSuffixOnly.Checked;

                    password = GenerateWordPassword(includeUpperCase, includeNumber, includeSpecial, includeIncomplete, restrictAddSpace, restrictTitleCase, restrictSuffixOnly, restrictBreak, count);
                    combinations = CalculateWordCombinations(includeUpperCase, includeNumber, includeSpecial, includeIncomplete, restrictAddSpace, restrictTitleCase, restrictSuffixOnly, restrictBreak, count);
                    txtWordPasswordLength.Text = password.Length.ToString(CultureInfo.InvariantCulture);
                }
            }

            if (!double.IsNaN(combinations)) {
                var entropyBits = Math.Floor(Math.Log(combinations, 2));
                var secondsToCrack = GetCrackDurationInSeconds(combinations);
                var crackDurationText = GetCrackDurationText(secondsToCrack);
                lblCombinations.Text = "About " + crackDurationText + " to crack";
                var tooltipText = $"{combinations:#,##0} ({GetEngineeringString(combinations)}) combinations.\n";
                tooltipText += $"{entropyBits} bits of entropy.\n";
                if (tabStyle.SelectedTab.Equals(tabStyle_Word)) { tooltipText += $"Based on dictionary with {Words.Count:#,##0} words.\n"; }
                tooltipText += $"\nGiven cracking duration was calculated assuming the potential\nattacker knows exactly which method and dictionary were used\nto generate password (i.e. the worst case scenario) and assuming\n the attacker can check {Math.Floor(GetCracksPerSecond() / 1000000000000)} trillions passwords per second.";
                tip.SetToolTip(lblCombinations, tooltipText);
                if (secondsToCrack > 365 * 24 * 60 * 60) {
                    Helpers.ScaleImage(picSecurityRating, "picSecurityHigh", 1);
                } else if (secondsToCrack > 24 * 60 * 60) {
                    Helpers.ScaleImage(picSecurityRating, "picSecurityMedium", 1);
                } else {
                    Helpers.ScaleImage(picSecurityRating, "picSecurityLow", 1);
                }
            } else {
                lblCombinations.Text = "?";
                tip.SetToolTip(lblCombinations, null);
                picSecurityRating.Image = null;
            }

            txtPassword.Text = password;

            btnCopy.Enabled = (txtPassword.TextLength > 0);
            btnSaveAndCopy.Enabled = (txtPassword.TextLength > 0);
            btnSave.Enabled = (txtPassword.TextLength > 0);
        }


        #region Word password

        private ReadOnlyCollection<string> Words = null;

        private string GenerateWordPassword(bool includeUpperCase, bool includeNumber, bool includeSpecial, bool includeIncomplete, bool spaceSeparated, bool restrictTitleCase, bool restrictSuffixOnly, bool restrictBreak, int count) {
            var sb = new StringBuilder();

            if (Words == null) {
                var sw = Stopwatch.StartNew();

                var wordDictionary = new Dictionary<string, object>();

                //read all word files
                foreach (var streamName in Assembly.GetExecutingAssembly().GetManifestResourceNames()) {
                    if (streamName.EndsWith(".words")) {
                        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(streamName))
                        using (var textStream = new StreamReader(stream)) {
                            var words = textStream.ReadToEnd().Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var word in words) {
                                if (!wordDictionary.ContainsKey(word)) { wordDictionary.Add(word, null); }
                            }
                        }
                    }
                }

                //remove common passwords
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Bimil.Resources.Common.passwords"))
                using (var textStream = new StreamReader(stream)) {
                    var words = textStream.ReadToEnd().Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var word in words) {
                        if (wordDictionary.ContainsKey(word)) { wordDictionary.Remove(word); }
                    }
                }

                var wordList = new List<string>(wordDictionary.Keys);
                Words = wordList.AsReadOnly();

                Debug.WriteLine($"Generated word list of {Words.Count} words in {sw.ElapsedMilliseconds} ms.");
            }

            var selectedWords = new List<List<char>>();
            for (var i = 0; i < count; i++) {
                var wordIndex = GetRandomNumber(Words.Count);
                selectedWords.Add(new List<char>(Words[wordIndex]));
            }

            if (includeIncomplete) {
                var wordIndex = restrictSuffixOnly ? selectedWords.Count - 1 : GetRandomNumber(selectedWords.Count); //incomplete may be restricted to last word only
                if (restrictBreak) { //break restriction only removes last character
                    selectedWords[wordIndex].RemoveAt(selectedWords[wordIndex].Count - 1);
                } else {
                    var charIndex = GetRandomNumber(selectedWords[wordIndex].Count);
                    selectedWords[wordIndex].RemoveAt(charIndex);
                }
            }

            if (includeUpperCase) {
                var wordIndex = restrictSuffixOnly ? 0 : GetRandomNumber(selectedWords.Count); //uppercase may be restricted to first word only
                int charIndex;
                if (restrictBreak || restrictSuffixOnly) { //break restriction only uppercases the first character.
                    charIndex = 0;
                } else {
                    charIndex = GetRandomNumber(selectedWords[wordIndex].Count);
                }
                selectedWords[wordIndex][charIndex] = char.ToUpperInvariant(selectedWords[wordIndex][charIndex]);
            }

            if (restrictTitleCase) {
                foreach (var selectedWord in selectedWords) {
                    selectedWord[0] = char.ToUpperInvariant(selectedWord[0]);
                }
            }

            if (includeNumber) {
                var wordIndex = restrictSuffixOnly ? selectedWords.Count - 1 : GetRandomNumber(selectedWords.Count); //number may be restricted to last word only
                int charIndex;
                if (restrictBreak) { //break restriction only adds number before or after the word
                    if (restrictSuffixOnly) {
                        charIndex = selectedWords[wordIndex].Count;
                    } else {
                        charIndex = (GetRandomNumber(2) == 0) ? 0 : selectedWords[wordIndex].Count;
                    }
                } else {
                    charIndex = GetRandomNumber(selectedWords[wordIndex].Count + 1);
                }
                var number = GetRandomNumber(100);
                selectedWords[wordIndex].InsertRange(charIndex, number.ToString(CultureInfo.InvariantCulture));
            }

            if (includeSpecial) {
                var wordIndex = restrictSuffixOnly ? selectedWords.Count - 1 : GetRandomNumber(selectedWords.Count); //special character may be restricted to last word only
                int charIndex;
                if (restrictBreak) { //break restriction only adds character before or after the word
                    if (restrictSuffixOnly) {
                        charIndex = selectedWords[wordIndex].Count;
                    } else {
                        charIndex = (GetRandomNumber(2) == 0) ? 0 : selectedWords[wordIndex].Count;
                    }
                } else {
                    charIndex = GetRandomNumber(selectedWords[wordIndex].Count + 1);
                }
                var specialIndex = GetRandomNumber(SpecialCharacters.Length);
                selectedWords[wordIndex].Insert(charIndex, SpecialCharacters[specialIndex]);
            }

            for (var i = 0; i < selectedWords.Count; i++) {
                if (spaceSeparated && (sb.Length > 0)) { sb.Append(" "); }
                sb.Append(new string(selectedWords[i].ToArray()));
            }

            return sb.ToString();
        }

        private double CalculateWordCombinations(bool includeUpperCase, bool includeNumber, bool includeSpecial, bool includeIncomplete, bool spaceSeparated, bool restrictTitleCase, bool restrictSuffixOnly, bool restrictBreak, int count) {
            //this is really rough calculation assuming everybody knows exactly how password was created and it assumes all words are 5 characters only

            var words = Words.Count;
            if (includeUpperCase && !restrictSuffixOnly) { words *= (1 + (restrictBreak ? 1 : 4) - (restrictTitleCase ? 1 : 0)); } //1 original + 5 characters (shortest length) that can be upper case; if break is restricted, only the first character will be upper-case; in case of title-case, first character is assumed fixed
            if (includeIncomplete && !restrictSuffixOnly) { words *= (1 + (restrictBreak ? 1 : 4)); } //1 original + 5 characters (shortest length) that can be upper case; if break is restricted, only the last character will be removed thus only doubling the space

            double wordCombinations;
            if (restrictSuffixOnly) {
                var wordsLast = Words.Count;
                if (includeIncomplete) { wordsLast *= 2; }
                if (includeNumber) { wordsLast *= 100; }
                if (includeSpecial) { wordsLast *= SpecialCharacters.Length; }
                wordCombinations = Math.Pow(words, count - 1) * wordsLast;
            } else {
                wordCombinations = Math.Pow(words, count);
                if (includeNumber) { wordCombinations *= (restrictBreak ? 4 : 20) * 100; } //assume attacker knows number between 0 and 100 is to be inserted; if restricted, assume it knows it will be on word-breaks
                if (includeSpecial) { wordCombinations *= (restrictBreak ? 1 : 20) * SpecialCharacters.Length; } //special character can be inserted in any word at any place; if break is restricted, only start and end are good
            }

            return wordCombinations;
        }

        #endregion


        #region Triplet password

        private ReadOnlyCollection<string> Triplets = null;

        private string GenerateTripletPassword(bool includeNumber, bool includeSpecial, bool includeRandomUpperCase, bool includeRandomDrop, bool restrictTitleCase, bool restrictBreak, bool restrictSuffixOnly, bool spaceSeparated, int count) {
            var sb = new StringBuilder();

            if (Triplets == null) {
                var sw = Stopwatch.StartNew();

                var tripletDictionary = new Dictionary<string, object>();

                //read all word files
                foreach (var streamName in Assembly.GetExecutingAssembly().GetManifestResourceNames()) {
                    if (streamName.EndsWith(".words")) {
                        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(streamName))
                        using (var textStream = new StreamReader(stream)) {
                            var words = textStream.ReadToEnd().Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var word in words) {
                                var triplet = word.Substring(0, 3);
                                if (!tripletDictionary.ContainsKey(triplet)) { tripletDictionary.Add(triplet, null); }
                            }
                        }
                    }
                }

                //remove common passwords
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Bimil.Resources.Common.passwords"))
                using (var textStream = new StreamReader(stream)) {
                    var words = textStream.ReadToEnd().Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var word in words) {
                        var triplet = word.Substring(0, 3);
                        if (tripletDictionary.ContainsKey(triplet)) { tripletDictionary.Remove(triplet); }
                    }
                }

                var tripletList = new List<string>(tripletDictionary.Keys);
                Triplets = tripletList.AsReadOnly();

                Debug.WriteLine($"Generated triplet list of {Triplets.Count} triplets in {sw.ElapsedMilliseconds} ms.");
            }

            var selectedTriplets = new List<List<char>>();
            for (var i = 0; i < count; i++) {
                var tripletIndex = GetRandomNumber(Triplets.Count);
                selectedTriplets.Add(new List<char>(Triplets[tripletIndex]));
            }

            if (includeRandomDrop) {
                var tripletIndex = restrictSuffixOnly ? selectedTriplets.Count - 1 : GetRandomNumber(selectedTriplets.Count); //drop may be restricted to last triplet only
                if (restrictBreak) { //break restriction only removes last character
                    selectedTriplets[tripletIndex].RemoveAt(selectedTriplets[tripletIndex].Count - 1);
                } else {
                    var charIndex = GetRandomNumber(selectedTriplets[tripletIndex].Count);
                    selectedTriplets[tripletIndex].RemoveAt(charIndex);
                }
            }

            if (includeRandomUpperCase) {
                var tripletIndex = restrictSuffixOnly ? 0 : GetRandomNumber(selectedTriplets.Count); //uppercase may be restricted to first triplet only
                int charIndex;
                if (restrictBreak || restrictSuffixOnly) { //break restriction only uppercases the first character.
                    charIndex = 0;
                } else {
                    charIndex = GetRandomNumber(selectedTriplets[tripletIndex].Count);
                }
                selectedTriplets[tripletIndex][charIndex] = char.ToUpperInvariant(selectedTriplets[tripletIndex][charIndex]);
            }

            if (restrictTitleCase) {
                foreach (var selectedTriplet in selectedTriplets) {
                    selectedTriplet[0] = char.ToUpperInvariant(selectedTriplet[0]);
                }
            }

            if (includeNumber) {
                var tripletIndex = restrictSuffixOnly ? selectedTriplets.Count - 1 : GetRandomNumber(selectedTriplets.Count); //number may be restricted to last triplet only
                int charIndex;
                if (restrictBreak) { //break restriction only adds number before or after the triplet
                    if (restrictSuffixOnly) {
                        charIndex = selectedTriplets[tripletIndex].Count;
                    } else {
                        charIndex = (GetRandomNumber(2) == 0) ? 0 : selectedTriplets[tripletIndex].Count;
                    }
                } else {
                    charIndex = GetRandomNumber(selectedTriplets[tripletIndex].Count + 1);
                }
                var number = GetRandomNumber(100);
                selectedTriplets[tripletIndex].InsertRange(charIndex, number.ToString(CultureInfo.InvariantCulture));
            }

            if (includeSpecial) {
                var tripletIndex = restrictSuffixOnly ? selectedTriplets.Count - 1 : GetRandomNumber(selectedTriplets.Count); //special character may be restricted to last triplet only
                int charIndex;
                if (restrictBreak) { //break restriction only adds character before or after the triplet
                    if (restrictSuffixOnly) {
                        charIndex = selectedTriplets[tripletIndex].Count;
                    } else {
                        charIndex = (GetRandomNumber(2) == 0) ? 0 : selectedTriplets[tripletIndex].Count;
                    }
                } else {
                    charIndex = GetRandomNumber(selectedTriplets[tripletIndex].Count + 1);
                }

                var specialCharacters = new List<char>(SpecialCharacters);
                RemoveCharacters(specialCharacters, RestrictedSimilar); //remove those that are difficult to write or can get confused

                var specialIndex = GetRandomNumber(specialCharacters.Count);
                selectedTriplets[tripletIndex].Insert(charIndex, specialCharacters[specialIndex]);
            }

            for (var i = 0; i < selectedTriplets.Count; i++) {
                if (spaceSeparated && (sb.Length > 0)) { sb.Append(" "); }
                sb.Append(new string(selectedTriplets[i].ToArray()));
            }

            return sb.ToString();
        }

        private double CalculateTripletCombinations(bool includeNumber, bool includeSpecial, bool includeRandomUpperCase, bool includeRandomDrop, bool restrictTitleCase, bool restrictBreak, bool restrictSuffixOnly, bool spaceSeparated, int count) {
            //this is really rough calculation assuming everybody knows exactly how password was created and it assumes all words are 5 characters only

            var triplets = Triplets.Count;
            if (includeRandomUpperCase && !restrictSuffixOnly) { triplets *= (1 + (restrictBreak ? 1 : 2) - (restrictTitleCase ? 1 : 0)); } //1 original + 3 characters (standard triplet length) that can be upper case; if break is restricted, only the first character will be upper-case; in case of title-case, first character is assumed fixed
            if (includeRandomDrop && !restrictSuffixOnly) { triplets *= (1 + (restrictBreak ? 1 : 2)); } //1 original + 3 characters (standard triplet length) that can be upper case; if break is restricted, only the last character will be removed thus only doubling the space

            var specialCharacters = new List<char>(SpecialCharacters);
            RemoveCharacters(specialCharacters, RestrictedSimilar); //remove those that are difficult to write or can get confused

            double tripletCombinations;
            if (restrictSuffixOnly) {
                var tripletsLast = Triplets.Count;
                if (includeRandomDrop) { tripletsLast *= 2; }
                if (includeNumber) { tripletsLast *= 100; }
                if (includeSpecial) { tripletsLast *= specialCharacters.Count; }
                tripletCombinations = Math.Pow(triplets, count - 1) * tripletsLast;
            } else {
                tripletCombinations = Math.Pow(triplets, count);
                if (includeNumber) { tripletCombinations *= (restrictBreak ? 4 : 20) * 100; } //assume attacker knows number between 0 and 100 is to be inserted; if restricted, assume it knows it will be on triplet-breaks
                if (includeSpecial) { tripletCombinations *= (restrictBreak ? 1 : 20) * specialCharacters.Count; } //special character can be inserted in any triplet at any place; if break is restricted, only start and end are good
            }

            return tripletCombinations;
        }

        #endregion


        #region Classic password

        private double CalculateClassicCombinations(bool includeUpperCase, bool includeLowerCase, bool includeNumbers, bool includeSpecial, bool restrictSimilar, bool restrictMovable, bool restrictPronounceable, bool restrictRepeated, int length) {
            var allCharacters = new List<char>();
            var vowelCharacters = new List<char>();
            var consonantCharacters = new List<char>();

            if (includeUpperCase) {
                IncludeCharacters(allCharacters, UpperCaseVowels, UpperCaseConsonants);
                IncludeCharacters(vowelCharacters, UpperCaseVowels);
                IncludeCharacters(consonantCharacters, UpperCaseConsonants);
            }
            if (includeLowerCase) {
                IncludeCharacters(allCharacters, LowerCaseVowels, LowerCaseConsonants);
                IncludeCharacters(vowelCharacters, LowerCaseVowels);
                IncludeCharacters(consonantCharacters, LowerCaseConsonants);
            }
            if (includeNumbers) {
                IncludeCharacters(allCharacters, Digits);
                IncludeCharacters(vowelCharacters, Digits);
                IncludeCharacters(consonantCharacters, Digits);
            }
            if (includeSpecial) {
                IncludeCharacters(allCharacters, SpecialCharacters);
                IncludeCharacters(vowelCharacters, SpecialCharacters);
                IncludeCharacters(consonantCharacters, SpecialCharacters);
            }

            if (restrictSimilar) {
                RemoveCharacters(allCharacters, RestrictedSimilar);
                RemoveCharacters(vowelCharacters, RestrictedSimilar);
                RemoveCharacters(consonantCharacters, RestrictedSimilar);
            }
            if (restrictMovable) {
                RemoveCharacters(allCharacters, RestrictedMoveable);
                RemoveCharacters(vowelCharacters, RestrictedMoveable);
                RemoveCharacters(consonantCharacters, RestrictedMoveable);
            }

            double combinations;
            if (restrictPronounceable) {
                var vowels = length / 2;
                var consonants = length - vowels;
                if (restrictRepeated) {
                    combinations = Math.Pow(vowelCharacters.Count - 1, vowels) * Math.Pow(consonantCharacters.Count - 1, consonants - 1) * (vowelCharacters.Count + consonantCharacters.Count);
                } else {
                    combinations = Math.Pow(vowelCharacters.Count, vowels) * Math.Pow(consonantCharacters.Count, consonants - 1) * (vowelCharacters.Count + consonantCharacters.Count);
                }
            } else {
                if (restrictRepeated) {
                    combinations = allCharacters.Count * Math.Pow(allCharacters.Count - 1, length - 1);
                } else {
                    combinations = Math.Pow(allCharacters.Count, length);
                }
            }

            return combinations;
        }

        private string GenerateClassicPassword(bool includeUpperCase, bool includeLowerCase, bool includeNumbers, bool includeSpecial, bool restrictSimilar, bool restrictMovable, bool restrictPronounceable, bool restrictRepeated, int length) {
            while (true) {
                var sb = new StringBuilder();

                var useVowelNext = false;
                while (sb.Length < length) {
                    var sixteenth = GetRandomNumber(16);

                    var characters = new List<char>();
                    if (includeUpperCase && (sixteenth >= 0) && (sixteenth <= 5)) { //Uppercase: 6/16th ~ 37.5%
                        if (restrictPronounceable) {
                            IncludeCharacters(characters, useVowelNext ? UpperCaseVowels : UpperCaseConsonants);
                            useVowelNext = !useVowelNext;
                        } else {
                            IncludeCharacters(characters, UpperCaseVowels, UpperCaseConsonants);
                        }
                    } else if (includeLowerCase && (sixteenth >= 6) && (sixteenth <= 11)) { //Lowercase: 6/16th ~ 37.5%
                        if (restrictPronounceable) {
                            IncludeCharacters(characters, useVowelNext ? LowerCaseVowels : LowerCaseConsonants);
                            useVowelNext = !useVowelNext;
                        } else {
                            IncludeCharacters(characters, LowerCaseVowels, LowerCaseConsonants);
                        }
                    } else if (includeNumbers && (sixteenth >= 12) && (sixteenth <= 13)) { //Number: 2/16th ~ 12.5%
                        if (restrictPronounceable && !useVowelNext) { continue; } //treat numbers as vowels
                        IncludeCharacters(characters, Digits);
                        useVowelNext = false;
                    } else if (includeSpecial && (sixteenth >= 14) && (sixteenth <= 15)) { //Number: 2/16th ~ 12.5%
                        if (restrictPronounceable && !useVowelNext) { continue; } //treat specials as vowels
                        IncludeCharacters(characters, SpecialCharacters);
                        useVowelNext = false;
                    }

                    if (restrictSimilar) { RemoveCharacters(characters, RestrictedSimilar); }
                    if (restrictMovable) { RemoveCharacters(characters, RestrictedMoveable); }

                    if (characters.Count > 0) {
                        var charIndex = GetRandomNumber(characters.Count);
                        var nextChar = characters[charIndex];
                        if (restrictRepeated && (sb.Length > 1) && (sb[sb.Length - 1] == nextChar)) { continue; }
                        sb.Append(nextChar);
                    }
                }

                int countUpper = 0, countLower = 0, countNumber = 0, countSpecial = 0;
                for (var i = 0; i < sb.Length; i++) {
                    if ((Array.IndexOf(LowerCaseConsonants, sb[i]) > 0) || (Array.IndexOf(LowerCaseVowels, sb[i]) > 0)) { countLower += 1; }
                    if ((Array.IndexOf(UpperCaseConsonants, sb[i]) > 0) || (Array.IndexOf(UpperCaseVowels, sb[i]) > 0)) { countUpper += 1; }
                    if (Array.IndexOf(Digits, sb[i]) > 0) { countNumber += 1; }
                    if (Array.IndexOf(SpecialCharacters, sb[i]) > 0) { countSpecial += 1; }
                }

                //another loop if one of selected is missing
                if (includeLowerCase && (countLower == 0)) { continue; }
                if (includeUpperCase && (countUpper == 0)) { continue; }
                if (includeNumbers && (countNumber == 0)) { continue; }
                if (includeSpecial && (countSpecial == 0)) { continue; }

                return sb.ToString();
            }
        }


        private static void IncludeCharacters(List<char> characterList, params ICollection<char>[] characterCollections) {
            foreach (var characterCollection in characterCollections) {
                characterList.AddRange(characterCollection);
            }
        }

        private static void RemoveCharacters(List<char> characterList, params ICollection<char>[] characterCollections) {
            foreach (var characterCollection in characterCollections) {
                foreach (var character in characterCollection) {
                    characterList.Remove(character);
                }
            }
        }

        #endregion


        private static int GetRandomNumber(int upperLimit) {
            var rndBuffer = new byte[4];
            Rnd.GetBytes(rndBuffer);

            var maxRandomCount = uint.MaxValue - (uint.MaxValue % (uint)upperLimit);
            uint randomNumber;
            do {
                Rnd.GetBytes(rndBuffer);
                randomNumber = BitConverter.ToUInt32(rndBuffer, 0);
            } while (randomNumber >= maxRandomCount);
            return (int)(randomNumber % (uint)upperLimit);
        }

        private double GetCracksPerSecond() {
            var cracksPerSecond = CracksPerSecond;
            for (var i = 2016; i <= DateTime.Now.Year - 1; i += 2) { cracksPerSecond *= 2; } //Moore's law
            return cracksPerSecond;
        }

        private double GetCrackDurationInSeconds(double combinations) {
            var cracksPerSecond = GetCracksPerSecond();

            var secondsToCrack = Math.Floor(combinations / cracksPerSecond);
            return secondsToCrack / 2;
        }

        private string GetCrackDurationText(double secondsToCrack) {
            var minutesToCrack = Math.Floor(secondsToCrack / 60);
            var hoursToCrack = Math.Floor(minutesToCrack / 60);
            var daysToCrack = Math.Floor(hoursToCrack / 24);
            var yearsToCrack = Math.Floor(daysToCrack / 365);
            var centuriesToCrack = Math.Floor(yearsToCrack / 100);
            var millenniumsToCrack = Math.Floor(yearsToCrack / 1000);
            var millionYearsToCrack = Math.Floor(yearsToCrack / 1000000);

            if (millionYearsToCrack >= 1000) {
                return "eternity";
            } else if (millionYearsToCrack >= 1) {
                return millenniumsToCrack.ToString("#,##0", CultureInfo.CurrentCulture) + " million years";
            } else if (millenniumsToCrack >= 1) {
                return millenniumsToCrack.ToString("#,##0", CultureInfo.CurrentCulture) + " " + ((millenniumsToCrack == 1) ? "millennium" : "millenniums");
            } else if (centuriesToCrack >= 1) {
                return centuriesToCrack.ToString("#,##0", CultureInfo.CurrentCulture) + " " + ((centuriesToCrack == 1) ? "century" : "centuries");
            } else if (yearsToCrack >= 1) {
                return yearsToCrack.ToString("#,##0", CultureInfo.CurrentCulture) + " " + ((yearsToCrack == 1) ? "year" : "years");
            } else if (daysToCrack >= 1) {
                return daysToCrack.ToString("#,##0", CultureInfo.CurrentCulture) + " " + ((daysToCrack == 1) ? "day" : "days");
            } else if (hoursToCrack >= 1) {
                return hoursToCrack.ToString("#,##0", CultureInfo.CurrentCulture) + " " + ((hoursToCrack == 1) ? "hour" : "hours");
            } else if (minutesToCrack >= 1) {
                return minutesToCrack.ToString("#,##0", CultureInfo.CurrentCulture) + " " + ((minutesToCrack == 1) ? "minute" : "minutes");
            } else {
                return secondsToCrack.ToString("#,##0", CultureInfo.CurrentCulture) + " " + ((secondsToCrack == 1) ? "second" : "seconds");
            }
        }

        private static string GetEngineeringString(double number) {
            var exponent = 0;
            while (number > 1000) {
                exponent += 3;
                number /= 1000;
            }
            number = Math.Floor(number); //always round down

            return string.Format(CultureInfo.CurrentCulture, "{0:0}e{1}", number, exponent);
        }

    }
}
