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
            this.RecordType = type;
        }

        internal Record(RecordType type, byte[] rawData) : base() {
            if ((type < 0) || (type >= RecordType.EndOfEntry)) { throw new ArgumentOutOfRangeException(nameof(type), "Type not supported."); }
            this.RecordType = type;
            this.RawData = rawData;
        }

        internal Record(RecordCollection owner, RecordType type)
            : this(type) {
            this.Owner = owner;
        }


        internal RecordCollection Owner { get; set; }


        /// <summary>
        /// Gets field type.
        /// </summary>
        public RecordType RecordType { get; set; }


        /// <summary>
        /// Used to mark document as changed.
        /// </summary>
        protected override void MarkAsChanged() {
            if (this.Owner != null) { this.Owner.MarkAsChanged(this.RecordType); }
        }

        /// <summary>
        /// Used to mark document as accessed.
        /// </summary>
        protected override void MarkAsAccessed() {
            if (this.Owner != null) { this.Owner.MarkAsAccessed(this.RecordType); }
        }

        /// <summary>
        /// Gets if object is read-only.
        /// </summary>
        protected override bool IsReadOnly {
            get { return this.Owner?.IsReadOnly ?? false; }
        }


        /// <summary>
        /// Gets underlying data type for field.
        /// </summary>
        protected override PasswordSafeFieldDataType DataType {
            get {
                switch (this.RecordType) {
                    case RecordType.Uuid: return PasswordSafeFieldDataType.Uuid;
                    case RecordType.Group:
                    case RecordType.Title:
                    case RecordType.UserName:
                    case RecordType.Notes:
                    case RecordType.Password:
                    case RecordType.Url:
                    case RecordType.Autotype:
                    case RecordType.PasswordHistory:
                    case RecordType.PasswordPolicy:
                    case RecordType.RunCommand:
                    case RecordType.EmailAddress:
                    case RecordType.OwnSymbolsForPassword:
                    case RecordType.PasswordPolicyName: return PasswordSafeFieldDataType.Text;
                    case RecordType.CreationTime:
                    case RecordType.PasswordModificationTime:
                    case RecordType.LastAccessTime:
                    case RecordType.PasswordExpiryTime:
                    case RecordType.LastModificationTime: return PasswordSafeFieldDataType.Time;
                    default: return PasswordSafeFieldDataType.Unknown;
                }
            }
        }

    }
}
