using System;
using System.Drawing;
using System.Windows.Forms;

namespace Bimil {
    internal partial class QRCodeForm : Form {
        public QRCodeForm(string text) {
            InitializeComponent();
            Font = SystemFonts.MessageBoxFont;

            Coder = new QRCoder.QRCode(text);

            int qrSize;
            using (var bmp = Coder.GetBitmap()) {
                qrSize = Math.Max(bmp.Width, bmp.Height);
            }

            var screen = Screen.GetWorkingArea(this);
            var factor = Math.Min(screen.Width, screen.Height) / qrSize / 2;

            var scaledBitmap = Coder.GetBitmap(factor);
            ClientSize = scaledBitmap.Size;
            BackgroundImage = scaledBitmap;
        }

        private readonly QRCoder.QRCode Coder;


        #region Disable minimize

        protected override void WndProc(ref Message m) {
            if ((Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major < 10)
                && (m != null) && (m.Msg == NativeMethods.WM_SYSCOMMAND) && (m.WParam == NativeMethods.SC_MINIMIZE)) {
                m.Result = IntPtr.Zero;
            } else if (m != null) {
                base.WndProc(ref m);
            }
        }


        private class NativeMethods {
            internal const int WM_SYSCOMMAND = 0x0112;
            internal static readonly IntPtr SC_MINIMIZE = new IntPtr(0xF020);
        }

        #endregion


        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);
            if ((e.KeyData == Keys.Escape) || (e.KeyData == Keys.Return)) { DialogResult = DialogResult.Cancel; }
        }

    }
}
