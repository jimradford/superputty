using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using log4net;
using System.Diagnostics;
using System.ComponentModel;

namespace SuperPutty.Utils
{

    /// <summary>
    /// Based on off article below, modified to return more info in nice managed view
    /// EnumDesktopWindows Demo - shows the caption of all desktop windows.
    /// Authors: Svetlin Nakov, Martin Kulov
    /// Bulgarian Association of Software Developers - http://www.devbg.org/en/
    /// </summary>
    public class DesktopWindow
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(DesktopWindow));

        public IntPtr Handle { get; set; }
        public string Title { get; set; }
        public int ProcessId { get; set; }
        public string Exe { get; set; }

        /// <summary>
        /// entry point of the program
        /// </summary>
        public static List<DesktopWindow> GetDesktopWindows()
        {
            var windows = new List<DesktopWindow>();
            NativeMethods.EnumDelegate filter = delegate(IntPtr hWnd, int lParam)
            {
                StringBuilder strbTitle = new StringBuilder(255);
                int nLength = NativeMethods.GetWindowText(hWnd, strbTitle, strbTitle.Capacity + 1);
                string strTitle = strbTitle.ToString();

                if (NativeMethods.IsWindowVisible(hWnd) && string.IsNullOrEmpty(strTitle) == false)
                {
                    uint pid;
                    NativeMethods.GetWindowThreadProcessId(hWnd, out pid);

                    Process process = Process.GetProcessById(Convert.ToInt32(pid));
                    windows.Add(
                        new DesktopWindow
                        {
                            Handle = hWnd, 
                            Title = strTitle, 
                            ProcessId = Convert.ToInt32(pid),
                            Exe = GetProcessExe(process)
                        });
                }
                return true;
            };

            if (!NativeMethods.EnumDesktopWindows(IntPtr.Zero, filter, IntPtr.Zero))
            {
                Log.Warn("Unable to enum desktop windows");
            }

            return windows;
        }

        static string GetProcessExe(Process process)
        {
            string exe = "?";
            try
            {
                if (process != null)
                {
                    exe = process.MainModule.FileName;
                }
            }
            catch (Win32Exception ex)
            {
                Log.ErrorFormat("Could not get exe.  error={0}", ex.Message);
            }
            return exe;
        }

        public static DesktopWindow GetFirstDesktopWindow() 
        {
            List<DesktopWindow> windows = GetDesktopWindows();
            return windows.FirstOrDefault();
        }

    }
}
