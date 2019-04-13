namespace Bimil {
    partial class SearchForm {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchForm));
            this.lblSearch = new System.Windows.Forms.Label();
            this.cmbSearch = new System.Windows.Forms.ComboBox();
            this.lsvEntries = new System.Windows.Forms.ListView();
            this.lsvEntries_colTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chbIncludeHiddenFields = new System.Windows.Forms.CheckBox();
            this.erp = new System.Windows.Forms.ErrorProvider(this.components);
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.erp)).BeginInit();
            this.SuspendLayout();
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(12, 15);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(39, 17);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Text:";
            // 
            // cmbSearch
            // 
            this.cmbSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSearch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.cmbSearch.FormattingEnabled = true;
            this.cmbSearch.Location = new System.Drawing.Point(90, 12);
            this.cmbSearch.Margin = new System.Windows.Forms.Padding(3, 3, 3, 9);
            this.cmbSearch.Name = "cmbSearch";
            this.cmbSearch.Size = new System.Drawing.Size(280, 24);
            this.cmbSearch.TabIndex = 1;
            this.cmbSearch.TextChanged += new System.EventHandler(this.cmbText_TextChanged);
            this.cmbSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbSearch_KeyDown);
            // 
            // lsvEntries
            // 
            this.lsvEntries.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsvEntries.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.lsvEntries_colTitle});
            this.lsvEntries.FullRowSelect = true;
            this.lsvEntries.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lsvEntries.HideSelection = false;
            this.lsvEntries.Location = new System.Drawing.Point(12, 48);
            this.lsvEntries.MultiSelect = false;
            this.lsvEntries.Name = "lsvEntries";
            this.lsvEntries.ShowItemToolTips = true;
            this.lsvEntries.Size = new System.Drawing.Size(358, 287);
            this.lsvEntries.TabIndex = 2;
            this.lsvEntries.UseCompatibleStateImageBehavior = false;
            this.lsvEntries.View = System.Windows.Forms.View.Details;
            this.lsvEntries.ItemActivate += new System.EventHandler(this.lsvEntries_ItemActivate);
            this.lsvEntries.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lsvEntries_KeyDown);
            // 
            // lsvEntries_colTitle
            // 
            this.lsvEntries_colTitle.Text = "Title";
            // 
            // chbIncludeHiddenFields
            // 
            this.chbIncludeHiddenFields.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chbIncludeHiddenFields.AutoSize = true;
            this.chbIncludeHiddenFields.Location = new System.Drawing.Point(12, 353);
            this.chbIncludeHiddenFields.Name = "chbIncludeHiddenFields";
            this.chbIncludeHiddenFields.Size = new System.Drawing.Size(159, 21);
            this.chbIncludeHiddenFields.TabIndex = 3;
            this.chbIncludeHiddenFields.TabStop = false;
            this.chbIncludeHiddenFields.Text = "Include &hidden fields";
            this.chbIncludeHiddenFields.UseVisualStyleBackColor = true;
            this.chbIncludeHiddenFields.CheckedChanged += new System.EventHandler(this.chbIncludeHiddenFields_CheckedChanged);
            // 
            // erp
            // 
            this.erp.ContainerControl = this;
            this.erp.Icon = ((System.Drawing.Icon)(resources.GetObject("erp.Icon")));
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(280, 353);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 28);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // SearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(382, 393);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.chbIncludeHiddenFields);
            this.Controls.Add(this.lsvEntries);
            this.Controls.Add(this.cmbSearch);
            this.Controls.Add(this.lblSearch);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 400);
            this.Name = "SearchForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Search";
            this.Shown += new System.EventHandler(this.Form_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_KeyDown);
            this.Resize += new System.EventHandler(this.Form_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.erp)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.ComboBox cmbSearch;
        private System.Windows.Forms.ListView lsvEntries;
        private System.Windows.Forms.ColumnHeader lsvEntries_colTitle;
        private System.Windows.Forms.CheckBox chbIncludeHiddenFields;
        private System.Windows.Forms.ErrorProvider erp;
        private System.Windows.Forms.Button btnCancel;
    }
}