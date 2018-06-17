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
            this.TotalPasswordLength = passwordLength;
        }

        internal PasswordPolicy(RecordCollection records) {
            if (records.Contains(RecordType.PasswordPolicy)) {
                var text = records[RecordType.PasswordPolicy].Text;
                FillPolicy(new StringBuilder(text));
            }

            if (records.Contains(RecordType.OwnSymbolsForPassword)) {
                var text = records[RecordType.OwnSymbolsForPassword].Text;
                this.SetSpecialSymbolSet(text.ToCharArray());
            }

            this.Records = records;
        }

        private readonly RecordCollection Records;


        /// <summary>
        /// Used to mark document as changed.
        /// </summary>
        protected void MarkPolicyAsChanged() {
            if (this.Records != null) {
                var record = this.Records[RecordType.PasswordPolicy];
                var sb = new StringBuilder();
                sb.Append(((ushort)this.Style).ToString("X4", CultureInfo.InvariantCulture));
                sb.Append(this.TotalPasswordLength.ToString("X3", CultureInfo.InvariantCulture));
                sb.Append(this.MinimumLowercaseCount.ToString("X3", CultureInfo.InvariantCulture));
                sb.Append(this.MinimumUppercaseCount.ToString("X3", CultureInfo.InvariantCulture));
                sb.Append(this.MinimumDigitCount.ToString("X3", CultureInfo.InvariantCulture));
                sb.Append(this.MinimumSymbolCount.ToString("X3", CultureInfo.InvariantCulture));
                record.Text = sb.ToString();
            }
        }

        /// <summary>
        /// Used to mark document as changed.
        /// </summary>
        protected void MarkSymbolsAsChanged() {
            if (this.Records != null) {
                var record = this.Records[RecordType.OwnSymbolsForPassword];
                record.Text = new string(this.GetSpecialSymbolSet());
            }
        }


        private PasswordPolicyStyle _style;
        /// <summary>
        /// Gets/sets style of password policy.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Value cannot be wider than 16-bit.</exception>
        public PasswordPolicyStyle Style {
            get { return this._style; }
            set {
                if (((int)value & 0xFFFF0000) != 0) { throw new ArgumentOutOfRangeException(nameof(value), "Value cannot be wider than 16-bit."); }
                if ((value & PasswordPolicyStyle.UseHexDigits) == 0) {
                    this._style = value;
                } else { //force hex values only
                    this._style = PasswordPolicyStyle.UseHexDigits;
                }
                this.MarkPolicyAsChanged();
            }
        }

        private int _totalPasswordLength;
        /// <summary>
        /// Gets/sets total password length.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Count must be between 1 and 4095.</exception>
        public int TotalPasswordLength {
            get { return this._totalPasswordLength; }
            set {
                if ((value < 1) || (value > 4095)) { throw new ArgumentOutOfRangeException(nameof(value), "Length must be between 1 and 4095."); }
                this._totalPasswordLength = value;
                this.MarkPolicyAsChanged();
            }
        }

        private int _minimumLowercaseCount;
        /// <summary>
        /// Gets/sets minimum lowercase count.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Count must be between 0 and 4095.</exception>
        public int MinimumLowercaseCount {
            get { return this._minimumLowercaseCount; }
            set {
                if ((value < 0) || (value > 4095)) { throw new ArgumentOutOfRangeException(nameof(value), "Count must be between 0 and 4095."); }
                this._minimumLowercaseCount = value;
                this.MarkPolicyAsChanged();
            }
        }

        private int _minimumUppercaseCount;
        /// <summary>
        /// Gets/sets minimum uppercase count.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Count must be between 0 and 4095.</exception>
        public int MinimumUppercaseCount {
            get { return this._minimumUppercaseCount; }
            set {
                if ((value < 0) || (value > 4095)) { throw new ArgumentOutOfRangeException(nameof(value), "Count must be between 0 and 4095."); }
                this._minimumUppercaseCount = value;
                this.MarkPolicyAsChanged();
            }
        }

        private int _minimumDigitCount;
        /// <summary>
        /// Gets/sets minimum digit count.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Count must be between 0 and 4095.</exception>
        public int MinimumDigitCount {
            get { return this._minimumDigitCount; }
            set {
                if ((value < 0) || (value > 4095)) { throw new ArgumentOutOfRangeException(nameof(value), "Count must be between 0 and 4095."); }
                this._minimumDigitCount = value;
                this.MarkPolicyAsChanged();
            }
        }

        private int _minimumSymbolCount;
        /// <summary>
        /// Gets/sets minimum symbol count.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Count must be between 0 and 4095.</exception>
        public int MinimumSymbolCount {
            get { return this._minimumSymbolCount; }
            set {
                if ((value < 0) || (value > 4095)) { throw new ArgumentOutOfRangeException(nameof(value), "Count must be between 0 and 4095."); }
                this._minimumSymbolCount = value;
                this.MarkPolicyAsChanged();
            }
        }


        private char[] _specialSymbolSet = new char[] { };
        /// <summary>
        /// Returns special symbols that are allowed in the password.
        /// </summary>
        public char[] GetSpecialSymbolSet() {
            return this._specialSymbolSet;
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

            this._specialSymbolSet = symbols.ToArray();
            this.MarkSymbolsAsChanged();
        }


        /// <summary>
        /// Returns true if objects are equal.
        /// </summary>
        /// <param name="obj">Other object.</param>
        public override bool Equals(object obj) {
            if (obj is NamedPasswordPolicy other) {
                return string.Equals(this.ToString(), obj.ToString(), StringComparison.Ordinal);
            }
            return false;
        }

        /// <summary>
        /// Returns hash code.
        /// </summary>
        public override int GetHashCode() {
            return this.Style.GetHashCode();
        }


        private void FillPolicy(StringBuilder text) {
            if ((text.Length < 4) || !int.TryParse(text.ToString(0, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var styleFlags)) {
                return;
            }
            text.Remove(0, 4);
            this.Style = (PasswordPolicyStyle)styleFlags;

            if ((text.Length < 3) || !int.TryParse(text.ToString(0, 3), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var totalPasswordLength)) {
                return;
            }
            text.Remove(0, 3);
            this.TotalPasswordLength = totalPasswordLength;

            if ((text.Length < 3) || !int.TryParse(text.ToString(0, 3), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var minimumLowercase)) {
                return;
            }
            text.Remove(0, 3);
            this.MinimumLowercaseCount = minimumLowercase;

            if ((text.Length < 3) || !int.TryParse(text.ToString(0, 3), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var minimumUppercase)) {
                return;
            }
            text.Remove(0, 3);
            this.MinimumUppercaseCount = minimumUppercase;

            if ((text.Length < 3) || !int.TryParse(text.ToString(0, 3), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var minimumDigits)) {
                return;
            }
            text.Remove(0, 3);
            this.MinimumDigitCount = minimumDigits;

            if ((text.Length < 3) || !int.TryParse(text.ToString(0, 3), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var minimumSymbols)) {
                return;
            }
            text.Remove(0, 3);
            this.MinimumSymbolCount = minimumSymbols;
        }

    }
}
