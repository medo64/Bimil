namespace Bimil {
    partial class PasswordGeneratorForm {
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
            this.grpInclude = new System.Windows.Forms.GroupBox();
            this.chbIncludeSpecialCharacters = new System.Windows.Forms.CheckBox();
            this.chbIncludeNumbers = new System.Windows.Forms.CheckBox();
            this.chbIncludeLowerCase = new System.Windows.Forms.CheckBox();
            this.chbIncludeUpperCase = new System.Windows.Forms.CheckBox();
            this.grpRestrictions = new System.Windows.Forms.GroupBox();
            this.chbRestrictRepeated = new System.Windows.Forms.CheckBox();
            this.chbRestrictPronounceable = new System.Windows.Forms.CheckBox();
            this.chbRestrictMovable = new System.Windows.Forms.CheckBox();
            this.chbRestrictSimilar = new System.Windows.Forms.CheckBox();
            this.lblCombinations = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.lblLength = new System.Windows.Forms.Label();
            this.txtLength = new System.Windows.Forms.TextBox();
            this.tip = new System.Windows.Forms.ToolTip(this.components);
            this.grpInclude.SuspendLayout();
            this.grpRestrictions.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpInclude
            // 
            this.grpInclude.Controls.Add(this.chbIncludeSpecialCharacters);
            this.grpInclude.Controls.Add(this.chbIncludeNumbers);
            this.grpInclude.Controls.Add(this.chbIncludeLowerCase);
            this.grpInclude.Controls.Add(this.chbIncludeUpperCase);
            this.grpInclude.Location = new System.Drawing.Point(12, 12);
            this.grpInclude.Name = "grpInclude";
            this.grpInclude.Padding = new System.Windows.Forms.Padding(3, 12, 3, 3);
            this.grpInclude.Size = new System.Drawing.Size(200, 138);
            this.grpInclude.TabIndex = 1;
            this.grpInclude.TabStop = false;
            this.grpInclude.Text = "Include";
            // 
            // chbIncludeSpecialCharacters
            // 
            this.chbIncludeSpecialCharacters.AutoSize = true;
            this.chbIncludeSpecialCharacters.Checked = true;
            this.chbIncludeSpecialCharacters.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbIncludeSpecialCharacters.Location = new System.Drawing.Point(6, 111);
            this.chbIncludeSpecialCharacters.Name = "chbIncludeSpecialCharacters";
            this.chbIncludeSpecialCharacters.Size = new System.Drawing.Size(147, 21);
            this.chbIncludeSpecialCharacters.TabIndex = 3;
            this.chbIncludeSpecialCharacters.Text = "Special characters";
            this.tip.SetToolTip(this.chbIncludeSpecialCharacters, "Include special characters");
            this.chbIncludeSpecialCharacters.UseVisualStyleBackColor = true;
            this.chbIncludeSpecialCharacters.CheckedChanged += new System.EventHandler(this.btnGenerate_Click);
            // 
            // chbIncludeNumbers
            // 
            this.chbIncludeNumbers.AutoSize = true;
            this.chbIncludeNumbers.Checked = true;
            this.chbIncludeNumbers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbIncludeNumbers.Location = new System.Drawing.Point(6, 84);
            this.chbIncludeNumbers.Name = "chbIncludeNumbers";
            this.chbIncludeNumbers.Size = new System.Drawing.Size(87, 21);
            this.chbIncludeNumbers.TabIndex = 2;
            this.chbIncludeNumbers.Text = "Numbers";
            this.tip.SetToolTip(this.chbIncludeNumbers, "Include numbers");
            this.chbIncludeNumbers.UseVisualStyleBackColor = true;
            this.chbIncludeNumbers.CheckedChanged += new System.EventHandler(this.btnGenerate_Click);
            // 
            // chbIncludeLowerCase
            // 
            this.chbIncludeLowerCase.AutoSize = true;
            this.chbIncludeLowerCase.Checked = true;
            this.chbIncludeLowerCase.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbIncludeLowerCase.Location = new System.Drawing.Point(6, 57);
            this.chbIncludeLowerCase.Name = "chbIncludeLowerCase";
            this.chbIncludeLowerCase.Size = new System.Drawing.Size(102, 21);
            this.chbIncludeLowerCase.TabIndex = 1;
            this.chbIncludeLowerCase.Text = "Lower case";
            this.tip.SetToolTip(this.chbIncludeLowerCase, "Include lower case letters");
            this.chbIncludeLowerCase.UseVisualStyleBackColor = true;
            this.chbIncludeLowerCase.CheckedChanged += new System.EventHandler(this.chbIncludeLowerCase_CheckedChanged);
            // 
            // chbIncludeUpperCase
            // 
            this.chbIncludeUpperCase.AutoSize = true;
            this.chbIncludeUpperCase.Checked = true;
            this.chbIncludeUpperCase.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbIncludeUpperCase.Location = new System.Drawing.Point(6, 30);
            this.chbIncludeUpperCase.Name = "chbIncludeUpperCase";
            this.chbIncludeUpperCase.Size = new System.Drawing.Size(103, 21);
            this.chbIncludeUpperCase.TabIndex = 0;
            this.chbIncludeUpperCase.Text = "Upper case";
            this.tip.SetToolTip(this.chbIncludeUpperCase, "Include upper case letters");
            this.chbIncludeUpperCase.UseVisualStyleBackColor = true;
            this.chbIncludeUpperCase.CheckedChanged += new System.EventHandler(this.chbIncludeUpperCase_CheckedChanged);
            // 
            // grpRestrictions
            // 
            this.grpRestrictions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpRestrictions.Controls.Add(this.chbRestrictRepeated);
            this.grpRestrictions.Controls.Add(this.chbRestrictPronounceable);
            this.grpRestrictions.Controls.Add(this.chbRestrictMovable);
            this.grpRestrictions.Controls.Add(this.chbRestrictSimilar);
            this.grpRestrictions.Location = new System.Drawing.Point(227, 12);
            this.grpRestrictions.Margin = new System.Windows.Forms.Padding(12, 3, 3, 3);
            this.grpRestrictions.Name = "grpRestrictions";
            this.grpRestrictions.Padding = new System.Windows.Forms.Padding(3, 12, 3, 3);
            this.grpRestrictions.Size = new System.Drawing.Size(200, 138);
            this.grpRestrictions.TabIndex = 2;
            this.grpRestrictions.TabStop = false;
            this.grpRestrictions.Text = "Restrictions";
            // 
            // chbRestrictRepeated
            // 
            this.chbRestrictRepeated.AutoSize = true;
            this.chbRestrictRepeated.Location = new System.Drawing.Point(6, 111);
            this.chbRestrictRepeated.Name = "chbRestrictRepeated";
            this.chbRestrictRepeated.Size = new System.Drawing.Size(163, 21);
            this.chbRestrictRepeated.TabIndex = 3;
            this.chbRestrictRepeated.Text = "Repeated characters";
            this.tip.SetToolTip(this.chbRestrictRepeated, "Don\'t allow characters to repeat immediatelly one after another");
            this.chbRestrictRepeated.UseVisualStyleBackColor = true;
            this.chbRestrictRepeated.CheckedChanged += new System.EventHandler(this.btnGenerate_Click);
            this.chbRestrictRepeated.TextChanged += new System.EventHandler(this.btnGenerate_Click);
            // 
            // chbRestrictPronounceable
            // 
            this.chbRestrictPronounceable.AutoSize = true;
            this.chbRestrictPronounceable.Location = new System.Drawing.Point(6, 84);
            this.chbRestrictPronounceable.Name = "chbRestrictPronounceable";
            this.chbRestrictPronounceable.Size = new System.Drawing.Size(126, 21);
            this.chbRestrictPronounceable.TabIndex = 2;
            this.chbRestrictPronounceable.Text = "Pronounceable";
            this.tip.SetToolTip(this.chbRestrictPronounceable, "Attempt to make password easier to pronounce by having every second letter a vowe" +
        "l");
            this.chbRestrictPronounceable.UseVisualStyleBackColor = true;
            this.chbRestrictPronounceable.CheckedChanged += new System.EventHandler(this.btnGenerate_Click);
            // 
            // chbRestrictMovable
            // 
            this.chbRestrictMovable.AutoSize = true;
            this.chbRestrictMovable.Location = new System.Drawing.Point(6, 57);
            this.chbRestrictMovable.Name = "chbRestrictMovable";
            this.chbRestrictMovable.Size = new System.Drawing.Size(122, 21);
            this.chbRestrictMovable.TabIndex = 1;
            this.chbRestrictMovable.Text = "Avoid movable";
            this.tip.SetToolTip(this.chbRestrictMovable, "Avoid characters that are often found on different spot on different keyboards");
            this.chbRestrictMovable.UseVisualStyleBackColor = true;
            this.chbRestrictMovable.CheckedChanged += new System.EventHandler(this.btnGenerate_Click);
            // 
            // chbRestrictSimilar
            // 
            this.chbRestrictSimilar.AutoSize = true;
            this.chbRestrictSimilar.Location = new System.Drawing.Point(6, 30);
            this.chbRestrictSimilar.Name = "chbRestrictSimilar";
            this.chbRestrictSimilar.Size = new System.Drawing.Size(109, 21);
            this.chbRestrictSimilar.TabIndex = 0;
            this.chbRestrictSimilar.Text = "Avoid similar";
            this.tip.SetToolTip(this.chbRestrictSimilar, "Avoid similarly looking characters (e.g. l and 1)");
            this.chbRestrictSimilar.UseVisualStyleBackColor = true;
            this.chbRestrictSimilar.CheckedChanged += new System.EventHandler(this.btnGenerate_Click);
            // 
            // lblCombinations
            // 
            this.lblCombinations.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCombinations.Location = new System.Drawing.Point(12, 159);
            this.lblCombinations.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.lblCombinations.Name = "lblCombinations";
            this.lblCombinations.Size = new System.Drawing.Size(412, 20);
            this.lblCombinations.TabIndex = 3;
            this.lblCombinations.Text = "-";
            this.lblCombinations.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Location = new System.Drawing.Point(68, 197);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.ReadOnly = true;
            this.txtPassword.Size = new System.Drawing.Size(167, 22);
            this.txtPassword.TabIndex = 6;
            this.tip.SetToolTip(this.txtPassword, "Generated password");
            // 
            // btnGenerate
            // 
            this.btnGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenerate.Location = new System.Drawing.Point(337, 194);
            this.btnGenerate.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(90, 28);
            this.btnGenerate.TabIndex = 0;
            this.btnGenerate.Text = "Generate";
            this.tip.SetToolTip(this.btnGenerate, "Generate new password");
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopy.Enabled = false;
            this.btnCopy.Location = new System.Drawing.Point(241, 194);
            this.btnCopy.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(90, 28);
            this.btnCopy.TabIndex = 7;
            this.btnCopy.Text = "Copy";
            this.tip.SetToolTip(this.btnCopy, "Copy to clipboard");
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // lblLength
            // 
            this.lblLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLength.AutoSize = true;
            this.lblLength.Location = new System.Drawing.Point(9, 177);
            this.lblLength.Name = "lblLength";
            this.lblLength.Size = new System.Drawing.Size(56, 17);
            this.lblLength.TabIndex = 4;
            this.lblLength.Text = "Length:";
            // 
            // txtLength
            // 
            this.txtLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtLength.Location = new System.Drawing.Point(12, 197);
            this.txtLength.MaxLength = 2;
            this.txtLength.Name = "txtLength";
            this.txtLength.ShortcutsEnabled = false;
            this.txtLength.Size = new System.Drawing.Size(50, 22);
            this.txtLength.TabIndex = 5;
            this.txtLength.Text = "12";
            this.txtLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tip.SetToolTip(this.txtLength, "Length of password");
            this.txtLength.TextChanged += new System.EventHandler(this.btnGenerate_Click);
            this.txtLength.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtLength_KeyDown);
            this.txtLength.Leave += new System.EventHandler(this.txtLength_Leave);
            // 
            // PasswordGeneratorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(439, 234);
            this.Controls.Add(this.lblLength);
            this.Controls.Add(this.txtLength);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblCombinations);
            this.Controls.Add(this.grpRestrictions);
            this.Controls.Add(this.grpInclude);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PasswordGeneratorForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Password generator";
            this.Load += new System.EventHandler(this.Form_Load);
            this.grpInclude.ResumeLayout(false);
            this.grpInclude.PerformLayout();
            this.grpRestrictions.ResumeLayout(false);
            this.grpRestrictions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpInclude;
        private System.Windows.Forms.CheckBox chbIncludeLowerCase;
        private System.Windows.Forms.CheckBox chbIncludeUpperCase;
        private System.Windows.Forms.CheckBox chbIncludeNumbers;
        private System.Windows.Forms.CheckBox chbIncludeSpecialCharacters;
        private System.Windows.Forms.GroupBox grpRestrictions;
        private System.Windows.Forms.CheckBox chbRestrictPronounceable;
        private System.Windows.Forms.CheckBox chbRestrictMovable;
        private System.Windows.Forms.CheckBox chbRestrictSimilar;
        private System.Windows.Forms.Label lblCombinations;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.CheckBox chbRestrictRepeated;
        private System.Windows.Forms.Label lblLength;
        private System.Windows.Forms.TextBox txtLength;
        private System.Windows.Forms.ToolTip tip;
    }
}