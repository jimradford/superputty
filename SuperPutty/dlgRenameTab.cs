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
    public partial class dlgRenameTab : Form
    {
        public dlgRenameTab()
        {
            InitializeComponent();
        }

        public string TabName {
            get { return this.txtTabName.Text; }
            set { this.txtTabName.Text = value; }
        }

        public string SessionName
        {
            get { return this.labelSessionName.Text; }
            set { this.labelSessionName.Text = value; }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
