using System;
using System.Windows.Forms;

namespace Bimil {
    internal class TextBoxEx : TextBox {

        protected override void WndProc(ref Message m) {
            if (m.Msg == NativeMethods.WM_PASTE) {
                if (Clipboard.ContainsText()) {
                    var lines = Clipboard.GetText().Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                    this.SelectedText = string.Join(Environment.NewLine, lines);
                }
            } else {
                base.WndProc(ref m);
            }
        }


        private static class NativeMethods {
            internal const Int32 WM_PASTE = 0x0302;
        }
    }
}
