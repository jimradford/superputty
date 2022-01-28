using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Web;
using log4net;
using System.Xml;
using System.IO;
using System.Linq;
using IniParser;
using IniParser.Model;

namespace SuperPutty.Data
{
    /// <summary>Helper methods used mostly for importing settings and session data from other applications</summary>
    public class PuttyDataHelper
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(PuttyDataHelper));

        public const string SessionDefaultSettings = "Default Settings";
        public const string SessionEmptySettings = "";

        public static RegistryKey RootAppKey
        {
            get
            {
                RegistryKey key = SuperPuTTY.IsKiTTY
                    ? Registry.CurrentUser.OpenSubKey(@"Software\9bis.com\KiTTY\Sessions")
                    : Registry.CurrentUser.OpenSubKey(@"Software\SimonTatham\PuTTY\Sessions");
                return key;
            }
        }

        private static IniData GetIniDataIfEnabled()
        {
            // Ini suppoted Putty uses ini file (IniFile is Enabled) only when
            // 1. "putty.ini" exists on the exe folder
            string puttyIni = Path.ChangeExtension(SuperPuTTY.Settings.PuttyExe, ".ini");

            if (File.Exists(puttyIni))
            {
                FileIniDataParser parser = new IniParser.FileIniDataParser();
                IniData data = parser.ReadFile(puttyIni);

                //  2. following Entry exists in putty.ini
                //     [Generic]
                //     UseIniFile=1
                if (data["Generic"]["UseIniFile"] == "1")
                {
                    return data;
                }
            }
            return null;
        }

        public static List<string> GetSessionNames()
        {
            IniData data = GetIniDataIfEnabled();
            if (data == null) {
                return GetSessionNamesFromRegistry();
            } else {
                return GetSessionNamesFromIni(data);
            }
        }

        private static List<string> GetSessionNamesFromRegistry()
        {
            List<string> names = new List<string> {SessionEmptySettings};
            RegistryKey key = RootAppKey;
            if (key != null)
            {
                string[] savedSessionNames = key.GetSubKeyNames();
                names.AddRange(savedSessionNames.Select(HttpUtility.UrlDecode));
            }

            if (!names.Contains(SessionDefaultSettings))
            {
                names.Insert(1, SessionDefaultSettings);
            }

            return names;
        }

        private static ConnectionProtocol? ParseIniProtocol(string proto_str)
        {
            proto_str = proto_str.ToLower().Replace("\"", "");
            switch (proto_str) {
                case "ssh":
                    return ConnectionProtocol.SSH;
                case "serial":
                    return ConnectionProtocol.Serial;
                case "telnet":
                    return ConnectionProtocol.Telnet;
                case "rlogin":
                    return ConnectionProtocol.Rlogin;
                case "raw":
                    return ConnectionProtocol.Raw;            
                default: // "supdup", "bare-ssh"
                    return null; // not supported
            }
        }

        public static List<string> GetSessionNamesFromIni(IniData data)
        {
            List<string> names = new List<string> {SessionEmptySettings};

            SectionDataCollection sections = data.Sections;
            foreach (SectionData section in sections)
            {
                string sectionName = section.SectionName;
                if (sectionName.StartsWith("Session:"))
                {
                    string session_name = HttpUtility.UrlDecode(sectionName.Substring(8)/*"Len("Session:")*/);
                    
                    if (ParseIniProtocol(data[sectionName]["Protocol"]) == null)
                        continue; // skip unsupported protocol session
                    names.Add(session_name);
                };
            }

            if (!names.Contains(SessionDefaultSettings))
            {
                names.Insert(1, SessionDefaultSettings);
            }

            return names;
        }

        public static List<SessionData> GetAllSessionsFromPuTTY()
        {
            IniData data = GetIniDataIfEnabled();
            if (data == null) {
                return GetAllSessionsFromPuTTYRegistry();
            } else {
                return GetAllSessionsFromPuTTYIni(data);
            }
        }

        private static List<SessionData> GetAllSessionsFromPuTTYRegistry()
        {
            List<SessionData> sessions = new List<SessionData>();

            RegistryKey key = RootAppKey;
            if (key != null)
            {
                string[] savedSessionNames = key.GetSubKeyNames();
                foreach (string keyName in savedSessionNames)
                {
                    RegistryKey sessionKey = key.OpenSubKey(keyName);
                    if (sessionKey != null)
                    {
                        SessionData session = new SessionData
                        {
                            Host = (string) sessionKey.GetValue("HostName", ""),
                            Port = (int) sessionKey.GetValue("PortNumber", 22),
                            Proto =
                                (ConnectionProtocol)
                                    Enum.Parse(typeof (ConnectionProtocol),
                                        (string) sessionKey.GetValue("Protocol", "SSH"), true),
                            PuttySession = (string) sessionKey.GetValue("PuttySession", HttpUtility.UrlDecode(keyName)),
                            SessionName = HttpUtility.UrlDecode(keyName),
                            Username = (string) sessionKey.GetValue("UserName", "")
                        };
                        sessions.Add(session);
                    }
                }
            }

            return sessions;
        }
        private static List<SessionData> GetAllSessionsFromPuTTYIni(IniData data)
        {
            List<SessionData> sessions = new List<SessionData>();

            SectionDataCollection sections = data.Sections;
            foreach (SectionData section in sections)
            {
                string sectionName = section.SectionName;                
                // parse "[Session:UrlEncodedSessionName]" sections only
                if (sectionName.StartsWith("Session:"))
                {
                    string session_name = HttpUtility.UrlDecode(sectionName.Substring(8)/*Remove "Session:"*/);
                    ConnectionProtocol? proto = ParseIniProtocol(section.Keys["Protocol"]);
                    if (proto == null)
                        continue; // skip unsupported protocol

                    SessionData session = new SessionData
                    {
                        Host = section.Keys["HostName"],
                        Port = Convert.ToInt32(section.Keys["PortNumber"]),
                        Proto = (ConnectionProtocol)proto,
                        PuttySession = session_name,
                        SessionName = session_name,
                        Username = section.Keys["UserName"]
                    };
                    sessions.Add(session);
                }
            }

            return sessions;
        }

        public static List<SessionData> GetAllSessionsFromPuTTYCM(string fileExport)
        {
            List<SessionData> sessions = new List<SessionData>();

            if (fileExport == null || !File.Exists(fileExport))
            {
                return sessions;
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(fileExport);


            XmlNodeList connections = doc.DocumentElement.SelectNodes("//connection[@type='PuTTY']");
            foreach (XmlElement connection in connections)
            {
                List<string> folders = new List<string>();
                XmlElement node = connection.ParentNode as XmlElement;
                while (node != null && node.Name != "root")
                {
                    if (node.Name == "container" && node.GetAttribute("type") == "folder")
                    {
                        folders.Add(node.GetAttribute("name"));
                    }
                    node = node.ParentNode as XmlElement;
                }
                folders.Reverse();
                string parentPath = string.Join("/", folders.ToArray());

                XmlElement info = (XmlElement)connection.SelectSingleNode("connection_info");
                XmlElement login = (XmlElement)connection.SelectSingleNode("login");

                SessionData session = new SessionData
                {
                    SessionName = info.SelectSingleNode("name").InnerText,
                    Host = info.SelectSingleNode("host").InnerText,
                    Port = Convert.ToInt32(info.SelectSingleNode("port").InnerText),
                    Proto =
                        (ConnectionProtocol)
                            Enum.Parse(typeof (ConnectionProtocol), info.SelectSingleNode("protocol").InnerText),
                    PuttySession = info.SelectSingleNode("session").InnerText
                };
                session.SessionId = string.IsNullOrEmpty(parentPath) 
                    ? session.SessionName 
                    : SessionData.CombineSessionIds(parentPath, session.SessionName);
                session.Username = login.SelectSingleNode("login").InnerText;

                sessions.Add(session);
            }

            return sessions;
        }
    }
}
