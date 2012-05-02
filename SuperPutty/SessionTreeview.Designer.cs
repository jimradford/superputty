namespace SuperPutty
{
    partial class SessionTreeview
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SessionTreeview));
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStripAddTreeItem = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createLikeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.fileBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripFolder = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newSessionToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.newFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timerDelayedSave = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStripAddTreeItem.SuspendLayout();
            this.contextMenuStripFolder.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.AllowDrop = true;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.ShowLines = false;
            this.treeView1.ShowRootLines = false;
            this.treeView1.Size = new System.Drawing.Size(167, 264);
            this.treeView1.TabIndex = 0;
            this.treeView1.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeView1_ItemDrag);
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            this.treeView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView1_DragDrop);
            this.treeView1.DragOver += new System.Windows.Forms.DragEventHandler(this.treeView1_DragOver);
            this.treeView1.DoubleClick += new System.EventHandler(this.treeView1_DoubleClick);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "folder.png");
            this.imageList1.Images.SetKeyName(1, "computer.png");
            // 
            // contextMenuStripAddTreeItem
            // 
            this.contextMenuStripAddTreeItem.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newSessionToolStripMenuItem,
            this.createLikeToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator1,
            this.connectToolStripMenuItem,
            this.toolStripSeparator2,
            this.fileBrowserToolStripMenuItem});
            this.contextMenuStripAddTreeItem.Name = "contextMenuStripAddTreeItem";
            this.contextMenuStripAddTreeItem.ShowImageMargin = false;
            this.contextMenuStripAddTreeItem.Size = new System.Drawing.Size(128, 170);
            // 
            // newSessionToolStripMenuItem
            // 
            this.newSessionToolStripMenuItem.Name = "newSessionToolStripMenuItem";
            this.newSessionToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.newSessionToolStripMenuItem.Text = "New";
            this.newSessionToolStripMenuItem.Click += new System.EventHandler(this.CreateOrEditSessionToolStripMenuItem_Click);
            // 
            // createLikeToolStripMenuItem
            // 
            this.createLikeToolStripMenuItem.Name = "createLikeToolStripMenuItem";
            this.createLikeToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.createLikeToolStripMenuItem.Text = "Copy As";
            this.createLikeToolStripMenuItem.Click += new System.EventHandler(this.CreateOrEditSessionToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.settingsToolStripMenuItem.Text = "Edit";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.CreateOrEditSessionToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(124, 6);
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            this.connectToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.connectToolStripMenuItem.Text = "Connect";
            this.connectToolStripMenuItem.Click += new System.EventHandler(this.connectToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(124, 6);
            // 
            // fileBrowserToolStripMenuItem
            // 
            this.fileBrowserToolStripMenuItem.Name = "fileBrowserToolStripMenuItem";
            this.fileBrowserToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.fileBrowserToolStripMenuItem.Text = "File Transfer";
            this.fileBrowserToolStripMenuItem.Click += new System.EventHandler(this.fileBrowserToolStripMenuItem_Click);
            // 
            // contextMenuStripFolder
            // 
            this.contextMenuStripFolder.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newSessionToolStripMenuItem1,
            this.toolStripSeparator3,
            this.newFolderToolStripMenuItem,
            this.removeFolderToolStripMenuItem,
            this.toolStripMenuItem1,
            this.renameToolStripMenuItem});
            this.contextMenuStripFolder.Name = "contextMenuStripAddTreeItem";
            this.contextMenuStripFolder.ShowImageMargin = false;
            this.contextMenuStripFolder.Size = new System.Drawing.Size(129, 104);
            this.contextMenuStripFolder.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripFolder_Opening);
            // 
            // newSessionToolStripMenuItem1
            // 
            this.newSessionToolStripMenuItem1.Name = "newSessionToolStripMenuItem1";
            this.newSessionToolStripMenuItem1.Size = new System.Drawing.Size(128, 22);
            this.newSessionToolStripMenuItem1.Text = "New";
            this.newSessionToolStripMenuItem1.Click += new System.EventHandler(this.CreateOrEditSessionToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(125, 6);
            // 
            // newFolderToolStripMenuItem
            // 
            this.newFolderToolStripMenuItem.Name = "newFolderToolStripMenuItem";
            this.newFolderToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.newFolderToolStripMenuItem.Text = "New Folder";
            this.newFolderToolStripMenuItem.Click += new System.EventHandler(this.newFolderToolStripMenuItem_Click);
            // 
            // removeFolderToolStripMenuItem
            // 
            this.removeFolderToolStripMenuItem.Name = "removeFolderToolStripMenuItem";
            this.removeFolderToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.removeFolderToolStripMenuItem.Text = "Remove Folder";
            this.removeFolderToolStripMenuItem.Click += new System.EventHandler(this.removeFolderToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(125, 6);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // timerDelayedSave
            // 
            this.timerDelayedSave.Interval = 3000;
            this.timerDelayedSave.Tick += new System.EventHandler(this.timerDelayedSave_Tick);
            // 
            // SessionTreeview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(167, 264);
            this.Controls.Add(this.treeView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SessionTreeview";
            this.ShowIcon = false;
            this.TabText = "Sessions";
            this.Text = "PuTTY Sessions";
            this.contextMenuStripAddTreeItem.ResumeLayout(false);
            this.contextMenuStripFolder.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripAddTreeItem;
        private System.Windows.Forms.ToolStripMenuItem newSessionToolStripMenuItem;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem fileBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripFolder;
        private System.Windows.Forms.ToolStripMenuItem newSessionToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem newFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeFolderToolStripMenuItem;
        private System.Windows.Forms.Timer timerDelayedSave;
        private System.Windows.Forms.ToolStripMenuItem createLikeToolStripMenuItem;
    }
}