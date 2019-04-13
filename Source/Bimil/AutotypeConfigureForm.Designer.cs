namespace Bimil {
    partial class AutotypeConfigureForm {
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
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.lsvItems = new System.Windows.Forms.ListView();
            this.lsvItems_colSpecification = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lsvItems_colDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnView = new System.Windows.Forms.Button();
            this.lblEditInfo = new System.Windows.Forms.Label();
            this.tip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAdd.Location = new System.Drawing.Point(12, 233);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(90, 28);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "&Add";
            this.tip.SetToolTip(this.btnAdd, "Add new auto-type entry.");
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(480, 233);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 28);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.tip.SetToolTip(this.btnCancel, "Cancel any changes.");
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(384, 233);
            this.btnOK.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(90, 28);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.tip.SetToolTip(this.btnOK, "Save auto-type settings.");
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemove.Enabled = false;
            this.btnRemove.Location = new System.Drawing.Point(204, 233);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(90, 28);
            this.btnRemove.TabIndex = 5;
            this.btnRemove.Text = "&Remove";
            this.tip.SetToolTip(this.btnRemove, "Remove selected auto-type entry.");
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // lsvItems
            // 
            this.lsvItems.AllowDrop = true;
            this.lsvItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsvItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.lsvItems_colSpecification,
            this.lsvItems_colDescription});
            this.lsvItems.FullRowSelect = true;
            this.lsvItems.GridLines = true;
            this.lsvItems.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lsvItems.HideSelection = false;
            this.lsvItems.LabelEdit = true;
            this.lsvItems.LabelWrap = false;
            this.lsvItems.Location = new System.Drawing.Point(12, 12);
            this.lsvItems.MultiSelect = false;
            this.lsvItems.Name = "lsvItems";
            this.lsvItems.ShowGroups = false;
            this.lsvItems.ShowItemToolTips = true;
            this.lsvItems.Size = new System.Drawing.Size(558, 203);
            this.lsvItems.TabIndex = 1;
            this.lsvItems.UseCompatibleStateImageBehavior = false;
            this.lsvItems.View = System.Windows.Forms.View.Details;
            this.lsvItems.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.lsvItems_AfterLabelEdit);
            this.lsvItems.ItemActivate += new System.EventHandler(this.lsvItems_ItemActivate);
            this.lsvItems.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.lsvItems_ItemDrag);
            this.lsvItems.SelectedIndexChanged += new System.EventHandler(this.lsvItems_SelectedIndexChanged);
            this.lsvItems.DragDrop += new System.Windows.Forms.DragEventHandler(this.lsvItems_DragDrop);
            this.lsvItems.DragEnter += new System.Windows.Forms.DragEventHandler(this.lsvItems_DragEnter);
            this.lsvItems.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lsvItems_KeyDown);
            // 
            // lsvItems_colSpecification
            // 
            this.lsvItems_colSpecification.Text = "Specification";
            this.lsvItems_colSpecification.Width = 180;
            // 
            // lsvItems_colDescription
            // 
            this.lsvItems_colDescription.Text = "Description";
            this.lsvItems_colDescription.Width = 360;
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEdit.Enabled = false;
            this.btnEdit.Location = new System.Drawing.Point(108, 233);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(90, 28);
            this.btnEdit.TabIndex = 3;
            this.btnEdit.Text = "&Edit";
            this.tip.SetToolTip(this.btnEdit, "Edit selected auto-type entry.");
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnView
            // 
            this.btnView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnView.Enabled = false;
            this.btnView.Location = new System.Drawing.Point(108, 233);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(90, 28);
            this.btnView.TabIndex = 4;
            this.btnView.Text = "&View";
            this.tip.SetToolTip(this.btnView, "View selected auto-type entry.");
            this.btnView.Visible = false;
            this.btnView.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // lblEditInfo
            // 
            this.lblEditInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblEditInfo.AutoSize = true;
            this.lblEditInfo.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblEditInfo.Location = new System.Drawing.Point(12, 215);
            this.lblEditInfo.Name = "lblEditInfo";
            this.lblEditInfo.Size = new System.Drawing.Size(293, 17);
            this.lblEditInfo.TabIndex = 7;
            this.lblEditInfo.Text = "To modify settings, you must be in edit mode.";
            this.lblEditInfo.Visible = false;
            // 
            // AutotypeConfigureForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(582, 273);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.lsvItems);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnView);
            this.Controls.Add(this.lblEditInfo);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(560, 320);
            this.Name = "AutotypeConfigureForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure auto-type";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.ListView lsvItems;
        private System.Windows.Forms.ColumnHeader lsvItems_colSpecification;
        private System.Windows.Forms.ColumnHeader lsvItems_colDescription;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnView;
        private System.Windows.Forms.Label lblEditInfo;
        private System.Windows.Forms.ToolTip tip;
    }
}