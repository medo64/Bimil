using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Medo.Security.Cryptography.PasswordSafe;

namespace Bimil {
    internal partial class SearchWeakForm : Form {
        public SearchWeakForm(Document document) {
            InitializeComponent();
            this.Font = SystemFonts.MessageBoxFont;
            Medo.Windows.Forms.State.SetupOnLoadAndClose(this);
            lsvEntries.SmallImageList = Helpers.GetImageList(this, "picWarning");

            this.Document = document;

            bwSearchWeak.RunWorkerAsync();
        }

        private readonly Document Document;


        private void Form_FormClosing(object sender, FormClosingEventArgs e) {
            if (bwSearchWeak.IsBusy) { bwSearchWeak.CancelAsync(); }
        }

        private void Form_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyData == Keys.Escape) {
                this.Close();
            }
        }

        private void Form_Resize(object sender, System.EventArgs e) {
            lsvEntries.Columns[0].Width = lsvEntries.ClientSize.Width;
        }


        private void bwSearchWeak_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            var sw = Stopwatch.StartNew();
            foreach (var entry in this.Document.Entries) {
                foreach (var record in entry.Records) {
                    if (record.RecordType == RecordType.Password) {
                        if (BadPasswords.IsCommon(record.Text, out var matchedPassword)) {
                            var lvi = new ListViewItem(entry.Title) {
                                Tag = entry,
                                ImageIndex = 0,
                                ToolTipText = $"Password is similar to a common password ({matchedPassword})."
                            };
                            bwSearchWeak.ReportProgress(0, lvi);
                            break;
                        }
                    }
                }
            }
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Items searched at {0:0.0} ms", sw.ElapsedMilliseconds));
        }

        private void bwSearchWeak_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e) {
            var lvi = (ListViewItem)e.UserState;

            lsvEntries.BeginUpdate();
            lsvEntries.Items.Add(lvi);
            lsvEntries.EndUpdate();
        }

        private void bwSearchWeak_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
        }
    }
}
