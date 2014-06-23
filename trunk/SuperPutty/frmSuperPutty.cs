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
using System.Collections;
using SuperPutty.Gui;
using System.Threading;
using log4net.Core;
using System.Text.RegularExpressions;

namespace SuperPutty
{
    public partial class frmSuperPutty : Form
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(frmSuperPutty));

        private static string XmlEditor = ConfigurationManager.AppSettings["SuperPuTTY.XmlEditor"];

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
        bool isShiftDown = false;
        bool isAltDown = false;
        int commandMRUIndex = 0;

        private readonly TabSwitcher tabSwitcher;
        private readonly ViewState fullscreenViewState;

        Dictionary<Keys, SuperPuttyAction> shortcuts = new Dictionary<Keys, SuperPuttyAction>();

        public frmSuperPutty()
        {
            // Verify Putty is set; Prompt user if necessary; exit otherwise
            dlgFindPutty.PuttyCheck();

            InitializeComponent();
            // force toolbar locations...designer likes to flip them around
            this.tsConnect.Location = new System.Drawing.Point(0, 24);
            this.tsCommands.Location = new System.Drawing.Point(0, 49);

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
            this.sendCommandsDocumentSelector.Owner = this;

            // Hook into status
            SuperPuTTY.StatusEvent += new Action<string>(delegate(String msg) { this.toolStripStatusLabelMessage.Text = msg; });
            SuperPuTTY.ReportStatus("Ready");


            // Check for updates if enabled. (disabled if compiled with DEBUG)
            if (SuperPuTTY.Settings.AutoUpdateCheck)
            {
#if DEBUG
                Log.Info("Automatic Update Check Disabled in DEBUG mode");
#else
                Log.Info("Checking for updates");
                this.checkForUpdatesToolStripMenuItem_Click(this, new EventArgs());
#endif
            }
            // Hook into LayoutChanging/Changed
            SuperPuTTY.LayoutChanging += new EventHandler<LayoutChangedEventArgs>(SuperPuTTY_LayoutChanging);

            // Low-Level Mouse and Keyboard hooks
            llkp = KBHookCallback;
            kbHookID = SetKBHook(llkp);
            //llmp = MHookCallback;
            //mHookID = SetMHook(llmp);

            this.focusHelper = new ChildWindowFocusHelper(this);
            this.focusHelper.Start();

            // Restore window location and size
            if (SuperPuTTY.Settings.RestoreWindowLocation)
            {
                FormUtils.RestoreFormPositionAndState(this, SuperPuTTY.Settings.WindowPosition, SuperPuTTY.Settings.WindowState);
            }

            this.ResizeEnd += new EventHandler(frmSuperPutty_ResizeEnd);

            // tab switching
            this.tabSwitcher = new TabSwitcher(this.DockPanel);

            // full screen
            this.fullscreenViewState = new ViewState(this);

            // Apply Settings
            this.ApplySettings();
            this.ApplySettingsToToolbars();

            this.DockPanel.ContentAdded += DockPanel_ContentAdded;
            this.DockPanel.ContentRemoved += DockPanel_ContentRemoved;
        }

        void DockPanel_ContentAdded(object sender, DockContentEventArgs e)
        {
            ctlPuttyPanel p = e.Content as ctlPuttyPanel;
            if (p != null)
            {
                p.TextChanged += puttyPanel_TextChanged;
            }
        }

        void DockPanel_ContentRemoved(object sender, DockContentEventArgs e)
        {
            ctlPuttyPanel p = e.Content as ctlPuttyPanel;
            if (p != null)
            {
                p.TextChanged -= puttyPanel_TextChanged;
            }
        }

        void puttyPanel_TextChanged(object sender, EventArgs e)
        {
            ctlPuttyPanel p = (ctlPuttyPanel)sender;
            if (p == this.DockPanel.ActiveDocument)
            {
                UpdateWindowText(p.Text);
            }
        }

        void UpdateWindowText(string text)
        {
            this.Text = string.Format("SuperPuTTY - {0}", text);
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
                    if (!this.tabSwitcher.IsSwitchingTabs)
                    {
                        this.tabSwitcher.CurrentDocument = window;
                    }

                    ctlPuttyPanel p = window as ctlPuttyPanel;
                    if (p != null)
                    {
                        p.SetFocusToChildApplication(caller);
                        this.UpdateWindowText(p.Text);
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
            if (saveDialog.ShowDialog(this) == DialogResult.OK)
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
            if (openDialog.ShowDialog(this) == DialogResult.OK)
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
            if (openDialog.ShowDialog(this) == DialogResult.OK)
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

        private void openSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QuickSelector q = new QuickSelector();
            QuickSelectorData data = new QuickSelectorData();
            data.CaseSensitive = SuperPuTTY.Settings.QuickSelectorCaseSensitiveSearch;

            foreach (SessionData sd in SuperPuTTY.Sessions)
            {
                data.ItemData.AddItemDataRow(
                    sd.SessionName,
                    sd.SessionId,
                    sd.Proto == ConnectionProtocol.Cygterm || sd.Proto == ConnectionProtocol.Mintty ? Color.Blue : Color.Black,
                    null);
            }

            QuickSelectorOptions opt = new QuickSelectorOptions();
            opt.Sort = data.ItemData.DetailColumn.ColumnName;
            opt.BaseText = "Open Session";

            QuickSelector d = new QuickSelector();
            if (d.ShowDialog(this, data, opt) == DialogResult.OK)
            {
                SuperPuTTY.OpenPuttySession(d.SelectedItem.Detail);
            }
        }

        private void switchSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QuickSelector q = new QuickSelector();
            QuickSelectorData data = new QuickSelectorData();
            data.CaseSensitive = SuperPuTTY.Settings.QuickSelectorCaseSensitiveSearch;

            foreach (ToolWindow content in this.tabSwitcher.Documents)
            {
                ctlPuttyPanel panel = content as ctlPuttyPanel;
                if (content != null)
                {
                    SessionData sd = panel.Session;
                    data.ItemData.AddItemDataRow(
                        panel.Text,
                        sd.SessionId,
                        sd.Proto == ConnectionProtocol.Cygterm || sd.Proto == ConnectionProtocol.Mintty ? Color.Blue : Color.Black,
                        panel);
                }
            }

            QuickSelectorOptions opt = new QuickSelectorOptions();
            opt.Sort = data.ItemData.DetailColumn.ColumnName;
            opt.BaseText = "Switch Session";
            opt.ShowNameColumn = true;

            QuickSelector d = new QuickSelector();
            if (d.ShowDialog(this, data, opt) == DialogResult.OK)
            {
                ctlPuttyPanel panel = (ctlPuttyPanel)d.SelectedItem.Tag;
                panel.Activate();
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
            SuperPuTTY.Settings.AlwaysOnTop = this.alwaysOnTopToolStripMenuItem.Checked;
            SuperPuTTY.Settings.ShowMenuBar = this.showMenuBarToolStripMenuItem.Checked;

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

            this.TopMost = SuperPuTTY.Settings.AlwaysOnTop;
            this.alwaysOnTopToolStripMenuItem.Checked = SuperPuTTY.Settings.AlwaysOnTop;

            this.menuStrip1.Visible = SuperPuTTY.Settings.ShowMenuBar;
            this.showMenuBarToolStripMenuItem.Checked = SuperPuTTY.Settings.ShowMenuBar;
        }

        private void sessionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.layouts.IsVisibleAsToolWindow)
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
            if (this.sessions.IsVisibleAsToolWindow)
            {
                this.layouts.ShowWindow(this.sessions.Instance.DockHandler.Pane, DockAlignment.Bottom, 0.5);
            }
            else
            {
                this.layouts.ShowWindow(DockState.DockRight);
            }
        }


        private void fullScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleFullScreen();
        }

        void ToggleFullScreen()
        {
            if (this.fullScreenToolStripMenuItem.Checked)
            {
                Log.InfoFormat("Restore from Fullscreen");
                this.fullscreenViewState.Restore();
            }
            else
            {
                Log.InfoFormat("Go to Fullscreen");
                this.fullscreenViewState.SaveState();
                this.fullscreenViewState.Hide();
            }
            this.fullScreenToolStripMenuItem.Checked = !this.fullScreenToolStripMenuItem.Checked;

        }

        class ViewState
        {
            public ViewState(frmSuperPutty mainForm)
            {
                this.MainForm = mainForm;
                this.ConnectionBarLocation = this.MainForm.tsConnect.Location;
                this.CommandBarLocation = this.MainForm.tsConnect.Location;
            }

            public frmSuperPutty MainForm { get; set; }
            public bool StatusBar { get; set; }
            public bool MenuBar { get; set; }
            public bool ConnectionBar { get; set; }
            public bool CommandBar { get; set; }
            public bool SessionsWindow { get; set; }
            public bool LogWindow { get; set; }
            public bool LayoutWindow { get; set; }

            public FormBorderStyle FormBorderStyle { get; set; }
            public FormWindowState FormWindowState { get; set; }

            public Point ConnectionBarLocation { get; set; }
            public Point CommandBarLocation { get; set; }

            public bool IsFullScreen { get; set; }

            public void SaveState()
            {
                this.StatusBar = this.MainForm.showStatusBarToolStripMenuItem.Checked;
                this.MenuBar = this.MainForm.showMenuBarToolStripMenuItem.Checked;

                this.ConnectionBar = this.MainForm.quickConnectionToolStripMenuItem.Checked;
                this.CommandBar = this.MainForm.sendCommandsToolStripMenuItem.Checked;

                this.SessionsWindow = this.MainForm.sessions.IsVisible;
                this.LogWindow = this.MainForm.logViewer.IsVisible;
                this.LayoutWindow = this.MainForm.layouts.IsVisible;

                this.FormBorderStyle = this.MainForm.FormBorderStyle;
                this.FormWindowState = this.MainForm.WindowState;

            }

            public void Hide()
            {
                try
                {
                    this.MainForm.DockPanel.Visible = false;

                    // windows
                    this.MainForm.sessions.Hide();
                    this.MainForm.layouts.Hide();
                    this.MainForm.logViewer.Hide();

                    // status bar
                    this.MainForm.statusStrip1.Hide();

                    // toolbars
                    this.MainForm.tsCommands.Visible = false;
                    this.MainForm.tsConnect.Visible = false;

                    // menubar
                    this.MainForm.menuStrip1.Hide();

                    this.MainForm.FormBorderStyle = FormBorderStyle.None;
                    if (this.MainForm.WindowState == FormWindowState.Maximized)
                    {
                        // if maximized, goto normal first
                        this.MainForm.WindowState = FormWindowState.Normal;
                    }
                    this.MainForm.WindowState = FormWindowState.Maximized;
                    this.MainForm.TopMost = true;

                    this.IsFullScreen = true;
                }
                finally
                {
                    this.MainForm.DockPanel.Visible = true;
                }

            }

            public void Restore()
            {
                try
                {
                    this.MainForm.DockPanel.Visible = false;

                    // windows
                    if (this.SessionsWindow) { this.MainForm.sessions.Restore(); }
                    if (this.LayoutWindow) { this.MainForm.layouts.Restore(); }
                    if (this.LogWindow) { this.MainForm.logViewer.Restore(); }

                    // status bar
                    if (this.StatusBar) { this.MainForm.statusStrip1.Show(); }

                    // toolbars
                    if (this.CommandBar && this.ConnectionBar)
                    {
                        // both visible so set locations
                        this.MainForm.tsConnect.Visible = true;
                        this.MainForm.tsConnect.Location = this.ConnectionBarLocation;
                        this.MainForm.tsCommands.Visible = true;
                        this.MainForm.tsCommands.Location = this.CommandBarLocation;
                    }
                    else if (this.CommandBar) { this.MainForm.tsCommands.Visible = true; }
                    else if (this.ConnectionBar) { this.MainForm.tsConnect.Visible = true; }

                    // menubar
                    if (this.MenuBar) { this.MainForm.menuStrip1.Show(); }

                    this.MainForm.TopMost = false;
                    this.MainForm.WindowState = this.FormWindowState;
                    this.MainForm.FormBorderStyle = this.FormBorderStyle;
                    this.IsFullScreen = false;
                }
                finally
                {
                    this.MainForm.DockPanel.Visible = true;

                }
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
                // coming from command line, so no don't load any layout
                //SuperPuTTY.LoadLayout(null);
                SuperPuTTY.OpenSession(SuperPuTTY.StartingSession);
            }
            else
            {
                // default layout or null for hard-coded default
                SuperPuTTY.LoadLayout(SuperPuTTY.StartingLayout);
                SuperPuTTY.ApplyDockRestrictions(this.DockPanel);
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
            p.StartInfo.FileName = SuperPuTTY.Settings.PuttyExe;
            p.Start();

            SuperPuTTY.ReportStatus("Lauched Putty Configuration");
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SuperPuTTY.ReportStatus("Editing Options");

            dlgFindPutty dialog = new dlgFindPutty();
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                ApplySettings();
            }

            SuperPuTTY.ReportStatus("Ready");
        }

        void ApplySettings()
        {
            SuperPuTTY.ApplyDockRestrictions(this.DockPanel);

            // apply tab switching strategy change
            this.tabSwitcher.TabSwitchStrategy = TabSwitcher.StrategyFromTypeName(SuperPuTTY.Settings.TabSwitcher);

            this.SaveLastWindowBounds();
            this.UpdateShortcutsFromSettings();
            this.Opacity = SuperPuTTY.Settings.Opacity;
            this.DockPanel.ShowDocumentIcon = SuperPuTTY.Settings.ShowDocumentIcons;
        }

        #endregion

        #region Help Menu
        private void aboutSuperPuttyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 about = new AboutBox1();
            about.ShowDialog(this);
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

        #endregion

        #region Toolbar


        private string oldHostName;

        private void tbBtnConnect_Click(object sender, EventArgs e)
        {
            TryConnectFromToolbar();
        }

        private void tbItemConnect_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
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
                    : connStr.Protocol.GetValueOrDefault((ConnectionProtocol)Enum.Parse(typeof(ConnectionProtocol), protoString));
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

        private void tsSendCommandCombo_KeyDown(object sender, KeyEventArgs e)
        {
            if (Log.Logger.IsEnabledFor(Level.Trace))
            {
                Log.DebugFormat("Keys={0}, control={1}, shift={2}, keyData={3}", e.KeyCode, e.Control, e.Shift, e.KeyData);
            }
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
                        tsSendCommandCombo.Text = (string)tsSendCommandCombo.Items[commandMRUIndex];
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
            else if (e.KeyCode == Keys.Enter)
            {
                // send commands
                TrySendCommandsFromToolbar(new CommandData(this.tsSendCommandCombo.Text), !this.tbBtnMaskText.Checked);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if ((e.Control && e.KeyCode != Keys.ControlKey))
            {
                // special keys
                TrySendCommandsFromToolbar(new CommandData(e), !this.tbBtnMaskText.Checked);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }


        private void tbBtnSendCommand_Click(object sender, EventArgs e)
        {
            TrySendCommandsFromToolbar(!this.tbBtnMaskText.Checked);
        }

        private void toggleCommandMaskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.tbBtnMaskText.PerformClick();
        }

        private void tbBtnMaskText_Click(object sender, EventArgs e)
        {
            IntPtr handle = NativeMethods.GetWindow(this.tsSendCommandCombo.ComboBox.Handle, NativeMethods.GetWindowCmd.GW_CHILD);
            if (this.tbBtnMaskText.Checked)
            {
                NativeMethods.SendMessage(handle, NativeMethods.EM_SETPASSWORDCHAR, (int)'*', 0);
            }
            else
            {
                NativeMethods.SendMessage(handle, NativeMethods.EM_SETPASSWORDCHAR, 0, 0);
            }
            this.tsSendCommandCombo.ComboBox.Refresh();
        }

        int TrySendCommandsFromToolbar(bool saveHistory)
        {
            return TrySendCommandsFromToolbar(new CommandData(this.tsSendCommandCombo.Text), saveHistory);
        }

        int TrySendCommandsFromToolbar(CommandData command, bool saveHistory)
        {
            int sent = 0;
            //String command = this.tsSendCommandCombo.Text; //this.tbTextCommand.Text;
            if (this.DockPanel.DocumentsCount > 0)
            {
                foreach (DockContent content in this.DockPanel.Documents)
                {
                    ctlPuttyPanel puttyPanel = content as ctlPuttyPanel;
                    if (puttyPanel != null && this.sendCommandsDocumentSelector.IsDocumentSelected(puttyPanel))
                    {
                        int handle = puttyPanel.AppPanel.AppWindowHandle.ToInt32();
                        Log.InfoFormat("SendCommand: session={0}, command=[{1}], handle={2}", puttyPanel.Session.SessionId, command, handle);

                        command.SendToTerminal(handle);
                        /*
                        foreach (Char c in command.Chars)
                        {
                            NativeMethods.SendMessage(handle, NativeMethods.WM_CHAR, (int)c, 0);
                        }

                        NativeMethods.SendMessage(handle, NativeMethods.WM_CHAR, (int)Keys.Enter, 0);*/
                        //NativeMethods.SendMessage(handle, NativeMethods.WM_KEYUP, (int)Keys.Enter, 0);
                        sent++;
                    }
                }
                if (sent > 0)
                {
                    // success...clear text and save in mru
                    this.tsSendCommandCombo.Text = string.Empty;
                    if (command != null && !string.IsNullOrEmpty(command.Command) && saveHistory)
                    {
                        this.tsSendCommandCombo.Items.Add(command.ToString());
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
            if (nCode >= 0)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Keys keys = (Keys)vkCode;

                // track key state globally for control/alt/shift is up/down
                bool isKeyDown = (wParam == (IntPtr)NativeMethods.WM_KEYDOWN || wParam == (IntPtr)NativeMethods.WM_SYSKEYDOWN);
                if (keys == Keys.LControlKey || keys == Keys.RControlKey) { isControlDown = isKeyDown; }
                if (keys == Keys.LShiftKey || keys == Keys.RShiftKey) { isShiftDown = isKeyDown; }
                if (keys == Keys.LMenu || keys == Keys.RMenu) { isAltDown = isKeyDown; }

                if (Log.Logger.IsEnabledFor(Level.Trace))
                {
                    Log.DebugFormat("### KBHook: nCode={0}, wParam={1}, lParam={2} ({4,-4} - {3}) [{5}{6}{7}]",
                        nCode, wParam, vkCode, keys, isKeyDown ? "Down" : "Up",
                        (isControlDown ? "Ctrl" : ""), (isAltDown ? "Alt" : ""), (isAltDown ? "Shift" : ""));
                }

                if (IsForegroundWindow(this))
                {
                    // SuperPutty or Putty is the window in front...

                    if (keys == Keys.LControlKey || keys == Keys.RControlKey)
                    {
                        // If Ctrl-Tab has been pressed to move to an older panel then
                        // make it current panel when Ctrl key is finally released.
                        if (SuperPuTTY.Settings.EnableControlTabSwitching && !isControlDown && !isShiftDown)
                        {
                            this.tabSwitcher.CurrentDocument = (ToolWindow)this.DockPanel.ActiveDocument;
                        }
                    }

                    if (keys == Keys.LShiftKey || keys == Keys.RShiftKey)
                    {
                        // If Ctrl-Shift-Tab has been pressed to move to an older panel then
                        // make it current panel when both keys are finally released.
                        if (SuperPuTTY.Settings.EnableControlTabSwitching && !isControlDown && !isShiftDown)
                        {
                            this.tabSwitcher.CurrentDocument = (ToolWindow)this.DockPanel.ActiveDocument;
                        }
                    }

                    if (SuperPuTTY.Settings.EnableControlTabSwitching && isControlDown && !isShiftDown && keys == Keys.Tab)
                    {
                        // Operator has pressed Ctrl-Tab, make next PuTTY panel active
                        if (isKeyDown && this.DockPanel.ActiveDocument is ToolWindowDocument)
                        {
                            if (this.tabSwitcher.MoveToNextDocument())
                            {
                                // Eat the keystroke
                                return (IntPtr)1;
                            }
                        }
                    }

                    if (SuperPuTTY.Settings.EnableControlTabSwitching && isControlDown && isShiftDown && keys == Keys.Tab)
                    {
                        // Operator has pressed Ctrl-Shift-Tab, make previous PuTTY panel active
                        if (isKeyDown && this.DockPanel.ActiveDocument is ToolWindowDocument)
                        {
                            if (this.tabSwitcher.MoveToPrevDocument())
                            {
                                // Eat the keystroke
                                return (IntPtr)1;
                            }
                        }
                    }

                    // misc action handling (eat keyup and down)
                    if (SuperPuTTY.Settings.EnableKeyboadShortcuts &&
                        isKeyDown &&
                        keys != Keys.LControlKey && keys != Keys.RControlKey &&
                        keys != Keys.LMenu && keys != Keys.RMenu &&
                        keys != Keys.LShiftKey && keys != Keys.RShiftKey)
                    {
                        if (isControlDown) keys |= Keys.Control;
                        if (isShiftDown) keys |= Keys.Shift;
                        if (isAltDown) keys |= Keys.Alt;

                        if (Log.Logger.IsEnabledFor(Level.Trace))
                        {
                            Log.DebugFormat("#### TryExecute shortcut: keys={0}", keys);
                        }
                        SuperPuttyAction action;
                        if (this.shortcuts.TryGetValue(keys, out action))
                        {
                            // post action to avoid getting errant keystrokes (e.g. allow current to be eaten)
                            this.BeginInvoke(new Action(() =>
                            {
                                ExecuteSuperPuttyAction(action);
                            }));
                            return (IntPtr)1;
                        }
                    }
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
            if (nCode >= 0 && (wParam == (IntPtr)NativeMethods.WM.LBUTTONUP || wParam == (IntPtr)NativeMethods.WM.RBUTTONUP) && IsForegroundWindow(this))
            {
                this.BringToFront();
                //if (!Menu_IsMouseOver()) dockPanel.Focus();
            }
            return NativeMethods.CallNextHookEx(mHookID, nCode, wParam, lParam);
        }

        private static bool IsForegroundWindow(Form parent)
        {
            IntPtr fgWindow = NativeMethods.GetForegroundWindow();
            if (parent.Handle == fgWindow) return true; // main form is FG
            //foreach (Form f in Application.OpenForms) { if (f.Handle == fgWindow) return true; }
            List<IntPtr> result = new List<IntPtr>();
            GCHandle listHandle = GCHandle.Alloc(result);
            try
            {
                NativeMethods.EnumWindowProc childProc = new NativeMethods.EnumWindowProc(EnumWindow);
                NativeMethods.EnumChildWindows(parent.Handle, childProc, GCHandle.ToIntPtr(listHandle));
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

        void UpdateShortcutsFromSettings()
        {
            // remove old
            this.shortcuts.Clear();
            this.fullScreenToolStripMenuItem.ShortcutKeys = Keys.None;
            this.optionsToolStripMenuItem.ShortcutKeys = Keys.None;

            // reload new definitions
            foreach (KeyboardShortcut ks in SuperPuTTY.Settings.LoadShortcuts())
            {
                try
                {
                    // shortcuts
                    SuperPuttyAction action = (SuperPuttyAction)Enum.Parse(typeof(SuperPuttyAction), ks.Name);
                    Keys keys = ks.Key | ks.Modifiers;
                    if (keys != Keys.None)
                    {
                        this.shortcuts.Add(keys, action);
                    }

                    // sync menu items
                    switch (action)
                    {
                        case SuperPuttyAction.FullScreen:
                            this.fullScreenToolStripMenuItem.ShortcutKeys = keys;
                            break;
                        case SuperPuttyAction.Options:
                            this.optionsToolStripMenuItem.ShortcutKeys = keys;
                            break;
                        case SuperPuttyAction.OpenSession:
                            this.openSessionToolStripMenuItem.ShortcutKeys = keys;
                            break;
                        case SuperPuttyAction.SwitchSession:
                            this.switchSessionToolStripMenuItem.ShortcutKeys = keys;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Log.ErrorFormat("Error creating shortcut: " + ks + ", disabled.", ex);
                }

            }
        }

        bool ExecuteSuperPuttyAction(SuperPuttyAction action)
        {
            bool success = true;

            Log.InfoFormat("Executing action, name={0}", action);
            switch (action)
            {
                case SuperPuttyAction.CloseTab:
                    ToolWindow win = this.DockPanel.ActiveDocument as ToolWindow;
                    if (win != null) { win.Close(); }
                    break;
                case SuperPuttyAction.NextTab:
                    this.tabSwitcher.MoveToNextDocument();
                    break;
                case SuperPuttyAction.PrevTab:
                    this.tabSwitcher.MoveToPrevDocument();
                    break;
                case SuperPuttyAction.FullScreen:
                    this.ToggleFullScreen();
                    break;
                case SuperPuttyAction.OpenSession:
                    KeyEventWindowActivator.ActivateForm(this);
                    this.openSessionToolStripMenuItem.PerformClick();
                    break;
                case SuperPuttyAction.SwitchSession:
                    KeyEventWindowActivator.ActivateForm(this);
                    this.switchSessionToolStripMenuItem.PerformClick();
                    break;
                case SuperPuttyAction.Options:
                    KeyEventWindowActivator.ActivateForm(this);
                    this.optionsToolStripMenuItem.PerformClick();
                    break;
                case SuperPuttyAction.DuplicateSession:
                    ctlPuttyPanel p = this.DockPanel.ActiveDocument as ctlPuttyPanel;
                    if (p != null && p.Session != null)
                    {
                        SuperPuTTY.OpenPuttySession(p.Session);
                    }
                    break;
                case SuperPuttyAction.GotoCommandBar:
                    if (!this.fullscreenViewState.IsFullScreen)
                    {
                        KeyEventWindowActivator.ActivateForm(this);
                        if (!this.tsCommands.Visible)
                        {
                            this.toggleCheckedState(this.sendCommandsToolStripMenuItem, EventArgs.Empty);
                        }
                        this.tsSendCommandCombo.Focus();
                    }
                    break;
                case SuperPuttyAction.GotoConnectionBar:
                    // perhaps consider allowing this later...need to really have a better approach to the state saving/invoking the toggle.
                    if (!this.fullscreenViewState.IsFullScreen)
                    {
                        KeyEventWindowActivator.ActivateForm(this);
                        if (!this.tsConnect.Visible)
                        {
                            this.toggleCheckedState(this.quickConnectionToolStripMenuItem, EventArgs.Empty);
                        }
                        this.tbTxtBoxHost.Focus();
                    }
                    break;
                case SuperPuttyAction.FocusActiveSession:
                    // focus on current super putty session...or at least try to
                    KeyEventWindowActivator.ActivateForm(this);
                    ctlPuttyPanel putty = this.DockPanel.ActiveDocument as ctlPuttyPanel;
                    if (putty != null)
                    {
                        putty.SetFocusToChildApplication("ExecuteAction");
                    }
                    break;
                default:
                    success = false;
                    break;
            }

            return success;
        }


        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((keyData & Keys.Alt) == Keys.Alt)
            {
                menuStrip1.Visible = true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
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

        #region Diagnostics

        private void logWindowLocationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (IDockContent c in this.DockPanel.Documents)
            {
                ctlPuttyPanel panel = c as ctlPuttyPanel;
                if (c != null)
                {
                    NativeMethods.RECT rect = new NativeMethods.RECT();
                    NativeMethods.GetWindowRect(panel.AppPanel.AppWindowHandle, ref rect);
                    Point p = panel.PointToScreen(new Point());
                    Log.InfoFormat(
                        "[{0,-20} {1,8}] WindowLocations: panel={2}, putty={3}, x={4}, y={5}",
                        panel.Text + (panel == panel.DockPanel.ActiveDocument ? "*" : ""),
                        panel.AppPanel.AppWindowHandle,
                        panel.DisplayRectangle,
                        rect, p.X, p.Y);
                }
            }
        }

        private void cleanUpStrayProcessesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Regex regex = new Regex(@"^(putty|pscp|cthelper|bash|mintty)$", RegexOptions.IgnoreCase);
                IDictionary<string, List<Process>> procs = new Dictionary<string, List<Process>>();
                foreach (Process p in Process.GetProcesses())
                {
                    if (regex.IsMatch(p.ProcessName))
                    {
                        List<Process> procList;
                        if (!procs.TryGetValue(p.ProcessName, out procList))
                        {
                            procList = new List<Process>();
                            procs.Add(p.ProcessName, procList);
                        }
                        procList.Add(p);
                    }
                }

                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<string, List<Process>> plist in procs)
                {
                    sb.AppendFormat("{0} ({1})", plist.Key, plist.Value.Count).AppendLine();
                }
                if (procs.Count > 0 && DialogResult.OK == MessageBox.Show(this, sb.ToString(), "Kill Processes?", MessageBoxButtons.OKCancel))
                {
                    int success = 0;
                    int error = 0;
                    foreach (KeyValuePair<string, List<Process>> plist in procs)
                    {
                        foreach (Process procToKill in plist.Value)
                        {
                            try
                            {
                                procToKill.Kill();
                                success++;
                            }
                            catch (Exception ex)
                            {
                                Log.ErrorFormat("Error killing proc: {0} ({1})", procToKill.ProcessName, procToKill.Id, ex);
                                error++;
                            }
                        }
                    }
                    MessageBox.Show(this, string.Format("Killed {0} processes, {1} errors", success, error), "Clean Up Complete");
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("");
                Log.Error(msg, ex);
                MessageBox.Show(this, msg, "Error Cleaning Processes");
            }
        }
        private void menuStrip1_MenuDeactivate(object sender, EventArgs e)
        {
            menuStrip1.Visible = SuperPuTTY.Settings.ShowMenuBar;
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.Info("Checking for application update");
            httpRequest r = new httpRequest();
            r.MakeRequest("https://code.google.com/p/superputty/wiki/Downloads?" + SuperPuTTY.Version, delegate(bool success, string content)
            {
                if (success)
                {
                    if (!content.Contains("Current stable version (" + SuperPuTTY.Version + "):"))
                    {
                        Log.Info("New Application version found! " + content.TrimEnd());

                        if (MessageBox.Show("An updated version of SuperPuTTY is Available Would you like to visit the download page?",
                            "SuperPutty Update Found",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button1,
                            MessageBoxOptions.DefaultDesktopOnly) == System.Windows.Forms.DialogResult.Yes)
                        {
                            Process.Start("https://code.google.com/p/superputty/wiki/Downloads");
                        }
                    }
                    else
                    {
                        if (sender.ToString().Equals(checkForUpdatesToolStripMenuItem.Text))
                        {
                            MessageBox.Show("You are running the latest version of SuperPutty", "SuperPuTTY Update Check", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            });

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

        public enum TabTextBehavior
        {
            Static,
            Dynamic,
            Mixed
        }

    }
}
