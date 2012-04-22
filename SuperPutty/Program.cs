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
using System.Windows.Forms;
using System.Threading;
using log4net;
using System.Runtime.InteropServices;
using System.Diagnostics;
using SuperPutty.Data;
using System.Configuration;
using SuperPutty.Utils;

namespace SuperPutty
{
    static class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        private static bool EnforceSingleInstance = Convert.ToBoolean(
            ConfigurationManager.AppSettings["SuperPuTTY.SingleInstance"] ?? "False");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (EnforceSingleInstance)
            {
                bool onlyInstance = false;
                Mutex mutex = new Mutex(true, "SuperPutty", out onlyInstance);
                if (!onlyInstance)
                {
                    string strArgs = "";
                    if (args.Length > 0)
                    {
                        strArgs += args[0];
                        for (int i = 1; i < args.Length; i++)
                        {
                            strArgs += " " + args[i];
                        }
                    }

                    SuperPutty.Utils.NativeMethods.COPYDATA cd = new SuperPutty.Utils.NativeMethods.COPYDATA();
                    cd.dwData = 0;
                    cd.cbData = (uint) strArgs.Length + 1;

                    cd.lpData = Marshal.StringToHGlobalAnsi(strArgs);
                    IntPtr lpPtr = Marshal.AllocHGlobal(Marshal.SizeOf(cd));
                    Marshal.StructureToPtr(cd, lpPtr, true);

                    Process[] plist = Debugger.IsAttached
                        ? Process.GetProcessesByName("SuperPutty")
                        : Process.GetProcessesByName("SuperPutty.vshost");

                    foreach (Process spProcess in plist)
                    {
                        Console.WriteLine(
                            "Sending command to existing instance: pid={0}, exe={1}, args={2}, msg={3}",
                            spProcess.MainModule.FileName,
                            spProcess.Id, strArgs, 0x004A);
                        NativeMethods.SendMessage(spProcess.MainWindowHandle, 0x004A, 0, lpPtr);
                    }
                    Marshal.FreeHGlobal(lpPtr);
                    Environment.Exit(0);
                }
            }

#if DEBUG
            Logger.OnLog += delegate(string logMessage)
            {
                //Console.WriteLine(logMessage);
                Log.Info(logMessage);
            };
#endif

            // logging
            log4net.Config.XmlConfigurator.Configure();

            try
            {

                Log.Info("Starting");
                SuperPuTTY.Initialize(args);

                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(SuperPuTTY.MainForm = new frmSuperPutty());
                SuperPuTTY.Shutdown();
            }
            catch (Exception ex)
            {
                Log.Error("Error in Main", ex);
            }
            finally
            {
                Log.Info("Shutdown");
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            String msg = String.Format("CurrentDomain_UnhandledException: IsTerminating={0}, ex={1}", e.IsTerminating, e.ExceptionObject);
            MessageBox.Show(msg, "Unhandled Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Log.Error(msg);
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString(), "Application_ThreadException", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Log.Error("Application_ThreadException", e.Exception);
        }



    }
}
