using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Bimil.Desktop;

internal class X11Clipboard {

    private X11Clipboard(string clipboardAtomName) {
        ClipboardAtomName = clipboardAtomName;
    }

    private readonly string ClipboardAtomName;


    private static readonly Lazy<X11Clipboard> _primary = new(() => new("PRIMARY"));
    public static X11Clipboard Primary { get { return _primary.Value; } }

    private static readonly Lazy<X11Clipboard> _clipboard = new(() => new("CLIPBOARD"));
    public static X11Clipboard Clipboard { get { return _clipboard.Value; } }


    public void Clear() {
        var displayPtr = NativeMethods.XOpenDisplay(null);
        if (displayPtr == IntPtr.Zero) { Debug.WriteLine("[X11Clipboard] Failed to open display"); return; }
        Debug.WriteLine("[X11Clipboard] Display: 0x" + displayPtr.ToString("X2"));

        try {
            var clipboardAtomPtr = NativeMethods.XInternAtom(displayPtr, ClipboardAtomName, onlyIfExists: false);
            if (clipboardAtomPtr == IntPtr.Zero) { Debug.WriteLine("[X11Clipboard] Failed to find " + ClipboardAtomName + " atom"); return; }
            Debug.WriteLine("[X11Clipboard] " + ClipboardAtomName + ": 0x" + clipboardAtomPtr.ToString("X2"));

            Debug.WriteLine("[X11Clipboard] Clearing " + ClipboardAtomName + " clipboard");
            NativeMethods.XSetSelectionOwner(displayPtr, clipboardAtomPtr, IntPtr.Zero, IntPtr.Zero);  // might not work on wayland
        } finally {
            NativeMethods.XCloseDisplay(displayPtr);
        }
    }


    private static class NativeMethods {

        [DllImport("libX11")]
        public extern static IntPtr XOpenDisplay(String? displayName);

        [DllImport("libX11")]
        public extern static void XCloseDisplay(IntPtr x11display);

        [DllImport("libX11")]
        public extern static IntPtr XInternAtom(IntPtr x11display, String atomName, bool onlyIfExists);

        [DllImport("libX11")]
        public extern static void XSetSelectionOwner(IntPtr display, IntPtr selection, IntPtr owner, IntPtr time);

    }
}
