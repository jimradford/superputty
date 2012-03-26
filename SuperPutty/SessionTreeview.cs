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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Microsoft.Win32;
using System.Xml.Serialization;

namespace SuperPutty
{
    public partial class SessionTreeview : ToolWindow
    {
        private DockPanel m_DockPanel;
        private Dictionary<string, SessionData> m_Sessions = new Dictionary<string, SessionData>();

        /// <summary>
        /// Instantiate the treeview containing the sessions
        /// </summary>
        /// <param name="dockPanel">The DockPanel container</param>
        /// <remarks>Having the dockpanel container is necessary to allow us to
        /// dock any terminal or file transfer sessions from within the treeview class</remarks>
        public SessionTreeview(DockPanel dockPanel)
        {
            m_DockPanel = dockPanel;
            InitializeComponent();

            // disable file transfers if pscp isn't configured.
            fileBrowserToolStripMenuItem.Enabled = frmSuperPutty.IsScpEnabled;

            // populate sessions in the treeview from the registry
            LoadSessions();
        }

        /// <summary>
        /// Load the sessions from the registry and populate the treeview control
        /// </summary>
        public void LoadSessions()
        {
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add("root", "PuTTY Sessions", 0);
            foreach (SessionData session in LoadSessionsFromRegistry())
            {
                TreeNode addedNode = treeView1.Nodes["root"].Nodes.Add(session.SessionName, session.SessionName, 1, 1);
                addedNode.Tag = session;
                m_Sessions[session.SessionName] = session;
            }
            treeView1.ExpandAll();
        }

        /// <summary>
        /// Read any existing saved sessions from the registry, decode and populat a list containing the data
        /// </summary>
        /// <returns>A list containing the entries retrieved from the registry</returns>
        private static List<SessionData> LoadSessionsFromRegistry()
        {
            List<SessionData> sessionList = new List<SessionData>();
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Jim Radford\SuperPuTTY\Sessions");
            if (key != null)
            {
                string[] sessionKeys = key.GetSubKeyNames();
                foreach (string session in sessionKeys)
                {
                    SessionData sessionData = new SessionData();
                    RegistryKey itemKey = key.OpenSubKey(session);
                    if (itemKey != null)
                    {
                        sessionData.Host = (string)itemKey.GetValue("Host", "");
                        sessionData.Port = (int)itemKey.GetValue("Port", 22);
                        sessionData.Proto = (ConnectionProtocol)Enum.Parse(typeof(ConnectionProtocol), (string)itemKey.GetValue("Proto", "SSH"));
                        sessionData.PuttySession = (string)itemKey.GetValue("PuttySession", "Default Session");
                        sessionData.SessionName = session;
                        sessionData.Username = (string)itemKey.GetValue("Login", "");
                        sessionData.LastDockstate = (DockState)itemKey.GetValue("Last Dock", DockState.Document);
                        sessionData.AutoStartSession = bool.Parse((string)itemKey.GetValue("Auto Start", "False"));
                        sessionList.Add(sessionData);
                    }
                }
            }
            return sessionList;
        }

        /// <summary>
        /// Save sessions from the registry to an XML file for importing later
        /// </summary>
        /// <param name="fileName">The path and name of the file to save the entries to</param>
        public static void ExportSessionsToXml(string fileName)
        {
            List<SessionData> sessions = LoadSessionsFromRegistry();
            XmlSerializer s = new XmlSerializer(sessions.GetType());
            TextWriter w = new StreamWriter(fileName);
            s.Serialize(w, sessions);
            w.Close();
        }

        /// <summary>
        /// Import saved sessions from an XML file and store them in the registry
        /// </summary>
        /// <param name="fileName">The path and name of the file to load the entries from</param>
        public static void ImportSessionsFromXml(string fileName)
        {
            List<SessionData> sessions = new List<SessionData>();
            XmlSerializer s = new XmlSerializer(sessions.GetType());
            TextReader r = new StreamReader(fileName);
            sessions = (List<SessionData>)s.Deserialize(r);
            r.Close();
            foreach (SessionData session in sessions)
            {
                session.SaveToRegistry();
            }
        }

