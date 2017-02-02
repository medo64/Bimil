namespace Bimil {
    partial class PasswordDetailsForm {
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
            this.grpPasswordHistory = new System.Windows.Forms.GroupBox();
            this.btnHistoryClean = new System.Windows.Forms.Button();
            this.lblHistoryCount = new System.Windows.Forms.Label();
            this.btnHistoryShow = new System.Windows.Forms.Button();
            this.nudHistoryCount = new System.Windows.Forms.NumericUpDown();
            this.chbHistoryEnabled = new System.Windows.Forms.CheckBox();
            this.lsvHistoryPasswords = new System.Windows.Forms.ListView();
            this.lsvHistoryPasswords_Date = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lsvHistoryPasswords_Password = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mnxHistoricalPassword = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnxHistoricalPasswordCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblEditInfo = new System.Windows.Forms.Label();
            this.tip = new System.Windows.Forms.ToolTip(this.components);
            this.grpPasswordHistory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHistoryCount)).BeginInit();
            this.mnxHistoricalPassword.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpPasswordHistory
            // 
            this.grpPasswordHistory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPasswordHistory.Controls.Add(this.btnHistoryClean);
            this.grpPasswordHistory.Controls.Add(this.lblHistoryCount);
            this.grpPasswordHistory.Controls.Add(this.btnHistoryShow);
            this.grpPasswordHistory.Controls.Add(this.nudHistoryCount);
            this.grpPasswordHistory.Controls.Add(this.chbHistoryEnabled);
            this.grpPasswordHistory.Controls.Add(this.lsvHistoryPasswords);
            this.grpPasswordHistory.Location = new System.Drawing.Point(12, 12);
            this.grpPasswordHistory.Name = "grpPasswordHistory";
            this.grpPasswordHistory.Size = new System.Drawing.Size(478, 156);
            this.grpPasswordHistory.TabIndex = 1;
            this.grpPasswordHistory.TabStop = false;
            this.grpPasswordHistory.Text = "Password history";
            // 
            // btnHistoryClean
            // 
            this.btnHistoryClean.Enabled = false;
            this.btnHistoryClean.Location = new System.Drawing.Point(6, 88);
            this.btnHistoryClean.Name = "btnHistoryClean";
            this.btnHistoryClean.Size = new System.Drawing.Size(114, 25);
            this.btnHistoryClean.TabIndex = 3;
            this.btnHistoryClean.Text = "Clean";
            this.tip.SetToolTip(this.btnHistoryClean, "Clean all saved passwords.");
            this.btnHistoryClean.UseVisualStyleBackColor = true;
            this.btnHistoryClean.Click += new System.EventHandler(this.btnHistoryClean_Click);
            // 
            // lblHistoryCount
            // 
            this.lblHistoryCount.AutoSize = true;
            this.lblHistoryCount.Enabled = false;
            this.lblHistoryCount.Location = new System.Drawing.Point(6, 62);
            this.lblHistoryCount.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.lblHistoryCount.Name = "lblHistoryCount";
            this.lblHistoryCount.Size = new System.Drawing.Size(49, 17);
            this.lblHistoryCount.TabIndex = 1;
            this.lblHistoryCount.Text = "Count:";
            // 
            // btnHistoryShow
            // 
            this.btnHistoryShow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnHistoryShow.Enabled = false;
            this.btnHistoryShow.Location = new System.Drawing.Point(6, 125);
            this.btnHistoryShow.Margin = new System.Windows.Forms.Padding(3, 9, 9, 3);
            this.btnHistoryShow.Name = "btnHistoryShow";
            this.btnHistoryShow.Size = new System.Drawing.Size(114, 25);
            this.btnHistoryShow.TabIndex = 4;
            this.btnHistoryShow.Text = "Show";
            this.tip.SetToolTip(this.btnHistoryShow, "Show old passwords.");
            this.btnHistoryShow.UseVisualStyleBackColor = true;
            this.btnHistoryShow.Click += new System.EventHandler(this.btnHistoryShow_Click);
            // 
            // nudHistoryCount
            // 
            this.nudHistoryCount.Enabled = false;
            this.nudHistoryCount.Location = new System.Drawing.Point(70, 60);
            this.nudHistoryCount.Margin = new System.Windows.Forms.Padding(3, 3, 9, 3);
            this.nudHistoryCount.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nudHistoryCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudHistoryCount.Name = "nudHistoryCount";
            this.nudHistoryCount.Size = new System.Drawing.Size(50, 22);
            this.nudHistoryCount.TabIndex = 2;
            this.nudHistoryCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tip.SetToolTip(this.nudHistoryCount, "Maximum number of passwords to save.");
            this.nudHistoryCount.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // chbHistoryEnabled
            // 
            this.chbHistoryEnabled.AutoSize = true;
            this.chbHistoryEnabled.Location = new System.Drawing.Point(6, 33);
            this.chbHistoryEnabled.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.chbHistoryEnabled.Name = "chbHistoryEnabled";
            this.chbHistoryEnabled.Size = new System.Drawing.Size(82, 21);
            this.chbHistoryEnabled.TabIndex = 0;
            this.chbHistoryEnabled.Text = "Enabled";
            this.tip.SetToolTip(this.chbHistoryEnabled, "Enable saving password history.");
            this.chbHistoryEnabled.UseVisualStyleBackColor = true;
            this.chbHistoryEnabled.CheckedChanged += new System.EventHandler(this.chbHistoryEnabled_CheckedChanged);
            // 
            // lsvHistoryPasswords
            // 
            this.lsvHistoryPasswords.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsvHistoryPasswords.BackColor = System.Drawing.SystemColors.Control;
            this.lsvHistoryPasswords.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.lsvHistoryPasswords_Date,
            this.lsvHistoryPasswords_Password});
            this.lsvHistoryPasswords.ContextMenuStrip = this.mnxHistoricalPassword;
            this.lsvHistoryPasswords.Enabled = false;
            this.lsvHistoryPasswords.FullRowSelect = true;
            this.lsvHistoryPasswords.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lsvHistoryPasswords.Location = new System.Drawing.Point(132, 33);
            this.lsvHistoryPasswords.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.lsvHistoryPasswords.MultiSelect = false;
            this.lsvHistoryPasswords.Name = "lsvHistoryPasswords";
            this.lsvHistoryPasswords.ShowItemToolTips = true;
            this.lsvHistoryPasswords.Size = new System.Drawing.Size(340, 117);
            this.lsvHistoryPasswords.TabIndex = 5;
            this.lsvHistoryPasswords.UseCompatibleStateImageBehavior = false;
            this.lsvHistoryPasswords.View = System.Windows.Forms.View.Details;
            this.lsvHistoryPasswords.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.lsvHistoryPasswords_ColumnWidthChanging);
            // 
            // lsvHistoryPasswords_Date
            // 
            this.lsvHistoryPasswords_Date.Text = "Date";
            this.lsvHistoryPasswords_Date.Width = 90;
            // 
            // lsvHistoryPasswords_Password
            // 
            this.lsvHistoryPasswords_Password.Text = "Password";
            // 
            // mnxHistoricalPassword
            // 
            this.mnxHistoricalPassword.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mnxHistoricalPassword.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnxHistoricalPasswordCopy});
            this.mnxHistoricalPassword.Name = "contextMenuStrip1";
            this.mnxHistoricalPassword.Size = new System.Drawing.Size(119, 30);
            this.mnxHistoricalPassword.Opening += new System.ComponentModel.CancelEventHandler(this.mnxHistoricalPassword_Opening);
            // 
            // mnxHistoricalPasswordCopy
            // 
            this.mnxHistoricalPasswordCopy.Image = global::Bimil.Properties.Resources.btnCopy_16;
            this.mnxHistoricalPasswordCopy.Name = "mnxHistoricalPasswordCopy";
            this.mnxHistoricalPasswordCopy.Size = new System.Drawing.Size(118, 26);
            this.mnxHistoricalPasswordCopy.Text = "&Copy";
            this.mnxHistoricalPasswordCopy.Click += new System.EventHandler(this.mnxHistoricalPasswordCopy_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(400, 186);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 28);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.tip.SetToolTip(this.btnCancel, "Close without saving changes.");
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(304, 186);
            this.btnOK.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(90, 28);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.tip.SetToolTip(this.btnOK, "Save changes.");
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Visible = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblEditInfo
            // 
            this.lblEditInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblEditInfo.AutoSize = true;
            this.lblEditInfo.Location = new System.Drawing.Point(12, 200);
            this.lblEditInfo.Name = "lblEditInfo";
            this.lblEditInfo.Size = new System.Drawing.Size(293, 17);
            this.lblEditInfo.TabIndex = 3;
            this.lblEditInfo.Text = "To modify settings, you must be in edit mode.";
            // 
            // PasswordDetailsForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(502, 226);
            this.Controls.Add(this.lblEditInfo);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.grpPasswordHistory);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PasswordDetailsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Password details";
            this.Load += new System.EventHandler(this.Form_Load);
            this.Resize += new System.EventHandler(this.Form_Resize);
            this.grpPasswordHistory.ResumeLayout(false);
            this.grpPasswordHistory.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHistoryCount)).EndInit();
            this.mnxHistoricalPassword.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpPasswordHistory;
        private System.Windows.Forms.CheckBox chbHistoryEnabled;
        private System.Windows.Forms.ListView lsvHistoryPasswords;
        private System.Windows.Forms.Button btnHistoryShow;
        private System.Windows.Forms.NumericUpDown nudHistoryCount;
        private System.Windows.Forms.Label lblHistoryCount;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ColumnHeader lsvHistoryPasswords_Date;
        private System.Windows.Forms.Button btnHistoryClean;
        private System.Windows.Forms.ColumnHeader lsvHistoryPasswords_Password;
        private System.Windows.Forms.Label lblEditInfo;
        private System.Windows.Forms.ToolTip tip;
        private System.Windows.Forms.ContextMenuStrip mnxHistoricalPassword;
        private System.Windows.Forms.ToolStripMenuItem mnxHistoricalPasswordCopy;
    }
}