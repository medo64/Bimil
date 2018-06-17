using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using static PasswordSafe.Helpers;

namespace Medo.Security.Cryptography.PasswordSafe {
    /// <summary>
    /// Password Safe document.
    /// </summary>
    public class Document : IDisposable {

        private Document() {
            this.Headers = new HeaderCollection(this, new Header[] {
                new Header(HeaderType.Version, BitConverter.GetBytes(Header.DefaultVersion)),
                new Header(HeaderType.Uuid,Guid.NewGuid().ToByteArray()),
            });
            this.Entries = new EntryCollection(this);
            this.NamedPasswordPolicies = new NamedPasswordPolicyCollection(this);

            this.TrackAccess = true;
            this.TrackModify = true;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="passphraseBuffer">Password bytes. Caller has to avoid keeping bytes unencrypted in memory.</param>
        public Document(byte[] passphraseBuffer)
            : this() {
            //no need for passphraseBuffer copy - will be done in property setter
            this.SetPassphraseBuffer(passphraseBuffer ?? throw new ArgumentNullException(nameof(passphraseBuffer), "Passphrase cannot be null."));
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="passphrase">Password.</param>
        /// <exception cref="ArgumentNullException">Passphrase cannot be null.</exception>
        public Document(string passphrase)
            : this() {
            this.SetPassphrase(passphrase);
        }


        internal Document(byte[] passphraseBuffer, int iterations, ICollection<Header> headers, params ICollection<Record>[] records) {
            this.Headers = new HeaderCollection(this, headers);
            this.NamedPasswordPolicies = new NamedPasswordPolicyCollection(this);
            this.Entries = new EntryCollection(this, records);
            this._iterations = iterations; //set directly to avoid usual minimum of 2048 iterations
            this.SetPassphrase(passphraseBuffer);

            this.TrackAccess = true;
            this.TrackModify = true;
        }


        /// <summary>
        /// Gets/sets database version.
        /// </summary>
        public int Version {
            get { return this.Headers[HeaderType.Version].Version; }
            set { this.Headers[HeaderType.Version].Version = value; }
        }


        /// <summary>
        /// Gets/sets UUID.
        /// </summary>
        public Guid Uuid {
            get { return this.Headers.Contains(HeaderType.Uuid) ? this.Headers[HeaderType.Uuid].Uuid : Guid.Empty; }
            set { this.Headers[HeaderType.Uuid].Uuid = value; }
        }

        /// <summary>
        /// Gets/sets last save time.
        /// </summary>
        public DateTime LastSaveTime {
            get { return this.Headers.Contains(HeaderType.TimestampOfLastSave) ? this.Headers[HeaderType.TimestampOfLastSave].Time : DateTime.MinValue; }
            set { this.Headers[HeaderType.TimestampOfLastSave].Time = value; }
        }

        /// <summary>
        /// Gets/sets last save application.
        /// </summary>
        public string LastSaveApplication {
            get { return this.Headers.Contains(HeaderType.WhatPerformedLastSave) ? this.Headers[HeaderType.WhatPerformedLastSave].Text : ""; }
            set { this.Headers[HeaderType.WhatPerformedLastSave].Text = value; }
        }

        /// <summary>
        /// Gets/sets last save user.
        /// </summary>
        public string LastSaveUser {
            get { return this.Headers.Contains(HeaderType.LastSavedByUser) ? this.Headers[HeaderType.LastSavedByUser].Text : ""; }
            set { this.Headers[HeaderType.LastSavedByUser].Text = value; }
        }

        /// <summary>
        /// Gets/sets last save computer.
        /// </summary>
        public string LastSaveHost {
            get { return this.Headers.Contains(HeaderType.LastSavedOnHost) ? this.Headers[HeaderType.LastSavedOnHost].Text : ""; }
            set { this.Headers[HeaderType.LastSavedOnHost].Text = value; }
        }

        /// <summary>
        /// Gets/sets database name.
        /// </summary>
        public string Name {
            get { return this.Headers.Contains(HeaderType.DatabaseName) ? this.Headers[HeaderType.DatabaseName].Text : ""; }
            set { this.Headers[HeaderType.DatabaseName].Text = value; }
        }

        /// <summary>
        /// Gets/sets database description.
        /// </summary>
        public string Description {
            get { return this.Headers.Contains(HeaderType.DatabaseDescription) ? this.Headers[HeaderType.DatabaseDescription].Text : ""; }
            set { this.Headers[HeaderType.DatabaseDescription].Text = value; }
        }


        private int _iterations = 2048;
        /// <summary>
        /// Gets/sets desired number of iterations.
        /// Cannot be less than 2048.
        /// </summary>
        public int Iterations {
            get { return this._iterations; }
            set {
                if (value < 2048) { value = 2048; }
                this._iterations = value;
            }
        }


        /// <summary>
        /// Gets list of headers.
        /// </summary>
        /// 
        public HeaderCollection Headers { get; }

        /// <summary>
        /// Gets list of entries.
        /// </summary>
        public EntryCollection Entries { get; }

        /// <summary>
        /// Gets list of named password policies.
        /// </summary>
        public NamedPasswordPolicyCollection NamedPasswordPolicies { get; }


        #region Load/Save

        private const int Tag = 0x33535750; //PWS3 in LE
        private const int TagEof = 0x464F452D; //-EOF in LE
        private static readonly Encoding Utf8Encoding = new UTF8Encoding(false);

        /// <summary>
        /// Loads data from a file.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="passphrase">Password.</param>
        /// <exception cref="ArgumentNullException">File name cannot be null. -or- Passphrase cannot be null.</exception>
        /// <exception cref="FormatException">Unrecognized file format. -or- Password mismatch. -or- Authentication mismatch.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "It is up to a caller to Dispose newly created document.")]
        public static Document Load(String fileName, string passphrase) {
            if (fileName == null) { throw new ArgumentNullException(nameof(fileName), "File name cannot be null."); }
            if (passphrase == null) { throw new ArgumentNullException(nameof(passphrase), "Passphrase cannot be null."); }

            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                return Load(stream, passphrase);
            }
        }

        /// <summary>
        /// Loads data from a file.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <param name="passphrase">Password.</param>
        /// <exception cref="ArgumentNullException">Stream cannot be null. -or- Passphrase cannot be null.</exception>
        /// <exception cref="FormatException">Unrecognized file format. -or- Password mismatch. -or- Authentication mismatch.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "It is up to a caller to Dispose newly created document.")]
        public static Document Load(Stream stream, string passphrase) {
            if (stream == null) { throw new ArgumentNullException(nameof(stream), "Stream cannot be null."); }
            if (passphrase == null) { throw new ArgumentNullException(nameof(passphrase), "Passphrase cannot be null."); }

            var passphraseBytes = Utf8Encoding.GetBytes(passphrase);
            try {
                return Load(stream, passphraseBytes);
            } finally {
                Array.Clear(passphraseBytes, 0, passphraseBytes.Length); //remove passphrase bytes from memory - nothing to do about the string. :(
            }
        }

        /// <summary>
        /// Loads data from a file.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <param name="passphraseBuffer">Password bytes. Caller has to avoid keeping bytes unencrypted in memory.</param>
        /// <exception cref="ArgumentNullException">Stream cannot be null. -or- Passphrase cannot be null.</exception>
        /// <exception cref="FormatException">Unrecognized file format. -or- Password mismatch. -or- Authentication mismatch.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "It is up to a caller to Dispose newly created document.")]
        public static Document Load(Stream stream, byte[] passphraseBuffer) {
            if (stream == null) { throw new ArgumentNullException(nameof(stream), "Stream cannot be null."); }
            if (passphraseBuffer == null) { throw new ArgumentNullException(nameof(passphraseBuffer), "Passphrase cannot be null."); }

            var buffer = new byte[16384];
            using (var ms = new MemoryStream()) {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0) {
                    ms.Write(buffer, 0, read);
                }
                buffer = ms.ToArray();
            }

            if ((buffer.Length < 200)
              || (BitConverter.ToInt32(buffer, 0) != Tag)
              || (BitConverter.ToInt32(buffer, buffer.Length - 32 - 16) != Tag)
              || (BitConverter.ToInt32(buffer, buffer.Length - 32 - 12) != TagEof)
              || (BitConverter.ToInt32(buffer, buffer.Length - 32 - 8) != Tag)
              || (BitConverter.ToInt32(buffer, buffer.Length - 32 - 4) != TagEof)) {
                throw new FormatException("Unrecognized file format.");
            }

            var salt = new byte[32];
            Buffer.BlockCopy(buffer, 4, salt, 0, salt.Length);

            var iter = BitConverter.ToUInt32(buffer, 36);

            byte[] stretchedKey = null, keyK = null, keyL = null, data = null;
            try {
                stretchedKey = GetStretchedKey(passphraseBuffer, salt, iter);
                if (!AreBytesTheSame(GetSha256Hash(stretchedKey), buffer, 40)) {
                    throw new CryptographicException("Password mismatch.");
                }

                keyK = DecryptKey(stretchedKey, buffer, 72);
                keyL = DecryptKey(stretchedKey, buffer, 104);

                var iv = new byte[16];
                Buffer.BlockCopy(buffer, 136, iv, 0, iv.Length);

                data = DecryptData(keyK, iv, buffer, 152, buffer.Length - 200);

                using (var dataHash = new HMACSHA256(keyL)) {
                    var dataOffset = 0;

                    var headerFields = new List<Header>();
                    while (dataOffset < data.Length) {
                        var fieldLength = BitConverter.ToInt32(data, dataOffset + 0);
                        var fieldLengthFull = ((fieldLength + 5 - 1) / 16 + 1) * 16;
                        var fieldType = (HeaderType)data[dataOffset + 4];
                        var fieldData = new byte[fieldLength];
                        try {
                            Buffer.BlockCopy(data, dataOffset + 5, fieldData, 0, fieldLength);
                            dataOffset += fieldLengthFull; //there is ALWAYS some random bytes added, thus extra block if 16 bytes

                            dataHash.TransformBlock(fieldData, 0, fieldData.Length, null, 0); //not hashing length nor type - wtf?
                            if (fieldType == HeaderType.EndOfEntry) { break; }

                            headerFields.Add(new Header(fieldType, fieldData));
                        } finally {
                            Array.Clear(fieldData, 0, fieldData.Length);
                        }
                    }

                    if ((headerFields.Count == 0) || (headerFields[0].Version < 0x0300)) { throw new FormatException("Unrecognized file format version."); }

                    var recordFields = new List<List<Record>>();
                    List<Record> records = null;
                    while (dataOffset < data.Length) {
                        var fieldLength = BitConverter.ToInt32(data, dataOffset + 0);
                        var fieldLengthFull = ((fieldLength + 5 - 1) / 16 + 1) * 16;
                        var fieldType = (RecordType)data[dataOffset + 4];
                        var fieldData = new byte[fieldLength];
                        try {
                            Buffer.BlockCopy(data, dataOffset + 5, fieldData, 0, fieldLength);
                            dataOffset += fieldLengthFull; //there is ALWAYS some random bytes added, thus extra block if 16 bytes

                            dataHash.TransformBlock(fieldData, 0, fieldData.Length, null, 0); //not hashing length nor type - wtf?
                            if (fieldType == RecordType.EndOfEntry) { records = null; continue; }

                            if (records == null) {
                                records = new List<Record>();
                                recordFields.Add(records);
                            }
                            records.Add(new Record(fieldType, fieldData));
                        } finally {
                            Array.Clear(fieldData, 0, fieldData.Length);
                        }
                    }

                    dataHash.TransformFinalBlock(new byte[] { }, 0, 0);

                    if (!AreBytesTheSame(dataHash.Hash, buffer, buffer.Length - 32)) {
                        throw new CryptographicException("Authentication mismatch.");
                    }

                    return new Document(passphraseBuffer, (int)iter, headerFields, recordFields.ToArray());
                }
            } catch (CryptographicException ex) {
                throw new FormatException(ex.Message, ex);
            } finally { //best effort to sanitize memory
                if (stretchedKey != null) { Array.Clear(stretchedKey, 0, stretchedKey.Length); }
                if (keyK != null) { Array.Clear(keyK, 0, keyK.Length); }
                if (keyL != null) { Array.Clear(keyL, 0, keyL.Length); }
                if (data != null) { Array.Clear(data, 0, data.Length); }
            }
        }


