//Copyright (c) 2007 Josip Medved <jmedved@jmedved.com>

//2007-12-29: New version.
//2008-01-03: Added Resources.
//2008-01-06: System.Environment.Exit returns E_ABORT (0x80004004).
//2008-01-08: Main method is now called Attach.
//2008-01-26: AutoExit parameter changed to NoAutoExit
//2008-04-10: NewInstanceEventArgs is not nested class anymore.
//2008-04-11: Cleaned code to match FxCop 1.36 beta 2 (SpecifyMarshalingForPInvokeStringArguments, NestedTypesShouldNotBeVisible).
//2008-11-14: Reworked code to use SafeHandle.
//2010-10-07: Added IsOtherInstanceRunning method.


using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Medo.Application {

    /// <summary>
    /// Handles detection and communication of programs multiple instances.
    /// This class is thread safe.
    /// </summary>
    public static class SingleInstance {

        private static Mutex _mtxFirstInstance;
        private static Thread _thread;
        private static readonly object _syncRoot = new object();



        /// <summary>
        /// Returns true if this application is not already started.
        /// Another instance is contacted via named pipe.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">API call failed.</exception>
        public static bool Attach() {
            return Attach(false);
        }

        /// <summary>
        /// Returns true if this application is not already started.
        /// Another instance is contacted via named pipe.
        /// </summary>
        /// <param name="noAutoExit">If true, application will exit after informing another instance.</param>
        /// <exception cref="System.InvalidOperationException">API call failed.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Needs to be cought all in order not to break in any case.")]
        public static bool Attach(bool noAutoExit) {
            lock (_syncRoot) {
                NativeMethods.FileSafeHandle handle = null;
                bool isFirstInstance = false;
                try {
                    _mtxFirstInstance = new Mutex(true, MutexName, out isFirstInstance);
                    if (isFirstInstance == false) { //we need to contact previous instance.
                        _mtxFirstInstance = null;

                        byte[] buffer;
                        using (System.IO.MemoryStream ms = new System.IO.MemoryStream()) {
                            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                            bf.Serialize(ms, new NewInstanceEventArgs(System.Environment.CommandLine, System.Environment.GetCommandLineArgs()));
                            ms.Flush();
                            buffer = ms.GetBuffer();
                        }

                        //open pipe
                        if (!NativeMethods.WaitNamedPipe(NamedPipeName, NativeMethods.NMPWAIT_USE_DEFAULT_WAIT)) { throw new System.InvalidOperationException(Resources.ExceptionWaitNamedPipeFailed); }
                        handle = NativeMethods.CreateFile(NamedPipeName, NativeMethods.GENERIC_READ | NativeMethods.GENERIC_WRITE, 0, System.IntPtr.Zero, NativeMethods.OPEN_EXISTING, NativeMethods.FILE_ATTRIBUTE_NORMAL, System.IntPtr.Zero);
                        if (handle.IsInvalid) {
                            throw new System.InvalidOperationException(Resources.ExceptionCreateFileFailed);
                        }

                        //send bytes
                        uint written = 0;
                        NativeOverlapped overlapped = new NativeOverlapped();
                        if (!NativeMethods.WriteFile(handle, buffer, (uint)buffer.Length, ref written, ref overlapped)) {
                            throw new System.InvalidOperationException(Resources.ExceptionWriteFileFailed);
                        }
                        if (written != buffer.Length) { throw new System.InvalidOperationException(Resources.ExceptionWriteFileWroteUnexpectedNumberOfBytes); }

                    } else {  //there is no application already running.

                        _thread = new Thread(Run);
                        _thread.Name = "Medo.Application.SingleInstance.0";
                        _thread.IsBackground = true;
                        _thread.Start();

                    }

                } catch (System.Exception ex) {
                    System.Diagnostics.Trace.TraceWarning(ex.Message + "  {Medo.Application.SingleInstance}");

                } finally {
                    //if (handle != null && (!(handle.IsClosed || handle.IsInvalid))) {
                    //    handle.Close();
                    //}
                    if (handle != null) {
                        handle.Dispose();
                    }
                }

                if ((isFirstInstance == false) && (noAutoExit == false)) {
                    System.Diagnostics.Trace.TraceInformation("Exit(E_ABORT): Another instance is running.  {Medo.Application.SingleInstance}");
                    System.Environment.Exit(unchecked((int)0x80004004)); //E_ABORT(0x80004004)
                }

                return isFirstInstance;
            }
        }

        private static string _mutexName;
        private static string MutexName {
            get {
                lock (_syncRoot) {
                    if (_mutexName == null) {
                        System.Text.StringBuilder sbComponents = new System.Text.StringBuilder();
                        sbComponents.AppendLine(System.Environment.MachineName);
                        sbComponents.AppendLine(System.Environment.UserName);
                        sbComponents.AppendLine(System.Reflection.Assembly.GetEntryAssembly().FullName);
                        sbComponents.AppendLine(System.Reflection.Assembly.GetEntryAssembly().CodeBase);
                        
                        byte[] hash;
                        using (var sha1 =System.Security.Cryptography.SHA1Managed.Create()){
                            hash = sha1.ComputeHash(System.Text.Encoding.Unicode.GetBytes(sbComponents.ToString()));
                        }

                        System.Text.StringBuilder sbFinal = new System.Text.StringBuilder();
                        string assName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
                        sbFinal.Append(assName, 0, System.Math.Min(assName.Length, 64));
                        sbFinal.Append('.');
                        for (int i = 0; i < hash.Length; ++i) {
                            sbFinal.AppendFormat("{0:X2}", hash[i]);
                        }
                        _mutexName = sbFinal.ToString();
                    }
                    return _mutexName;
                }
            }
        }
        private static string NamedPipeName = @"\\.\pipe\" + MutexName;

        /// <summary>
        /// Gets whether there is another instance running.
        /// It temporary creates mutex.
        /// </summary>
        public static bool IsOtherInstanceRunning {
            get {
                lock (_syncRoot) {
                    if (_mtxFirstInstance != null) {
                        return false; //no other instance is running
                    } else {
                        bool isFirstInstance = false;
                        var tempInstance = new Mutex(true, MutexName, out isFirstInstance);
                        tempInstance.Close();
                        return (isFirstInstance == false);
                    }
                }
            }
        }

        /// <summary>
        /// Occurs in first instance when new instance is detected.
        /// </summary>
        public static event System.EventHandler<NewInstanceEventArgs> NewInstanceDetected;


        /// <summary>
        /// Thread function.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Needs to be cought all in order not to break in any case.")]
        private static void Run() {
            while (_mtxFirstInstance != null) {
                IntPtr handle = IntPtr.Zero;
                try {
                    handle = NativeMethods.CreateNamedPipe(NamedPipeName, NativeMethods.PIPE_ACCESS_DUPLEX, NativeMethods.PIPE_TYPE_BYTE | NativeMethods.PIPE_READMODE_BYTE | NativeMethods.PIPE_WAIT, NativeMethods.PIPE_UNLIMITED_INSTANCES, 4096, 4096, NativeMethods.NMPWAIT_USE_DEFAULT_WAIT, System.IntPtr.Zero);
                    if (handle.Equals(IntPtr.Zero)) { throw new System.InvalidOperationException(Resources.ExceptionCreateNamedPipeFailed); }
                    bool connected = NativeMethods.ConnectNamedPipe(handle, System.IntPtr.Zero);
                    if (!connected) { throw new System.InvalidOperationException(Resources.ExceptionConnectNamedPipeFailed); }

                    uint available = 0;
                    while (available == 0) {
                        uint bytesRead = 0, thismsg = 0;
                        if (!NativeMethods.PeekNamedPipe(handle, null, 0, ref bytesRead, ref available, ref thismsg)) {
                            Thread.Sleep(100);
                            available = 0;
                        }
                    }
                    byte[] buffer = new byte[available];
                    uint read = 0;
                    NativeOverlapped overlapped = new NativeOverlapped();
                    if (!NativeMethods.ReadFile(handle, buffer, (uint)buffer.Length, ref read, ref overlapped)) {
                        throw new System.InvalidOperationException(Resources.ExceptionReadFileFailed);
                    }
                    if (read != available) {
                        throw new System.InvalidOperationException(Resources.ExceptionReadFileReturnedUnexpectedNumberOfBytes);
                    }

                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer)) {
                        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        if (NewInstanceDetected != null) { NewInstanceDetected(null, (NewInstanceEventArgs)bf.Deserialize(ms)); }
                    }

                } catch (System.Exception ex) {
                    System.Diagnostics.Trace.TraceWarning(ex.Message + "  {Medo.Application.SingleInstance");
                    Thread.Sleep(1000);
                } finally { //closing native resources.
                    if (!handle.Equals(System.IntPtr.Zero)) {
                        NativeMethods.CloseHandle(handle);
                    }
                }//try
            }//while
        }

        private static class Resources {

            internal static string ExceptionWaitNamedPipeFailed { get { return "WaitNamedPipe failed."; } }

            internal static string ExceptionCreateFileFailed { get { return "CreateFile failed."; } }

            internal static string ExceptionWriteFileFailed { get { return "WriteFile failed."; } }

            internal static string ExceptionWriteFileWroteUnexpectedNumberOfBytes { get { return "WriteFile wrote unexpected number of bytes."; } }

            internal static string ExceptionCreateNamedPipeFailed { get { return "CreateNamedPipe failed."; } }

            internal static string ExceptionConnectNamedPipeFailed { get { return "ConnectNamedPipe failed."; } }

            internal static string ExceptionReadFileFailed { get { return "ReadFile failed."; } }

            internal static string ExceptionReadFileReturnedUnexpectedNumberOfBytes { get { return "ReadFile returned unexpected number of bytes."; } }

        }

        private static class NativeMethods {

            public const uint FILE_ATTRIBUTE_NORMAL = 0;
            public const uint GENERIC_READ = 0x80000000;
            public const uint GENERIC_WRITE = 0x40000000;
            public const int INVALID_HANDLE_VALUE = -1;
            public const uint NMPWAIT_USE_DEFAULT_WAIT = 0x00000000;
            public const uint OPEN_EXISTING = 3;
            public const uint PIPE_ACCESS_DUPLEX = 0x00000003;
            public const uint PIPE_READMODE_BYTE = 0x00000000;
            public const uint PIPE_TYPE_BYTE = 0x00000000;
            public const uint PIPE_UNLIMITED_INSTANCES = 255;
            public const uint PIPE_WAIT = 0x00000000;


            public class FileSafeHandle : SafeHandle {
                private static IntPtr minusOne = new IntPtr(-1);


                public FileSafeHandle()
                    : base(minusOne, true) { }


                public override bool IsInvalid {
                    get { return (this.IsClosed) || (base.handle == minusOne); }
                }

                protected override bool ReleaseHandle() {
                    return CloseHandle(this.handle);
                }

                public override string ToString() {
                    return this.handle.ToString();
                }

            }


            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CloseHandle(System.IntPtr hObject);

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool ConnectNamedPipe(System.IntPtr hNamedPipe, System.IntPtr lpOverlapped);

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern FileSafeHandle CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, System.IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, System.IntPtr hTemplateFile);

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern System.IntPtr CreateNamedPipe(string lpName, uint dwOpenMode, uint dwPipeMode, uint nMaxInstances, uint nOutBufferSize, uint nInBufferSize, uint nDefaultTimeOut, System.IntPtr lpSecurityAttributes);

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool PeekNamedPipe(System.IntPtr hNamedPipe, byte[] lpBuffer, uint nBufferSize, ref uint lpBytesRead, ref uint lpTotalBytesAvail, ref uint lpBytesLeftThisMessage);

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool ReadFile(System.IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToRead, ref uint lpNumberOfBytesRead, ref NativeOverlapped lpOverlapped);

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool WriteFile(FileSafeHandle hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite, ref uint lpNumberOfBytesWritten, ref NativeOverlapped lpOverlapped);

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool WaitNamedPipe(string lpNamedPipeName, uint nTimeOut);

        }

    }

}


namespace Medo.Application {

    /// <summary>
    /// Arguments for newly detected application instance.
    /// </summary>
    [System.Serializable()]
    public class NewInstanceEventArgs : System.EventArgs {
        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="commandLine">Command line.</param>
        /// <param name="commandLineArgs">String array containing the command line arguments.</param>
        public NewInstanceEventArgs(string commandLine, string[] commandLineArgs) {
            this._commandLine = commandLine;
            this._commandLineArgs = commandLineArgs;
        }

        private string _commandLine;
        /// <summary>
        /// Gets the command line.
        /// </summary>
        public string CommandLine {
            get { return this._commandLine; }
        }

        private string[] _commandLineArgs;
        /// <summary>
        /// Returns a string array containing the command line arguments.
        /// </summary>
        public string[] GetCommandLineArgs() {
            return this._commandLineArgs;
        }

    }

}
