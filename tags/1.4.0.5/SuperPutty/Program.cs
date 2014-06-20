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
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Text;

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
            // send log to console
            log4net.Config.BasicConfigurator.Configure();

            bool onlyInstance = false;
            Mutex mutex = new Mutex(true, "SuperPutty", out onlyInstance);

            Log.InfoFormat(
                "IsFirstRun={0}, SingleInstanceMode={1}, onlyInstance={2}", 
                SuperPuTTY.IsFirstRun, SuperPuTTY.Settings.SingleInstanceMode, onlyInstance);
            if ((EnforceSingleInstance  || SuperPuTTY.Settings.SingleInstanceMode) && !SuperPuTTY.IsFirstRun && !onlyInstance)
            {
                SingleInstanceHelper.LaunchInExistingInstance(args);
                Log.InfoFormat("Sent Command to Existing Instance: [{0}]", String.Join(" ", args));
                Environment.Exit(0);
            }

            // open full file
            log4net.Config.XmlConfigurator.Configure();

#if DEBUG
            Logger.OnLog += delegate(string logMessage)
            {
                //Console.WriteLine(logMessage);
                Log.Info(logMessage);
            };
#endif

            try
            {

                Log.Info("Starting");
                SuperPuTTY.Initialize(args);

                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                frmSuperPutty mainForm = new frmSuperPutty();
                SetDefaultIcon(mainForm);
                Application.Run(SuperPuTTY.MainForm = mainForm);
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

        static void SetDefaultIcon(Form form)
        {
            // pure evil
            try
            {
                string iconFile = Path.Combine(Application.StartupPath, "SuperPutty.ico");
                if (File.Exists(iconFile))
                {
                    form.Icon = new Icon(iconFile);
                }

                FieldInfo field = typeof(Form).GetField("defaultIcon", BindingFlags.NonPublic | BindingFlags.Static);
                field.SetValue(null, form.Icon);
            }
            catch (Exception ex)
            {
                Log.WarnFormat("Could not set default icon: error={0}", ex.Message);
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            String msg = String.Format("CurrentDomain_UnhandledException: IsTerminating={0}, ex={1}", e.IsTerminating, e.ExceptionObject);
            MessageBox.Show(msg, "Unhandled Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Log.Error(msg);
        }

        /// <summary>
        /// For issue 218:
        /// http://stackoverflow.com/questions/295161/how-to-avoid-filenotfoundexception-if-net-3-5-is-not-installed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            if (e.Exception.Message.Contains("Could not load file or assembly 'System.Core, Version=3.5.0.0"))
            {
                sb.Append("SuperPutty requires the Microsoft .NET Framework version 3.5, or greater, in order to run.\n\nPlease contact your System Administrator for more information.");
            }
            else
            {
                sb.Append(e.Exception);
            }

            Log.Error("Application_ThreadException", e.Exception);
            MessageBox.Show(sb.ToString(), "Application_ThreadException", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }



    }
}
