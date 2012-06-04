using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using log4net;

namespace SuperPutty.Utils
{
    #region WindowActivator
    public abstract class WindowActivator
    {
        protected ILog Log { get {  return LogManager.GetLogger(this.GetType()); } }

        public abstract void ActivateForm(Form form, DesktopWindow window, IntPtr hwnd);
    } 
	#endregion

    #region BringToFrontActivator
    public class BringToFrontActivator : WindowActivator 
    {
        public override void ActivateForm(Form form, DesktopWindow window, IntPtr hwnd)
        {
            if (window == null || window.Handle != form.Handle)
            {
                Log.InfoFormat("[{0}] Activating Main Window - current=({1})", hwnd, window != null ? window.Exe : "?");

                form.BringToFront();
                form.Focus();
                form.Show();
                form.Activate();

                // stop flashing...happens occassionally when switching quickly when activate manuver is fails
                NativeMethods.FlashWindow(form.Handle, NativeMethods.FLASHW_STOP);
            }
        }
    }
    #endregion

    #region TopMostActivator
    public class TopMostActivator : WindowActivator
    {
        public override void ActivateForm(Form form, DesktopWindow window, IntPtr hwnd)
        {
            if (window == null || window.Handle != form.Handle)
            {
                Log.InfoFormat("[{0}] Activating Main Window - current=({1})", hwnd, window != null ? window.Exe : "?");

                // bring to top
                form.TopMost = true;
                form.TopMost = false;

                // set as active form in task bar
                form.Activate();

                // stop flashing...happens occassionally when switching quickly when activate manuver is fails
                NativeMethods.FlashWindow(form.Handle, NativeMethods.FLASHW_STOP);
            }
        }
    }
    #endregion

    #region SetFGWindowActivator
    public class SetFGWindowActivator : WindowActivator
    {
        public override void ActivateForm(Form form, DesktopWindow window, IntPtr hwnd)
        {
            if (window == null || window.Handle != form.Handle)
            {
                Log.InfoFormat("[{0}] Activating Main Window - current=({1})", hwnd, window != null ? window.Exe : "?");


                // bring to top
                form.TopMost = true;
                form.TopMost = false;

                // set as active form in task bar
                NativeMethods.SetForegroundWindow(form.Handle);

                // stop flashing...happens occassionally when switching quickly when activate manuver is fails
                NativeMethods.FlashWindow(form.Handle, NativeMethods.FLASHW_STOP);
            }
        }
    }
    #endregion

    #region RestoreWindowActivator
    public class RestoreWindowActivator : WindowActivator
    {
        public override void ActivateForm(Form form, DesktopWindow window, IntPtr hwnd)
        {
            if (window == null || window.Handle != form.Handle)
            {
                Log.InfoFormat("[{0}] Activating Main Window - current=({1})", hwnd, window != null ? window.Exe : "?");

                // set as active form in task bar
                NativeMethods.ShowWindow(form.Handle, NativeMethods.WindowShowStyle.Restore);
                NativeMethods.SetForegroundWindow(form.Handle);
                Application.DoEvents();

                // stop flashing...happens occassionally when switching quickly when activate manuver is fails
                NativeMethods.FlashWindow(form.Handle, NativeMethods.FLASHW_STOP);
            }
        }
    }
    #endregion

    #region SetFGAttachThreadWindowActivator
    public class SetFGAttachThreadWindowActivator : WindowActivator
    {
        public override void ActivateForm(Form form, DesktopWindow window, IntPtr hwnd)
        {
            if (window == null || window.Handle != form.Handle)
            {
                Log.InfoFormat("[{0}] Activating Main Window - current=({1})", hwnd, window != null ? window.Exe : "?");

                uint fgProcessId;
                uint spProcessId;
                NativeMethods.GetWindowThreadProcessId(NativeMethods.GetForegroundWindow(), out fgProcessId);
                NativeMethods.GetWindowThreadProcessId(form.Handle, out spProcessId);

                if (fgProcessId != spProcessId)
                {
                    if (NativeMethods.AttachThreadInput(fgProcessId, spProcessId, true))
                    {
                        NativeMethods.SetForegroundWindow(form.Handle);
                        NativeMethods.AttachThreadInput(fgProcessId, spProcessId, false);
                    }
                }
                else
                {
                    NativeMethods.SetForegroundWindow(form.Handle);
                }

                // stop flashing...happens occassionally when switching quickly when activate manuver is fails
                NativeMethods.FlashWindow(form.Handle, NativeMethods.FLASHW_STOP);
            }
        }
    }
    #endregion

    #region CombinedWindowActivator

    /// <summary>
    /// Uses all the tricks in the book at once to do it...shady
    /// </summary>
    public class CombinedWindowActivator : WindowActivator
    {
        private const int SW_HIDE = 0;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_NORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_MAXIMIZE = 3;
        private const int SW_SHOWNOACTIVATE = 4;
        private const int SW_SHOW = 5;
        private const int SW_MINIMIZE = 6;
        private const int SW_SHOWMINNOACTIVE = 7;
        private const int SW_SHOWNA = 8;
        private const int SW_RESTORE = 9;
        private const int SW_SHOWDEFAULT = 10;
        private const int SW_MAX = 10;

        private const uint SPI_GETFOREGROUNDLOCKTIMEOUT = 0x2000;
        private const uint SPI_SETFOREGROUNDLOCKTIMEOUT = 0x2001;
        private const int SPIF_SENDCHANGE = 0x2;

