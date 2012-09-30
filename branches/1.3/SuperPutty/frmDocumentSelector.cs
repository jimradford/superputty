using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using log4net;
using SuperPutty.Data;
using SuperPutty.Utils;

namespace SuperPutty
{
    public partial class frmDocumentSelector : Form
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(frmDocumentSelector));

        private DockPanel dockPanel;

        public frmDocumentSelector(DockPanel dockPanel)
        {
            this.dockPanel = dockPanel;
            InitializeComponent();
            this.checkSendToVisible.Checked = SuperPuTTY.Settings.SendCommandsToVisibleOnly;
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (this.Visible)
            {
                // load docs into the ListView
                this.listViewDocs.Items.Clear();
                int i = 0;
                foreach (IDockContent doc in VisualOrderTabSwitchStrategy.GetDocuments(this.dockPanel))
                {
                    i++;
                    ctlPuttyPanel pp = doc as ctlPuttyPanel;
                    if (pp != null)
                    {
                        string tabNum = pp == this.dockPanel.ActiveDocument ? i + "*" : i.ToString();
                        ListViewItem item = this.listViewDocs.Items.Add(tabNum);
                        item.SubItems.Add(new ListViewItem.ListViewSubItem(item, pp.Text));
                        item.SubItems.Add(new ListViewItem.ListViewSubItem(item, pp.Session.SessionId));
                        item.SubItems.Add(new ListViewItem.ListViewSubItem(item, pp.GetHashCode().ToString()));

                        item.Selected = IsDocumentSelected(pp);
                        item.Tag = pp;
                    }

                }
                this.BeginInvoke(new Action(delegate { this.listViewDocs.Focus(); }));
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Log.Debug("Cancel");
            this.Hide();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Log.Debug("OK");
            foreach (ListViewItem item in this.listViewDocs.Items)
            {
                ((ctlPuttyPanel)item.Tag).AcceptCommands = item.Selected;
            }
            this.DialogResult = DialogResult.OK;
            this.Hide();

            SuperPuTTY.Settings.SendCommandsToVisibleOnly = this.checkSendToVisible.Checked;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            e.Cancel = true;
            this.Hide();
        }

        public bool IsDocumentSelected(ctlPuttyPanel document)
        {
            bool selected = false;
            if (document != null && document.Session != null)
            {
                selected = this.checkSendToVisible.Checked ? document.Visible : document.AcceptCommands;
            }
            return selected;
        }

        private void checkSendToVisible_CheckedChanged(object sender, EventArgs e)
        {
            this.listViewDocs.Enabled = !this.checkSendToVisible.Checked;
        }
    }
}
