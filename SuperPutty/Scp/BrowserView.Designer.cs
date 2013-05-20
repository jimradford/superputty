namespace SuperPutty.Scp
{
    partial class BrowserView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BrowserView));
            this.panelBottom = new System.Windows.Forms.Panel();
            this.labelBrowserState = new System.Windows.Forms.Label();
            this.bindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.lblStatus = new System.Windows.Forms.Label();
            this.listViewFiles = new System.Windows.Forms.ListView();
            this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderModTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderPermissions = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderOwner = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderGroup = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageListLarge = new System.Windows.Forms.ImageList(this.components);
            this.imageListSmall = new System.Windows.Forms.ImageList(this.components);
            this.panelTop = new System.Windows.Forms.Panel();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.toolStripTools = new System.Windows.Forms.ToolStrip();
            this.toolStripSplitButtonView = new System.Windows.Forms.ToolStripSplitButton();
            this.detailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.smallIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.largeIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsBtnRefresh = new System.Windows.Forms.ToolStripButton();
            this.panelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).BeginInit();
            this.panelTop.SuspendLayout();
            this.toolStripTools.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.labelBrowserState);
            this.panelBottom.Controls.Add(this.lblStatus);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 581);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(600, 22);
            this.panelBottom.TabIndex = 0;
            // 
            // labelBrowserState
            // 
            this.labelBrowserState.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "BrowserState", true));
            this.labelBrowserState.Dock = System.Windows.Forms.DockStyle.Right;
            this.labelBrowserState.Location = new System.Drawing.Point(531, 0);
            this.labelBrowserState.Name = "labelBrowserState";
            this.labelBrowserState.Size = new System.Drawing.Size(69, 22);
            this.labelBrowserState.TabIndex = 1;
            this.labelBrowserState.Text = "<ConnState>";
            this.labelBrowserState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bindingSource
            // 
            this.bindingSource.DataSource = typeof(SuperPutty.Scp.IBrowserViewModel);
            // 
            // lblStatus
            // 
            this.lblStatus.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "Status", true));
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStatus.Location = new System.Drawing.Point(0, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.lblStatus.Size = new System.Drawing.Size(600, 22);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "<Status>";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // listViewFiles
            // 
            this.listViewFiles.AllowDrop = true;
            this.listViewFiles.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderSize,
            this.columnHeaderModTime,
            this.columnHeaderPermissions,
            this.columnHeaderOwner,
            this.columnHeaderGroup});
            this.listViewFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewFiles.FullRowSelect = true;
            this.listViewFiles.HideSelection = false;
            this.listViewFiles.LargeImageList = this.imageListLarge;
            this.listViewFiles.Location = new System.Drawing.Point(0, 51);
            this.listViewFiles.Name = "listViewFiles";
            this.listViewFiles.Size = new System.Drawing.Size(600, 530);
            this.listViewFiles.SmallImageList = this.imageListSmall;
            this.listViewFiles.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listViewFiles.TabIndex = 1;
            this.listViewFiles.UseCompatibleStateImageBehavior = false;
            this.listViewFiles.View = System.Windows.Forms.View.Details;
            this.listViewFiles.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewFiles_ColumnClick);
            this.listViewFiles.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listViewFiles_ItemDrag);
            this.listViewFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.listViewFiles_DragDrop);
            this.listViewFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.listViewFiles_DragEnter);
            this.listViewFiles.DragOver += new System.Windows.Forms.DragEventHandler(this.listViewFiles_DragOver);
            this.listViewFiles.DragLeave += new System.EventHandler(this.listViewFiles_DragLeave);
            this.listViewFiles.DoubleClick += new System.EventHandler(this.listViewFiles_DoubleClick);
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Name";
            this.columnHeaderName.Width = 180;
            // 
            // columnHeaderSize
            // 
            this.columnHeaderSize.Text = "Size";
            this.columnHeaderSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // columnHeaderModTime
            // 
            this.columnHeaderModTime.Text = "Last Mod";
            this.columnHeaderModTime.Width = 135;
            // 
            // columnHeaderPermissions
            // 
            this.columnHeaderPermissions.Text = "Perm";
            // 
            // columnHeaderOwner
            // 
            this.columnHeaderOwner.Text = "Owner";
            this.columnHeaderOwner.Width = 100;
            // 
            // columnHeaderGroup
            // 
            this.columnHeaderGroup.Text = "Group";
            // 
            // imageListLarge
            // 
            this.imageListLarge.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListLarge.ImageStream")));
            this.imageListLarge.TransparentColor = System.Drawing.Color.Fuchsia;
            this.imageListLarge.Images.SetKeyName(0, "File-Large.png");
            this.imageListLarge.Images.SetKeyName(1, "Folder-Large.png");
            this.imageListLarge.Images.SetKeyName(2, "Folder-Green-Large.png");
            // 
            // imageListSmall
            // 
            this.imageListSmall.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListSmall.ImageStream")));
            this.imageListSmall.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListSmall.Images.SetKeyName(0, "page-blue.png");
            this.imageListSmall.Images.SetKeyName(1, "folder.png");
            this.imageListSmall.Images.SetKeyName(2, "folder-up.png");
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.txtPath);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 25);
            this.panelTop.Name = "panelTop";
            this.panelTop.Padding = new System.Windows.Forms.Padding(3);
            this.panelTop.Size = new System.Drawing.Size(600, 26);
            this.panelTop.TabIndex = 3;
            // 
            // txtPath
            // 
            this.txtPath.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "CurrentPath", true));
            this.txtPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPath.Location = new System.Drawing.Point(3, 3);
            this.txtPath.Name = "txtPath";
            this.txtPath.ReadOnly = true;
            this.txtPath.Size = new System.Drawing.Size(594, 20);
            this.txtPath.TabIndex = 0;
            this.txtPath.Text = "<Path>";
            // 
            // toolStripTools
            // 
            this.toolStripTools.AutoSize = false;
            this.toolStripTools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSplitButtonView,
            this.tsBtnRefresh});
            this.toolStripTools.Location = new System.Drawing.Point(0, 0);
            this.toolStripTools.Name = "toolStripTools";
            this.toolStripTools.Size = new System.Drawing.Size(600, 25);
            this.toolStripTools.Stretch = true;
            this.toolStripTools.TabIndex = 4;
            // 
            // toolStripSplitButtonView
            // 
            this.toolStripSplitButtonView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.detailsToolStripMenuItem,
            this.smallIconsToolStripMenuItem,
            this.largeIconsToolStripMenuItem,
            this.tileToolStripMenuItem,
            this.listToolStripMenuItem});
            this.toolStripSplitButtonView.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButtonView.Image")));
            this.toolStripSplitButtonView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButtonView.Name = "toolStripSplitButtonView";
            this.toolStripSplitButtonView.Size = new System.Drawing.Size(64, 22);
            this.toolStripSplitButtonView.Text = "View";
            // 
            // detailsToolStripMenuItem
            // 
            this.detailsToolStripMenuItem.Checked = true;
            this.detailsToolStripMenuItem.CheckOnClick = true;
            this.detailsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.detailsToolStripMenuItem.Name = "detailsToolStripMenuItem";
            this.detailsToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.detailsToolStripMenuItem.Text = "Details";
            this.detailsToolStripMenuItem.Click += new System.EventHandler(this.detailsToolStripMenuItem_Click);
            // 
            // smallIconsToolStripMenuItem
            // 
            this.smallIconsToolStripMenuItem.Name = "smallIconsToolStripMenuItem";
            this.smallIconsToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.smallIconsToolStripMenuItem.Text = "Small Icons";
            this.smallIconsToolStripMenuItem.Click += new System.EventHandler(this.smallIconsToolStripMenuItem_Click);
            // 
            // largeIconsToolStripMenuItem
            // 
            this.largeIconsToolStripMenuItem.Name = "largeIconsToolStripMenuItem";
            this.largeIconsToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.largeIconsToolStripMenuItem.Text = "Large Icons";
            this.largeIconsToolStripMenuItem.Click += new System.EventHandler(this.largeIconsToolStripMenuItem_Click);
            // 
            // tileToolStripMenuItem
            // 
            this.tileToolStripMenuItem.Name = "tileToolStripMenuItem";
            this.tileToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.tileToolStripMenuItem.Text = "Tile";
            this.tileToolStripMenuItem.Click += new System.EventHandler(this.tileToolStripMenuItem_Click);
            // 
            // listToolStripMenuItem
            // 
            this.listToolStripMenuItem.Name = "listToolStripMenuItem";
            this.listToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.listToolStripMenuItem.Text = "List";
            this.listToolStripMenuItem.Click += new System.EventHandler(this.listToolStripMenuItem_Click);
            // 
            // tsBtnRefresh
            // 
            this.tsBtnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsBtnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnRefresh.Image")));
            this.tsBtnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnRefresh.Name = "tsBtnRefresh";
            this.tsBtnRefresh.Size = new System.Drawing.Size(23, 22);
            this.tsBtnRefresh.Text = "Refresh";
            this.tsBtnRefresh.Click += new System.EventHandler(this.tsBtnRefresh_Click);
            // 
            // BrowserView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listViewFiles);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.toolStripTools);
            this.Name = "BrowserView";
            this.Size = new System.Drawing.Size(600, 603);
            this.panelBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).EndInit();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.toolStripTools.ResumeLayout(false);
            this.toolStripTools.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.ListView listViewFiles;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.BindingSource bindingSource;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderSize;
        private System.Windows.Forms.ColumnHeader columnHeaderPermissions;
        private System.Windows.Forms.ColumnHeader columnHeaderModTime;
        private System.Windows.Forms.ColumnHeader columnHeaderOwner;
        private System.Windows.Forms.ColumnHeader columnHeaderGroup;
        private System.Windows.Forms.ToolStrip toolStripTools;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButtonView;
        private System.Windows.Forms.ToolStripMenuItem detailsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem smallIconsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem largeIconsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton tsBtnRefresh;
        private System.Windows.Forms.ImageList imageListSmall;
        private System.Windows.Forms.ImageList imageListLarge;
        private System.Windows.Forms.Label labelBrowserState;
    }
}
