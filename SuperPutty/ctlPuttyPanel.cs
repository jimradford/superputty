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


namespace SuperPutty
{
    public partial class ctlPuttyPanel : ToolWindow
    {        
        private string ApplicationName = String.Empty;
        private string ApplicationParameters = String.Empty;

        private ApplicationPanel applicationwrapper1;
        private SessionData m_Session;
        private PuttyClosedCallback m_ApplicationExit;
        public ctlPuttyPanel(SessionData session, PuttyClosedCallback callback)
        {
            m_Session = session;
            m_ApplicationExit = callback;

            string args = "-" + session.Proto.ToString().ToLower() + " ";            
            args += (!String.IsNullOrEmpty(m_Session.Password) && m_Session.Password.Length > 0) ? "-pw " + m_Session.Password + " " : "";
            args += "-P " + m_Session.Port + " ";
            args += (!String.IsNullOrEmpty(m_Session.PuttySession)) ? "-load \"" + m_Session.PuttySession + "\" " : "";
            args += (!String.IsNullOrEmpty(m_Session.Username) && m_Session.Username.Length > 0) ? m_Session.Username + "@" : "";
            args += m_Session.Host;
            Console.WriteLine("Args: '{0}'", args);
            this.ApplicationParameters = args;

            InitializeComponent();

            this.Text = session.SessionName;

            CreatePanel();
        }

        private void CreatePanel()
        {
            this.applicationwrapper1 = new ApplicationPanel();
            this.SuspendLayout();            
            this.applicationwrapper1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.applicationwrapper1.ApplicationName = frmSuperPutty.PuttyExe;
            this.applicationwrapper1.ApplicationParameters = this.ApplicationParameters;
            this.applicationwrapper1.Location = new System.Drawing.Point(0, 0);
            this.applicationwrapper1.Name = "applicationControl1";
            this.applicationwrapper1.Size = new System.Drawing.Size(284, 264);
            this.applicationwrapper1.TabIndex = 0;            
            this.applicationwrapper1.m_CloseCallback = this.m_ApplicationExit;
            this.Controls.Add(this.applicationwrapper1);

            this.ResumeLayout();
        }

        private void closeSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }       
    }
}
