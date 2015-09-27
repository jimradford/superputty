using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SuperPutty.Utils;

namespace SuperPutty
{
    public partial class dlgSelectPW : Form
    {
        private string _Password;

        public string Password
        {
            get { return _Password; }
            private set { _Password = value; }
        }

        public dlgSelectPW()
        {
            InitializeComponent();
            tbOtherPassword.Text = "";

            if (rbOtherPassword.Checked){
                tbOtherPassword.Enabled = true;
            }else{
                tbOtherPassword.Enabled = false;
                
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (rbOtherPassword.Checked)
            {
                tbOtherPassword.Enabled = true;
            }
            else {
                tbOtherPassword.Enabled = false;
                tbOtherPassword.Text = "";
            }
        }

        private void btSelectPasswordOK_Click(object sender, EventArgs e)
        {
            if (rbUseMasterPassword.Checked){
                Password = SingletonSessionPasswordManager.Instance.getMasterPassword();
            }
            else if(rbWPw.Checked || String.IsNullOrEmpty(tbOtherPassword.Text)) {
                Password = "";            
            }else{
                Password = Hash.GetHashString(tbOtherPassword.Text);             
            }
            DialogResult = DialogResult.OK;
        }

        private void btSelectPasswordCancel_Click(object sender, EventArgs e){
            Password = "";
            DialogResult = DialogResult.Cancel;
        }
    }
}
