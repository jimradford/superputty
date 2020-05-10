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
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.IO;
using log4net;
using System.Configuration;
using System.Collections.Generic;
using SuperPutty.Utils;
using System.Text;

namespace SuperPutty
{
    public delegate void PuttyClosedCallback(bool error);

    public class ApplicationPanel : System.Windows.Forms.Panel
    {
        #region Private Member Variables

        private static readonly ILog Log = LogManager.GetLogger(typeof(ApplicationPanel));

        private static bool RefocusOnVisChanged = Convert.ToBoolean(ConfigurationManager.AppSettings["SuperPuTTY.RefocusOnVisChanged"] ?? "False");
        private static bool LoopWaitForHandle = Convert.ToBoolean(ConfigurationManager.AppSettings["SuperPuTTY.LoopWaitForHandle"] ?? "False");
        private static int ClosePuttyWaitTimeMs = Convert.ToInt32(ConfigurationManager.AppSettings["SuperPuTTY.ClosePuttyWaitTimeMs"] ?? "100");
        private static string ActivatorTypeName = ConfigurationManager.AppSettings["SuperPuTTY.ActivatorTypeName"] ?? typeof(KeyEventWindowActivator).FullName;

        private Process m_Process;
        private bool m_Created = false;
        private IntPtr m_AppWin;
        private bool b_AppWinFinal = true;
        private System.ComponentModel.BackgroundWorker bgWinTracker;
        private List<IntPtr> m_hWinEventHooks = new List<IntPtr>();
        private List<NativeMethods.WinEventDelegate> lpfnWinEventProcs = new List<NativeMethods.WinEventDelegate>();
        private WindowActivator m_windowActivator = null;
        private SuperPutty.Data.ConnectionProtocol proto;

        internal PuttyClosedCallback m_CloseCallback;

