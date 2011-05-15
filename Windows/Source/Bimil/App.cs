using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Bimil {
    internal static class App {

        private static readonly Mutex SetupMutex = new Mutex(false, @"Global\JosipMedved_Bimil");
        public static MainForm MainForm = null;
        public static Form ThreadProxyForm = null;

        [STAThread]
        internal static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");

            Medo.Application.UnhandledCatch.ThreadException += new EventHandler<ThreadExceptionEventArgs>(UnhandledCatch_ThreadException);
            Medo.Application.UnhandledCatch.Attach();

            Medo.Application.SingleInstance.NewInstanceDetected += new EventHandler<Medo.Application.NewInstanceEventArgs>(SingleInstance_NewInstanceDetected);
            if (Medo.Application.SingleInstance.IsOtherInstanceRunning) {
                var currProcess = Process.GetCurrentProcess();
                foreach (var iProcess in Process.GetProcessesByName(currProcess.ProcessName)) {
                    try {
                        if (iProcess.Id != currProcess.Id) {
                            NativeMethods.AllowSetForegroundWindow(iProcess.Id);
                            break;
                        }
                    } catch (Win32Exception) { }
                }
            }
            Medo.Application.SingleInstance.Attach();

            App.MainForm = new MainForm();
            Application.Run(App.MainForm);

            SetupMutex.Close();
        }



        private static void UnhandledCatch_ThreadException(object sender, ThreadExceptionEventArgs e) {
#if !DEBUG
            Medo.Diagnostics.ErrorReport.ShowDialog(null, e.Exception, new Uri("http://jmedved.com/ErrorReport/"));
#else
            throw e.Exception;
#endif
        }


        private static void SingleInstance_NewInstanceDetected(object sender, Medo.Application.NewInstanceEventArgs e) {
            try {
                NewInstanceDetectedProcDelegate method = new NewInstanceDetectedProcDelegate(NewInstanceDetectedProc);
                App.ThreadProxyForm.Invoke(method);
            } catch (Exception) { }
        }

        private delegate void NewInstanceDetectedProcDelegate();

        private static void NewInstanceDetectedProc() {
            if (App.MainForm == null) { App.MainForm = new MainForm(); }
            if (App.MainForm.IsHandleCreated == false) {
                App.MainForm.CreateControl();
                App.MainForm.Handle.GetType();
            }

            App.MainForm.Show();
            if (App.MainForm.WindowState == FormWindowState.Minimized) { App.MainForm.WindowState = FormWindowState.Normal; }
            App.MainForm.Activate();
        }



        private static class NativeMethods {

            [DllImportAttribute("user32.dll", EntryPoint = "AllowSetForegroundWindow")]
            [return: MarshalAsAttribute(UnmanagedType.Bool)]
            public static extern bool AllowSetForegroundWindow(int dwProcessId);

        }

    }
}
