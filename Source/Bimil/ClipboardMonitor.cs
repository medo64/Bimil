//https://stackoverflow.com/questions/621577/clipboard-event-c-sharp/#1394225

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Bimil {

    [DefaultEvent("ClipboardChanged")]
    internal class ClipboardMonitor : Control {
        public ClipboardMonitor() {
            CreateHandle();
            try {
                NextViewerPtr = NativeMethods.SetClipboardViewer(Handle);
            } catch (EntryPointNotFoundException) { } //happens on Linux
        }

        ~ClipboardMonitor() {
            Dispose(disposing: false);
        }

        private IntPtr NextViewerPtr;


        /// <summary>
        /// Clipboard contents changed.
        /// </summary>
        public event EventHandler<ClipboardChangedEventArgs> ClipboardChanged;


        protected override void Dispose(bool disposing) {
            if (NextViewerPtr != IntPtr.Zero) {
                NativeMethods.ChangeClipboardChain(Handle, NextViewerPtr);
                NextViewerPtr = IntPtr.Zero;
            }
        }


        protected override void WndProc(ref Message m) {
            if (m != null) {
                switch (m.Msg) {
                    case NativeMethods.WM_DRAWCLIPBOARD:
                        Debug.WriteLine("ClipboardMonitor WndProc: WM_DRAWCLIPBOARD");
                        NativeMethods.SendMessage(NextViewerPtr, m.Msg, m.WParam, m.LParam);
                        OnClipboardChanged();
                        break;

                    case NativeMethods.WM_CHANGECBCHAIN:
                        Debug.WriteLine("ClipboardMonitor WndProc: WM_CHANGECBCHAIN");
                        if (m.WParam == NextViewerPtr) {
                            NextViewerPtr = m.LParam;
                        } else {
                            NativeMethods.SendMessage(NextViewerPtr, m.Msg, m.WParam, m.LParam);
                        }
                        break;

                    default:
                        Debug.WriteLine("ClipboardMonitor WndProc: 0x" + m.Msg.ToString("X4"));
                        base.WndProc(ref m);
                        break;
                }
            }
        }


        private void OnClipboardChanged() {
            try {
                var dataObject = Clipboard.GetDataObject();
                if (dataObject != null) {
                    ClipboardChanged?.Invoke(this, new ClipboardChangedEventArgs(dataObject));
                }
            } catch (ExternalException) { }
        }



        private static class NativeMethods {

            internal const int WM_DRAWCLIPBOARD = 0x0308;
            internal const int WM_CHANGECBCHAIN = 0x030D;


            [DllImport("User32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

            [DllImport("User32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

            [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern IntPtr SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        }
    }


    internal class ClipboardChangedEventArgs : EventArgs {
        public ClipboardChangedEventArgs(IDataObject dataObject) {
            DataObject = dataObject;
        }

        public IDataObject DataObject { get; }
    }
}
