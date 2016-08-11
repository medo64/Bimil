using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

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
            this.Title = title;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="group">Group.</param>
        /// <param name="title">Title.</param>
        public Entry(GroupPath group, string title) : this() {
            this.Group = group;
            this.Title = title;
        }


        internal Entry(ICollection<Record> records) {
            this.Records = new RecordCollection(this, records);
        }


        internal EntryCollection Owner { get; set; }

        /// <summary>
        /// Used to mark document as changed.
        /// </summary>
        protected void MarkAsChanged() {
            if (this.Owner != null) { this.Owner.MarkAsChanged(); }
        }


        /// <summary>
        /// Gets/sets UUID.
        /// </summary>
        public Guid Uuid {
            get { return this.Records.Contains(RecordType.Uuid) ? this.Records[RecordType.Uuid].Uuid : Guid.Empty; }
            set { this.Records[RecordType.Uuid].Uuid = value; }
        }

        /// <summary>
        /// Gets/sets group.
        /// </summary>
        public GroupPath Group {
            get { return this.Records.Contains(RecordType.Group) ? this.Records[RecordType.Group].Text : ""; }
            set { this.Records[RecordType.Group].Text = value; }
        }


        /// <summary>
        /// Gets/sets title.
        /// </summary>
        public string Title {
            get { return this.Records.Contains(RecordType.Title) ? this.Records[RecordType.Title].Text : ""; }
            set { this.Records[RecordType.Title].Text = value; }
        }

        /// <summary>
        /// Gets/sets user name.
        /// </summary>
        public string UserName {
            get { return this.Records.Contains(RecordType.UserName) ? this.Records[RecordType.UserName].Text : ""; }
            set { this.Records[RecordType.UserName].Text = value; }
        }

        /// <summary>
        /// Gets/sets notes.
        /// </summary>
        public string Notes {
            get { return this.Records.Contains(RecordType.Notes) ? this.Records[RecordType.Notes].Text : ""; }
            set { this.Records[RecordType.Notes].Text = value; }
        }

        /// <summary>
        /// Gets/sets password.
        /// </summary>
        public string Password {
            get { return this.Records.Contains(RecordType.Password) ? this.Records[RecordType.Password].Text : ""; }
            set { this.Records[RecordType.Password].Text = value; }
        }


        /// <summary>
        /// Gets/sets creation time.
        /// </summary>
        public DateTime CreationTime {
            get { return this.Records.Contains(RecordType.CreationTime) ? this.Records[RecordType.CreationTime].Time : DateTime.MinValue; }
            set { this.Records[RecordType.CreationTime].Time = value; }
        }

        /// <summary>
        /// Gets/sets password modification time.
        /// </summary>
        public DateTime PasswordModificationTime {
            get { return this.Records.Contains(RecordType.PasswordModificationTime) ? this.Records[RecordType.PasswordModificationTime].Time : DateTime.MinValue; }
            set { this.Records[RecordType.PasswordModificationTime].Time = value; }
        }

        /// <summary>
        /// Gets/sets last access time.
        /// </summary>
        public DateTime LastAccessTime {
            get { return this.Records.Contains(RecordType.LastAccessTime) ? this.Records[RecordType.LastAccessTime].Time : DateTime.MinValue; }
            set { this.Records[RecordType.LastAccessTime].Time = value; }
        }

        /// <summary>
        /// Gets/sets password expiry time.
        /// </summary>
        public DateTime PasswordExpiryTime {
            get { return this.Records.Contains(RecordType.PasswordExpiryTime) ? this.Records[RecordType.PasswordExpiryTime].Time : DateTime.MinValue; }
            set { this.Records[RecordType.PasswordExpiryTime].Time = value; }
        }

        /// <summary>
        /// Gets/sets last modification time.
        /// </summary>
        public DateTime LastModificationTime {
            get { return this.Records.Contains(RecordType.LastModificationTime) ? this.Records[RecordType.LastModificationTime].Time : DateTime.MinValue; }
            set { this.Records[RecordType.LastModificationTime].Time = value; }
        }


        /// <summary>
        /// Gets/sets URL.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Password Safe file format doesn't require this URL to follow URL format.")]
        public string Url {
            get { return this.Records.Contains(RecordType.Url) ? this.Records[RecordType.Url].Text : ""; }
            set { this.Records[RecordType.Url].Text = value; }
        }


        /// <summary>
        /// Gets/sets e-mail address.
        /// </summary>
        public string Email {
            get { return this.Records.Contains(RecordType.EmailAddress) ? this.Records[RecordType.EmailAddress].Text : ""; }
            set { this.Records[RecordType.EmailAddress].Text = value; }
        }


        /// <summary>
        /// Gets/sets two factor key.
        /// Should be encoded as base 32.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Property returns copy of the array (in Field.GetBytes() and Field.SetBytes()).")]
        public byte[] TwoFactorKey {
            get { return this.Records.Contains(RecordType.TwoFactorKey) ? this.Records[RecordType.TwoFactorKey].GetBytes() : new byte[0]; }
            set { this.Records[RecordType.TwoFactorKey].SetBytes(value); }
        }


        /// <summary>
        /// Gets/sets credit card number.
        /// Number should consist of digits and spaces.
        /// </summary>
        public string CreditCardNumber {
            get { return this.Records.Contains(RecordType.CreditCardNumber) ? this.Records[RecordType.CreditCardNumber].Text : ""; }
            set { this.Records[RecordType.CreditCardNumber].Text = value; }
        }

        /// <summary>
        /// Gets/sets credit card expiration.
        /// Format should be MM/YY, where MM is 01-12, and YY 00-99.
        /// </summary>
        public string CreditCardExpiration {
            get { return this.Records.Contains(RecordType.CreditCardExpiration) ? this.Records[RecordType.CreditCardExpiration].Text : ""; }
            set { this.Records[RecordType.CreditCardExpiration].Text = value; }
        }

        /// <summary>
        /// Gets/sets credit card verification value.
        /// CVV (CVV2) is three or four digits.
        /// </summary>
        public string CreditCardVerificationValue {
            get { return this.Records.Contains(RecordType.CreditCardVerificationValue) ? this.Records[RecordType.CreditCardVerificationValue].Text : ""; }
            set { this.Records[RecordType.CreditCardVerificationValue].Text = value; }
        }

        /// <summary>
        /// Gets/sets credit card PIN.
        /// PIN is four to twelve digits long (ISO-9564).
        /// </summary>
        public string CreditCardPin {
            get { return this.Records.Contains(RecordType.CreditCardPin) ? this.Records[RecordType.CreditCardPin].Text : ""; }
            set { this.Records[RecordType.CreditCardPin].Text = value; }
        }

        /// <summary>
        /// Gets/sets UTF-8 encoded text used for QR code generation.
        /// </summary>
        public string QRCode {
            get { return this.Records.Contains(RecordType.QRCode) ? this.Records[RecordType.QRCode].Text : ""; }
            set { this.Records[RecordType.QRCode].Text = value; }
        }


        #region Auto-type

        /// <summary>
        /// Gets/sets auto-type text.
        /// </summary>
        public string Autotype {
            get { return this.Records.Contains(RecordType.Autotype) ? this.Records[RecordType.Autotype].Text : ""; }
            set { this.Records[RecordType.Autotype].Text = value; }
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
            get {
                foreach (var token in this.RawAutotypeTokens) {
                    if (token.Kind == AutotypeTokenKind.Command) {
                        var parts = token.Content.Split(':');
                        var command = parts[0];
                        var argument = (parts.Length > 1) ? parts[1] : null;
                        switch (command) {
                            case "UserName": foreach (var key in AutotypeToken.GetIndividualKeyTokens(this.UserName)) { yield return key; } break;
                            case "Password": foreach (var key in AutotypeToken.GetIndividualKeyTokens(this.Password)) { yield return key; } break;
                            case "TwoFactorCode": yield return token; break;

                            case "CreditCardNumber": foreach (var key in AutotypeToken.GetIndividualKeyTokens(this.CreditCardNumber)) { yield return key; } break;
                            case "CreditCardExpiration": foreach (var key in AutotypeToken.GetIndividualKeyTokens(this.CreditCardExpiration)) { yield return key; } break;
                            case "CreditCardVerificationValue": foreach (var key in AutotypeToken.GetIndividualKeyTokens(this.CreditCardVerificationValue)) { yield return key; } break;
                            case "CreditCardPin": foreach (var key in AutotypeToken.GetIndividualKeyTokens(this.CreditCardPin)) { yield return key; } break;

                            case "Group": foreach (var key in AutotypeToken.GetIndividualKeyTokens(this.Group)) { yield return key; } break;
                            case "Title": foreach (var key in AutotypeToken.GetIndividualKeyTokens(this.Title)) { yield return key; } break;
                            case "Url": foreach (var key in AutotypeToken.GetIndividualKeyTokens(this.Url)) { yield return key; } break;
                            case "Email": foreach (var key in AutotypeToken.GetIndividualKeyTokens(this.Email)) { yield return key; } break;

                            case "Notes":
                                var noteLines = this.Notes.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
                                if (string.IsNullOrEmpty(argument)) {
                                    foreach (var key in AutotypeToken.GetIndividualKeyTokens(string.Join("\n", noteLines))) {
                                        yield return key;
                                    }
                                } else {
                                    int lineNumber;
                                    if (int.TryParse(argument, NumberStyles.Integer, CultureInfo.InvariantCulture, out lineNumber)) {
                                        if (lineNumber <= noteLines.Length) {
                                            var lineText = noteLines[lineNumber - 1];
                                            if (lineText.Length > 0) {
                                                foreach (var key in AutotypeToken.GetIndividualKeyTokens(lineText)) {
                                                    yield return key;
                                                }
                                            }
                                        }
                                    }
                                }
                                break;

                            case "Delay": yield return token; break;
                            case "Wait": yield return token; break;
                            case "Legacy": yield return token; break;
                        }
                    } else {
                        yield return token;
                    }
                }
            }
        }

        /// <summary>
        /// Return auto-type tokens without any field processing; i.e. UserName won't be converted to actual user name.
        /// </summary>
        public IEnumerable<AutotypeToken> RawAutotypeTokens {
            get {
                if (string.IsNullOrEmpty(this.Autotype)) {
                    yield return new AutotypeToken("UserName", AutotypeTokenKind.Command);
                    yield return new AutotypeToken("{Tab}");
                    yield return new AutotypeToken("Password", AutotypeTokenKind.Command);
                    yield return new AutotypeToken("{Tab}");
                    yield return new AutotypeToken("{Enter}");
                } else {
                    var state = AutoTypeState.Default;
                    string command = null;
                    var sbCommandArguments = new StringBuilder();
                    foreach (var ch in this.Autotype) {
                        switch (state) {
                            case AutoTypeState.Default:
                                if (ch == '\\') {
                                    state = AutoTypeState.Escape;
                                } else {
                                    yield return new AutotypeToken(ch.ToString());
                                }
                                break;

                            case AutoTypeState.Escape:
                                switch (ch) {
                                    case 'u':
                                    case 'p':
                                    case '2':
                                    case 'g':
                                    case 'i':
                                    case 'l':
                                    case 'm':
                                    case 'z': //single character escape
                                        yield return GetCommandToken(ch.ToString(), null);
                                        state = AutoTypeState.Default;
                                        break;

                                    case 'c': //double character escape
                                        state = AutoTypeState.EscapeCreditCard;
                                        break;

                                    case 'b':
                                        yield return new AutotypeToken("{Backspace}");
                                        state = AutoTypeState.Default;
                                        break;

                                    case 't':
                                        yield return new AutotypeToken("{Tab}");
                                        state = AutoTypeState.Default;
                                        break;

                                    case 's':
                                        yield return new AutotypeToken("+{Tab}");
                                        state = AutoTypeState.Default;
                                        break;

                                    case 'n':
                                        yield return new AutotypeToken("{Enter}");
                                        state = AutoTypeState.Default;
                                        break;

                                    case 'd':
                                    case 'w':
                                    case 'W': //mandatory number characters
                                        command = ch.ToString();
                                        state = AutoTypeState.EscapeMandatoryNumber;
                                        break;

                                    case 'o': //optional number characters
                                        command = ch.ToString();
                                        state = AutoTypeState.EscapeOptionalNumber;
                                        break;

                                    default: //if escape doesn't exist
                                        yield return new AutotypeToken(ch.ToString());
                                        state = AutoTypeState.Default;
                                        break;
                                }
                                break;

                            case AutoTypeState.EscapeCreditCard:
                                switch (ch) {
                                    case 'n':
                                    case 'e':
                                    case 'v':
                                    case 'p': //double character escapes
                                        yield return GetCommandToken("c" + ch.ToString(), null);
                                        state = AutoTypeState.Default;
                                        break;

                                    default: //if escape doesn't exist
                                        foreach (var key in AutotypeToken.GetIndividualKeyTokens("c" + ch)) {
                                            yield return key;
                                        }
                                        state = AutoTypeState.Default;
                                        break;
                                }
                                break;

                            case AutoTypeState.EscapeMandatoryNumber:
                                if (char.IsDigit(ch)) {
                                    sbCommandArguments.Append(ch);
                                    state = AutoTypeState.EscapeOptionalNumber;
                                } else {
                                    foreach (var key in AutotypeToken.GetIndividualKeyTokens(command + sbCommandArguments.ToString() + ch)) {
                                        yield return key;
                                    }
                                    command = null;
                                    state = AutoTypeState.Default;
                                }
                                break;


                            case AutoTypeState.EscapeOptionalNumber:
                                if (char.IsDigit(ch) && (sbCommandArguments.Length < 3)) {
                                    sbCommandArguments.Append(ch);
                                } else {
                                    yield return GetCommandToken(command, sbCommandArguments.ToString());
                                    command = null; sbCommandArguments.Length = 0;
                                    if (ch == '\\') {
                                        state = AutoTypeState.Escape;
                                    } else {
                                        yield return new AutotypeToken(ch.ToString());
                                        state = AutoTypeState.Default;
                                    }
                                }
                                break;

                            default: throw new NotImplementedException("Unknown state");
                        }
                    }

                    if (command != null) {
                        if ((sbCommandArguments.Length == 0) && (command.Equals("d", StringComparison.Ordinal) || command.Equals("w", StringComparison.Ordinal) || command.Equals("W", StringComparison.Ordinal))) {
                            foreach (var key in AutotypeToken.GetIndividualKeyTokens(command)) {
                                yield return key;
                            }
                        } else {
                            yield return GetCommandToken(command, sbCommandArguments.ToString());
                        }
                    } else if (state == AutoTypeState.Escape) {
                        yield return new AutotypeToken(@"\");
                    }
                }
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Code is straightforward list of returns.")]
        private static AutotypeToken GetCommandToken(string command, string argument) {
            switch (command) {
                case "u": return new AutotypeToken("UserName", AutotypeTokenKind.Command);
                case "p": return new AutotypeToken("Password", AutotypeTokenKind.Command);
                case "2": return new AutotypeToken("TwoFactorCode", AutotypeTokenKind.Command);
                case "cn": return new AutotypeToken("CreditCardNumber", AutotypeTokenKind.Command);
                case "ce": return new AutotypeToken("CreditCardExpiration", AutotypeTokenKind.Command);
                case "cv": return new AutotypeToken("CreditCardVerificationValue", AutotypeTokenKind.Command);
                case "cp": return new AutotypeToken("CreditCardPin", AutotypeTokenKind.Command);
                case "g": return new AutotypeToken("Group", AutotypeTokenKind.Command);
                case "i": return new AutotypeToken("Title", AutotypeTokenKind.Command);
                case "l": return new AutotypeToken("Url", AutotypeTokenKind.Command);
                case "m": return new AutotypeToken("Email", AutotypeTokenKind.Command);
                case "o": return new AutotypeToken("Notes" + (string.IsNullOrEmpty(argument) ? "" : ":" + argument), AutotypeTokenKind.Command);
                case "d": return new AutotypeToken("Delay" + (string.IsNullOrEmpty(argument) ? "" : ":" + argument), AutotypeTokenKind.Command);
                case "w": return new AutotypeToken("Wait" + (string.IsNullOrEmpty(argument) ? "" : ":" + argument), AutotypeTokenKind.Command);
                case "W": return new AutotypeToken("Wait" + (string.IsNullOrEmpty(argument) ? "" : ":" + argument + "000"), AutotypeTokenKind.Command);
                case "z": return new AutotypeToken("Legacy", AutotypeTokenKind.Command);
                default: return new AutotypeToken(command);
            }
        }

        private enum AutoTypeState {
            Default,
            Escape,
            EscapeCreditCard,
            EscapeMandatoryNumber,
            EscapeOptionalNumber,
            EscapeNumber,
        }

        #endregion


        /// <summary>
        /// Gets password history.
        /// </summary>
        public PasswordHistoryCollection PasswordHistory {
            get { return new PasswordHistoryCollection(this.Records); }
        }


        /// <summary>
        /// Gets list of records.
        /// </summary>
        public RecordCollection Records { get; }


        /// <summary>
        /// Returns a string representation of an object.
        /// </summary>
        public override string ToString() {
            return this.Records.Contains(RecordType.Title) ? this.Records[RecordType.Title].Text : "";
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
            get { return this.Records[type]; }
            set { this.Records[type] = value; }
        }

        #endregion

    }
}
