using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SuperPutty.Data;
using SuperPutty.Gui;

namespace SuperPutty
{
    public partial class LayoutsList : ToolWindow
    {
        public LayoutsList()
        {
            InitializeComponent();

            this.listBoxLayouts.DataSource = SuperPuTTY.Layouts;
        }

        protected override void OnClosed(EventArgs e)
        {
            this.listBoxLayouts.DataSource = null;
            base.OnClosed(e);
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = IndexAtCursor() == -1;

            loadInNewInstanceToolStripMenuItem.Enabled = !SuperPuTTY.Settings.SingleInstanceMode;
        }

        private void listBoxLayouts_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // select item under mouse
                int idx = this.listBoxLayouts.IndexFromPoint(e.X, e.Y);
                if (idx != -1)
                {
                    this.listBoxLayouts.SelectedIndex = idx;
                }
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutData layout = (LayoutData) this.listBoxLayouts.SelectedItem;
            if (layout != null)
            {
                SuperPuTTY.LoadLayout(layout);
            }
        }

        private void loadInNewInstanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutData layout = (LayoutData)this.listBoxLayouts.SelectedItem;
            if (layout != null)
            {
                SuperPuTTY.LoadLayoutInNewInstance(layout);
            }
        }

        private void setAsDefaultLayoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutData layout = (LayoutData)this.listBoxLayouts.SelectedItem;
            if (layout != null)
            {
                SuperPuTTY.SetLayoutAsDefault(layout.Name);
            }
        }

        private void listBoxLayouts_DoubleClick(object sender, EventArgs e)
        {
            int idx = IndexAtCursor();
            if (idx != -1)
            {
                LayoutData layout = (LayoutData)this.listBoxLayouts.Items[idx];
                if (layout != null)
                {
                    SuperPuTTY.LoadLayout(layout);
                }
            }
            
        }

        int IndexAtCursor()
        {
            Point p = this.listBoxLayouts.PointToClient(Cursor.Position);
            return this.listBoxLayouts.IndexFromPoint(p.X, p.Y);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutData layout = (LayoutData)this.listBoxLayouts.SelectedItem;
            if (layout != null)
            {
                if (DialogResult.Yes == MessageBox.Show(this, "Delete Layout (" + layout.Name + ")?", "Delete Layout", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                {
                    SuperPuTTY.RemoveLayout(layout.Name, true);
                }
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutData layout = (LayoutData)this.listBoxLayouts.SelectedItem;
            if (layout != null)
            {
                dlgRenameItem renameDialog = new dlgRenameItem();
                renameDialog.DetailName = String.Empty;
                renameDialog.ItemName = layout.Name;
                renameDialog.ItemNameValidator = this.ValidateLayoutName;
                if (DialogResult.OK == renameDialog.ShowDialog(this))
                {
                    SuperPuTTY.RenameLayout(layout, renameDialog.ItemName);
                }
            }
            
        }

        bool ValidateLayoutName(string name, out string error)
        {
            LayoutData layout = SuperPuTTY.FindLayout(name);
            if (layout != null)
            {
                error = "Layout exists with same name";
                return false;
            }

            error = null;
            return true;
        }
    }
}
