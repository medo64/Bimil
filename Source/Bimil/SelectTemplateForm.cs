using System;
using System.Drawing;
using System.Windows.Forms;

namespace Bimil {
    internal partial class SelectTemplateForm : Form {
        public SelectTemplateForm() {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;

            cmbTemplate.Items.AddRange(Templates.GetTemplates());
            cmbTemplate.SelectedIndex = 0;
        }


        #region Disable minimize

        protected override void WndProc(ref Message m) {
            if ((Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major < 10)
                && (m.Msg == NativeMethods.WM_SYSCOMMAND) && (m.WParam == NativeMethods.SC_MINIMIZE)) {
                m.Result = IntPtr.Zero;
            } else {
                base.WndProc(ref m);
            }
        }


        private class NativeMethods {
            internal const Int32 WM_SYSCOMMAND = 0x0112;
            internal readonly static IntPtr SC_MINIMIZE = new IntPtr(0xF020);
        }

        #endregion


        private void btnOK_Click(object sender, EventArgs e) {
            this.Template = (Template)cmbTemplate.SelectedItem;
        }

        public Template Template { get; private set; }

    }
}
