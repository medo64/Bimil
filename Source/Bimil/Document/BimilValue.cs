using System;
using System.Text;

namespace Medo.Security.Cryptography.Bimil {
    
    /// <summary>
    /// Encrypted value.
    /// </summary>
    public class BimilValue {

        private readonly BimilDocument Document;

        internal BimilValue(BimilDocument document) {
            this.Document = document;
            this.Text = "";
        }


        /// <summary>
        /// Gets/sets text.
        /// </summary>
        public string Text {
            get {
                if (this.Bytes == null) { return null; }
                byte[] decBuffer;
                using (var dec = this.Document.Crypto.CreateDecryptor()) {
                    decBuffer = dec.TransformFinalBlock(this.Bytes, 0, this.Bytes.Length);
                }
                return UTF8Encoding.UTF8.GetString(decBuffer, 16, decBuffer.Length - 16);
            }
            set {
                if (value == null) { throw new ArgumentNullException("value", "Value cannot be null."); }

                byte[] bufferSalt = new byte[16];
                this.Document.Rng.GetBytes(bufferSalt);
                byte[] bufferText = UTF8Encoding.UTF8.GetBytes(value);
                byte[] buffer = new byte[16 + bufferText.Length];

                Buffer.BlockCopy(bufferSalt, 0, buffer, 0, 16);
                Buffer.BlockCopy(bufferText, 0, buffer, 16, bufferText.Length);

                byte[] encBuffer;
                using (var enc = this.Document.Crypto.CreateEncryptor()) {
                    encBuffer = enc.TransformFinalBlock(buffer, 0, buffer.Length);
                }
                this.Bytes = encBuffer;
            }
        }

        internal byte[] Bytes { get; private set; }


        internal static BimilValue Parse(BimilDocument document, byte[] buffer, int offset, int count) {
            var encBuffer = new byte[count];
            Buffer.BlockCopy(buffer, offset, encBuffer, 0, count);

            return new BimilValue(document) { Bytes = encBuffer };
        }


        /// <summary>
        /// Returns text.
        /// </summary>
        /// <param name="value">Encrypted text.</param>
        public static implicit operator string(BimilValue value) {
            return value.Text;
        }


        /// <summary>
        /// Returns text.
        /// </summary>
        public override string ToString() {
            return this.Text;
        }

    }
}



// FORMAT
//--------
// 16x Salt
//  ?x Text
//
//   * Salt is there so we do not leak key with common text.
//   * Data is double-encrypted because it needs to stay as such once document is decrypted.
