using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using NUnit.Gui;
using SuperPuttyUnitTests.Scp;
using System.Windows.Forms;
using log4net;
using System.Drawing;
using System.IO;
using System.Threading;

namespace SuperPuttyUnitTests
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        static bool initialized = false;

        public static void InitLoggingForUnitTests()
        {
            lock (Log)
            {
                if (!initialized)
                {
                    log4net.Config.BasicConfigurator.Configure();
                    initialized = true;
                }
            }
        }

        [STAThread]
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            Log.Info("Starting...");

            AppDomain.CurrentDomain.UnhandledException += (CurrentDomain_UnhandledException);
            Application.ThreadException += (Application_ThreadException);
            Application.EnableVisualStyles();
            Application.Run(new TestAppRunner());
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string msg = String.Format("Unhandled Domain Exception: isTerminating={0}, ex={1}", e.IsTerminating, e.ExceptionObject);
            Log.Error(msg);
            MessageBox.Show(msg, "CurrentDomain_UnhandledException");
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            string msg = String.Format("ThreadException : ex={0}", e.Exception);
            Log.Error(msg);
            MessageBox.Show(msg, "Application_ThreadException");
        }


        static void RunConsole()
        {
            string[] my_args = { Assembly.GetExecutingAssembly().Location };

            int returnCode = NUnit.ConsoleRunner.Runner.Main(my_args);

            if (returnCode != 0)
                Console.Beep();

            Console.WriteLine("Complete - Any key to kill");
            Console.ReadLine();
        }

    }


}
