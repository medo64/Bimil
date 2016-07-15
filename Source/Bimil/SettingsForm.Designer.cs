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
            this.chbItemTimeout = new System.Windows.Forms.CheckBox();
            this.chbAppTimeout = new System.Windows.Forms.CheckBox();
            this.tip = new System.Windows.Forms.ToolTip(this.components);
            this.chbPasswordSafeWarnings = new System.Windows.Forms.CheckBox();
            this.txtItemTimeout = new System.Windows.Forms.TextBox();
            this.txtAppTimeout = new System.Windows.Forms.TextBox();
            this.chbEditableByDefault = new System.Windows.Forms.CheckBox();
            this.chbAutoCloseSave = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(220, 216);
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
            this.btnOK.Location = new System.Drawing.Point(124, 216);
            this.btnOK.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(90, 28);
            this.btnOK.TabIndex = 10;
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
            // chbItemTimeout
            // 
            this.chbItemTimeout.AutoSize = true;
            this.chbItemTimeout.Location = new System.Drawing.Point(12, 122);
            this.chbItemTimeout.Name = "chbItemTimeout";
            this.chbItemTimeout.Size = new System.Drawing.Size(183, 21);
            this.chbItemTimeout.TabIndex = 5;
            this.chbItemTimeout.Text = "Auto-close entry timeout";
            this.tip.SetToolTip(this.chbItemTimeout, "Time in seconds for item window to automatically close if it loses focus.");
            this.chbItemTimeout.UseVisualStyleBackColor = true;
            this.chbItemTimeout.CheckedChanged += new System.EventHandler(this.chbItemTimeout_CheckedChanged);
            // 
            // chbAppTimeout
            // 
            this.chbAppTimeout.AutoSize = true;
            this.chbAppTimeout.Location = new System.Drawing.Point(12, 150);
            this.chbAppTimeout.Name = "chbAppTimeout";
            this.chbAppTimeout.Size = new System.Drawing.Size(219, 21);
            this.chbAppTimeout.TabIndex = 7;
            this.chbAppTimeout.Text = "Auto-close application timeout";
            this.tip.SetToolTip(this.chbAppTimeout, "Time in seconds for main window to automatically close if it loses focus.");
            this.chbAppTimeout.UseVisualStyleBackColor = true;
            this.chbAppTimeout.CheckedChanged += new System.EventHandler(this.chbAppTimeout_CheckedChanged);
            // 
            // chbPasswordSafeWarnings
            // 
            this.chbPasswordSafeWarnings.AutoSize = true;
            this.chbPasswordSafeWarnings.Location = new System.Drawing.Point(12, 93);
            this.chbPasswordSafeWarnings.Name = "chbPasswordSafeWarnings";
            this.chbPasswordSafeWarnings.Size = new System.Drawing.Size(261, 21);
            this.chbPasswordSafeWarnings.TabIndex = 4;
            this.chbPasswordSafeWarnings.Text = "PasswordSafe compatibility warnings";
            this.tip.SetToolTip(this.chbPasswordSafeWarnings, "If true, warning will be shown upon adding fields not compatible with PasswordSaf" +
        "e.");
            this.chbPasswordSafeWarnings.UseVisualStyleBackColor = true;
            // 
            // txtItemTimeout
            // 
            this.txtItemTimeout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtItemTimeout.Enabled = false;
            this.txtItemTimeout.Location = new System.Drawing.Point(250, 120);
            this.txtItemTimeout.MaxLength = 4;
            this.txtItemTimeout.Name = "txtItemTimeout";
            this.txtItemTimeout.ShortcutsEnabled = false;
            this.txtItemTimeout.Size = new System.Drawing.Size(60, 22);
            this.txtItemTimeout.TabIndex = 6;
            this.txtItemTimeout.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tip.SetToolTip(this.txtItemTimeout, "Value between 10 and 3600 seconds.");
            this.txtItemTimeout.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTimeout_KeyDown);
            this.txtItemTimeout.Leave += new System.EventHandler(this.txtItemTimeout_Leave);
            // 
            // txtAppTimeout
            // 
            this.txtAppTimeout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAppTimeout.Enabled = false;
            this.txtAppTimeout.Location = new System.Drawing.Point(250, 148);
            this.txtAppTimeout.MaxLength = 4;
            this.txtAppTimeout.Name = "txtAppTimeout";
            this.txtAppTimeout.ShortcutsEnabled = false;
            this.txtAppTimeout.Size = new System.Drawing.Size(60, 22);
            this.txtAppTimeout.TabIndex = 8;
            this.txtAppTimeout.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tip.SetToolTip(this.txtAppTimeout, "Value between 10 and 3600 seconds.");
            this.txtAppTimeout.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTimeout_KeyDown);
            this.txtAppTimeout.Leave += new System.EventHandler(this.txtAppTimeout_Leave);
            // 
            // chbEditableByDefault
            // 
            this.chbEditableByDefault.AutoSize = true;
            this.chbEditableByDefault.Location = new System.Drawing.Point(12, 66);
            this.chbEditableByDefault.Name = "chbEditableByDefault";
            this.chbEditableByDefault.Size = new System.Drawing.Size(147, 21);
            this.chbEditableByDefault.TabIndex = 3;
            this.chbEditableByDefault.Text = "Editable by default";
            this.tip.SetToolTip(this.chbEditableByDefault, "If checked, fields will be editable by default");
            this.chbEditableByDefault.UseVisualStyleBackColor = true;
            // 
            // chbAutoCloseSave
            // 
            this.chbAutoCloseSave.AutoSize = true;
            this.chbAutoCloseSave.Location = new System.Drawing.Point(12, 177);
            this.chbAutoCloseSave.Name = "chbAutoCloseSave";
            this.chbAutoCloseSave.Size = new System.Drawing.Size(152, 21);
            this.chbAutoCloseSave.TabIndex = 9;
            this.chbAutoCloseSave.Text = "Save on auto-close";
            this.tip.SetToolTip(this.chbAutoCloseSave, "If checked, any pending changes will be saved on timeout.");
            this.chbAutoCloseSave.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(322, 256);
            this.Controls.Add(this.chbAutoCloseSave);
            this.Controls.Add(this.chbEditableByDefault);
            this.Controls.Add(this.txtAppTimeout);
            this.Controls.Add(this.txtItemTimeout);
            this.Controls.Add(this.chbPasswordSafeWarnings);
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
            this.Load += new System.EventHandler(this.Form_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox chbCloseOnEscape;
        private System.Windows.Forms.CheckBox chbShowStart;
        private System.Windows.Forms.CheckBox chbItemTimeout;
        private System.Windows.Forms.CheckBox chbAppTimeout;
        private System.Windows.Forms.ToolTip tip;
        private System.Windows.Forms.CheckBox chbPasswordSafeWarnings;
        private System.Windows.Forms.TextBox txtItemTimeout;
        private System.Windows.Forms.TextBox txtAppTimeout;
        private System.Windows.Forms.CheckBox chbEditableByDefault;
        private System.Windows.Forms.CheckBox chbAutoCloseSave;
    }
}