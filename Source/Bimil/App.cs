using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Bimil {
    internal static class App {

        private static readonly Mutex SetupMutex = new Mutex(false, @"Global\JosipMedved_Bimil");

        [STAThread]
        internal static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");

            Medo.Application.UnhandledCatch.ThreadException += new EventHandler<ThreadExceptionEventArgs>(UnhandledCatch_ThreadException);
            Medo.Application.UnhandledCatch.Attach();


            Application.Run(new MainForm());

            SetupMutex.Close();
        }



        private static void UnhandledCatch_ThreadException(object sender, ThreadExceptionEventArgs e) {
#if !DEBUG
            Medo.Diagnostics.ErrorReport.ShowDialog(null, e.Exception, new Uri("http://jmedved.com/ErrorReport/"));
#else
            throw e.Exception;
#endif
        }

    }
}
