namespace Bimil {
    partial class GoogleDocsOpenForm {
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lsvDocuments = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.bwFill = new System.ComponentModel.BackgroundWorker();
            this.bwOpen = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(452, 328);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 28);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(356, 328);
            this.btnOK.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(90, 28);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lsvDocuments
            // 
            this.lsvDocuments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsvDocuments.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lsvDocuments.FullRowSelect = true;
            this.lsvDocuments.GridLines = true;
            this.lsvDocuments.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lsvDocuments.Location = new System.Drawing.Point(12, 12);
            this.lsvDocuments.MultiSelect = false;
            this.lsvDocuments.Name = "lsvDocuments";
            this.lsvDocuments.Size = new System.Drawing.Size(530, 298);
            this.lsvDocuments.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lsvDocuments.TabIndex = 0;
            this.lsvDocuments.UseCompatibleStateImageBehavior = false;
            this.lsvDocuments.View = System.Windows.Forms.View.Details;
            this.lsvDocuments.ItemActivate += new System.EventHandler(this.lsvDocuments_ItemActivate);
            this.lsvDocuments.SelectedIndexChanged += new System.EventHandler(this.lsvDocuments_SelectedIndexChanged);
            // 
            // bwFill
            // 
            this.bwFill.WorkerSupportsCancellation = true;
            this.bwFill.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwFill_DoWork);
            this.bwFill.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwFill_RunWorkerCompleted);
            // 
            // bwOpen
            // 
            this.bwOpen.WorkerSupportsCancellation = true;
            this.bwOpen.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwOpen_DoWork);
            this.bwOpen.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwOpen_RunWorkerCompleted);
            // 
            // GoogleDocsOpenForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(554, 368);
            this.Controls.Add(this.lsvDocuments);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GoogleDocsOpenForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Open from Google Docs";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GoogleDocsOpenForm_FormClosing);
            this.Load += new System.EventHandler(this.GoogleDocsSaveForm_Load);
            this.Resize += new System.EventHandler(this.GoogleDocsOpenForm_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ListView lsvDocuments;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.ComponentModel.BackgroundWorker bwFill;
        private System.ComponentModel.BackgroundWorker bwOpen;
    }
}