        public override void ActivateForm(Form form, DesktopWindow window, IntPtr hwnd)
        {
            if (window == null || window.Handle != form.Handle)
            {
                Log.InfoFormat("[{0}] Activating Main Window - current=({1})", hwnd, window != null ? window.Exe : "?");

                IntPtr Dummy = IntPtr.Zero;

                IntPtr hWnd = form.Handle;
                if (NativeMethods.IsIconic(hWnd))
                {
                    NativeMethods.ShowWindowAsync(hWnd, SW_RESTORE);
                }
                else
                {
                    NativeMethods.ShowWindowAsync(hWnd, SW_SHOW);
                }
                NativeMethods.SetForegroundWindow(hWnd);

                // Code from Karl E. Peterson, www.mvps.org/vb/sample.htm
                // Converted to Delphi by Ray Lischner
                // Published in The Delphi Magazine 55, page 16
                // Converted to C# by Kevin Gale
                IntPtr foregroundWindow = NativeMethods.GetForegroundWindow();
                if (foregroundWindow != hWnd)
                {
                    uint foregroundThreadId = NativeMethods.GetWindowThreadProcessId(foregroundWindow, Dummy);
                    uint thisThreadId = NativeMethods.GetWindowThreadProcessId(hWnd, Dummy);

                    if (NativeMethods.AttachThreadInput(thisThreadId, foregroundThreadId, true))
                    {
                        NativeMethods.BringWindowToTop(hWnd); // IE 5.5 related hack
                        NativeMethods.SetForegroundWindow(hWnd);
                        NativeMethods.AttachThreadInput(thisThreadId, foregroundThreadId, false);
                    }
                }

                if (NativeMethods.GetForegroundWindow() != hWnd)
                {
                    // Code by Daniel P. Stasinski
                    // Converted to C# by Kevin Gale
                    IntPtr Timeout = IntPtr.Zero;
                    NativeMethods.SystemParametersInfo(SPI_GETFOREGROUNDLOCKTIMEOUT, 0, Timeout, 0);
                    NativeMethods.SystemParametersInfo(SPI_SETFOREGROUNDLOCKTIMEOUT, 0, Dummy, SPIF_SENDCHANGE);
                    NativeMethods.BringWindowToTop(hWnd); // IE 5.5 related hack
                    NativeMethods.SetForegroundWindow(hWnd);
                    NativeMethods.SystemParametersInfo(SPI_SETFOREGROUNDLOCKTIMEOUT, 0, Timeout, SPIF_SENDCHANGE);
                }


                NativeMethods.FlashWindow(form.Handle, NativeMethods.FLASHW_STOP);

            }


        }
    }
    #endregion


    #region ShowWindowWindowActivator

    public class ShowWindowWindowActivator : WindowActivator
    {
        private const int SW_HIDE = 0;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_NORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_MAXIMIZE = 3;
        private const int SW_SHOWNOACTIVATE = 4;
        private const int SW_SHOW = 5;
        private const int SW_MINIMIZE = 6;
        private const int SW_SHOWMINNOACTIVE = 7;
        private const int SW_SHOWNA = 8;
        private const int SW_RESTORE = 9;
        private const int SW_SHOWDEFAULT = 10;
        private const int SW_MAX = 10;

        public override void ActivateForm(Form form, DesktopWindow window, IntPtr hwnd)
        {
            if (window == null || window.Handle != form.Handle)
            {
                Log.InfoFormat("[{0}] Activating Main Window - current=({1})", hwnd, window != null ? window.Exe : "?");

                IntPtr Dummy = IntPtr.Zero;

                IntPtr hWnd = form.Handle;
                if (NativeMethods.IsIconic(hWnd))
                {
                    NativeMethods.ShowWindowAsync(hWnd, SW_RESTORE);
                }
                else
                {
                    NativeMethods.ShowWindowAsync(hWnd, SW_SHOW);
                }
                NativeMethods.SetForegroundWindow(hWnd);

                NativeMethods.FlashWindow(form.Handle, NativeMethods.FLASHW_STOP);
            }

        }
    }
    #endregion

    #region KeyEventWindowActivator

    /// <summary>
    /// Method posted by Breker on issue #1
    /// 
    /// Some of the pinvoke and const pulled from here
    /// http://social.msdn.microsoft.com/Forums/en/csharplanguage/thread/51ab8259-d9ab-4469-a07d-2ecbb30a8b23
    /// </summary>
    public class KeyEventWindowActivator : WindowActivator
    {
        private const byte VK_MENU = 0x12;
        private const byte VK_TAB = 0x09;
        private const int KEYEVENTF_EXTENDEDKEY = 0x01;
        private const int KEYEVENTF_KEYUP = 0x02;

        public override void ActivateForm(Form form, DesktopWindow window, IntPtr hwnd)
        {
            if (window == null || window.Handle != form.Handle)
            {
                Log.InfoFormat("[{0}] Activating Main Window - current=({1})", hwnd, window != null ? window.Exe : "?");

                // Send press of Alt key so that the main window can be activated w/o user interaction
                NativeMethods.keybd_event(VK_MENU, 0xb8, 0, 0);
                // Activate main window
                form.Activate();
                // Release Alt key
                NativeMethods.keybd_event(VK_MENU, 0xb8, KEYEVENTF_KEYUP, 0);

                // Set foreground back to terminal window
                //NativeMethods.SetForegroundWindow(hwnd);
            }

        }
    }
    #endregion

}
