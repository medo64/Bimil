using System;
using System.Drawing;
using System.Windows.Forms;
using Medo.Security.Cryptography.PasswordSafe;

namespace Bimil {
    internal partial class DocumentInfoForm : Form {
        public DocumentInfoForm(Document document) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;
            Medo.Windows.Forms.State.SetupOnLoadAndClose(this);

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
        }

        private void btnOK_Click(object sender, EventArgs e) {
            this.Document.Name = txtName.Text;
            this.Document.Description = txtDescription.Text;
        }
    }
}
