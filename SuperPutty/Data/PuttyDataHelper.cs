using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Web;

namespace SuperPutty.Data
{
    public class PuttyDataHelper
    {

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

            RegistryKey key = RootAppKey;
            if (key != null)
            {
                string[] savedSessionNames = key.GetSubKeyNames();
                foreach (string rawSession in savedSessionNames)
                {
                    names.Add(HttpUtility.UrlDecode(rawSession));
                }
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
    }
}
