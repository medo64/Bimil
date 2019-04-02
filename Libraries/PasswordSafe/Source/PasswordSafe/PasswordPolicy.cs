using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Medo.Security.Cryptography.PasswordSafe {
    /// <summary>
    /// Password policy definition.
    /// </summary>
    public class PasswordPolicy {

        /// <summary>
        /// Create a new policy.
        /// </summary>
        /// <param name="passwordLength">Password length.</param>
        public PasswordPolicy(int passwordLength) {
            TotalPasswordLength = passwordLength;
        }

        internal PasswordPolicy(RecordCollection records) {
            if (records.Contains(RecordType.PasswordPolicy)) {
                var text = records[RecordType.PasswordPolicy].Text;
                FillPolicy(new StringBuilder(text));
            }

            if (records.Contains(RecordType.OwnSymbolsForPassword)) {
                var text = records[RecordType.OwnSymbolsForPassword].Text;
                SetSpecialSymbolSet(text.ToCharArray());
            }

            Records = records;
        }

        private readonly RecordCollection Records;


        /// <summary>
        /// Used to mark document as changed.
        /// </summary>
        protected void MarkPolicyAsChanged() {
            if (Records != null) {
                var record = Records[RecordType.PasswordPolicy];
                var sb = new StringBuilder();
                sb.Append(((ushort)Style).ToString("X4", CultureInfo.InvariantCulture));
                sb.Append(TotalPasswordLength.ToString("X3", CultureInfo.InvariantCulture));
                sb.Append(MinimumLowercaseCount.ToString("X3", CultureInfo.InvariantCulture));
                sb.Append(MinimumUppercaseCount.ToString("X3", CultureInfo.InvariantCulture));
                sb.Append(MinimumDigitCount.ToString("X3", CultureInfo.InvariantCulture));
                sb.Append(MinimumSymbolCount.ToString("X3", CultureInfo.InvariantCulture));
                record.Text = sb.ToString();
            }
        }

        /// <summary>
        /// Used to mark document as changed.
        /// </summary>
        protected void MarkSymbolsAsChanged() {
            if (Records != null) {
                var record = Records[RecordType.OwnSymbolsForPassword];
                record.Text = new string(GetSpecialSymbolSet());
            }
        }


        private PasswordPolicyStyle _style;
        /// <summary>
        /// Gets/sets style of password policy.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Value cannot be wider than 16-bit.</exception>
        public PasswordPolicyStyle Style {
            get { return _style; }
            set {
                if (((int)value & 0xFFFF0000) != 0) { throw new ArgumentOutOfRangeException(nameof(value), "Value cannot be wider than 16-bit."); }
                if ((value & PasswordPolicyStyle.UseHexDigits) == 0) {
                    _style = value;
                } else { //force hex values only
                    _style = PasswordPolicyStyle.UseHexDigits;
                }
                MarkPolicyAsChanged();
            }
        }

        private int _totalPasswordLength;
        /// <summary>
        /// Gets/sets total password length.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Count must be between 1 and 4095.</exception>
        public int TotalPasswordLength {
            get { return _totalPasswordLength; }
            set {
                if ((value < 1) || (value > 4095)) { throw new ArgumentOutOfRangeException(nameof(value), "Length must be between 1 and 4095."); }
                _totalPasswordLength = value;
                MarkPolicyAsChanged();
            }
        }

        private int _minimumLowercaseCount;
        /// <summary>
        /// Gets/sets minimum lowercase count.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Count must be between 0 and 4095.</exception>
        public int MinimumLowercaseCount {
            get { return _minimumLowercaseCount; }
            set {
                if ((value < 0) || (value > 4095)) { throw new ArgumentOutOfRangeException(nameof(value), "Count must be between 0 and 4095."); }
                _minimumLowercaseCount = value;
                MarkPolicyAsChanged();
            }
        }

        private int _minimumUppercaseCount;
        /// <summary>
        /// Gets/sets minimum uppercase count.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Count must be between 0 and 4095.</exception>
        public int MinimumUppercaseCount {
            get { return _minimumUppercaseCount; }
            set {
                if ((value < 0) || (value > 4095)) { throw new ArgumentOutOfRangeException(nameof(value), "Count must be between 0 and 4095."); }
                _minimumUppercaseCount = value;
                MarkPolicyAsChanged();
            }
        }

        private int _minimumDigitCount;
        /// <summary>
        /// Gets/sets minimum digit count.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Count must be between 0 and 4095.</exception>
        public int MinimumDigitCount {
            get { return _minimumDigitCount; }
            set {
                if ((value < 0) || (value > 4095)) { throw new ArgumentOutOfRangeException(nameof(value), "Count must be between 0 and 4095."); }
                _minimumDigitCount = value;
                MarkPolicyAsChanged();
            }
        }

        private int _minimumSymbolCount;
        /// <summary>
        /// Gets/sets minimum symbol count.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Count must be between 0 and 4095.</exception>
        public int MinimumSymbolCount {
            get { return _minimumSymbolCount; }
            set {
                if ((value < 0) || (value > 4095)) { throw new ArgumentOutOfRangeException(nameof(value), "Count must be between 0 and 4095."); }
                _minimumSymbolCount = value;
                MarkPolicyAsChanged();
            }
        }


        private char[] _specialSymbolSet = new char[] { };
        /// <summary>
        /// Returns special symbols that are allowed in the password.
        /// </summary>
        public char[] GetSpecialSymbolSet() {
            return _specialSymbolSet;
        }

        /// <summary>
        /// Sets which special characters are allowed.
        /// </summary>
        /// <param name="specialSymbols"></param>
        /// <exception cref="ArgumentNullException">Value cannot be null.</exception>
        public void SetSpecialSymbolSet(params char[] specialSymbols) {
            if (specialSymbols == null) { throw new ArgumentNullException(nameof(specialSymbols), "Value cannot be null."); }

            //filter the same
            var symbols = new List<char>(specialSymbols);
            if (symbols.Count > 1) {
                symbols.Sort();
                var prevCh = symbols[symbols.Count - 1];
                for (var i = symbols.Count - 2; i >= 0; i--) {
                    var currCh = symbols[i];
                    if (currCh == prevCh) {
                        symbols.RemoveAt(i);
                    } else {
                        prevCh = currCh;
                    }
                }
            }

            _specialSymbolSet = symbols.ToArray();
            MarkSymbolsAsChanged();
        }


        /// <summary>
        /// Returns true if objects are equal.
        /// </summary>
        /// <param name="obj">Other object.</param>
        public override bool Equals(object obj) {
            if (obj is NamedPasswordPolicy other) {
                return string.Equals(ToString(), other.ToString(), StringComparison.Ordinal);
            }
            return false;
        }

        /// <summary>
        /// Returns hash code.
        /// </summary>
        public override int GetHashCode() {
            return Style.GetHashCode();
        }


        private void FillPolicy(StringBuilder text) {
            if ((text.Length < 4) || !int.TryParse(text.ToString(0, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var styleFlags)) {
                return;
            }
            text.Remove(0, 4);
            Style = (PasswordPolicyStyle)styleFlags;

            if ((text.Length < 3) || !int.TryParse(text.ToString(0, 3), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var totalPasswordLength)) {
                return;
            }
            text.Remove(0, 3);
            TotalPasswordLength = totalPasswordLength;

            if ((text.Length < 3) || !int.TryParse(text.ToString(0, 3), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var minimumLowercase)) {
                return;
            }
            text.Remove(0, 3);
            MinimumLowercaseCount = minimumLowercase;

            if ((text.Length < 3) || !int.TryParse(text.ToString(0, 3), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var minimumUppercase)) {
                return;
            }
            text.Remove(0, 3);
            MinimumUppercaseCount = minimumUppercase;

            if ((text.Length < 3) || !int.TryParse(text.ToString(0, 3), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var minimumDigits)) {
                return;
            }
            text.Remove(0, 3);
            MinimumDigitCount = minimumDigits;

            if ((text.Length < 3) || !int.TryParse(text.ToString(0, 3), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var minimumSymbols)) {
                return;
            }
            text.Remove(0, 3);
            MinimumSymbolCount = minimumSymbols;
        }

    }
}
