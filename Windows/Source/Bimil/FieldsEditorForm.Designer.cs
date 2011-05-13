namespace Bimil {
    partial class FieldsEditorForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.pnlFields = new System.Windows.Forms.Panel();
            this.lsvFields = new System.Windows.Forms.ListView();
            this.lsvFields_colTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.pnlFields.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlFields
            // 
            this.pnlFields.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlFields.Controls.Add(this.lsvFields);
            this.pnlFields.Location = new System.Drawing.Point(12, 12);
            this.pnlFields.Name = "pnlFields";
            this.pnlFields.Size = new System.Drawing.Size(450, 297);
            this.pnlFields.TabIndex = 0;
            // 
            // lsvFields
            // 
            this.lsvFields.AllowDrop = true;
            this.lsvFields.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lsvFields.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.lsvFields_colTitle});
            this.lsvFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsvFields.FullRowSelect = true;
            this.lsvFields.GridLines = true;
            this.lsvFields.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lsvFields.LabelEdit = true;
            this.lsvFields.LabelWrap = false;
            this.lsvFields.Location = new System.Drawing.Point(0, 0);
            this.lsvFields.MultiSelect = false;
            this.lsvFields.Name = "lsvFields";
            this.lsvFields.ShowGroups = false;
            this.lsvFields.Size = new System.Drawing.Size(450, 297);
            this.lsvFields.TabIndex = 0;
            this.lsvFields.UseCompatibleStateImageBehavior = false;
            this.lsvFields.View = System.Windows.Forms.View.Details;
            this.lsvFields.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.lsvFields_ColumnWidthChanging);
            this.lsvFields.ItemActivate += new System.EventHandler(this.lsvFields_ItemActivate);
            this.lsvFields.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.lsvFields_ItemDrag);
            this.lsvFields.SelectedIndexChanged += new System.EventHandler(this.lsvFields_SelectedIndexChanged);
            this.lsvFields.DragDrop += new System.Windows.Forms.DragEventHandler(this.lsvFields_DragDrop);
            this.lsvFields.DragEnter += new System.Windows.Forms.DragEventHandler(this.lsvFields_DragEnter);
            this.lsvFields.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lsvFields_KeyDown);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(372, 327);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 28);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(276, 327);
            this.btnOK.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(90, 28);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAdd.Location = new System.Drawing.Point(12, 327);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(90, 28);
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemove.Enabled = false;
            this.btnRemove.Location = new System.Drawing.Point(108, 327);
            this.btnRemove.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(90, 28);
            this.btnRemove.TabIndex = 2;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // FieldsEditorForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(474, 367);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.pnlFields);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FieldsEditorForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Fields editor";
            this.Load += new System.EventHandler(this.Form_Load);
            this.pnlFields.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlFields;
        private System.Windows.Forms.ListView lsvFields;
        private System.Windows.Forms.ColumnHeader lsvFields_colTitle;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
    }
}