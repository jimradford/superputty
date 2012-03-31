namespace SuperPutty
{
    partial class ctlPuttyPanel
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
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.closeSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutPuttyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renameTabToolStripMenuItem,
            this.toolStripSeparator2,
            this.newSessionToolStripMenuItem,
            this.duplicateSessionToolStripMenuItem,
            this.toolStripSeparator3,
            this.aboutPuttyToolStripMenuItem,
            this.toolStripMenuItem1,
            this.closeSessionToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowImageMargin = false;
            this.contextMenuStrip1.Size = new System.Drawing.Size(142, 154);
            // 
            // newSessionToolStripMenuItem
            // 
            this.newSessionToolStripMenuItem.Enabled = false;
            this.newSessionToolStripMenuItem.Name = "newSessionToolStripMenuItem";
            this.newSessionToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.newSessionToolStripMenuItem.Text = "New Session";
            this.newSessionToolStripMenuItem.Click += new System.EventHandler(this.newSessionToolStripMenuItem_Click);
            // 
            // duplicateSessionToolStripMenuItem
            // 
            this.duplicateSessionToolStripMenuItem.Name = "duplicateSessionToolStripMenuItem";
            this.duplicateSessionToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.duplicateSessionToolStripMenuItem.Text = "Duplicate Session";
            this.duplicateSessionToolStripMenuItem.Click += new System.EventHandler(this.duplicateSessionToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(138, 6);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(138, 6);
            // 
            // closeSessionToolStripMenuItem
            // 
            this.closeSessionToolStripMenuItem.Name = "closeSessionToolStripMenuItem";
            this.closeSessionToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.closeSessionToolStripMenuItem.Text = "Close";
            this.closeSessionToolStripMenuItem.Click += new System.EventHandler(this.closeSessionToolStripMenuItem_Click);
            // 
            // renameTabToolStripMenuItem
            // 
            this.renameTabToolStripMenuItem.Name = "renameTabToolStripMenuItem";
            this.renameTabToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.renameTabToolStripMenuItem.Text = "Rename";
            this.renameTabToolStripMenuItem.Click += new System.EventHandler(this.renameTabToolStripMenuItem_Click);
            // 
            // aboutPuttyToolStripMenuItem
            // 
            this.aboutPuttyToolStripMenuItem.Name = "aboutPuttyToolStripMenuItem";
            this.aboutPuttyToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.aboutPuttyToolStripMenuItem.Text = "About Putty";
            this.aboutPuttyToolStripMenuItem.Click += new System.EventHandler(this.aboutPuttyToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(138, 6);
            // 
            // ctlPuttyPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1339, 203);
            this.Name = "ctlPuttyPanel";
            this.TabPageContextMenuStrip = this.contextMenuStrip1;
            this.Text = "PuTTY Session";
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem newSessionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem duplicateSessionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeSessionToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem renameTabToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutPuttyToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;

    }
}