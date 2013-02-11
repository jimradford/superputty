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
                if (args.Length > 0)
                {
                    Parse(args);
                    this.IsValid = true;
                }
                else
                {
                    // no args to consider
                    this.IsValid = false;
                }
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
                    case "-ssh2":
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
                    case "--help":
                        this.Help = true;
                        return;
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
            if (this.SessionId != null)
            {
                // first try to resolve by sessionId
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
            else if (this.Host != null ||  this.PuttySession != null)
            {
                // Host or puttySession provided
                string sessionName;
                if (this.Host != null)
                {
                    // Decode URL type host spec, if provided (e.g. ssh://localhost:2020)
                    HostConnectionString connStr = new HostConnectionString(this.Host);
                    this.Host = connStr.Host;
                    this.Protocol = connStr.Protocol.GetValueOrDefault(this.Protocol.GetValueOrDefault(ConnectionProtocol.SSH));
                    this.Port = connStr.Port.GetValueOrDefault(this.Port.GetValueOrDefault(dlgEditSession.GetDefaultPort(this.Protocol.GetValueOrDefault())));
                    sessionName = this.Host;
                }
                else
                {
                    // no host provided so assume sss
                    sessionName = this.PuttySession;
                }

                ssi = new SessionDataStartInfo();
                ssi.Session = new SessionData
                {
                    Host = this.Host,
                    SessionName = sessionName,
                    SessionId = SuperPuTTY.MakeUniqueSessionId(SessionData.CombineSessionIds("CLI", this.Host)),
                    Port = this.Port.GetValueOrDefault(22),
                    Proto = this.Protocol.GetValueOrDefault(ConnectionProtocol.SSH),
                    Username = this.UserName,
                    Password = this.Password,
                    PuttySession = this.PuttySession
                };
                ssi.UseScp = this.UseScp;
            }

            if (ssi == null)
            {
                Log.WarnFormat("Could not determine session or host to connect.  SessionId or Host or PuttySession must be provided");
            }

            return ssi;
        }

        /// <summary>
        /// Return usage string
        /// </summary>
        /// <returns></returns>
        public static string Usage()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Usage:");
            sb.AppendLine("");
            sb.AppendLine("  SuperPutty.exe -session SESSION");
            sb.AppendLine("  SuperPutty.exe -layout LAYOUT");
            sb.AppendLine("  SuperPutty.exe -load SETTINGS");
            sb.AppendLine("  SuperPutty.exe -PROTO -P PORT -l USER -pw PASSWORD -load SETTINGS HOST");
            sb.AppendLine("  SuperPutty.exe -l USER -pw PASSWORD -load SETTINGS PROTO://HOST:PORT");
            sb.AppendLine();
            sb.AppendLine("Options:");
            sb.AppendLine();
            sb.AppendLine("  SESSION\t\t - Session id");
            sb.AppendLine("  LAYOUT\t\t - Layout name");
            sb.AppendLine("  SETTINGS\t - Putty Saved Session Profile");
            sb.AppendLine("  PROTO\t\t - Protocol - (ssh|ssh2|telnet|serial|raw|scp|cygterm|rlogin|mintty)");
            sb.AppendLine("  USER\t\t - User name");
            sb.AppendLine("  PASSWORD\t - Login Password");
            sb.AppendLine("  HOST\t\t - Hostname");
            sb.AppendLine();
            sb.AppendLine("Examples:");
            sb.AppendLine();
            sb.AppendLine("  SuperPutty.exe -session nyc-qa-1");
            sb.AppendLine("  SuperPutty.exe -layout prod");
            sb.AppendLine("  SuperPutty.exe -ssh -P 22 -l homer -pw springfield -load pp1 prod-reactor");
            sb.AppendLine("  SuperPutty.exe -l peter -pw griffin stewie01");
            sb.AppendLine("  SuperPutty.exe -load localhost");
            
            return sb.ToString();
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
        public bool Help { get; private set; }

    }
}
