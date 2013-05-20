using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SuperPutty.Gui;
using System.IO;

namespace SuperPutty.Scp
{
    public partial class FileTransferView : ToolWindowDocument
    {
        public FileTransferView()
        {
            InitializeComponent();
        }

        public FileTransferView(IFileTransferPresenter presenter) : this()
        {
            this.Presenter = presenter;
            this.bindingSource.DataSource = presenter.ViewModel.FileTransfers;
        }

        #region Context Menu

        private void contextMenu_Opening(object sender, CancelEventArgs e)
        {
            Point p = PointToClient(MousePosition);
            DataGridView.HitTestInfo hit = this.grid.HitTest(p.X, p.Y);
            if (hit.Type == DataGridViewHitTestType.Cell)
            {
                // toggle on/off the actions based on view model
                FileTransferViewItem item = (FileTransferViewItem) ((DataGridViewRow) grid.Rows[hit.RowIndex]).DataBoundItem;
                this.runAgainToolStripMenuItem.Enabled = !item.IsActive;
                this.cancelToolStripMenuItem.Enabled = item.IsActive;
                this.deleteToolStripMenuItem.Enabled = !item.IsActive;
            }
            else
            {
                // Only open on cells
                e.Cancel = true;
            }
        }

        private void grid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (MouseButtons == MouseButtons.Right)
            {
                if (!grid.Rows[e.RowIndex].Selected)
                {
                    grid.ClearSelection();
                    grid.Rows[e.RowIndex].Selected = true;
                }
            }
        }


        private void cancelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileTransferViewItem item = GetSelectedItem<FileTransferViewItem>();
            if (item != null)
            {
                this.Presenter.Cancel(item.Id);
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileTransferViewItem item = GetSelectedItem<FileTransferViewItem>();
            if (item != null)
            {
                this.Presenter.Remove(item.Id);
            }
        }

        private void runAgainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileTransferViewItem item = GetSelectedItem<FileTransferViewItem>();
            if (item != null)
            {
                this.Presenter.Restart(item.Id);
            }
        }

        #endregion

        T GetSelectedItem<T>()
        {
            T item = default(T);
            foreach (DataGridViewRow row in this.grid.SelectedRows)
            {
                item = (T) row.DataBoundItem;
                break;
            }
            return item;
        }

        IFileTransferPresenter Presenter { get; set; }
    }
}
