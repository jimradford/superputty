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

namespace SuperPutty
{
    public partial class frmSuperPutty : Form
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(frmSuperPutty));

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

        private SessionTreeview m_Sessions;
        private LayoutsList m_Layouts;
        private Log4netLogViewer m_logViewer = null;
        private NativeMethods.LowLevelKMProc llkp;
        private NativeMethods.LowLevelKMProc llmp;
        private static IntPtr kbHookID = IntPtr.Zero;
        private static IntPtr mHookID = IntPtr.Zero;

        int commandMRUIndex = 0;

        public frmSuperPutty()
        {
            // Verify Putty is set; Prompt user if necessary; exit otherwise
            dlgFindPutty.PuttyCheck();
            
            InitializeComponent();
            this.tbTxtBoxPassword.TextBox.PasswordChar = '*';
            this.RefreshConnectionToolbarData();

            /* 
             * Open the session treeview and dock it on the right
             */
            m_Sessions = new SessionTreeview(DockPanel);
            m_Sessions.CloseButtonVisible = false;

            m_Layouts = new LayoutsList();
            m_Layouts.CloseButtonVisible = false;

            // Hook into status
            SuperPuTTY.StatusEvent += new Action<string>(delegate(String msg) { this.toolStripStatusLabelMessage.Text = msg; });
            SuperPuTTY.ReportStatus("Ready");

            // Hook into LayoutChanging/Changed
            SuperPuTTY.LayoutChanging += new EventHandler<LayoutChangedEventArgs>(SuperPuTTY_LayoutChanging);

            this.toolStripStatusLabelVersion.Text = SuperPuTTY.Version;

            // Low-Level Mouse and Keyboard hooks
            llkp = KBHookCallback;
            //kbHookID = SetKBHook(llkp);
            llmp = MHookCallback;
            mHookID = SetMHook(llmp);

            // Restore window location and size
            if (SuperPuTTY.Settings.RestoreWindowLocation)
            {
                FormUtils.RestoreFormPositionAndState(this, SuperPuTTY.Settings.WindowPosition, SuperPuTTY.Settings.WindowState);
            }

            ApplySettingsToToolbars();
        }

        private void frmSuperPutty_Load(object sender, EventArgs e)
        {
            this.BeginInvoke(new Action(this.LoadLayout));
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // free hooks
            //NativeMethods.UnhookWindowsHookEx(kbHookID);
            NativeMethods.UnhookWindowsHookEx(mHookID);

            // save window size and location if not maximized or minimized
            if (SuperPuTTY.Settings.RestoreWindowLocation && this.WindowState != FormWindowState.Minimized)
            {
                SuperPuTTY.Settings.WindowPosition = this.DesktopBounds;
                SuperPuTTY.Settings.WindowState = this.WindowState;
                SuperPuTTY.Settings.Save();
            }

            base.OnFormClosed(e);
        }



        private void frmSuperPutty_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (SuperPuTTY.Settings.ExitConfirmation)
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
            if (DockPanel.ActiveDocument is ctlPuttyPanel)
            {
                ctlPuttyPanel p = (ctlPuttyPanel)DockPanel.ActiveDocument;
                p.SetFocusToChildApplication();

                this.Text = string.Format("SuperPuTTY - {0}", p.Text);
            }
        }

        private void frmSuperPutty_Activated(object sender, EventArgs e)
        {
            //dockPanel1_ActiveDocumentChanged(null, null);
        }

        #region File

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "XML Files|*.xml";
            saveDialog.FileName = "Sessions.XML";
            saveDialog.InitialDirectory = Application.StartupPath;
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                SessionData.SaveSessionsToFile(SuperPuTTY.GetAllSessions(), saveDialog.FileName);
            }
        }

        private void importSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "XML Files|*.xml";
            openDialog.FileName = "Sessions.XML";
            openDialog.CheckFileExists = true;
            openDialog.InitialDirectory = Application.StartupPath;
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                SuperPuTTY.ImportSessionsFromFile(openDialog.FileName);
            }
        }

        private void editSessionsInNotepadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("notepad", Path.Combine(SuperPuTTY.Settings.SettingsFolder, "Sessions.XML"));
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

        #region CmdLine

        protected override void WndProc(ref Message m)
        {
            //Log.Info("## - " + m.Msg);
            if (m.Msg == 0x004A)
            {
                NativeMethods.COPYDATA cd = (NativeMethods.COPYDATA)Marshal.PtrToStructure(m.LParam, typeof(NativeMethods.COPYDATA));
                string strArgs = Marshal.PtrToStringAnsi(cd.lpData);
                string[] args = strArgs.Split(' ');

                CommandLineOptions opts = new CommandLineOptions(args);
                if (opts.IsValid)
                {
                    SessionDataStartInfo ssi = opts.ToSessionStartInfo();
                    if (ssi != null)
                    {
                        SuperPuTTY.OpenSession(ssi);
                    }
                }
            }
            base.WndProc(ref m);
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
                    m_Sessions.Show(this.DockPanel, WeifenLuo.WinFormsUI.Docking.DockState.DockRight);
                    m_Layouts.Show(m_Sessions.DockHandler.Pane, DockAlignment.Bottom, 0.5);
                    toolStripStatusLabelLayout.Text = "";
                    SuperPuTTY.ReportStatus("Initialized default layout");
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

        private void saveLayoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SuperPuTTY.CurrentLayout != null)
            {
                String file = SuperPuTTY.CurrentLayout.FilePath;
                SuperPuTTY.ReportStatus("Saving layout: {0}", file);
                this.DockPanel.SaveAsXml(file);
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
                SuperPuTTY.ReportStatus("Saving layout as: {0}", file);
                this.DockPanel.SaveAsXml(file);
                SuperPuTTY.AddLayout(file);
            } 
        }

        private IDockContent RestoreLayoutFromPersistString(String persistString)
        {
            if (typeof(SessionTreeview).FullName == persistString)
            {
                // session tree
                return this.m_Sessions;
            }
            else if (typeof(LayoutsList).FullName == persistString)
            {
                // session tree
                return this.m_Layouts;
            }
            else if (typeof(Log4netLogViewer).FullName == persistString)
            {
                InitLogViewer();
                return this.m_logViewer;
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

        private void logViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //DebugLogViewer logView = new DebugLogViewer();
            //logView.Show(dockPanel1, WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide);
            if (this.m_logViewer == null)
            {
                InitLogViewer();
                this.m_logViewer.Show(DockPanel, WeifenLuo.WinFormsUI.Docking.DockState.DockBottom);
                SuperPuTTY.ReportStatus("Showing Log Viewer");
            }
            else
            {
                this.m_logViewer.Show(DockPanel);
                SuperPuTTY.ReportStatus("Bringing Log Viewer to Front");
            }

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
            }

            SuperPuTTY.ReportStatus("Ready");
        }

        void InitLogViewer()
        {
            if (this.m_logViewer == null)
            {
                this.m_logViewer = new Log4netLogViewer();
                this.m_logViewer.FormClosed += delegate { this.m_logViewer = null; };
            }
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
            String port = null;
            int idx = this.tbTxtBoxHost.Text.LastIndexOf(":");
            if (idx != -1)
            {
                host = this.tbTxtBoxHost.Text.Substring(0, idx);
                port = this.tbTxtBoxHost.Text.Substring(idx + 1);
            }
            String protoString = (string)this.tbComboProtocol.SelectedItem;

            if (!String.IsNullOrEmpty(host))
            {
                bool isScp = "SCP" == protoString;
                ConnectionProtocol proto = isScp ? ConnectionProtocol.SSH : (ConnectionProtocol) Enum.Parse(typeof(ConnectionProtocol), protoString);
                SessionData session = new SessionData
                {
                    Host = host,
                    SessionName = this.tbTxtBoxHost.Text,
                    SessionId = SuperPuTTY.MakeUniqueSessionId(SessionData.CombineSessionIds("ConnectBar", this.tbTxtBoxHost.Text)),
                    Proto = proto,
                    Port = port == null ? dlgEditSession.GetDefaultPort(proto) : int.Parse(port),
                    Username = this.tbTxtBoxLogin.Text,
                    Password = this.tbTxtBoxPassword.Text,
                    PuttySession = (string)this.tbComboSession.SelectedItem
                };
                SuperPuTTY.OpenSession(new SessionDataStartInfo { Session = session, UseScp = isScp });

                RefreshConnectionToolbarData();
            }
        }

        void RefreshConnectionToolbarData()
        {
            String prevProto = (string) this.tbComboProtocol.SelectedItem;
            this.tbComboProtocol.Items.Clear();
            foreach (ConnectionProtocol protocol in Enum.GetValues(typeof(ConnectionProtocol)))
            {
                this.tbComboProtocol.Items.Add(protocol.ToString());
            }
            this.tbComboProtocol.Items.Add("SCP");
            this.tbComboProtocol.SelectedItem = prevProto ?? ConnectionProtocol.SSH.ToString();

            String prevSession = (string)this.tbComboSession.SelectedItem;
            this.tbComboSession.Items.Clear();
            foreach (string sessionName in PuttyDataHelper.GetSessionNames())
            {
                this.tbComboSession.Items.Add(sessionName);
            }
            this.tbComboSession.SelectedItem = prevSession ?? "Default Settings";
        }

        private void tbComboProtocol_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((string)this.tbComboProtocol.SelectedItem == ConnectionProtocol.Cygterm.ToString())
            {
                oldHostName = this.tbTxtBoxHost.Text;
                this.tbTxtBoxHost.Text = oldHostName.StartsWith(CygtermInfo.LocalHost) ? oldHostName : CygtermInfo.LocalHost;
            }
            else
            {
                this.tbTxtBoxHost.Text = oldHostName;
            }
        }

        private void tbTextCommand_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                TrySendCommandsFromToolbar();
                e.Handled = true;
            }
        }

        private void tsSendCommandCombo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                TrySendCommandsFromToolbar();
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
            TrySendCommandsFromToolbar();
        }

        int TrySendCommandsFromToolbar()
        {
            int sent = 0;
            String command = this.tsSendCommandCombo.Text; //this.tbTextCommand.Text;
            if (this.DockPanel.DocumentsCount > 0)
            {
                foreach (DockContent content in this.DockPanel.Documents)
                {
                    ctlPuttyPanel puttyPanel = content as ctlPuttyPanel;
                    int handle = puttyPanel.AppPanel.AppWindowHandle.ToInt32();
                    if (puttyPanel != null)
                    {
                        Log.InfoFormat("SendCommand: session={0}, command=[{1}]", puttyPanel.Session.SessionId, command);
                        foreach (char c in command)
                        {
                            NativeMethods.SendMessage(handle, NativeMethods.WM_CHAR, (int)c, 0);
                        }

                        NativeMethods.SendMessage(handle, NativeMethods.WM_KEYUP, (int)Keys.Enter, 0);
                        sent++;
                    }
                }
                if (sent > 0)
                {
                    // success...clear text and save in mru
                    this.tsSendCommandCombo.Text = string.Empty;
                    if (!string.IsNullOrEmpty(command))
                    {
                        this.tsSendCommandCombo.Items.Add(command);
                    }
                }
            }
            return sent;
        }

        #endregion

        #region Mouse Hooks

        private static IntPtr SetKBHook(NativeMethods.LowLevelKMProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return NativeMethods.SetWindowsHookEx(NativeMethods.WH_KEYBOARD_LL, proc, NativeMethods.GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr KBHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            Log.InfoFormat("KBHook={0}, Keys={1}", nCode, wParam);
            if (nCode >= 0 && wParam == (IntPtr)NativeMethods.WM_SYSKEYDOWN && IsForegroundWindow(this.Handle))
            {
                int vkCode = Marshal.ReadInt32(lParam);

                Log.InfoFormat("VK={0}, Keys={1}", vkCode, (Keys) vkCode);
                if ((Keys)vkCode == Keys.Menu || (Keys)vkCode == Keys.LMenu || (Keys)vkCode == Keys.RMenu)
                {
                    //menuStrip.Visible = true;
                    //menuStrip.Focus();
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


    }
}
