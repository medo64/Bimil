using System;
using System.Diagnostics;

namespace Medo.Security.Cryptography.PasswordSafe {
    /// <summary>
    /// Header field.
    /// </summary>
    [DebuggerDisplay("{HeaderType}: {ToString(),nq}")]
    public class Header : Field {

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="type">Header type.</param>
        public Header(HeaderType type)
            : base() {
            HeaderType = type;
        }


        internal Header(HeaderCollection owner, HeaderType type)
            : this(type) {
            Owner = owner;
        }

        internal Header(HeaderType type, byte[] rawData)
            : base() {
            if ((type < 0) || (type >= HeaderType.EndOfEntry)) { throw new ArgumentOutOfRangeException(nameof(type), "Type not supported."); }
            HeaderType = type;
            RawData = rawData;
        }


        internal HeaderCollection Owner { get; set; }


        /// <summary>
        /// Used to mark document as changed.
        /// </summary>
        protected override void MarkAsChanged() {
            if (Owner != null) { Owner.MarkAsChanged(); }
        }

        /// <summary>
        /// Used to mark document as accessed.
        /// </summary>
        protected override void MarkAsAccessed() { } //nobody cares

        /// <summary>
        /// Gets if object is read-only.
        /// </summary>
        protected override bool IsReadOnly {
            get { return Owner?.IsReadOnly ?? false; }
        }


        /// <summary>
        /// Gets field type.
        /// </summary>
        public HeaderType HeaderType { get; }


        /// <summary>
        /// Gets underlying data type for field.
        /// </summary>
        protected override PasswordSafeFieldDataType DataType {
            get {
                switch (HeaderType) {
                    case HeaderType.Version:
                        return PasswordSafeFieldDataType.Version;

                    case HeaderType.Uuid:
                        return PasswordSafeFieldDataType.Uuid;

                    case HeaderType.NonDefaultPreferences:
                    case HeaderType.TreeDisplayStatus:
                    case HeaderType.WhoPerformedLastSave:
                    case HeaderType.WhatPerformedLastSave:
                    case HeaderType.LastSavedByUser:
                    case HeaderType.LastSavedOnHost:
                    case HeaderType.DatabaseName:
                    case HeaderType.DatabaseDescription:
                    case HeaderType.DatabaseFilters:
                    case HeaderType.RecentlyUsedEntries:
                    case HeaderType.NamedPasswordPolicies:
                    case HeaderType.EmptyGroups:
                    case HeaderType.Yubico:
                        return PasswordSafeFieldDataType.Text;

                    case HeaderType.TimestampOfLastSave:
                        return PasswordSafeFieldDataType.Time;

                    default: return PasswordSafeFieldDataType.Unknown;
                }
            }
        }


        internal static ushort DefaultVersion = 0x030D;

    }
}
