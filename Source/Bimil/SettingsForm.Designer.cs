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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
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
            this.chbShowCommonPasswordWarnings = new System.Windows.Forms.CheckBox();
            this.chbSavePasswordHistoryByDefault = new System.Windows.Forms.CheckBox();
            this.chbCheckWeakPasswordAtHibp = new System.Windows.Forms.CheckBox();
            this.txtClearClipboardTimeout = new System.Windows.Forms.TextBox();
            this.chbClearClipboardTimeout = new System.Windows.Forms.CheckBox();
            this.chbClearOnlySensitveItems = new System.Windows.Forms.CheckBox();
            this.chbLoadLast = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(220, 378);
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
            this.btnOK.Location = new System.Drawing.Point(124, 378);
            this.btnOK.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(90, 28);
            this.btnOK.TabIndex = 17;
            this.btnOK.Text = "OK";
            this.tip.SetToolTip(this.btnOK, "Save changes.");
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // chbCloseOnEscape
            // 
            this.chbCloseOnEscape.AutoSize = true;
            this.chbCloseOnEscape.Location = new System.Drawing.Point(12, 66);
            this.chbCloseOnEscape.Name = "chbCloseOnEscape";
            this.chbCloseOnEscape.Size = new System.Drawing.Size(135, 21);
            this.chbCloseOnEscape.TabIndex = 3;
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
            this.chbShowStart.CheckedChanged += new System.EventHandler(this.chbShowStart_CheckedChanged);
            // 
            // chbItemTimeout
            // 
            this.chbItemTimeout.AutoSize = true;
            this.chbItemTimeout.Location = new System.Drawing.Point(12, 284);
            this.chbItemTimeout.Name = "chbItemTimeout";
            this.chbItemTimeout.Size = new System.Drawing.Size(183, 21);
            this.chbItemTimeout.TabIndex = 12;
            this.chbItemTimeout.Text = "Auto-close entry timeout";
            this.tip.SetToolTip(this.chbItemTimeout, "Time in seconds for item window to automatically close if it loses focus.");
            this.chbItemTimeout.UseVisualStyleBackColor = true;
            this.chbItemTimeout.CheckedChanged += new System.EventHandler(this.chbItemTimeout_CheckedChanged);
            // 
            // chbAppTimeout
            // 
            this.chbAppTimeout.AutoSize = true;
            this.chbAppTimeout.Location = new System.Drawing.Point(12, 312);
            this.chbAppTimeout.Name = "chbAppTimeout";
            this.chbAppTimeout.Size = new System.Drawing.Size(219, 21);
            this.chbAppTimeout.TabIndex = 14;
            this.chbAppTimeout.Text = "Auto-close application timeout";
            this.tip.SetToolTip(this.chbAppTimeout, "Time in seconds for main window to automatically close if it loses focus.");
            this.chbAppTimeout.UseVisualStyleBackColor = true;
            this.chbAppTimeout.CheckedChanged += new System.EventHandler(this.chbAppTimeout_CheckedChanged);
            // 
            // chbPasswordSafeWarnings
            // 
            this.chbPasswordSafeWarnings.AutoSize = true;
            this.chbPasswordSafeWarnings.Location = new System.Drawing.Point(12, 147);
            this.chbPasswordSafeWarnings.Name = "chbPasswordSafeWarnings";
            this.chbPasswordSafeWarnings.Size = new System.Drawing.Size(261, 21);
            this.chbPasswordSafeWarnings.TabIndex = 6;
            this.chbPasswordSafeWarnings.Text = "PasswordSafe compatibility warnings";
            this.tip.SetToolTip(this.chbPasswordSafeWarnings, "If true, warning will be shown upon adding fields not compatible with PasswordSaf" +
        "e.");
            this.chbPasswordSafeWarnings.UseVisualStyleBackColor = true;
            // 
            // txtItemTimeout
            // 
            this.txtItemTimeout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtItemTimeout.Enabled = false;
            this.txtItemTimeout.Location = new System.Drawing.Point(250, 282);
            this.txtItemTimeout.MaxLength = 4;
            this.txtItemTimeout.Name = "txtItemTimeout";
            this.txtItemTimeout.ShortcutsEnabled = false;
            this.txtItemTimeout.Size = new System.Drawing.Size(60, 22);
            this.txtItemTimeout.TabIndex = 13;
            this.txtItemTimeout.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tip.SetToolTip(this.txtItemTimeout, "Value between 10 and 3600 seconds.");
            this.txtItemTimeout.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTimeout_KeyDown);
            this.txtItemTimeout.Leave += new System.EventHandler(this.txtItemTimeout_Leave);
            // 
            // txtAppTimeout
            // 
            this.txtAppTimeout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAppTimeout.Enabled = false;
            this.txtAppTimeout.Location = new System.Drawing.Point(250, 310);
            this.txtAppTimeout.MaxLength = 4;
            this.txtAppTimeout.Name = "txtAppTimeout";
            this.txtAppTimeout.ShortcutsEnabled = false;
            this.txtAppTimeout.Size = new System.Drawing.Size(60, 22);
            this.txtAppTimeout.TabIndex = 15;
            this.txtAppTimeout.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tip.SetToolTip(this.txtAppTimeout, "Value between 10 and 3600 seconds.");
            this.txtAppTimeout.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTimeout_KeyDown);
            this.txtAppTimeout.Leave += new System.EventHandler(this.txtAppTimeout_Leave);
            // 
            // chbEditableByDefault
            // 
            this.chbEditableByDefault.AutoSize = true;
            this.chbEditableByDefault.Location = new System.Drawing.Point(12, 93);
            this.chbEditableByDefault.Name = "chbEditableByDefault";
            this.chbEditableByDefault.Size = new System.Drawing.Size(147, 21);
            this.chbEditableByDefault.TabIndex = 4;
            this.chbEditableByDefault.Text = "Editable by default";
            this.tip.SetToolTip(this.chbEditableByDefault, "If checked, fields will be editable by default");
            this.chbEditableByDefault.UseVisualStyleBackColor = true;
            // 
            // chbAutoCloseSave
            // 
            this.chbAutoCloseSave.AutoSize = true;
            this.chbAutoCloseSave.Location = new System.Drawing.Point(12, 339);
            this.chbAutoCloseSave.Name = "chbAutoCloseSave";
            this.chbAutoCloseSave.Size = new System.Drawing.Size(152, 21);
            this.chbAutoCloseSave.TabIndex = 16;
            this.chbAutoCloseSave.Text = "Save on auto-close";
            this.tip.SetToolTip(this.chbAutoCloseSave, "If checked, any pending changes will be saved on timeout.");
            this.chbAutoCloseSave.UseVisualStyleBackColor = true;
            // 
            // chbShowCommonPasswordWarnings
            // 
            this.chbShowCommonPasswordWarnings.AutoSize = true;
            this.chbShowCommonPasswordWarnings.Location = new System.Drawing.Point(12, 120);
            this.chbShowCommonPasswordWarnings.Name = "chbShowCommonPasswordWarnings";
            this.chbShowCommonPasswordWarnings.Size = new System.Drawing.Size(209, 21);
            this.chbShowCommonPasswordWarnings.TabIndex = 5;
            this.chbShowCommonPasswordWarnings.Text = "Common password warnings";
            this.tip.SetToolTip(this.chbShowCommonPasswordWarnings, "If true, warning will be shown when password is in the list of common passwords.");
            this.chbShowCommonPasswordWarnings.UseVisualStyleBackColor = true;
            // 
            // chbSavePasswordHistoryByDefault
            // 
            this.chbSavePasswordHistoryByDefault.AutoSize = true;
            this.chbSavePasswordHistoryByDefault.Location = new System.Drawing.Point(12, 201);
            this.chbSavePasswordHistoryByDefault.Name = "chbSavePasswordHistoryByDefault";
            this.chbSavePasswordHistoryByDefault.Size = new System.Drawing.Size(269, 21);
            this.chbSavePasswordHistoryByDefault.TabIndex = 8;
            this.chbSavePasswordHistoryByDefault.Text = "Save password history for new entries";
            this.tip.SetToolTip(this.chbSavePasswordHistoryByDefault, "If checked, all new items will have password history saving turned on");
            this.chbSavePasswordHistoryByDefault.UseVisualStyleBackColor = true;
            // 
            // chbCheckWeakPasswordAtHibp
            // 
            this.chbCheckWeakPasswordAtHibp.AutoSize = true;
            this.chbCheckWeakPasswordAtHibp.Location = new System.Drawing.Point(12, 174);
            this.chbCheckWeakPasswordAtHibp.Name = "chbCheckWeakPasswordAtHibp";
            this.chbCheckWeakPasswordAtHibp.Size = new System.Drawing.Size(289, 21);
            this.chbCheckWeakPasswordAtHibp.TabIndex = 7;
            this.chbCheckWeakPasswordAtHibp.Text = "Check passwords at Have I been pwned?";
            this.tip.SetToolTip(this.chbCheckWeakPasswordAtHibp, resources.GetString("chbCheckWeakPasswordAtHibp.ToolTip"));
            this.chbCheckWeakPasswordAtHibp.UseVisualStyleBackColor = true;
            // 
            // txtClearClipboardTimeout
            // 
            this.txtClearClipboardTimeout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtClearClipboardTimeout.Enabled = false;
            this.txtClearClipboardTimeout.Location = new System.Drawing.Point(250, 226);
            this.txtClearClipboardTimeout.MaxLength = 4;
            this.txtClearClipboardTimeout.Name = "txtClearClipboardTimeout";
            this.txtClearClipboardTimeout.ShortcutsEnabled = false;
            this.txtClearClipboardTimeout.Size = new System.Drawing.Size(60, 22);
            this.txtClearClipboardTimeout.TabIndex = 10;
            this.txtClearClipboardTimeout.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tip.SetToolTip(this.txtClearClipboardTimeout, "Value between 10 and 3600 seconds.");
            this.txtClearClipboardTimeout.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTimeout_KeyDown);
            this.txtClearClipboardTimeout.Leave += new System.EventHandler(this.txtClearClipboardTimeout_Leave);
            // 
            // chbClearClipboardTimeout
            // 
            this.chbClearClipboardTimeout.AutoSize = true;
            this.chbClearClipboardTimeout.Location = new System.Drawing.Point(12, 228);
            this.chbClearClipboardTimeout.Name = "chbClearClipboardTimeout";
            this.chbClearClipboardTimeout.Size = new System.Drawing.Size(175, 21);
            this.chbClearClipboardTimeout.TabIndex = 9;
            this.chbClearClipboardTimeout.Text = "Clear clipboard timeout";
            this.tip.SetToolTip(this.chbClearClipboardTimeout, "Time in seconds before clipboard is cleared after copy.");
            this.chbClearClipboardTimeout.UseVisualStyleBackColor = true;
            this.chbClearClipboardTimeout.CheckedChanged += new System.EventHandler(this.chbClearClipboardTimeout_CheckedChanged);
            // 
            // chbClearOnlySensitveItems
            // 
            this.chbClearOnlySensitveItems.AutoSize = true;
            this.chbClearOnlySensitveItems.Location = new System.Drawing.Point(32, 255);
            this.chbClearOnlySensitveItems.Name = "chbClearOnlySensitveItems";
            this.chbClearOnlySensitveItems.Size = new System.Drawing.Size(192, 21);
            this.chbClearOnlySensitveItems.TabIndex = 11;
            this.chbClearOnlySensitveItems.Text = "Clean only sensitive items";
            this.tip.SetToolTip(this.chbClearOnlySensitveItems, "If checked, clipboard is only cleared from sensitive items like password and two-" +
        "factor codes.");
            this.chbClearOnlySensitveItems.UseVisualStyleBackColor = true;
            // 
            // chbLoadLast
            // 
            this.chbLoadLast.AutoSize = true;
            this.chbLoadLast.Location = new System.Drawing.Point(12, 39);
            this.chbLoadLast.Name = "chbLoadLast";
            this.chbLoadLast.Size = new System.Drawing.Size(227, 21);
            this.chbLoadLast.TabIndex = 2;
            this.chbLoadLast.Text = "Automatically load last file used";
            this.tip.SetToolTip(this.chbLoadLast, "If true, the last used file will be loaded automatically.");
            this.chbLoadLast.UseVisualStyleBackColor = true;
            this.chbLoadLast.CheckedChanged += new System.EventHandler(this.chbLoadLast_CheckedChanged);
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(322, 418);
            this.Controls.Add(this.chbLoadLast);
            this.Controls.Add(this.chbClearOnlySensitveItems);
            this.Controls.Add(this.txtClearClipboardTimeout);
            this.Controls.Add(this.chbClearClipboardTimeout);
            this.Controls.Add(this.chbCheckWeakPasswordAtHibp);
            this.Controls.Add(this.chbSavePasswordHistoryByDefault);
            this.Controls.Add(this.chbShowCommonPasswordWarnings);
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
        private System.Windows.Forms.CheckBox chbShowCommonPasswordWarnings;
        private System.Windows.Forms.CheckBox chbSavePasswordHistoryByDefault;
        private System.Windows.Forms.CheckBox chbCheckWeakPasswordAtHibp;
        private System.Windows.Forms.TextBox txtClearClipboardTimeout;
        private System.Windows.Forms.CheckBox chbClearClipboardTimeout;
        private System.Windows.Forms.CheckBox chbClearOnlySensitveItems;
        private System.Windows.Forms.CheckBox chbLoadLast;
    }
}
