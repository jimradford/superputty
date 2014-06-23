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
using System.Text;
using System.Net;
using Microsoft.Win32;
using WeifenLuo.WinFormsUI.Docking;
using log4net;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using System.Reflection;

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
        Mintty
    }

    /// <summary>The main class containing configuration settings for a session</summary>
    public class SessionData : IComparable, ICloneable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SessionData));

        /// <summary>Full session id (includes path for session tree)e.g. FolderName/SessionName</summary>
        private string _SessionId;
        [XmlAttribute]
        public string SessionId
        {
            get { return this._SessionId; }
            set
            {
                this.OldSessionId = SessionId;
                this._SessionId = value;
            }
        }
        internal string OldSessionId { get; set; }

        private string _OldName;
        [XmlIgnore]
        public string OldName
        {
            get { return _OldName; }
            set { _OldName = value; }
        }

        private string _SessionName;
        [XmlAttribute]
        public string SessionName
        {
            get { return _SessionName; }
            set { OldName = _SessionName; 
                _SessionName = value;
                if (SessionId == null)
                {
                    SessionId = value;
                }
            }
        }

        private string _ImageKey;
        /// <summary>A string containing the key of the image associated with this session</summary>
        [XmlAttribute]
        public string ImageKey
        {
            get { return _ImageKey; }
            set { _ImageKey = value; }
        }

        private string _Host;
        [XmlAttribute]
        public string Host
        {
            get { return _Host; }
            set { _Host = value; }
        }

        private int _Port;
        [XmlAttribute]
        public int Port
        {
            get { return _Port; }
            set { _Port = value; }
        }

        private ConnectionProtocol _Proto;
        [XmlAttribute]
        public ConnectionProtocol Proto
        {
            get { return _Proto; }
            set { _Proto = value; }
        }

        private string _PuttySession;
        [XmlAttribute]
        public string PuttySession
        {
            get { return _PuttySession; }
            set { _PuttySession = value; }
        }

        private string _Username;
        [XmlAttribute]
        public string Username
        {
            get { return _Username; }
            set { _Username = value; }
        }

        private string _Password;
        [XmlIgnore]
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        private string _ExtraArgs;
        [XmlAttribute]
        public string ExtraArgs
        {
            get { return _ExtraArgs; }
            set { _ExtraArgs = value; }
        }

        private DockState m_LastDockstate = DockState.Document;
        [XmlIgnore]
        public DockState LastDockstate
        {
            get { return m_LastDockstate; }
            set { m_LastDockstate = value; }
        }

        private bool m_AutoStartSession = false;
        [XmlIgnore]
        public bool AutoStartSession
        {
            get { return m_AutoStartSession; }
            set { m_AutoStartSession = value; }
        }

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
                        sessionList.Add(sessionData);
                    }
                }
            }
            return sessionList;
        }

        /// <summary>Load session configuration data from the specified XML file</summary>
        /// <param name="fileName">The filename containing the settings</param>
        /// <returns>A <seealso cref="List"/> containing the session configuration data</returns>
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
        /// <param name="sessions">A <seealso cref="List"/> containing the session configuration data</param>
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
            foreach (PropertyInfo pi in this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (pi.CanWrite)
                {
                    pi.SetValue(session, pi.GetValue(this, null), null);
                }
            }
            return session;
        }

        /// <summary>Return a string containing a uri to the protocol://host:port of this sesssions defined host</summary>
        /// <returns>A string in uri format containing connection information to this sessions host</returns>
        public override string ToString()
        {
            if (this.Proto == ConnectionProtocol.Cygterm || this.Proto == ConnectionProtocol.Mintty)
            {
                return string.Format("{0}://{1}", this.Proto.ToString().ToLower(), this.Host);
            }
            else
            {
                return string.Format("{0}://{1}:{2}", this.Proto.ToString().ToLower(), this.Host, this.Port);
            }
        }
    }
}
