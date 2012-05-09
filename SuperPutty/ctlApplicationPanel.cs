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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using WeifenLuo.WinFormsUI.Docking;
using System.IO;
using log4net;
using System.Configuration;
using SuperPutty.Utils;

namespace SuperPutty
{
    public delegate void PuttyClosedCallback(bool error);

    public class ApplicationPanel : System.Windows.Forms.Panel
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ApplicationPanel));

        private static bool RefocusOnVisChanged = Convert.ToBoolean(ConfigurationManager.AppSettings["SuperPuTTY.RefocusOnVisChanged"] ?? "False");
        private static bool LoopWaitForHandle = Convert.ToBoolean(ConfigurationManager.AppSettings["SuperPuTTY.LoopWaitForHandle"] ?? "False");
        private static int ClosePuttyWaitTimeMs = Convert.ToInt32(ConfigurationManager.AppSettings["SuperPuTTY.ClosePuttyWaitTimeMs"] ?? "100");

        // Win32 Exceptions which might occur trying to start the process
        const int ERROR_FILE_NOT_FOUND = 2;
        const int ERROR_ACCESS_DENIED = 5;

        #region Private Member Variables
        private Process m_Process;
        private bool m_Created = false;
        private IntPtr m_AppWin;
        private string m_ApplicationName = "";
        private string m_ApplicationParameters = "";
        private string m_ApplicationWorkingDirectory = "";

        internal PuttyClosedCallback m_CloseCallback;

        /// <summary>Set the name of the application executable to launch</summary>
        [Category("Data"), Description("The path/file to launch"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ApplicationName
        {
            get { return m_ApplicationName; }
            set { m_ApplicationName = value; }
        }
        
        [Category("Data"), Description("The parameters to pass to the application being launched"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ApplicationParameters
        {
            get { return m_ApplicationParameters; }
            set { m_ApplicationParameters = value; }
        }
        [Category("Data"), Description("The starting directory for the putty shell.  Relevant only to cygterm sessions"),
DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ApplicationWorkingDirectory
        {
            get { return m_ApplicationWorkingDirectory; }
            set { m_ApplicationWorkingDirectory = value; }
        }

        public IntPtr AppWindowHandle { get { return this.m_AppWin; } }

        #endregion

        public ApplicationPanel()
        {
            this.Disposed += new EventHandler(ApplicationPanel_Disposed);
            SuperPuTTY.LayoutChanged += new EventHandler<Data.LayoutChangedEventArgs>(SuperPuTTY_LayoutChanged);
        }

        void ApplicationPanel_Disposed(object sender, EventArgs e)
        {
            this.Disposed -= new EventHandler(ApplicationPanel_Disposed);
            SuperPuTTY.LayoutChanged -= new EventHandler<Data.LayoutChangedEventArgs>(SuperPuTTY_LayoutChanged);
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
             
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
       
        public bool ReFocusPuTTY()
        {
            Log.InfoFormat("ReFocusPuTTY - {0}", this.m_AppWin);
            return (this.m_AppWin != null 
                && GetForegroundWindow() != this.m_AppWin 
                && !SetForegroundWindow(this.m_AppWin));
        }

        /// <summary>
        /// Create (start) the hosted application when the parent becomes visible
        /// </summary>
        /// <param name="e">Not used</param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            //Log.Debug("OnVisibleChanged");
            if (!m_Created && !String.IsNullOrEmpty(ApplicationName)) // only allow one instance of the child
            {
                m_Created = true;
                m_AppWin = IntPtr.Zero;

                try
                {
                    m_Process = new Process();
                    m_Process.EnableRaisingEvents = true;
                    //m_Process.Exited += new EventHandler(p_Exited);
                    m_Process.StartInfo.FileName = ApplicationName;
                    m_Process.StartInfo.Arguments = ApplicationParameters;

                    if (!string.IsNullOrEmpty(this.ApplicationWorkingDirectory) &&
                        Directory.Exists(this.ApplicationWorkingDirectory))
                    {
                        m_Process.StartInfo.WorkingDirectory = this.ApplicationWorkingDirectory;
                    }

                    m_Process.Exited += delegate(object sender, EventArgs ev)
                    {
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
                    if (ex.NativeErrorCode == ERROR_ACCESS_DENIED)
                    {
                        throw;
                    }
                    else if (ex.NativeErrorCode == ERROR_FILE_NOT_FOUND)
                    {
                        throw;
                    }
                }

                //Logger.Log("ApplicationPanel Handle: {0}", this.Handle.ToString("X"));              
                //Logger.Log("Process Handle: {0}", m_AppWin.ToString("X"));
                // Set the application as a child of the parent form
                NativeMethods.SetParent(m_AppWin, this.Handle);

                // Show it! (must be done before we set the windows visibility parameters below                
                NativeMethods.ShowWindow(m_AppWin, NativeMethods.WindowShowStyle.Maximize);

                // set window parameters (how it's displayed)
                long lStyle = NativeMethods.GetWindowLong(m_AppWin, NativeMethods.GWL_STYLE);
                lStyle &= ~(NativeMethods.WS_BORDER | NativeMethods.WS_THICKFRAME);
                NativeMethods.SetWindowLong(m_AppWin, NativeMethods.GWL_STYLE, lStyle);
            }
            if (this.Visible && this.m_Created)
            {
                // Move the child so it's located over the parent
                this.MoveWindow("OnVisChanged");
                //MoveWindow(m_AppWin, 0, 0, this.Width, this.Height, true);
                if (RefocusOnVisChanged && GetForegroundWindow() != this.m_AppWin)
                {
                    this.BeginInvoke(new MethodInvoker(delegate { this.ReFocusPuTTY(); }));
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
            if (m_AppWin != IntPtr.Zero)
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
            if (this.m_AppWin != IntPtr.Zero)
            {
                // if not minimizing && visible
                if (this.Height > 0 && this.Width > 0 && this.Visible)
                {
                    MoveWindow("OnResize");
                }
            }
            base.OnResize(e);
        }

        private void MoveWindow(string src)
        {

            if (!SuperPuTTY.IsLayoutChanging)
            {
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("MoveWindow [{3,-15}{4,20}] w={0,4}, h={1,4}, visible={2}", this.Width, this.Height, this.Visible, src, this.Name);
                }

                NativeMethods.MoveWindow(m_AppWin, 0, 0, this.Width, this.Height, this.Visible);
            }
        }

        #endregion        
    }

}
