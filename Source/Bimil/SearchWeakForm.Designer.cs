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
            this.btnCancel = new System.Windows.Forms.Button();
            this.chbIncludeHibp = new System.Windows.Forms.CheckBox();
            this.bwSearchHibp = new System.ComponentModel.BackgroundWorker();
            this.sta = new System.Windows.Forms.StatusStrip();
            this.staStatusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.staProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.sta.SuspendLayout();
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
            this.lsvEntries.Size = new System.Drawing.Size(358, 310);
            this.lsvEntries.TabIndex = 0;
            this.lsvEntries.UseCompatibleStateImageBehavior = false;
            this.lsvEntries.View = System.Windows.Forms.View.Details;
            this.lsvEntries.ItemActivate += new System.EventHandler(this.lsvEntries_ItemActivate);
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
            this.bwSearchWeak.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bwSearch_ProgressChanged);
            this.bwSearchWeak.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwSearchWeak_RunWorkerCompleted);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(280, 340);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 28);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // chbIncludeHibp
            // 
            this.chbIncludeHibp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chbIncludeHibp.AutoSize = true;
            this.chbIncludeHibp.Location = new System.Drawing.Point(12, 340);
            this.chbIncludeHibp.Name = "chbIncludeHibp";
            this.chbIncludeHibp.Size = new System.Drawing.Size(223, 21);
            this.chbIncludeHibp.TabIndex = 4;
            this.chbIncludeHibp.TabStop = false;
            this.chbIncludeHibp.Tag = "If selected, search will be done toward \';--Have i been pwned? website to check f" +
    "or leaked passwords.";
            this.chbIncludeHibp.Text = "Include \';--&have i been pwned?";
            this.chbIncludeHibp.UseVisualStyleBackColor = true;
            this.chbIncludeHibp.CheckedChanged += new System.EventHandler(this.chbIncludeHibp_CheckedChanged);
            // 
            // bwSearchHibp
            // 
            this.bwSearchHibp.WorkerReportsProgress = true;
            this.bwSearchHibp.WorkerSupportsCancellation = true;
            this.bwSearchHibp.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwSearchHibp_DoWork);
            this.bwSearchHibp.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bwSearch_ProgressChanged);
            this.bwSearchHibp.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwSearchHibp_RunWorkerCompleted);
            // 
            // sta
            // 
            this.sta.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.sta.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.staStatusText,
            this.staProgress});
            this.sta.Location = new System.Drawing.Point(0, 369);
            this.sta.Name = "sta";
            this.sta.Size = new System.Drawing.Size(382, 24);
            this.sta.TabIndex = 5;
            this.sta.Text = "statusStrip1";
            // 
            // staStatusText
            // 
            this.staStatusText.Margin = new System.Windows.Forms.Padding(3, 3, 0, 2);
            this.staStatusText.Name = "staStatusText";
            this.staStatusText.Size = new System.Drawing.Size(223, 19);
            this.staStatusText.Spring = true;
            this.staStatusText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // staProgress
            // 
            this.staProgress.Name = "staProgress";
            this.staProgress.Size = new System.Drawing.Size(100, 18);
            // 
            // SearchWeakForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(382, 393);
            this.Controls.Add(this.sta);
            this.Controls.Add(this.chbIncludeHibp);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lsvEntries);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 400);
            this.Name = "SearchWeakForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Search weak passwords";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_KeyDown);
            this.Resize += new System.EventHandler(this.Form_Resize);
            this.sta.ResumeLayout(false);
            this.sta.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListView lsvEntries;
        private System.Windows.Forms.ColumnHeader lsvEntries_colTitle;
        private System.ComponentModel.BackgroundWorker bwSearchWeak;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chbIncludeHibp;
        private System.ComponentModel.BackgroundWorker bwSearchHibp;
        private System.Windows.Forms.StatusStrip sta;
        private System.Windows.Forms.ToolStripStatusLabel staStatusText;
        private System.Windows.Forms.ToolStripProgressBar staProgress;
    }
}