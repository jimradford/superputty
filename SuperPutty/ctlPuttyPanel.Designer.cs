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
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.renameTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.acceptCommandsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripPuttySep1 = new System.Windows.Forms.ToolStripSeparator();
            this.eventLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripPuttySep2 = new System.Windows.Forms.ToolStripSeparator();
            this.changeSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyAllToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restartSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearScrollbackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetTerminalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutPuttyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.closeSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeOthersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeOthersToTheRightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newSessionToolStripMenuItem,
            this.duplicateSessionToolStripMenuItem,
            this.toolStripSeparator1,
            this.renameTabToolStripMenuItem,
            this.refreshToolStripMenuItem,
            this.acceptCommandsToolStripMenuItem,
            this.toolStripPuttySep1,
            this.eventLogToolStripMenuItem,
            this.toolStripPuttySep2,
            this.changeSettingsToolStripMenuItem,
            this.copyAllToClipboardToolStripMenuItem,
            this.restartSessionToolStripMenuItem,
            this.clearScrollbackToolStripMenuItem,
            this.resetTerminalToolStripMenuItem,
            this.toolStripSeparator3,
            this.aboutPuttyToolStripMenuItem,
            this.toolStripSeparator4,
            this.closeSessionToolStripMenuItem,
            this.closeOthersToolStripMenuItem,
            this.closeOthersToTheRightToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(207, 386);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // newSessionToolStripMenuItem
            // 
            this.newSessionToolStripMenuItem.Name = "newSessionToolStripMenuItem";
            this.newSessionToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.newSessionToolStripMenuItem.Text = "New Session";
            // 
            // duplicateSessionToolStripMenuItem
            // 
            this.duplicateSessionToolStripMenuItem.Name = "duplicateSessionToolStripMenuItem";
            this.duplicateSessionToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.duplicateSessionToolStripMenuItem.Text = "Duplicate Session";
            this.duplicateSessionToolStripMenuItem.Click += new System.EventHandler(this.duplicateSessionToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.BackColor = System.Drawing.Color.Red;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(203, 6);
            // 
            // renameTabToolStripMenuItem
            // 
            this.renameTabToolStripMenuItem.Name = "renameTabToolStripMenuItem";
            this.renameTabToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.renameTabToolStripMenuItem.Text = "Rename Tab";
            this.renameTabToolStripMenuItem.Click += new System.EventHandler(this.renameTabToolStripMenuItem_Click);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.refreshToolStripMenuItem.Text = "Refresh Tab";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // acceptCommandsToolStripMenuItem
            // 
            this.acceptCommandsToolStripMenuItem.Checked = true;
            this.acceptCommandsToolStripMenuItem.CheckOnClick = true;
            this.acceptCommandsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.acceptCommandsToolStripMenuItem.Name = "acceptCommandsToolStripMenuItem";
            this.acceptCommandsToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.acceptCommandsToolStripMenuItem.Text = "Accept Commands";
            // 
            // toolStripPuttySep1
            // 
            this.toolStripPuttySep1.Name = "toolStripPuttySep1";
            this.toolStripPuttySep1.Size = new System.Drawing.Size(203, 6);
            // 
            // eventLogToolStripMenuItem
            // 
            this.eventLogToolStripMenuItem.Name = "eventLogToolStripMenuItem";
            this.eventLogToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.eventLogToolStripMenuItem.Tag = "0x0010";
            this.eventLogToolStripMenuItem.Text = "Event Log";
            this.eventLogToolStripMenuItem.Click += new System.EventHandler(this.puTTYMenuTSMI_Click);
            // 
            // toolStripPuttySep2
            // 
            this.toolStripPuttySep2.Name = "toolStripPuttySep2";
            this.toolStripPuttySep2.Size = new System.Drawing.Size(203, 6);
            // 
            // changeSettingsToolStripMenuItem
            // 
            this.changeSettingsToolStripMenuItem.Name = "changeSettingsToolStripMenuItem";
            this.changeSettingsToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.changeSettingsToolStripMenuItem.Tag = "0x0050";
            this.changeSettingsToolStripMenuItem.Text = "Change Settings";
            this.changeSettingsToolStripMenuItem.Click += new System.EventHandler(this.puTTYMenuTSMI_Click);
            // 
            // copyAllToClipboardToolStripMenuItem
            // 
            this.copyAllToClipboardToolStripMenuItem.Name = "copyAllToClipboardToolStripMenuItem";
            this.copyAllToClipboardToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.copyAllToClipboardToolStripMenuItem.Tag = "0x0170";
            this.copyAllToClipboardToolStripMenuItem.Text = "Copy All to Clipboard";
            this.copyAllToClipboardToolStripMenuItem.Click += new System.EventHandler(this.puTTYMenuTSMI_Click);
            // 
            // restartSessionToolStripMenuItem
            // 
            this.restartSessionToolStripMenuItem.Name = "restartSessionToolStripMenuItem";
            this.restartSessionToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.restartSessionToolStripMenuItem.Tag = "0x0040";
            this.restartSessionToolStripMenuItem.Text = "Restart Session";
            this.restartSessionToolStripMenuItem.Click += new System.EventHandler(this.puTTYMenuTSMI_Click);
            // 
            // clearScrollbackToolStripMenuItem
            // 
            this.clearScrollbackToolStripMenuItem.Name = "clearScrollbackToolStripMenuItem";
            this.clearScrollbackToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.clearScrollbackToolStripMenuItem.Tag = "0x0060";
            this.clearScrollbackToolStripMenuItem.Text = "Clear Scrollback";
            this.clearScrollbackToolStripMenuItem.Click += new System.EventHandler(this.puTTYMenuTSMI_Click);
            // 
            // resetTerminalToolStripMenuItem
            // 
            this.resetTerminalToolStripMenuItem.Name = "resetTerminalToolStripMenuItem";
            this.resetTerminalToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.resetTerminalToolStripMenuItem.Tag = "0x0070";
            this.resetTerminalToolStripMenuItem.Text = "Reset Terminal";
            this.resetTerminalToolStripMenuItem.Click += new System.EventHandler(this.puTTYMenuTSMI_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(203, 6);
            // 
            // aboutPuttyToolStripMenuItem
            // 
            this.aboutPuttyToolStripMenuItem.Name = "aboutPuttyToolStripMenuItem";
            this.aboutPuttyToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.aboutPuttyToolStripMenuItem.Text = "About Putty";
            this.aboutPuttyToolStripMenuItem.Click += new System.EventHandler(this.aboutPuttyToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(203, 6);
            // 
            // closeSessionToolStripMenuItem
            // 
            this.closeSessionToolStripMenuItem.Name = "closeSessionToolStripMenuItem";
            this.closeSessionToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.closeSessionToolStripMenuItem.Text = "Close";
            this.closeSessionToolStripMenuItem.Click += new System.EventHandler(this.closeSessionToolStripMenuItem_Click);
            // 
            // closeOthersToolStripMenuItem
            // 
            this.closeOthersToolStripMenuItem.Name = "closeOthersToolStripMenuItem";
            this.closeOthersToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.closeOthersToolStripMenuItem.Text = "Close Others ";
            this.closeOthersToolStripMenuItem.Click += new System.EventHandler(this.closeOthersToolStripMenuItem_Click);
            // 
            // closeOthersToTheRightToolStripMenuItem
            // 
            this.closeOthersToTheRightToolStripMenuItem.Name = "closeOthersToTheRightToolStripMenuItem";
            this.closeOthersToTheRightToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.closeOthersToTheRightToolStripMenuItem.Text = "Close Others to the Right";
            this.closeOthersToTheRightToolStripMenuItem.Click += new System.EventHandler(this.closeOthersToTheRightToolStripMenuItem_Click);
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
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripPuttySep1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem duplicateSessionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeSessionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameTabToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutPuttyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newSessionToolStripMenuItem;

        private System.Windows.Forms.ToolStripMenuItem eventLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restartSessionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearScrollbackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetTerminalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyAllToClipboardToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripPuttySep2;
        private System.Windows.Forms.ToolStripMenuItem changeSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem acceptCommandsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeOthersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeOthersToTheRightToolStripMenuItem;

    }
}