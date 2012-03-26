using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using SuperPutty.Properties;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace SuperPutty
{
    /// <summary>
    /// Represents the SuperPuTTY application itself
    /// </summary>
    public static class SuperPuTTY 
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SuperPuTTY));

        public static event Action<LayoutChangedEventArgs> CurrentLayoutChanged;
        public static event Action<String> StatusEvent;

        static BindingList<LayoutData> layouts = new BindingList<LayoutData>();

        public static void Initialize()
        {
            Log.Info("Initializing...");

            SuperPuTTY.IsFirstRun = String.IsNullOrEmpty(Settings.PuttyExe);
            CommandLine = new CommandLineOptions();
            LoadLayouts();

            Log.Info("Initialized");
        }

        public static void Shutdown()
        {
            Log.Info("Shutting down...");
        }

        public static void ReportStatus(String status, params Object[] args)
        {
            String msg = (args.Length > 0) ? String.Format(status, args) : status;
            Log.DebugFormat("STATUS: {0}", msg);

            if (StatusEvent != null)
            {
                StatusEvent(msg);
            }
        }

        #region Layouts

        public static void AddLayout(String file)
        {
            LayoutData layout = new LayoutData(file);
            if (FindLayout(layout.Name) == null)
            {
                layouts.Add(layout);
                LoadLayout(layout, true);
            }
        }

        public static void RemoveLayout(String name)
        {
            LayoutData layout = FindLayout(name);
            if (layout != null)
            {
                layouts.Remove(layout);
            }
        }

        public static LayoutData FindLayout(String name)
        {
            LayoutData target = null;
            foreach (LayoutData layout in layouts)
            {
                if (name == layout.Name)
                {
                    target = layout;
                    break;
                }
            }
            return target;
        }
        public static void LoadLayouts()
        {
            if (!String.IsNullOrEmpty(Settings.SettingsFolder))
            {
                if (Directory.Exists(LayoutsDir))
                {
                    List<LayoutData> newLayouts = new List<LayoutData>();
                    foreach (String file in Directory.GetFiles(LayoutsDir))
                    {
                        newLayouts.Add(new LayoutData(file));
                    }

                    layouts.Clear();
                    foreach (LayoutData layout in newLayouts)
                    {
                        layouts.Add(layout);
                    }
                    Log.InfoFormat("Loaded {0} layouts", newLayouts.Count);
                }
                else
                {
                    Log.InfoFormat("Creating layouts dir: " + SuperPuTTY.LayoutsDir);
                    Directory.CreateDirectory(SuperPuTTY.LayoutsDir);
                }
            }

            // determine starting layout, if any.  CLI has priority
            if (CommandLine.Layout != null)
            {
                StartingLayout = FindLayout(CommandLine.Layout);
                if (StartingLayout != null)
                {
                    Log.InfoFormat("Starting with layout from command line, {0}", CommandLine.Layout);
                }
            }
            if (StartingLayout == null)
            {
                StartingLayout = FindLayout(Settings.DefaultLayoutName);
                if (StartingLayout != null)
                {
                    Log.InfoFormat("Starting with default layout, {0}", Settings.DefaultLayoutName);
                }
            }
            
        }

        public static void LoadLayout(LayoutData layout)
        {
            LoadLayout(layout, false);
        }

        public static void LoadLayout(LayoutData layout, bool isNewLayoutAlreadyActive)
        {
            if (CurrentLayoutChanged != null)
            {
                LayoutChangedEventArgs args = new LayoutChangedEventArgs { New = layout, Old = CurrentLayout };
                CurrentLayout = layout;
                CurrentLayoutChanged(args);
            }
        }

        public static void LoadLayoutInNewInstance(LayoutData layout)
        {
            ReportStatus("Starting new instance with layout, {0}", layout.Name);
            Process.Start(Assembly.GetExecutingAssembly().Location, "-layout \"" + layout + "\"");
        }


        #endregion

        #region Properties

        public static bool IsFirstRun { get; private set; }

        public static frmSuperPutty MainForm { get; set; }

        internal static Settings Settings { get { return Settings.Default; } }

        public static string LayoutsDir { get { return Path.Combine(Settings.SettingsFolder, "layouts"); } }


        public static LayoutData CurrentLayout { get; private set; }

        public static LayoutData StartingLayout { get; private set; }

        public static BindingList<LayoutData> Layouts { get { return layouts; } }

        public static CommandLineOptions CommandLine { get; private set; }

        #endregion
    }

    public class CommandLineOptions
    {
        public CommandLineOptions()
        {
            Queue<string> queue = new Queue<string>(Environment.GetCommandLineArgs());
            this.ExePath = queue.Dequeue(); 

            while (queue.Count > 0)
            {
                String arg = queue.Dequeue();
                switch (arg)
                {
                    case "-layout":
                        if (queue.Count > 0)
                        {
                            this.Layout = queue.Dequeue();
                        }
                        break;
                }
            }
        }

        public string ExePath { get; private set; }
        public string Layout { get; private set; }
    }
}
