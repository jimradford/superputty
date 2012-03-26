using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SuperPutty
{
    public partial class LayoutsList : ToolWindow
    {
        public LayoutsList()
        {
            InitializeComponent();

            this.listBoxLayouts.DataSource = SuperPuTTY.Layouts;
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = IndexAtCursor() == -1;
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
                SuperPuTTY.Settings.DefaultLayoutName = layout.Name;
                SuperPuTTY.Settings.Save();
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
    }
}
