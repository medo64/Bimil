using System;
using System.Drawing;
using System.Windows.Forms;

namespace Bimil {
    internal partial class PasswordForm : Form {

        private static readonly Font FixedFont = Settings.MonotypeFont;

        public PasswordForm(string extraTitle = null) {
            InitializeComponent();
            Font = SystemFonts.MessageBoxFont;
            txtPassword.Font = FixedFont;

            erp.SetIconAlignment(txtPassword, ErrorIconAlignment.MiddleLeft);
            erp.SetIconPadding(txtPassword, SystemInformation.Border3DSize.Width);

            if (extraTitle != null) { Text += " (" + extraTitle + ")"; }
        }


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


        private void chbShowPassword_CheckedChanged(object sender, EventArgs e) {
            txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;
        }

        private void btnOK_Click(object sender, EventArgs e) {
            Password = txtPassword.Text;
        }

        public string Password { get; private set; }

        private void tmrCapsLock_Tick(object sender, EventArgs e) {
            var isCapsLock = Control.IsKeyLocked(Keys.CapsLock);
            var wasCapsLock = !string.IsNullOrEmpty(erp.GetError(txtPassword));
            if (isCapsLock && !wasCapsLock) {
                erp.SetError(txtPassword, "CapsLock is on.");
            } else if (!isCapsLock && wasCapsLock) {
                erp.SetError(txtPassword, null);
            }
        }

    }
}
