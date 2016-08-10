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
            this.tmrType = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // tmrRestore
            // 
            this.tmrRestore.Interval = 500;
            this.tmrRestore.Tick += new System.EventHandler(this.tmrRestore_Tick);
            // 
            // tmrType
            // 
            this.tmrType.Tick += new System.EventHandler(this.tmrType_Tick);
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
        private System.Windows.Forms.Timer tmrType;
    }
}