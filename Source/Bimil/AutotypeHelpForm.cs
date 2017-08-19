using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Medo.Security.Cryptography.PasswordSafe;

namespace Bimil {
    internal partial class AutotypeHelpForm : Form {
        public AutotypeHelpForm(string autotypeText, bool isReadOnly) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;
            Medo.Windows.Forms.State.SetupOnLoadAndClose(this);

            if (string.IsNullOrEmpty(autotypeText)) {
                txtAutotype.Text = @"\u\t\p\n";
            } else {
                txtAutotype.Text = autotypeText;
            }

            if (isReadOnly) {
                lsvHelp.BackColor = SystemColors.Control;
                txtAutotype.ReadOnly = isReadOnly;
                txtAutotype.Width += btnOK.Width + (btnCancel.Left - btnOK.Right);
                btnOK.Visible = false;
            }
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


        private void Form_Load(object sender, System.EventArgs e) {
            AddItem("Fields", @"\u", "User name");
            AddItem("Fields", @"\p", "Password");
            AddItem("Fields", @"\2", "Two-factor authentication code");
            AddItem("Fields", @"\cn", "Credit card number");
            AddItem("Fields", @"\ct", "Credit card number (tab separated)");
            AddItem("Fields", @"\ce", "Credit card expiration");
            AddItem("Fields", @"\cv", "Credit card security code");
            AddItem("Fields", @"\cp", "Credit card pin number");
            AddItem("Fields", @"\g", "Group field");
            AddItem("Fields", @"\i", "Title field");
            AddItem("Fields", @"\l", "URL");
            AddItem("Fields", @"\m", "E-mail");
            AddItem("Fields", @"\o", "Notes");
            AddItem("Fields", @"\o###", "Nth line of Notes field (if line doesn't exist, it has no effect)");

            AddItem("Keys", @"\b", "Backpace");
            AddItem("Keys", @"\t", "Tab");
            AddItem("Keys", @"\s", "Shift+Tab");
            AddItem("Keys", @"\n", "Enter");
            AddItem("Keys", @"\\", "Backslash (\\)");

            AddItem("Time", @"\d###", "Delay between characters in milliseconds, instead of 10 (default)");
            AddItem("Time", @"\w###", "Wait in milliseconds");
            AddItem("Time", @"\W###", "Wait in seconds");
        }

        private void Form_Resize(object sender, System.EventArgs e) {
            lsvHelp_colHelp.Width = lsvHelp.ClientRectangle.Width - lsvHelp_colEscape.Width - SystemInformation.VerticalScrollBarWidth;
        }

        private void Form_Shown(object sender, System.EventArgs e) {
            txtAutotype.SelectionStart = txtAutotype.Text.Length;
        }


        public string AutotypeText { get; private set; }

        private void btnOK_Click(object sender, System.EventArgs e) {
            this.AutotypeText = txtAutotype.Text;
        }


        private void lsvHelp_ItemActivate(object sender, System.EventArgs e) {
            if (!txtAutotype.ReadOnly && (lsvHelp.SelectedItems.Count == 1)) {
                txtAutotype.SelectedText = lsvHelp.SelectedItems[0].Text;
            }
        }


        private Dictionary<string, ListViewGroup> Groups = new Dictionary<string, ListViewGroup>();

        private void AddItem(string group, string escapeSequence, string helpText) {
            var lvi = new ListViewItem(escapeSequence);
            lvi.SubItems.Add(helpText);

            if (this.Groups.ContainsKey(group)) {
                lvi.Group = this.Groups[group];
            } else {
                lvi.Group = new ListViewGroup(group);
                this.Groups.Add(group, lvi.Group);
                lsvHelp.Groups.Add(lvi.Group);
            }

            lsvHelp.Items.Add(lvi);
        }
    }
}
