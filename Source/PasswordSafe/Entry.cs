using System;
using System.Collections.Generic;

namespace Medo.Security.Cryptography.PasswordSafe {
    /// <summary>
    /// Entry.
    /// </summary>
    public class Entry {

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public Entry()
            : this(new Record[] {
                new Record(RecordType.Uuid, Guid.NewGuid().ToByteArray()),
                new Record(RecordType.Title, new byte[0]),
                new Record(RecordType.Password, new byte[0])
            }) {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="title">Title.</param>
        public Entry(string title) : this() {
            Title = title;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="group">Group.</param>
        /// <param name="title">Title.</param>
        public Entry(GroupPath group, string title) : this() {
            Group = group;
            Title = title;
        }


        internal Entry(ICollection<Record> records) {
            Records = new RecordCollection(this, records);
        }


        internal EntryCollection Owner { get; set; }

        /// <summary>
        /// Used to mark document as changed.
        /// </summary>
        protected void MarkAsChanged() {
            if (Owner != null) { Owner.MarkAsChanged(); }
        }


        /// <summary>
        /// Gets/sets UUID.
        /// </summary>
        public Guid Uuid {
            get { return Records.Contains(RecordType.Uuid) ? Records[RecordType.Uuid].Uuid : Guid.Empty; }
            set { Records[RecordType.Uuid].Uuid = value; }
        }

        /// <summary>
        /// Gets/sets group.
        /// </summary>
        public GroupPath Group {
            get { return Records.Contains(RecordType.Group) ? Records[RecordType.Group].Text : ""; }
            set { Records[RecordType.Group].Text = value; }
        }


        /// <summary>
        /// Gets/sets title.
        /// </summary>
        public string Title {
            get { return Records.Contains(RecordType.Title) ? Records[RecordType.Title].Text : ""; }
            set { Records[RecordType.Title].Text = value; }
        }

        /// <summary>
        /// Gets/sets user name.
        /// </summary>
        public string UserName {
            get { return Records.Contains(RecordType.UserName) ? Records[RecordType.UserName].Text : ""; }
            set { Records[RecordType.UserName].Text = value; }
        }

        /// <summary>
        /// Gets/sets notes.
        /// </summary>
        public string Notes {
            get { return Records.Contains(RecordType.Notes) ? Records[RecordType.Notes].Text : ""; }
            set { Records[RecordType.Notes].Text = value; }
        }

        /// <summary>
        /// Gets/sets password.
        /// </summary>
        public string Password {
            get { return Records.Contains(RecordType.Password) ? Records[RecordType.Password].Text : ""; }
            set { Records[RecordType.Password].Text = value; }
        }


        /// <summary>
        /// Gets/sets creation time.
        /// </summary>
        public DateTime CreationTime {
            get { return Records.Contains(RecordType.CreationTime) ? Records[RecordType.CreationTime].Time : DateTime.MinValue; }
            set { Records[RecordType.CreationTime].Time = value; }
        }

        /// <summary>
        /// Gets/sets password modification time.
        /// </summary>
        public DateTime PasswordModificationTime {
            get { return Records.Contains(RecordType.PasswordModificationTime) ? Records[RecordType.PasswordModificationTime].Time : DateTime.MinValue; }
            set { Records[RecordType.PasswordModificationTime].Time = value; }
        }

        /// <summary>
        /// Gets/sets last access time.
        /// </summary>
        public DateTime LastAccessTime {
            get { return Records.Contains(RecordType.LastAccessTime) ? Records[RecordType.LastAccessTime].Time : DateTime.MinValue; }
            set { Records[RecordType.LastAccessTime].Time = value; }
        }

        /// <summary>
        /// Gets/sets password expiry time.
        /// </summary>
        public DateTime PasswordExpiryTime {
            get { return Records.Contains(RecordType.PasswordExpiryTime) ? Records[RecordType.PasswordExpiryTime].Time : DateTime.MinValue; }
            set { Records[RecordType.PasswordExpiryTime].Time = value; }
        }

        /// <summary>
        /// Gets/sets last modification time.
        /// </summary>
        public DateTime LastModificationTime {
            get { return Records.Contains(RecordType.LastModificationTime) ? Records[RecordType.LastModificationTime].Time : DateTime.MinValue; }
            set { Records[RecordType.LastModificationTime].Time = value; }
        }


        /// <summary>
        /// Gets/sets URL.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Password Safe file format doesn't require this URL to follow URL format.")]
        public string Url {
            get { return Records.Contains(RecordType.Url) ? Records[RecordType.Url].Text : ""; }
            set { Records[RecordType.Url].Text = value; }
        }


        /// <summary>
        /// Gets/sets e-mail address.
        /// </summary>
        public string Email {
            get { return Records.Contains(RecordType.EmailAddress) ? Records[RecordType.EmailAddress].Text : ""; }
            set { Records[RecordType.EmailAddress].Text = value; }
        }


        /// <summary>
        /// Gets/sets two factor key.
        /// Should be encoded as base 32.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Property returns copy of the array (in Field.GetBytes() and Field.SetBytes()).")]
        public byte[] TwoFactorKey {
            get { return Records.Contains(RecordType.TwoFactorKey) ? Records[RecordType.TwoFactorKey].GetBytes() : new byte[0]; }
            set { Records[RecordType.TwoFactorKey].SetBytes(value); }
        }


        /// <summary>
        /// Gets/sets credit card number.
        /// Number should consist of digits and spaces.
        /// </summary>
        public string CreditCardNumber {
            get { return Records.Contains(RecordType.CreditCardNumber) ? Records[RecordType.CreditCardNumber].Text : ""; }
            set { Records[RecordType.CreditCardNumber].Text = value; }
        }

        /// <summary>
        /// Gets/sets credit card expiration.
        /// Format should be MM/YY, where MM is 01-12, and YY 00-99.
        /// </summary>
        public string CreditCardExpiration {
            get { return Records.Contains(RecordType.CreditCardExpiration) ? Records[RecordType.CreditCardExpiration].Text : ""; }
            set { Records[RecordType.CreditCardExpiration].Text = value; }
        }

        /// <summary>
        /// Gets/sets credit card verification value.
        /// CVV (CVV2) is three or four digits.
        /// </summary>
        public string CreditCardVerificationValue {
            get { return Records.Contains(RecordType.CreditCardVerificationValue) ? Records[RecordType.CreditCardVerificationValue].Text : ""; }
            set { Records[RecordType.CreditCardVerificationValue].Text = value; }
        }

        /// <summary>
        /// Gets/sets credit card PIN.
        /// PIN is four to twelve digits long (ISO-9564).
        /// </summary>
        public string CreditCardPin {
            get { return Records.Contains(RecordType.CreditCardPin) ? Records[RecordType.CreditCardPin].Text : ""; }
            set { Records[RecordType.CreditCardPin].Text = value; }
        }

        /// <summary>
        /// Gets/sets UTF-8 encoded text used for QR code generation.
        /// </summary>
        public string QRCode {
            get { return Records.Contains(RecordType.QRCode) ? Records[RecordType.QRCode].Text : ""; }
            set { Records[RecordType.QRCode].Text = value; }
        }


        /// <summary>
        /// Gets/sets auto-type text.
        /// </summary>
        public string Autotype {
            get { return Records.Contains(RecordType.Autotype) ? Records[RecordType.Autotype].Text : ""; }
            set { Records[RecordType.Autotype].Text = value; }
        }

        /// <summary>
        /// Return auto-type tokens with textual fields filled in. Command fields, e.g. Wait, are not filled.
        /// Following commands are possible:
        /// * TwoFactorCode: 6-digit code for two-factor authentication.
        /// * Delay: Delay between characters in milliseconds.
        /// * Wait: Pause in milliseconds.
        /// * Legacy: Switches processing to legacy mode.
        /// </summary>
        public IEnumerable<AutotypeToken> AutotypeTokens {
            get { return AutotypeToken.GetAutotypeTokens(Autotype, this); }
        }


        /// <summary>
        /// Gets password history.
        /// </summary>
        public PasswordHistoryCollection PasswordHistory {
            get { return new PasswordHistoryCollection(Records); }
        }


        /// <summary>
        /// Gets password policy.
        /// </summary>
        public PasswordPolicy PasswordPolicy {
            get { return new PasswordPolicy(Records); }
        }

        /// <summary>
        /// Gets password policy name.
        /// </summary>
        public string PasswordPolicyName {
            get { return Records.Contains(RecordType.PasswordPolicyName) ? Records[RecordType.PasswordPolicyName].Text : ""; }
            set { Records[RecordType.PasswordPolicyName].Text = value; }
        }


        /// <summary>
        /// Gets list of records.
        /// </summary>
        public RecordCollection Records { get; }


        /// <summary>
        /// Returns a string representation of an object.
        /// </summary>
        public override string ToString() {
            return Records.Contains(RecordType.Title) ? Records[RecordType.Title].Text : "";
        }


        #region ICollection extra

        /// <summary>
        /// Gets field based on a type.
        /// If multiple elements exist with the same field type, the first one is returned.
        /// If type does not exist, it is created.
        /// 
        /// If value is set to null, field is removed.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <exception cref="ArgumentOutOfRangeException">Only null value is supported.</exception>
        public Record this[RecordType type] {
            get { return Records[type]; }
            set { Records[type] = value; }
        }

        #endregion


        #region Clone

        /// <summary>
        /// Returns the exact copy of the entry.
        /// </summary>
        public Entry Clone() {
            var records = new List<Record>();
            foreach (var record in Records) {
                records.Add(record.Clone());
            }
            return new Entry(records);
        }

        #endregion

    }
}
