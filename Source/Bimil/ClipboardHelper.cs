using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Medo.Security.Cryptography.PasswordSafe;

namespace Bimil {
    internal static class ClipboardHelper {

        private static readonly string FormatName = "Bimil";

        public static void SetClipboardData(Entry entry) {
            var bytes = new List<byte>();

            foreach (var record in entry.Records) {
                bytes.AddRange(BitConverter.GetBytes((int)record.RecordType));
                bytes.AddRange(BitConverter.GetBytes(record.RawDataDirect.Length));
                bytes.AddRange(record.RawDataDirect);
            }
            var buffer = bytes.ToArray();
            for (int i = 0; i < bytes.Count; i++) { bytes[i] = 0; }

            var protectedBuffer = ProtectedData.Protect(buffer, null, DataProtectionScope.CurrentUser);
            Array.Clear(buffer, 0, buffer.Length);

            Clipboard.Clear();
            Clipboard.SetData(FormatName, protectedBuffer);
        }

        public static Entry GetClipboardData() {
            if (Clipboard.ContainsData(FormatName)) {
                if (Clipboard.GetData(FormatName) is byte[] protectedBuffer) {
                    var buffer = ProtectedData.Unprotect(protectedBuffer, null, DataProtectionScope.CurrentUser);
                    var offset = 0;
                    try {
                        var records = new List<Record>();
                        while (offset < buffer.Length) {
                            var type = BitConverter.ToInt32(buffer, offset); offset += 4;
                            var length = BitConverter.ToInt32(buffer, offset); offset += 4;
                            var dataBytes = new byte[length];
                            Buffer.BlockCopy(buffer, offset, dataBytes, 0, length); offset += length;
                            var record = new Record((RecordType)type, dataBytes);
                            records.Add(record);
                        }
                        Array.Clear(buffer, 0, buffer.Length);
                        return new Entry(records);
                    } catch (ArgumentException) { }
                }
            }
            return null;
        }

        public static bool HasDataOnClipboard {
            get {
                return Clipboard.ContainsData(FormatName);
            }
        }

    }
}
