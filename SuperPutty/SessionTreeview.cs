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
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using log4net;
using SuperPutty.Data;
using SuperPutty.Utils;
using WeifenLuo.WinFormsUI.Docking;


namespace SuperPutty
{
    public partial class SessionTreeview : ToolWindow, IComparer
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SessionTreeview));

        private static int MaxSessionsToOpen = Convert.ToInt32(ConfigurationManager.AppSettings["SuperPuTTY.MaxSessionsToOpen"] ?? "10");

        public const string SessionIdDelim = "/";

        private DockPanel m_DockPanel;
        private bool isRenamingNode;
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

            // populate sessions in the treeview from the registry
            LoadSessions();
            SuperPuTTY.Sessions.ListChanged += new ListChangedEventHandler(Sessions_ListChanged);
            SuperPuTTY.Settings.SettingsSaving += new SettingsSavingEventHandler(Settings_SettingsSaving);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (SuperPuTTY.Settings.ExpandSessionsTreeOnStartup)
            {
                nodeRoot.ExpandAll();
                this.treeView1.SelectedNode = nodeRoot;
            }
            else
            {
                // start with semi-collapsed view
                nodeRoot.Expand();
                foreach (TreeNode node in this.nodeRoot.Nodes)
                {
                    if (!IsSessionNode(node))
                    {
                        node.Collapse();
                    }
                }
            }

            this.ApplySettings();
        }

        void Settings_SettingsSaving(object sender, CancelEventArgs e)
        {
            this.ApplySettings();
        }

        void ApplySettings()
        {
            this.treeView1.ShowLines = SuperPuTTY.Settings.SessionsTreeShowLines;
            this.treeView1.Font = SuperPuTTY.Settings.SessionsTreeFont;
        }

        protected override void OnClosed(EventArgs e)
        {
            SuperPuTTY.Sessions.ListChanged -= new ListChangedEventHandler(Sessions_ListChanged);
            SuperPuTTY.Settings.SettingsSaving -= new SettingsSavingEventHandler(Settings_SettingsSaving);
            base.OnClosed(e);
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
        }

        void Sessions_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (isRenamingNode)
            {
                return;
            }
            BindingList<SessionData> sessions = (BindingList<SessionData>) sender;
            if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                SessionData session = sessions[e.NewIndex];
                TreeNode nodeParent = FindOrCreateParentNode(session.SessionId);
                AddSessionNode(nodeParent, session, false);
            }
            else if (e.ListChangedType == ListChangedType.Reset)
            {
                // clear
                List<TreeNode> nodesToRemove = new List<TreeNode>();
                foreach(TreeNode node in nodeRoot.Nodes)
                {
                    nodesToRemove.Add(node);
                }
                foreach (TreeNode node in nodesToRemove)
                {
                    node.Remove();
                }
            }
            // @TODO: implement more later, note delete will be tricky...need a copy of the list
        }

        /// <summary>
        /// Opens the selected session when the node is double clicked in the treeview
        /// </summary>
        /// <param name="sender">The treeview control that was double clicked</param>
        /// <param name="e">An Empty EventArgs object</param>
        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // e is null if this method is called from connectToolStripMenuItem_Click
            TreeNode node = (e != null) ? e.Node : treeView1.SelectedNode;

            if (IsSessionNode(node) && node == treeView1.SelectedNode)
            {
                SessionData sessionData = (SessionData)node.Tag;
                SuperPuTTY.OpenPuttySession(sessionData);
            }
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
            TreeNode nodeRef = this.nodeRoot;
            bool isEdit = false;
            string title = null;
            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
                bool isFolderNode = IsFolderNode(treeView1.SelectedNode);
                if (menuItem.Text.ToLower().Equals("new") || isFolderNode)
                {
                    session = new SessionData();
                    nodeRef = isFolderNode ? treeView1.SelectedNode : treeView1.SelectedNode.Parent;
                    title = "Create New Session";
                }
                else if (menuItem == this.createLikeToolStripMenuItem)
                {
                    // copy as
                    session = (SessionData) ((SessionData) treeView1.SelectedNode.Tag).Clone();
                    session.SessionId = SuperPuTTY.MakeUniqueSessionId(session.SessionId);
                    session.SessionName = SessionData.GetSessionNameFromId(session.SessionId);
                    nodeRef = treeView1.SelectedNode.Parent;
                    title = "Create New Session Like " + session.OldName;
                }
                else
                {
                    // edit, session node selected
                    session = (SessionData)treeView1.SelectedNode.Tag;
                    node = treeView1.SelectedNode;
                    nodeRef = node.Parent;
                    isEdit = true;
                    title = "Edit Session: " + session.SessionName;
                }
            }

            dlgEditSession form = new dlgEditSession(session);
            form.Text = title;
            form.SessionNameValidator += delegate(string txt, out string error)
            {
                error = String.Empty;
                bool isDupeNode = isEdit ? txt != node.Text && nodeRef.Nodes.ContainsKey(txt) : nodeRef.Nodes.ContainsKey(txt);
                if (isDupeNode)
                {
                    error = "Session with same name exists";
                }
                else if (txt.Contains(SessionIdDelim))
                {
                    error = "Invalid character ( " + SessionIdDelim + " ) in name";
                }
                else if (string.IsNullOrEmpty(txt) || txt.Trim() == String.Empty)
                {
                    error = "Empty name";
                }
                return string.IsNullOrEmpty(error);
            };
            
            if (form.ShowDialog() == DialogResult.OK)
            {
                /* "node" will only be assigned if we're editing an existing session entry */
                if (node == null)
                {

                    // get the path up to the ref (parent) node
                    if (nodeRoot != nodeRef)
                    {
                        UpdateSessionId(nodeRef, session);
                        session.SessionId = SessionData.CombineSessionIds(session.SessionId, session.SessionName);
                    }
                    SuperPuTTY.AddSession(session);

                    // find new node and select it
                    TreeNode nodeNew = nodeRef.Nodes[session.SessionName];
                    if (nodeNew != null)
                    {
                        this.treeView1.SelectedNode = nodeNew;
                    }
                }
                else
                {
                    // handle renames
                    node.Text = session.SessionName;
                    node.Name = session.SessionName;
                    if (session.SessionId != session.OldSessionId)
                    {
                        try
                        {
                            this.isRenamingNode = true;
                            SuperPuTTY.RemoveSession(session.OldSessionId);
                            SuperPuTTY.AddSession(session);
                        }
                        finally
                        {
                            this.isRenamingNode = false;
                        }

                    }
                    ResortNodes();
                    this.treeView1.SelectedNode = node;
                }

                //treeView1.ExpandAll();
                SuperPuTTY.SaveSessions();
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
            SuperPuTTY.OpenScpSession(session);
        }

        /// <summary>
        /// Shortcut for double clicking an entries node.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView1_NodeMouseDoubleClick(null, null);
        }

        /// <summary>
        /// Open putty with args but as external process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectExternalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = this.treeView1.SelectedNode;
            if (IsSessionNode(node))
            {
                SessionData sessionData = (SessionData)node.Tag;
                PuttyStartInfo startInfo = new PuttyStartInfo(sessionData);
                startInfo.StartStandalone();
            }
        }

        private void connectInNewSuperPuTTYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = this.treeView1.SelectedNode;
            if (IsSessionNode(node))
            {
                SuperPuTTY.LoadSessionInNewInstance(((SessionData)node.Tag).SessionId);
            }
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
                    else if (string.IsNullOrEmpty(txt) || txt.Trim() == String.Empty)
                    {
                        error = "Empty folder name";
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
                    node.Name = dialog.ItemName;
                    UpdateSessionId(node);
                    SuperPuTTY.SaveSessions();
                    ResortNodes();
                }
            }
        }

        private void removeFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = this.treeView1.SelectedNode;
            if (node != null)
            {
                if (node.Nodes.Count > 0)
                {
                    List<SessionData> sessions = new List<SessionData>();
                    GetAllSessions(node, sessions);
                    if (DialogResult.Yes == MessageBox.Show(
                        "Remove Folder [" + node.Text + "] and [" + sessions.Count + "] sessions?",
                        "Remove Folder?", 
                        MessageBoxButtons.YesNo))
                    {
                        foreach (SessionData session in sessions)
                        {
                            SuperPuTTY.RemoveSession(session.SessionId);
                        }
                        node.Remove();
                        SuperPuTTY.ReportStatus("Removed Folder, {0} and {1} sessions", node.Text, sessions.Count);
                        SuperPuTTY.SaveSessions();
                    }
                }
                else
                {
                    node.Remove();
                    SuperPuTTY.ReportStatus("Removed Folder, {0}", node.Text);
                }
            }
        }

        private void connectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = this.treeView1.SelectedNode;
            if (node != null && !IsSessionNode(node))
            {
                List<SessionData> sessions = new List<SessionData>();
                GetAllSessions(node, sessions);
                Log.InfoFormat("Found {0} sessions", sessions.Count);

                if (sessions.Count > MaxSessionsToOpen)
                {
                    if (DialogResult.Cancel == MessageBox.Show(
                        "Open All " + sessions.Count + " sessions?", 
                        "WARNING", 
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
                    {
                        // bug out...too many sessions to open
                        return;
                    }
                }
                foreach (SessionData session in sessions)
                {
                        SuperPuTTY.OpenPuttySession(session);
                }
            }
        }

        private void contextMenuStripFolder_Opening(object sender, CancelEventArgs e)
        {
            bool isRootNode = this.treeView1.SelectedNode != this.nodeRoot;
            this.renameToolStripMenuItem.Enabled = isRootNode;
            // TODO: handle removing folder and nodes in it recursively
            this.removeFolderToolStripMenuItem.Enabled = isRootNode;// && this.treeView1.SelectedNode.Nodes.Count == 0;
        }

        private void contextMenuStripAddTreeItem_Opening(object sender, CancelEventArgs e)
        {
            // disable file transfers if pscp isn't configured.
            fileBrowserToolStripMenuItem.Enabled = frmSuperPutty.IsScpEnabled;

            connectInNewSuperPuTTYToolStripMenuItem.Enabled = !SuperPuTTY.Settings.SingleInstanceMode;
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
            //session.SessionId = sessionId;
            if (session != null) session.SessionId = sessionId;
            //SuperPuTTY.SaveSessions();
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


        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
              TreeNode node = this.treeView1.SelectedNode;
              if (node != null)
              {
                  node.ExpandAll();
              }
        }

        private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = this.treeView1.SelectedNode;
            if (node != null)
            {
                node.Collapse();
                if (node == this.nodeRoot)
                {
                    nodeRoot.Expand();
                }
            }
        }

        void GetAllSessions(TreeNode nodeFolder, List<SessionData> sessions)
        {
            if (nodeFolder != null)
            {
                foreach (TreeNode child in nodeFolder.Nodes)
                {
                    if (IsSessionNode(child))
                    {
                        SessionData session = (SessionData) child.Tag;
                        sessions.Add(session);
                    }
                    else
                    {
                        GetAllSessions(child, sessions);
                    }
                }
            }
        }
        #endregion

        #region Drag Drop

        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // Get the tree
            TreeView tree = (TreeView)sender;

            // Get the node underneath the mouse.
            TreeNode node = e.Item as TreeNode;

            // Start the drag-and-drop operation with a cloned copy of the node.
            //if (node != null && IsSessionNode(node))
            if (node != null && tree.Nodes[0] != node)
            {
                this.treeView1.DoDragDrop(node, DragDropEffects.Copy);
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
                if (node != null && node != nodePayload && nodePayload.Nodes.Find(node.Text, true).Length == 0)
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

                // If node was expanded before, ensure new node is also expanded when moved
                if (nodePayload.IsExpanded)
                {
                    nodeNew.Expand();
                }

                // remove old
                nodePayload.Remove();

                // add new
                UpdateSessionId(nodeNew, (SessionData)nodeNew.Tag); //
                node.Nodes.Add(nodeNew);

                // If this a folder, reset it's childrens sessionIds
                if (IsFolderNode(nodeNew))
                {
                    resetFoldersChildrenPaths(nodeNew);
                    
                }

                // remove old
                nodePayload.Remove();

                // Show the newly added node if it is not already visible.
                node.Expand();

                // auto save settings...use timer to prevent excessive saves while dragging and dropping nodes
                timerDelayedSave.Stop();
                timerDelayedSave.Start();
                //SuperPuTTY.SaveSessions();
            }
        }

        #endregion

        public void resetFoldersChildrenPaths(TreeNode nodePayload)
        {
            // Reset folders children nodes sessionId (path)
            foreach (TreeNode node in nodePayload.Nodes)
            {
                UpdateSessionId(node, (SessionData)node.Tag);
            }
        }

        private void timerDelayedSave_Tick(object sender, EventArgs e)
        {
            // stop timer
            timerDelayedSave.Stop();

            // do save
            SuperPuTTY.SaveSessions();
            SuperPuTTY.ReportStatus("Saved Sessions after Drag-Drop @ {0}", DateTime.Now);
        }


    }

}
