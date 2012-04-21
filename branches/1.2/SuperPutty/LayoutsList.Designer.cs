namespace SuperPutty
{
    partial class LayoutsList
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
            this.listBoxLayouts = new System.Windows.Forms.ListBox();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadInNewInstanceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.setAsDefaultLayoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBoxLayouts
            // 
            this.listBoxLayouts.ContextMenuStrip = this.contextMenuStrip;
            this.listBoxLayouts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxLayouts.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxLayouts.FormattingEnabled = true;
            this.listBoxLayouts.ItemHeight = 15;
            this.listBoxLayouts.Location = new System.Drawing.Point(0, 0);
            this.listBoxLayouts.Margin = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.listBoxLayouts.Name = "listBoxLayouts";
            this.listBoxLayouts.Size = new System.Drawing.Size(235, 287);
            this.listBoxLayouts.Sorted = true;
            this.listBoxLayouts.TabIndex = 0;
            this.listBoxLayouts.DoubleClick += new System.EventHandler(this.listBoxLayouts_DoubleClick);
            this.listBoxLayouts.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listBoxLayouts_MouseDown);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.loadInNewInstanceToolStripMenuItem,
            this.toolStripMenuItem1,
            this.setAsDefaultLayoutToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(188, 76);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.loadToolStripMenuItem.Text = "Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // loadInNewInstanceToolStripMenuItem
            // 
            this.loadInNewInstanceToolStripMenuItem.Name = "loadInNewInstanceToolStripMenuItem";
            this.loadInNewInstanceToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.loadInNewInstanceToolStripMenuItem.Text = "Load in New Instance";
            this.loadInNewInstanceToolStripMenuItem.Click += new System.EventHandler(this.loadInNewInstanceToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(184, 6);
            // 
            // setAsDefaultLayoutToolStripMenuItem
            // 
            this.setAsDefaultLayoutToolStripMenuItem.Name = "setAsDefaultLayoutToolStripMenuItem";
            this.setAsDefaultLayoutToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.setAsDefaultLayoutToolStripMenuItem.Text = "Set as Default Layout";
            this.setAsDefaultLayoutToolStripMenuItem.Click += new System.EventHandler(this.setAsDefaultLayoutToolStripMenuItem_Click);
            // 
            // LayoutsList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(235, 287);
            this.Controls.Add(this.listBoxLayouts);
            this.Name = "LayoutsList";
            this.Text = "Layouts";
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxLayouts;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadInNewInstanceToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem setAsDefaultLayoutToolStripMenuItem;
    }
}