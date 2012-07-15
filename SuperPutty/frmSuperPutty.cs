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
using System.Windows.Input;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using WeifenLuo.WinFormsUI.Docking;
using SuperPutty.Properties;
using SuperPutty.Data;
using log4net;
using System.Reflection;
using System.Runtime.InteropServices;
using SuperPutty.Utils;
using System.Configuration;

namespace SuperPutty
{
    public partial class frmSuperPutty : Form
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(frmSuperPutty));

        private static string XmlEditor = ConfigurationManager.AppSettings["SuperPuTTY.XmlEditor"];

        public static string PuttyExe
        {
            get { return SuperPuTTY.Settings.PuttyExe; }
        }

        public static string PscpExe
        {
            get { return SuperPuTTY.Settings.PscpExe; }
        }

        public static bool IsScpEnabled
        {
            get { return File.Exists(PscpExe); }
        }

        internal DockPanel DockPanel { get { return this.dockPanel1; } }

        public ToolWindowDocument CurrentPanel { get; set; }

        private SingletonToolWindowHelper<SessionTreeview> sessions;
        private SingletonToolWindowHelper<LayoutsList> layouts;
        private SingletonToolWindowHelper<Log4netLogViewer> logViewer;

        private TextBoxFocusHelper tbFocusHelperHost;
        private TextBoxFocusHelper tbFocusHelperUserName;
        private TextBoxFocusHelper tbFocusHelperPassword;
        private frmDocumentSelector sendCommandsDocumentSelector;

        private NativeMethods.LowLevelKMProc llkp;
        //private NativeMethods.LowLevelKMProc llmp;
        private static IntPtr kbHookID = IntPtr.Zero;
        private static IntPtr mHookID = IntPtr.Zero;
        private bool forceClose;
        private FormWindowState lastNonMinimizedWindowState = FormWindowState.Normal;
        private Rectangle lastNormalDesktopBounds;
        private ChildWindowFocusHelper focusHelper;
        bool isControlDown = false;
        ToolWindowDocument currentTabPanel;
        int commandMRUIndex = 0;

        public frmSuperPutty()
        {
            // Verify Putty is set; Prompt user if necessary; exit otherwise
            dlgFindPutty.PuttyCheck();
            
            InitializeComponent();
            if (this.DesignMode) return;

            // setup connection bar
            this.tbTxtBoxPassword.TextBox.PasswordChar = '*';
            this.RefreshConnectionToolbarData();

            // version in status bar
            this.toolStripStatusLabelVersion.Text = SuperPuTTY.Version;

            // tool windows
            this.sessions = new SingletonToolWindowHelper<SessionTreeview>("Sessions", this.DockPanel, x => new SessionTreeview(x.DockPanel));
            this.layouts = new SingletonToolWindowHelper<LayoutsList>("Layouts", this.DockPanel);
            this.logViewer = new SingletonToolWindowHelper<Log4netLogViewer>("Log Viewer", this.DockPanel);

            // for toolbar
            this.tbFocusHelperHost = new TextBoxFocusHelper(this.tbTxtBoxHost.TextBox);
            this.tbFocusHelperUserName = new TextBoxFocusHelper(this.tbTxtBoxLogin.TextBox);
            this.tbFocusHelperPassword = new TextBoxFocusHelper(this.tbTxtBoxPassword.TextBox);
            this.sendCommandsDocumentSelector = new frmDocumentSelector(this.DockPanel);

            // Hook into status
            SuperPuTTY.StatusEvent += new Action<string>(delegate(String msg) { this.toolStripStatusLabelMessage.Text = msg; });
            SuperPuTTY.ReportStatus("Ready");

            // Hook into LayoutChanging/Changed
            SuperPuTTY.LayoutChanging += new EventHandler<LayoutChangedEventArgs>(SuperPuTTY_LayoutChanging);

            // Low-Level Mouse and Keyboard hooks
            llkp = KBHookCallback;
            kbHookID = SetKBHook(llkp);
            //llmp = MHookCallback;
            //mHookID = SetMHook(llmp);

            this.focusHelper = new ChildWindowFocusHelper(this);

            // Restore window location and size
            if (SuperPuTTY.Settings.RestoreWindowLocation)
            {
                FormUtils.RestoreFormPositionAndState(this, SuperPuTTY.Settings.WindowPosition, SuperPuTTY.Settings.WindowState);
            }

            // show/hide toolbars and status bar
            ApplySettingsToToolbars();
            this.ResizeEnd += new EventHandler(frmSuperPutty_ResizeEnd);
        }

        private void frmSuperPutty_Load(object sender, EventArgs e)
        {
            this.BeginInvoke(new Action(this.LoadLayout));
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // free hooks
            NativeMethods.UnhookWindowsHookEx(kbHookID);
            //NativeMethods.UnhookWindowsHookEx(mHookID);

            // save window size and location if not maximized or minimized
            if (SuperPuTTY.Settings.RestoreWindowLocation)// && this.WindowState != FormWindowState.Minimized)
            {
                SuperPuTTY.Settings.WindowPosition = this.lastNormalDesktopBounds;
                SuperPuTTY.Settings.WindowState = this.WindowState == FormWindowState.Minimized ? this.WindowState = this.lastNonMinimizedWindowState : this.WindowState;
                SuperPuTTY.Settings.Save();
            }

            // save layout for auto-restore
            if (SuperPuTTY.Settings.DefaultLayoutName == LayoutData.AutoRestore)
            {
                SaveLayout(SuperPuTTY.AutoRestoreLayoutPath, "Saving auto-restore layout");
            }

            this.focusHelper.Dispose();

            base.OnFormClosed(e);
        }

        void frmSuperPutty_ResizeEnd(object sender, EventArgs e)
        {
            SaveLastWindowBounds();
        }

        private void frmSuperPutty_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (SuperPuTTY.Settings.ExitConfirmation && !forceClose)
            {
                if (MessageBox.Show("Exit SuperPuTTY?", "Confirm Exit", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// Handles focusing on tabs/windows which host PuTTY
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dockPanel1_ActiveDocumentChanged(object sender, EventArgs e)
        {
            FocusActiveDocument("ActiveDocumentChanged");
        }

        public void FocusActiveDocument(string caller)
        {
            if (this.DockPanel.ActiveDocument == null)
            {
                this.Text = string.Format("SuperPuTTY");
            }
            else
            {
                ToolWindowDocument window = this.DockPanel.ActiveDocument as ToolWindowDocument;
                if (window != null)
                {
                    // If we aren't using Ctrl-Tab to move between panels,
                    // i.e. we got here because the operator clicked on the
                    // panel directly, then record it as the current panel.
                    if (currentTabPanel == null)
                    {
                        window.MakePanelCurrent();
                    }

                    ctlPuttyPanel p = window as ctlPuttyPanel;
                    if (p != null)
                    {
                        p.SetFocusToChildApplication(caller);
                        this.Text = string.Format("SuperPuTTY - {0}", p.Text);
                    }
                }
            }
        }

        private void frmSuperPutty_Activated(object sender, EventArgs e)
        {
            Log.DebugFormat("[{0}] Activated", this.Handle);
            //dockPanel1_ActiveDocumentChanged(null, null);
        }

        public void SetActiveDocument(ToolWindow toolWindow)
        {
            if (this.DockPanel.ActiveDocument != toolWindow)
            {
                toolWindow.Show();
            }
        }

        #region File

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "XML Files|*.xml|All files|*.*";
            saveDialog.FileName = "Sessions.XML";
            saveDialog.InitialDirectory = Application.StartupPath;
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                SessionData.SaveSessionsToFile(SuperPuTTY.GetAllSessions(), saveDialog.FileName);
            }
        }

        private void fromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "XML Files|*.xml|All files|*.*";
            openDialog.FileName = "Sessions.XML";
            openDialog.CheckFileExists = true;
            openDialog.InitialDirectory = Application.StartupPath;
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                SuperPuTTY.ImportSessionsFromFile(openDialog.FileName);
            }
        }


        private void fromPuTTYCMExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "XML Files|*.xml|All files|*.*";
            openDialog.FileName = "export.xml";
            openDialog.CheckFileExists = true;
            openDialog.InitialDirectory = Application.StartupPath;
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                SuperPuTTY.ImportSessionsFromPuttyCM(openDialog.FileName);
            }
        }


        private void fromPuTTYSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show(
                "Do you want to copy all sessions from PuTTY/KiTTY?  Duplicates may be created.",
                "SuperPuTTY",
                MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                SuperPuTTY.ImportSessionsFromPuTTY();
            }
        }

        private void editSessionsInNotepadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(XmlEditor ?? "notepad", Path.Combine(SuperPuTTY.Settings.SettingsFolder, "Sessions.XML"));
        }

        private void reloadSessionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SuperPuTTY.LoadSessions();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion 

        #region View Menu

        private void toggleCheckedState(object sender, EventArgs e)
        {
            // toggle
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            mi.Checked = !mi.Checked;

            // save
            SuperPuTTY.Settings.ShowStatusBar = this.showStatusBarToolStripMenuItem.Checked;
            SuperPuTTY.Settings.ShowToolBarConnections = this.quickConnectionToolStripMenuItem.Checked;
            SuperPuTTY.Settings.ShowToolBarCommands = this.sendCommandsToolStripMenuItem.Checked;

            SuperPuTTY.Settings.Save();

            // apply
            ApplySettingsToToolbars();
        }

        void ApplySettingsToToolbars()
        {
            this.statusStrip1.Visible = SuperPuTTY.Settings.ShowStatusBar;
            this.showStatusBarToolStripMenuItem.Checked = SuperPuTTY.Settings.ShowStatusBar;

            this.tsConnect.Visible = SuperPuTTY.Settings.ShowToolBarConnections;
            this.quickConnectionToolStripMenuItem.Checked = SuperPuTTY.Settings.ShowToolBarConnections;

            this.tsCommands.Visible = SuperPuTTY.Settings.ShowToolBarCommands;
            this.sendCommandsToolStripMenuItem.Checked = SuperPuTTY.Settings.ShowToolBarCommands;
        }

        private void sessionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.layouts.Instance != null && !this.layouts.Instance.DockHandler.Pane.IsAutoHide)
            {
                this.sessions.ShowWindow(this.layouts.Instance.DockHandler.Pane, DockAlignment.Top, 0.5);
            }
            else
            {
                this.sessions.ShowWindow(DockState.DockRight);
            }
        }

        private void logViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.logViewer.ShowWindow(DockState.DockBottom);
        }


        private void layoutsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.sessions.Instance != null && !this.sessions.Instance.DockHandler.Pane.IsAutoHide)
            {
                this.layouts.ShowWindow(this.sessions.Instance.DockHandler.Pane, DockAlignment.Bottom, 0.5);
            }
            else
            {
                this.layouts.ShowWindow(DockState.DockRight);
            }
        }
        #endregion

        #region Layout

        void LoadLayout()
        {
            String dir = SuperPuTTY.LayoutsDir;
            if (Directory.Exists(dir))
            {
                this.openFileDialogLayout.InitialDirectory = dir;
                this.saveFileDialogLayout.InitialDirectory = dir;
            }

            if (SuperPuTTY.StartingSession != null)
            {
                // load empty layout then open session
                SuperPuTTY.LoadLayout(null);
                SuperPuTTY.OpenSession(SuperPuTTY.StartingSession);
            }
            else
            {
                // default layout or null for hard-coded default
                SuperPuTTY.LoadLayout(SuperPuTTY.StartingLayout);
            }

        }

        void SuperPuTTY_LayoutChanging(object sender, LayoutChangedEventArgs eventArgs)
        {
            if (eventArgs.IsNewLayoutAlreadyActive)
            {
                toolStripStatusLabelLayout.Text = eventArgs.New.Name;
            }
            else
            {
                // reset old layout (close old putty instances)
                foreach (DockContent dockContent in this.DockPanel.DocumentsToArray())
                {
                    Log.Debug("Unhooking document: " + dockContent);
                    dockContent.DockPanel = null;
                    // close old putty
                    if (dockContent.CloseButtonVisible)
                    {
                        dockContent.Close();
                    }
                }
                List<DockContent> contents = new List<DockContent>();
                foreach (DockContent dockContent in this.DockPanel.Contents)
                {
                    contents.Add(dockContent);
                }
                foreach (DockContent dockContent in contents)
                {
                    Log.Debug("Unhooking dock content: " + dockContent);
                    dockContent.DockPanel = null;
                    // close non-persistant windows
                    if (dockContent.CloseButtonVisible)
                    {
                        dockContent.Close();
                    }
                }


                if (eventArgs.New == null)
                {
                    // 1st time or reset
                    Log.Debug("Initializing default layout");
                    this.InitDefaultLayout();
                    toolStripStatusLabelLayout.Text = "";
                    SuperPuTTY.ReportStatus("Initialized default layout");
                }
                else if (!File.Exists(eventArgs.New.FilePath))
                {
                    // file missing
                    Log.WarnFormat("Layout file doesn't exist, file={0}", eventArgs.New.FilePath);
                    this.InitDefaultLayout();
                    toolStripStatusLabelLayout.Text = eventArgs.New.Name;
                    SuperPuTTY.ReportStatus("Could not load layout, file missing: {0}", eventArgs.New.FilePath);
                }
                else
                {
                    // load new one
                    Log.DebugFormat("Loading layout: {0}", eventArgs.New.FilePath);
                    this.DockPanel.LoadFromXml(eventArgs.New.FilePath, RestoreLayoutFromPersistString);
                    toolStripStatusLabelLayout.Text = eventArgs.New.Name;
                    SuperPuTTY.ReportStatus("Loaded layout: {0}", eventArgs.New.FilePath);
                }

                // after all is done, cause a repaint to 
            }
        }

        void InitDefaultLayout()
        {
            this.sessionsToolStripMenuItem_Click(this, EventArgs.Empty);
            this.layoutsToolStripMenuItem_Click(this, EventArgs.Empty);
        }

        private void saveLayoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SuperPuTTY.CurrentLayout != null)
            {
                String file = SuperPuTTY.CurrentLayout.FilePath;
                SaveLayout(file, string.Format("Saving layout: {0}", file));
            }
            else
            {
                saveLayoutAsToolStripMenuItem_Click(sender, e);
            }
        }

        private void saveLayoutAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == this.saveFileDialogLayout.ShowDialog(this))
            {
                String file = this.saveFileDialogLayout.FileName;
                SaveLayout(file, string.Format("Saving layout as: {0}", file));
                SuperPuTTY.AddLayout(file);
            } 
        }

        void SaveLayout(string file, string statusMsg)
        {
            SuperPuTTY.ReportStatus(statusMsg);
            this.DockPanel.SaveAsXml(file);
        }

        private IDockContent RestoreLayoutFromPersistString(String persistString)
        {
            if (typeof(SessionTreeview).FullName == persistString)
            {
                // session tree
                return this.sessions.Instance ?? this.sessions.Initialize();
            }
            else if (typeof(LayoutsList).FullName == persistString)
            {
                // layouts list
                return this.layouts.Instance ?? this.layouts.Initialize();
            }
            else if (typeof(Log4netLogViewer).FullName == persistString)
            {
                return this.logViewer.Instance ?? this.logViewer.Initialize();
            }
            else
            {
                // putty session
                ctlPuttyPanel puttyPanel = ctlPuttyPanel.FromPersistString(persistString);
                if (puttyPanel != null)
                {
                    return puttyPanel;
                }

                // pscp session (is this possible...prompt is a dialog...make inline?)
                //ctlPuttyPanel puttyPanel = ctlPuttyPanel.FromPersistString(m_Sessions, persistString);
                //if (puttyPanel != null)
                //{
                //    return puttyPanel;
                //}

            }
            return null;
        }


        #endregion

        #region Tools

        private void puTTYConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process p = new Process();
            p.StartInfo.FileName = PuttyExe;
            p.Start();

            SuperPuTTY.ReportStatus("Lauched Putty Configuration");
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SuperPuTTY.ReportStatus("Editing Options");

            dlgFindPutty dialog = new dlgFindPutty();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // try to apply settings to existing documents (don't worry about the ones docked on sides)
                foreach (DockContent dockContent in this.DockPanel.Documents)
                {
                    SuperPuTTY.ApplyDockRestrictions(dockContent);
                }

                this.SaveLastWindowBounds();
            }

            SuperPuTTY.ReportStatus("Ready");
        }

        #endregion

        #region Help Menu
        private void aboutSuperPuttyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 about = new AboutBox1();
            about.ShowDialog();
            about = null;
        }

        private void superPuttyWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://code.google.com/p/superputty/");
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (File.Exists(Application.StartupPath + @"\superputty.chm"))
            {
                Process.Start(Application.StartupPath + @"\superputty.chm");
            }
            else
            {
                DialogResult result = MessageBox.Show("Local documentation could not be found. Would you like to view the documentation online instead?", "Documentation Not Found", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    Process.Start("http://code.google.com/p/superputty/wiki/Documentation");
                }
            }
        }

        private void puTTYScpLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dlgFindPutty dialog = new dlgFindPutty();
            dialog.ShowDialog();
        }
        #endregion

        #region Toolbar


        private string oldHostName;

        private void tbBtnConnect_Click(object sender, EventArgs e)
        {
            TryConnectFromToolbar();
        }

        private void tbItemConnect_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) Keys.Enter)
            {
                TryConnectFromToolbar();
                e.Handled = true;
            }
        }

        void TryConnectFromToolbar()
        {
            String host = this.tbTxtBoxHost.Text;
            String protoString = (string)this.tbComboProtocol.SelectedItem;

            if (!String.IsNullOrEmpty(host))
            {
                HostConnectionString connStr = new HostConnectionString(host);
                bool isScp = "SCP" == protoString;
                ConnectionProtocol proto = isScp 
                    ? ConnectionProtocol.SSH 
                    : connStr.Protocol.GetValueOrDefault((ConnectionProtocol) Enum.Parse(typeof(ConnectionProtocol), protoString));
                SessionData session = new SessionData
                {
                    Host = connStr.Host,
                    SessionName = connStr.Host,
                    SessionId = SuperPuTTY.MakeUniqueSessionId(SessionData.CombineSessionIds("ConnectBar", connStr.Host)),
                    Proto = proto,
                    Port = connStr.Port.GetValueOrDefault(dlgEditSession.GetDefaultPort(proto)),
                    Username = this.tbTxtBoxLogin.Text,
                    Password = this.tbTxtBoxPassword.Text,
                    PuttySession = (string)this.tbComboSession.SelectedItem
                };
                SuperPuTTY.OpenSession(new SessionDataStartInfo { Session = session, UseScp = isScp });
                oldHostName = this.tbTxtBoxHost.Text;
                RefreshConnectionToolbarData();
            }
        }

        void RefreshConnectionToolbarData()
        {
            if (this.tbComboProtocol.Items.Count == 0)
            {
                this.tbComboProtocol.Items.Clear();
                foreach (ConnectionProtocol protocol in Enum.GetValues(typeof(ConnectionProtocol)))
                {
                    this.tbComboProtocol.Items.Add(protocol.ToString());
                }
                this.tbComboProtocol.Items.Add("SCP");
                this.tbComboProtocol.SelectedItem = ConnectionProtocol.SSH.ToString();
            }

            String prevSession = (string)this.tbComboSession.SelectedItem;
            this.tbComboSession.Items.Clear();
            foreach (string sessionName in PuttyDataHelper.GetSessionNames())
            {
                this.tbComboSession.Items.Add(sessionName);
            }
            this.tbComboSession.SelectedItem = prevSession ?? PuttyDataHelper.SessionEmptySettings;
        }

        private void toolStripButtonClearFields_Click(object sender, EventArgs e)
        {
            this.tbComboProtocol.SelectedItem = ConnectionProtocol.SSH.ToString();
            this.tbTxtBoxHost.Clear();
            this.tbTxtBoxLogin.Clear();
            this.tbTxtBoxPassword.Clear();
            this.tbComboSession.SelectedItem = PuttyDataHelper.SessionEmptySettings;
        }

        /// <summary>
        /// Show selector below toolbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsBtnSelectDocs_Click(object sender, EventArgs e)
        {
            Rectangle rect = this.tbBtnSelectDocs.Bounds;
            int top = this.tsCommands.Top + this.tsCommands.Height + 3;
            int left = rect.Left + rect.Width - this.sendCommandsDocumentSelector.Width + 3;
            this.sendCommandsDocumentSelector.StartPosition = FormStartPosition.Manual;
            this.sendCommandsDocumentSelector.Location = this.PointToScreen(new Point(left, top));
            this.sendCommandsDocumentSelector.Show();
        }

        private void tbTextCommand_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                TrySendCommandsFromToolbar(true);
                e.Handled = true;
            }
        }

        private void tsSendCommandCombo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                TrySendCommandsFromToolbar(true);
                e.Handled = true;
            }
        }

        private void tsSendCommandCombo_KeyDown(object sender, KeyEventArgs e)
        {
            
            if (e.KeyCode == Keys.Up)
            {
                if (tsSendCommandCombo.Items.Count > 0)
                {
                    commandMRUIndex--;
                    if (commandMRUIndex < 0)
                    {
                        commandMRUIndex = tsSendCommandCombo.Items.Count - 1;
                    }
                    if (commandMRUIndex >= 0)
                    {
                        tsSendCommandCombo.Text = (string) tsSendCommandCombo.Items[commandMRUIndex];
                        tsSendCommandCombo.SelectionStart = tsSendCommandCombo.Text.Length;
                    }
                }
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (tsSendCommandCombo.Items.Count > 0)
                {
                    commandMRUIndex++;
                    if (commandMRUIndex >= tsSendCommandCombo.Items.Count)
                    {
                        commandMRUIndex = 0;
                    }
                    if (commandMRUIndex < tsSendCommandCombo.Items.Count)
                    {
                        tsSendCommandCombo.Text = (string)tsSendCommandCombo.Items[commandMRUIndex];
                        tsSendCommandCombo.SelectionStart = tsSendCommandCombo.Text.Length;
                    }
                }
                e.Handled = true;
            }
        }

        private void tbBtnSendCommand_Click(object sender, EventArgs e)
        {
            TrySendCommandsFromToolbar(true);
        }


        private void tbBtnSendCommandNoHist_Click(object sender, EventArgs e)
        {
            TrySendCommandsFromToolbar(false);
        }

        int TrySendCommandsFromToolbar(bool saveHistory)
        {
            int sent = 0;
            String command = this.tsSendCommandCombo.Text; //this.tbTextCommand.Text;
            if (this.DockPanel.DocumentsCount > 0)
            {
                foreach (DockContent content in this.DockPanel.Documents)
                {
                    ctlPuttyPanel puttyPanel = content as ctlPuttyPanel;
                    int handle = puttyPanel.AppPanel.AppWindowHandle.ToInt32();
                    if (puttyPanel != null && this.sendCommandsDocumentSelector.IsDocumentSelected(puttyPanel))
                    {
                        Log.InfoFormat("SendCommand: session={0}, command=[{1}]", puttyPanel.Session.SessionId, command);
                        foreach (char c in command)
                        {
                            NativeMethods.SendMessage(handle, NativeMethods.WM_CHAR, (int)c, 0);
                        }

                        NativeMethods.SendMessage(handle, NativeMethods.WM_CHAR, (int) Keys.Enter, 0);
                        //NativeMethods.SendMessage(handle, NativeMethods.WM_KEYUP, (int)Keys.Enter, 0);
                        sent++;
                    }
                }
                if (sent > 0)
                {
                    // success...clear text and save in mru
                    this.tsSendCommandCombo.Text = string.Empty;
                    if (!string.IsNullOrEmpty(command) && saveHistory)
                    {
                        this.tsSendCommandCombo.Items.Add(command);
                    }
                }
            }
            return sent;
        }

        #endregion

        #region Mouse and Keyboard Hooks

        private static IntPtr SetKBHook(NativeMethods.LowLevelKMProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return NativeMethods.SetWindowsHookEx(NativeMethods.WH_KEYBOARD_LL, proc, NativeMethods.GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        // Intercept keyboard messages for Ctrl-F4 and Ctrl-Tab handling
        private IntPtr KBHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && IsForegroundWindow(this.Handle))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                // Log.InfoFormat("KBHook={0}, wParam={1}, lParam={2}", nCode, wParam, vkCode);

                // Detect control key state for left and right control keys
                if ((Keys)vkCode == Keys.LControlKey || (Keys)vkCode == Keys.RControlKey)
                {
                    // Set flag to indicate if Ctrl key is up or down
                    isControlDown = (wParam == (IntPtr)NativeMethods.WM_KEYDOWN);

                    // If Ctrl-Tab has been pressed to move to an older panel then
                    // make it current panel when Ctrl key is finally released.
                    if (!isControlDown && currentTabPanel != null)
                    {
                        currentTabPanel.MakePanelCurrent();
                        currentTabPanel = null;
                    }
                }

                // Operator has pressed Ctrl-F4, close the active PuTTY or file transfer panel
                if (isControlDown && (Keys)vkCode == Keys.F4)
                {
                    if (wParam == (IntPtr)NativeMethods.WM_KEYDOWN)
                    {
                        ToolWindow tw = DockPanel.ActiveDocument as ToolWindow;
                        if (tw != null) { tw.Close(); }
                    }

                    // Eat the keystroke
                    return (IntPtr)1;
                }

                // Operator has pressed Ctrl-End, Move to Next Tab
                if (isControlDown && (Keys)vkCode == Keys.End && wParam == (IntPtr)NativeMethods.WM_KEYDOWN)
                {
                    this.currentTabPanel = null;
                    ToolWindow dcNext = (ToolWindow) this.DockPanel.ActiveDocument;
                    List<IDockContent> docs = new List<IDockContent>(this.DockPanel.DocumentsToArray());
                    int idx = docs.IndexOf(this.DockPanel.ActiveDocument);
                    if (idx != -1){
                        dcNext = (ToolWindow) docs[idx == docs.Count - 1 ? 0 : idx + 1 ];
                        dcNext.Activate();

                        // Eat the keystroke
                        return (IntPtr)1;
                    }
                }

                // Operator has pressed Ctrl-Home, Move to Prev Tab
                if (isControlDown && (Keys)vkCode == Keys.Home && wParam == (IntPtr)NativeMethods.WM_KEYDOWN)
                {
                    this.currentTabPanel = null;
                    ToolWindow dcPrev = (ToolWindow)this.DockPanel.ActiveDocument;
                    List<IDockContent> docs = new List<IDockContent>(this.DockPanel.DocumentsToArray());
                    int idx = docs.IndexOf(this.DockPanel.ActiveDocument);
                    if (idx != -1)
                    {
                        dcPrev = (ToolWindow)docs[idx == 0 ? docs.Count - 1 : idx - 1];
                        dcPrev.Activate();

                        // Eat the keystroke
                        return (IntPtr)1;
                    }
                }

                // Operator has pressed Ctrl-Tab, make previous PuTTY panel active
                if (isControlDown && (Keys)vkCode == Keys.Tab)
                {
                    if (wParam == (IntPtr)NativeMethods.WM_KEYDOWN && this.DockPanel.ActiveDocument is ToolWindowDocument)
                    {

                        if (currentTabPanel == null)
                            currentTabPanel = CurrentPanel;
                        if (currentTabPanel != null && currentTabPanel.PreviousPanel != null)
                        {
                            currentTabPanel = currentTabPanel.PreviousPanel;
                            currentTabPanel.Activate();
                            // RML: Need code to activate main frame SuperPutty window
                            //      while still leaving focus in PuTTY session
                            //                          FocusActiveDocument("After Ctrl-Tab");
                        }
                    }

                    // Eat the keystroke
                    return (IntPtr)1;
                }
            }
                                
            return NativeMethods.CallNextHookEx(kbHookID, nCode, wParam, lParam);
        }

        private static IntPtr SetMHook(NativeMethods.LowLevelKMProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return NativeMethods.SetWindowsHookEx(NativeMethods.WH_MOUSE_LL, proc, NativeMethods.GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr MHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)NativeMethods.WM.LBUTTONUP || wParam == (IntPtr)NativeMethods.WM.RBUTTONUP) && IsForegroundWindow(this.Handle))
            {
                this.BringToFront();
                //if (!Menu_IsMouseOver()) dockPanel.Focus();
            }
            return NativeMethods.CallNextHookEx(mHookID, nCode, wParam, lParam);
        }

        private static bool IsForegroundWindow(IntPtr parent)
        {
            if (parent == NativeMethods.GetForegroundWindow()) return true;
            List<IntPtr> result = new List<IntPtr>();
            GCHandle listHandle = GCHandle.Alloc(result);
            try
            {
                NativeMethods.EnumWindowProc childProc = new NativeMethods.EnumWindowProc(EnumWindow);
                NativeMethods.EnumChildWindows(parent, childProc, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                if (listHandle.IsAllocated)
                    listHandle.Free();
            }
            return result.Count > 0;
        }

        private static bool EnumWindow(IntPtr handle, IntPtr pointer)
        {
            GCHandle gch = GCHandle.FromIntPtr(pointer);
            List<IntPtr> list = gch.Target as List<IntPtr>;
            if (handle == NativeMethods.GetForegroundWindow()) list.Add(handle);
            if (list.Count == 0) return true; else return false;
        }

        #endregion

        #region Tray 
        private void frmSuperPutty_Resize(object sender, EventArgs e)
        {
            if (SuperPuTTY.Settings.MinimizeToTray)
            {
                if (FormWindowState.Minimized == this.WindowState && !notifyicon.Visible)
                {
                    notifyicon.Visible = true;
                    this.ShowInTaskbar = false;

                }
                else if (FormWindowState.Normal == this.WindowState || FormWindowState.Maximized == this.WindowState)
                {
                    notifyicon.Visible = false;
                    this.lastNonMinimizedWindowState = this.WindowState;
                }
            }

            SaveLastWindowBounds();
        }

        private void SaveLastWindowBounds()
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.lastNormalDesktopBounds = this.DesktopBounds;
            }
        }

        private void notifyicon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.ShowInTaskbar = true;
                this.WindowState = this.lastNonMinimizedWindowState;
            }
        }

        private void exitSuperPuTTYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            forceClose = true;
            this.Close();
        }

        #endregion 


        protected override void WndProc(ref Message m)
        {
            bool callBase = this.focusHelper.WndProcForFocus(ref m);
            if (callBase)
            {
                base.WndProc(ref m);
            }
        }


    }
}
