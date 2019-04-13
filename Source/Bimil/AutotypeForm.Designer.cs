namespace Bimil {
    partial class AutotypeForm {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AutotypeForm));
            this.tmrRestore = new System.Windows.Forms.Timer(this.components);
            this.bwType = new System.ComponentModel.BackgroundWorker();
            this.tryProgress = new System.Windows.Forms.NotifyIcon(this.components);
            this.tip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // tmrRestore
            // 
            this.tmrRestore.Interval = 500;
            this.tmrRestore.Tick += new System.EventHandler(this.tmrRestore_Tick);
            // 
            // bwType
            // 
            this.bwType.WorkerReportsProgress = true;
            this.bwType.WorkerSupportsCancellation = true;
            this.bwType.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwType_DoWork);
            this.bwType.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bwType_ProgressChanged);
            this.bwType.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwType_RunWorkerCompleted);
            // 
            // tryProgress
            // 
            this.tryProgress.Text = "Bimil auto-type";
            // 
            // AutotypeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(222, 253);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AutotypeForm";
            this.Text = "Bimil auto-type";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer tmrRestore;
        private System.ComponentModel.BackgroundWorker bwType;
        private System.Windows.Forms.NotifyIcon tryProgress;
        private System.Windows.Forms.ToolTip tip;
    }
}