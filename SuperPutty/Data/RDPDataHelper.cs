using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Web;
using log4net;
using System.Xml;
using System.IO;
using System.Linq;

namespace SuperPutty.Data
{
    /// <summary>Helper methods used mostly for importing settings and session data from other applications</summary>
    public class RDPDataHelper
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(RDPDataHelper));

        public static RegistryKey RootAppKey
        {
            get
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Terminal Server Client\Default");
                return key;
            }
        }

        public static List<SessionData> GetAllSessionsFromRegistry()
        {
            List<SessionData> sessions = new List<SessionData>();

            RegistryKey key = RootAppKey;
            if (key != null)
            {
                string[] cachedEntriesNames = key.GetValueNames();
                foreach (string keyName in cachedEntriesNames)
                {
                    if (!keyName.StartsWith("MRU", StringComparison .CurrentCulture))
                        continue;
                    string sessionName = (string)key.GetValue(keyName,"",RegistryValueOptions.DoNotExpandEnvironmentNames);
                    if (sessionName != null)
                    {
                        SessionData session = new SessionData
                        {
                            Host = sessionName,
                            Port = 3389,
                            Proto = 
                                (ConnectionProtocol)
                                    Enum.Parse(typeof (ConnectionProtocol),
                                        "RDP", true),
                            PuttySession = sessionName,
                            SessionName = sessionName,
                            Username = ""
                        };
                        sessions.Add(session);
                    }
                }
            }

            return sessions;
        }
    }
}
