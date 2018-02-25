using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using Medo.Configuration;

namespace Bimil {
    internal static class App {

        private static readonly Mutex SetupMutex = new Mutex(false, @"Global\JosipMedved_Bimil");
        internal static RecentlyUsed Recent;
        internal static TimeSpan? LastNtpDrift;
        internal static MainForm MainForm;

        [STAThread]
        internal static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");

            Medo.Application.UnhandledCatch.ThreadException += new EventHandler<ThreadExceptionEventArgs>(UnhandledCatch_ThreadException);
            Medo.Application.UnhandledCatch.Attach();


            var recentLegacy = new RecentFiles();
            if (recentLegacy.Count > 0) {
                var fileList = new List<string>();
                foreach (var item in recentLegacy.Items) {
                    fileList.Add(item.FileName);
                }
                Recent = new RecentlyUsed(fileList);
                Config.Write("RecentFile", Recent.FileNames);
                recentLegacy.Clear();
            } else {
                Recent = new RecentlyUsed(Config.Read("RecentFile"));
            }
            Recent.Changed += (o, i) => {
                Config.Write("RecentFile", Recent.FileNames);
            };

            if (!Config.IsAssumedInstalled) {
                Medo.Windows.Forms.State.ReadState += delegate (object sender, Medo.Windows.Forms.StateReadEventArgs e) {
                    e.Value = Config.Read("State!" + e.Name.Replace("Bimil.", ""), e.DefaultValue);
                };
                Medo.Windows.Forms.State.WriteState += delegate (object sender, Medo.Windows.Forms.StateWriteEventArgs e) {
                    Config.Write("State!" + e.Name.Replace("Bimil.", ""), e.Value);
                };
            }

            App.MainForm = new MainForm();
            Application.Run(App.MainForm);

            SetupMutex.Close();
        }



        private static void UnhandledCatch_ThreadException(object sender, ThreadExceptionEventArgs e) {
#if !DEBUG
            Medo.Diagnostics.ErrorReport.ShowDialog(null, e.Exception, new Uri("https://medo64.com/feedback/"));
#else
            throw e.Exception;
#endif
        }

    }
}
