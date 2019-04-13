/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2007-12-31: New version.
//2008-01-03: Added Resources.
//2008-04-11: Cleaned code to match FxCop 1.36 beta 2 (SpecifyMarshalingForPInvokeStringArguments, NormalizeStringsToUppercase).
//2008-11-14: Reworked code to use SafeHandle.
//            Fixed ToInt32 call on x64 bit windows.
//2008-12-01: Deleted methods without owner parameter.
//2009-07-04: Compatibility with Mono 2.4.
//2012-11-24: Suppressing bogus CA5122 warning (http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical).


using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Medo {

    /// <summary>
    /// Displays a message box that can contain text, buttons, and symbols that inform and instruct the user.
    /// </summary>
    public static class MessageBox {

        private readonly static object _syncRoot = new object();


        #region With owner

        /// <summary>
        /// Displays a message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        public static DialogResult ShowDialog(IWin32Window owner, string text) {
            lock (_syncRoot) {
                return ShowDialog(owner, text, Resources.DefaultCaption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        public static DialogResult ShowDialog(IWin32Window owner, string text, MessageBoxButtons buttons) {
            lock (_syncRoot) {
                return ShowDialog(owner, text, Resources.DefaultCaption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="icon">One of the MessageBoxIcon values that specifies which icon to display in the message box.</param>
        public static DialogResult ShowDialog(IWin32Window owner, string text, MessageBoxButtons buttons, MessageBoxIcon icon) {
            lock (_syncRoot) {
                return ShowDialog(owner, text, Resources.DefaultCaption, buttons, icon, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="icon">One of the MessageBoxIcon values that specifies which icon to display in the message box.</param>
        /// <param name="defaultButton">One of the MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        public static DialogResult ShowDialog(IWin32Window owner, string text, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton) {
            lock (_syncRoot) {
                return ShowDialog(owner, text, Resources.DefaultCaption, buttons, icon, defaultButton);
            }
        }

        /// <summary>
        /// Displays a message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        public static DialogResult ShowDialog(IWin32Window owner, string text, string caption) {
            lock (_syncRoot) {
                return ShowDialog(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        public static DialogResult ShowDialog(IWin32Window owner, string text, string caption, MessageBoxButtons buttons) {
            lock (_syncRoot) {
                return ShowDialog(owner, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="icon">One of the MessageBoxIcon values that specifies which icon to display in the message box.</param>
        public static DialogResult ShowDialog(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) {
            lock (_syncRoot) {
                return ShowDialog(owner, text, caption, buttons, icon, MessageBoxDefaultButton.Button1);
            }
        }

        #endregion


        /// <summary>
        /// Displays a message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="icon">One of the MessageBoxIcon values that specifies which icon to display in the message box.</param>
        /// <param name="defaultButton">One of the MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        public static DialogResult ShowDialog(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton) {
            if (!MessageBox.IsRunningOnMono) {
                lock (_syncRoot) {
                    if (owner != null) {
                        using (CbtHook ch = new CbtHook(owner)) {
                            return (DialogResult)NativeMethods.MessageBox(owner.Handle, text, caption, (uint)buttons | (uint)icon | (uint)defaultButton);
                        }
                    } else {
                        using (CbtHook ch = new CbtHook(null)) {
                            return (DialogResult)NativeMethods.MessageBox(IntPtr.Zero, text, caption, (uint)buttons | (uint)icon | (uint)defaultButton);
                        }
                    }
                } //lock
            } else { //MONO
                return System.Windows.Forms.MessageBox.Show(owner, text, caption, buttons, icon, defaultButton, 0);
            }
        }


        #region ShowInformation

        /// <summary>
        /// Displays a information message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        public static DialogResult ShowInformation(IWin32Window owner, string text) {
            lock (_syncRoot) {
                return ShowInformation(owner, text, Resources.DefaultCaption, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a information message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        public static DialogResult ShowInformation(IWin32Window owner, string text, MessageBoxButtons buttons) {
            lock (_syncRoot) {
                return ShowInformation(owner, text, Resources.DefaultCaption, buttons, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a information message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="defaultButton">One of the MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        public static DialogResult ShowInformation(IWin32Window owner, string text, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton) {
            lock (_syncRoot) {
                return ShowInformation(owner, text, Resources.DefaultCaption, buttons, defaultButton);
            }
        }

        /// <summary>
        /// Displays a information message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        public static DialogResult ShowInformation(IWin32Window owner, string text, string caption) {
            lock (_syncRoot) {
                return ShowInformation(owner, text, caption, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a information message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        public static DialogResult ShowInformation(IWin32Window owner, string text, string caption, MessageBoxButtons buttons) {
            lock (_syncRoot) {
                return ShowInformation(owner, text, caption, buttons, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a information message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="defaultButton">One of the MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        public static DialogResult ShowInformation(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton) {
            lock (_syncRoot) {
                return ShowDialog(owner, text, caption, buttons, MessageBoxIcon.Information, defaultButton);
            }
        }

        #endregion


        #region ShowWarning

        /// <summary>
        /// Displays a warning message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        public static DialogResult ShowWarning(IWin32Window owner, string text) {
            lock (_syncRoot) {
                return ShowWarning(owner, text, Resources.DefaultCaption, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a warning message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        public static DialogResult ShowWarning(IWin32Window owner, string text, MessageBoxButtons buttons) {
            lock (_syncRoot) {
                return ShowWarning(owner, text, Resources.DefaultCaption, buttons, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a warning message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="defaultButton">One of the MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        public static DialogResult ShowWarning(IWin32Window owner, string text, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton) {
            lock (_syncRoot) {
                return ShowWarning(owner, text, Resources.DefaultCaption, buttons, defaultButton);
            }
        }

        /// <summary>
        /// Displays a warning message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        public static DialogResult ShowWarning(IWin32Window owner, string text, string caption) {
            lock (_syncRoot) {
                return ShowWarning(owner, text, caption, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a warning message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        public static DialogResult ShowWarning(IWin32Window owner, string text, string caption, MessageBoxButtons buttons) {
            lock (_syncRoot) {
                return ShowWarning(owner, text, caption, buttons, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a warning message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="defaultButton">One of the MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        public static DialogResult ShowWarning(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton) {
            lock (_syncRoot) {
                return ShowDialog(owner, text, caption, buttons, MessageBoxIcon.Warning, defaultButton);
            }
        }

        #endregion


        #region ShowError

        /// <summary>
        /// Displays a error message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        public static DialogResult ShowError(IWin32Window owner, string text) {
            lock (_syncRoot) {
                return ShowError(owner, text, Resources.DefaultCaption, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a error message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        public static DialogResult ShowError(IWin32Window owner, string text, MessageBoxButtons buttons) {
            lock (_syncRoot) {
                return ShowError(owner, text, Resources.DefaultCaption, buttons, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a error message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="defaultButton">One of the MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        public static DialogResult ShowError(IWin32Window owner, string text, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton) {
            lock (_syncRoot) {
                return ShowError(owner, text, Resources.DefaultCaption, buttons, defaultButton);
            }
        }

        /// <summary>
        /// Displays a error message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        public static DialogResult ShowError(IWin32Window owner, string text, string caption) {
            lock (_syncRoot) {
                return ShowError(owner, text, caption, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a error message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        public static DialogResult ShowError(IWin32Window owner, string text, string caption, MessageBoxButtons buttons) {
            lock (_syncRoot) {
                return ShowError(owner, text, caption, buttons, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a error message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="defaultButton">One of the MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        public static DialogResult ShowError(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton) {
            lock (_syncRoot) {
                return ShowDialog(owner, text, caption, buttons, MessageBoxIcon.Error, defaultButton);
            }
        }

        #endregion


        #region ShowQuestion

        /// <summary>
        /// Displays a question message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        public static DialogResult ShowQuestion(IWin32Window owner, string text) {
            lock (_syncRoot) {
                return ShowQuestion(owner, text, Resources.DefaultCaption, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a question message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        public static DialogResult ShowQuestion(IWin32Window owner, string text, MessageBoxButtons buttons) {
            lock (_syncRoot) {
                return ShowQuestion(owner, text, Resources.DefaultCaption, buttons, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a question message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="defaultButton">One of the MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        public static DialogResult ShowQuestion(IWin32Window owner, string text, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton) {
            lock (_syncRoot) {
                return ShowQuestion(owner, text, Resources.DefaultCaption, buttons, defaultButton);
            }
        }

        /// <summary>
        /// Displays a question message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        public static DialogResult ShowQuestion(IWin32Window owner, string text, string caption) {
            lock (_syncRoot) {
                return ShowQuestion(owner, text, caption, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a question message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        public static DialogResult ShowQuestion(IWin32Window owner, string text, string caption, MessageBoxButtons buttons) {
            lock (_syncRoot) {
                return ShowQuestion(owner, text, caption, buttons, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Displays a question message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="defaultButton">One of the MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        public static DialogResult ShowQuestion(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton) {
            lock (_syncRoot) {
                return ShowDialog(owner, text, caption, buttons, MessageBoxIcon.Question, defaultButton);
            }
        }

        #endregion


        private static class Resources {

            internal static string DefaultCaption {
                get {
                    var assembly = Assembly.GetEntryAssembly();

                    string caption;
                    object[] productAttributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), true);
                    if ((productAttributes != null) && (productAttributes.Length >= 1)) {
                        caption = ((AssemblyProductAttribute)productAttributes[productAttributes.Length - 1]).Product;
                    } else {
                        object[] titleAttributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), true);
                        if ((titleAttributes != null) && (titleAttributes.Length >= 1)) {
                            caption = ((AssemblyTitleAttribute)titleAttributes[titleAttributes.Length - 1]).Title;
                        } else {
                            caption = assembly.GetName().Name;
                        }
                    }

                    return caption;
                }
            }


            internal static string OK {
                get { return GetInCurrentLanguage("OK", "U redu"); }
            }

            internal static string Cancel {
                get { return GetInCurrentLanguage("Cancel", "Odustani"); }
            }

            internal static string Abort {
                get { return GetInCurrentLanguage("&Abort", "P&rekini"); }
            }

            internal static string Retry {
                get { return GetInCurrentLanguage("&Retry", "&Ponovi"); }
            }

            internal static string Ignore {
                get { return GetInCurrentLanguage("&Ignore", "&Zanemari"); }
            }

            internal static string Yes {
                get { return GetInCurrentLanguage("&Yes", "&Da"); }
            }

            internal static string No {
                get { return GetInCurrentLanguage("&No", "&Ne"); }
            }


            internal static string ExceptionCbtHookCannotBeRemoved { get { return "CBT Hook cannot be removed."; } }


            internal static bool IsTranslatable {
                get {
                    switch (Thread.CurrentThread.CurrentUICulture.Name.ToUpperInvariant()) {
                        case "EN":
                        case "EN-US":
                        case "EN-GB":
                        case "HR":
                        case "HR-HR":
                        case "HR-BA":
                            return true;

                        default:
                            return false;
                    }
                }
            }

            private static string GetInCurrentLanguage(string en_US, string hr_HR) {
                switch (Thread.CurrentThread.CurrentUICulture.Name.ToUpperInvariant()) {
                    case "EN":
                    case "EN-US":
                    case "EN-GB":
                        return en_US;

                    case "HR":
                    case "HR-HR":
                    case "HR-BA":
                        return hr_HR;

                    default:
                        return en_US;
                }
            }

        }


        #region Native

        private class CbtHook : IDisposable {

            private readonly IWin32Window _owner;

            private readonly NativeMethods.WindowsHookSafeHandle _hook;
            private readonly NativeMethods.CbtHookProcDelegate _cbtHookProc;


            public CbtHook(IWin32Window owner) {
                _owner = owner;
                _cbtHookProc = new NativeMethods.CbtHookProcDelegate(CbtHookProc);
                _hook = NativeMethods.SetWindowsHookEx(NativeMethods.WH_CBT, _cbtHookProc, IntPtr.Zero, NativeMethods.GetCurrentThreadId());
                Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "I: Created CBT hook (ID={0}).    {{Medo.MessageBox}}", _hook.ToString()));
            }

            ~CbtHook() {
                Dispose();
            }


            public IntPtr CbtHookProc(int nCode, IntPtr wParam, IntPtr lParam) {
                switch (nCode) {
                    case NativeMethods.HCBT_ACTIVATE:
                        Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "I: Dialog HCBT_ACTIVATE (hWnd={0}).    {{Medo.MessageBox}}", wParam.ToString()));

                        if (_owner != null) {
                            NativeMethods.RECT rectMessage = new NativeMethods.RECT();
                            NativeMethods.RECT rectOwner = new NativeMethods.RECT();
                            if ((NativeMethods.GetWindowRect(wParam, ref rectMessage)) && (NativeMethods.GetWindowRect(_owner.Handle, ref rectOwner))) {
                                int widthMessage = rectMessage.right - rectMessage.left;
                                int heightMessage = rectMessage.bottom - rectMessage.top;
                                int widthOwner = rectOwner.right - rectOwner.left;
                                int heightOwner = rectOwner.bottom - rectOwner.top;

                                int newLeft = rectOwner.left + (widthOwner - widthMessage) / 2;
                                int newTop = rectOwner.top + (heightOwner - heightMessage) / 2;

                                NativeMethods.SetWindowPos(wParam, IntPtr.Zero, newLeft, newTop, 0, 0, NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOACTIVATE);
                            }
                        }

                        if (Resources.IsTranslatable) {
                            NativeMethods.SetDlgItemText(wParam, NativeMethods.DLG_ID_OK, Resources.OK);
                            NativeMethods.SetDlgItemText(wParam, NativeMethods.DLG_ID_CANCEL, Resources.Cancel);
                            NativeMethods.SetDlgItemText(wParam, NativeMethods.DLG_ID_ABORT, Resources.Abort);
                            NativeMethods.SetDlgItemText(wParam, NativeMethods.DLG_ID_RETRY, Resources.Retry);
                            NativeMethods.SetDlgItemText(wParam, NativeMethods.DLG_ID_IGNORE, Resources.Ignore);
                            NativeMethods.SetDlgItemText(wParam, NativeMethods.DLG_ID_YES, Resources.Yes);
                            NativeMethods.SetDlgItemText(wParam, NativeMethods.DLG_ID_NO, Resources.No);
                        }

                        try {
                            return NativeMethods.CallNextHookEx(_hook, nCode, wParam, lParam);
                        } finally {
                            Dispose();
                        }
                }
                return NativeMethods.CallNextHookEx(_hook, nCode, wParam, lParam);
            }


            #region IDisposable Members

            public void Dispose() {
                if (!_hook.IsClosed) {
                    _hook.Close();
                    if (_hook.IsClosed) {
                        Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "I: CBT Hook destroyed (ID={0}).    {{Medo.MessageBox}}", _hook.ToString()));
                    } else {
                        throw new InvalidOperationException(Resources.ExceptionCbtHookCannotBeRemoved);
                    }
                }
                _hook.Dispose();
                GC.SuppressFinalize(this);
            }

            #endregion

        }


        private static class NativeMethods {
#pragma warning disable IDE0049 // Simplify Names

            public const Int32 WH_CBT = 0x5;

            public const Int32 DLG_ID_OK = 0x01;
            public const Int32 DLG_ID_CANCEL = 0x02;
            public const Int32 DLG_ID_ABORT = 0x03;
            public const Int32 DLG_ID_RETRY = 0x04;
            public const Int32 DLG_ID_IGNORE = 0x05;
            public const Int32 DLG_ID_YES = 0x06;
            public const Int32 DLG_ID_NO = 0x07;

            public const Int32 HCBT_ACTIVATE = 0x5;

            public const Int32 SWP_NOSIZE = 0x01;
            public const Int32 SWP_NOZORDER = 0x04;
            public const Int32 SWP_NOACTIVATE = 0x10;


            [StructLayout(LayoutKind.Sequential)]
            public struct RECT {
                public Int32 left;
                public Int32 top;
                public Int32 right;
                public Int32 bottom;
            }


            public class WindowsHookSafeHandle : SafeHandle {
                public WindowsHookSafeHandle()
                    : base(IntPtr.Zero, true) {
                }


                public override bool IsInvalid {
                    get { return (IsClosed) || (base.handle == IntPtr.Zero); }
                }

                protected override bool ReleaseHandle() {
                    return UnhookWindowsHookEx(base.handle);
                }

                public override string ToString() {
                    return base.handle.ToString();
                }

            }


            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "Warning is bogus.")]
            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr CallNextHookEx(WindowsHookSafeHandle idHook, Int32 nCode, IntPtr wParam, IntPtr lParam);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "Warning is bogus.")]
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 GetCurrentThreadId();

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "Warning is bogus.")]
            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern Boolean GetWindowRect(IntPtr hWnd, ref RECT lpRect);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "Warning is bogus.")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2205:UseManagedEquivalentsOfWin32Api", Justification = "Managed equivalent does not support all needed features.")]
            [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern Int32 MessageBox(IntPtr hWnd, String lpText, String lpCaption, UInt32 uType);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "Warning is bogus.")]
            [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern Boolean SetDlgItemText(IntPtr hWnd, Int32 nIDDlgItem, String lpString);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "Warning is bogus.")]
            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern Boolean SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, Int32 X, Int32 Y, Int32 cx, Int32 cy, UInt32 uFlags);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "Warning is bogus.")]
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            public static extern WindowsHookSafeHandle SetWindowsHookEx(Int32 idHook, CbtHookProcDelegate lpfn, IntPtr hInstance, Int32 threadId);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule", Justification = "Warning is bogus.")]
            [ReliabilityContract(Consistency.MayCorruptProcess, Cer.Success)]
            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern Boolean UnhookWindowsHookEx(IntPtr idHook);


            public delegate IntPtr CbtHookProcDelegate(Int32 nCode, IntPtr wParam, IntPtr lParam);

#pragma warning restore IDE0049 // Simplify Names
        }

        #endregion


        private static bool IsRunningOnMono {
            get {
                return (Type.GetType("Mono.Runtime") != null);
            }
        }

    }
}