        /// <summary>
        /// Save document using the same password as for Load.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <exception cref="ArgumentNullException">File name cannot be null.</exception>
        /// <exception cref="NotSupportedException">Missing passphrase.</exception>
        public void Save(string fileName) {
            if (fileName == null) { throw new ArgumentNullException(nameof(fileName), "File name cannot be null."); }

            using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write)) {
                Save(stream);
            }
        }

        /// <summary>
        /// Save document using the same password as for Load.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <exception cref="ArgumentNullException">Stream cannot be null.</exception>
        /// <exception cref="NotSupportedException">Missing passphrase.</exception>
        public void Save(Stream stream) {
            if (stream == null) { throw new ArgumentNullException(nameof(stream), "Stream cannot be null."); }

            var passphraseBytes = this.GetPassphrase();
            if (passphraseBytes == null) { throw new NotSupportedException("Missing passphrase."); }
            try {
                Save(stream, passphraseBytes);
            } finally {
                Array.Clear(passphraseBytes, 0, passphraseBytes.Length); //remove passphrase bytes from memory - nothing to do about the string. :(
            }
        }

        /// <summary>
        /// Save document.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="passphrase">Password.</param>
        /// <exception cref="ArgumentNullException">File name cannot be null. -or- Passphrase cannot be null.</exception>
        public void Save(string fileName, string passphrase) {
            if (fileName == null) { throw new ArgumentNullException(nameof(fileName), "File name cannot be null."); }
            if (passphrase == null) { throw new ArgumentNullException(nameof(passphrase), "Passphrase cannot be null."); }

            using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write)) {
                Save(stream, passphrase);
            }
        }

        /// <summary>
        /// Save document.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <param name="passphrase">Password.</param>
        /// <exception cref="ArgumentNullException">Stream cannot be null. -or- Passphrase cannot be null.</exception>
        public void Save(Stream stream, string passphrase) {
            if (stream == null) { throw new ArgumentNullException(nameof(stream), "Stream cannot be null."); }
            if (passphrase == null) { throw new ArgumentNullException(nameof(passphrase), "Passphrase cannot be null."); }

            var passphraseBytes = Utf8Encoding.GetBytes(passphrase);
            try {
                Save(stream, passphraseBytes);
            } finally {
                Array.Clear(passphraseBytes, 0, passphraseBytes.Length); //remove passphrase bytes from memory - nothing to do about the string. :(
            }
        }


        /// <summary>
        /// Save document.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <param name="passphraseBuffer">Password bytes. Caller has to avoid keeping bytes unencrypted in memory.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Medo.Security.Cryptography.PasswordSafe.Field.set_Text(System.String)", Justification = "String is not exposed to the end user.")]
        public void Save(Stream stream, byte[] passphraseBuffer) {
            if (stream == null) { throw new ArgumentNullException(nameof(stream), "Stream cannot be null."); }
            if (passphraseBuffer == null) { throw new ArgumentNullException(nameof(passphraseBuffer), "Passphrase cannot be null."); }

            if (!this.IsReadOnly && this.TrackModify) {
                this.Headers[HeaderType.TimestampOfLastSave].Time = DateTime.UtcNow;

                var assemblyName = Assembly.GetExecutingAssembly().GetName();
                this.Headers[HeaderType.WhatPerformedLastSave].Text = string.Format(CultureInfo.InvariantCulture, "{0} V{1}.{2:00}", assemblyName.Name, assemblyName.Version.Major, assemblyName.Version.Minor);

                this.Headers[HeaderType.LastSavedByUser].Text = Environment.UserName;
                this.Headers[HeaderType.LastSavedOnHost].Text = Environment.MachineName;
            }

            byte[] stretchedKey = null;
            byte[] keyK = null;
            byte[] keyL = null;
            //byte[] data = null;
            try {
                stream.Write(BitConverter.GetBytes(Tag), 0, 4);

                var salt = new byte[32];
                Rnd.GetBytes(salt);
                stream.Write(salt, 0, salt.Length);

                this.Iterations = this.Iterations; //to force minimum iteration count
                var iter = (uint)this.Iterations;
                stream.Write(BitConverter.GetBytes(iter), 0, 4);

                stretchedKey = GetStretchedKey(passphraseBuffer, salt, iter);
                stream.Write(GetSha256Hash(stretchedKey), 0, 32);

                keyK = new byte[32];
                Rnd.GetBytes(keyK);
                stream.Write(EncryptKey(stretchedKey, keyK, 0), 0, 32);

                keyL = new byte[32];
                Rnd.GetBytes(keyL);
                stream.Write(EncryptKey(stretchedKey, keyL, 0), 0, 32);

                var iv = new byte[16];
                Rnd.GetBytes(iv);
                stream.Write(iv, 0, iv.Length);

                using (var dataHash = new HMACSHA256(keyL))
                using (var twofish = new TwofishManaged()) {
                    twofish.Mode = CipherMode.CBC;
                    twofish.Padding = PaddingMode.None;
                    twofish.KeySize = 256;
                    twofish.Key = keyK;
                    twofish.IV = iv;
                    using (var dataEncryptor = twofish.CreateEncryptor()) {
                        foreach (var field in this.Headers) {
                            WriteBlock(stream, dataHash, dataEncryptor, (byte)field.HeaderType, field.RawData);
                        }
                        WriteBlock(stream, dataHash, dataEncryptor, (byte)HeaderType.EndOfEntry, new byte[] { });

                        foreach (var entry in this.Entries) {
                            foreach (var field in entry.Records) {
                                WriteBlock(stream, dataHash, dataEncryptor, (byte)field.RecordType, field.RawData);
                            }
                            WriteBlock(stream, dataHash, dataEncryptor, (byte)RecordType.EndOfEntry, new byte[] { });
                        }
                    }

                    dataHash.TransformFinalBlock(new byte[] { }, 0, 0);

                    stream.Write(BitConverter.GetBytes(Tag), 0, 4);
                    stream.Write(BitConverter.GetBytes(TagEof), 0, 4);
                    stream.Write(BitConverter.GetBytes(Tag), 0, 4);
                    stream.Write(BitConverter.GetBytes(TagEof), 0, 4);

                    stream.Write(dataHash.Hash, 0, dataHash.Hash.Length);
                    this.HasChanged = false;
                }
            } finally {
                if (stretchedKey != null) { Array.Clear(stretchedKey, 0, stretchedKey.Length); }
                if (keyK != null) { Array.Clear(keyK, 0, keyK.Length); }
                if (keyL != null) { Array.Clear(keyL, 0, keyL.Length); }
                //if (data != null) { Array.Clear(data, 0, data.Length); }
            }
        }


        #region Passphrase

        private static RandomNumberGenerator Rnd = RandomNumberGenerator.Create();
        private byte[] PassphraseEntropy = new byte[16];

        private byte[] _passphraseBuffer;

        /// <summary>
        /// Returns passphrase used to open a file.
        /// Bytes are kept encrypted in memory until accessed.
        /// It's caller responsibility to dispose of returned bytes.
        /// </summary>
        public byte[] GetPassphrase() {
            return (this._passphraseBuffer != null) ? UnprotectData(this._passphraseBuffer, this.PassphraseEntropy) : null;
        }

        /// <summary>
        /// Sets password.
        /// </summary>
        /// <param name="newPassphraseBuffer">New password bytes. Buffer is cleared upon execution.</param>
        /// <exception cref="ArgumentNullException">Passphrase cannot be null.</exception>
        private void SetPassphrase(byte[] newPassphraseBuffer) {
            if (newPassphraseBuffer == null) { throw new ArgumentNullException(nameof(newPassphraseBuffer), "Passphrase cannot be null."); }

            try {
                Rnd.GetBytes(this.PassphraseEntropy);
                this._passphraseBuffer = ProtectData(newPassphraseBuffer, this.PassphraseEntropy);
            } finally {
                Array.Clear(newPassphraseBuffer, 0, newPassphraseBuffer.Length);
            }
        }


        /// <summary>
        /// Change password.
        /// </summary>
        /// <param name="newPassphraseBuffer">New password bytes. Buffer is cleared upon execution.</param>
        /// <exception cref="ArgumentNullException">Passphrase cannot be null.</exception>
        public void ChangePassphrase(byte[] newPassphraseBuffer) {
            this.SetPassphrase(newPassphraseBuffer);
            this.MarkAsChanged();
        }

        /// <summary>
        /// Change password.
        /// </summary>
        /// <param name="newPassphrase">New passphrase. Caller has to avoid keeping bytes unencrypted in memory.</param>
        public void ChangePassphrase(string newPassphrase) {
            if (newPassphrase == null) { throw new ArgumentNullException(nameof(newPassphrase), "Passphrase cannot be null."); }

            var passphraseBuffer = Utf8Encoding.GetBytes(newPassphrase);
            try {
                this.ChangePassphrase(passphraseBuffer);
            } finally {
                Array.Clear(passphraseBuffer, 0, passphraseBuffer.Length); //remove passphrase bytes from memory - nothing to do about the string. :(
            }
        }


        /// <summary>
        /// Change password only if old password matches and returns whether operation was successful.
        /// </summary>
        /// <param name="oldPassphraseBuffer">Old password bytes. Caller has to avoid keeping bytes unencrypted in memory.</param>
        /// <param name="newPassphraseBuffer">New password bytes. Caller has to avoid keeping bytes unencrypted in memory.</param>
        public bool TryChangePassphrase(byte[] oldPassphraseBuffer, byte[] newPassphraseBuffer) {
            if (oldPassphraseBuffer == null) { throw new ArgumentNullException(nameof(oldPassphraseBuffer), "Passphrase cannot be null."); }
            if (newPassphraseBuffer == null) { throw new ArgumentNullException(nameof(newPassphraseBuffer), "Passphrase cannot be null."); }

            if (this.ValidatePassphrase(oldPassphraseBuffer)) {
                this.ChangePassphrase(newPassphraseBuffer);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Change password only if old password matches and returns whether operation was successful.
        /// </summary>
        /// <param name="oldPassphrase">Old password.</param>
        /// <param name="newPassphrase">New password.</param>
        public bool TryChangePassphrase(string oldPassphrase, string newPassphrase) {
            if (oldPassphrase == null) { throw new ArgumentNullException(nameof(oldPassphrase), "Passphrase cannot be null."); }
            if (newPassphrase == null) { throw new ArgumentNullException(nameof(newPassphrase), "Passphrase cannot be null."); }

            var oldPassphraseBuffer = Utf8Encoding.GetBytes(oldPassphrase);
            var newPassphraseBuffer = Utf8Encoding.GetBytes(newPassphrase);
            try {
                return this.TryChangePassphrase(oldPassphraseBuffer, newPassphraseBuffer);
            } finally {
                Array.Clear(oldPassphraseBuffer, 0, oldPassphraseBuffer.Length); //remove passphrase bytes from memory - nothing to do about the string. :(
                Array.Clear(newPassphraseBuffer, 0, newPassphraseBuffer.Length); //remove passphrase bytes from memory - nothing to do about the string. :(
            }
        }


        /// <summary>
        /// Returns true if password matches.
        /// </summary>
        /// <param name="oldPassphraseBuffer">Old password bytes. Caller has to avoid keeping bytes unencrypted in memory.</param>
        public bool ValidatePassphrase(byte[] oldPassphraseBuffer) {
            if (oldPassphraseBuffer == null) { throw new ArgumentNullException(nameof(oldPassphraseBuffer), "Passphrase cannot be null."); }

            var currPassphraseBuffer = this.GetPassphrase();
            try {
                if (currPassphraseBuffer != null) {
                    if (currPassphraseBuffer.Length != oldPassphraseBuffer.Length) { return false; }
                    for (var i = 0; i < currPassphraseBuffer.Length; i++) {
                        if (currPassphraseBuffer[i] != oldPassphraseBuffer[i]) { return false; }
                    }
                }
                return true;
            } finally {
                Array.Clear(currPassphraseBuffer, 0, currPassphraseBuffer.Length); //remove passphrase bytes from memory - nothing to do about the string. :(
            }
        }

        /// <summary>
        /// Returns true if password matches.
        /// </summary>
        /// <param name="oldPassphrase">Old password.</param>
        public bool ValidatePassphrase(string oldPassphrase) {
            if (oldPassphrase == null) { throw new ArgumentNullException(nameof(oldPassphrase), "Passphrase cannot be null."); }

            var oldPassphraseBuffer = Utf8Encoding.GetBytes(oldPassphrase);
            try {
                return this.ValidatePassphrase(oldPassphraseBuffer);
            } finally {
                Array.Clear(oldPassphraseBuffer, 0, oldPassphraseBuffer.Length); //remove passphrase bytes from memory - nothing to do about the string. :(
            }
        }

        #endregion Passphrase


        private static void WriteBlock(Stream stream, HashAlgorithm dataHash, ICryptoTransform dataEncryptor, byte type, byte[] fieldData) {
            dataHash.TransformBlock(fieldData, 0, fieldData.Length, null, 0);

            byte[] fieldBlock = null;
            try {
                var fieldLengthPadded = ((fieldData.Length + 5 - 1) / 16 + 1) * 16;
                fieldBlock = new byte[fieldLengthPadded];

                Rnd.GetBytes(fieldBlock);
                Buffer.BlockCopy(BitConverter.GetBytes(fieldData.Length), 0, fieldBlock, 0, 4);
                fieldBlock[4] = type;
                Buffer.BlockCopy(fieldData, 0, fieldBlock, 5, fieldData.Length);

                dataEncryptor.TransformBlock(fieldBlock, 0, fieldBlock.Length, fieldBlock, 0);
                stream.Write(fieldBlock, 0, fieldBlock.Length);
            } finally {
                Array.Clear(fieldData, 0, fieldData.Length);
                if (fieldBlock != null) { Array.Clear(fieldBlock, 0, fieldBlock.Length); }
            }
        }


        /// <summary>
        /// Sets passphrase to be used for the file.
        /// Bytes are kept encrypted in memory until accessed.
        /// Buffer is cleared upon exit.
        /// </summary>
        /// <param name="passphraseBuffer">Passphrase bytes.</param>
        /// <exception cref="ArgumentNullException">Passphrase cannot be null.</exception>
        public void SetPassphraseBuffer(byte[] passphraseBuffer) {
            if (passphraseBuffer == null) { throw new ArgumentNullException(nameof(passphraseBuffer), "Passphrase cannot be null."); }
            try {
                Rnd.GetBytes(this.PassphraseEntropy);
                this._passphraseBuffer = ProtectData(passphraseBuffer, this.PassphraseEntropy);
            } finally {
                Array.Clear(passphraseBuffer, 0, passphraseBuffer.Length);
            }
        }

        /// <summary>
        /// Sets passphrase to be used for the file.
        /// </summary>
        /// <param name="passphrase">Passphrase.</param>
        /// <exception cref="ArgumentNullException">Passphrase cannot be null.</exception>
        public void SetPassphrase(string passphrase) {
            if (passphrase == null) { throw new ArgumentNullException(nameof(passphrase), "Passphrase cannot be null."); }

            var passphraseBuffer = Utf8Encoding.GetBytes(passphrase);
            this.SetPassphraseBuffer(passphraseBuffer);
        }

        #endregion Load/Save


        /// <summary>
        /// Gets/sets if document is read-only.
        /// </summary>
        public bool IsReadOnly { get; set; }


        /// <summary>
        /// Gets if document will automatically fill fields with access information.
        /// </summary>
        public bool TrackAccess { get; set; }

        /// <summary>
        /// Gets if document will automatically fill fields with modification information.
        /// </summary>
        public bool TrackModify { get; set; }


        private bool _hasChanged;
        /// <summary>
        /// Gets is document has been changed since last save.
        /// </summary>
        public bool HasChanged {
            get { return this._hasChanged; }
            private set { this._hasChanged = value; }
        }

        internal void MarkAsChanged() {
            this.HasChanged = true;
        }


        #region Utility functions

        private static byte[] DecryptKey(byte[] stretchedKey, byte[] buffer, int offset) {
            using (var twofish = new TwofishManaged()) {
                twofish.Mode = CipherMode.ECB;
                twofish.Padding = PaddingMode.None;
                twofish.KeySize = 256;
                twofish.Key = stretchedKey;
                using (var transform = twofish.CreateDecryptor()) {
                    return transform.TransformFinalBlock(buffer, offset, 32);
                }
            }
        }

        private static byte[] EncryptKey(byte[] stretchedKey, byte[] buffer, int offset) {
            using (var twofish = new TwofishManaged()) {
                twofish.Mode = CipherMode.ECB;
                twofish.Padding = PaddingMode.None;
                twofish.KeySize = 256;
                twofish.Key = stretchedKey;
                using (var transform = twofish.CreateEncryptor()) {
                    return transform.TransformFinalBlock(buffer, offset, 32);
                }
            }
        }

        private static byte[] DecryptData(byte[] key, byte[] iv, byte[] buffer, int offset, int length) {
            using (var twofish = new TwofishManaged()) {
                twofish.Mode = CipherMode.CBC;
                twofish.Padding = PaddingMode.None;
                twofish.KeySize = 256;
                twofish.Key = key;
                twofish.IV = iv;
                using (var dataDecryptor = twofish.CreateDecryptor()) {
                    return dataDecryptor.TransformFinalBlock(buffer, offset, length);
                }
            }
        }

        private static byte[] GetStretchedKey(byte[] passphrase, byte[] salt, uint iterations) {
            var hash = GetSha256Hash(passphrase, salt);
            for (var i = 0; i < iterations; i++) {
                hash = GetSha256Hash(hash);
            }
            return hash;
        }

        private static byte[] GetSha256Hash(params byte[][] buffers) {
            using (var hash = new SHA256Managed()) {
                foreach (var buffer in buffers) {
                    hash.TransformBlock(buffer, 0, buffer.Length, buffer, 0);
                }
                hash.TransformFinalBlock(new byte[] { }, 0, 0);
                return hash.Hash;
            }
        }

        private static bool AreBytesTheSame(byte[] buffer1, byte[] buffer2, int buffer2Offset) {
            if (buffer1.Length == 0) { return false; }
            if (buffer2Offset + buffer1.Length > buffer2.Length) { return false; }
            for (var i = 0; i < buffer1.Length; i++) {
                if (buffer1[i] != buffer2[buffer2Offset + i]) { return false; }
            }
            return true;
        }

        #endregion


        #region IDisposable

        /// <summary>
        /// Disposes resources.
        /// </summary>
        /// <param name="disposing">True if managed resources are to be disposed.</param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                if (this._passphraseBuffer != null) { Array.Clear(this._passphraseBuffer, 0, this._passphraseBuffer.Length); }
            }
        }

        /// <summary>
        /// Disposes resources.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}
