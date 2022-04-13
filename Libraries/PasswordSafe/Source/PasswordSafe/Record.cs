using System;
using System.Diagnostics;

namespace Medo.Security.Cryptography.PasswordSafe {
    /// <summary>
    /// Record field.
    /// </summary>
    [DebuggerDisplay("{RecordType}: {ToString(),nq}")]
    public class Record : Field {

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="type">Record type.</param>
        public Record(RecordType type)
            : base() {
            RecordType = type;
            if (RecordType == RecordType.Autotype) { Text = @"\u\t\p\n"; } //to have default value
        }

        internal Record(RecordType type, byte[] rawData) : base() {
            if (type is < 0 or >= RecordType.EndOfEntry) { throw new ArgumentOutOfRangeException(nameof(type), "Type not supported."); }
            RecordType = type;
            RawData = rawData;
        }

        internal Record(RecordCollection owner, RecordType type)
            : this(type) {
            Owner = owner;
        }


        internal RecordCollection? Owner { get; set; }


        /// <summary>
        /// Gets field type.
        /// </summary>
        public RecordType RecordType { get; set; }

        /// <summary>
        /// Gets/sets text data.
        /// Null will be returned if conversion cannot be performed.
        /// For unknown field types, conversion will always be attempted.
        /// </summary>
        public override string? Text {
            get { return base.Text; }
            set {
                if (RecordType == RecordType.Password) { //only for password change update history
                    if ((Owner != null) && Owner.Contains(RecordType.PasswordHistory) && Owner.Contains(RecordType.Password)) {
                        var history = new PasswordHistoryCollection(Owner);
                        if (history.Enabled && (Text != null)) {
                            var time = Owner.Contains(RecordType.PasswordModificationTime) ? Owner[RecordType.PasswordModificationTime].Time : DateTime.UtcNow;
                            history.AddPasswordToHistory(time, Text); //save current password
                        }
                    }
                }
                base.Text = value;
            }
        }


        /// <summary>
        /// Used to mark document as changed.
        /// </summary>
        protected override void MarkAsChanged() {
            if (Owner != null) { Owner.MarkAsChanged(RecordType); }
        }

        /// <summary>
        /// Used to mark document as accessed.
        /// </summary>
        protected override void MarkAsAccessed() {
            if (Owner != null) { Owner.MarkAsAccessed(RecordType); }
        }

        /// <summary>
        /// Gets if object is read-only.
        /// </summary>
        protected override bool IsReadOnly {
            get { return Owner?.IsReadOnly ?? false; }
        }


        /// <summary>
        /// Gets underlying data type for field.
        /// </summary>
        protected override PasswordSafeFieldDataType DataType {
            get {
                return RecordType switch {
                    RecordType.Uuid => PasswordSafeFieldDataType.Uuid,
                    RecordType.Group => PasswordSafeFieldDataType.Text,
                    RecordType.Title => PasswordSafeFieldDataType.Text,
                    RecordType.UserName => PasswordSafeFieldDataType.Text,
                    RecordType.Notes => PasswordSafeFieldDataType.Text,
                    RecordType.Password => PasswordSafeFieldDataType.Text,
                    RecordType.Url => PasswordSafeFieldDataType.Text,
                    RecordType.Autotype => PasswordSafeFieldDataType.Text,
                    RecordType.PasswordHistory => PasswordSafeFieldDataType.Text,
                    RecordType.PasswordPolicy => PasswordSafeFieldDataType.Text,
                    RecordType.RunCommand => PasswordSafeFieldDataType.Text,
                    RecordType.EmailAddress => PasswordSafeFieldDataType.Text,
                    RecordType.OwnSymbolsForPassword => PasswordSafeFieldDataType.Text,
                    RecordType.PasswordPolicyName => PasswordSafeFieldDataType.Text,
                    RecordType.CreditCardNumber => PasswordSafeFieldDataType.Text,
                    RecordType.CreditCardExpiration => PasswordSafeFieldDataType.Text,
                    RecordType.CreditCardVerificationValue => PasswordSafeFieldDataType.Text,
                    RecordType.CreditCardPin => PasswordSafeFieldDataType.Text,
                    RecordType.QRCode => PasswordSafeFieldDataType.Text,
                    RecordType.CreationTime => PasswordSafeFieldDataType.Time,
                    RecordType.PasswordModificationTime => PasswordSafeFieldDataType.Time,
                    RecordType.LastAccessTime => PasswordSafeFieldDataType.Time,
                    RecordType.PasswordExpiryTime => PasswordSafeFieldDataType.Time,
                    RecordType.LastModificationTime => PasswordSafeFieldDataType.Time,
                    RecordType.TwoFactorKey => PasswordSafeFieldDataType.Binary,
                    _ => PasswordSafeFieldDataType.Unknown,
                };
            }
        }


        #region Clone

        /// <summary>
        /// Returns the exact copy of the record.
        /// </summary>
        public Record Clone() {
            return new Record(RecordType, base.RawDataDirect);
        }

        #endregion

    }
}
