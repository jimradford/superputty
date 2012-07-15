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

namespace SuperPutty
{
    public partial class frmDocumentSelector : Form
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(frmDocumentSelector));

        private DockPanel dockPanel;
        private List<string> selectedSessionIds = new List<string>();
        private List<string> sesiondIds = new List<string>();

        public frmDocumentSelector(DockPanel dockPanel)
        {
            this.dockPanel = dockPanel;
            InitializeComponent();

            // init
            this.checkBoxSelectAll.Checked = true;
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (this.Visible)
            {
                // load docs into the ListView
                this.listViewDocs.Items.Clear();
                foreach (IDockContent doc in this.dockPanel.Documents)
                {
                    ctlPuttyPanel pp = doc as ctlPuttyPanel;
                    if (pp != null)
                    {
                        ListViewItem item = this.listViewDocs.Items.Add(pp.Session.SessionId);
                        item.Selected = this.selectedSessionIds.Contains(pp.Session.SessionId);
                        item.Tag = pp.Session;
                        item.SubItems.Add(new ListViewItem.ListViewSubItem(item, pp.Text));
                        item.SubItems.Add(new ListViewItem.ListViewSubItem(item, pp.Session.SessionId));
                    }
                }

                // select the active sessionIds
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
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void checkBoxSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            this.listViewDocs.Enabled = !this.checkBoxSelectAll.Checked;
        }

        public bool IsDocumentSelected(ctlPuttyPanel document)
        {
            bool selected = false;
            if (document != null && document.Session != null)
            {
                string sid = document.Session.SessionId;
                selected = this.checkBoxSelectAll.Checked || this.selectedSessionIds.Contains(sid);
            }
            return selected;
        }
    }
}
