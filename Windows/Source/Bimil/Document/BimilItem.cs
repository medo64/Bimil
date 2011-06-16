using System;
using System.Collections.Generic;
using System.Text;

namespace Medo.Security.Cryptography.Bimil {

    /// <summary>
    /// Item with encrypted records.
    /// </summary>
    public class BimilItem {

        private readonly BimilDocument Document;

        internal BimilItem(BimilDocument document) {
            this.Document = document;
            this.Records = new List<BimilRecord>();
        }


        /// <summary>
        /// Gets/sets icon index.
        /// </summary>
        public int IconIndex { get; set; }

        /// <summary>
        /// Gets/sets name.
        /// </summary>
        public string Name {
            get { return this.NameRecord.Value.Text; }
        }

        /// <summary>
        /// Gets name record.
        /// </summary>
        public BimilRecord NameRecord {
            get { return GetSystemRecord("Name"); }
        }

        /// <summary>
        /// Gets category record.
        /// </summary>
        public BimilRecord CategoryRecord {
            get { return GetSystemRecord("Category"); }
        }


        private void SetSystemRecordValue(string key, string value) {
            foreach (var record in this.Records) {
                if (record.Format == BimilRecordFormat.System) {
                    if (string.Equals(key, record.Key.Text, StringComparison.Ordinal)) {
                        record.Value.Text = value;
                        return;
                    }
                }
            }
            {
                var record = new BimilRecord(this.Document, key, value, BimilRecordFormat.System);
                this.Records.Add(record);
            }
        }


        /// <summary>
        /// Gets list of records.
        /// </summary>
        public IList<BimilRecord> Records { get; private set; }

        public void ClearNonSystemRecords() {
            for (int i = this.Records.Count - 1; i >= 0; i--) {
                if (this.Records[i].Format != BimilRecordFormat.System) {
                    this.Records.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Adds new text record.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public BimilRecord AddRecord(string key, string value, BimilRecordFormat format) {
            var record = new BimilRecord(this.Document, key, value, format);
            this.Records.Add(record);
            return record;
        }

        /// <summary>
        /// Adds new text record.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public BimilRecord AddTextRecord(string key, string value) {
            return AddRecord(key, value, BimilRecordFormat.Text);
        }

        /// <summary>
        /// Adds new multiline text record.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public BimilRecord AddMultilineTextRecord(string key, string value) {
            return AddRecord(key, value, BimilRecordFormat.MultilineText);
        }

        /// <summary>
        /// Adds new fixed-font record.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public BimilRecord AddMonospacedTextRecord(string key, string value) {
            return AddRecord(key, value, BimilRecordFormat.MonospacedText);
        }

        /// <summary>
        /// Adds new URL record.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public BimilRecord AddUrlRecord(string key, string value) {
            return AddRecord(key, value, BimilRecordFormat.Url);
        }

        /// <summary>
        /// Adds new password record.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public BimilRecord AddPasswordRecord(string key, string value) {
            return AddRecord(key, value, BimilRecordFormat.Password);
        }


        internal byte[] GetBytes() {
            var buffer = new List<byte>(2048);
            foreach (var record in this.Records) {
                buffer.AddRange(GetInt32Bytes(record.Key.Bytes.Length));
                buffer.AddRange(GetInt32Bytes(record.Value.Bytes.Length));
                buffer.AddRange(GetInt32Bytes((int)record.Format));
                buffer.AddRange(record.Key.Bytes);
                buffer.AddRange(record.Value.Bytes);
            }
            return buffer.ToArray();
        }


        internal static BimilItem Parse(BimilDocument document, byte[] buffer, int offset, int count) {
            if (count < 4) { throw new FormatException("Invalid buffer size."); }

            var res = new BimilItem(document);

            int currOffset = offset;
            while (currOffset < (offset + count)) {
                int keyLength = GetInt32(buffer, currOffset);
                int valueLength = GetInt32(buffer, currOffset + 4);
                BimilRecordFormat type = (BimilRecordFormat)GetInt32(buffer, currOffset + 8);
                var key = BimilValue.Parse(document, buffer, currOffset + 12, keyLength);
                var value = BimilValue.Parse(document, buffer, currOffset + 12 + keyLength, valueLength);
                res.Records.Add(new BimilRecord(document, key.Text, value.Text, type));
                currOffset += 12 + keyLength + valueLength;
            }
            if (currOffset != (offset + count)) { throw new FormatException("Invalid buffer content."); }

            return res;
        }



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

        private BimilRecord GetSystemRecord(string key) {
            foreach (var record in this.Records) {
                if (record.Format == BimilRecordFormat.System) {
                    if (string.Equals(key, record.Key.Text, StringComparison.Ordinal)) {
                        return record;
                    }
                }
            }
            {
                var record = new BimilRecord(this.Document, key, "", BimilRecordFormat.System);
                this.Records.Add(record);
                return record;
            }
        }

        #endregion

    }
}



// FORMAT (A128)
//--------
//  4x Key[0]-Length
//  4x Value[0]-Length
//  4x Type[0]
//  ?x Key[0]  (Value*)
//  ?x Value[0]  (Value*)
//  4x Key[1]-Length
//  4x Value[1]-Length
//  4x Type[1]
//  ?x Key[1]  (Value*)
//  ?x Value[1]  (Value*)
//  4x Key[n]-Length
//  4x Value[n]-Length
//  4x Type[n]
//  ?x Key[n]  (Value*)
//  ?x Value[n]  (Value*)
//
//   * This data is not encrypted (it is not needed since document itself is encrypted).
