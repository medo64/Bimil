namespace Bimil {
    partial class StartForm {
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
            this.lsvRecent = new System.Windows.Forms.ListView();
            this.lsvRecent_colFile = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.tip = new System.Windows.Forms.ToolTip(this.components);
            this.btnOpenReadOnly = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lsvRecent
            // 
            this.lsvRecent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsvRecent.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lsvRecent.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.lsvRecent_colFile});
            this.lsvRecent.FullRowSelect = true;
            this.lsvRecent.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lsvRecent.Location = new System.Drawing.Point(0, 0);
            this.lsvRecent.MultiSelect = false;
            this.lsvRecent.Name = "lsvRecent";
            this.lsvRecent.ShowGroups = false;
            this.lsvRecent.ShowItemToolTips = true;
            this.lsvRecent.Size = new System.Drawing.Size(481, 296);
            this.lsvRecent.TabIndex = 0;
            this.lsvRecent.UseCompatibleStateImageBehavior = false;
            this.lsvRecent.View = System.Windows.Forms.View.Details;
            this.lsvRecent.ItemActivate += new System.EventHandler(this.lsvRecent_ItemActivate);
            this.lsvRecent.SelectedIndexChanged += new System.EventHandler(this.lsvRecent_SelectedIndexChanged);
            // 
            // lsvRecent_colFile
            // 
            this.lsvRecent_colFile.Text = "File";
            this.lsvRecent_colFile.Width = 433;
            // 
            // btnOpen
            // 
            this.btnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOpen.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOpen.Enabled = false;
            this.btnOpen.Location = new System.Drawing.Point(12, 311);
            this.btnOpen.Margin = new System.Windows.Forms.Padding(3, 12, 3, 3);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(100, 30);
            this.btnOpen.TabIndex = 1;
            this.btnOpen.Text = "&Open";
            this.tip.SetToolTip(this.btnOpen, "Open file");
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(370, 311);
            this.btnClose.Margin = new System.Windows.Forms.Padding(3, 12, 3, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 30);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Close";
            this.tip.SetToolTip(this.btnClose, "Close dialog");
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // btnNew
            // 
            this.btnNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNew.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnNew.Location = new System.Drawing.Point(233, 311);
            this.btnNew.Margin = new System.Windows.Forms.Padding(12, 12, 3, 3);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(100, 30);
            this.btnNew.TabIndex = 3;
            this.btnNew.Text = "&New";
            this.tip.SetToolTip(this.btnNew, "Create a new file");
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnOpenReadOnly
            // 
            this.btnOpenReadOnly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOpenReadOnly.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOpenReadOnly.Enabled = false;
            this.btnOpenReadOnly.Location = new System.Drawing.Point(118, 311);
            this.btnOpenReadOnly.Margin = new System.Windows.Forms.Padding(3, 12, 3, 3);
            this.btnOpenReadOnly.Name = "btnOpenReadOnly";
            this.btnOpenReadOnly.Size = new System.Drawing.Size(100, 30);
            this.btnOpenReadOnly.TabIndex = 2;
            this.btnOpenReadOnly.Text = "Open (&r/o)";
            this.tip.SetToolTip(this.btnOpenReadOnly, "Open read/only");
            this.btnOpenReadOnly.UseVisualStyleBackColor = true;
            this.btnOpenReadOnly.Click += new System.EventHandler(this.btnOpenReadOnly_Click);
            // 
            // StartForm
            // 
            this.AcceptButton = this.btnOpen;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(482, 353);
            this.Controls.Add(this.btnOpenReadOnly);
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.lsvRecent);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 400);
            this.Name = "StartForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Start";
            this.Shown += new System.EventHandler(this.Form_Shown);
            this.Resize += new System.EventHandler(this.Form_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lsvRecent;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.ColumnHeader lsvRecent_colFile;
        private System.Windows.Forms.ToolTip tip;
        private System.Windows.Forms.Button btnOpenReadOnly;
    }
}