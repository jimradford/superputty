/*
 * Copyright (c) 2009 - 2015 Jim Radford http://www.jimradford.com
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
using Microsoft.Win32;
using WeifenLuo.WinFormsUI.Docking;
using log4net;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Drawing.Design;
using System.Drawing;
using SuperPutty.Utils;
using System.Web;

namespace SuperPutty.Data
{
    public enum ConnectionProtocol
    {
        SSH,
        SSH2,
        Telnet,
        Rlogin,
        Raw,
        Serial,
        Cygterm,
        Mintty,
        VNC,
        RDP,
        WINCMD,
        PS
    }

    /// <summary>The main class containing configuration settings for a session</summary>
    public class SessionData : IComparable, ICloneable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SessionData));
        public delegate void OnPropertyChangedHandler(SessionData Session, String AttributeName);
        public delegate void OnPropertyChangingHandler(SessionData Session, String AttributeName, Object NewValue, ref bool CancelChange);

        [XmlIgnore]
        public OnPropertyChangedHandler OnPropertyChanged;
        [XmlIgnore]
        public OnPropertyChangingHandler OnPropertyChanging;

        /// <summary>Full session id (includes path for session tree)e.g. FolderName/SessionName</summary>
        private string _SessionId;
        [XmlAttribute]
        [Browsable(false)]
        public string SessionId
        {
            get { return this._SessionId; }
            set
            {
                if (_SessionId != value)
                {
                    this.OldSessionId = SessionId;

                    UpdateField(ref _SessionId, value, "SessionId");
                }
            }
        }
        internal string OldSessionId { get; set; }

        private string _OldName;
        [XmlIgnore]
        [Browsable(false)]
        public string OldName
        {
            get { return _OldName; }
            set 
            {
                UpdateField(ref _OldName, value, "OldName");
            }
        }

        private string _SessionName;
        [XmlAttribute]
        [DisplayName("Session Name")]
        [Description("This is the name of the session.")]
        public string SessionName
        {
            get { return _SessionName; }
            set 
            { 
                if (_SessionName != value)
                {
                    OldName = _SessionName;
                    UpdateField(ref _SessionName, value, "SessionName");

                    if (SessionId == null)
                    {
                        SessionId = value;
                    }
                }
            }
        }

        private string _ImageKey;
        /// <summary>A string containing the key of the image associated with this session</summary>
        [XmlAttribute]
        [DisplayName("Image")]
        [Description("This is the image associated to the session.")]
        [Editor(typeof(ImageKeyEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(ImageKeyConverter))]
        public string ImageKey
        {
            get { return _ImageKey; }
            set 
            {
                UpdateField(ref _ImageKey, value, "ImageKey");
            }
        }

        private string _Host;
        [XmlAttribute]
        [DisplayName("Host Name/Ip")]
        [Description("This is the host name or the IP address of the destination.")]
        public string Host
        {
            get { return _Host; }
            set 
            {
                UpdateField(ref _Host, value, "Host");
            }
        }

        private int _Port;
        [XmlAttribute]
        [DisplayName("Port")]
        [Description("This is the port that will be used to connect to the destination.")]
        public int Port
        {
            get { return _Port; }
            set 
            {
                UpdateField(ref _Port, value, "Port");
            }
        }

        private ConnectionProtocol _Proto;
        [XmlAttribute]
        [DisplayName("Connection Type")]
        [Description("This is the login protocol.")]
        public ConnectionProtocol Proto
        {
            get { return _Proto; }
            set 
            {
                UpdateField(ref _Proto, value, "Proto");
            }
        }

        private string _PuttySession;
        [XmlAttribute]
        [TypeConverter(typeof(PuttySessionConverter))]
        [DisplayName("Putty Profile")]
        [Description("This is the PuTTY session profile associated to this session.")]
        public string PuttySession
        {
            get { return _PuttySession; }
            set 
            {
                UpdateField(ref _PuttySession, value, "PuttySession");
            }
        }

        private string _Username;
        [XmlAttribute]
        [DisplayName("Username")]
        [Description("This is the username that will be used to login.")]
        public string Username
        {
            get { return _Username; }
            set 
            {
                UpdateField(ref _Username, value, "Username");
            }
        }

        private string _Password;
        [XmlIgnore]
        [Browsable(false)]
        public string Password
        {
            get {
                
                 if (String.IsNullOrEmpty(_Password)){
                    // search if ExtraArgs contains the password
                    UpdateField(ref _Password, CommandLineOptions.getcommand(this.ExtraArgs, "-pw"), "Password");
                }
                return _Password;
            }
            set 
            {
                UpdateField(ref _Password, value, "Password");
            }
        }

        private string _ExtraArgs;
        [XmlAttribute]
        [DisplayName("Extra Arguments")]
        [Description("Extra PuTTY arguments.")]
        public string ExtraArgs
        {
            get { return _ExtraArgs; }
            set 
            {
                UpdateField(ref _ExtraArgs, value, "ExtraArgs");
            }
        }

        private DockState m_LastDockstate = DockState.Document;
        [XmlIgnore]
        [Browsable(false)]
        public DockState LastDockstate
        {
            get { return m_LastDockstate; }
            set 
            {
                UpdateField(ref m_LastDockstate, value, "LastDockstate");
            }
        }

        private bool m_AutoStartSession = false;
        [XmlIgnore]
        [Browsable(false)]
        public bool AutoStartSession
        {
            get { return m_AutoStartSession; }
            set 
            {
                UpdateField(ref m_AutoStartSession, value, "AutoStartSession");
            }
        }

        private string m_SPSLFileName = string.Empty;
        [XmlAttribute]
        [DisplayName("SPSL File")]
        [Description("SPSL Script Filename")]
        public string SPSLFileName
        {
            get { return m_SPSLFileName; }
            set
            {
                UpdateField(ref m_SPSLFileName, value, "SPSLFileName");
            }
        }


        [XmlAttribute]
        [DisplayName("Remote Path")]
        [Description("Remote Path used in file transfer")]
        public string RemotePath { get; set; }

        [XmlAttribute]
        [DisplayName("Local Path")]
        [Description("Local path used in file transfer")]
        public string LocalPath { get; set; }


        /// <summary>Construct a new session data object</summary>
        /// <param name="sessionName">A string representing the name of the session</param>
        /// <param name="hostName">The hostname or ip address of the remote host</param>
        /// <param name="port">The port on the remote host</param>
        /// <param name="protocol">The protocol to use when connecting to the remote host</param>
        /// <param name="sessionConfig">the name of the saved configuration settings from putty to use</param>
        public SessionData(string sessionName, string hostName, int port, ConnectionProtocol protocol, string sessionConfig)
        {
            SessionName = sessionName;
            Host = hostName;
            Port = port;
            Proto = protocol;
            PuttySession = sessionConfig;
        }
        
        /// <summary>Default constructor, instantiate a new <seealso cref="SessionData"/> object</summary>
        public SessionData()
        {

        }

        private void UpdateField<T>(ref T Field, T NewValue, string PropertyName)
        {
            if (!EqualityComparer<T>.Default.Equals(Field, NewValue))
            {
                Object NewValueObj = NewValue;
                bool CancelChange = false;
                if (OnPropertyChanging != null)
                    OnPropertyChanging(this, PropertyName, NewValueObj, ref CancelChange);
                if (!CancelChange)
                {
                    Field = NewValue;
                    if (OnPropertyChanged != null)
                        OnPropertyChanged(this, PropertyName);
                }
            }
        }

        /// <summary>Read any existing saved sessions from the registry, decode and populate a list containing the data</summary>
        /// <returns>A list containing the configuration entries retrieved from the registry</returns>
        public static List<SessionData> LoadSessionsFromRegistry()
        {
            Log.Info("LoadSessionsFromRegistry...");
            List<SessionData> sessionList = new List<SessionData>();
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Jim Radford\SuperPuTTY\Sessions");
            if (key != null)
            {
                string[] sessionKeys = key.GetSubKeyNames();
                foreach (string session in sessionKeys)
                {
                    SessionData sessionData = new SessionData();
                    RegistryKey itemKey = key.OpenSubKey(session);
                    if (itemKey != null)
                    {
                        sessionData.Host = (string)itemKey.GetValue("Host", "");
                        sessionData.Port = (int)itemKey.GetValue("Port", 22);
                        sessionData.Proto = (ConnectionProtocol)Enum.Parse(typeof(ConnectionProtocol), (string)itemKey.GetValue("Proto", "SSH"));
                        sessionData.PuttySession = (string)itemKey.GetValue("PuttySession", "Default Session");
                        sessionData.SessionName = session;
                        sessionData.SessionId = (string)itemKey.GetValue("SessionId", session);
                        sessionData.Username = (string)itemKey.GetValue("Login", "");
                        sessionData.LastDockstate = (DockState)itemKey.GetValue("Last Dock", DockState.Document);
                        sessionData.AutoStartSession = bool.Parse((string)itemKey.GetValue("Auto Start", "False"));
                        sessionData.RemotePath = (string)itemKey.GetValue("RemotePath", "");
                        sessionData.LocalPath = (string)itemKey.GetValue("LocalPath", "");
                        sessionList.Add(sessionData);
                    }
                }
            }
            return sessionList;
        }

        /// <summary>Load session configuration data from the specified XML file</summary>
        /// <param name="fileName">The filename containing the settings</param>
        public static List<SessionData> LoadSessionsFromFile(string fileName)
        {
            List<SessionData> sessions = new List<SessionData>();
            if (File.Exists(fileName))
            {
                WorkaroundCygwinBug();

                XmlSerializer s = new XmlSerializer(sessions.GetType());
                using (TextReader r = new StreamReader(fileName))
                {
                    sessions = (List<SessionData>)s.Deserialize(r);
                }
                Log.InfoFormat("Loaded {0} sessions from {1}", sessions.Count, fileName);
            }
            else
            {
                Log.WarnFormat("Could not load sessions, file doesn't exist.  file={0}", fileName);
            }
            return sessions;
        }

        /// <summary>Load session configuration data from files located in a specified folder</summary>
        /// <param name="folderName">The folder containing the settings files</param>
        public static List<SessionData> LoadSessionsFromFolder(string folderName)
        {
            List<SessionData> sessions = new List<SessionData>();
            if (Directory.Exists(folderName))
            {
				var sessionFiles = Directory.EnumerateFiles(folderName, "*");
                foreach (string sessionFile in sessionFiles)
                {
                    string sessionName = HttpUtility.UrlDecode(Path.GetFileName(sessionFile));
                    SessionData sessionData = new SessionData();
                    Hashtable sessionInfoKv = new Hashtable();
                    foreach (string cfgLine in File.ReadLines(sessionFile))
                    {
                        char[] sep = new char[] {'\\'};
                        string[] cfgData = cfgLine.Split(sep, 2);
                        if (cfgData.Length == 2)
                            sessionInfoKv[cfgData[0]] = cfgData[1].TrimEnd(sep);
                    }

                    try
                    {
                        sessionData.Host = sessionInfoKv.ContainsKey("HostName") ? (string)sessionInfoKv["HostName"] : "";
                        sessionData.Port = sessionInfoKv.ContainsKey("PortNumber") ? Int32.Parse((string)sessionInfoKv["PortNumber"]) : 22;
                        sessionData.Proto = (ConnectionProtocol)Enum.Parse(typeof(ConnectionProtocol), sessionInfoKv.ContainsKey("Protocol") ? ((string)sessionInfoKv["Protocol"]).ToUpper() : "SSH");
                        sessionData.PuttySession = sessionName;
                        sessionData.SessionName = sessionName;
                        sessionData.SessionId = sessionName;
                        sessionData.Username = sessionInfoKv.ContainsKey("UserName") ? (string)sessionInfoKv["UserName"] : "";
                        sessionData.LastDockstate = DockState.Document;
                        sessionData.AutoStartSession = false;
                        sessionData.RemotePath = "";
                        sessionData.LocalPath = "";
                        sessions.Add(sessionData);
                    }
                    catch (Exception exp)
                    {
                        Log.WarnFormat("Could not load session {0}", sessionName);
                    }
                }
                Log.InfoFormat("Loaded {0} sessions from {1}", sessions.Count, folderName);
            }
            else
            {
                Log.WarnFormat("Could not load sessions, folder doesn't exist.  folder={0}", folderName);
            }
            return sessions;
        }

        static void WorkaroundCygwinBug()
        {
            try
            {
                // work around known bug with cygwin
                Dictionary<string, string> envVars = new Dictionary<string, string>();
                foreach (DictionaryEntry de in Environment.GetEnvironmentVariables())
                {
                    string envVar = (string) de.Key;
                    if (envVars.ContainsKey(envVar.ToUpper()))
                    {
                        // duplicate found... (e.g. TMP and tmp)
                        Log.DebugFormat("Clearing duplicate envVariable, {0}", envVar);
                        Environment.SetEnvironmentVariable(envVar, null);
                        continue;
                    }
                    envVars.Add(envVar.ToUpper(), envVar);
                }

            }
            catch (Exception ex)
            {
                Log.WarnFormat("Error working around cygwin issue: {0}", ex.Message);
            }
        }

        /// <summary>Save session configuration to the specified XML file</summary>
        /// <param name="sessions">A List containing the session configuration data</param>
        /// <param name="fileName">A path to a filename to save the data in</param>
        public static void SaveSessionsToFile(List<SessionData> sessions, string fileName)
        {
            Log.InfoFormat("Saving {0} sessions to {1}", sessions.Count, fileName);

            BackUpFiles(fileName, 20);

            // sort and save file
            sessions.Sort();
            XmlSerializer s = new XmlSerializer(sessions.GetType());
            using (TextWriter w = new StreamWriter(fileName))
            {
                s.Serialize(w, sessions);
            }
        }

        private static void BackUpFiles(string fileName, int count)
        {
            if (File.Exists(fileName) && count > 0)
            {
                try
                {
                    // backup
                    string fileBaseName = Path.GetFileNameWithoutExtension(fileName);
                    string dirName = Path.GetDirectoryName(fileName);
                    string backupName = Path.Combine(dirName, string.Format("{0}.{1:yyyyMMdd_hhmmss}.XML", fileBaseName, DateTime.Now));
                    File.Copy(fileName, backupName, true);

                    // limit last count saves
                    List<string> oldFiles = new List<string>(Directory.GetFiles(dirName, fileBaseName + ".*.XML"));
                    oldFiles.Sort();
                    oldFiles.Reverse();
                    if (oldFiles.Count > count)
                    {
                        for (int i = 20; i < oldFiles.Count; i++)
                        {
                            Log.InfoFormat("Cleaning up old file, {0}", oldFiles[i]);
                            File.Delete(oldFiles[i]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Error backing up files", ex);
                }
            }
        }

        public int CompareTo(object obj)
        {
            SessionData s = obj as SessionData;
            return s == null ? 1 : this.SessionId.CompareTo(s.SessionId);
        }

        public static string CombineSessionIds(string parent, string child) 
        {
            if (parent == null && child == null)
            {
                return null;
            } 
            else if (child == null) 
            {
                return parent;
            }
            else if (parent == null)
            {
                return child;
            }
            else
            {
                return parent + "/" + child;
            }
        }

        public static string GetSessionNameFromId(string sessionId)
        {
            string[] parts = GetSessionNameParts(sessionId);
            return parts.Length > 0 ? parts[parts.Length - 1] : sessionId;
        }

        /// <summary>Split the SessionID into its parent/child parts</summary>
        /// <param name="sessionId">The SessionID</param>
        /// <returns>A string array containing the individual path components</returns>
        public static string[] GetSessionNameParts(string sessionId)
        {
            return sessionId.Split('/');
        }

        /// <summary>Get the parent ID of the specified session</summary>
        /// <param name="sessionId">the ID of the session</param>
        /// <returns>A string containing the parent sessions ID</returns>
        public static string GetSessionParentId(string sessionId)
        {
            string parentPath = null;
            if (sessionId != null)
            {
                int idx = sessionId.LastIndexOf('/');
                if (idx != -1)
                {
                    parentPath = sessionId.Substring(0, idx);
                }
            }
            return parentPath;
        }

        /// <summary>Create a deep copy of the SessionData object</summary>
        /// <returns>A clone of the <seealso cref="SessionData"/> object</returns>
        public object Clone()
        {
            SessionData session = new SessionData();
            session.CopyFrom(this);
            
            return session;
        }

        public void CopyFrom(SessionData SessionToCopy)
        {
            if (SessionToCopy == null)
                return;

            foreach (PropertyInfo pi in SessionToCopy.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (pi.CanWrite)
                {
                    pi.SetValue(this, pi.GetValue(SessionToCopy, null), null);
                }
            }
        }

        /// <summary>Return a string containing a uri to the protocol://host:port of this sesssions defined host</summary>
        /// <returns>A string in uri format containing connection information to this sessions host</returns>
        public override string ToString()
        {
            if (this.Proto == ConnectionProtocol.Cygterm || this.Proto == ConnectionProtocol.Mintty)
            {
                return string.Format("{0}://{1}", this.Proto.ToString().ToLower(), this.Host);
            }

            if (this.Proto == ConnectionProtocol.VNC || this.Proto == ConnectionProtocol.RDP)
            {
                if (this.Port == 0)
                    return string.Format("{0}://{1}", this.Proto.ToString().ToLower(), this.Host);
                else
                    return string.Format("{0}://{1}::{2}", this.Proto.ToString().ToLower(), this.Host, this.Port);
            }

            return string.Format("{0}://{1}:{2}", this.Proto.ToString().ToLower(), this.Host, this.Port);
        }

        class PuttySessionConverter : StringConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }
            public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                return new StandardValuesCollection(PuttyDataHelper.GetSessionNames());
            }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return true;
            }
        }

        class ImageKeyConverter : StringConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }
            public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                return new StandardValuesCollection(SuperPuTTY.Images.Images.Keys);
            }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return true;
            }
            
        }

        class ImageKeyEditor : UITypeEditor
        {
            public override bool GetPaintValueSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override void PaintValue(PaintValueEventArgs e)
            {
                string ImageKey = e.Value.ToString();
                Image img = SuperPuTTY.Images.Images[ImageKey];

                if (img == null)
                    return;

                Rectangle destRect = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Height, e.Bounds.Height);
                e.Graphics.DrawImage(img, destRect);
                e.Graphics.ExcludeClip(e.Bounds);
            }
        }
    }
}
