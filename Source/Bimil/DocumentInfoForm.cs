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
            Font = SystemFonts.MessageBoxFont;
            Medo.Windows.Forms.State.Attach(this);

            Document = document;

            txtName.ReadOnly = document.IsReadOnly;
            txtDescription.ReadOnly = document.IsReadOnly;
            btnOK.Enabled = !document.IsReadOnly;
        }


        private readonly Document Document;


        private void Form_Load(object sender, EventArgs e) {
            txtName.Text = Document.Name;
            txtDescription.Text = Document.Description;

            txtUuid.Text = Document.Uuid.ToString();

            txtLastSaveApplication.Text = Document.LastSaveApplication;
            txtLastSaveUser.Text = Document.LastSaveUser + " (" + Document.LastSaveHost + ")";
            txtLastSaveTime.Text = Document.LastSaveTime.ToString("f");

            foreach (var header in Document.Headers) {
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
            if (Document.IsReadOnly) { chbStaticKey.Enabled = false; }
        }


        private void chbStaticKey_CheckedChanged(object sender, EventArgs e) {
            var heightDiff = grpStaticKey.Bottom - chbStaticKey.Bottom;
            if (chbStaticKey.Checked && !grpStaticKey.Visible) {
                grpStaticKey.Visible = true;
                MinimumSize = new Size(MinimumSize.Width, MinimumSize.Height + heightDiff);
                Size = new Size(Size.Width, MinimumSize.Height);
                MaximumSize = new Size(MaximumSize.Width, MinimumSize.Height);

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
                MinimumSize = new Size(MinimumSize.Width, MinimumSize.Height - heightDiff);
                Size = new Size(Size.Width, MinimumSize.Height);
                MaximumSize = new Size(MaximumSize.Width, MinimumSize.Height);
            }
        }


        private void btnOK_Click(object sender, EventArgs e) {
            Document.Name = txtName.Text;
            Document.Description = txtDescription.Text;

            if (chbStaticKey.Enabled) { //just if document is not read-only
                //first remove old static key header - if any
                for (var i = Document.Headers.Count - 1; i >= 0; i--) {
                    var header = Document.Headers[i];
                    if (header.HeaderType == Helpers.HeaderConstants.StaticKey) {
                        var bytes = header.GetBytes();
                        try {
                            if (bytes.Length == 64) {
                                Document.Headers.RemoveAt(i);
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
                        Document.Headers.Add(header);
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
