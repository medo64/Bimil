//https://stackoverflow.com/questions/621577/clipboard-event-c-sharp/#1394225

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Bimil {

    [DefaultEvent("ClipboardChanged")]
    internal class ClipboardMonitor : Control {
        public ClipboardMonitor() {
            this.CreateHandle();
            try {
                this.NextViewerPtr = NativeMethods.SetClipboardViewer(this.Handle);
            } catch (EntryPointNotFoundException) { } //happens on Linux
        }

        ~ClipboardMonitor() {
            this.Dispose(disposing: false);
        }


        IntPtr NextViewerPtr;


        /// <summary>
        /// Clipboard contents changed.
        /// </summary>
        public event EventHandler<ClipboardChangedEventArgs> ClipboardChanged;


        protected override void Dispose(bool disposing) {
            if (this.NextViewerPtr != IntPtr.Zero) {
                NativeMethods.ChangeClipboardChain(this.Handle, NextViewerPtr);
                this.NextViewerPtr = IntPtr.Zero;
            }
        }


        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case NativeMethods.WM_DRAWCLIPBOARD:
                    NativeMethods.SendMessage(NextViewerPtr, m.Msg, m.WParam, m.LParam);
                    OnClipboardChanged();
                    break;

                case NativeMethods.WM_CHANGECBCHAIN:
                    if (m.WParam == this.NextViewerPtr) {
                        this.NextViewerPtr = m.LParam;
                    } else {
                        NativeMethods.SendMessage(this.NextViewerPtr, m.Msg, m.WParam, m.LParam);
                    }
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }


        private void OnClipboardChanged() {
            var dataObject = Clipboard.GetDataObject();
            if (dataObject != null) {
                ClipboardChanged?.Invoke(this, new ClipboardChangedEventArgs(dataObject));
            }
        }



        private static class NativeMethods {

            internal const Int32 WM_DRAWCLIPBOARD = 0x0308;
            internal const Int32 WM_CHANGECBCHAIN = 0x030D;


            [DllImport("User32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

            [DllImport("User32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

            [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern IntPtr SendMessage(IntPtr hwnd, Int32 wMsg, IntPtr wParam, IntPtr lParam);

        }
    }


    internal class ClipboardChangedEventArgs : EventArgs {
        public ClipboardChangedEventArgs(IDataObject dataObject) {
            this.DataObject = dataObject;
        }

        public IDataObject DataObject { get; }
    }
}
