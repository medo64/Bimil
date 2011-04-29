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


        private void btnOK_Click(object sender, EventArgs e) {
            this.Template = (Template)cmbTemplate.SelectedItem;
        }

        public Template Template { get; private set; }

    }
}
