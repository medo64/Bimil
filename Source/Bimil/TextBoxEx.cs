using System;
using System.Windows.Forms;

namespace Bimil {
    internal class TextBoxEx : TextBox {

        protected override void OnKeyDown(KeyEventArgs e) {
            if (e.KeyData == (Keys.Control | Keys.A)) {
                SelectAll();
            } else {
                base.OnKeyDown(e);
            }
        }

        protected override void WndProc(ref Message m) {
            if ((m != null) && (m.Msg == NativeMethods.WM_PASTE)) {
                if (Clipboard.ContainsText()) {
                    var lines = Clipboard.GetText().Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                    SelectedText = string.Join(Environment.NewLine, lines);
                }
            } else if (m != null) {
                base.WndProc(ref m);
            }
        }


        private static class NativeMethods {
            internal const Int32 WM_PASTE = 0x0302;
        }
    }
}
