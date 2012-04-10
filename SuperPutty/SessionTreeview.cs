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
using SuperPutty.Data;
using log4net;


namespace SuperPutty
{
    public partial class SessionTreeview : ToolWindow, IComparer
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SessionTreeview));

        public const string SessionIdDelim = "/";

        private DockPanel m_DockPanel;
        //private Dictionary<string, SessionData> m_SessionsById = new Dictionary<string, SessionData>();

        TreeNode nodeRoot;

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
            this.treeView1.TreeViewNodeSorter = this;

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

            this.nodeRoot = treeView1.Nodes.Add("root", "PuTTY Sessions", 0);
            this.nodeRoot.ContextMenuStrip = this.contextMenuStripFolder;

            foreach (SessionData session in SuperPuTTY.GetAllSessions())
            {
                TreeNode nodeParent = this.nodeRoot;
                if (session.SessionId != null && session.SessionId != session.SessionName)
                {
                    // take session id and create folder nodes
                    nodeParent = FindOrCreateParentNode(session.SessionId);
                }
                AddSessionNode(nodeParent, session, true);
            }
            treeView1.ExpandAll();
        }



        /// <summary>
        /// Save sessions from the registry to an XML file for importing later
        /// </summary>
        /// <param name="fileName">The path and name of the file to save the entries to</param>
        public static void ExportSessionsToXml(string fileName)
        {
            List<SessionData> sessions = SessionData.LoadSessionsFromRegistry();
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
            TreeNode node = this.treeView1.SelectedNode;
            if (IsSessionNode(node))
            {
                SessionData sessionData = (SessionData) node.Tag;
                
                ctlPuttyPanel sessionPanel = null;
                sessionPanel = NewPuttyPanel(sessionData);
                sessionPanel.Show(m_DockPanel, sessionData.LastDockstate);
            }
        }

        public ctlPuttyPanel NewPuttyPanel(string sessionId)
        {
            SessionData session = SuperPuTTY.GetSessionById(sessionId);
            //m_SessionsById.TryGetValue(sessionId, out session);
            return session == null ? null : NewPuttyPanel(session);
        }

        public ctlPuttyPanel NewPuttyPanel(SessionData sessionData)
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
                        this.BeginInvoke((MethodInvoker)delegate()
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
                    node = AddSessionNode(treeView1.SelectedNode, session, false);
                    //node = treeView1.Nodes["root"].Nodes.Add(session.SessionName, session.SessionName, 1, 1);
                    if (node != null)
                    {
                        node.Tag = session;
                    }
                }
                else
                {
                    // handle renames
                    node.Text = session.SessionName;
                    SuperPuTTY.RemoveSession(session.OldSessionId);
                    SuperPuTTY.AddSession(session);
                    //m_SessionsById.Remove(session.OldSessionId);
                    //m_SessionsById[session.SessionId] = session;

                    UpdateSessionId(node, session);
                    ResortNodes();
                }

                //node.Tag = session;
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
                //session.RegistryRemove(session.SessionName);
                treeView1.SelectedNode.Remove();
                SuperPuTTY.RemoveSession(session.SessionId);
                SuperPuTTY.SaveSessions();
                //m_SessionsById.Remove(session.SessionId);
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
        /// Shortcut for double clicking an entries node.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView1_DoubleClick(null, EventArgs.Empty);
        }

        private void newFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = this.treeView1.SelectedNode;
            if (node != null)
            {
                dlgRenameItem dialog = new dlgRenameItem();
                dialog.Text = "New Folder";
                dialog.ItemName = "New Folder";
                dialog.DetailName = "";
                dialog.ItemNameValidator = delegate(string txt, out string error)
                {
                    error = String.Empty;
                    if (node.Nodes.ContainsKey(txt))
                    {
                        error = "Node with same name exists";
                    }
                    else if (txt.Contains(SessionIdDelim))
                    {
                        error = "Invalid character ( " + SessionIdDelim + " ) in name";
                    }

                    return string.IsNullOrEmpty(error);
                };
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    AddFolderNode(node, dialog.ItemName);
                }
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = this.treeView1.SelectedNode;
            if (node != null)
            {
                dlgRenameItem dialog = new dlgRenameItem();
                dialog.Text = "Rename Folder";
                dialog.ItemName = node.Text;
                dialog.DetailName = "";
                dialog.ItemNameValidator = delegate(string txt, out string error)
                {
                    error = String.Empty;
                    if (node.Parent.Nodes.ContainsKey(txt) && txt != node.Text)
                    {
                        error = "Node with same name exists";
                    }
                    else if (txt.Contains(SessionIdDelim))
                    {
                        error = "Invalid character ( " + SessionIdDelim + " ) in name";
                    }
                    return string.IsNullOrEmpty(error);
                };
                if (dialog.ShowDialog(this) == DialogResult.OK && node.Text != dialog.ItemName)
                {
                    node.Text = dialog.ItemName;
                    UpdateSessionId(node);
                    ResortNodes();
                }
            }
        }

        private void removeFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = this.treeView1.SelectedNode;
            if (node != null && node.Nodes.Count == 0)
            {
                node.Remove();
                SuperPuTTY.ReportStatus("Removed Folder, {0}", node.Text);

            }
        }

        private void contextMenuStripFolder_Opening(object sender, CancelEventArgs e)
        {
            bool isRootNode = this.treeView1.SelectedNode != this.nodeRoot;
            this.renameToolStripMenuItem.Enabled = isRootNode;
            // TODO: handle removing folder and nodes in it recursively
            this.removeFolderToolStripMenuItem.Enabled = isRootNode && this.treeView1.SelectedNode.Nodes.Count == 0;
        }

        #region Node helpers

        TreeNode AddSessionNode(TreeNode parentNode, SessionData session, bool isInitializing)
        {
            TreeNode addedNode = null;
            if (parentNode.Nodes.ContainsKey(session.SessionName))
            {
                SuperPuTTY.ReportStatus("Node with the same name exists.  New node ({0}) NOT added", session.SessionName);
            }
            else
            {
                addedNode = parentNode.Nodes.Add(session.SessionName, session.SessionName, 1, 1);
                addedNode.Tag = session;
                addedNode.ContextMenuStrip = this.contextMenuStripAddTreeItem;
                parentNode.Expand();

                if (!isInitializing)
                {
                    SuperPuTTY.ReportStatus("Adding new session, {1}.  parent={0}", parentNode.Text, session.SessionName);
                    SuperPuTTY.AddSession(session);
                    //m_SessionsById[session.SessionId] = session;

                    UpdateSessionId(addedNode, session);
                }
            }

            return addedNode;
        }

        TreeNode AddFolderNode(TreeNode parentNode, String nodeName)
        {
            TreeNode nodeNew = null;
            if (parentNode.Nodes.ContainsKey(nodeName))
            {
                SuperPuTTY.ReportStatus("Node with the same name exists.  New node ({0}) NOT added", nodeName);
            }
            else
            {
                SuperPuTTY.ReportStatus("Adding new folder, {1}.  parent={0}", parentNode.Text, nodeName);
                nodeNew = parentNode.Nodes.Add(nodeName, nodeName, 0);
                nodeNew.ContextMenuStrip = this.contextMenuStripFolder;
                parentNode.Expand();
            }
            return nodeNew;
        }

        bool IsSessionNode(TreeNode node)
        {
            return node != null && node.Tag is SessionData;
        }

        bool IsFolderNode(TreeNode node)
        {
            return !IsSessionNode(node);
        }

        private void UpdateSessionId(TreeNode parentNode)
        {
            foreach (TreeNode node in parentNode.Nodes)
            {
                if (IsSessionNode(node))
                {
                    UpdateSessionId(node, (SessionData)node.Tag);
                }
                else
                {
                    UpdateSessionId(node);
                }
            }
        }
        private void UpdateSessionId(TreeNode addedNode, SessionData session)
        {
            // set session id as node path
            List<string> parentNodeNames = new List<string>();
            for (TreeNode node = addedNode; node.Parent != null; node = node.Parent)
            {
                parentNodeNames.Add(node.Text);
            }
            parentNodeNames.Reverse();
            String sessionId = String.Join(SessionIdDelim, parentNodeNames.ToArray());
            //Log.InfoFormat("sessionId={0}", sessionId);
            session.SessionId = sessionId;
            SuperPuTTY.SaveSessions();
            //session.SaveToRegistry();
        }

        TreeNode FindOrCreateParentNode(string sessionId)
        {
            Log.DebugFormat("Finding Node for sessionId ({0})", sessionId);
            TreeNode nodeParent = this.nodeRoot;

            string[] parts = sessionId.Split(SessionIdDelim.ToCharArray());
            if (parts.Length > 1)
            {
                for (int i = 0; i < parts.Length - 1; i++)
                {
                    string part = parts[i];
                    TreeNode node = nodeParent.Nodes[part];
                    if (node == null)
                    {
                        nodeParent = this.AddFolderNode(nodeParent, part);
                    }
                    else if (IsFolderNode(node))
                    {
                        nodeParent = node;
                    }
                }
            }

            Log.DebugFormat("Returning node ({0})", nodeParent.Text);
            return nodeParent;
        }

        public int Compare(object x, object y)
        {
            TreeNode tx = x as TreeNode;
            TreeNode ty = y as TreeNode;

            return string.Compare(tx.Text, ty.Text);

        }

        void ResortNodes()
        {
            this.treeView1.TreeViewNodeSorter = null;
            this.treeView1.TreeViewNodeSorter = this;
        }

        #endregion

        #region Drag Drop

        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && Control.ModifierKeys == Keys.Shift)
            {
                // Get the tree.
                TreeView tree = (TreeView)sender;

                // Get the node underneath the mouse.
                TreeNode node = tree.GetNodeAt(e.X, e.Y);
                tree.SelectedNode = node;

                // Start the drag-and-drop operation with a cloned copy of the node.
                if (node != null && IsSessionNode(node))
                {
                    this.treeView1.DoDragDrop(node, DragDropEffects.Copy);
                }
            }
        }

        private void treeView1_DragOver(object sender, DragEventArgs e)
        {
            // Get the tree.
            TreeView tree = (TreeView)sender;

            // Drag and drop denied by default.
            e.Effect = DragDropEffects.None;

            // Is it a valid format?
            TreeNode nodePayload = (TreeNode) e.Data.GetData(typeof(TreeNode));
            if (nodePayload != null)
            {
                // Get the screen point.
                Point pt = new Point(e.X, e.Y);

                // Convert to a point in the TreeView's coordinate system.
                pt = tree.PointToClient(pt);

                TreeNode node = tree.GetNodeAt(pt);
                // Is the mouse over a valid node?
                if (node != null)
                {
                    tree.SelectedNode = node;
                    // folder that is not the same parent and new node name is not already present
                    if (IsFolderNode(node) && node != nodePayload.Parent && !node.Nodes.ContainsKey(nodePayload.Text))
                    {
                        e.Effect = DragDropEffects.Copy;
                    }
                }
            }
        }

        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            // Get the tree.
            TreeView tree = (TreeView)sender;

            // Get the screen point.
            Point pt = new Point(e.X, e.Y);

            // Convert to a point in the TreeView's coordinate system.
            pt = tree.PointToClient(pt);

            // Get the node underneath the mouse.
            TreeNode node = tree.GetNodeAt(pt);

            if (IsFolderNode(node))
            {
                Log.DebugFormat("Drag drop");

                TreeNode nodePayload = (TreeNode)e.Data.GetData(typeof(TreeNode));
                TreeNode nodeNew = (TreeNode)nodePayload.Clone();

                // add new
                node.Nodes.Add(nodeNew);
                UpdateSessionId(nodeNew, (SessionData)nodeNew.Tag);

                // remove old
                nodePayload.Remove();

                // Show the newly added node if it is not already visible.
                node.Expand();
            }
        }

        #endregion

    }

}
