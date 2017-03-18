//Josip Medved <jmedved@jmedved.com>   www.medo64.com

//2012-11-24: Suppressing bogus CA5122 warning (http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical).
//2010-11-29: Initial version.


using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

namespace Medo.Windows.Forms {

    /// <summary>
    /// Simple input form.
    /// </summary>
    public sealed class InputBox : IDisposable {

        /// <summary>
        /// Create new instance.
        /// </summary>
        /// <param name="description">Short description.</param>
        public InputBox(string description) : this(description, null) { }

        /// <summary>
        /// Create new instance.
        /// </summary>
        /// <param name="description">Short description.</param>
        /// <param name="defaultText">Default text.</param>
        public InputBox(string description, string defaultText) {
            this.Description = description;
            this.DefaultText = defaultText;
        }


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        public void Dispose() { }


        /// <summary>
        /// Gets short description.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets default text.
        /// </summary>
        public string DefaultText { get; private set; }

        /// <summary>
        /// Gets selected text.
        /// </summary>
        public string Text { get; private set; }

        private TextBox txtValue;
        private Button btnOk;
        private Form frm;


        /// <summary>
        /// Opens a window and returns only when the newly opened window is closed.
        /// </summary>
        /// <param name="owner">Window that owns this window.</param>
        public DialogResult ShowDialog(IWin32Window owner) {
            using (frm = new Form()) {
                frm.AutoScaleMode = AutoScaleMode.None;
                frm.BackColor = SystemColors.Control;
                frm.ForeColor = SystemColors.ControlText;
                frm.Font = SystemFonts.MessageBoxFont;
                frm.FormBorderStyle = FormBorderStyle.FixedDialog;
                frm.MaximizeBox = false;
                frm.MinimizeBox = false;
                frm.Text = Localizations.Title;

                if (owner != null) {
                    this.frm.ShowInTaskbar = false;
                    Form formOwner = owner as Form;
                    if ((formOwner != null) && (formOwner.TopMost == true)) {
                        this.frm.TopMost = false;
                        this.frm.TopMost = true;
                    }
                    this.frm.StartPosition = FormStartPosition.CenterParent;
                } else {
                    this.frm.Icon = GetAppIcon(Assembly.GetEntryAssembly().Location);
                    this.frm.ShowInTaskbar = true;
                    this.frm.StartPosition = FormStartPosition.CenterScreen;
                }

                float zoomFactorX;
                float zoomFactorY;
                using (var g = frm.CreateGraphics()) {
                    zoomFactorX = g.DpiX / 96;
                    zoomFactorY = g.DpiY / 96;
                }

                frm.Width = (int)(400 * zoomFactorX);

                using (var panel = new Panel())
                using (var lblDescription = new Label())
                using (txtValue = new TextBox())
                using (btnOk = new Button())
                using (var btnCancel = new Button()) {
                    panel.AutoSize = false;
                    panel.BackColor = SystemColors.Window;
                    panel.Font = SystemFonts.MessageBoxFont;
                    panel.Location = new Point(0, 0);
                    panel.Width = frm.ClientSize.Width;
                    frm.Controls.Add(panel);

                    lblDescription.AutoEllipsis = true;
                    lblDescription.AutoSize = false;
                    lblDescription.BackColor = SystemColors.Window;
                    lblDescription.ForeColor = SystemColors.WindowText;
                    lblDescription.Location = new Point(11, 11);
                    lblDescription.Size = new Size(panel.Width - 11 - 11, (int)(lblDescription.PreferredSize.Height * zoomFactorY));
                    lblDescription.Text = this.Description;
                    panel.Controls.Add(lblDescription);

                    txtValue.Location = new Point(11, lblDescription.Bottom + 7);
                    txtValue.Text = this.DefaultText;
                    txtValue.Width = panel.Width - 11 - 11;
                    panel.Controls.Add(txtValue);

                    panel.Height = txtValue.Bottom + 11;

                    btnOk.BackColor = SystemColors.Control;
                    btnOk.ForeColor = SystemColors.ControlText;
                    btnOk.DialogResult = DialogResult.OK;
                    btnOk.Size = new Size((int)(75 * zoomFactorX), (int)(23 * zoomFactorY));
                    btnOk.Text = Localizations.Ok;

                    btnCancel.BackColor = SystemColors.Control;
                    btnCancel.ForeColor = SystemColors.ControlText;
                    btnCancel.DialogResult = DialogResult.Cancel;
                    btnCancel.Size = btnOk.Size;
                    btnCancel.Text = Localizations.Cancel;

                    btnCancel.Location = new Point(frm.ClientRectangle.Width - 11 - btnCancel.Width, panel.Bottom + 11);
                    btnOk.Location = new Point(btnCancel.Left - 7 - btnOk.Width, btnCancel.Top);
                    frm.Controls.Add(btnOk);
                    frm.Controls.Add(btnCancel);

                    frm.AcceptButton = btnOk;
                    frm.CancelButton = btnCancel;
                    frm.ClientSize = new Size(frm.ClientSize.Width, btnOk.Bottom + 11);

                    btnOk.Click += new EventHandler(btnOk_Click);
                    frm.Load += new EventHandler(frm_Load);

                    return this.frm.ShowDialog(owner);
                }
            }
        }

        void btnOk_Click(object sender, EventArgs e) {
            this.Text = this.txtValue.Text;
            this.frm.DialogResult = DialogResult.OK;
        }

        void frm_Load(object sender, EventArgs e) {
            this.txtValue.Text = this.DefaultText;
            this.txtValue.SelectAll();
            this.txtValue.Focus();
        }


        private static Icon GetAppIcon(string fileName) {
            if (!InputBox.IsRunningOnMono) {
                System.IntPtr hLibrary = NativeMethods.LoadLibrary(fileName);
                if (!hLibrary.Equals(System.IntPtr.Zero)) {
                    System.IntPtr hIcon = NativeMethods.LoadIcon(hLibrary, "#32512");
                    if (!hIcon.Equals(System.IntPtr.Zero)) {
                        var icon = System.Drawing.Icon.FromHandle(hIcon);
                        if (icon != null) { return icon; }
                    }
                }
            }
            return null;
        }

        private static bool IsRunningOnMono {
            get {
                return (Type.GetType("Mono.Runtime") != null);
            }
        }


        private static class NativeMethods {

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "Warning is bogus.")]
            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            static extern internal IntPtr LoadIcon(IntPtr hInstance, string lpIconName);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "Warning is bogus.")]
            [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
            static extern internal IntPtr LoadLibrary(string lpFileName);

        }

        private static class Localizations {

            internal static string Title {
                get {
                    if (Thread.CurrentThread.CurrentUICulture.Name.StartsWith("hr-", StringComparison.OrdinalIgnoreCase)) {
                        return "Unesite tekst";
                    } else {
                        return "Input text";
                    }
                }
            }

            internal static string Ok {
                get {
                    if (Thread.CurrentThread.CurrentUICulture.Name.StartsWith("hr-", StringComparison.OrdinalIgnoreCase)) {
                        return "U redu";
                    } else {
                        return "OK";
                    }
                }
            }

            internal static string Cancel {
                get {
                    if (Thread.CurrentThread.CurrentUICulture.Name.StartsWith("hr-", StringComparison.OrdinalIgnoreCase)) {
                        return "Odustani";
                    } else {
                        return "Cancel";
                    }
                }
            }

        }

    }

}
