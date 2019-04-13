namespace Bimil {
    partial class ItemForm {
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
            this.pnl = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnFields = new System.Windows.Forms.Button();
            this.tip = new System.Windows.Forms.ToolTip(this.components);
            this.btnAutotype = new System.Windows.Forms.Button();
            this.btnAutotypeConfigure = new System.Windows.Forms.Button();
            this.tmrClose = new System.Windows.Forms.Timer(this.components);
            this.erp = new System.Windows.Forms.ErrorProvider(this.components);
            this.bwCheckTime = new System.ComponentModel.BackgroundWorker();
            this.lblNtpWarning = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.erp)).BeginInit();
            this.SuspendLayout();
            // 
            // pnl
            // 
            this.pnl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnl.AutoScroll = true;
            this.pnl.Location = new System.Drawing.Point(12, 12);
            this.pnl.Name = "pnl";
            this.pnl.Size = new System.Drawing.Size(558, 325);
            this.pnl.TabIndex = 1;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(480, 355);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 28);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.tip.SetToolTip(this.btnCancel, "Close form without saving.");
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(384, 355);
            this.btnOK.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(90, 28);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.tip.SetToolTip(this.btnOK, "Save changes and close form.");
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Visible = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEdit.Location = new System.Drawing.Point(135, 355);
            this.btnEdit.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(90, 28);
            this.btnEdit.TabIndex = 4;
            this.btnEdit.Text = "Edit";
            this.tip.SetToolTip(this.btnEdit, "Allow entry editing.");
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnFields
            // 
            this.btnFields.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFields.Location = new System.Drawing.Point(135, 355);
            this.btnFields.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnFields.Name = "btnFields";
            this.btnFields.Size = new System.Drawing.Size(90, 28);
            this.btnFields.TabIndex = 5;
            this.btnFields.Text = "Fields";
            this.tip.SetToolTip(this.btnFields, "Add or remove fields.");
            this.btnFields.UseVisualStyleBackColor = true;
            this.btnFields.Visible = false;
            this.btnFields.Click += new System.EventHandler(this.btnFields_Click);
            // 
            // btnAutotype
            // 
            this.btnAutotype.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAutotype.Location = new System.Drawing.Point(12, 355);
            this.btnAutotype.Name = "btnAutotype";
            this.btnAutotype.Size = new System.Drawing.Size(90, 28);
            this.btnAutotype.TabIndex = 2;
            this.btnAutotype.Text = "Auto-type";
            this.tip.SetToolTip(this.btnAutotype, "Shows auto-type menu.");
            this.btnAutotype.UseVisualStyleBackColor = true;
            this.btnAutotype.Click += new System.EventHandler(this.btnAutotype_Click);
            // 
            // btnAutotypeConfigure
            // 
            this.btnAutotypeConfigure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAutotypeConfigure.Image = global::Bimil.Properties.Resources.btnConfigure_16;
            this.btnAutotypeConfigure.Location = new System.Drawing.Point(101, 355);
            this.btnAutotypeConfigure.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.btnAutotypeConfigure.Name = "btnAutotypeConfigure";
            this.btnAutotypeConfigure.Size = new System.Drawing.Size(28, 28);
            this.btnAutotypeConfigure.TabIndex = 3;
            this.btnAutotypeConfigure.Tag = "btnConfigure";
            this.tip.SetToolTip(this.btnAutotypeConfigure, "Shows auto-type configuration.");
            this.btnAutotypeConfigure.UseVisualStyleBackColor = true;
            this.btnAutotypeConfigure.Click += new System.EventHandler(this.btnAutotypeConfigure_Click);
            // 
            // tmrClose
            // 
            this.tmrClose.Tick += new System.EventHandler(this.tmrClose_Tick);
            // 
            // erp
            // 
            this.erp.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.erp.ContainerControl = this;
            // 
            // bwCheckTime
            // 
            this.bwCheckTime.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwCheckTime_DoWork);
            this.bwCheckTime.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwCheckTime_RunWorkerCompleted);
            // 
            // lblNtpWarning
            // 
            this.lblNtpWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblNtpWarning.AutoSize = true;
            this.lblNtpWarning.ForeColor = System.Drawing.Color.Red;
            this.lblNtpWarning.Location = new System.Drawing.Point(12, 335);
            this.lblNtpWarning.Name = "lblNtpWarning";
            this.lblNtpWarning.Size = new System.Drawing.Size(189, 17);
            this.lblNtpWarning.TabIndex = 7;
            this.lblNtpWarning.Text = "Computer time is not correct.";
            this.lblNtpWarning.Visible = false;
            // 
            // ItemForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(582, 395);
            this.Controls.Add(this.lblNtpWarning);
            this.Controls.Add(this.btnFields);
            this.Controls.Add(this.btnAutotypeConfigure);
            this.Controls.Add(this.btnAutotype);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.pnl);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(480, 320);
            this.Name = "ItemForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit item";
            this.Activated += new System.EventHandler(this.Form_Activated);
            this.Deactivate += new System.EventHandler(this.Form_Deactivate);
            this.Load += new System.EventHandler(this.Form_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_KeyDown);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.Form_PreviewKeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.erp)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnl;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnFields;
        private System.Windows.Forms.ToolTip tip;
        private System.Windows.Forms.Timer tmrClose;
        private System.Windows.Forms.Button btnAutotype;
        private System.Windows.Forms.ErrorProvider erp;
        private System.Windows.Forms.Button btnAutotypeConfigure;
        private System.ComponentModel.BackgroundWorker bwCheckTime;
        private System.Windows.Forms.Label lblNtpWarning;
    }
}