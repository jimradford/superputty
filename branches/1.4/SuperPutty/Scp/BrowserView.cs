using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using log4net;
using SuperPutty.Gui;
using SuperPutty.Utils;
using System.IO;

namespace SuperPutty.Scp
{
    /// <summary>
    /// http://www.codeproject.com/Articles/88390/MVP-VM-Model-View-Presenter-ViewModel-with-Data-Bi
    /// </summary>
    public partial class BrowserView : UserControl 
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(BrowserView));

        public BrowserView(IBrowserPresenter presenter, BrowserFileInfo startingDir) : this()
        {
            this.Presenter = presenter;
            this.Presenter.AuthRequest += (Presenter_AuthRequest);
            this.Bind(this.Presenter.ViewModel);

            this.Presenter.LoadDirectory(startingDir);
            this.ConfirmTransfer = true;
        }

        public BrowserView()
        {
            InitializeComponent();

            this.Comparer = new BrowserFileInfoComparer();
            this.listViewFiles.ListViewItemSorter = this.Comparer;
        }

        void Presenter_AuthRequest(object sender, AuthEventArgs e)
        {
            // present login
            using (dlgLogin login = new dlgLogin(e.UserName))
            {
                login.StartPosition = FormStartPosition.CenterParent;
                if (login.ShowDialog(this) == DialogResult.OK)
                {
                    e.UserName = login.Username;
                    e.Password = login.Password;
                    e.Handled = true;
                }
                else
                {
                    Log.InfoFormat("Login canceled.  Closing parent form");
                    this.ParentForm.Close();
                }
            }
        }

        #region Data Binding
        void Bind(IBrowserViewModel model)
        {
            // Bind the controls
            this.bindingSource.DataSource = model;

            // Ugh, ListView not bindable, do it manually
            this.PopulateListView(model.Files);
            model.Files.ListChanged += (Files_ListChanged);
            model.PropertyChanged += (s, e) => EnableDisableControls(model.BrowserState);
        }

        void EnableDisableControls(BrowserState state)
        {
            bool enabled = state == BrowserState.Ready;
            this.tsBtnRefresh.Enabled = enabled;
            this.listViewFiles.Enabled = enabled;
        }

        void PopulateListView(BindingList<BrowserFileInfo> files)
        {
            this.listViewFiles.BeginUpdate();
            this.listViewFiles.Items.Clear();
            this.listViewFiles.ListViewItemSorter = null;

            foreach (BrowserFileInfo file in files)
            {
                string sizeKB = file.Type == FileType.File ? (file.Size / 1024).ToString("#,##0 KB") : String.Empty;
                ListViewItem addedItem = this.listViewFiles.Items.Add(file.Name, file.Name);
                addedItem.Tag = file;
                addedItem.ImageIndex = file.Type == FileType.ParentDirectory 
                    ? 2 
                    : file.Type == FileType.Directory ? 1 : 0;
                addedItem.SubItems.Add(new ListViewItem.ListViewSubItem(addedItem, sizeKB));
                addedItem.SubItems.Add(new ListViewItem.ListViewSubItem(addedItem, file.LastModTime.ToString("yyyy-MM-dd  h:mm:ss tt")));
                addedItem.SubItems.Add(new ListViewItem.ListViewSubItem(addedItem, file.Permissions));
                addedItem.SubItems.Add(new ListViewItem.ListViewSubItem(addedItem, file.Owner));
                addedItem.SubItems.Add(new ListViewItem.ListViewSubItem(addedItem, file.Group));
            }

            listViewFiles.EndUpdate();
            this.listViewFiles.ListViewItemSorter = this.Comparer;
        }

        void Files_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.Reset)
            {
                BindingList<BrowserFileInfo> list = (BindingList<BrowserFileInfo>)sender;
                PopulateListView(list);
            }
        }
        #endregion

        #region ListView Sorting 

        private void listViewFiles_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Get BrowserFileInfo propertyName
            ColumnHeader header = this.listViewFiles.Columns[e.Column];

            // Do Sort
            this.listViewFiles.Sorting = listViewFiles.Sorting == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            this.Comparer.Column = e.Column;
            this.Comparer.SortOrder = listViewFiles.Sorting;

            Log.InfoFormat("Sorting ListView: field={0}, dir={1}", header.Text, this.Comparer.SortOrder);
            this.listViewFiles.Sort();
            this.listViewFiles.SetSortIcon(e.Column, listViewFiles.Sorting);
        }

        public class BrowserFileInfoComparer : IComparer
        {
            public int Column { get; set; }
            public SortOrder SortOrder { get; set; }

            public int Compare(object x, object y)
            {
                ListViewItem lviX = (ListViewItem)x;
                ListViewItem lviY = (ListViewItem)y;
                BrowserFileInfo a = (BrowserFileInfo)lviX.Tag;
                BrowserFileInfo b = (BrowserFileInfo)lviY.Tag;

                // direction
                int dir = this.SortOrder == SortOrder.Descending ? -1 : 1;

                // identity
                if (a == b) return 0;

                // preference based on type
                int type = a.Type.CompareTo(b.Type);
                if (type != 0) { return type; }

                // resolve based on field
                switch (this.Column)
                {
                    case 1: return dir * Comparer<long>.Default.Compare(a.Size, b.Size);
                    case 2: return dir * Comparer<DateTime>.Default.Compare(a.LastModTime, b.LastModTime);
                    case 3: return dir * Comparer<string>.Default.Compare(a.Permissions, b.Permissions);
                    case 4: return dir * Comparer<string>.Default.Compare(a.Owner, b.Owner);
                    case 5: return dir * Comparer<string>.Default.Compare(a.Group, b.Group);
                }

                // default to using name 
                return dir * Comparer<string>.Default.Compare(a.Name, b.Name);
            }
        }
        
        #endregion

        #region ListView View Modes
        private void detailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckViewModeItem((ToolStripMenuItem)sender);
            this.listViewFiles.View = View.Details;
        }

        private void smallIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckViewModeItem((ToolStripMenuItem)sender);
            this.listViewFiles.View = View.SmallIcon;
        }

        private void largeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckViewModeItem((ToolStripMenuItem)sender);
            this.listViewFiles.View = View.LargeIcon;
        }

        private void tileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckViewModeItem((ToolStripMenuItem)sender);
            this.listViewFiles.View = View.Tile;
        }

        private void listToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckViewModeItem((ToolStripMenuItem)sender);
            this.listViewFiles.View = View.List;
        }

        void CheckViewModeItem(ToolStripMenuItem itemToSelect)
        {
            foreach (ToolStripMenuItem item in this.toolStripSplitButtonView.DropDownItems)
            {
                item.Checked = false;
            }
            itemToSelect.Checked = true;
        }
        #endregion

        #region ListView DragDrop

        int dragDropLastX = -1;
        int dragDropLastY = -1;
        bool dragDropIsValid = false;
        FileTransferRequest dragDropFileTransfer;

        private void listViewFiles_DragEnter(object sender, DragEventArgs e)
        {
            // Check for copy allowed, payload = DataFormats.FileDrop or BrowserFileInfo
            bool copyAllowed = (e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy;
            bool isFile = e.Data.GetDataPresent(DataFormats.FileDrop, false);
            bool isBrowserFile = e.Data.GetDataPresent(typeof(BrowserFileInfo[]));
            this.dragDropIsValid = copyAllowed && ( isFile || isBrowserFile);

            // update effect
            e.Effect = dragDropIsValid ? DragDropEffects.Copy : DragDropEffects.None;

            // parse out payload
            if (this.dragDropIsValid)
            {
                this.dragDropFileTransfer = new FileTransferRequest { Session = this.Presenter.Session };

                // Get source files (payload)
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    // windows files
                    Array files = (Array)e.Data.GetData(DataFormats.FileDrop, false);
                    foreach (string fileName in files)
                    {
                        BrowserFileInfo item = Directory.Exists(fileName)
                            ? new BrowserFileInfo(new DirectoryInfo(fileName))
                            : new BrowserFileInfo(new FileInfo(fileName));
                        this.dragDropFileTransfer.SourceFiles.Add(item);
                    }
                }
                else if (e.Data.GetDataPresent(typeof(BrowserFileInfo[])))
                {
                    // from another browser
                    BrowserFileInfo[] files = (BrowserFileInfo[])e.Data.GetData(typeof(BrowserFileInfo[]));
                    this.dragDropFileTransfer.SourceFiles.AddRange(files);
                }
            }

            Log.DebugFormat(
                "DragEnter: allowedEffect={0}, effect={1}, isFile={2}, isBrowserFile={3}",
                e.AllowedEffect, e.Effect, isFile, isBrowserFile);
        }

        private void listViewFiles_DragLeave(object sender, EventArgs e) 
        {
            ResetDragDrop();
        }

        private void listViewFiles_DragOver(object sender, DragEventArgs e)
        {
            // Prevent event from firing too often
            if (!dragDropIsValid || (e.X == dragDropLastX && e.Y == dragDropLastY))
            {
                return;
            }
            dragDropLastX = e.X;
            dragDropLastY = e.Y;

            // Get item under mouse
            ListView listView = (ListView) sender;
            Point p = listView.PointToClient(new Point(e.X, e.Y));
            ListViewHitTestInfo hti = listView.HitTest(p.X, p.Y);

            BrowserFileInfo target = hti.Item != null ? (BrowserFileInfo) hti.Item.Tag : this.Presenter.CurrentPath;
            this.dragDropFileTransfer.TargetFile = target;

            // Clear selection and select item under mouse if folder
            listView.SelectedItems.Clear();
            if (hti.Item != null && target.Type == FileType.Directory)
            {
                hti.Item.Selected = true;
            }

            // Validate source/targets and update effect
            // - Windows file to remote panel (do transfer)
            // - Windows file to Local (do transfer?)
            // - Local BrowserFileInfo to Remote (do transfer)
            // - Remote BrowserFileInfo to Local (do transfer)
            // Ask model if allowed?
            // - Local BrowserFileInfo to Local (not allowed)
            // - Remote BrowserFileInfo to Remote (not allowed)
            e.Effect = DragDropEffects.Copy;
            foreach (BrowserFileInfo source in this.dragDropFileTransfer.SourceFiles)
            {
                if (!this.Presenter.CanTransferFile(source, target))
                {
                    e.Effect = DragDropEffects.None;
                    break;
                }
            }
        }

        private void listViewFiles_DragDrop(object sender, DragEventArgs e)
        {
            Log.InfoFormat("DragDrop: valid={0}, effect={1}", this.dragDropIsValid, e.Effect);

            // Ask for confirmation
            if (this.ConfirmTransfer)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Source Files:");
                foreach (BrowserFileInfo source in this.dragDropFileTransfer.SourceFiles)
                {
                    sb.AppendLine(source.Path);
                }
                sb.AppendLine();
                sb.AppendLine("Target:");
                sb.AppendLine(this.dragDropFileTransfer.TargetFile.Path);
                sb.AppendLine();

                DialogResult res = MessageBox.Show(this, sb.ToString(), "Transfer Files?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.No)
                {
                    Log.InfoFormat("FileTransfer canceled: {0}", this.dragDropFileTransfer);
                    ResetDragDrop();
                    return;
                }
            }

            // Request file transfers   
            this.Presenter.TransferFiles(this.dragDropFileTransfer);
            ResetDragDrop();
        }

        private void listViewFiles_ItemDrag(object sender, ItemDragEventArgs e)
        {
            ListView listView = (ListView) sender;

            List<BrowserFileInfo> items = new List<BrowserFileInfo>();
            foreach (ListViewItem item in listView.SelectedItems)
            {
                BrowserFileInfo bfi = (BrowserFileInfo) item.Tag;
                if (bfi.Type == FileType.ParentDirectory)
                {
                    // can't work with parent dir...bug out
                    return;
                }
                items.Add(bfi);
            }
                
            DoDragDrop(items.ToArray(), DragDropEffects.Copy);
        }

        private void ResetDragDrop()
        {
            this.dragDropLastX = -1;
            this.dragDropLastY = -1;
            this.dragDropIsValid = false;
            this.dragDropFileTransfer = null;
        }

        #endregion

        private void tsBtnRefresh_Click(object sender, EventArgs e)
        {
            this.Presenter.Refresh();
        }

        private void listViewFiles_DoubleClick(object sender, EventArgs e)
        {
            if (this.listViewFiles.SelectedItems.Count != 0)
            {
                BrowserFileInfo bfi = (BrowserFileInfo) this.listViewFiles.SelectedItems[0].Tag;
                if (bfi.Type == FileType.Directory || bfi.Type == FileType.ParentDirectory)
                {
                    this.Presenter.LoadDirectory(bfi);
                }
            }
        }

        IBrowserPresenter Presenter { get; set; }
        BrowserFileInfoComparer Comparer { get; set; }
        public bool ConfirmTransfer { get; set; }


    }
}
