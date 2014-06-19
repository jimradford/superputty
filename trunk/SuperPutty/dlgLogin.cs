/*
 * Copyright (c) 2009 Jim Radford http://www.jimradford.com
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions: 
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security.Principal;
using SuperPutty.Data;


namespace SuperPutty
{
    public partial class dlgLogin : Form
    {
        private string _Username = WindowsIdentity.GetCurrent().Name.Split('\\')[1];

        public string Username
        {
            get { return _Username; }
            private set { _Username = value; }
        }

        private string _Password;

        public string Password
        {
            get { return _Password; }
            private set { _Password = value; }
        }

        private bool _Remember = false;
        public bool Remember
        {
            get { return _Remember; }
            private set { _Remember = value; }
        }

        private SessionData m_Session;

        public dlgLogin(string userName)
        {
            InitializeComponent();

            if (!String.IsNullOrEmpty(userName))
                this.Username = userName;

            textBoxUsername.Text = this.Username;
        }

        public dlgLogin(SessionData session) : this(session.Username)
        {
            m_Session = session;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Username = textBoxUsername.Text;
            Password = textBoxPasssword.Text;
            DialogResult = DialogResult.OK;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // a Password is not required since user could be using keys
            button1.Enabled = textBoxUsername.Text.Length > 0;
        }

        private void checkBoxRemember_CheckedChanged(object sender, EventArgs e)
        {
            Remember = checkBoxRemember.Checked;
        }

        private void dlgLogin_Shown(object sender, EventArgs e)
        {
            if(!String.IsNullOrEmpty(textBoxUsername.Text))
                textBoxPasssword.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
