namespace Bimil {
    partial class AutotypeHelpForm {
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
            this.components = new System.ComponentModel.Container();
            this.lsvHelp = new System.Windows.Forms.ListView();
            this.lsvHelp_colEscape = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lsvHelp_colHelp = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.txtAutotype = new System.Windows.Forms.TextBox();
            this.tip = new System.Windows.Forms.ToolTip(this.components);
            this.lblDescription = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lsvHelp
            // 
            this.lsvHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsvHelp.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.lsvHelp_colEscape,
            this.lsvHelp_colHelp});
            this.lsvHelp.FullRowSelect = true;
            this.lsvHelp.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lsvHelp.Location = new System.Drawing.Point(12, 12);
            this.lsvHelp.MultiSelect = false;
            this.lsvHelp.Name = "lsvHelp";
            this.lsvHelp.ShowItemToolTips = true;
            this.lsvHelp.Size = new System.Drawing.Size(598, 323);
            this.lsvHelp.TabIndex = 0;
            this.lsvHelp.UseCompatibleStateImageBehavior = false;
            this.lsvHelp.View = System.Windows.Forms.View.Details;
            this.lsvHelp.ItemActivate += new System.EventHandler(this.lsvHelp_ItemActivate);
            // 
            // lsvHelp_colEscape
            // 
            this.lsvHelp_colEscape.Width = 90;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(520, 393);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 28);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.tip.SetToolTip(this.btnCancel, "Close without saving changes.");
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(424, 393);
            this.btnOK.Margin = new System.Windows.Forms.Padding(15, 15, 3, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(90, 28);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "OK";
            this.tip.SetToolTip(this.btnOK, "Save changes.");
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // txtAutotype
            // 
            this.txtAutotype.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAutotype.HideSelection = false;
            this.txtAutotype.Location = new System.Drawing.Point(12, 396);
            this.txtAutotype.Name = "txtAutotype";
            this.txtAutotype.Size = new System.Drawing.Size(394, 22);
            this.txtAutotype.TabIndex = 8;
            this.txtAutotype.TextChanged += new System.EventHandler(this.txtAutotype_TextChanged);
            // 
            // lblDescription
            // 
            this.lblDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDescription.Location = new System.Drawing.Point(12, 338);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(598, 40);
            this.lblDescription.TabIndex = 9;
            // 
            // AutotypeHelpForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(622, 433);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.txtAutotype);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lsvHelp);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(480, 320);
            this.Name = "AutotypeHelpForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Auto-type help";
            this.Load += new System.EventHandler(this.Form_Load);
            this.Shown += new System.EventHandler(this.Form_Shown);
            this.Resize += new System.EventHandler(this.Form_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lsvHelp;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox txtAutotype;
        private System.Windows.Forms.ColumnHeader lsvHelp_colEscape;
        private System.Windows.Forms.ColumnHeader lsvHelp_colHelp;
        private System.Windows.Forms.ToolTip tip;
        private System.Windows.Forms.Label lblDescription;
    }
}