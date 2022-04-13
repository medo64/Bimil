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
            if (type is < 0 or >= HeaderType.EndOfEntry) { throw new ArgumentOutOfRangeException(nameof(type), "Type not supported."); }
            HeaderType = type;
            RawData = rawData;
        }


        internal HeaderCollection? Owner { get; set; }


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
                return HeaderType switch {
                    HeaderType.Version => PasswordSafeFieldDataType.Version,
                    HeaderType.Uuid => PasswordSafeFieldDataType.Uuid,
                    HeaderType.NonDefaultPreferences => PasswordSafeFieldDataType.Text,
                    HeaderType.TreeDisplayStatus => PasswordSafeFieldDataType.Text,
                    HeaderType.WhoPerformedLastSave => PasswordSafeFieldDataType.Text,
                    HeaderType.WhatPerformedLastSave => PasswordSafeFieldDataType.Text,
                    HeaderType.LastSavedByUser => PasswordSafeFieldDataType.Text,
                    HeaderType.LastSavedOnHost => PasswordSafeFieldDataType.Text,
                    HeaderType.DatabaseName => PasswordSafeFieldDataType.Text,
                    HeaderType.DatabaseDescription => PasswordSafeFieldDataType.Text,
                    HeaderType.DatabaseFilters => PasswordSafeFieldDataType.Text,
                    HeaderType.RecentlyUsedEntries => PasswordSafeFieldDataType.Text,
                    HeaderType.NamedPasswordPolicies => PasswordSafeFieldDataType.Text,
                    HeaderType.EmptyGroups => PasswordSafeFieldDataType.Text,
                    HeaderType.Yubico => PasswordSafeFieldDataType.Text,
                    HeaderType.TimestampOfLastSave => PasswordSafeFieldDataType.Time,
                    _ => PasswordSafeFieldDataType.Unknown,
                };
            }
        }


        internal static ushort DefaultVersion = 0x030D;

    }
}
