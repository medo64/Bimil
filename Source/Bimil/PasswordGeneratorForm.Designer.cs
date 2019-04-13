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
            this.chbWordRestrictSuffixOnly = new System.Windows.Forms.CheckBox();
            this.txtWordPasswordLength = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnSaveAndCopy = new System.Windows.Forms.Button();
            this.txtTripletPasswordLength = new System.Windows.Forms.TextBox();
            this.chbTripletRestrictSuffixOnly = new System.Windows.Forms.CheckBox();
            this.chbTripletRestrictTitleCase = new System.Windows.Forms.CheckBox();
            this.txtTripletCount = new System.Windows.Forms.TextBox();
            this.chbTripletIncludeSpecialCharacter = new System.Windows.Forms.CheckBox();
            this.chbTripletIncludeNumber = new System.Windows.Forms.CheckBox();
            this.chbTripletIncludeRandomLetterDrop = new System.Windows.Forms.CheckBox();
            this.chbTripletIncludeRandomUpperCase = new System.Windows.Forms.CheckBox();
            this.chbTripletRestrictAddSpace = new System.Windows.Forms.CheckBox();
            this.chbTripletRestrictBreak = new System.Windows.Forms.CheckBox();
            this.tabStyle = new System.Windows.Forms.TabControl();
            this.tabStyle_Word = new System.Windows.Forms.TabPage();
            this.lblWordPasswordLength = new System.Windows.Forms.Label();
            this.grpWordRestrict = new System.Windows.Forms.GroupBox();
            this.lblWordCount = new System.Windows.Forms.Label();
            this.grpWordInclude = new System.Windows.Forms.GroupBox();
            this.tabStyle_Triplet = new System.Windows.Forms.TabPage();
            this.lblTripletPasswordLength = new System.Windows.Forms.Label();
            this.grpTripletRestrict = new System.Windows.Forms.GroupBox();
            this.lblTripletCount = new System.Windows.Forms.Label();
            this.grpTripletInclude = new System.Windows.Forms.GroupBox();
            this.tabStyle_Classic = new System.Windows.Forms.TabPage();
            this.lblLength = new System.Windows.Forms.Label();
            this.grpRestrictions = new System.Windows.Forms.GroupBox();
            this.grpInclude = new System.Windows.Forms.GroupBox();
            this.picSecurityRating = new System.Windows.Forms.PictureBox();
            this.tabStyle.SuspendLayout();
            this.tabStyle_Word.SuspendLayout();
            this.grpWordRestrict.SuspendLayout();
            this.grpWordInclude.SuspendLayout();
            this.tabStyle_Triplet.SuspendLayout();
            this.grpTripletRestrict.SuspendLayout();
            this.grpTripletInclude.SuspendLayout();
            this.tabStyle_Classic.SuspendLayout();
            this.grpRestrictions.SuspendLayout();
            this.grpInclude.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picSecurityRating)).BeginInit();
            this.SuspendLayout();
            // 
            // lblCombinations
            // 
            this.lblCombinations.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCombinations.Location = new System.Drawing.Point(12, 188);
            this.lblCombinations.Margin = new System.Windows.Forms.Padding(3);
            this.lblCombinations.Name = "lblCombinations";
            this.lblCombinations.Size = new System.Drawing.Size(328, 16);
            this.lblCombinations.TabIndex = 2;
            this.lblCombinations.Text = "-";
            this.lblCombinations.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Location = new System.Drawing.Point(11, 210);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.ReadOnly = true;
            this.txtPassword.Size = new System.Drawing.Size(328, 20);
            this.txtPassword.TabIndex = 3;
            this.txtPassword.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tip.SetToolTip(this.txtPassword, "Generated password");
            // 
            // btnGenerate
            // 
            this.btnGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenerate.Location = new System.Drawing.Point(88, 248);
            this.btnGenerate.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(80, 23);
            this.btnGenerate.TabIndex = 4;
            this.btnGenerate.Text = "&Regenerate";
            this.tip.SetToolTip(this.btnGenerate, "Generate new password.");
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopy.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnCopy.Enabled = false;
            this.btnCopy.Location = new System.Drawing.Point(261, 249);
            this.btnCopy.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(80, 23);
            this.btnCopy.TabIndex = 6;
            this.btnCopy.Text = "&Copy";
            this.tip.SetToolTip(this.btnCopy, "Copy password to clipboard.");
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // chbIncludeSpecialCharacters
            // 
            this.chbIncludeSpecialCharacters.AutoSize = true;
            this.chbIncludeSpecialCharacters.Checked = true;
            this.chbIncludeSpecialCharacters.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbIncludeSpecialCharacters.Location = new System.Drawing.Point(6, 90);
            this.chbIncludeSpecialCharacters.Name = "chbIncludeSpecialCharacters";
            this.chbIncludeSpecialCharacters.Size = new System.Drawing.Size(114, 17);
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
            this.chbIncludeNumbers.Location = new System.Drawing.Point(6, 67);
            this.chbIncludeNumbers.Name = "chbIncludeNumbers";
            this.chbIncludeNumbers.Size = new System.Drawing.Size(68, 17);
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
            this.chbIncludeLowerCase.Location = new System.Drawing.Point(6, 21);
            this.chbIncludeLowerCase.Margin = new System.Windows.Forms.Padding(3, 24, 3, 3);
            this.chbIncludeLowerCase.Name = "chbIncludeLowerCase";
            this.chbIncludeLowerCase.Size = new System.Drawing.Size(81, 17);
            this.chbIncludeLowerCase.TabIndex = 0;
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
            this.chbIncludeUpperCase.Location = new System.Drawing.Point(6, 44);
            this.chbIncludeUpperCase.Name = "chbIncludeUpperCase";
            this.chbIncludeUpperCase.Size = new System.Drawing.Size(81, 17);
            this.chbIncludeUpperCase.TabIndex = 1;
            this.chbIncludeUpperCase.Text = "Upper case";
            this.tip.SetToolTip(this.chbIncludeUpperCase, "Include upper case letters");
            this.chbIncludeUpperCase.UseVisualStyleBackColor = true;
            this.chbIncludeUpperCase.CheckedChanged += new System.EventHandler(this.chbIncludeUpperCase_CheckedChanged);
            // 
            // chbRestrictRepeated
            // 
            this.chbRestrictRepeated.AutoSize = true;
            this.chbRestrictRepeated.Location = new System.Drawing.Point(6, 90);
            this.chbRestrictRepeated.Name = "chbRestrictRepeated";
            this.chbRestrictRepeated.Size = new System.Drawing.Size(126, 17);
            this.chbRestrictRepeated.TabIndex = 3;
            this.chbRestrictRepeated.Text = "Repeated characters";
            this.tip.SetToolTip(this.chbRestrictRepeated, "Don\'t allow characters to repeat immediatelly one after another");
            this.chbRestrictRepeated.UseVisualStyleBackColor = true;
            this.chbRestrictRepeated.CheckedChanged += new System.EventHandler(this.btnGenerate_Click);
            // 
            // chbRestrictPronounceable
            // 
            this.chbRestrictPronounceable.AutoSize = true;
            this.chbRestrictPronounceable.Location = new System.Drawing.Point(6, 67);
            this.chbRestrictPronounceable.Name = "chbRestrictPronounceable";
            this.chbRestrictPronounceable.Size = new System.Drawing.Size(98, 17);
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
            this.chbRestrictMovable.Location = new System.Drawing.Point(6, 44);
            this.chbRestrictMovable.Name = "chbRestrictMovable";
            this.chbRestrictMovable.Size = new System.Drawing.Size(96, 17);
            this.chbRestrictMovable.TabIndex = 1;
            this.chbRestrictMovable.Text = "Avoid movable";
            this.tip.SetToolTip(this.chbRestrictMovable, "Avoid characters that are often found on different spot on different keyboards");
            this.chbRestrictMovable.UseVisualStyleBackColor = true;
            this.chbRestrictMovable.CheckedChanged += new System.EventHandler(this.btnGenerate_Click);
            // 
            // chbRestrictSimilar
            // 
            this.chbRestrictSimilar.AutoSize = true;
            this.chbRestrictSimilar.Location = new System.Drawing.Point(6, 21);
            this.chbRestrictSimilar.Margin = new System.Windows.Forms.Padding(3, 24, 3, 3);
            this.chbRestrictSimilar.Name = "chbRestrictSimilar";
            this.chbRestrictSimilar.Size = new System.Drawing.Size(84, 17);
            this.chbRestrictSimilar.TabIndex = 0;
            this.chbRestrictSimilar.Text = "Avoid similar";
            this.tip.SetToolTip(this.chbRestrictSimilar, "Avoid similarly looking characters (e.g. l and 1)");
            this.chbRestrictSimilar.UseVisualStyleBackColor = true;
            this.chbRestrictSimilar.CheckedChanged += new System.EventHandler(this.btnGenerate_Click);
            // 
            // txtLength
            // 
            this.txtLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtLength.Location = new System.Drawing.Point(115, 125);
            this.txtLength.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.txtLength.MaxLength = 2;
            this.txtLength.Name = "txtLength";
            this.txtLength.ShortcutsEnabled = false;
            this.txtLength.Size = new System.Drawing.Size(38, 20);
            this.txtLength.TabIndex = 3;
            this.txtLength.Text = "14";
            this.txtLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tip.SetToolTip(this.txtLength, "Length of password (1-99).");
            this.txtLength.TextChanged += new System.EventHandler(this.btnGenerate_Click);
            this.txtLength.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtNumber_KeyDown);
            this.txtLength.Leave += new System.EventHandler(this.txtLength_Leave);
            // 
            // txtWordCount
            // 
            this.txtWordCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtWordCount.Location = new System.Drawing.Point(115, 125);
            this.txtWordCount.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.txtWordCount.MaxLength = 1;
            this.txtWordCount.Name = "txtWordCount";
            this.txtWordCount.ShortcutsEnabled = false;
            this.txtWordCount.Size = new System.Drawing.Size(38, 20);
            this.txtWordCount.TabIndex = 3;
            this.txtWordCount.Text = "5";
            this.txtWordCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tip.SetToolTip(this.txtWordCount, "Number of words to use (1-9)");
            this.txtWordCount.TextChanged += new System.EventHandler(this.btnGenerate_Click);
            this.txtWordCount.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtNumber_KeyDown);
            this.txtWordCount.Leave += new System.EventHandler(this.txtWordCount_Leave);
            // 
            // chbWordIncludeSpecialCharacter
            // 
            this.chbWordIncludeSpecialCharacter.AutoSize = true;
            this.chbWordIncludeSpecialCharacter.Location = new System.Drawing.Point(6, 44);
            this.chbWordIncludeSpecialCharacter.Name = "chbWordIncludeSpecialCharacter";
            this.chbWordIncludeSpecialCharacter.Size = new System.Drawing.Size(109, 17);
            this.chbWordIncludeSpecialCharacter.TabIndex = 1;
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
            this.chbWordIncludeNumber.Location = new System.Drawing.Point(6, 21);
            this.chbWordIncludeNumber.Margin = new System.Windows.Forms.Padding(3, 24, 3, 3);
            this.chbWordIncludeNumber.Name = "chbWordIncludeNumber";
            this.chbWordIncludeNumber.Size = new System.Drawing.Size(63, 17);
            this.chbWordIncludeNumber.TabIndex = 0;
            this.chbWordIncludeNumber.Text = "Number";
            this.tip.SetToolTip(this.chbWordIncludeNumber, "Include number in one of words");
            this.chbWordIncludeNumber.UseVisualStyleBackColor = true;
            this.chbWordIncludeNumber.CheckedChanged += new System.EventHandler(this.btnGenerate_Click);
            // 
            // chbWordIncludeIncomplete
            // 
            this.chbWordIncludeIncomplete.AutoSize = true;
            this.chbWordIncludeIncomplete.Location = new System.Drawing.Point(6, 90);
            this.chbWordIncludeIncomplete.Name = "chbWordIncludeIncomplete";
            this.chbWordIncludeIncomplete.Size = new System.Drawing.Size(104, 17);
            this.chbWordIncludeIncomplete.TabIndex = 3;
            this.chbWordIncludeIncomplete.Text = "Incomplete word";
            this.tip.SetToolTip(this.chbWordIncludeIncomplete, "Remove a character from one of the words.");
            this.chbWordIncludeIncomplete.UseVisualStyleBackColor = true;
            this.chbWordIncludeIncomplete.CheckedChanged += new System.EventHandler(this.btnGenerate_Click);
            // 
            // chbWordIncludeUpperCase
            // 
            this.chbWordIncludeUpperCase.AutoSize = true;
            this.chbWordIncludeUpperCase.Location = new System.Drawing.Point(6, 67);
            this.chbWordIncludeUpperCase.Name = "chbWordIncludeUpperCase";
            this.chbWordIncludeUpperCase.Size = new System.Drawing.Size(107, 17);
            this.chbWordIncludeUpperCase.TabIndex = 2;
            this.chbWordIncludeUpperCase.Text = "Upper case letter";
            this.tip.SetToolTip(this.chbWordIncludeUpperCase, "Include upper case letter in one of words");
            this.chbWordIncludeUpperCase.UseVisualStyleBackColor = true;
            this.chbWordIncludeUpperCase.CheckedChanged += new System.EventHandler(this.btnGenerate_Click);
            // 
            // chbWordRestrictAddSpace
            // 
            this.chbWordRestrictAddSpace.AutoSize = true;
            this.chbWordRestrictAddSpace.Location = new System.Drawing.Point(6, 90);
            this.chbWordRestrictAddSpace.Name = "chbWordRestrictAddSpace";
            this.chbWordRestrictAddSpace.Size = new System.Drawing.Size(85, 17);
            this.chbWordRestrictAddSpace.TabIndex = 3;
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
            this.chbWordRestrictBreak.Location = new System.Drawing.Point(6, 44);
            this.chbWordRestrictBreak.Name = "chbWordRestrictBreak";
            this.chbWordRestrictBreak.Size = new System.Drawing.Size(122, 17);
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
            this.chbWordRestrictTitleCase.Location = new System.Drawing.Point(6, 21);
            this.chbWordRestrictTitleCase.Margin = new System.Windows.Forms.Padding(3, 24, 3, 3);
            this.chbWordRestrictTitleCase.Name = "chbWordRestrictTitleCase";
            this.chbWordRestrictTitleCase.Size = new System.Drawing.Size(72, 17);
            this.chbWordRestrictTitleCase.TabIndex = 0;
            this.chbWordRestrictTitleCase.Text = "Title case";
            this.tip.SetToolTip(this.chbWordRestrictTitleCase, "First character of every word will be capitalized.");
            this.chbWordRestrictTitleCase.UseVisualStyleBackColor = true;
            this.chbWordRestrictTitleCase.CheckedChanged += new System.EventHandler(this.btnGenerate_Click);
            // 
            // chbWordRestrictSuffixOnly
            // 
            this.chbWordRestrictSuffixOnly.AutoSize = true;
            this.chbWordRestrictSuffixOnly.Checked = true;
            this.chbWordRestrictSuffixOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbWordRestrictSuffixOnly.Location = new System.Drawing.Point(6, 67);
            this.chbWordRestrictSuffixOnly.Name = "chbWordRestrictSuffixOnly";
            this.chbWordRestrictSuffixOnly.Size = new System.Drawing.Size(124, 17);
            this.chbWordRestrictSuffixOnly.TabIndex = 2;
            this.chbWordRestrictSuffixOnly.Text = "Append to suffix-only";
            this.tip.SetToolTip(this.chbWordRestrictSuffixOnly, "Modifications to the words are done at the end of password only.");
            this.chbWordRestrictSuffixOnly.UseVisualStyleBackColor = true;
            this.chbWordRestrictSuffixOnly.CheckedChanged += new System.EventHandler(this.btnGenerate_Click);
            // 
            // txtWordPasswordLength
            // 
            this.txtWordPasswordLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtWordPasswordLength.Location = new System.Drawing.Point(280, 125);
            this.txtWordPasswordLength.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.txtWordPasswordLength.MaxLength = 2;
            this.txtWordPasswordLength.Name = "txtWordPasswordLength";
            this.txtWordPasswordLength.ReadOnly = true;
            this.txtWordPasswordLength.ShortcutsEnabled = false;
            this.txtWordPasswordLength.Size = new System.Drawing.Size(38, 20);
            this.txtWordPasswordLength.TabIndex = 5;
            this.txtWordPasswordLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tip.SetToolTip(this.txtWordPasswordLength, "Length of password (1-99).");
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(260, 248);
            this.btnSave.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(80, 23);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "&Save";
            this.tip.SetToolTip(this.btnSave, "Save password");
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnSaveAndCopy
            // 
            this.btnSaveAndCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveAndCopy.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSaveAndCopy.Enabled = false;
            this.btnSaveAndCopy.Location = new System.Drawing.Point(174, 248);
            this.btnSaveAndCopy.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnSaveAndCopy.Name = "btnSaveAndCopy";
            this.btnSaveAndCopy.Size = new System.Drawing.Size(80, 23);
            this.btnSaveAndCopy.TabIndex = 5;
            this.btnSaveAndCopy.Text = "&Save && Copy";
            this.tip.SetToolTip(this.btnSaveAndCopy, "Save password and copy to clipboard.");
            this.btnSaveAndCopy.UseVisualStyleBackColor = true;
            this.btnSaveAndCopy.Click += new System.EventHandler(this.btnSaveAndCopy_Click);
            // 
            // txtTripletPasswordLength
            // 
            this.txtTripletPasswordLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtTripletPasswordLength.Location = new System.Drawing.Point(280, 125);
            this.txtTripletPasswordLength.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.txtTripletPasswordLength.MaxLength = 2;
            this.txtTripletPasswordLength.Name = "txtTripletPasswordLength";
            this.txtTripletPasswordLength.ReadOnly = true;
            this.txtTripletPasswordLength.ShortcutsEnabled = false;
            this.txtTripletPasswordLength.Size = new System.Drawing.Size(38, 20);
            this.txtTripletPasswordLength.TabIndex = 11;
            this.txtTripletPasswordLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tip.SetToolTip(this.txtTripletPasswordLength, "Length of password (1-99).");
            // 
            // chbTripletRestrictSuffixOnly
            // 
            this.chbTripletRestrictSuffixOnly.AutoSize = true;
            this.chbTripletRestrictSuffixOnly.Checked = true;
            this.chbTripletRestrictSuffixOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbTripletRestrictSuffixOnly.Location = new System.Drawing.Point(6, 67);
            this.chbTripletRestrictSuffixOnly.Name = "chbTripletRestrictSuffixOnly";
            this.chbTripletRestrictSuffixOnly.Size = new System.Drawing.Size(124, 17);
            this.chbTripletRestrictSuffixOnly.TabIndex = 2;
            this.chbTripletRestrictSuffixOnly.Text = "Append to suffix-only";
            this.tip.SetToolTip(this.chbTripletRestrictSuffixOnly, "Modifications to the words are done at the end of password only.");
            this.chbTripletRestrictSuffixOnly.UseVisualStyleBackColor = true;
            this.chbTripletRestrictSuffixOnly.CheckedChanged += new System.EventHandler(this.btnGenerate_Click);
            // 
            // chbTripletRestrictTitleCase
            // 
            this.chbTripletRestrictTitleCase.AutoSize = true;
            this.chbTripletRestrictTitleCase.Checked = true;
            this.chbTripletRestrictTitleCase.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbTripletRestrictTitleCase.Location = new System.Drawing.Point(6, 21);
            this.chbTripletRestrictTitleCase.Margin = new System.Windows.Forms.Padding(3, 24, 3, 3);
            this.chbTripletRestrictTitleCase.Name = "chbTripletRestrictTitleCase";
            this.chbTripletRestrictTitleCase.Size = new System.Drawing.Size(72, 17);
            this.chbTripletRestrictTitleCase.TabIndex = 0;
            this.chbTripletRestrictTitleCase.Text = "Title case";
            this.tip.SetToolTip(this.chbTripletRestrictTitleCase, "First character of every word will be capitalized.");
            this.chbTripletRestrictTitleCase.UseVisualStyleBackColor = true;
            this.chbTripletRestrictTitleCase.CheckedChanged += new System.EventHandler(this.btnGenerate_Click);
            // 
            // txtTripletCount
            // 
            this.txtTripletCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtTripletCount.Location = new System.Drawing.Point(115, 125);
            this.txtTripletCount.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.txtTripletCount.MaxLength = 1;
            this.txtTripletCount.Name = "txtTripletCount";
            this.txtTripletCount.ShortcutsEnabled = false;
            this.txtTripletCount.Size = new System.Drawing.Size(38, 20);
            this.txtTripletCount.TabIndex = 9;
            this.txtTripletCount.Text = "5";
            this.txtTripletCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tip.SetToolTip(this.txtTripletCount, "Number of triplets to use (1-9)");
            this.txtTripletCount.TextChanged += new System.EventHandler(this.btnGenerate_Click);
            this.txtTripletCount.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtNumber_KeyDown);
            this.txtTripletCount.Leave += new System.EventHandler(this.txtTripletCount_Leave);
            // 
            // chbTripletIncludeSpecialCharacter
            // 
            this.chbTripletIncludeSpecialCharacter.AutoSize = true;
            this.chbTripletIncludeSpecialCharacter.Checked = true;
            this.chbTripletIncludeSpecialCharacter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbTripletIncludeSpecialCharacter.Location = new System.Drawing.Point(6, 44);
            this.chbTripletIncludeSpecialCharacter.Name = "chbTripletIncludeSpecialCharacter";
            this.chbTripletIncludeSpecialCharacter.Size = new System.Drawing.Size(109, 17);
            this.chbTripletIncludeSpecialCharacter.TabIndex = 1;
            this.chbTripletIncludeSpecialCharacter.Text = "Special character";
            this.tip.SetToolTip(this.chbTripletIncludeSpecialCharacter, "Include special character in one of the words");
            this.chbTripletIncludeSpecialCharacter.UseVisualStyleBackColor = true;
            this.chbTripletIncludeSpecialCharacter.CheckedChanged += new System.EventHandler(this.btnGenerate_Click);
            // 
            // chbTripletIncludeNumber
            // 
            this.chbTripletIncludeNumber.AutoSize = true;
            this.chbTripletIncludeNumber.Checked = true;
            this.chbTripletIncludeNumber.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbTripletIncludeNumber.Location = new System.Drawing.Point(6, 21);
            this.chbTripletIncludeNumber.Margin = new System.Windows.Forms.Padding(3, 24, 3, 3);
            this.chbTripletIncludeNumber.Name = "chbTripletIncludeNumber";
            this.chbTripletIncludeNumber.Size = new System.Drawing.Size(63, 17);
            this.chbTripletIncludeNumber.TabIndex = 0;
            this.chbTripletIncludeNumber.Text = "Number";
            this.tip.SetToolTip(this.chbTripletIncludeNumber, "Include number in one of words");
            this.chbTripletIncludeNumber.UseVisualStyleBackColor = true;
            this.chbTripletIncludeNumber.CheckedChanged += new System.EventHandler(this.btnGenerate_Click);
            // 
            // chbTripletIncludeRandomLetterDrop
            // 
            this.chbTripletIncludeRandomLetterDrop.AutoSize = true;
            this.chbTripletIncludeRandomLetterDrop.Location = new System.Drawing.Point(6, 90);
            this.chbTripletIncludeRandomLetterDrop.Name = "chbTripletIncludeRandomLetterDrop";
            this.chbTripletIncludeRandomLetterDrop.Size = new System.Drawing.Size(116, 17);
            this.chbTripletIncludeRandomLetterDrop.TabIndex = 3;
            this.chbTripletIncludeRandomLetterDrop.Text = "Random letter drop";
            this.tip.SetToolTip(this.chbTripletIncludeRandomLetterDrop, "Remove a character from one of the words.");
            this.chbTripletIncludeRandomLetterDrop.UseVisualStyleBackColor = true;
            this.chbTripletIncludeRandomLetterDrop.CheckedChanged += new System.EventHandler(this.btnGenerate_Click);
            // 
            // chbTripletIncludeRandomUpperCase
            // 
            this.chbTripletIncludeRandomUpperCase.AutoSize = true;
            this.chbTripletIncludeRandomUpperCase.Location = new System.Drawing.Point(6, 67);
            this.chbTripletIncludeRandomUpperCase.Name = "chbTripletIncludeRandomUpperCase";
            this.chbTripletIncludeRandomUpperCase.Size = new System.Drawing.Size(122, 17);
            this.chbTripletIncludeRandomUpperCase.TabIndex = 2;
            this.chbTripletIncludeRandomUpperCase.Text = "Random upper case";
            this.tip.SetToolTip(this.chbTripletIncludeRandomUpperCase, "Upper case letter will be added at random locations.");
            this.chbTripletIncludeRandomUpperCase.UseVisualStyleBackColor = true;
            this.chbTripletIncludeRandomUpperCase.CheckedChanged += new System.EventHandler(this.btnGenerate_Click);
            // 
            // chbTripletRestrictAddSpace
            // 
            this.chbTripletRestrictAddSpace.AutoSize = true;
            this.chbTripletRestrictAddSpace.Location = new System.Drawing.Point(6, 90);
            this.chbTripletRestrictAddSpace.Name = "chbTripletRestrictAddSpace";
            this.chbTripletRestrictAddSpace.Size = new System.Drawing.Size(85, 17);
            this.chbTripletRestrictAddSpace.TabIndex = 3;
            this.chbTripletRestrictAddSpace.Text = "Add spacing";
            this.tip.SetToolTip(this.chbTripletRestrictAddSpace, "Adds spacing between words");
            this.chbTripletRestrictAddSpace.UseVisualStyleBackColor = true;
            this.chbTripletRestrictAddSpace.CheckedChanged += new System.EventHandler(this.btnGenerate_Click);
            // 
            // chbTripletRestrictBreak
            // 
            this.chbTripletRestrictBreak.AutoSize = true;
            this.chbTripletRestrictBreak.Checked = true;
            this.chbTripletRestrictBreak.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbTripletRestrictBreak.Location = new System.Drawing.Point(6, 44);
            this.chbTripletRestrictBreak.Name = "chbTripletRestrictBreak";
            this.chbTripletRestrictBreak.Size = new System.Drawing.Size(122, 17);
            this.chbTripletRestrictBreak.TabIndex = 1;
            this.chbTripletRestrictBreak.Text = "Don\'t break up word";
            this.tip.SetToolTip(this.chbTripletRestrictBreak, "Modifications to the words are done at the begining or at the end of word only.");
            this.chbTripletRestrictBreak.UseVisualStyleBackColor = true;
            this.chbTripletRestrictBreak.CheckedChanged += new System.EventHandler(this.btnGenerate_Click);
            // 
            // tabStyle
            // 
            this.tabStyle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabStyle.Controls.Add(this.tabStyle_Word);
            this.tabStyle.Controls.Add(this.tabStyle_Triplet);
            this.tabStyle.Controls.Add(this.tabStyle_Classic);
            this.tabStyle.Location = new System.Drawing.Point(11, 12);
            this.tabStyle.Name = "tabStyle";
            this.tabStyle.SelectedIndex = 0;
            this.tabStyle.Size = new System.Drawing.Size(329, 174);
            this.tabStyle.TabIndex = 1;
            this.tabStyle.SelectedIndexChanged += new System.EventHandler(this.btnGenerate_Click);
            // 
            // tabStyle_Word
            // 
            this.tabStyle_Word.Controls.Add(this.lblWordPasswordLength);
            this.tabStyle_Word.Controls.Add(this.txtWordPasswordLength);
            this.tabStyle_Word.Controls.Add(this.grpWordRestrict);
            this.tabStyle_Word.Controls.Add(this.lblWordCount);
            this.tabStyle_Word.Controls.Add(this.txtWordCount);
            this.tabStyle_Word.Controls.Add(this.grpWordInclude);
            this.tabStyle_Word.Location = new System.Drawing.Point(4, 22);
            this.tabStyle_Word.Name = "tabStyle_Word";
            this.tabStyle_Word.Size = new System.Drawing.Size(321, 148);
            this.tabStyle_Word.TabIndex = 0;
            this.tabStyle_Word.Text = "Word";
            this.tabStyle_Word.UseVisualStyleBackColor = true;
            // 
            // lblWordPasswordLength
            // 
            this.lblWordPasswordLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblWordPasswordLength.AutoSize = true;
            this.lblWordPasswordLength.Location = new System.Drawing.Point(170, 128);
            this.lblWordPasswordLength.Name = "lblWordPasswordLength";
            this.lblWordPasswordLength.Size = new System.Drawing.Size(88, 13);
            this.lblWordPasswordLength.TabIndex = 4;
            this.lblWordPasswordLength.Text = "Password length:";
            // 
            // grpWordRestrict
            // 
            this.grpWordRestrict.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpWordRestrict.Controls.Add(this.chbWordRestrictSuffixOnly);
            this.grpWordRestrict.Controls.Add(this.chbWordRestrictTitleCase);
            this.grpWordRestrict.Controls.Add(this.chbWordRestrictBreak);
            this.grpWordRestrict.Controls.Add(this.chbWordRestrictAddSpace);
            this.grpWordRestrict.Location = new System.Drawing.Point(168, 3);
            this.grpWordRestrict.Margin = new System.Windows.Forms.Padding(12, 3, 3, 3);
            this.grpWordRestrict.Name = "grpWordRestrict";
            this.grpWordRestrict.Padding = new System.Windows.Forms.Padding(3, 12, 3, 3);
            this.grpWordRestrict.Size = new System.Drawing.Size(150, 113);
            this.grpWordRestrict.TabIndex = 1;
            this.grpWordRestrict.TabStop = false;
            this.grpWordRestrict.Text = "Restrictions";
            // 
            // lblWordCount
            // 
            this.lblWordCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblWordCount.AutoSize = true;
            this.lblWordCount.Location = new System.Drawing.Point(6, 128);
            this.lblWordCount.Name = "lblWordCount";
            this.lblWordCount.Size = new System.Drawing.Size(66, 13);
            this.lblWordCount.TabIndex = 2;
            this.lblWordCount.Text = "Word count:";
            // 
            // grpWordInclude
            // 
            this.grpWordInclude.Controls.Add(this.chbWordIncludeSpecialCharacter);
            this.grpWordInclude.Controls.Add(this.chbWordIncludeNumber);
            this.grpWordInclude.Controls.Add(this.chbWordIncludeIncomplete);
            this.grpWordInclude.Controls.Add(this.chbWordIncludeUpperCase);
            this.grpWordInclude.Location = new System.Drawing.Point(3, 3);
            this.grpWordInclude.Name = "grpWordInclude";
            this.grpWordInclude.Padding = new System.Windows.Forms.Padding(3, 12, 3, 3);
            this.grpWordInclude.Size = new System.Drawing.Size(150, 113);
            this.grpWordInclude.TabIndex = 0;
            this.grpWordInclude.TabStop = false;
            this.grpWordInclude.Text = "Include";
            // 
            // tabStyle_Triplet
            // 
            this.tabStyle_Triplet.Controls.Add(this.lblTripletPasswordLength);
            this.tabStyle_Triplet.Controls.Add(this.txtTripletPasswordLength);
            this.tabStyle_Triplet.Controls.Add(this.grpTripletRestrict);
            this.tabStyle_Triplet.Controls.Add(this.lblTripletCount);
            this.tabStyle_Triplet.Controls.Add(this.txtTripletCount);
            this.tabStyle_Triplet.Controls.Add(this.grpTripletInclude);
            this.tabStyle_Triplet.Location = new System.Drawing.Point(4, 22);
            this.tabStyle_Triplet.Name = "tabStyle_Triplet";
            this.tabStyle_Triplet.Size = new System.Drawing.Size(321, 148);
            this.tabStyle_Triplet.TabIndex = 2;
            this.tabStyle_Triplet.Text = "Triplet";
            this.tabStyle_Triplet.UseVisualStyleBackColor = true;
            // 
            // lblTripletPasswordLength
            // 
            this.lblTripletPasswordLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTripletPasswordLength.AutoSize = true;
            this.lblTripletPasswordLength.Location = new System.Drawing.Point(170, 128);
            this.lblTripletPasswordLength.Name = "lblTripletPasswordLength";
            this.lblTripletPasswordLength.Size = new System.Drawing.Size(88, 13);
            this.lblTripletPasswordLength.TabIndex = 10;
            this.lblTripletPasswordLength.Text = "Password length:";
            // 
            // grpTripletRestrict
            // 
            this.grpTripletRestrict.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpTripletRestrict.Controls.Add(this.chbTripletRestrictSuffixOnly);
            this.grpTripletRestrict.Controls.Add(this.chbTripletRestrictTitleCase);
            this.grpTripletRestrict.Controls.Add(this.chbTripletRestrictBreak);
            this.grpTripletRestrict.Controls.Add(this.chbTripletRestrictAddSpace);
            this.grpTripletRestrict.Location = new System.Drawing.Point(168, 3);
            this.grpTripletRestrict.Margin = new System.Windows.Forms.Padding(12, 3, 3, 3);
            this.grpTripletRestrict.Name = "grpTripletRestrict";
            this.grpTripletRestrict.Padding = new System.Windows.Forms.Padding(3, 12, 3, 3);
            this.grpTripletRestrict.Size = new System.Drawing.Size(150, 113);
            this.grpTripletRestrict.TabIndex = 7;
            this.grpTripletRestrict.TabStop = false;
            this.grpTripletRestrict.Text = "Restrictions";
            // 
            // lblTripletCount
            // 
            this.lblTripletCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTripletCount.AutoSize = true;
            this.lblTripletCount.Location = new System.Drawing.Point(6, 128);
            this.lblTripletCount.Name = "lblTripletCount";
            this.lblTripletCount.Size = new System.Drawing.Size(69, 13);
            this.lblTripletCount.TabIndex = 8;
            this.lblTripletCount.Text = "Triplet count:";
            // 
            // grpTripletInclude
            // 
            this.grpTripletInclude.Controls.Add(this.chbTripletIncludeSpecialCharacter);
            this.grpTripletInclude.Controls.Add(this.chbTripletIncludeNumber);
            this.grpTripletInclude.Controls.Add(this.chbTripletIncludeRandomLetterDrop);
            this.grpTripletInclude.Controls.Add(this.chbTripletIncludeRandomUpperCase);
            this.grpTripletInclude.Location = new System.Drawing.Point(3, 3);
            this.grpTripletInclude.Name = "grpTripletInclude";
            this.grpTripletInclude.Padding = new System.Windows.Forms.Padding(3, 12, 3, 3);
            this.grpTripletInclude.Size = new System.Drawing.Size(150, 113);
            this.grpTripletInclude.TabIndex = 6;
            this.grpTripletInclude.TabStop = false;
            this.grpTripletInclude.Text = "Include";
            // 
            // tabStyle_Classic
            // 
            this.tabStyle_Classic.Controls.Add(this.lblLength);
            this.tabStyle_Classic.Controls.Add(this.txtLength);
            this.tabStyle_Classic.Controls.Add(this.grpRestrictions);
            this.tabStyle_Classic.Controls.Add(this.grpInclude);
            this.tabStyle_Classic.Location = new System.Drawing.Point(4, 22);
            this.tabStyle_Classic.Margin = new System.Windows.Forms.Padding(2);
            this.tabStyle_Classic.Name = "tabStyle_Classic";
            this.tabStyle_Classic.Padding = new System.Windows.Forms.Padding(2);
            this.tabStyle_Classic.Size = new System.Drawing.Size(321, 148);
            this.tabStyle_Classic.TabIndex = 1;
            this.tabStyle_Classic.Text = "Classic";
            this.tabStyle_Classic.UseVisualStyleBackColor = true;
            // 
            // lblLength
            // 
            this.lblLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLength.AutoSize = true;
            this.lblLength.Location = new System.Drawing.Point(6, 128);
            this.lblLength.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblLength.Name = "lblLength";
            this.lblLength.Size = new System.Drawing.Size(43, 13);
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
            this.grpRestrictions.Location = new System.Drawing.Point(168, 3);
            this.grpRestrictions.Margin = new System.Windows.Forms.Padding(12, 3, 3, 3);
            this.grpRestrictions.Name = "grpRestrictions";
            this.grpRestrictions.Padding = new System.Windows.Forms.Padding(3, 12, 3, 3);
            this.grpRestrictions.Size = new System.Drawing.Size(150, 113);
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
            this.grpInclude.Location = new System.Drawing.Point(3, 3);
            this.grpInclude.Name = "grpInclude";
            this.grpInclude.Padding = new System.Windows.Forms.Padding(3, 12, 3, 3);
            this.grpInclude.Size = new System.Drawing.Size(150, 113);
            this.grpInclude.TabIndex = 0;
            this.grpInclude.TabStop = false;
            this.grpInclude.Text = "Include";
            // 
            // picSecurityRating
            // 
            this.picSecurityRating.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.picSecurityRating.Location = new System.Drawing.Point(9, 235);
            this.picSecurityRating.Margin = new System.Windows.Forms.Padding(0);
            this.picSecurityRating.Name = "picSecurityRating";
            this.picSecurityRating.Size = new System.Drawing.Size(36, 39);
            this.picSecurityRating.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picSecurityRating.TabIndex = 5;
            this.picSecurityRating.TabStop = false;
            // 
            // PasswordGeneratorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(352, 283);
            this.Controls.Add(this.btnSaveAndCopy);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.picSecurityRating);
            this.Controls.Add(this.tabStyle);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblCombinations);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2);
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
            this.tabStyle_Word.ResumeLayout(false);
            this.tabStyle_Word.PerformLayout();
            this.grpWordRestrict.ResumeLayout(false);
            this.grpWordRestrict.PerformLayout();
            this.grpWordInclude.ResumeLayout(false);
            this.grpWordInclude.PerformLayout();
            this.tabStyle_Triplet.ResumeLayout(false);
            this.tabStyle_Triplet.PerformLayout();
            this.grpTripletRestrict.ResumeLayout(false);
            this.grpTripletRestrict.PerformLayout();
            this.grpTripletInclude.ResumeLayout(false);
            this.grpTripletInclude.PerformLayout();
            this.tabStyle_Classic.ResumeLayout(false);
            this.tabStyle_Classic.PerformLayout();
            this.grpRestrictions.ResumeLayout(false);
            this.grpRestrictions.PerformLayout();
            this.grpInclude.ResumeLayout(false);
            this.grpInclude.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picSecurityRating)).EndInit();
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
        private System.Windows.Forms.TabPage tabStyle_Word;
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
        private System.Windows.Forms.CheckBox chbWordRestrictSuffixOnly;
        private System.Windows.Forms.PictureBox picSecurityRating;
        private System.Windows.Forms.Label lblWordPasswordLength;
        private System.Windows.Forms.TextBox txtWordPasswordLength;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnSaveAndCopy;
        private System.Windows.Forms.TabPage tabStyle_Triplet;
        private System.Windows.Forms.Label lblTripletPasswordLength;
        private System.Windows.Forms.TextBox txtTripletPasswordLength;
        private System.Windows.Forms.GroupBox grpTripletRestrict;
        private System.Windows.Forms.CheckBox chbTripletRestrictSuffixOnly;
        private System.Windows.Forms.CheckBox chbTripletRestrictTitleCase;
        private System.Windows.Forms.Label lblTripletCount;
        private System.Windows.Forms.TextBox txtTripletCount;
        private System.Windows.Forms.GroupBox grpTripletInclude;
        private System.Windows.Forms.CheckBox chbTripletIncludeSpecialCharacter;
        private System.Windows.Forms.CheckBox chbTripletIncludeNumber;
        private System.Windows.Forms.CheckBox chbTripletIncludeRandomLetterDrop;
        private System.Windows.Forms.CheckBox chbTripletIncludeRandomUpperCase;
        private System.Windows.Forms.CheckBox chbTripletRestrictAddSpace;
        private System.Windows.Forms.CheckBox chbTripletRestrictBreak;
    }
}