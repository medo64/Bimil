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
            this.lblCombinations = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.tip = new System.Windows.Forms.ToolTip(this.components);
            this.chbIncludeSpecialCharacters = new System.Windows.Forms.CheckBox();
            this.chbIncludeNumbers = new System.Windows.Forms.CheckBox();
            this.chbIncludeLowerCase = new System.Windows.Forms.CheckBox();
            this.chbIncludeUpperCase = new System.Windows.Forms.CheckBox();
            this.chbRestrictRepeated = new System.Windows.Forms.CheckBox();
            this.chbRestrictPronounceable = new System.Windows.Forms.CheckBox();
            this.chbRestrictMovable = new System.Windows.Forms.CheckBox();
            this.chbRestrictSimilar = new System.Windows.Forms.CheckBox();
            this.txtLength = new System.Windows.Forms.TextBox();
            this.txtWordCount = new System.Windows.Forms.TextBox();
            this.chbWordIncludeSpecialCharacter = new System.Windows.Forms.CheckBox();
            this.chbWordIncludeNumber = new System.Windows.Forms.CheckBox();
            this.chbWordIncludeIncomplete = new System.Windows.Forms.CheckBox();
            this.chbWordIncludeUpperCase = new System.Windows.Forms.CheckBox();
            this.chbWordRestrictAddSpace = new System.Windows.Forms.CheckBox();
            this.chbWordRestrictBreak = new System.Windows.Forms.CheckBox();
            this.chbWordRestrictTitleCase = new System.Windows.Forms.CheckBox();
            this.tabStyle = new System.Windows.Forms.TabControl();
            this.tabStyle_Words = new System.Windows.Forms.TabPage();
            this.grpWordRestrict = new System.Windows.Forms.GroupBox();
            this.lblWordCount = new System.Windows.Forms.Label();
            this.grpWordInclude = new System.Windows.Forms.GroupBox();
            this.tabStyle_Classic = new System.Windows.Forms.TabPage();
            this.lblLength = new System.Windows.Forms.Label();
            this.grpRestrictions = new System.Windows.Forms.GroupBox();
            this.grpInclude = new System.Windows.Forms.GroupBox();
            this.tabStyle.SuspendLayout();
            this.tabStyle_Words.SuspendLayout();
            this.grpWordRestrict.SuspendLayout();
            this.grpWordInclude.SuspendLayout();
            this.tabStyle_Classic.SuspendLayout();
            this.grpRestrictions.SuspendLayout();
            this.grpInclude.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblCombinations
            // 
            this.lblCombinations.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCombinations.Location = new System.Drawing.Point(12, 228);
            this.lblCombinations.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.lblCombinations.Name = "lblCombinations";
            this.lblCombinations.Size = new System.Drawing.Size(435, 20);
            this.lblCombinations.TabIndex = 2;
            this.lblCombinations.Text = "-";
            this.lblCombinations.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Location = new System.Drawing.Point(12, 251);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.ReadOnly = true;
            this.txtPassword.Size = new System.Drawing.Size(435, 22);
            this.txtPassword.TabIndex = 3;
            this.txtPassword.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tip.SetToolTip(this.txtPassword, "Generated password");
            // 
            // btnGenerate
            // 
            this.btnGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenerate.Location = new System.Drawing.Point(261, 291);
            this.btnGenerate.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(90, 28);
            this.btnGenerate.TabIndex = 4;
            this.btnGenerate.Text = "Generate";
            this.tip.SetToolTip(this.btnGenerate, "Generate new password");
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopy.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnCopy.Enabled = false;
            this.btnCopy.Location = new System.Drawing.Point(357, 291);
            this.btnCopy.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(90, 28);
            this.btnCopy.TabIndex = 0;
            this.btnCopy.Text = "Copy";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
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
            // txtLength
            // 
            this.txtLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtLength.Location = new System.Drawing.Point(156, 150);
            this.txtLength.MaxLength = 2;
            this.txtLength.Name = "txtLength";
            this.txtLength.ShortcutsEnabled = false;
            this.txtLength.Size = new System.Drawing.Size(50, 22);
            this.txtLength.TabIndex = 3;
            this.txtLength.Text = "12";
            this.txtLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tip.SetToolTip(this.txtLength, "Length of password (1-99).");
            this.txtLength.TextChanged += new System.EventHandler(this.btnGenerate_Click);
            this.txtLength.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtNumber_KeyDown);
            this.txtLength.Leave += new System.EventHandler(this.txtLength_Leave);
            // 
            // txtWordCount
            // 
            this.txtWordCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtWordCount.Location = new System.Drawing.Point(156, 150);
            this.txtWordCount.MaxLength = 1;
            this.txtWordCount.Name = "txtWordCount";
            this.txtWordCount.ShortcutsEnabled = false;
            this.txtWordCount.Size = new System.Drawing.Size(50, 22);
            this.txtWordCount.TabIndex = 2;
            this.txtWordCount.Text = "4";
            this.txtWordCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tip.SetToolTip(this.txtWordCount, "Number of words to use (1-9)");
            this.txtWordCount.TextChanged += new System.EventHandler(this.btnGenerate_Click);
            this.txtWordCount.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtNumber_KeyDown);
            this.txtWordCount.Leave += new System.EventHandler(this.txtWordCount_Leave);
            // 
            // chbWordIncludeSpecialCharacter
            // 
            this.chbWordIncludeSpecialCharacter.AutoSize = true;
            this.chbWordIncludeSpecialCharacter.Location = new System.Drawing.Point(6, 84);
            this.chbWordIncludeSpecialCharacter.Name = "chbWordIncludeSpecialCharacter";
            this.chbWordIncludeSpecialCharacter.Size = new System.Drawing.Size(140, 21);
            this.chbWordIncludeSpecialCharacter.TabIndex = 2;
            this.chbWordIncludeSpecialCharacter.Text = "Special character";
            this.tip.SetToolTip(this.chbWordIncludeSpecialCharacter, "Include special character in one of the words");
            this.chbWordIncludeSpecialCharacter.UseVisualStyleBackColor = true;
            this.chbWordIncludeSpecialCharacter.CheckedChanged += new System.EventHandler(this.btnGenerate_Click);
            // 
            // chbWordIncludeNumber
            // 
            this.chbWordIncludeNumber.AutoSize = true;
            this.chbWordIncludeNumber.Checked = true;
            this.chbWordIncludeNumber.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbWordIncludeNumber.Location = new System.Drawing.Point(6, 30);
            this.chbWordIncludeNumber.Name = "chbWordIncludeNumber";
            this.chbWordIncludeNumber.Size = new System.Drawing.Size(80, 21);
            this.chbWordIncludeNumber.TabIndex = 0;
            this.chbWordIncludeNumber.Text = "Number";
            this.tip.SetToolTip(this.chbWordIncludeNumber, "Include number in one of words");
            this.chbWordIncludeNumber.UseVisualStyleBackColor = true;
            this.chbWordIncludeNumber.CheckedChanged += new System.EventHandler(this.btnGenerate_Click);
            // 
            // chbWordIncludeIncomplete
            // 
            this.chbWordIncludeIncomplete.AutoSize = true;
            this.chbWordIncludeIncomplete.Location = new System.Drawing.Point(6, 111);
            this.chbWordIncludeIncomplete.Name = "chbWordIncludeIncomplete";
            this.chbWordIncludeIncomplete.Size = new System.Drawing.Size(132, 21);
            this.chbWordIncludeIncomplete.TabIndex = 3;
            this.chbWordIncludeIncomplete.Text = "Incomplete word";
            this.tip.SetToolTip(this.chbWordIncludeIncomplete, "Remove a character from one of the words.");
            this.chbWordIncludeIncomplete.UseVisualStyleBackColor = true;
            this.chbWordIncludeIncomplete.CheckedChanged += new System.EventHandler(this.btnGenerate_Click);
            // 
            // chbWordIncludeUpperCase
            // 
            this.chbWordIncludeUpperCase.AutoSize = true;
            this.chbWordIncludeUpperCase.Location = new System.Drawing.Point(6, 57);
            this.chbWordIncludeUpperCase.Name = "chbWordIncludeUpperCase";
            this.chbWordIncludeUpperCase.Size = new System.Drawing.Size(139, 21);
            this.chbWordIncludeUpperCase.TabIndex = 1;
            this.chbWordIncludeUpperCase.Text = "Upper case letter";
            this.tip.SetToolTip(this.chbWordIncludeUpperCase, "Include upper case letter in one of words");
            this.chbWordIncludeUpperCase.UseVisualStyleBackColor = true;
            this.chbWordIncludeUpperCase.CheckedChanged += new System.EventHandler(this.btnGenerate_Click);
            // 
            // chbWordRestrictAddSpace
            // 
            this.chbWordRestrictAddSpace.AutoSize = true;
            this.chbWordRestrictAddSpace.Location = new System.Drawing.Point(6, 111);
            this.chbWordRestrictAddSpace.Name = "chbWordRestrictAddSpace";
            this.chbWordRestrictAddSpace.Size = new System.Drawing.Size(108, 21);
            this.chbWordRestrictAddSpace.TabIndex = 2;
            this.chbWordRestrictAddSpace.Text = "Add spacing";
            this.tip.SetToolTip(this.chbWordRestrictAddSpace, "Adds spacing between words");
            this.chbWordRestrictAddSpace.UseVisualStyleBackColor = true;
            this.chbWordRestrictAddSpace.CheckedChanged += new System.EventHandler(this.btnGenerate_Click);
            // 
            // chbWordRestrictBreak
            // 
            this.chbWordRestrictBreak.AutoSize = true;
            this.chbWordRestrictBreak.Checked = true;
            this.chbWordRestrictBreak.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbWordRestrictBreak.Location = new System.Drawing.Point(6, 57);
            this.chbWordRestrictBreak.Name = "chbWordRestrictBreak";
            this.chbWordRestrictBreak.Size = new System.Drawing.Size(157, 21);
            this.chbWordRestrictBreak.TabIndex = 1;
            this.chbWordRestrictBreak.Text = "Don\'t break up word";
            this.tip.SetToolTip(this.chbWordRestrictBreak, "Modifications to the words are done at the begining or at the end of word only.");
            this.chbWordRestrictBreak.UseVisualStyleBackColor = true;
            this.chbWordRestrictBreak.CheckedChanged += new System.EventHandler(this.btnGenerate_Click);
            // 
            // chbWordRestrictTitleCase
            // 
            this.chbWordRestrictTitleCase.AutoSize = true;
            this.chbWordRestrictTitleCase.Checked = true;
            this.chbWordRestrictTitleCase.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbWordRestrictTitleCase.Location = new System.Drawing.Point(6, 30);
            this.chbWordRestrictTitleCase.Name = "chbWordRestrictTitleCase";
            this.chbWordRestrictTitleCase.Size = new System.Drawing.Size(91, 21);
            this.chbWordRestrictTitleCase.TabIndex = 0;
            this.chbWordRestrictTitleCase.Text = "Title case";
            this.tip.SetToolTip(this.chbWordRestrictTitleCase, "First character of every word will be capitalized.");
            this.chbWordRestrictTitleCase.UseVisualStyleBackColor = true;
            this.chbWordRestrictTitleCase.CheckedChanged += new System.EventHandler(this.btnGenerate_Click);
            // 
            // tabStyle
            // 
            this.tabStyle.Controls.Add(this.tabStyle_Words);
            this.tabStyle.Controls.Add(this.tabStyle_Classic);
            this.tabStyle.Location = new System.Drawing.Point(12, 12);
            this.tabStyle.Name = "tabStyle";
            this.tabStyle.SelectedIndex = 0;
            this.tabStyle.Size = new System.Drawing.Size(435, 207);
            this.tabStyle.TabIndex = 1;
            this.tabStyle.SelectedIndexChanged += new System.EventHandler(this.btnGenerate_Click);
            // 
            // tabStyle_Words
            // 
            this.tabStyle_Words.Controls.Add(this.grpWordRestrict);
            this.tabStyle_Words.Controls.Add(this.lblWordCount);
            this.tabStyle_Words.Controls.Add(this.txtWordCount);
            this.tabStyle_Words.Controls.Add(this.grpWordInclude);
            this.tabStyle_Words.Location = new System.Drawing.Point(4, 25);
            this.tabStyle_Words.Name = "tabStyle_Words";
            this.tabStyle_Words.Padding = new System.Windows.Forms.Padding(3);
            this.tabStyle_Words.Size = new System.Drawing.Size(427, 178);
            this.tabStyle_Words.TabIndex = 0;
            this.tabStyle_Words.Text = "Word-based";
            this.tabStyle_Words.UseVisualStyleBackColor = true;
            // 
            // grpWordRestrict
            // 
            this.grpWordRestrict.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpWordRestrict.Controls.Add(this.chbWordRestrictTitleCase);
            this.grpWordRestrict.Controls.Add(this.chbWordRestrictBreak);
            this.grpWordRestrict.Controls.Add(this.chbWordRestrictAddSpace);
            this.grpWordRestrict.Location = new System.Drawing.Point(221, 6);
            this.grpWordRestrict.Margin = new System.Windows.Forms.Padding(12, 3, 3, 3);
            this.grpWordRestrict.Name = "grpWordRestrict";
            this.grpWordRestrict.Padding = new System.Windows.Forms.Padding(3, 12, 3, 3);
            this.grpWordRestrict.Size = new System.Drawing.Size(200, 138);
            this.grpWordRestrict.TabIndex = 1;
            this.grpWordRestrict.TabStop = false;
            this.grpWordRestrict.Text = "Restrictions";
            // 
            // lblWordCount
            // 
            this.lblWordCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblWordCount.AutoSize = true;
            this.lblWordCount.Location = new System.Drawing.Point(6, 153);
            this.lblWordCount.Name = "lblWordCount";
            this.lblWordCount.Size = new System.Drawing.Size(85, 17);
            this.lblWordCount.TabIndex = 1;
            this.lblWordCount.Text = "Word count:";
            // 
            // grpWordInclude
            // 
            this.grpWordInclude.Controls.Add(this.chbWordIncludeSpecialCharacter);
            this.grpWordInclude.Controls.Add(this.chbWordIncludeNumber);
            this.grpWordInclude.Controls.Add(this.chbWordIncludeIncomplete);
            this.grpWordInclude.Controls.Add(this.chbWordIncludeUpperCase);
            this.grpWordInclude.Location = new System.Drawing.Point(6, 6);
            this.grpWordInclude.Name = "grpWordInclude";
            this.grpWordInclude.Padding = new System.Windows.Forms.Padding(3, 12, 3, 3);
            this.grpWordInclude.Size = new System.Drawing.Size(200, 138);
            this.grpWordInclude.TabIndex = 0;
            this.grpWordInclude.TabStop = false;
            this.grpWordInclude.Text = "Include";
            // 
            // tabStyle_Classic
            // 
            this.tabStyle_Classic.Controls.Add(this.lblLength);
            this.tabStyle_Classic.Controls.Add(this.txtLength);
            this.tabStyle_Classic.Controls.Add(this.grpRestrictions);
            this.tabStyle_Classic.Controls.Add(this.grpInclude);
            this.tabStyle_Classic.Location = new System.Drawing.Point(4, 25);
            this.tabStyle_Classic.Name = "tabStyle_Classic";
            this.tabStyle_Classic.Padding = new System.Windows.Forms.Padding(3);
            this.tabStyle_Classic.Size = new System.Drawing.Size(427, 178);
            this.tabStyle_Classic.TabIndex = 1;
            this.tabStyle_Classic.Text = "Classic";
            this.tabStyle_Classic.UseVisualStyleBackColor = true;
            // 
            // lblLength
            // 
            this.lblLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLength.AutoSize = true;
            this.lblLength.Location = new System.Drawing.Point(6, 153);
            this.lblLength.Name = "lblLength";
            this.lblLength.Size = new System.Drawing.Size(56, 17);
            this.lblLength.TabIndex = 2;
            this.lblLength.Text = "Length:";
            // 
            // grpRestrictions
            // 
            this.grpRestrictions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpRestrictions.Controls.Add(this.chbRestrictRepeated);
            this.grpRestrictions.Controls.Add(this.chbRestrictPronounceable);
            this.grpRestrictions.Controls.Add(this.chbRestrictMovable);
            this.grpRestrictions.Controls.Add(this.chbRestrictSimilar);
            this.grpRestrictions.Location = new System.Drawing.Point(221, 6);
            this.grpRestrictions.Margin = new System.Windows.Forms.Padding(12, 3, 3, 3);
            this.grpRestrictions.Name = "grpRestrictions";
            this.grpRestrictions.Padding = new System.Windows.Forms.Padding(3, 12, 3, 3);
            this.grpRestrictions.Size = new System.Drawing.Size(200, 138);
            this.grpRestrictions.TabIndex = 1;
            this.grpRestrictions.TabStop = false;
            this.grpRestrictions.Text = "Restrictions";
            // 
            // grpInclude
            // 
            this.grpInclude.Controls.Add(this.chbIncludeSpecialCharacters);
            this.grpInclude.Controls.Add(this.chbIncludeNumbers);
            this.grpInclude.Controls.Add(this.chbIncludeLowerCase);
            this.grpInclude.Controls.Add(this.chbIncludeUpperCase);
            this.grpInclude.Location = new System.Drawing.Point(6, 6);
            this.grpInclude.Name = "grpInclude";
            this.grpInclude.Padding = new System.Windows.Forms.Padding(3, 12, 3, 3);
            this.grpInclude.Size = new System.Drawing.Size(200, 138);
            this.grpInclude.TabIndex = 0;
            this.grpInclude.TabStop = false;
            this.grpInclude.Text = "Include";
            // 
            // PasswordGeneratorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(459, 331);
            this.Controls.Add(this.tabStyle);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblCombinations);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PasswordGeneratorForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Password generator";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form_FormClosed);
            this.Load += new System.EventHandler(this.Form_Load);
            this.tabStyle.ResumeLayout(false);
            this.tabStyle_Words.ResumeLayout(false);
            this.tabStyle_Words.PerformLayout();
            this.grpWordRestrict.ResumeLayout(false);
            this.grpWordRestrict.PerformLayout();
            this.grpWordInclude.ResumeLayout(false);
            this.grpWordInclude.PerformLayout();
            this.tabStyle_Classic.ResumeLayout(false);
            this.tabStyle_Classic.PerformLayout();
            this.grpRestrictions.ResumeLayout(false);
            this.grpRestrictions.PerformLayout();
            this.grpInclude.ResumeLayout(false);
            this.grpInclude.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblCombinations;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.ToolTip tip;
        private System.Windows.Forms.TabControl tabStyle;
        private System.Windows.Forms.TabPage tabStyle_Words;
        private System.Windows.Forms.TabPage tabStyle_Classic;
        private System.Windows.Forms.Label lblLength;
        private System.Windows.Forms.TextBox txtLength;
        private System.Windows.Forms.GroupBox grpRestrictions;
        private System.Windows.Forms.CheckBox chbRestrictRepeated;
        private System.Windows.Forms.CheckBox chbRestrictPronounceable;
        private System.Windows.Forms.CheckBox chbRestrictMovable;
        private System.Windows.Forms.CheckBox chbRestrictSimilar;
        private System.Windows.Forms.GroupBox grpInclude;
        private System.Windows.Forms.CheckBox chbIncludeSpecialCharacters;
        private System.Windows.Forms.CheckBox chbIncludeNumbers;
        private System.Windows.Forms.CheckBox chbIncludeLowerCase;
        private System.Windows.Forms.CheckBox chbIncludeUpperCase;
        private System.Windows.Forms.Label lblWordCount;
        private System.Windows.Forms.TextBox txtWordCount;
        private System.Windows.Forms.GroupBox grpWordInclude;
        private System.Windows.Forms.CheckBox chbWordIncludeSpecialCharacter;
        private System.Windows.Forms.CheckBox chbWordIncludeNumber;
        private System.Windows.Forms.CheckBox chbWordIncludeIncomplete;
        private System.Windows.Forms.CheckBox chbWordIncludeUpperCase;
        private System.Windows.Forms.GroupBox grpWordRestrict;
        private System.Windows.Forms.CheckBox chbWordRestrictAddSpace;
        private System.Windows.Forms.CheckBox chbWordRestrictBreak;
        private System.Windows.Forms.CheckBox chbWordRestrictTitleCase;
    }
}