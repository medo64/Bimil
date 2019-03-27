using Medo.Security.Cryptography.PasswordSafe;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using System.Windows.Forms;

namespace Bimil {
    internal static class ClipboardHelper {

        #region Clipboard: Text

        public static void SetClipboardText(IWin32Window owner, string text, RecordType recordType) {
            SetClipboardText(owner, text, sensitiveData: Helpers.GetIsHideable(recordType));
        }

        public static void SetClipboardText(IWin32Window owner, string text, bool sensitiveData) {
            var data = new DataObject();
            if (text.Length > 0) {
                data.SetText(text, TextDataFormat.Text);
                data.SetText(text, TextDataFormat.UnicodeText);
            }

            PauseMonitor();

            try {
                Debug.WriteLine("Clipboard: SetData (text)");
                Clipboard.SetDataObject(data, false, 5, 100);
            } catch (ExternalException ex) {
                Medo.MessageBox.ShowError(owner, "Cannot copy to clipboard!\n\n" + ex.Message);
                return;
            }

            if (sensitiveData || (Settings.AutoClearClipboardForSensitiveDataOnly == false)) {
                StartMonitor();
                ClipboardClearThread.ScheduleClear();
            }
        }

        #endregion Clipboard: Text


        #region Clipboard: Object

        private static readonly string FormatName = "Bimil";

        public static void SetClipboardData(IWin32Window owner, IEnumerable<Entry> entries, bool sensitiveData) {
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

                var data = new DataObject();
                data.SetData(FormatName, protectedBuffer);

                PauseMonitor();

                try {
                    Debug.WriteLine("Clipboard: SetData (object)");
                    Clipboard.SetDataObject(data, false, 5, 100);
                } catch (ExternalException ex) {
                    Medo.MessageBox.ShowError(owner, "Cannot copy to clipboard!\n\n" + ex.Message);
                }

                if (sensitiveData || (Settings.AutoClearClipboardForSensitiveDataOnly == false)) {
                    StartMonitor();
                    ClipboardClearThread.ScheduleClear();
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
                            var type = BitConverter.ToInt32(buffer, offset);
                            offset += 4;
                            var length = BitConverter.ToInt32(buffer, offset);
                            offset += 4;
                            if ((type == 0) && (length == 0)) { //end of item
                                yield return new Entry(records);
                                records.Clear();
                                continue;
                            }

                            var dataBytes = new byte[length];
                            Buffer.BlockCopy(buffer, offset, dataBytes, 0, length);
                            offset += length;
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

        #endregion Clipboard: Object


        #region ClipboardMonitor

        private static ClipboardMonitor ClipboardMonitor;


        private static void StartMonitor() {
            if ((Settings.AutoClearClipboardTimeout == 0) || !Settings.AutoClearClipboardAfterPaste) { return; } //only clear if both clearing after paste and timeout are set.

            if (ClipboardMonitor == null) { ClipboardMonitor = new ClipboardMonitor(); }
            Debug.WriteLine("Clipboard: Monitor Start");
            ClipboardMonitor.ClipboardChanged += ClipboardMonitor_ClipboardChanged;
        }

        private static void PauseMonitor() {
            if (ClipboardMonitor != null) {
                Debug.WriteLine("Clipboard: Monitor Pause");
                ClipboardMonitor.ClipboardChanged -= ClipboardMonitor_ClipboardChanged;
            }
        }


        private static void ClipboardMonitor_ClipboardChanged(object sender, ClipboardChangedEventArgs e) {
            Debug.WriteLine("Clipboard: Monitor Changed");
            ClipboardClearThread.UnscheduleClear();

            if (ClipboardMonitor != null) {
                Debug.WriteLine("Clipboard: Monitor Pause (change)");
                ClipboardMonitor.ClipboardChanged -= ClipboardMonitor_ClipboardChanged;
            }
        }

        #endregion ClipboardMonitor


        #region Thread

        public static void Cancel() {
            PauseMonitor();

            Debug.WriteLine("Clipboard: Clear (cancel)");
            Clipboard.Clear();
            ClipboardClearThread.Terminate();
        }


        private static class ClipboardClearThread {

            private static readonly object SyncRoot = new object();
            private static Stopwatch ClearingStopatch = new Stopwatch();

            public static void ScheduleClear() {
                if (Settings.AutoClearClipboardTimeout == 0) { return; }

                Initialize();
                lock (SyncRoot) {
                    ClearingStopatch.Restart();
                }
            }

            public static void UnscheduleClear() {
                lock (SyncRoot) {
                    ClearingStopatch.Reset();
                }
            }


            private static Thread WaitingThread = null;

            public static void Initialize() {
                if (WaitingThread != null) { return; }

                WaitingThread = new Thread(new ThreadStart(Run)) {
                    Name = "Clipboard clearing thread",
                    IsBackground = true,
                };
                WaitingThread.Start();
            }

            public static void Terminate() {
                if (WaitingThread != null) {
                    WaitingThread.Abort();
                    WaitingThread = null;
                }
            }


            public static void Run() {
                try {
                    while (true) {
                        Thread.Sleep(1000);
                        if (Settings.AutoClearClipboardTimeout == 0) { continue; }

                        long elapsed;
                        lock (SyncRoot) {
                            elapsed = ClearingStopatch.ElapsedMilliseconds;
                        }

                        if (elapsed > Settings.AutoClearClipboardTimeout * 1000) {
                            lock (SyncRoot) {
                                ClearingStopatch.Reset();
                            }
                            if (!App.MainForm.IsDisposed) {
                                App.MainForm.Invoke((MethodInvoker)delegate () {
                                    PauseMonitor();

                                    Debug.WriteLine("Clipboard: Clear (timeout)");
                                    for (var i = 0; i < 3; i++) {
                                        try {
                                            Clipboard.Clear();
                                            break;
                                        } catch (ExternalException) {
                                            Thread.Sleep(500);
                                        }
                                    }
                                });
                            }
                        }
                    }
                } catch (ThreadAbortException) { };
            }
        }

        #endregion Thread

    }
}
