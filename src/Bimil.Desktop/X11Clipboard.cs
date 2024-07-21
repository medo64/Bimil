/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2024-07-20: Initial version

namespace Medo.X11;

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

/// <summary>
/// X11 clipboard handling operations.
/// </summary>
public class X11Clipboard {

    private X11Clipboard(string clipboardAtomName) {
        ClipboardTag = clipboardAtomName[0..3];
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
            Debug.WriteLine($"[X11Clipboard:{ClipboardTag}] Only supported on Linux");
            return;
        }

        try {
            DisplayPtr = NativeMethods.XOpenDisplay(null);  // let's not worry about dispose
            if (DisplayPtr == IntPtr.Zero) {
                Debug.WriteLine($"[X11Clipboard:{ClipboardTag}] Failed to open display");
                return;
            }
            Debug.WriteLine($"[X11Clipboard:{ClipboardTag}] Display: 0x{DisplayPtr:X2}");
        } catch (DllNotFoundException) {
            Debug.WriteLine($"[X11Clipboard:{ClipboardTag}] X11 not available");
            return;
        }

        RootWindowPtr = NativeMethods.XDefaultRootWindow(DisplayPtr);
        if (RootWindowPtr == IntPtr.Zero) {
            Debug.WriteLine($"[X11Clipboard:{ClipboardTag}] Failed to open root window");
            return;
        }
        Debug.WriteLine($"[X11Clipboard:{ClipboardTag}] RootWindow: 0x{RootWindowPtr:X2}");

        WindowPtr = NativeMethods.XCreateSimpleWindow(DisplayPtr, RootWindowPtr, -10, -10, 1, 1, 0, 0, 0);
        if (WindowPtr == IntPtr.Zero) {
            Debug.WriteLine($"[X11Clipboard:{ClipboardTag}] Failed to open new window");
            return;
        }
        Debug.WriteLine($"[X11Clipboard:{ClipboardTag}] Window: 0x{WindowPtr:X2}");

        TargetsAtom = NativeMethods.XInternAtom(DisplayPtr, "TARGETS", only_if_exists: false);
        if (TargetsAtom == IntPtr.Zero) {
            Debug.WriteLine($"[X11Clipboard:{ClipboardTag}] Failed to open TARGETS atom");
            return;
        }
        Debug.WriteLine($"[X11Clipboard:{ClipboardTag}] Atom[TARGETS]: 0x{TargetsAtom:X2}");

        ClipboardAtom = NativeMethods.XInternAtom(DisplayPtr, clipboardAtomName, only_if_exists: false);
        if (ClipboardAtom == 0) {
            Debug.WriteLine($"[X11Clipboard:{ClipboardTag}] Failed to open {clipboardAtomName} atom");
            return;
        }
        Debug.WriteLine($"[X11Clipboard:{ClipboardTag}] Atom[{clipboardAtomName}]: 0x{ClipboardAtom:X2}");

        Utf8StringAtom = NativeMethods.XInternAtom(DisplayPtr, "UTF8_STRING", only_if_exists: false);
        if (Utf8StringAtom == 0) {
            Debug.WriteLine($"[X11Clipboard:{ClipboardTag}] Failed to open UTF8_STRING atom");
            return;
        }
        Debug.WriteLine($"[X11Clipboard:{ClipboardTag}] Atom[UTF8_STRING]: 0x{Utf8StringAtom:X2}");

        var metaSelectionAtomName = "MEDO_SELECTION_0x" + RandomNumberGenerator.GetHexString(16, lowercase: true);
        MetaSelectionAtom = NativeMethods.XInternAtom(DisplayPtr, metaSelectionAtomName, only_if_exists: false);
        if (MetaSelectionAtom == 0) {
            Debug.WriteLine($"[X11Clipboard:{ClipboardTag}] Failed to open {metaSelectionAtomName} atom");
            return;
        }
        Debug.WriteLine($"[X11Clipboard:{ClipboardTag}] Atom[{metaSelectionAtomName}]: 0x{MetaSelectionAtom:X2}");

