using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperPutty.Data;
using log4net;

namespace SuperPutty.Utils
{
    /// <summary>
    /// Helper class to parse host connection strings (e.g. ssh://localhost:2222)
    /// </summary>
    public class HostConnectionString
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(HostConnectionString));

        public HostConnectionString(string hostString)
        {

            int idx = hostString.IndexOf("://");
            string hostPort = hostString;
            if (idx != -1)
            {
                // ssh://localhost:2020, or ssh2://localhost:2020
                this.Protocol = (ConnectionProtocol)Enum.Parse(typeof(ConnectionProtocol), hostString.Substring(0, idx), true);
                if (this.Protocol == ConnectionProtocol.SSH2)
                {
                    // ssh2 is accepted only as a compatibility feature
                    this.Protocol = ConnectionProtocol.SSH;
                }
                hostPort = hostString.Substring(idx + 3);
            }

            // Firefox addes a '/'...
            hostPort = hostPort.TrimEnd('/');
            int idxPort = hostPort.IndexOf(":");
            if (idxPort != -1)
            {
                // localhost:2020
                int port;
                if (int.TryParse(hostPort.Substring(idxPort + 1), out port))
                {
                    this.Host = hostPort.Substring(0, idxPort);
                    this.Port = port;
                }
                else
                {
                    this.Host = hostPort;
                }

            }
            else
            {
                // localhost
                this.Host = hostPort;
            }


            log.InfoFormat(
                "Parsed[{0}]: proto={1}, host={2}, port={3}", 
                hostString, 
                this.Protocol.HasValue ? this.Protocol.ToString() : "", 
                this.Host, 
                this.Port.HasValue ? this.Port.ToString() : "");
        }

        public ConnectionProtocol? Protocol { get; set; }
        public string Host { get; set; }
        public int? Port { get; set; }
    }
}