        /// <summary>Set the name of the application executable to launch</summary>
        [Category("Data"), Description("The path/file to launch"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ApplicationName { get; set; }

        [Category("Data"), Description("The parameters to pass to the application being launched"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ApplicationParameters { get; set; }

        [Category("Data"), Description("The starting directory for the putty shell.  Relevant only to cygterm sessions"),
DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ApplicationWorkingDirectory { get; set; }

        public IntPtr AppWindowHandle { get { return this.m_AppWin; } } 
        
        // Some windows need closed with WM_DESTROY others need closed with WM_CLOSE or they leave zombies
        public bool ApplicationCloseWithDestroy { get; set; }

        #endregion

        public ApplicationPanel(SuperPutty.Data.ConnectionProtocol proto)
        {
            this.ApplicationName = "";
            this.ApplicationParameters = "";
            this.ApplicationWorkingDirectory = "";
            this.proto = proto;
            this.bgWinTracker = new System.ComponentModel.BackgroundWorker();
            this.bgWinTracker.WorkerSupportsCancellation = true;

            this.Disposed += new EventHandler(ApplicationPanel_Disposed);
            SuperPuTTY.LayoutChanged += new EventHandler<Data.LayoutChangedEventArgs>(SuperPuTTY_LayoutChanged);

            // setup up the hook to watch for all EVENT_SYSTEM_FOREGROUND events system wide

            string typeName = string.IsNullOrEmpty(SuperPuTTY.Settings.WindowActivator) ? ActivatorTypeName : SuperPuTTY.Settings.WindowActivator;
            this.m_windowActivator = (WindowActivator)Activator.CreateInstance(Type.GetType(typeName));
            //this.m_windowActivator = new SetFGCombinedWindowActivator();
            SuperPuTTY.Settings.SettingsSaving += Settings_SettingsSaving;
            SuperPuTTY.WindowEvents.SystemSwitch += new EventHandler<GlobalWindowEventArgs>(OnSystemSwitch);
        }

        void Settings_SettingsSaving(object sender, CancelEventArgs e)
        {
            this.UpdateTitle();
        }

        void ApplicationPanel_Disposed(object sender, EventArgs e)
        {
            this.Disposed -= new EventHandler(ApplicationPanel_Disposed);
            SuperPuTTY.LayoutChanged -= new EventHandler<Data.LayoutChangedEventArgs>(SuperPuTTY_LayoutChanged);
            SuperPuTTY.Settings.SettingsSaving -= Settings_SettingsSaving;
            SuperPuTTY.WindowEvents.SystemSwitch -= new EventHandler<GlobalWindowEventArgs>(OnSystemSwitch);
            this.m_hWinEventHooks.ForEach(delegate(IntPtr hook) {
                NativeMethods.UnhookWinEvent(hook);
            });
            this.m_hWinEventHooks.Clear();
            this.lpfnWinEventProcs.Clear();
        }

        void SuperPuTTY_LayoutChanged(object sender, Data.LayoutChangedEventArgs e)
        {
            // move 1x after we're done loading
            this.MoveWindow("LayoutChanged");
        }

        public void RefreshAppWindow()
        {
            this.MoveWindow("RefreshWindow");
        }

        private void MoveWindow(string src)
        {
            // if there is more than one screen and we're maximizing the window on the non-primary screen
            // and the non-primary screen has greater resolution than the primary, do an extra move window
            if (Screen.AllScreens.Length > 1 && SuperPuTTY.MainForm.WindowState == FormWindowState.Maximized)
            {
                Screen screen = Screen.FromControl(this);
                Screen primary = Screen.PrimaryScreen;
                int screenArea = screen.WorkingArea.Height * screen.WorkingArea.Width;
                int primaryArea = primary.WorkingArea.Height * primary.WorkingArea.Width;
                if (screen != primary && screenArea > primaryArea)
                {
                    this.MoveWindow("2ndScreenFix", 0, 1);
                }
            }
            MoveWindow(src, 0, 0);
        }

        private void MoveWindow(string src, int x, int y)
        {
            if (!SuperPuTTY.IsLayoutChanging)
            {
                bool success = NativeMethods.MoveWindow(m_AppWin, x, y, this.Width, this.Height, this.Visible);
                if (Log.IsInfoEnabled)
                {
                    Log.InfoFormat("MoveWindow [{3,-15}{4,20}] w={0,4}, h={1,4}, visible={2}, success={5}", this.Width, this.Height, this.Visible, src, this.Name, success);
                }
            }
        }

        public bool ReFocusPuTTY(string caller)
        {
            bool result = false;
            if (this.proto == SuperPutty.Data.ConnectionProtocol.RDP) /* Otherwise window will be hidden and require SuperPutTTY minimize-restore cycle */
                this.MoveWindow("RestoreTabSwitch");
            if (this.ExternalProcessCaptured && NativeMethods.GetForegroundWindow() != this.m_AppWin)
            {
                //Log.InfoFormat("[{0}] ReFocusPuTTY - puttyTab={1}, caller={2}", this.m_AppWin, this.Parent.Text, caller);
                settingForeground = true;
                result = NativeMethods.SetForegroundWindow(this.m_AppWin);
                if (result)
                    NativeMethods.InvalidateRect(this.m_AppWin, IntPtr.Zero, false);
                Log.InfoFormat("[{0}] ReFocusPuTTY - puttyTab={1}, caller={2}, result={3}", this.m_AppWin, this.Parent.Text, caller, result);
            }
            //return (this.m_AppWin != null
            //    && NativeMethods.GetForegroundWindow() != this.m_AppWin
            //    && !NativeMethods.SetForegroundWindow(this.m_AppWin));

            return result;
        }

        /// <summary>
        /// Wait for child process tree to reach final window state.
        /// Process can spawn children and end itself, so try to track it if needed
        ///  and return new process if needed
        /// </summary>
        /// <param name="prc">The first child process to start tracing on</param>
        private Process WaitForTargetProcess(Process prc)
        {
            // Wait for application to start and become idle
            if (this.proto != SuperPutty.Data.ConnectionProtocol.WINCMD && this.proto != SuperPutty.Data.ConnectionProtocol.PS) /* Console applications don't respond to some GUI specific calls */
                prc.WaitForInputIdle();

            return prc;
        }

        private void AttachToWindow()
        {
            if (this.m_AppWin != IntPtr.Zero)
            {
                // Set the application as a child of the parent form
                NativeMethods.SetParent(m_AppWin, this.Handle);

                // Show it! (must be done before we set the windows visibility parameters below
                NativeMethods.ShowWindow(m_AppWin, NativeMethods.WindowShowStyle.Maximize);

                // set window parameters (how it's displayed)
                long lStyle = NativeMethods.GetWindowLong(m_AppWin, NativeMethods.GWL_STYLE);
                lStyle &= ~NativeMethods.WS_BORDER;
                if (this.proto == SuperPutty.Data.ConnectionProtocol.VNC)
                    lStyle |= NativeMethods.WS_HSCROLL | NativeMethods.WS_VSCROLL;
                NativeMethods.SetWindowLong(m_AppWin, NativeMethods.GWL_STYLE, lStyle);
                NativeMethods.WinEventDelegate lpfnWinEventProc = new NativeMethods.WinEventDelegate(WinEventProc);
                this.lpfnWinEventProcs.Add(lpfnWinEventProc);
                uint eventType = (uint)NativeMethods.WinEvents.EVENT_OBJECT_NAMECHANGE;
                this.m_hWinEventHooks.Add(NativeMethods.SetWinEventHook(eventType, eventType, IntPtr.Zero, lpfnWinEventProc, (uint)m_Process.Id, 0, NativeMethods.WINEVENT_OUTOFCONTEXT));
                if ((lStyle & NativeMethods.WS_DLGFRAME) == 0)
                {
                    eventType = (uint)NativeMethods.WinEvents.EVENT_SYSTEM_FOREGROUND;
                    this.m_hWinEventHooks.Add(NativeMethods.SetWinEventHook(eventType, eventType, IntPtr.Zero, lpfnWinEventProc, (uint)m_Process.Id, 0, NativeMethods.WINEVENT_OUTOFCONTEXT));
                }
            }
            else
            {
                MessageBox.Show("Process window not found.", "Process Window Not Found");
                try {
                    m_Process.Kill();
                } 
                catch (InvalidOperationException ex)
                {
                    Log.WarnFormat("no process window found to kill: {0}", ex.Message);
                }
                return;
            }
        }

        private int GetMaxWindowPoolingTime()
        {
            return this.proto == SuperPutty.Data.ConnectionProtocol.RDP ? 30 : 10;
        }

        private bool IsWindowAppliesForInherit(IntPtr hWnd)
        {
            switch (this.proto)
            {
                case SuperPutty.Data.ConnectionProtocol.RDP:
                    StringBuilder winTitleBuf = new StringBuilder(256);
                    int winTitleLen = NativeMethods.GetWindowText(hWnd, winTitleBuf, winTitleBuf.Capacity - 1);
                    Log.Info("IsWindowAppliesForInherit: Evaluating window " + winTitleBuf.ToString());
                    if (winTitleLen > 0 && (winTitleBuf.ToString().Contains(" - Remote Desktop Connection") || winTitleBuf.ToString().Contains("FreeRDP: ")))
                        return true;
                    return false;
                default:
                    return true;
            }
        }

        private void CreateVirtWindow()
        {
            Log.Info("Creating virtual window");
            IntPtr hWnd = NativeMethods.CreateWindowEx(NativeMethods.WS_EX_TRANSPARENT ,new StringBuilder("STATIC"), new StringBuilder(""), 0, 1, 1,
                        1, 1, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            m_AppWin = hWnd;
            if (hWnd == IntPtr.Zero)
                Log.Info("Failed to create virtual window");
        }

        private void bgWinTracker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                System.Threading.Thread.Sleep(1000);
                m_Process.Refresh();
                if (this.IsWindowAppliesForInherit(m_Process.MainWindowHandle))
                {
                    e.Result = m_Process.MainWindowHandle;
                    break;
                }
            }
        }

        private void bgWinTracker_Done(
            object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null || e.Cancelled)
                return;

            m_AppWin = (IntPtr)e.Result;
            Log.Info("Catched delayed window from caller bgWinTracker_Done");
            b_AppWinFinal = true;

            this.AttachToWindow();
            // Move the child so it's located over the parent
            this.MoveWindow("OnVisChanged");
            
            if (RefocusOnVisChanged && NativeMethods.GetForegroundWindow() != this.m_AppWin)
            {
                this.BeginInvoke(new MethodInvoker(delegate { this.ReFocusPuTTY("OnVisChanged"); }));
            }
        }

        private void __UNUSED_REFONLY_UpdateTrackedWindowHandle()
        {
            bool bNeedsUpdate = true;
            if (this.b_AppWinFinal)
                return;
            switch (this.proto)
            {
                case SuperPutty.Data.ConnectionProtocol.RDP:
                    Log.Info("Update track is called.");
                    this.m_Process.Refresh();
                    IntPtr winCandidate = this.m_Process.MainWindowHandle;
                    Log.Info("Update, WinCnd="+((int)winCandidate));
                    if (winCandidate == IntPtr.Zero || winCandidate == this.m_AppWin)
                        break;
                    NativeMethods.RECT rect = new NativeMethods.RECT();
                    if (!NativeMethods.GetWindowRect(winCandidate, ref rect))
                        break;
                    int w = rect.Right - rect.Left;
                    int h = rect.Bottom - rect.Top;
                    Log.Info("Checking window handle [PID="+ this.m_Process.Id +"] to " + ((int)winCandidate) + ". Win size is " + w + "x" + h + ". Title is: " + this.m_Process.MainWindowTitle);
                    if ((this.m_Process.MainWindowTitle.Contains(":") && this.m_Process.MainWindowTitle.Contains(" - Remote Desktop Connection")) /*|| (w >= 400 && h >= 120)*/) /* WIP - Heuristic using caption(Non-localized OS) or size for other cases, quite hacky till find better way */
                    {
                        //this.b_AppWinFinal = true;
                        Log.Info("Updated window handle to " + ((int)winCandidate) + ". Win size is " + w + "x" + h + ". Title is: " + this.m_Process.MainWindowTitle);
                        this.m_AppWin = winCandidate;
                    }
                    else if (this.m_Process.MainWindowTitle.Equals("Remote Desktop Connection") /*|| w < 400*/) /* RDP master process or auth dialog */
                    {
                        System.Threading.Thread.Sleep(5000);
                        IntPtr childWin = NativeMethods.GetWindow(winCandidate, NativeMethods.GetWindowCmd.GW_CHILD);
                        Log.Info("Got child win: " + ((int)childWin));
                        StringBuilder childWinTitleBuf = new StringBuilder(256);
                        int childWinTitleLen = NativeMethods.GetWindowText(childWin, childWinTitleBuf, childWinTitleBuf.Capacity - 1);
                        if (childWinTitleLen > 0 && childWinTitleBuf.ToString().Contains(" - Remote Desktop Connection"))
                        {
                            this.b_AppWinFinal = true;
                            Log.Info("Updated transitional child window handle to " + ((int)childWin) + ". Win size is " + w + "x" + h + ". Title is: " + this.m_Process.MainWindowTitle);
                            this.m_AppWin = childWin;
                        }
                        else
                        {
                            //this.b_AppWinFinal = true;
                            Log.Info("Updated transitional window handle to " + ((int)winCandidate) + ". Win size is " + w + "x" + h + ". Title is: " + this.m_Process.MainWindowTitle);
                            this.m_AppWin = winCandidate;
                        }
                    }
                    else
                    {
                        Log.Info("Updated transitional window handle 2 to " + ((int)winCandidate) + ". Win size is " + w + "x" + h + ". Title is: " + this.m_Process.MainWindowTitle);
                        this.m_AppWin = winCandidate;
                    }
                    break;
                default:
                    this.b_AppWinFinal = true;
                    bNeedsUpdate = false;
                    break;
            }

            if (this.b_AppWinFinal && bNeedsUpdate)
            {
                this.AttachToWindow(); /* Must attach to the window */

                if (this.Visible && this.m_Created && this.ExternalProcessCaptured)
                {
                    // Move the child so it's located over the parent
                    this.MoveWindow("UpdateWindow");
                    
                    if (RefocusOnVisChanged && NativeMethods.GetForegroundWindow() != this.m_AppWin)
                    {
                        this.BeginInvoke(new MethodInvoker(delegate { this.ReFocusPuTTY("UpdateWindow"); }));
                    }
                }
            }
        }

        #region Focus Change Handling
        /*************************** Begin Hack to watch for windows focus change events **************************************
        * This is based on this form post:
        * http://social.msdn.microsoft.com/Forums/en-US/clr/thread/c04e343f-f2e7-469a-8a54-48ca84f78c28
        *
        * The idea is to watch for the EVENT_SYSTEM_FOREGROUND window, and when we see that from the putty terminal window
        * bring the superputty window to the foreground
         * 
         * Other hacks:
         * http://stackoverflow.com/questions/4867210/how-to-bring-a-window-foreground-using-c
         * http://stackoverflow.com/questions/46030/c-sharp-force-form-focus
        */

        bool settingForeground = false;
        bool isSwitchingViaAltTab = false;

        void OnSystemSwitch(object sender, GlobalWindowEventArgs e)
        {
            switch (e.eventType)
            {
                case (uint)NativeMethods.WinEvents.EVENT_SYSTEM_SWITCHSTART:
                    this.isSwitchingViaAltTab = true;
                    break;
                case (uint)NativeMethods.WinEvents.EVENT_SYSTEM_SWITCHEND:
                    this.isSwitchingViaAltTab = false;
                    break;
            }
        }

        void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (m_AppWin != hwnd)
                return;

            switch (eventType)
            {
                case (uint)NativeMethods.WinEvents.EVENT_OBJECT_NAMECHANGE:
                    UpdateTitle();
                    break;
                case (uint)NativeMethods.WinEvents.EVENT_SYSTEM_FOREGROUND:
                    UpdateForeground();
                    break;
            }
        }

        void UpdateForeground()
        {
            // if we got the EVENT_SYSTEM_FOREGROUND, and the hwnd is the putty terminal hwnd (m_AppWin)
            // then bring the supperputty window to the foreground
            Log.DebugFormat("[{0}] HandlingForegroundEvent: settingFG={1}", m_AppWin, settingForeground);
            if (settingForeground)
            {
                settingForeground = false;
                return;
            }


            // This is the easiest way I found to get the superputty window to be brought to the top
            // if you leave TopMost = true; then the window will always be on top.
            if (this.TopLevelControl != null)
            {
                Form form = SuperPuTTY.MainForm;
                if (form.WindowState == FormWindowState.Minimized)
                {
                    return;
                }

                DesktopWindow window = DesktopWindow.GetFirstDesktopWindow();
                this.m_windowActivator.ActivateForm(form, window, m_AppWin);

                // focus back to putty via setting active dock panel
                ctlPuttyPanel parent = (ctlPuttyPanel)this.Parent;
                if (parent != null && parent.DockPanel != null)
                {
                    if (parent.DockPanel.ActiveDocument != parent && parent.DockState == DockState.Document)
                    {
                        string activeDoc = parent.DockPanel.ActiveDocument != null
                            ? ((ToolWindow)parent.DockPanel.ActiveDocument).Text : "?";
                        Log.InfoFormat("[{0}] Setting Active Document: {1} -> {2}", m_AppWin, activeDoc, parent.Text);
                        parent.Show();
                    }
                    else
                    {
                        // give focus back
                        this.ReFocusPuTTY("WinEventProc-FG, AltTab=" + isSwitchingViaAltTab);
                    }
                }
            }
        }

        private void UpdateTitle()
        {
            int length = NativeMethods.SendMessage(m_AppWin, NativeMethods.WM_GETTEXTLENGTH, 0, 0) + 1;
            StringBuilder sb = new StringBuilder(length + 1);
            NativeMethods.SendMessage(m_AppWin, NativeMethods.WM_GETTEXT, sb.Capacity, sb);
            string controlText = sb.ToString();
            string parentText = ((ctlPuttyPanel)this.Parent).TextOverride;

            switch ((SuperPutty.frmSuperPutty.TabTextBehavior)Enum.Parse(typeof(frmSuperPutty.TabTextBehavior), SuperPuTTY.Settings.TabTextBehavior))
            {
                case frmSuperPutty.TabTextBehavior.Static:
                    this.Parent.Text = parentText;
                    break;
                case frmSuperPutty.TabTextBehavior.Dynamic:
                    this.Parent.Text = controlText;
                    break;
                case frmSuperPutty.TabTextBehavior.Mixed:
                    this.Parent.Text = parentText + ": " + controlText;
                    break;
            }
        }

        public event EventHandler InnerApplicationFocused;

        void OnInnerApplicationFocused()
        {
            if (this.InnerApplicationFocused != null)
            {
                this.InnerApplicationFocused(this, EventArgs.Empty);
            }
        }

        /*************************** End Hack to watch for windows focus change events ***************************************/
        #endregion

        #region Base Overrides
       
        /// <summary>
        /// Force redraw of control when size changes
        /// </summary>
        /// <param name="e">Not used</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            this.Invalidate();
            base.OnSizeChanged(e);
        }
       
        /// <summary>
        /// Create (start) the hosted application when the parent becomes visible
        /// </summary>
        /// <param name="e">Not used</param>
        protected override void OnVisibleChanged(EventArgs e)
        {           
            if (!m_Created && !String.IsNullOrEmpty(ApplicationName)) // only allow one instance of the child
            {
                m_Created = true;
                m_AppWin = IntPtr.Zero;
                try
                {
                    if(!File.Exists(ApplicationName))
                    {
                        MessageBox.Show(ApplicationName + " not found in configured path, please go into tools->settings and set the correct path", "Application Not Found");
                        return;
                    }
                    m_Process = new Process
                    {
                        EnableRaisingEvents = true,
                        StartInfo =
                        {
                            FileName = ApplicationName,
                            Arguments = ApplicationParameters
                        }
                    };

                    if (!string.IsNullOrEmpty(this.ApplicationWorkingDirectory) &&
                        Directory.Exists(this.ApplicationWorkingDirectory))
                    {
                        m_Process.StartInfo.WorkingDirectory = this.ApplicationWorkingDirectory;
                    }

                    m_Process.Exited += delegate {
                        m_CloseCallback(true);
                    };

                    m_Process.Start();

                    m_Process = this.WaitForTargetProcess(m_Process);
                    if (this.IsWindowAppliesForInherit(m_Process.MainWindowHandle))
                    {
                        m_AppWin = m_Process.MainWindowHandle;

                        if (IntPtr.Zero == m_AppWin)
                        {
                            int poolInterval = this.GetMaxWindowPoolingTime();
                            Log.WarnFormat("Unable to get handle for process on first try.{0}", LoopWaitForHandle ? "  Polling " + poolInterval + " s for handle." : "");
                            if (LoopWaitForHandle)
                            {
                                DateTime startTime = DateTime.Now;
                                while ((DateTime.Now - startTime).TotalSeconds < poolInterval)
                                {
                                    System.Threading.Thread.Sleep(50);

                                    // Refresh Process object's view of real process
                                    m_Process.Refresh();
                                    if (!this.IsWindowAppliesForInherit(m_Process.MainWindowHandle))
                                        continue;
                                    m_AppWin = m_Process.MainWindowHandle;
                                    if (IntPtr.Zero != m_AppWin)
                                    {
                                        Log.Info("Successfully found handle via polling " + (DateTime.Now - startTime).TotalMilliseconds + " ms");
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        b_AppWinFinal = false;
                        this.CreateVirtWindow();
                        bgWinTracker.DoWork += new DoWorkEventHandler(bgWinTracker_DoWork);
                        bgWinTracker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWinTracker_Done);
                        bgWinTracker.RunWorkerAsync();
                    }
                }
                catch (InvalidOperationException ex)
                {
                    /* Possible Causes:
                     * No file name was specified in the Process component's StartInfo.
                     * -or-
                     * The ProcessStartInfo.UseShellExecute member of the StartInfo property is true while ProcessStartInfo.RedirectStandardInput, 
                     * ProcessStartInfo.RedirectStandardOutput, or ProcessStartInfo.RedirectStandardError is true. 
                     */
                    MessageBox.Show(this, ex.Message, "Invalid Operation Error");
                    throw;
                }
                catch (Win32Exception ex)
                {
                    /*
                     * Checks are elsewhere to ensure these don't occur, but incase they do we're gonna bail with a nasty exception
                     * which will hopefully send users kicking and screaming at me to fix this (And hopefully they will include a 
                     * stacktrace!)
                     */
                    if (ex.NativeErrorCode == NativeMethods.ERROR_ACCESS_DENIED)
                    {
                        throw;
                    }
                    else if (ex.NativeErrorCode == NativeMethods.ERROR_FILE_NOT_FOUND)
                    {
                        throw;
                    }
                }

                if (SuperPuTTY.PuTTYAppName + " Command Line Error" == this.m_Process.MainWindowTitle)
                {
                    // dont' try to capture or manipulate the window
                    Log.WarnFormat("Error while creating putty session: title={0}, handle={1}.  Abort capture window", this.m_Process.MainWindowTitle, this.m_AppWin);
                    MessageBox.Show("Could not start putty session: Arguments passed to commandline invalid.", "putty command line error.");
                    this.m_AppWin = IntPtr.Zero;
                }
                
                this.AttachToWindow();
            }

            if (this.Visible && this.m_Created && this.b_AppWinFinal && this.ExternalProcessCaptured)
            {
                // Move the child so it's located over the parent
                this.MoveWindow("OnVisChanged");
                
                if (RefocusOnVisChanged && NativeMethods.GetForegroundWindow() != this.m_AppWin)
                {
                    this.BeginInvoke(new MethodInvoker(delegate { this.ReFocusPuTTY("OnVisChanged"); }));
                }
            }

            base.OnVisibleChanged(e);
        }

        
        /// <summary>
        /// Send a close message to the hosted application window when the parent is destroyed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (this.ExternalProcessCaptured)
            {
                // Send WM_DESTROY instead of WM_CLOSE, so that the Client doesn't
                // ask in the Background whether the session shall be closed.
                // Otherwise an annoying beep is generated everytime a terminal session is closed.
                if (this.ApplicationCloseWithDestroy) {
                    NativeMethods.PostMessage(m_AppWin, NativeMethods.WM_DESTROY, 0, 0);
                } else {
                    NativeMethods.PostMessage(m_AppWin, NativeMethods.WM_CLOSE, 0, 0);
                }

                System.Threading.Thread.Sleep(ClosePuttyWaitTimeMs);

                m_AppWin = IntPtr.Zero;
                if (this.bgWinTracker.IsBusy == true)
                    this.bgWinTracker.CancelAsync();
            }

            base.OnHandleDestroyed(e);
        }

        /// <summary>
        /// Refresh the hosted applications window when the parent changes size
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            // if valid
            if (ExternalProcessCaptured)
            {
                // if not minimizing && visible
                if (this.Height > 0 && this.Width > 0 && this.Visible)
                {
                    this.MoveWindow("OnResize");
                }
            }
            base.OnResize(e);
        }

        public bool ExternalProcessCaptured { get { return this.m_AppWin != IntPtr.Zero; } }

        #endregion    
    
    }

}
