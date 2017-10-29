namespace Bimil {
    partial class SearchWeakForm {
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
            this.lsvEntries = new System.Windows.Forms.ListView();
            this.lsvEntries_colTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.bwSearchWeak = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
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
            this.lsvEntries.Location = new System.Drawing.Point(12, 12);
            this.lsvEntries.MultiSelect = false;
            this.lsvEntries.Name = "lsvEntries";
            this.lsvEntries.ShowItemToolTips = true;
            this.lsvEntries.Size = new System.Drawing.Size(358, 329);
            this.lsvEntries.TabIndex = 4;
            this.lsvEntries.UseCompatibleStateImageBehavior = false;
            this.lsvEntries.View = System.Windows.Forms.View.Details;
            // 
            // lsvEntries_colTitle
            // 
            this.lsvEntries_colTitle.Text = "Title";
            // 
            // bwSearchWeak
            // 
            this.bwSearchWeak.WorkerReportsProgress = true;
            this.bwSearchWeak.WorkerSupportsCancellation = true;
            this.bwSearchWeak.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwSearchWeak_DoWork);
            this.bwSearchWeak.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bwSearchWeak_ProgressChanged);
            this.bwSearchWeak.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwSearchWeak_RunWorkerCompleted);
            // 
            // SearchWeakForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(382, 353);
            this.Controls.Add(this.lsvEntries);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(300, 300);
            this.Name = "SearchWeakForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Search weak passwords";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_KeyDown);
            this.Resize += new System.EventHandler(this.Form_Resize);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ListView lsvEntries;
        private System.Windows.Forms.ColumnHeader lsvEntries_colTitle;
        private System.ComponentModel.BackgroundWorker bwSearchWeak;
    }
}