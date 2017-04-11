﻿/*
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
        private List<IntPtr> m_hWinEventHooks = new List<IntPtr>();
        private List<NativeMethods.WinEventDelegate> lpfnWinEventProcs = new List<NativeMethods.WinEventDelegate>();
        private WindowActivator m_windowActivator = null;

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

        public IntPtr AppWindowHandle;

        #endregion

        public ApplicationPanel()
        {
            this.Disposed += new EventHandler(ApplicationPanel_Disposed);
            SuperPuTTY.LayoutChanged += new EventHandler<Data.LayoutChangedEventArgs>(SuperPuTTY_LayoutChanged);

            // setup up the hook to watch for all EVENT_SYSTEM_FOREGROUND events system wide

            string typeName = string.IsNullOrEmpty(SuperPuTTY.Settings.WindowActivator) ? ActivatorTypeName : SuperPuTTY.Settings.WindowActivator;
            this.m_windowActivator = (WindowActivator)Activator.CreateInstance(Type.GetType(typeName));
            //this.m_windowActivator = new SetFGCombinedWindowActivator();
            SuperPuTTY.Settings.SettingsSaving += Settings_SettingsSaving;
            SuperPuTTY.WindowEvents.SystemSwitch += new EventHandler<GlobalWindowEventArgs>(OnSystemSwitch);
            this.ApplicationName = "";
            this.ApplicationParameters = "";
            this.ApplicationWorkingDirectory = "";
            this.AppWindowHandle = this.m_AppWin;
            this.ExternalProcessCaptured = (this.m_AppWin != IntPtr.Zero);
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
                // OG hack
                // Last line of terminal can be hidden due to the height being a fraction of the line height.
                // Need to make height a multiple of the line height. But don't have putty configuration font,
                // so I'm hacking this and assuming a line height of 17 px.
                int HeightMultipleOfFont = this.Height - this.Height % 17;

                bool success = NativeMethods.MoveWindow(m_AppWin, x, y, this.Width, HeightMultipleOfFont, this.Visible);
                if (Log.IsInfoEnabled)
                {
                    Log.InfoFormat("MoveWindow [{3,-15}{4,20}] w={0,4}, h={1,4}, visible={2}, success={5}", this.Width, this.Height, this.Visible, src, this.Name, success);
                }
            }
        }

        public bool ReFocusPuTTY(string caller)
        {
            bool result = false;
            if (this.ExternalProcessCaptured && NativeMethods.GetForegroundWindow() != this.m_AppWin)
            {
                //Log.InfoFormat("[{0}] ReFocusPuTTY - puttyTab={1}, caller={2}", this.m_AppWin, this.Parent.Text, caller);
                settingForeground = true;
                result = NativeMethods.SetForegroundWindow(this.m_AppWin);
                Log.InfoFormat("[{0}] ReFocusPuTTY - puttyTab={1}, caller={2}, result={3}", this.m_AppWin, this.Parent.Text, caller, result);
            }
            //return (this.m_AppWin != null
            //    && NativeMethods.GetForegroundWindow() != this.m_AppWin
            //    && !NativeMethods.SetForegroundWindow(this.m_AppWin));

            return result;
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
                        MessageBox.Show("putty.exe not found in configured path, please go into tools->settings and set the correct path", "Application Not Found");
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
                    //m_Process.Exited += new EventHandler(p_Exited);

                    if (!string.IsNullOrEmpty(this.ApplicationWorkingDirectory) &&
                        Directory.Exists(this.ApplicationWorkingDirectory))
                    {
                        m_Process.StartInfo.WorkingDirectory = this.ApplicationWorkingDirectory;
                    }

                    m_Process.Exited += delegate {
                        m_CloseCallback(true);
                    };

                    m_Process.Start();

                    // Wait for application to start and become idle
                    m_Process.WaitForInputIdle();
                    m_AppWin = m_Process.MainWindowHandle;

                    if (IntPtr.Zero == m_AppWin)
                    {
                        Log.WarnFormat("Unable to get handle for process on first try.{0}", LoopWaitForHandle ? "  Polling 10 s for handle." : "");
                        if (LoopWaitForHandle)
                        {
                            DateTime startTime = DateTime.Now;
                            while ((DateTime.Now - startTime).TotalSeconds < 10)
                            {
                                System.Threading.Thread.Sleep(50);

                                // Refresh Process object's view of real process
                                m_Process.Refresh();
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
                
                if(this.m_AppWin != IntPtr.Zero)
                {                    
                    // Set the application as a child of the parent form
                    NativeMethods.SetParent(m_AppWin, this.Handle);

                    // Show it! (must be done before we set the windows visibility parameters below                
                    NativeMethods.ShowWindow(m_AppWin, NativeMethods.WindowShowStyle.Maximize);

                    // set window parameters (how it's displayed)
                    long lStyle = NativeMethods.GetWindowLong(m_AppWin, NativeMethods.GWL_STYLE);
                    lStyle &= ~(NativeMethods.WS_BORDER | NativeMethods.WS_THICKFRAME);
                    NativeMethods.SetWindowLong(m_AppWin, NativeMethods.GWL_STYLE, lStyle);
                    NativeMethods.WinEventDelegate lpfnWinEventProc = new NativeMethods.WinEventDelegate(WinEventProc);
                    this.lpfnWinEventProcs.Add(lpfnWinEventProc);
                    uint eventType = (uint)NativeMethods.WinEvents.EVENT_OBJECT_NAMECHANGE;
                    this.m_hWinEventHooks.Add(NativeMethods.SetWinEventHook(eventType, eventType, IntPtr.Zero, lpfnWinEventProc, (uint)m_Process.Id, 0, NativeMethods.WINEVENT_OUTOFCONTEXT));
                    eventType = (uint)NativeMethods.WinEvents.EVENT_SYSTEM_FOREGROUND;
                    this.m_hWinEventHooks.Add(NativeMethods.SetWinEventHook(eventType, eventType, IntPtr.Zero, lpfnWinEventProc, (uint)m_Process.Id, 0, NativeMethods.WINEVENT_OUTOFCONTEXT));
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
            if (this.Visible && this.m_Created && this.ExternalProcessCaptured)
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
                NativeMethods.PostMessage(m_AppWin, NativeMethods.WM_DESTROY, 0, 0);

                System.Threading.Thread.Sleep(ClosePuttyWaitTimeMs);

                m_AppWin = IntPtr.Zero;
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

        public bool ExternalProcessCaptured;

        #endregion    
    
    }

}
