namespace Bimil {
    partial class FillForm {
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
            this.tmrRestore = new System.Windows.Forms.Timer(this.components);
            this.bwType = new System.ComponentModel.BackgroundWorker();
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
            // FillForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(222, 253);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FillForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Bimil";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer tmrRestore;
        private System.ComponentModel.BackgroundWorker bwType;
    }
}