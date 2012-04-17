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
using SuperPutty.Data;

namespace SuperPutty
{
    /// <summary>
    /// Represents the SuperPuTTY application itself
    /// </summary>
    public static class SuperPuTTY 
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SuperPuTTY));

        public static event EventHandler<LayoutChangedEventArgs> LayoutChanging;
        public static event EventHandler<LayoutChangedEventArgs> LayoutChanged;

        public static event Action<String> StatusEvent;

        static BindingList<LayoutData> layouts = new BindingList<LayoutData>();
        static SortedList<string, SessionData> sessions = new SortedList<string, SessionData>();

        public static void Initialize()
        {
            Log.Info("Initializing...");

            SuperPuTTY.IsFirstRun = String.IsNullOrEmpty(Settings.PuttyExe);
            CommandLine = new CommandLineOptions();
            LoadLayouts();
            LoadSessions();

            Log.Info("Initialized");
        }

        public static void Shutdown()
        {
            Log.Info("Shutting down...");
            //SaveSessions();
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

        public static bool IsLayoutChanging { get; private set; }

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
            Log.InfoFormat("LoadLayout: layout={0}, isNewLayoutAlreadyActive={1}", layout == null ? "NULL" : layout.Name, isNewLayoutAlreadyActive);
            LayoutChangedEventArgs args = new LayoutChangedEventArgs
            {
                New = layout,
                Old = CurrentLayout,
                IsNewLayoutAlreadyActive = isNewLayoutAlreadyActive
            };

            try
            {
                IsLayoutChanging = true;

                if (LayoutChanging != null)
                {
                    LayoutChanging(typeof(SuperPuTTY), args);
                }

            }
            finally
            {
                IsLayoutChanging = false;
            }


            if (LayoutChanged != null)
            {
                CurrentLayout = layout;
                LayoutChanged(typeof(SuperPuTTY), args);
            }
        }

        public static void LoadLayoutInNewInstance(LayoutData layout)
        {
            ReportStatus("Starting new instance with layout, {0}", layout.Name);
            Process.Start(Assembly.GetExecutingAssembly().Location, "-layout \"" + layout + "\"");
        }


        #endregion

        #region Sessions

        public static string SessionsFileName
        {
            get
            {
                return Path.Combine(Settings.SettingsFolder, "Sessions.XML");
            }
        }
        public static void LoadSessions()
        {
            string fileName = SessionsFileName;
            Log.InfoFormat("Loading all sessions.  file={0}", fileName);

            List<SessionData> sessions;
            if (!File.Exists(fileName))
            {
                Log.InfoFormat("Sessions file does not exist.  Attempting import from registry");
                sessions = SessionData.LoadSessionsFromRegistry();

                if (sessions != null)
                {
                    // create default
                    Log.InfoFormat("Imported {0} sessions.  Saving default file.", sessions.Count);
                    SessionData.SaveSessionsToFile(sessions, fileName);
                }
            }
            else
            {
                sessions = SessionData.LoadSessionsFromFile(fileName);
            }

            foreach (SessionData session in sessions)
            {
                AddSession(session);
            }
        }

        public static void SaveSessions()
        {
            Log.InfoFormat("Saving all sessions");
            SessionData.SaveSessionsToFile(GetAllSessions(), SessionsFileName);
        }

        public static SessionData RemoveSession(string sessionId)
        {
            SessionData session = GetSessionById(sessionId);
            sessions.Remove(sessionId);
            return session;
        }

        public static SessionData GetSessionById(string sessionId)
        {
            SessionData session;
            sessions.TryGetValue(sessionId, out session);
            return session;
        }

        public static bool AddSession(SessionData session)
        {
            bool success = false;
            if (GetSessionById(session.SessionId) == null)
            {
                Log.InfoFormat("Added Session, id={0}", session.SessionId);
                sessions.Add(session.SessionId, session);
                success = true;
            }
            else
            {
                Log.InfoFormat("Failed to Add Session, id={0}.  Session already exists", session.SessionId);
            }
            return success;
        }

        public static List<SessionData> GetAllSessions()
        {
            return sessions.Values.ToList();
        }

        public static void OpenSession(string sessionId)
        {
            Log.InfoFormat("Opening session, id={0}", sessionId);
            SessionData session = GetSessionById(sessionId);
            if (session != null)
            {
            }
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
