using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Web;
using log4net;
using System.Xml;
using System.IO;

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
        public static List<string> GetSessionNames()
        {
            List<string> names = new List<string>();
            names.Add(SessionEmptySettings);
            RegistryKey key = RootAppKey;
            if (key != null)
            {
                string[] savedSessionNames = key.GetSubKeyNames();
                foreach (string rawSession in savedSessionNames)
                {
                    names.Add(HttpUtility.UrlDecode(rawSession));
                }
            }

            if (!names.Contains(SessionDefaultSettings))
            {
                names.Insert(1, SessionDefaultSettings);
            }

            return names;
        }

        public static List<SessionData> GetAllSessionsFromPuTTY()
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
                        SessionData session = new SessionData();
                        session.Host = (string)sessionKey.GetValue("HostName", "");
                        session.Port = (int)sessionKey.GetValue("PortNumber", 22);
                        session.Proto = (ConnectionProtocol)Enum.Parse(typeof(ConnectionProtocol), (string)sessionKey.GetValue("Protocol", "SSH"), true);
                        session.PuttySession = (string)sessionKey.GetValue("PuttySession", HttpUtility.UrlDecode(keyName));
                        session.SessionName = HttpUtility.UrlDecode(keyName);
                        session.Username = (string)sessionKey.GetValue("UserName", "");
                        sessions.Add(session);
                    }
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

                SessionData session = new SessionData();
                session.SessionName = info.SelectSingleNode("name").InnerText;
                session.Host = info.SelectSingleNode("host").InnerText;
                session.Port = Convert.ToInt32(info.SelectSingleNode("port").InnerText);
                session.Proto = (ConnectionProtocol)Enum.Parse(typeof(ConnectionProtocol), info.SelectSingleNode("protocol").InnerText);
                session.PuttySession = info.SelectSingleNode("session").InnerText;
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
