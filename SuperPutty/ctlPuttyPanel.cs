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
using log4net;
using System.Diagnostics;
using System.Web;
using System.Collections.Specialized;


namespace SuperPutty
{
    public partial class ctlPuttyPanel : ToolWindow
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ctlPuttyPanel));

        private string ApplicationName = String.Empty;
        private string ApplicationParameters = String.Empty;
        private string ApplicationWorkingDirectory = null;

        private ApplicationPanel applicationwrapper1;
        private SessionData m_Session;
        private PuttyClosedCallback m_ApplicationExit;
        public ctlPuttyPanel(SessionData session, PuttyClosedCallback callback)
        {
            m_Session = session;
            m_ApplicationExit = callback;

            string args;
            if (session.Proto == ConnectionProtocol.Cygterm)
            {
                CygtermInfo cyg = new CygtermInfo(m_Session);
                args = cyg.Args;
                ApplicationWorkingDirectory = cyg.StartingDir;
            }
            else
            {
                args = "-" + session.Proto.ToString().ToLower() + " ";
                args += (!String.IsNullOrEmpty(m_Session.Password) && m_Session.Password.Length > 0) ? "-pw " + m_Session.Password + " " : "";
                args += "-P " + m_Session.Port + " ";
                args += (!String.IsNullOrEmpty(m_Session.PuttySession)) ? "-load \"" + m_Session.PuttySession + "\" " : "";
                args += (!String.IsNullOrEmpty(m_Session.Username) && m_Session.Username.Length > 0) ? m_Session.Username + "@" : "";
                args += m_Session.Host;
            } 
            Log.InfoFormat("Putty Args: '{0}'", args);
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
            this.applicationwrapper1.ApplicationWorkingDirectory = this.ApplicationWorkingDirectory;
            this.applicationwrapper1.Location = new System.Drawing.Point(0, 0);
            this.applicationwrapper1.Name = "applicationControl1";
            this.applicationwrapper1.Size = new System.Drawing.Size(this.Width, this.Height);
            this.applicationwrapper1.TabIndex = 0;            
            this.applicationwrapper1.m_CloseCallback = this.m_ApplicationExit;
            this.Controls.Add(this.applicationwrapper1);

            this.ResumeLayout();
        }

        private void closeSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Reset the focus to the child application window
        /// </summary>
        internal void SetFocusToChildApplication()
        {
            this.applicationwrapper1.ReFocusPuTTY();         
        }

        protected override string GetPersistString()
        {
            string str = String.Format("{0}?SessionName={1}&TabName={2}", 
                this.GetType().FullName, 
                HttpUtility.UrlEncodeUnicode(this.m_Session.SessionName), 
                HttpUtility.UrlEncodeUnicode(this.Text));
            return str;
        }

        public static ctlPuttyPanel FromPersistString(SessionTreeview view, String persistString)
        {
            ctlPuttyPanel panel = null;
            if (persistString.StartsWith(typeof(ctlPuttyPanel).FullName))
            {
                int idx = persistString.IndexOf("?");
                if (idx != -1)
                {
                    NameValueCollection data = HttpUtility.ParseQueryString(persistString.Substring(idx + 1));
                    string sessionName = data["SessionName"];
                    string tabName = data["TabName"];

                    Log.InfoFormat("Restoring putty session, sessionName={0}, tabName={1}", sessionName, tabName);

                    panel = view.NewPuttyPanel(sessionName);
                    panel.Text = tabName;
                }
                else
                {
                    idx = persistString.IndexOf(":");
                    if (idx != -1)
                    {
                        string sessionName = persistString.Substring(idx + 1);
                        Log.InfoFormat("Restoring putty session, sessionName={0}", sessionName);
                        panel = view.NewPuttyPanel(sessionName);
                    }
                }
            }
            return panel;
        }

        private void aboutPuttyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.chiark.greenend.org.uk/~sgtatham/putty/");
        }

        private void newSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void duplicateSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void renameTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dlgRenameTab dialog = new dlgRenameTab();
            dialog.TabName = this.Text;
            dialog.SessionName = this.m_Session.SessionName;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.Text = dialog.TabName;
            }
        }
    }
}
