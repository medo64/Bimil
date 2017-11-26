using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Windows.Forms;
using Medo.Security.Cryptography.PasswordSafe;

namespace Bimil {
    internal static class ClipboardHelper {

        #region Text

        public static void SetClipboardText(IWin32Window owner, string text) {
            try {
                var data = new DataObject();
                if (text.Length > 0) {
                    data.SetText(text, TextDataFormat.UnicodeText);
                }
                Clipboard.SetDataObject(data, false, 5, 100);
            } catch (ExternalException ex) {
                Medo.MessageBox.ShowError(owner, "Cannot copy to clipboard!\n\n" + ex.Message);
            }
        }

        #endregion Text

        #region Custom

        private static readonly string FormatName = "Bimil";

        public static void SetClipboardData(IWin32Window owner, IEnumerable<Entry> entries) {
            var bytes = new List<byte>();
            var buffer = new byte[0];
            try {
                foreach (var entry in entries) {
                    foreach (var record in entry.Records) {
                        var dataBytes = record.GetBytesSilently(); //to avoid modifications to access time
                        try {
                            bytes.AddRange(BitConverter.GetBytes((int)record.RecordType));
                            bytes.AddRange(BitConverter.GetBytes(dataBytes.Length));
                            bytes.AddRange(dataBytes);
                        } finally {
                            Array.Clear(dataBytes, 0, dataBytes.Length);
                        }
                    }

                    //add entry separator
                    bytes.AddRange(new byte[] { 0, 0, 0, 0 }); //RecordType=0
                    bytes.AddRange(new byte[] { 0, 0, 0, 0 }); //Length=0
                }
                buffer = bytes.ToArray();

                var protectedBuffer = ProtectedData.Protect(buffer, null, DataProtectionScope.CurrentUser);

                try {
                    var data = new DataObject();
                    data.SetData(FormatName, protectedBuffer);
                    Clipboard.SetDataObject(data, false, 5, 100);
                } catch (ExternalException ex) {
                    Medo.MessageBox.ShowError(owner, "Cannot copy to clipboard!\n\n" + ex.Message);
                }
            } finally {
                for (var i = 0; i < bytes.Count; i++) { bytes[i] = 0; }
                Array.Clear(buffer, 0, buffer.Length);
            }
        }

        public static IEnumerable<Entry> GetClipboardData() {
            if (Clipboard.ContainsData(FormatName)) {
                if (Clipboard.GetData(FormatName) is byte[] protectedBuffer) {
                    var buffer = ProtectedData.Unprotect(protectedBuffer, null, DataProtectionScope.CurrentUser);
                    var offset = 0;
                    var records = new List<Record>();
                    try {
                        while (offset < buffer.Length) {
                            var type = BitConverter.ToInt32(buffer, offset); offset += 4;
                            var length = BitConverter.ToInt32(buffer, offset); offset += 4;
                            if ((type == 0) && (length == 0)) { //end of item
                                yield return new Entry(records);
                                records.Clear();
                                continue;
                            }

                            var dataBytes = new byte[length];
                            Buffer.BlockCopy(buffer, offset, dataBytes, 0, length); offset += length;
                            var record = new Record((RecordType)type, dataBytes);
                            records.Add(record);
                        }
                        if (records.Count > 0) { //return any records left (compatibility with old single-item copy)
                            yield return new Entry(records);
                            records.Clear();
                        }
                    } finally {
                        Array.Clear(buffer, 0, buffer.Length);
                    }
                }
            }
        }

        public static bool HasDataOnClipboard {
            get {
                return Clipboard.ContainsData(FormatName);
            }
        }

        #endregion Custom
    }
}
