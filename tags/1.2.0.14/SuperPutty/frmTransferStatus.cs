using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SuperPutty
{
    public partial class frmTransferStatus : ToolWindow
    {
        public TransferUpdateCallback m_callback;
        public frmTransferStatus()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Update the progress bars and associated text labels with transfer progress
        /// </summary>
        /// <param name="currentFile">Object containing the current file being transferred</param>
        /// <param name="sofar">The bytes transferred sofar</param>
        /// <param name="total">The total number of bytes we're expecting</param>
        /// <param name="fileNum">The current file number being transferred.</param>
        /// <param name="totalFiles">The total number of files we're expecting to be transferred</param>
        public void UpdateProgress(FileTransferStatus currentFile, int sofar, int total, int fileNum, int totalFiles)
        {
            if (this.InvokeRequired)
                this.BeginInvoke((MethodInvoker)delegate
                {
                    UpdateProgress(currentFile, sofar, total, fileNum, totalFiles);
                });
            else
            {
                labelCurrentFile.Text = String.Format("File {0}, {1} kB Transferred ({2} kB/s) Time left {3}",
                    currentFile.Filename, currentFile.BytesTransferred, currentFile.TransferRate, currentFile.TimeLeft);
                progressBarCurrentFile.Value = currentFile.PercentComplete;
                labelCurrentPercent.Text = currentFile.PercentComplete + "%";

                labelOverall.Visible = progressBarOverall.Visible  = labelOverallPct.Visible = totalFiles > 1;
                
                if (fileNum >= totalFiles)
                {
                    progressBarOverall.Value = 100;
                    labelOverallPct.Text = "100%";
                    button1.Text = "Close";
                }
                else if(totalFiles > 1)
                {
                    progressBarOverall.Value = (int)(((float)sofar / (float)total) * 100);
                    labelOverallPct.Text = String.Format("{0}%", progressBarOverall.Value);

                    labelOverall.Text = String.Format("Transfered {0}/{1} kB (file {2} of {3})",
                    sofar, total, fileNum, totalFiles);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (m_callback != null)
                m_callback(false, true, new FileTransferStatus());
        }
    }
}
