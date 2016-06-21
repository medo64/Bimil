using System;
using System.Drawing;
using System.Windows.Forms;

namespace Bimil {
    internal partial class QRCodeForm : Form {
        public QRCodeForm(string text) {
            InitializeComponent();

            this.Coder = new QRCoder.QRCode(text);

            int qrSize;
            using (var bmp = this.Coder.GetBitmap()) {
                qrSize = Math.Max(bmp.Width, bmp.Height);
            }

            var screen = Screen.GetWorkingArea(this);
            var factor = Math.Min(screen.Width, screen.Height) / qrSize / 2;

            var scaledBitmap = this.Coder.GetBitmap(factor);
            this.ClientSize = scaledBitmap.Size;
            this.BackgroundImage = scaledBitmap;
        }

        private readonly QRCoder.QRCode Coder;


        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);
            if ((e.KeyData == Keys.Escape) || (e.KeyData == Keys.Return)) { this.DialogResult = DialogResult.Cancel; }
        }

    }
}
