using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Medo.Security.Cryptography.PasswordSafe {
    /// <summary>
    /// Password policy collection.
    /// </summary>
    [DebuggerDisplay("{Count} policies")]
    public sealed class NamedPasswordPolicyCollection : IEnumerable<NamedPasswordPolicy> {

        internal NamedPasswordPolicyCollection(Document owner) {
            Owner = owner;

            if (owner.Headers.Contains(HeaderType.NamedPasswordPolicies)) {
                var text = owner.Headers[HeaderType.NamedPasswordPolicies].Text;
                foreach (var item in ParseMultiple(text)) {
                    BaseCollection.Add(item);
                }
            }
        }


        private readonly Document Owner;


        /// <summary>
        /// Used to mark document as changed.
        /// </summary>
        internal void MarkAsChanged() {
            var header = Owner.Headers[HeaderType.NamedPasswordPolicies];

            var sb = new StringBuilder();
            sb.Append(BaseCollection.Count.ToString("X2", CultureInfo.InvariantCulture));

            foreach (var item in BaseCollection) {
                sb.Append(item.Name.Length.ToString("X2", CultureInfo.InvariantCulture));
                sb.Append(item.Name);

                sb.Append(((ushort)item.Style).ToString("X4", CultureInfo.InvariantCulture));
                sb.Append(item.TotalPasswordLength.ToString("X3", CultureInfo.InvariantCulture));
                sb.Append(item.MinimumLowercaseCount.ToString("X3", CultureInfo.InvariantCulture));
                sb.Append(item.MinimumUppercaseCount.ToString("X3", CultureInfo.InvariantCulture));
                sb.Append(item.MinimumDigitCount.ToString("X3", CultureInfo.InvariantCulture));
                sb.Append(item.MinimumSymbolCount.ToString("X3", CultureInfo.InvariantCulture));

                var symbolSet = item.GetSpecialSymbolSet();
                sb.Append(symbolSet.Length.ToString("X2", CultureInfo.InvariantCulture));
                sb.Append(symbolSet);
            }

            header.Text = sb.ToString();
        }


        /// <summary>
        /// Removes all passwords currently stored.
        /// </summary>
        public void Clear() {
            BaseCollection.Clear();
            MarkAsChanged();
        }


        private readonly List<NamedPasswordPolicy> BaseCollection = new List<NamedPasswordPolicy>();


        /// <summary>
        /// Gets the number of items contained in the collection.
        /// </summary>
        public int Count {
            get { return BaseCollection.Count; }
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <exception cref="ArgumentOutOfRangeException">Index is less than 0. -or- Index is equal to or greater than collection count. -or- Duplicate name in collection.</exception>
        public NamedPasswordPolicy this[int index] {
            get { return BaseCollection[index]; }
        }

        #region IEnumerable

        IEnumerator<NamedPasswordPolicy> IEnumerable<NamedPasswordPolicy>.GetEnumerator() {
            return BaseCollection.AsReadOnly().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return BaseCollection.AsReadOnly().GetEnumerator();
        }

        #endregion


        #region Parse

        private IEnumerable<NamedPasswordPolicy> ParseMultiple(string text) {
            var sb = new StringBuilder(text);

            if ((sb.Length < 2) || !int.TryParse(sb.ToString(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var count)) {
                yield break;
            }
            sb.Remove(0, 2);

            for (var i = 0; i < count; i++) {
                var item = ParseSingle(sb);
                if (item != null) { yield return item; }
            }
        }

        private NamedPasswordPolicy ParseSingle(StringBuilder text) {
            if ((text.Length < 2) || !int.TryParse(text.ToString(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var nameLength)) {
                return null;
            }
            text.Remove(0, 2);

            if (text.Length < nameLength) {
                return null;
            }
            var name = text.ToString(0, nameLength);
            text.Remove(0, nameLength);

            if ((text.Length < 4) || !int.TryParse(text.ToString(0, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var styleFlags)) {
                return null;
            }
            text.Remove(0, 4);

            if ((text.Length < 3) || !int.TryParse(text.ToString(0, 3), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var totalPasswordLength)) {
                return null;
            }
            text.Remove(0, 3);

            if ((text.Length < 3) || !int.TryParse(text.ToString(0, 3), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var minimumLowercase)) {
                return null;
            }
            text.Remove(0, 3);

            if ((text.Length < 3) || !int.TryParse(text.ToString(0, 3), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var minimumUppercase)) {
                return null;
            }
            text.Remove(0, 3);

            if ((text.Length < 3) || !int.TryParse(text.ToString(0, 3), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var minimumDigits)) {
                return null;
            }
            text.Remove(0, 3);

            if ((text.Length < 3) || !int.TryParse(text.ToString(0, 3), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var minimumSymbols)) {
                return null;
            }
            text.Remove(0, 3);

            if ((text.Length < 2) || !int.TryParse(text.ToString(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var symbolLength)) {
                return null;
            }
            text.Remove(0, 2);

            if (text.Length < symbolLength) {
                return null;
            }
            var symbols = text.ToString(0, symbolLength);
            text.Remove(0, symbolLength);

            try {
                return new NamedPasswordPolicy(this, name, (PasswordPolicyStyle)styleFlags, totalPasswordLength,
                    minimumLowercase, minimumUppercase, minimumDigits, minimumSymbols, symbols.ToCharArray());
            } catch (ArgumentException) {
                return null;
            }
        }

        #endregion Parse

    }
}
