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
    public partial class dlgRenameItem : Form
    {
        public delegate bool ItemNameValidationHandler(string name, out string error);

        public dlgRenameItem()
        {
            InitializeComponent();
        }

        public string ItemName {
            get { return this.txtItemName.Text; }
            set { this.txtItemName.Text = value; }
        }

        public string DetailName
        {
            get { return this.labelDetailName.Text; }
            set { this.labelDetailName.Text = value; }
        }

        public ItemNameValidationHandler ItemNameValidator { get; set; }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtItemName_Validating(object sender, CancelEventArgs e)
        {
            if (this.ItemNameValidator != null)
            {
                string error;
                if (!this.ItemNameValidator(txtItemName.Text, out error))
                {
                    this.errorProvider.SetError(this.txtItemName, error ?? "Invalid Name");
                    this.btnOK.Enabled = false;
                }
                else
                {
                    this.errorProvider.SetError(this.txtItemName, String.Empty);
                    this.btnOK.Enabled = true;
                }
            }

        }

        private void txtItemName_Validated(object sender, EventArgs e)
        {
            //this.errorProvider.SetError(this.txtItemName, String.Empty);
        }

        /// <summary>
        /// Allow them to close the form if validation is not passed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dlgRenameItem_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = false;
        }

        private void folderForm_TextChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = (txtItemName.Text.Length > 0);
        }

    }
}
