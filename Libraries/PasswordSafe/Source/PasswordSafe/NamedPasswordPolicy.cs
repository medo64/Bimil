using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Medo.Security.Cryptography.PasswordSafe {
    /// <summary>
    /// Password policy definition.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public class NamedPasswordPolicy {

        /// <summary>
        /// Create a new policy.
        /// </summary>
        /// <param name="name">Policy name.</param>
        /// <param name="passwordLength">Password length.</param>
        public NamedPasswordPolicy(string name, int passwordLength) {
            this.Name = name;
            this.TotalPasswordLength = passwordLength;
        }

        internal NamedPasswordPolicy(NamedPasswordPolicyCollection owner, string name, PasswordPolicyStyle style, int totalPasswordLength, int minimumLowercaseCount, int minimumUppercaseCount, int minimumDigitCount, int minimumSymbolCount, char[] specialSymbolSet) {
            this.Name = name;
            this.Style = style;
            this.TotalPasswordLength = totalPasswordLength;
            this.MinimumLowercaseCount = minimumLowercaseCount;
            this.MinimumUppercaseCount = minimumUppercaseCount;
            this.MinimumDigitCount = minimumDigitCount;
            this.MinimumSymbolCount = minimumSymbolCount;
            this.SetSpecialSymbolSet(specialSymbolSet);
            this.Owner = owner;
        }

        private readonly NamedPasswordPolicyCollection Owner;


        /// <summary>
        /// Used to mark document as changed.
        /// </summary>
        protected void MarkAsChanged() {
            if (this.Owner != null) { this.Owner.MarkAsChanged(); }
        }


        private string _name;
        /// <summary>
        /// Gets/sets name of the policy.
        /// </summary>
        /// <exception cref="ArgumentNullException">Name cannot be null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Name cannot be empty. -or- Name cannot be longer than 255 characters.</exception>
        public string Name {
            get { return this._name; }
            set {
                if (value == null) { throw new ArgumentNullException(nameof(value), "Name cannot be null."); }
                if (string.IsNullOrEmpty(value)) { throw new ArgumentOutOfRangeException(nameof(value), "Name cannot be empty."); }
                if (value.Length > 255) { throw new ArgumentOutOfRangeException(nameof(value), "Name cannot be longer than 255 characters."); }
                this._name = value;
                this.MarkAsChanged();
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
                this.MarkAsChanged();
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
                this.MarkAsChanged();
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
                this.MarkAsChanged();
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
                this.MarkAsChanged();
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
                this.MarkAsChanged();
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
                this.MarkAsChanged();
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
            this.MarkAsChanged();
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
            return this.Name.GetHashCode() ^ this.Style.GetHashCode();
        }

        /// <summary>
        /// Returns text representing this policy.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return this.Name;
        }

    }
}