        EventThread = new Thread(EventLoop) {  // last to initialize so we can use it as detection for successful init
            IsBackground = true,
            Name = clipboardAtomName,
        };
        EventThread.Start();
    }

    private readonly string ClipboardTag;
    private readonly IntPtr DisplayPtr;
    private readonly IntPtr RootWindowPtr;
    private readonly IntPtr WindowPtr;
    private readonly Int32 ClipboardAtom;
    private readonly Int32 TargetsAtom;
    private readonly Int32 Utf8StringAtom;
    private readonly Int32 MetaSelectionAtom;
    private readonly Thread? EventThread;

    private readonly object BytesOutLock = new();  // locked when BytesOut is accessed
    private byte[] BytesOut = [];
    private readonly AutoResetEvent BytesInLock = new(false);  // signaled when BytesIn is set
    private byte[] BytesIn = [];

    private void EventLoop() {
        Debug.WriteLine($"[X11Clipboard:{ClipboardTag}] Ready for events");

        while (true) {  // this is background thread thus it will stop when app stops
            try {
                NativeMethods.XEvent @event = new();
                NativeMethods.XNextEvent(DisplayPtr, ref @event);
                Debug.WriteLine($"[X11Clipboard:{ClipboardTag}] NextEvent: {@event.type}");

                switch (@event.type) {
                    case NativeMethods.XEventType.SelectionRequest: {
                            var requestEvent = @event.xselectionrequest;
                            if (NativeMethods.XGetSelectionOwner(DisplayPtr, requestEvent.selection) != WindowPtr) { continue; }  // not for us
                            if (requestEvent.selection != ClipboardAtom) { continue; }  // we ignore anything not clipboard
                            if (requestEvent.property == IntPtr.Zero) { continue; }  // we ignore empty propertty

                            if (requestEvent.target == TargetsAtom) {  // asking for formats
                                Debug.WriteLine($"[X11Clipboard:{ClipboardTag}]   Query for {NativeMethods.XGetAtomName(DisplayPtr, requestEvent.property.ToInt32())}");

                                NativeMethods.XChangeProperty(requestEvent.display,
                                                              requestEvent.requestor,
                                                              requestEvent.property,
                                                              4,   // XA_ATOM
                                                              32,  // 32-bit data
                                                              0,   // Replace
                                                              [Utf8StringAtom],
                                                              1);

                                var sendEvent = GetNewSelectionEventFromSelectionRequestEvent(@event, NativeMethods.XEventType.SelectionNotify, sendEvent: true);
                                var resSend = NativeMethods.XSendEvent(DisplayPtr,
                                                                       requestEvent.requestor,
                                                                       propagate: false,
                                                                       eventMask: IntPtr.Zero,
                                                                       ref sendEvent);
                                if (resSend == 0) { Debug.WriteLine($"[X11Clipboard:{ClipboardTag}]   Failed to send event"); }

                            } else if (requestEvent.target == Utf8StringAtom) {
                                Debug.WriteLine($"[X11Clipboard:{ClipboardTag}]   Request for {NativeMethods.XGetAtomName(DisplayPtr, requestEvent.property.ToInt32())}");

                                var bufferPtr = IntPtr.Zero;
                                int bufferLength;
                                try {
                                    lock (BytesOutLock) {
                                        bufferPtr = Marshal.AllocHGlobal(BytesOut.Length);
                                        bufferLength = BytesOut.Length;
                                        Marshal.Copy(BytesOut, 0, bufferPtr, BytesOut.Length);
                                    }

                                    NativeMethods.XChangeProperty(DisplayPtr,
                                                                  requestEvent.requestor,
                                                                  requestEvent.property,
                                                                  requestEvent.target,
                                                                  8,  // 8-bit data
                                                                  0,  // Replace
                                                                  bufferPtr,
                                                                  bufferLength);
                                } finally {
                                    if (bufferPtr != IntPtr.Zero) { Marshal.FreeHGlobal(bufferPtr); }
                                }

                                var sendEvent = GetNewSelectionEventFromSelectionRequestEvent(@event, NativeMethods.XEventType.SelectionNotify, sendEvent: true);
                                var resSend = NativeMethods.XSendEvent(DisplayPtr,
                                                                       requestEvent.requestor,
                                                                       propagate: false,
                                                                       eventMask: IntPtr.Zero,
                                                                       ref sendEvent);
                                if (resSend == 0) { Debug.WriteLine($"[X11Clipboard:{ClipboardTag}]   Failed to send event"); }
                            }
                        }
                        break;

                    case NativeMethods.XEventType.SelectionNotify: {
                            var selectionEvent = @event.xselection;
                            if (selectionEvent.target != Utf8StringAtom) { continue; }  // we ignore anything not clipboard

                            if (selectionEvent.property == 0) {
                                Debug.WriteLine($"[X11Clipboard:{ClipboardTag}]   Notification for empty");
                                BytesIn = [];
                                BytesInLock.Set();
                                continue;
                            }

                            Debug.WriteLine($"[X11Clipboard:{ClipboardTag}]   Notification for {NativeMethods.XGetAtomName(DisplayPtr, selectionEvent.property.ToInt32())}");

                            var data = IntPtr.Zero;
                            NativeMethods.XGetWindowProperty(DisplayPtr,
                                                             selectionEvent.requestor,
                                                             selectionEvent.property,
                                                             long_offset: 0,
                                                             long_length: int.MaxValue,
                                                             delete: false,
                                                             0,  // AnyPropertyType
                                                             out var type,
                                                             out var format,
                                                             out var nitems,
                                                             out var bytes_after,
                                                             ref data);
                            if (data != IntPtr.Zero) {
                                BytesIn = new byte[nitems.ToInt32()];
                                Marshal.Copy(data, BytesIn, 0, BytesIn.Length);
                                BytesInLock.Set();
                                NativeMethods.XFree(data);
                            } else {
                                Debug.WriteLine($"[X11Clipboard:{ClipboardTag}]   Cannot retrieve data");
                                BytesIn = [];
                                BytesInLock.Set();
                            }

                        }
                        break;
                }
            } catch (Exception ex) {
                Debug.WriteLine($"[X11Clipboard:{ClipboardTag}] Error: {ex.Message}");
            }
        }
    }

    private static NativeMethods.XEvent GetNewSelectionEventFromSelectionRequestEvent(NativeMethods.XEvent @event, NativeMethods.XEventType? type = null, bool? sendEvent = null) {
        var newEvent = new NativeMethods.XEvent();
        newEvent.xselection.type = (type != null) ? type.Value : @event.xselectionrequest.type;
        newEvent.xselection.serial = @event.xselectionrequest.serial;
        newEvent.xselection.send_event = (sendEvent != null) ? sendEvent.Value : @event.xselectionrequest.send_event;
        newEvent.xselection.display = @event.xselectionrequest.display;
        newEvent.xselection.requestor = @event.xselectionrequest.requestor;
        newEvent.xselection.selection = @event.xselectionrequest.selection;
        newEvent.xselection.target = @event.xselectionrequest.target;
        newEvent.xselection.property = @event.xselectionrequest.property;
        newEvent.xselection.time = @event.xselectionrequest.time;
        return newEvent;
    }


    private static readonly Lazy<X11Clipboard> _primary = new(() => new("PRIMARY"));
    /// <summary>
    /// Gets primary X11 clipboard.
    /// </summary>
    public static X11Clipboard Primary { get { return _primary.Value; } }

    private static readonly Lazy<X11Clipboard> _clipboard = new(() => new("CLIPBOARD"));
    /// <summary>
    /// Gets X11 clipboard.
    /// </summary>
    public static X11Clipboard Clipboard { get { return _clipboard.Value; } }


    /// <summary>
    /// Gets if clipboard service is available.
    /// </summary>
    public bool IsAvailable {
        get {
            return (EventThread != null);
        }
    }

    /// <summary>
    /// Clears the clipboard.
    /// </summary>
    public void Clear() {
        if (EventThread == null) { return; }  // something went wrong when initializing

        lock (BytesOutLock) {
            BytesOut = [];
        }
        NativeMethods.XSetSelectionOwner(DisplayPtr, ClipboardAtom, IntPtr.Zero, 0);
        Debug.WriteLine($"[X11Clipboard:{ClipboardTag}] Clear(): Ownership cleared");
    }

    /// <summary>
    /// Returns clipboard text data or Empty if the clipboard does not contain UTF-8 string.
    /// </summary>
    /// <param name="text">Text.</param>
    public string GetText() {
        if (EventThread == null) { return string.Empty; }  // something went wrong when initializing

        BytesInLock.Reset();  // shouldn't be set but let's make sure
        NativeMethods.XConvertSelection(DisplayPtr,
                                        ClipboardAtom,
                                        Utf8StringAtom,
                                        MetaSelectionAtom,
                                        WindowPtr,
                                        IntPtr.Zero);
        NativeMethods.XFlush(DisplayPtr);
        Debug.WriteLine($"[X11Clipboard:{ClipboardTag}] GetText(): Text requested");

        if (BytesInLock.WaitOne(100)) {  // don't wait long
            return Encoding.UTF8.GetString(BytesIn);
        } else {
            Debug.WriteLine($"[X11Clipboard:{ClipboardTag}] GetText(): Timeout reading text");
            return string.Empty;
        }
    }

    /// <summary>
    /// Clears the clipboard and then sets text data in UTF-8 format.
    /// </summary>
    /// <param name="text">Text.</param>
    public void SetText(string text) {
        if (EventThread == null) { return; }  // something went wrong when initializing

        lock (BytesOutLock) {
            BytesOut = Encoding.UTF8.GetBytes(text);
        }
        NativeMethods.XSetSelectionOwner(DisplayPtr, ClipboardAtom, WindowPtr, 0);
        Debug.WriteLine($"[X11Clipboard:{ClipboardTag}] SetText(): Ownership set");
    }



    private static class NativeMethods {  //https://www.x.org/releases/current/doc/libX11/libX11/libX11.html

        internal enum XEventType {
            KeyPress = 2,
            KeyRelease = 3,
            ButtonPress = 4,
            ButtonRelease = 5,
            MotionNotify = 6,
            EnterNotify = 7,
            LeaveNotify = 8,
            FocusIn = 9,
            FocusOut = 10,
            KeymapNotify = 11,
            Expose = 12,
            GraphicsExpose = 13,
            NoExpose = 14,
            VisibilityNotify = 15,
            CreateNotify = 16,
            DestroyNotify = 17,
            UnmapNotify = 18,
            MapNotify = 19,
            MapRequest = 20,
            ReparentNotify = 21,
            ConfigureNotify = 22,
            ConfigureRequest = 23,
            GravityNotify = 24,
            ResizeRequest = 25,
            CirculateNotify = 26,
            CirculateRequest = 27,
            PropertyNotify = 28,
            SelectionClear = 29,
            SelectionRequest = 30,
            SelectionNotify = 31,
            ColormapNotify = 32,
            ClientMessage = 33,
            MappingNotify = 34,
        }


        [StructLayout(LayoutKind.Sequential)]
        internal struct XSelectionClearEvent {
            internal XEventType type;
            internal IntPtr serial;
            internal bool send_event;
            internal IntPtr display;
            internal IntPtr window;
            internal IntPtr selection;
            internal IntPtr time;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct XSelectionEvent {
            internal XEventType type;
            internal IntPtr serial;
            internal bool send_event;
            internal IntPtr display;
            internal IntPtr requestor;
            internal IntPtr selection;
            internal IntPtr target;
            internal IntPtr property;
            internal IntPtr time;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct XSelectionRequestEvent {
            internal XEventType type;
            internal IntPtr serial;
            internal bool send_event;
            internal IntPtr display;
            internal IntPtr owner;
            internal IntPtr requestor;
            internal IntPtr selection;
            internal IntPtr target;
            internal IntPtr property;
            internal IntPtr time;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct XEventPad {
            internal IntPtr pad00;
            internal IntPtr pad01;
            internal IntPtr pad02;
            internal IntPtr pad03;
            internal IntPtr pad04;
            internal IntPtr pad05;
            internal IntPtr pad06;
            internal IntPtr pad07;
            internal IntPtr pad08;
            internal IntPtr pad09;
            internal IntPtr pad10;
            internal IntPtr pad11;
            internal IntPtr pad12;
            internal IntPtr pad13;
            internal IntPtr pad14;
            internal IntPtr pad15;
            internal IntPtr pad16;
            internal IntPtr pad17;
            internal IntPtr pad18;
            internal IntPtr pad19;
            internal IntPtr pad20;
            internal IntPtr pad21;
            internal IntPtr pad22;
            internal IntPtr pad23;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct XEvent {
            [FieldOffset(0)] internal XEventType type;
            [FieldOffset(0)] internal XSelectionClearEvent xselectionclear;
            [FieldOffset(0)] internal XSelectionRequestEvent xselectionrequest;
            [FieldOffset(0)] internal XSelectionEvent xselection;
            [FieldOffset(0)] internal XEventPad pad;
        }

#pragma warning disable SYSLIB1054

        [DllImport("libX11")]  // actually returns Int32 but we don't care
        public extern static void XChangeProperty(IntPtr display, IntPtr w, IntPtr property, IntPtr type, Int32 format, Int32 mode, IntPtr data, int nelements);

        [DllImport("libX11")]  // actually returns Int32 but we don't care
        public extern static void XChangeProperty(IntPtr display, IntPtr w, IntPtr property, UInt32 type, Int32 format, Int32 mode, Int32[] data, int nelements);

        [DllImport("libX11")]
        public extern static void XCloseDisplay(IntPtr display);

        [DllImport("libX11")]
        internal extern static void XConvertSelection(IntPtr display, IntPtr selection, IntPtr target, IntPtr property, IntPtr requestor, IntPtr time);

        [DllImport("libX11")]
        public extern static IntPtr XCreateSimpleWindow(IntPtr display, IntPtr parent, Int32 x, Int32 y, UInt32 width, UInt32 height, UInt32 border_width, nuint border, nuint background);

        [DllImport("libX11")]
        public extern static IntPtr XDefaultRootWindow(IntPtr display);

        [DllImport("libX11")]  // actually returns Int32 but we don't care
        public extern static void XFlush(IntPtr display);

        [DllImport("libX11")]
        public extern static void XFree(IntPtr data);

        [DllImport("libX11", BestFitMapping = false)]
        [return: MarshalAs(UnmanagedType.LPUTF8Str)]
        internal extern static String XGetAtomName(IntPtr display, Int32 atom);

        [DllImport("libX11")]
        public extern static IntPtr XGetSelectionOwner(IntPtr display, IntPtr selection);

        [DllImport("libX11")]  // actually returns Int32 but we don't care
        internal extern static void XGetWindowProperty(IntPtr display, IntPtr w, IntPtr property, IntPtr long_offset, IntPtr long_length, bool delete, IntPtr req_type, out IntPtr actual_type_return, out Int32 actual_format_return, out IntPtr nitems_return, out IntPtr bytes_after_return, ref IntPtr prop_return);

        [DllImport("libX11", BestFitMapping = false)]
        public extern static Int32 XInternAtom(IntPtr x11display, [MarshalAs(UnmanagedType.LPUTF8Str)] String atom_name, bool only_if_exists);

        [DllImport("libX11")]
        public extern static void XNextEvent(IntPtr display, ref XEvent event_return);

        [DllImport("libX11", BestFitMapping = false)]
        public extern static IntPtr XOpenDisplay([MarshalAs(UnmanagedType.LPUTF8Str)] String? display_name);

        [DllImport("libX11")]
        public extern static Int32 XSendEvent(IntPtr display, IntPtr window, bool propagate, IntPtr eventMask, ref XEvent sendEvent);

        [DllImport("libX11")]
        public extern static void XSetSelectionOwner(IntPtr display, IntPtr selection, IntPtr owner, UInt32 time);

#pragma warning restore SYSLIB1054

    }
}
