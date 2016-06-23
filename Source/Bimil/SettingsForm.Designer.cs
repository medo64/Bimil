namespace Bimil {
    partial class SettingsForm {
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.chbCloseOnEscape = new System.Windows.Forms.CheckBox();
            this.chbShowStart = new System.Windows.Forms.CheckBox();
            this.nudItemTimeout = new System.Windows.Forms.NumericUpDown();
            this.chbItemTimeout = new System.Windows.Forms.CheckBox();
            this.chbAppTimeout = new System.Windows.Forms.CheckBox();
            this.nudAppTimeout = new System.Windows.Forms.NumericUpDown();
            this.tip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.nudItemTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAppTimeout)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(170, 134);
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
            this.btnOK.Location = new System.Drawing.Point(74, 134);
            this.btnOK.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(90, 28);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // chbCloseOnEscape
            // 
            this.chbCloseOnEscape.AutoSize = true;
            this.chbCloseOnEscape.Location = new System.Drawing.Point(12, 39);
            this.chbCloseOnEscape.Name = "chbCloseOnEscape";
            this.chbCloseOnEscape.Size = new System.Drawing.Size(135, 21);
            this.chbCloseOnEscape.TabIndex = 2;
            this.chbCloseOnEscape.Text = "Close on escape";
            this.tip.SetToolTip(this.chbCloseOnEscape, "If true, escape will close application.");
            this.chbCloseOnEscape.UseVisualStyleBackColor = true;
            // 
            // chbShowStart
            // 
            this.chbShowStart.AutoSize = true;
            this.chbShowStart.Location = new System.Drawing.Point(12, 12);
            this.chbShowStart.Name = "chbShowStart";
            this.chbShowStart.Size = new System.Drawing.Size(145, 21);
            this.chbShowStart.TabIndex = 1;
            this.chbShowStart.Text = "Show start window";
            this.tip.SetToolTip(this.chbShowStart, "If true, Start window will be shown.");
            this.chbShowStart.UseVisualStyleBackColor = true;
            // 
            // nudItemTimeout
            // 
            this.nudItemTimeout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudItemTimeout.Enabled = false;
            this.nudItemTimeout.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudItemTimeout.Location = new System.Drawing.Point(195, 66);
            this.nudItemTimeout.Maximum = new decimal(new int[] {
            3600,
            0,
            0,
            0});
            this.nudItemTimeout.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudItemTimeout.Name = "nudItemTimeout";
            this.nudItemTimeout.Size = new System.Drawing.Size(65, 22);
            this.nudItemTimeout.TabIndex = 4;
            this.nudItemTimeout.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudItemTimeout.Value = new decimal(new int[] {
            120,
            0,
            0,
            0});
            // 
            // chbItemTimeout
            // 
            this.chbItemTimeout.AutoSize = true;
            this.chbItemTimeout.Location = new System.Drawing.Point(12, 67);
            this.chbItemTimeout.Name = "chbItemTimeout";
            this.chbItemTimeout.Size = new System.Drawing.Size(106, 21);
            this.chbItemTimeout.TabIndex = 3;
            this.chbItemTimeout.Text = "Item timeout";
            this.tip.SetToolTip(this.chbItemTimeout, "Time in seconds for item window to automatically close if it loses focus.");
            this.chbItemTimeout.UseVisualStyleBackColor = true;
            this.chbItemTimeout.CheckedChanged += new System.EventHandler(this.chbItemTimeout_CheckedChanged);
            // 
            // chbAppTimeout
            // 
            this.chbAppTimeout.AutoSize = true;
            this.chbAppTimeout.Location = new System.Drawing.Point(12, 95);
            this.chbAppTimeout.Name = "chbAppTimeout";
            this.chbAppTimeout.Size = new System.Drawing.Size(149, 21);
            this.chbAppTimeout.TabIndex = 5;
            this.chbAppTimeout.Text = "Application timeout";
            this.tip.SetToolTip(this.chbAppTimeout, "Time in seconds for main window to automatically close if it loses focus.");
            this.chbAppTimeout.UseVisualStyleBackColor = true;
            this.chbAppTimeout.CheckedChanged += new System.EventHandler(this.chbAppTimeout_CheckedChanged);
            // 
            // nudAppTimeout
            // 
            this.nudAppTimeout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudAppTimeout.Enabled = false;
            this.nudAppTimeout.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudAppTimeout.Location = new System.Drawing.Point(195, 94);
            this.nudAppTimeout.Maximum = new decimal(new int[] {
            3600,
            0,
            0,
            0});
            this.nudAppTimeout.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudAppTimeout.Name = "nudAppTimeout";
            this.nudAppTimeout.Size = new System.Drawing.Size(65, 22);
            this.nudAppTimeout.TabIndex = 6;
            this.nudAppTimeout.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudAppTimeout.Value = new decimal(new int[] {
            900,
            0,
            0,
            0});
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(272, 174);
            this.Controls.Add(this.nudAppTimeout);
            this.Controls.Add(this.nudItemTimeout);
            this.Controls.Add(this.chbAppTimeout);
            this.Controls.Add(this.chbItemTimeout);
            this.Controls.Add(this.chbShowStart);
            this.Controls.Add(this.chbCloseOnEscape);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudItemTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAppTimeout)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox chbCloseOnEscape;
        private System.Windows.Forms.CheckBox chbShowStart;
        private System.Windows.Forms.NumericUpDown nudItemTimeout;
        private System.Windows.Forms.CheckBox chbItemTimeout;
        private System.Windows.Forms.CheckBox chbAppTimeout;
        private System.Windows.Forms.NumericUpDown nudAppTimeout;
        private System.Windows.Forms.ToolTip tip;
    }
}