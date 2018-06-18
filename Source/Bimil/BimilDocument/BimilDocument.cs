using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Medo.Security.Cryptography.Bimil {

    /// <summary>
    /// Encrypted document containing multiple items.
    /// </summary>
    public class BimilDocument : IDisposable {

        internal readonly RandomNumberGenerator Rng;
        internal readonly RijndaelManaged Crypto;
        private byte[] PasswordSalt;


        private BimilDocument() {
            this.Rng = RandomNumberGenerator.Create();

            this.Crypto = new RijndaelManaged {
                BlockSize = 128,
                KeySize = 128,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            };

            this.Items = new List<BimilItem>();
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="password">Password to use.</param>
        public BimilDocument(string password)
            : this() {
            this.PasswordSalt = new byte[16];
            this.Rng.GetBytes(this.PasswordSalt);

            var deriveBytes = new Rfc2898DeriveBytes(UTF8Encoding.UTF8.GetBytes(password), this.PasswordSalt, 4096);
            this.Crypto.Key = deriveBytes.GetBytes(16);
            this.Crypto.IV = deriveBytes.GetBytes(16);
        }


        /// <summary>
        /// Saves current document to stream.
        /// </summary>
        /// <param name="stream">Stream.</param>
        public void Save(Stream stream) {
            using (var timer = new Medo.Diagnostics.LifetimeWatch("Save")) {
                stream.Write(this.PasswordSalt, 0, this.PasswordSalt.Length);

                var buffer = new List<byte>(65536);
                buffer.AddRange(new byte[] { 0x41, 0x31, 0x32, 0x38 });
                foreach (var item in this.Items) {
                    var bytes = item.GetBytes();
                    buffer.AddRange(GetInt32Bytes(bytes.Length));
                    buffer.AddRange(bytes);
                }
                buffer.AddRange(new byte[] { 0x41, 0x31, 0x32, 0x38 });

                byte[] encBuffer;
                using (var enc = this.Crypto.CreateEncryptor()) {
                    encBuffer = enc.TransformFinalBlock(buffer.ToArray(), 0, buffer.Count);
                }

                stream.Write(encBuffer, 0, encBuffer.Length);
            }
        }

        /// <summary>
        /// Saves current document to file.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public void Save(string fileName) {
            using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                Save(stream);
            }
        }

        /// <summary>
        /// Saves current document to stream using another password. Current document is not affected.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <param name="newPassword">Password.</param>
        public void Save(Stream stream, string newPassword) {
            using (var newDoc = new BimilDocument(newPassword)) {
                foreach (var item in this.Items) {
                    var newItem = newDoc.AddItem(item.Name, item.IconIndex);
                    foreach (var record in item.Records) {
                        newItem.AddRecord(record.Key.Text, record.Value.Text, record.Format);
                    }
                }
                newDoc.Save(stream);
            }
        }

        /// <summary>
        /// Saves current document to file using another password. Current document is not affected.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="newPassword">Password.</param>
        public void Save(string fileName, string newPassword) {
            using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                Save(stream, newPassword);
            }
        }


        /// <summary>
        /// Returns document.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <param name="password">Password.</param>
        /// <exception cref="FormatException">Cannot parse document.</exception>
        public static BimilDocument Open(Stream stream, byte[] passphraseBytes) {
            try {
                using (var timer = new Medo.Diagnostics.LifetimeWatch("Open")) {
                    var doc = new BimilDocument();

                    var salt = new byte[16];
                    stream.Read(salt, 0, 16);

                    doc.PasswordSalt = salt;

                    var deriveBytes = new Rfc2898DeriveBytes(passphraseBytes, doc.PasswordSalt, 4096);
                    doc.Crypto.Key = deriveBytes.GetBytes(16);
                    doc.Crypto.IV = deriveBytes.GetBytes(16);

                    var encBuffer = new byte[stream.Length - 16];
                    stream.Read(encBuffer, 0, encBuffer.Length);

                    byte[] decBuffer;
                    using (var dec = doc.Crypto.CreateDecryptor()) {
                        decBuffer = dec.TransformFinalBlock(encBuffer, 0, encBuffer.Length);
                    }
                    if ((decBuffer[0] == 0x41) && (decBuffer[1] == 0x31) && (decBuffer[2] == 0x32) && (decBuffer[3] == 0x38)) {
                        if ((decBuffer[decBuffer.Length - 4] != 0x41) || (decBuffer[decBuffer.Length - 3] != 0x31) || (decBuffer[decBuffer.Length - 2] != 0x32) || (decBuffer[decBuffer.Length - 1] != 0x38)) { throw new FormatException("Invalid secondary identifier."); }
                        doc.Id = "A128";
                    } else {
                        throw new FormatException("Invalid primary identifier."); 
                    }

                    var currOffset = 4;
                    while (currOffset < (decBuffer.Length - 4)) {
                        var itemLen = GetInt32(decBuffer, currOffset);
                        var item = BimilItem.Parse(doc, decBuffer, currOffset + 4, itemLen);
                        doc.Items.Add(item);

                        currOffset += 4 + itemLen;
                    }

                    return doc;
                }
            } catch (CryptographicException ex) {
                throw new FormatException("Cannot parse document.", ex);
            } catch (OverflowException ex) {
                throw new FormatException("Cannot parse document.", ex);
            } catch (SecurityException ex) {
                throw new FormatException("Cannot parse document.", ex);
            }
        }

        /// <summary>
        /// Returns document.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="password">Password.</param>
        /// <exception cref="FormatException">Cannot parse document.</exception>
        public static BimilDocument Open(string fileName, byte[] passphraseBytes) {
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                return Open(stream, passphraseBytes);
            }
        }


        /// <summary>
        /// Gets document's ID.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets items.
        /// </summary>
        public IList<BimilItem> Items { get; private set; }


        /// <summary>
        /// Adds new item.
        /// </summary>
        /// <param name="name">Title.</param>
        /// <param name="iconIndex">Icon index.</param>
        public BimilItem AddItem(string name, int iconIndex) {
            var item = new BimilItem(this) { IconIndex = iconIndex };
            item.NameRecord.Value.Text = name;
            this.Items.Add(item);
            return item;
        }


        #region IDisposable

        /// <summary>
        /// Releases the unmanaged resources used by the System.IO.FileStream and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                this.Crypto.Clear();
            }
        }

        /// <summary>
        /// Releases all resources used.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion



        #region Helpers

        private static byte[] GetInt32Bytes(int value) {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) {
                return new byte[] { bytes[3], bytes[2], bytes[1], bytes[0] };
            } else {
                return bytes;
            }
        }

        private static int GetInt32(byte[] buffer, int offset) {
            if (BitConverter.IsLittleEndian) {
                return BitConverter.ToInt32(new byte[] { buffer[offset + 3], buffer[offset + 2], buffer[offset + 1], buffer[offset] }, 0);
            } else {
                return BitConverter.ToInt32(buffer, offset);
            }
        }

        #endregion

    }
}



// FORMAT
//--------
// 16x Salt
//  4x "A128"
//  4x Item[0]-Length
//  ?x Item[0]
//  4x Item[1]-Length
//  ?x Item[1]
//  4x Item[n]-Length
//  ?x Item[n]
//  4x "A128"
//
//   * Second "A128" is there so we can be sure that nobody changed the text.
