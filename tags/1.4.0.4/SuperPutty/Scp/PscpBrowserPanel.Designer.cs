namespace SuperPutty.Scp
{
    partial class PscpBrowserPanel
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainerUpDown = new System.Windows.Forms.SplitContainer();
            this.splitContainerBrowsers = new System.Windows.Forms.SplitContainer();
            this.browserViewLocal = new SuperPutty.Scp.BrowserView();
            this.browserViewRemote = new SuperPutty.Scp.BrowserView();
            this.fileTransferView = new SuperPutty.Scp.FileTransferView();
            this.splitContainerUpDown.Panel1.SuspendLayout();
            this.splitContainerUpDown.Panel2.SuspendLayout();
            this.splitContainerUpDown.SuspendLayout();
            this.splitContainerBrowsers.Panel1.SuspendLayout();
            this.splitContainerBrowsers.Panel2.SuspendLayout();
            this.splitContainerBrowsers.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerUpDown
            // 
            this.splitContainerUpDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerUpDown.Location = new System.Drawing.Point(3, 3);
            this.splitContainerUpDown.Name = "splitContainerUpDown";
            this.splitContainerUpDown.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerUpDown.Panel1
            // 
            this.splitContainerUpDown.Panel1.Controls.Add(this.splitContainerBrowsers);
            // 
            // splitContainerUpDown.Panel2
            // 
            this.splitContainerUpDown.Panel2.Controls.Add(this.fileTransferView);
            this.splitContainerUpDown.Size = new System.Drawing.Size(907, 733);
            this.splitContainerUpDown.SplitterDistance = 538;
            this.splitContainerUpDown.TabIndex = 0;
            // 
            // splitContainerBrowsers
            // 
            this.splitContainerBrowsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerBrowsers.Location = new System.Drawing.Point(0, 0);
            this.splitContainerBrowsers.Name = "splitContainerBrowsers";
            // 
            // splitContainerBrowsers.Panel1
            // 
            this.splitContainerBrowsers.Panel1.Controls.Add(this.browserViewLocal);
            // 
            // splitContainerBrowsers.Panel2
            // 
            this.splitContainerBrowsers.Panel2.Controls.Add(this.browserViewRemote);
            this.splitContainerBrowsers.Size = new System.Drawing.Size(907, 538);
            this.splitContainerBrowsers.SplitterDistance = 441;
            this.splitContainerBrowsers.TabIndex = 0;
            // 
            // browserViewLocal
            // 
            this.browserViewLocal.ConfirmTransfer = false;
            this.browserViewLocal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.browserViewLocal.Location = new System.Drawing.Point(0, 0);
            this.browserViewLocal.Name = "browserViewLocal";
            this.browserViewLocal.Size = new System.Drawing.Size(441, 538);
            this.browserViewLocal.TabIndex = 0;
            // 
            // browserViewRemote
            // 
            this.browserViewRemote.ConfirmTransfer = false;
            this.browserViewRemote.Dock = System.Windows.Forms.DockStyle.Fill;
            this.browserViewRemote.Location = new System.Drawing.Point(0, 0);
            this.browserViewRemote.Name = "browserViewRemote";
            this.browserViewRemote.Size = new System.Drawing.Size(462, 538);
            this.browserViewRemote.TabIndex = 0;
            // 
            // fileTransferView
            // 
            this.fileTransferView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileTransferView.Location = new System.Drawing.Point(0, 0);
            this.fileTransferView.Name = "fileTransferView";
            this.fileTransferView.Size = new System.Drawing.Size(907, 191);
            this.fileTransferView.TabIndex = 0;
            // 
            // PscpBrowserPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(913, 739);
            this.Controls.Add(this.splitContainerUpDown);
            this.Name = "PscpBrowserPanel";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "PscpBrowserPanel";
            this.splitContainerUpDown.Panel1.ResumeLayout(false);
            this.splitContainerUpDown.Panel2.ResumeLayout(false);
            this.splitContainerUpDown.ResumeLayout(false);
            this.splitContainerBrowsers.Panel1.ResumeLayout(false);
            this.splitContainerBrowsers.Panel2.ResumeLayout(false);
            this.splitContainerBrowsers.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerUpDown;
        private System.Windows.Forms.SplitContainer splitContainerBrowsers;
        private BrowserView browserViewLocal;
        private BrowserView browserViewRemote;
        private FileTransferView fileTransferView;

    }
}