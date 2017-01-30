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
            this.mnxList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnxListOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.mnxListOpenReadOnly = new System.Windows.Forms.ToolStripMenuItem();
            this.mnxList0 = new System.Windows.Forms.ToolStripSeparator();
            this.mnxListRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.mnxListReadOnly = new System.Windows.Forms.ToolStripMenuItem();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.tip = new System.Windows.Forms.ToolTip(this.components);
            this.btnOpenReadOnly = new System.Windows.Forms.Button();
            this.mnxList.SuspendLayout();
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
            this.lsvRecent.ContextMenuStrip = this.mnxList;
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
            // mnxList
            // 
            this.mnxList.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mnxList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnxListOpen,
            this.mnxListOpenReadOnly,
            this.mnxList0,
            this.mnxListRemove,
            this.mnxListReadOnly});
            this.mnxList.Name = "mnxList";
            this.mnxList.Size = new System.Drawing.Size(207, 114);
            this.mnxList.Opening += new System.ComponentModel.CancelEventHandler(this.mnxList_Opening);
            // 
            // mnxListOpen
            // 
            this.mnxListOpen.Name = "mnxListOpen";
            this.mnxListOpen.Size = new System.Drawing.Size(206, 26);
            this.mnxListOpen.Text = "&Open";
            this.mnxListOpen.Click += new System.EventHandler(this.mnxListOpen_Click);
            // 
            // mnxListOpenReadOnly
            // 
            this.mnxListOpenReadOnly.Name = "mnxListOpenReadOnly";
            this.mnxListOpenReadOnly.Size = new System.Drawing.Size(206, 26);
            this.mnxListOpenReadOnly.Text = "Open as &read-only";
            this.mnxListOpenReadOnly.Click += new System.EventHandler(this.mnxListOpenReadOnly_Click);
            // 
            // mnxList0
            // 
            this.mnxList0.Name = "mnxList0";
            this.mnxList0.Size = new System.Drawing.Size(203, 6);
            // 
            // mnxListRemove
            // 
            this.mnxListRemove.Name = "mnxListRemove";
            this.mnxListRemove.Size = new System.Drawing.Size(206, 26);
            this.mnxListRemove.Text = "Remove";
            this.mnxListRemove.Click += new System.EventHandler(this.mnxListRemove_Click);
            // 
            // mnxListReadOnly
            // 
            this.mnxListReadOnly.Name = "mnxListReadOnly";
            this.mnxListReadOnly.Size = new System.Drawing.Size(206, 26);
            this.mnxListReadOnly.Text = "Read-only";
            this.mnxListReadOnly.Click += new System.EventHandler(this.mnxListReadOnly_Click);
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
            this.tip.SetToolTip(this.btnOpen, "Open file.");
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
            this.tip.SetToolTip(this.btnClose, "Close.");
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
            this.tip.SetToolTip(this.btnNew, "Create a new file.");
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
            this.tip.SetToolTip(this.btnOpenReadOnly, "Open read/only.");
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
            this.mnxList.ResumeLayout(false);
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
        private System.Windows.Forms.ContextMenuStrip mnxList;
        private System.Windows.Forms.ToolStripMenuItem mnxListRemove;
        private System.Windows.Forms.ToolStripMenuItem mnxListReadOnly;
        private System.Windows.Forms.ToolStripMenuItem mnxListOpen;
        private System.Windows.Forms.ToolStripMenuItem mnxListOpenReadOnly;
        private System.Windows.Forms.ToolStripSeparator mnxList0;
    }
}