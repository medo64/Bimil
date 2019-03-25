using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Windows.Forms;
using Medo.Convert;
using Medo.Security.Cryptography.PasswordSafe;

namespace Bimil {
    internal partial class DocumentInfoForm : Form {
        public DocumentInfoForm(Document document) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;
            Medo.Windows.Forms.State.Attach(this);

            this.Document = document;

            txtName.ReadOnly = document.IsReadOnly;
            txtDescription.ReadOnly = document.IsReadOnly;
            btnOK.Enabled = !document.IsReadOnly;
        }


        private readonly Document Document;


        private void Form_Load(object sender, EventArgs e) {
            txtName.Text = this.Document.Name;
            txtDescription.Text = this.Document.Description;

            txtUuid.Text = this.Document.Uuid.ToString();

            txtLastSaveApplication.Text = this.Document.LastSaveApplication;
            txtLastSaveUser.Text = this.Document.LastSaveUser + " (" + this.Document.LastSaveHost + ")";
            txtLastSaveTime.Text = this.Document.LastSaveTime.ToString("f");

            foreach (var header in this.Document.Headers) {
                if (header.HeaderType == Helpers.HeaderConstants.StaticKey) {
                    var bytes = header.GetBytes();
                    try {
                        if (bytes.Length == 64) {
                            chbStaticKey.Checked = true;
                            txtStaticKey.Text = Base58.ToString(bytes);
                            break;
                        } else {
                            Array.Clear(bytes, 0, bytes.Length);
                        }
                    } finally {
                        Array.Clear(bytes, 0, bytes.Length);
                    }
                }
            }
            chbStaticKey_CheckedChanged(null, null); //to sort out size diffs
            if (this.Document.IsReadOnly) { chbStaticKey.Enabled = false; }
        }


        private void chbStaticKey_CheckedChanged(object sender, EventArgs e) {
            var heightDiff = grpStaticKey.Bottom - chbStaticKey.Bottom;
            if (chbStaticKey.Checked && !grpStaticKey.Visible) {
                grpStaticKey.Visible = true;
                this.MinimumSize = new Size(this.MinimumSize.Width, this.MinimumSize.Height + heightDiff);
                this.Size = new Size(this.Size.Width, this.MinimumSize.Height);
                this.MaximumSize = new Size(this.MaximumSize.Width, this.MinimumSize.Height);

                //make a new key
                var keyBytes = new byte[64];
                try {
                    var random = RandomNumberGenerator.Create();
                    random.GetBytes(keyBytes);
                    txtStaticKey.Text = Base58.ToString(keyBytes);
                } finally {
                    Array.Clear(keyBytes, 0, keyBytes.Length);
                }
            } else if (!chbStaticKey.Checked && grpStaticKey.Visible) {
                grpStaticKey.Visible = false;
                this.MinimumSize = new Size(this.MinimumSize.Width, this.MinimumSize.Height - heightDiff);
                this.Size = new Size(this.Size.Width, this.MinimumSize.Height);
                this.MaximumSize = new Size(this.MaximumSize.Width, this.MinimumSize.Height);
            }
        }


        private void btnOK_Click(object sender, EventArgs e) {
            this.Document.Name = txtName.Text;
            this.Document.Description = txtDescription.Text;

            if (chbStaticKey.Enabled) { //just if document is not read-only
                //first remove old static key header - if any
                for (var i = this.Document.Headers.Count - 1; i >= 0; i--) {
                    var header = this.Document.Headers[i];
                    if (header.HeaderType == Helpers.HeaderConstants.StaticKey) {
                        var bytes = header.GetBytes();
                        try {
                            if (bytes.Length == 64) {
                                this.Document.Headers.RemoveAt(i);
                            }
                        } finally {
                            Array.Clear(bytes, 0, bytes.Length);
                        }
                    }
                }

                //add new static key header
                if (chbStaticKey.Checked) {
                    var bytes = Base58.AsBytes(txtStaticKey.Text);
                    try {
                        var header = new Header(Helpers.HeaderConstants.StaticKey, bytes);
                        this.Document.Headers.Add(header);
                    } finally {
                        Array.Clear(bytes, 0, bytes.Length);
                        txtStaticKey.Text = "";
                        GC.Collect();
                    }
                }
            }
        }

    }
}
