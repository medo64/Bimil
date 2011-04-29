using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Bimil {
    internal static class Tray {

        private static NotifyIcon Notify;

        internal static void Show(bool interactive = false) {
            Tray.Notify = new NotifyIcon();
            Tray.Notify.Icon = GetApplicationIcon();
            Tray.Notify.Text = "Bimil";
            Tray.Notify.Visible = true;

            Tray.Notify.DoubleClick += Menu_Show_OnClick;

            Tray.UpdateContextMenu();
        }

        internal static void UpdateContextMenu() {
            if (Tray.Notify == null) { return; }

            Tray.Notify.ContextMenuStrip = new ContextMenuStrip();
            Tray.Notify.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Show", null, Menu_Show_OnClick));
            Tray.Notify.ContextMenuStrip.Items[0].Font = new Font(Tray.Notify.ContextMenuStrip.Items[0].Font, FontStyle.Bold);
            Tray.Notify.ContextMenuStrip.Items.Add("-");
            Tray.Notify.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Exit", null, Menu_Exit_OnClick));
        }

        internal static void Hide() {
            if (Tray.Notify != null) {
                Tray.Notify.Visible = false;
            }
        }


        private static void Menu_Show_OnClick(object sender, EventArgs e) {
            if ((App.MainForm == null) || (App.MainForm.IsDisposed)) { App.MainForm = new MainForm(); }
            App.MainForm.Show();
            App.MainForm.Activate();
        }

        private static void Menu_Exit_OnClick(object sender, EventArgs e) {
            Application.Exit();
        }



        #region Helpers

        private static Icon GetApplicationIcon() {
            IntPtr hLibrary = NativeMethods.LoadLibrary(Assembly.GetEntryAssembly().Location);
            if (!hLibrary.Equals(IntPtr.Zero)) {
                IntPtr hIcon = NativeMethods.LoadImage(hLibrary, "#32512", NativeMethods.IMAGE_ICON, 20, 20, 0);
                if (!hIcon.Equals(System.IntPtr.Zero)) {
                    Icon icon = Icon.FromHandle(hIcon);
                    if (icon != null) { return icon; }
                }
            }
            return null;
        }

        private static class NativeMethods {

            public const UInt32 IMAGE_ICON = 1;


            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            static extern internal IntPtr LoadImage(IntPtr hInstance, String lpIconName, UInt32 uType, Int32 cxDesired, Int32 cyDesired, UInt32 fuLoad);

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
            static extern internal IntPtr LoadLibrary(string lpFileName);

        }

        #endregion

    }
}
