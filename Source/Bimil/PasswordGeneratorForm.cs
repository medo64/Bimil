using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Bimil {
    public partial class PasswordGeneratorForm : Form {
        public PasswordGeneratorForm() {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;
        }

        protected override bool ProcessDialogKey(Keys keyData) {
            if (keyData == Keys.Escape) {
                this.Close();
                return true;
            } else {
                return base.ProcessDialogKey(keyData);
            }
        }


        private void Form_Load(object sender, EventArgs e) {
            btnGenerate_Click(null, null);
        }


        private void chbIncludeUpperCase_CheckedChanged(object sender, EventArgs e) {
            if (chbIncludeUpperCase.Checked == false) { chbIncludeLowerCase.Checked = true; }
            btnGenerate_Click(null, null);
        }

        private void chbIncludeLowerCase_CheckedChanged(object sender, EventArgs e) {
            if (chbIncludeLowerCase.Checked == false) { chbIncludeUpperCase.Checked = true; }
            btnGenerate_Click(null, null);
        }


        private void txtLength_KeyDown(object sender, KeyEventArgs e) {
            TextBox textBox = (TextBox)sender;

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
                    ChangeTimeout(textBox, -10);
                    e.SuppressKeyPress = true;
                    break;

                case Keys.PageDown:
                    ChangeTimeout(textBox, +10);
                    e.SuppressKeyPress = true;
                    break;

                default:
                    e.SuppressKeyPress = true;
                    break;
            }
        }

        private void txtLength_Leave(object sender, EventArgs e) {
            int length;
            if (!int.TryParse(txtLength.Text, NumberStyles.Integer, CultureInfo.CurrentCulture, out length)) {
                length = 12;
            }
            txtLength.Text = Math.Min(Math.Max(length, 1), 99).ToString(CultureInfo.CurrentCulture);
        }

        private void ChangeTimeout(TextBox textBox, int delta) {
            int seconds;
            if (int.TryParse(textBox.Text, NumberStyles.Integer, CultureInfo.CurrentCulture, out seconds)) {
                var newSeconds = Math.Min(Math.Max(seconds + delta, 1), 99);
                textBox.Text = newSeconds.ToString(CultureInfo.CurrentCulture);
                textBox.SelectAll();
            }
        }


        private void btnCopy_Click(object sender, EventArgs e) {
            Clipboard.Clear();
            Clipboard.SetText(txtPassword.Text, TextDataFormat.Text);
        }


        private readonly char[] UpperCaseConsonants = new char[] { 'B', 'C', 'D', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'X', 'Z' };
        private readonly char[] UpperCaseVowels = new char[] { 'A', 'E', 'I', 'O', 'U', 'Y' };
        private readonly char[] LowerCaseConsonants = new char[] { 'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'z' };
        private readonly char[] LowerCaseVowels = new char[] { 'a', 'e', 'i', 'o', 'u', 'y' };
        private readonly char[] Digits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        private readonly char[] SpecialCharacters = new char[] { '`', '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '=', '+', '[', '{', ']', '}', '\\', '|', ';', ':', '\'', '\"', ',', '<', '.', '>', '/', '?' };

        private readonly char[] RestrictedSimilar = new char[] { 'I', 'O', 'Q', 'l', '0', '`', '-', '_', ';', ':', '\'', '\"', ',', '.' };
        private readonly char[] RestrictedMoveable = new char[] { 'Y', 'Z', 'y', 'z', '`', '~', '#', '$', '%', '-', ',', '.' };

        private readonly double CracksPerSecond = 100000000000000;

        private readonly RandomNumberGenerator rnd = RandomNumberGenerator.Create();

        private void btnGenerate_Click(object sender, EventArgs e) {
            txtPassword.Text = "";
            lblCombinations.Text = "?";

            int length;
            if (int.TryParse(txtLength.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out length) && (length >= 1)) {
                var includeUpperCase = chbIncludeUpperCase.Checked;
                var includeLowerCase = chbIncludeLowerCase.Checked;
                var includeNumbers = chbIncludeNumbers.Checked;
                var includeSpecial = chbIncludeSpecialCharacters.Checked;
                var restrictSimilar = chbRestrictSimilar.Checked;
                var restrictMovable = chbRestrictMovable.Checked;
                var restrictPronounceable = chbRestrictPronounceable.Checked;
                var restrictRepeated = chbRestrictRepeated.Checked;

                var cracksPerSecond = CracksPerSecond;
                for (var i = 2016; i <= DateTime.Now.Year - 1; i += 2) { cracksPerSecond *= 2; } //Moore's law

                var combinations = CalculateCombinations(includeUpperCase, includeLowerCase, includeNumbers, includeSpecial, restrictSimilar, restrictMovable, restrictPronounceable, restrictRepeated, length);
                var secondsToCrack = Math.Floor(combinations / cracksPerSecond);
                var minutesToCrack = Math.Floor(secondsToCrack / 60);
                var hoursToCrack = Math.Floor(minutesToCrack / 60);
                var daysToCrack = Math.Floor(hoursToCrack / 24);
                var yearsToCrack = Math.Floor(daysToCrack / 365);
                var centuriesToCrack = Math.Floor(yearsToCrack / 100);
                var millenniumsToCrack = Math.Floor(yearsToCrack / 1000);
                var millionYearsToCrack = Math.Floor(yearsToCrack / 1000000);

                if (millionYearsToCrack >= 1000) {
                    lblCombinations.Text = "Eternity to crack";
                } else if (millionYearsToCrack >= 1) {
                    lblCombinations.Text = "About " + millenniumsToCrack.ToString("#,##0", CultureInfo.CurrentCulture) + " million years to crack";
                } else if (millenniumsToCrack >= 1) {
                    lblCombinations.Text = "About " + millenniumsToCrack.ToString("#,##0", CultureInfo.CurrentCulture) + " " + (((millenniumsToCrack % 10) == 1) ? "millennium" : "millenniums") + " to crack";
                } else if (centuriesToCrack >= 1) {
                    lblCombinations.Text = "About " + centuriesToCrack.ToString("#,##0", CultureInfo.CurrentCulture) + " " + (((centuriesToCrack % 10) == 1) ? "century" : "centuries") + " to crack";
                } else if (yearsToCrack >= 1) {
                    lblCombinations.Text = "About " + yearsToCrack.ToString("#,##0", CultureInfo.CurrentCulture) + " " + (((yearsToCrack % 10) == 1) ? "year" : "years") + " to crack";
                } else if (daysToCrack >= 1) {
                    lblCombinations.Text = "About " + daysToCrack.ToString("#,##0", CultureInfo.CurrentCulture) + " " + (((daysToCrack % 10) == 1) ? "day" : "days") + " to crack";
                } else if (hoursToCrack >= 1) {
                    lblCombinations.Text = "About " + hoursToCrack.ToString("#,##0", CultureInfo.CurrentCulture) + " " + (((hoursToCrack % 10) == 1) ? "hour" : "hours") + " to crack";
                } else if (minutesToCrack >= 1) {
                    lblCombinations.Text = "About " + minutesToCrack.ToString("#,##0", CultureInfo.CurrentCulture) + " " + (((minutesToCrack % 10) == 1) ? "minute" : "minutes") + " to crack";
                } else {
                    lblCombinations.Text = "About " + secondsToCrack.ToString("#,##0", CultureInfo.CurrentCulture) + " " + (((secondsToCrack % 10) == 1) ? "second" : "seconds") + " to crack";
                }
                tip.SetToolTip(lblCombinations, combinations.ToString("#,##0", CultureInfo.CurrentCulture) + " combinations");

                txtPassword.Text = GeneratePassword(includeUpperCase, includeLowerCase, includeNumbers, includeSpecial, restrictSimilar, restrictMovable, restrictPronounceable, restrictRepeated, length);
            }

            btnCopy.Enabled = (txtPassword.TextLength > 0);
        }


        private double CalculateCombinations(bool includeUpperCase, bool includeLowerCase, bool includeNumbers, bool includeSpecial, bool restrictSimilar, bool restrictMovable, bool restrictPronounceable, bool restrictRepeated, int length) {
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

        private string GeneratePassword(bool includeUpperCase, bool includeLowerCase, bool includeNumbers, bool includeSpecial, bool restrictSimilar, bool restrictMovable, bool restrictPronounceable, bool restrictRepeated, int length) {
            var sb = new StringBuilder();

            var useVowelNext = false;
            var rndBuffer = new byte[4];
            while (sb.Length < length) {
                rnd.GetBytes(rndBuffer);
                var sixteenth = rndBuffer[0] % 16;

                List<char> characters = new List<char>();
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
                    uint maxRandomCount = uint.MaxValue - (uint.MaxValue % (uint)characters.Count);
                    uint randomNumber;
                    do {
                        rnd.GetBytes(rndBuffer);
                        randomNumber = BitConverter.ToUInt32(rndBuffer, 0);
                    } while (randomNumber >= maxRandomCount);
                    var charIndex = (int)(randomNumber % (uint)characters.Count);

                    var nextChar = characters[charIndex];
                    if (restrictRepeated && (sb.Length > 1) && (sb[sb.Length - 1] == nextChar)) { continue; }
                    sb.Append(nextChar);
                }
            }

            return sb.ToString();
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
    }
}
