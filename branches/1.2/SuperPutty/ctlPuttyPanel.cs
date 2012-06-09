/*
 * Copyright (c) 2009 Jim Radford http://www.jimradford.com
 * Copyright (c) 2012 John Peterson
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
using SuperPutty.Data;
using WeifenLuo.WinFormsUI.Docking;
using SuperPutty.Utils;


namespace SuperPutty
{
    public partial class ctlPuttyPanel : ToolWindow
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ctlPuttyPanel));

        private PuttyStartInfo m_puttyStartInfo;
        private ApplicationPanel m_AppPanel;
        private SessionData m_Session;
        private PuttyClosedCallback m_ApplicationExit;
        public ctlPuttyPanel(SessionData session, PuttyClosedCallback callback)
        {
            m_Session = session;
            m_ApplicationExit = callback;
            m_puttyStartInfo = new PuttyStartInfo(session);

            /*
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
                args += (!String.IsNullOrEmpty(m_Session.ExtraArgs) ? m_Session.ExtraArgs + " " : "");
                args += (!String.IsNullOrEmpty(m_Session.Username) && m_Session.Username.Length > 0) ? m_Session.Username + "@" : "";
                args += m_Session.Host;
            } 
            Log.InfoFormat("Putty Args: '{0}'", args);
            this.ApplicationParameters = args;
            */
            InitializeComponent();

            this.Text = session.SessionName;

            CreatePanel();
            AdjustMenu();
        }

        private void CreatePanel()
        {
            this.m_AppPanel = new ApplicationPanel();
            this.SuspendLayout();            
            this.m_AppPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_AppPanel.ApplicationName = this.m_puttyStartInfo.Executable;
            this.m_AppPanel.ApplicationParameters = this.m_puttyStartInfo.Args;
            this.m_AppPanel.ApplicationWorkingDirectory = this.m_puttyStartInfo.WorkingDir;
            this.m_AppPanel.Location = new System.Drawing.Point(0, 0);
            this.m_AppPanel.Name = this.m_Session.SessionId; // "applicationControl1";
            this.m_AppPanel.Size = new System.Drawing.Size(this.Width, this.Height);
            this.m_AppPanel.TabIndex = 0;            
            this.m_AppPanel.m_CloseCallback = this.m_ApplicationExit;
            this.Controls.Add(this.m_AppPanel);

            this.ResumeLayout();
        }

        void AdjustMenu()
        {
            // for mintty, disable the putty menu items
            if (this.Session.Proto == ConnectionProtocol.Mintty)
            {
                this.toolStripPuttySep1.Visible = false;
                this.eventLogToolStripMenuItem.Visible = false;
                this.toolStripPuttySep2.Visible = false;
                this.changeSettingsToolStripMenuItem.Visible = false;
                this.copyAllToClipboardToolStripMenuItem.Visible = false;
                this.restartSessionToolStripMenuItem.Visible = false;
                this.clearScrollbackToolStripMenuItem.Visible = false;
                this.resetTerminalToolStripMenuItem.Visible = false;
            }


        }

        void CreateMenu()
        {
            newSessionToolStripMenuItem.DropDownItems.Clear();
            foreach (SessionData session in SuperPuTTY.GetAllSessions())
            {
                ToolStripMenuItem tsmiParent = newSessionToolStripMenuItem;
                foreach (string part in SessionData.GetSessionNameParts(session.SessionId))
                {
                    if (part == session.SessionName)
                    {
                        ToolStripMenuItem newSessionTSMI = new ToolStripMenuItem();
                        newSessionTSMI.Tag = session;
                        newSessionTSMI.Text = session.SessionName;
                        newSessionTSMI.Click += new System.EventHandler(newSessionTSMI_Click);
                        tsmiParent.DropDownItems.Add(newSessionTSMI);
                    }
                    else
                    {
                        if (tsmiParent.DropDownItems.ContainsKey(part))
                        {
                            tsmiParent = (ToolStripMenuItem) tsmiParent.DropDownItems[part];
                        }
                        else
                        {
                            ToolStripMenuItem newSessionFolder = new ToolStripMenuItem(part);
                            newSessionFolder.Name = part;
                            tsmiParent.DropDownItems.Add(newSessionFolder);
                            tsmiParent = newSessionFolder;
                        }
                    }
                }
            }
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
            this.m_AppPanel.ReFocusPuTTY();         
        }

        protected override string GetPersistString()
        {
            string str = String.Format("{0}?SessionId={1}&TabName={2}", 
                this.GetType().FullName, 
                HttpUtility.UrlEncodeUnicode(this.m_Session.SessionId), 
                HttpUtility.UrlEncodeUnicode(this.Text));
            return str;
        }

        public static ctlPuttyPanel FromPersistString(String persistString)
        {
            ctlPuttyPanel panel = null;
            if (persistString.StartsWith(typeof(ctlPuttyPanel).FullName))
            {
                int idx = persistString.IndexOf("?");
                if (idx != -1)
                {
                    NameValueCollection data = HttpUtility.ParseQueryString(persistString.Substring(idx + 1));
                    string sessionId = data["SessionId"] ?? data["SessionName"];
                    string tabName = data["TabName"];

                    Log.InfoFormat("Restoring putty session, sessionId={0}, tabName={1}", sessionId, tabName);

                    SessionData session = SuperPuTTY.GetSessionById(sessionId);
                    if (session != null)
                    {
                        panel = ctlPuttyPanel.NewPanel(session);
                        if (panel == null)
                        {
                            Log.WarnFormat("Could not restore putty session, sessionId={0}", sessionId);
                        }
                        else
                        {
                            panel.Text = tabName;
                        }
                    }
                    else
                    {
                        Log.WarnFormat("Session not found, sessionId={0}", sessionId);
                    }
                }
                else
                {
                    idx = persistString.IndexOf(":");
                    if (idx != -1)
                    {
                        string sessionId = persistString.Substring(idx + 1);
                        Log.InfoFormat("Restoring putty session, sessionId={0}", sessionId);
                        SessionData session = SuperPuTTY.GetSessionById(sessionId);
                        if (session != null)
                        {
                            panel = ctlPuttyPanel.NewPanel(session);
                        }
                        else
                        {
                            Log.WarnFormat("Session not found, sessionId={0}", sessionId);
                        }
                    }
                }
            }
            return panel;
        }

        private void aboutPuttyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.chiark.greenend.org.uk/~sgtatham/putty/");
        }

 
        private void duplicateSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SuperPuTTY.OpenPuttySession(this.m_Session);
        }

        private void renameTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dlgRenameItem dialog = new dlgRenameItem();
            dialog.ItemName = this.Text;
            dialog.DetailName = this.m_Session.SessionId;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.Text = dialog.ItemName;
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.m_AppPanel != null)
            {
                this.m_AppPanel.RefreshAppWindow();
            }
        }

        public SessionData Session { get { return this.m_Session; } }
        public ApplicationPanel AppPanel { get { return this.m_AppPanel; } }

        public static ctlPuttyPanel NewPanel(SessionData sessionData)
        {
            ctlPuttyPanel puttyPanel = null;
            // This is the callback fired when the panel containing the terminal is closed
            // We use this to save the last docking location
            PuttyClosedCallback callback = delegate(bool closed)
            {
                if (puttyPanel != null)
                {
                    // save the last dockstate (if it has been changed)
                    if (sessionData.LastDockstate != puttyPanel.DockState
                        && puttyPanel.DockState != DockState.Unknown
                        && puttyPanel.DockState != DockState.Hidden)
                    {
                        sessionData.LastDockstate = puttyPanel.DockState;
                        SuperPuTTY.SaveSessions();
                        //sessionData.SaveToRegistry();
                    }

                    if (puttyPanel.InvokeRequired)
                    {
                        puttyPanel.BeginInvoke((MethodInvoker)delegate()
                        {
                            puttyPanel.Close();
                        });
                    }
                    else
                    {
                        puttyPanel.Close();
                    }
                }
            };
            puttyPanel = new ctlPuttyPanel(sessionData, callback);
            return puttyPanel;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            CreateMenu();
        }

        private void newSessionTSMI_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem) sender;
            SessionData session = menuItem.Tag as SessionData;
            if (session != null)
            {
                SuperPuTTY.OpenPuttySession(session);
            }
        }

        private void puTTYMenuTSMI_Click(object sender, EventArgs e)
        {
            string tag = ((ToolStripMenuItem)sender).Tag.ToString();
            uint command = Convert.ToUInt32(tag, 16);
            Log.DebugFormat("Sending Putty Command: tag={0}, command={1}", tag, command);
            NativeMethods.SetForegroundWindow(m_AppPanel.AppWindowHandle);
            NativeMethods.SendMessage(m_AppPanel.AppWindowHandle, (uint) NativeMethods.WM.SYSCOMMAND, command, 0);
            //SuperPuTTY.MainForm.BringToFront();
        }

    }
}
