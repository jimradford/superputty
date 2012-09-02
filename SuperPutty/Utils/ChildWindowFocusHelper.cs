using System;

using System.Drawing;
using System.Windows.Forms;
using log4net;
using System.Collections.Generic;
using WeifenLuo.WinFormsUI.Docking;

namespace SuperPutty.Utils
{
    public class ChildWindowFocusHelper : IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ChildWindowFocusHelper));

        private int m_shellHookNotify;
        private bool m_externalWindow = false;
        private DateTime m_lastMouseDownOnTitleBar = DateTime.Now;
        private TimeSpan m_delayUntilMouseMove = new TimeSpan(0, 0, 0, 0, 200); // 200ms
        private Point m_mouseDownLocation = new Point(0, 0);

        private IDictionary<IntPtr, ctlPuttyPanel> childWindows = new Dictionary<IntPtr, ctlPuttyPanel>();

        public ChildWindowFocusHelper(frmSuperPutty form)
        {
            this.MainForm = form;
            this.MainForm.ResizeEnd += HandleResizeEnd;

            foreach (IDockContent doc in this.MainForm.DockPanel.Contents)
            {
                ctlPuttyPanel pp = doc as ctlPuttyPanel;
                if (pp != null)
                {
                    this.childWindows.Add(pp.AppPanel.AppWindowHandle, pp);
                }
            }
            this.MainForm.DockPanel.ContentAdded += DockPanel_ContentAdded;
            this.MainForm.DockPanel.ContentRemoved += DockPanel_ContentRemoved;
        }

        void DockPanel_ContentAdded(object sender, DockContentEventArgs e)
        {
            ctlPuttyPanel pp = e.Content as ctlPuttyPanel;
            if (pp != null)
            {
                this.childWindows.Add(pp.AppPanel.AppWindowHandle, pp);
            }
        }

        void DockPanel_ContentRemoved(object sender, DockContentEventArgs e)
        {
            ctlPuttyPanel pp = e.Content as ctlPuttyPanel;
            if (pp != null)
            {
                this.childWindows.Remove(pp.AppPanel.AppWindowHandle);
            }
        }

        public void Start()
        {
            this.m_shellHookNotify = NativeMethods.RegisterWindowMessage("SHELLHOOK");
            NativeMethods.RegisterShellHookWindow(this.MainForm.Handle);
        }

        public void Dispose()
        {
            this.MainForm.ResizeEnd -= HandleResizeEnd;
            this.MainForm.DockPanel.ContentAdded -= DockPanel_ContentAdded;
            this.MainForm.DockPanel.ContentRemoved -= DockPanel_ContentRemoved;
        }

        private int GET_X_LPARAM(int lParam)
        {
            return (lParam & 0xffff);
        }

        private int GET_Y_LPARAM(int lParam)
        {
            return (lParam >> 16);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns>true if base winproc should be called</returns>
        public bool WndProcForFocus(ref Message m)
        {
            NativeMethods.WM wm = (NativeMethods.WM) m.Msg;
            
            /*if (wm != NativeMethods.WM.GETICON){
                Log.InfoFormat("WndProcForFocus: " + m);
            }*/
            switch (wm)
            {
                case NativeMethods.WM.NCLBUTTONDOWN:
                    // This is in conjunction with the WM_NCMOUSEMOVE. We cannot detect
                    // WM_NCLBUTTONUP because it gets swallowed up on many occasions. As a result
                    // we detect the button down and check the NCMOUSEMOVE to see if it has
                    // changed location. If the mouse location is different, then we let
                    // the resize handler deal with the focus. If not, then we assume that it
                    // is a mouseup action.
                    this.m_lastMouseDownOnTitleBar = DateTime.Now;
                    m_mouseDownLocation = new Point(GET_X_LPARAM((int)m.LParam), GET_Y_LPARAM((int)m.LParam));
                    break;
                case NativeMethods.WM.NCMOUSEMOVE:
                    Point currentLocation = new Point(GET_X_LPARAM((int)m.LParam), GET_Y_LPARAM((int)m.LParam));
                    if ((this.m_lastMouseDownOnTitleBar - DateTime.Now < this.m_delayUntilMouseMove)
                            && currentLocation == m_mouseDownLocation)
                    {
                        //this.MainForm.FocusActiveDocument("MouseMove");
                    }
                    break;
                case NativeMethods.WM.NCACTIVATE:
                    // Never allow this window to display itself as inactive
                    NativeMethods.DefWindowProc(this.MainForm.Handle, m.Msg, (IntPtr)1, m.LParam);
                    m.Result = (IntPtr)1;
                    return false;
                case NativeMethods.WM.WINDOWPOSCHANGED:
                    // after restore from clicking on task bar (non minimized, just not active)
                    //this.MainForm.FocusActiveDocument();
                    break;
                case NativeMethods.WM.SYSCOMMAND:
                    // Check for maximizing and restoring from maxed.
                    // Removing the last 4 bits. This is necessary because
                    // maximizing by double click gives you 0xF032, not 0xF030.
                    switch ((int)m.WParam & 0xFFF0)
                    {
                        case NativeMethods.SC_MAXIMIZE:
                        case NativeMethods.SC_RESTORE:
                            //Log.InfoFormat("SysCommand: {0}", m.WParam);
                            this.MainForm.BeginInvoke(new Action<string>(this.MainForm.FocusActiveDocument), "SYSCommand-Restore");
                            break;
                    }
                    break;
                default:

                    if (m.Msg == m_shellHookNotify)
                    {
                        switch (m.WParam.ToInt32())
                        {
                            case 4:
                                IntPtr current = NativeMethods.GetForegroundWindow();
                                if (current != this.MainForm.Handle && !this.ContainsChild(current))
                                {
                                    m_externalWindow = true;
                                }
                                else if (m_externalWindow)
                                {
                                    m_externalWindow = false;
                                    NativeMethods.BringWindowToTop(this.MainForm.Handle);
                                    this.MainForm.FocusActiveDocument("SHELLHOOK");
                                }
                                //else if (current == this.MainForm.Handle)
                                //{
                                //    //Log.Info("### hwd=" + this.MainForm.Handle + ", m.h=" + m.HWnd + ", blah=" + DesktopWindow.GetFirstDesktopWindow().Title);
                                //    foreach (DesktopWindow dw in DesktopWindow.GetDesktopWindows())
                                //    {
                                //        //Log.Info("blah=" + dw.Title);
                                //    }
                                //}
                                break;
                            default:
                                break;
                        }
                    }
                    break;
            }

            return true;
        }

        bool ContainsChild(IntPtr childHandle)
        {
            return this.childWindows.ContainsKey(childHandle);
        }

        /// <summary>
        /// Handles resizing of the super putty window AND moving the form from the title bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleResizeEnd(Object sender, EventArgs e)
        {
            this.MainForm.FocusActiveDocument("ResizeEnd");
        }

        frmSuperPutty MainForm { get; set; }
    }
}
