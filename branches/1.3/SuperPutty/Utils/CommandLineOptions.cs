using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperPutty.Data;
using log4net;
using System.Web;

namespace SuperPutty.Utils
{
    /// <summary>
    /// Use:
    /// --------------------------------------------------------------------------
    /// SuperPutty.exe -layout LAYOUT_NAME
    /// OR
    /// SuperPutty.exe -session SESSION_NAME
    /// OR 
    /// SuperPutty.exe -[PROTOCOL] -P PORT -l USER -pw PASSWORD -load SETTINGS HOSTNAME
    /// OR 
    /// SuperPutty.exe -l USER -pw PASSWORD -load SETTINGS PROTOCOL://HOSTNAME:port
    /// ------------
    /// Options:
    /// -ssh|-serial|-telnet|-scp|-raw|-rlogin|-cygterm   -Choose Protocol (default: ssh)
    /// -P                                                -Port            (default: 22)
    /// -l                                                -Login Name
    /// -pw                                               -Login Password
    /// -load                                             -Session to load (default: Default Session)
    /// --------------------------------------------------------------------------
    /// SuperPutty.exe -layout LAYOUT_NAME
    /// SuperPutty.exe -session SESSION_ID
    /// SuperPutty.exe -ssh -P 22 -l homer -pw springfield -load pp1 prod-reactor
    /// SuperPutty.exe -l peter -pw donut foobar
    /// </summary>
    public class CommandLineOptions
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CommandLineOptions));

        public CommandLineOptions(string[] args)
        {
            try
            {
                if (args.Length == 1 && args[0].EndsWith("/") && args[0].Contains("://") && args[0].Contains("%20"))
                {
                    // special case for ssh links in browser
                    // ssh://localhost:22%20-l%20beau/
                    string cmdLine = HttpUtility.UrlDecode(args[0].TrimEnd('/'));
                    args = cmdLine.Split(' ');
                }
                Parse(args);
                this.IsValid = true;
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("Error parsing args [{0}]", String.Join(" ", args)), ex);
                this.IsValid = false;
            }
        }

        void Parse(string[] args)
        {
            Log.InfoFormat("CommandLine: [{0}]", String.Join(" ", args));
            Queue<string> queue = new Queue<string>(args);
            string arg = null;
            while(queue.Count > 0)
            {
                arg = queue.Dequeue();
                switch (arg)
                {
                    case "-layout":
                        this.Layout = queue.Dequeue();
                        break;
                    case "-session":
                        this.SessionId = queue.Dequeue();
                        break;
                    case "-ssh":
                        this.Protocol = ConnectionProtocol.SSH;
                        break;
                    case "-telnet":
                        this.Protocol = ConnectionProtocol.Telnet;
                        break;
                    case "-rlogin":
                        this.Protocol = ConnectionProtocol.Rlogin;
                        break;
                    case "-raw":
                        this.Protocol = ConnectionProtocol.Raw;
                        break;
                    case "-serial":
                        this.Protocol = ConnectionProtocol.Serial;
                        break;
                    case "-cygterm":
                        this.Protocol = ConnectionProtocol.Cygterm;
                        break;
                    case "-scp":
                        this.UseScp = true;
                        break;
                    case "-P":
                        this.Port = int.Parse(queue.Dequeue());
                        break;
                    case "-l":
                        this.UserName = queue.Dequeue();
                        break;
                    case "-pw":
                        this.Password = queue.Dequeue();
                        break;
                    case "-load":
                        this.PuttySession = queue.Dequeue();
                        break;
                    default:
                        // unflagged arg must be the host...
                        this.Host = arg;
                        break;
                }
            }
        }


        public SessionDataStartInfo ToSessionStartInfo()
        {
            SessionDataStartInfo ssi = null;
            if (this.Host == null && this.SessionId == null)
            {
                Log.Warn("Host or SessionId not provided, cannot create session");
            }
            else if (this.SessionId != null)
            {
                SessionData session = SuperPuTTY.GetSessionById(this.SessionId);
                if (session == null)
                {
                    Log.WarnFormat("Session from command line not found, id={0}", this.SessionId);
                }
                else
                {
                    ssi = new SessionDataStartInfo 
                    { 
                        Session = session, 
                        UseScp = this.UseScp 
                    };
                }
            }
            else if (this.Host == null)
            {
                Log.WarnFormat("Host not provided, cannot create session");
            }
            else
            {
                // ssh://localhost:2020
                HostConnectionString connStr = new HostConnectionString(this.Host);
                this.Host = connStr.Host;
                this.Protocol = connStr.Protocol.GetValueOrDefault(this.Protocol.GetValueOrDefault(ConnectionProtocol.SSH));
                this.Port = connStr.Port.GetValueOrDefault(this.Port.GetValueOrDefault(dlgEditSession.GetDefaultPort(this.Protocol.GetValueOrDefault())));

                /*
                int idx = this.Host.IndexOf("://");
                if (idx != -1)
                {
                    this.Protocol = (ConnectionProtocol) Enum.Parse(typeof(ConnectionProtocol), this.Host.Substring(0, idx), true);
                    string hostPort = this.Host.Substring(idx + 3);

                    int idxPort = hostPort.IndexOf(":");
                    if (idxPort != -1)
                    {
                        this.Host = hostPort.Substring(0, idxPort);
                        this.Port = Convert.ToInt32(hostPort.Substring(idxPort + 1));
                    }
                    else
                    {
                        this.Host = hostPort;
                    }
                }*/

                ssi = new SessionDataStartInfo();
                ssi.Session = new SessionData
                {
                    Host = this.Host,
                    SessionName = this.Host,
                    SessionId = SuperPuTTY.MakeUniqueSessionId(SessionData.CombineSessionIds("CLI", this.Host)),
                    Port = this.Port.GetValueOrDefault(22),
                    Proto = this.Protocol.GetValueOrDefault(ConnectionProtocol.SSH),
                    Username = this.UserName,
                    Password = this.Password,
                    PuttySession = this.PuttySession
                };
                ssi.UseScp = this.UseScp;
            }

            return ssi;
        }

        public string ExePath { get; private set; }
        public string Layout { get; private set; }
        public string SessionId { get; private set; }
        public bool IsValid { get; private set; }

        public bool UseScp { get; private set; }
        public ConnectionProtocol? Protocol { get; private set; }
        public int? Port { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public string PuttySession { get; private set; }

        public string Host { get; private set; }

    }
}
