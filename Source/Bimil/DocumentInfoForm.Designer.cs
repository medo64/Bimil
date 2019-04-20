namespace Bimil {
    partial class DocumentInfoForm {
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
            this.lblDescription = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblName = new System.Windows.Forms.Label();
            this.lblUuid = new System.Windows.Forms.Label();
            this.grpLastSave = new System.Windows.Forms.GroupBox();
            this.lblLastSaveTime = new System.Windows.Forms.Label();
            this.txtLastSaveTime = new Bimil.TextBoxEx();
            this.lblLastSaveUser = new System.Windows.Forms.Label();
            this.txtLastSaveUser = new Bimil.TextBoxEx();
            this.lblLastSaveApplication = new System.Windows.Forms.Label();
            this.txtLastSaveApplication = new Bimil.TextBoxEx();
            this.grpStaticKey = new System.Windows.Forms.GroupBox();
            this.lblStaticKey = new System.Windows.Forms.Label();
            this.txtStaticKey = new Bimil.TextBoxEx();
            this.chbStaticKey = new System.Windows.Forms.CheckBox();
            this.txtUuid = new Bimil.TextBoxEx();
            this.txtName = new Bimil.TextBoxEx();
            this.txtDescription = new Bimil.TextBoxEx();
            this.grpLastSave.SuspendLayout();
            this.grpStaticKey.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(12, 77);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(83, 17);
            this.lblDescription.TabIndex = 5;
            this.lblDescription.Text = "Description:";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(400, 357);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 28);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(304, 357);
            this.btnOK.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(90, 28);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(12, 49);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(49, 17);
            this.lblName.TabIndex = 3;
            this.lblName.Text = "Name:";
            // 
            // lblUuid
            // 
            this.lblUuid.AutoSize = true;
            this.lblUuid.Location = new System.Drawing.Point(12, 15);
            this.lblUuid.Name = "lblUuid";
            this.lblUuid.Size = new System.Drawing.Size(25, 17);
            this.lblUuid.TabIndex = 1;
            this.lblUuid.Text = "ID:";
            // 
            // grpLastSave
            // 
            this.grpLastSave.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpLastSave.Controls.Add(this.lblLastSaveTime);
            this.grpLastSave.Controls.Add(this.txtLastSaveTime);
            this.grpLastSave.Controls.Add(this.lblLastSaveUser);
            this.grpLastSave.Controls.Add(this.txtLastSaveUser);
            this.grpLastSave.Controls.Add(this.lblLastSaveApplication);
            this.grpLastSave.Controls.Add(this.txtLastSaveApplication);
            this.grpLastSave.Location = new System.Drawing.Point(15, 160);
            this.grpLastSave.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.grpLastSave.Name = "grpLastSave";
            this.grpLastSave.Size = new System.Drawing.Size(475, 111);
            this.grpLastSave.TabIndex = 7;
            this.grpLastSave.TabStop = false;
            this.grpLastSave.Text = "Last saved";
            // 
            // lblLastSaveTime
            // 
            this.lblLastSaveTime.AutoSize = true;
            this.lblLastSaveTime.Location = new System.Drawing.Point(6, 86);
            this.lblLastSaveTime.Name = "lblLastSaveTime";
            this.lblLastSaveTime.Size = new System.Drawing.Size(43, 17);
            this.lblLastSaveTime.TabIndex = 4;
            this.lblLastSaveTime.Text = "Time:";
            // 
            // txtLastSaveTime
            // 
            this.txtLastSaveTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLastSaveTime.Location = new System.Drawing.Point(115, 83);
            this.txtLastSaveTime.Name = "txtLastSaveTime";
            this.txtLastSaveTime.ReadOnly = true;
            this.txtLastSaveTime.Size = new System.Drawing.Size(354, 22);
            this.txtLastSaveTime.TabIndex = 5;
            // 
            // lblLastSaveUser
            // 
            this.lblLastSaveUser.AutoSize = true;
            this.lblLastSaveUser.Location = new System.Drawing.Point(6, 58);
            this.lblLastSaveUser.Name = "lblLastSaveUser";
            this.lblLastSaveUser.Size = new System.Drawing.Size(42, 17);
            this.lblLastSaveUser.TabIndex = 2;
            this.lblLastSaveUser.Text = "User:";
            // 
            // txtLastSaveUser
            // 
            this.txtLastSaveUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLastSaveUser.Location = new System.Drawing.Point(115, 55);
            this.txtLastSaveUser.Name = "txtLastSaveUser";
            this.txtLastSaveUser.ReadOnly = true;
            this.txtLastSaveUser.Size = new System.Drawing.Size(354, 22);
            this.txtLastSaveUser.TabIndex = 3;
            // 
            // lblLastSaveApplication
            // 
            this.lblLastSaveApplication.AutoSize = true;
            this.lblLastSaveApplication.Location = new System.Drawing.Point(6, 30);
            this.lblLastSaveApplication.Name = "lblLastSaveApplication";
            this.lblLastSaveApplication.Size = new System.Drawing.Size(81, 17);
            this.lblLastSaveApplication.TabIndex = 0;
            this.lblLastSaveApplication.Text = "Application:";
            // 
            // txtLastSaveApplication
            // 
            this.txtLastSaveApplication.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLastSaveApplication.Location = new System.Drawing.Point(115, 27);
            this.txtLastSaveApplication.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.txtLastSaveApplication.Name = "txtLastSaveApplication";
            this.txtLastSaveApplication.ReadOnly = true;
            this.txtLastSaveApplication.Size = new System.Drawing.Size(354, 22);
            this.txtLastSaveApplication.TabIndex = 1;
            // 
            // grpStaticKey
            // 
            this.grpStaticKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpStaticKey.Controls.Add(this.lblStaticKey);
            this.grpStaticKey.Controls.Add(this.txtStaticKey);
            this.grpStaticKey.Location = new System.Drawing.Point(15, 283);
            this.grpStaticKey.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.grpStaticKey.Name = "grpStaticKey";
            this.grpStaticKey.Size = new System.Drawing.Size(475, 56);
            this.grpStaticKey.TabIndex = 9;
            this.grpStaticKey.TabStop = false;
            // 
            // lblStaticKey
            // 
            this.lblStaticKey.AutoSize = true;
            this.lblStaticKey.Location = new System.Drawing.Point(6, 30);
            this.lblStaticKey.Name = "lblStaticKey";
            this.lblStaticKey.Size = new System.Drawing.Size(36, 17);
            this.lblStaticKey.TabIndex = 0;
            this.lblStaticKey.Text = "Key:";
            // 
            // txtStaticKey
            // 
            this.txtStaticKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStaticKey.Location = new System.Drawing.Point(115, 27);
            this.txtStaticKey.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.txtStaticKey.Name = "txtStaticKey";
            this.txtStaticKey.ReadOnly = true;
            this.txtStaticKey.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStaticKey.Size = new System.Drawing.Size(354, 22);
            this.txtStaticKey.TabIndex = 1;
            this.txtStaticKey.WordWrap = false;
            // 
            // chbStaticKey
            // 
            this.chbStaticKey.AutoSize = true;
            this.chbStaticKey.Location = new System.Drawing.Point(20, 280);
            this.chbStaticKey.Name = "chbStaticKey";
            this.chbStaticKey.Size = new System.Drawing.Size(207, 21);
            this.chbStaticKey.TabIndex = 12;
            this.chbStaticKey.Text = "Use static key (less secure!)";
            this.chbStaticKey.UseVisualStyleBackColor = true;
            this.chbStaticKey.CheckedChanged += new System.EventHandler(this.chbStaticKey_CheckedChanged);
            // 
            // txtUuid
            // 
            this.txtUuid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUuid.Location = new System.Drawing.Point(130, 12);
            this.txtUuid.Name = "txtUuid";
            this.txtUuid.ReadOnly = true;
            this.txtUuid.Size = new System.Drawing.Size(360, 22);
            this.txtUuid.TabIndex = 2;
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(130, 46);
            this.txtName.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(360, 22);
            this.txtName.TabIndex = 4;
            // 
            // txtDescription
            // 
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescription.Location = new System.Drawing.Point(130, 74);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescription.Size = new System.Drawing.Size(360, 74);
            this.txtDescription.TabIndex = 6;
            // 
            // DocumentInfoForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(502, 397);
            this.Controls.Add(this.chbStaticKey);
            this.Controls.Add(this.grpStaticKey);
            this.Controls.Add(this.grpLastSave);
            this.Controls.Add(this.lblUuid);
            this.Controls.Add(this.txtUuid);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.lblDescription);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1024, 444);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 444);
            this.Name = "DocumentInfoForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Document information";
            this.Load += new System.EventHandler(this.Form_Load);
            this.grpLastSave.ResumeLayout(false);
            this.grpLastSave.PerformLayout();
            this.grpStaticKey.ResumeLayout(false);
            this.grpStaticKey.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDescription;
        private TextBoxEx txtDescription;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private TextBoxEx txtName;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblUuid;
        private TextBoxEx txtUuid;
        private System.Windows.Forms.GroupBox grpLastSave;
        private System.Windows.Forms.Label lblLastSaveTime;
        private TextBoxEx txtLastSaveTime;
        private System.Windows.Forms.Label lblLastSaveUser;
        private TextBoxEx txtLastSaveUser;
        private System.Windows.Forms.Label lblLastSaveApplication;
        private TextBoxEx txtLastSaveApplication;
        private System.Windows.Forms.GroupBox grpStaticKey;
        private System.Windows.Forms.Label lblStaticKey;
        private TextBoxEx txtStaticKey;
        private System.Windows.Forms.CheckBox chbStaticKey;
    }
}