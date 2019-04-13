using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Medo.Security.Cryptography.PasswordSafe {
    /// <summary>
    /// Password history.
    /// </summary>
    [DebuggerDisplay("{Count} historical passwords")]
    public sealed class PasswordHistoryCollection : IEnumerable<PasswordHistoryItem> {

        internal PasswordHistoryCollection(RecordCollection records) {
            Records = records;

            if (Records.Contains(RecordType.PasswordHistory)) {
                var text = Records[RecordType.PasswordHistory].Text;
                if (text.Length >= 5) {
                    _enabled = text[0] == '0' ? false : true;
                    if (!int.TryParse(text.Substring(1, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _maximumCount)) {
                        _maximumCount = DefaultMaximumCount;
                    }

                    if (int.TryParse(text.Substring(3, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var count)) {
                        var j = 5; //where parsing starts
                        for (var i = 0; i < count; i++) {
                            if (text.Length >= j + 12) {
                                if (int.TryParse(text.Substring(j, 8), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var time)
                                    && int.TryParse(text.Substring(j + 8, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var length)) {
                                    j += 12; //skip time and length
                                    if (text.Length >= j + length) {
                                        var item = new PasswordHistoryItem(this, UnixEpoch.AddSeconds(time), text.Substring(j, length));
                                        BaseCollection.Add(item);
                                        j += length; //skip password
                                    } else {
                                        break; //something is wrong with parsing
                                    }
                                } else {
                                    break; //something is wrong with parsing
                                }
                            } else {
                                break; //something is wrong with parsing
                            }
                        }
                    }
                }
            }
        }

        private readonly RecordCollection Records;


        /// <summary>
        /// Used to mark document as changed.
        /// </summary>
        internal void MarkAsChanged() {
            var record = Records[RecordType.PasswordHistory];

            var sb = new StringBuilder();
            sb.Append(Enabled ? "1" : "0");
            sb.Append(_maximumCount.ToString("x2", CultureInfo.InvariantCulture));
            sb.Append(BaseCollection.Count.ToString("x2", CultureInfo.InvariantCulture));
            foreach (var item in BaseCollection) {
                var seconds = (item.TimeFirstUsed - UnixEpoch).TotalSeconds;
                uint unixTime;
                if (seconds < 0) {
                    unixTime = 0;
                } else if (seconds > uint.MaxValue) {
                    unixTime = uint.MaxValue;
                } else {
                    unixTime = (uint)seconds;
                }
                sb.Append(unixTime.ToString("x8", CultureInfo.InvariantCulture));

                sb.Append(item.HistoricalPassword.Length.ToString("x4", CultureInfo.InvariantCulture));
                sb.Append(item.HistoricalPassword);
            }

            record.Text = sb.ToString();
        }

        private bool _enabled;
        /// <summary>
        /// Gets/sets whether password history is enabled.
        /// </summary>
        public bool Enabled {
            get { return _enabled; }
            set {
                if (_enabled != value) {
                    _enabled = value;
                    if (value == false) { BaseCollection.Clear(); } //remove all history
                    MarkAsChanged();
                }
            }
        }

        private int _maximumCount = DefaultMaximumCount;
        /// <summary>
        /// Gets/sets how many passwords are to be remembered.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Value must be between 1 and 255.</exception>
        public int MaximumCount {
            get {
                return _maximumCount;
            }
            set {
                if ((value < 1) || (value > 255)) { throw new ArgumentOutOfRangeException("value", "Value must be between 1 and 255."); }
                if (_maximumCount != value) {
                    _maximumCount = value;
                    MarkAsChanged();
                }
            }
        }


        /// <summary>
        /// Removes all passwords currently stored.
        /// </summary>
        public void Clear() {
            BaseCollection.Clear();
            MarkAsChanged();
        }


        internal void AddPasswordToHistory(DateTime time, string password) {
            if (Enabled && !string.IsNullOrEmpty(password)) { //change only if enabled and not empty
                BaseCollection.Add(new PasswordHistoryItem(this, time, password));
                while (BaseCollection.Count > MaximumCount) {
                    BaseCollection.RemoveAt(0);
                }
                MarkAsChanged();
            }
        }


        private readonly List<PasswordHistoryItem> BaseCollection = new List<PasswordHistoryItem>();


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
        public PasswordHistoryItem this[int index] {
            get { return BaseCollection[index]; }
        }

        #region IEnumerable

        IEnumerator<PasswordHistoryItem> IEnumerable<PasswordHistoryItem>.GetEnumerator() {
            return BaseCollection.AsReadOnly().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return BaseCollection.AsReadOnly().GetEnumerator();
        }

        #endregion


        private const int DefaultMaximumCount = 3;
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    }
}