        /// <summary>
        /// Opens the selected session when the node is double clicked in the treeview
        /// </summary>
        /// <param name="sender">The treeview control that was double clicked</param>
        /// <param name="e">An Empty EventArgs object</param>
        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode.ImageIndex > 0)
            {
                SessionData sessionData = (SessionData)treeView1.SelectedNode.Tag;
                
                ctlPuttyPanel sessionPanel = null;
                /*
                // This is the callback fired when the panel containing the terminal is closed
                // We use this to save the last docking location
                PuttyClosedCallback callback = delegate(bool closed)
                {
                    if (sessionPanel != null)
                    {
                        // save the last dockstate (if it has been changed)
                        if (sessionData.LastDockstate != sessionPanel.DockState
                            && sessionPanel.DockState != DockState.Unknown
                            && sessionPanel.DockState != DockState.Hidden)
                        {
                            sessionData.LastDockstate = sessionPanel.DockState;
                            sessionData.SaveToRegistry();
                        }

                        if (sessionPanel.InvokeRequired)
                        {
                            this.BeginInvoke((MethodInvoker)delegate()
                            {
                                sessionPanel.Close();
                            });
                        }
                        else
                        {
                            sessionPanel.Close();
                        }
                    }
                };

                sessionPanel = new ctlPuttyPanel(sessionData, callback);*/
                sessionPanel = NewPuttyPanel(sessionData);
                sessionPanel.Show(m_DockPanel, sessionData.LastDockstate);
            }
        }

        public ctlPuttyPanel NewPuttyPanel(string sessionName)
        {
            SessionData session;
            m_Sessions.TryGetValue(sessionName, out session);
            return session == null ? null : NewPuttyPanel(session);
        }
        public ctlPuttyPanel NewPuttyPanel(SessionData sessionData)
        {
            ctlPuttyPanel sessionPanel = null;
            // This is the callback fired when the panel containing the terminal is closed
            // We use this to save the last docking location
            PuttyClosedCallback callback = delegate(bool closed)
            {
                if (sessionPanel != null)
                {
                    // save the last dockstate (if it has been changed)
                    if (sessionData.LastDockstate != sessionPanel.DockState
                        && sessionPanel.DockState != DockState.Unknown
                        && sessionPanel.DockState != DockState.Hidden)
                    {
                        sessionData.LastDockstate = sessionPanel.DockState;
                        sessionData.SaveToRegistry();
                    }

                    if (sessionPanel.InvokeRequired)
                    {
                        this.BeginInvoke((MethodInvoker)delegate()
                        {
                            sessionPanel.Close();
                        });
                    }
                    else
                    {
                        sessionPanel.Close();
                    }
                }
            };
            sessionPanel = new ctlPuttyPanel(sessionData, callback);
            return sessionPanel;
        }

        /// <summary>
        /// Create/Update a session entry
        /// </summary>
        /// <param name="sender">The toolstripmenuitem control that was clicked</param>
        /// <param name="e">An Empty EventArgs object</param>
        private void CreateOrEditSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SessionData session = null;
            TreeNode node = null;

            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
                if (menuItem.Text.ToLower().Equals("new") || treeView1.SelectedNode.Tag == null)
                {
                    session = new SessionData();
                }
                else
                {
                    session = (SessionData)treeView1.SelectedNode.Tag;
                    node = treeView1.SelectedNode;
                }
            }

            dlgEditSession form = new dlgEditSession(session);
            if (form.ShowDialog() == DialogResult.OK)
            {
                /* "node" will only be assigned if we're editing an existing session entry */
                if (node == null)
                {
                    node = treeView1.Nodes["root"].Nodes.Add(session.SessionName, session.SessionName, 1, 1);
                }
                else
                {
                    // handle renames
                    node.Text = session.SessionName;
                    m_Sessions.Remove(session.OldName);
                    m_Sessions[session.SessionName] = session;
                }

                node.Tag = session;
                treeView1.ExpandAll();
            }
        }

        /// <summary>
        /// Forces a node to be selected when right clicked, this assures the context menu will be operating
        /// on the proper session entry.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeView1.SelectedNode = treeView1.GetNodeAt(e.X, e.Y);
            }          
        }

        /// <summary>
        /// Delete a session entry from the treeview and the registry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SessionData session = (SessionData)treeView1.SelectedNode.Tag;
            if (MessageBox.Show("Are you sure you want to delete " + session.SessionName + "?", "Delete Session?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                session.RegistryRemove(session.SessionName);
                treeView1.SelectedNode.Remove();
                m_Sessions.Remove(session.SessionName);
            }
        }

        /// <summary>
        /// Open a directory listing on the selected nodes host to allow dropping files
        /// for drag + drop copy.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SessionData session = (SessionData)treeView1.SelectedNode.Tag;
            RemoteFileListPanel dir = null;
            bool cancelShow = false;
            if (session != null)
            {
                PuttyClosedCallback callback = delegate(bool error)
                {
                    cancelShow = error;
                };
                PscpTransfer xfer = new PscpTransfer(session);
                xfer.PuttyClosed = callback;

                dir = new RemoteFileListPanel(xfer, m_DockPanel, session);
                if (!cancelShow)
                {
                    dir.Show(m_DockPanel);
                }
            }
        }

        /// <summary>
        /// Longcut for double clicking an entries node.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView1_DoubleClick(null, EventArgs.Empty);
        }


    }
}
