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
                // ssh://localhost:2020
                this.Protocol = (ConnectionProtocol)Enum.Parse(typeof(ConnectionProtocol), hostString.Substring(0, idx), true);
                hostPort = hostString.Substring(idx + 3);
            }

            // Firefox addes a '/'...
            hostPort = hostPort.TrimEnd('/');
            int idxPort = hostPort.IndexOf(":");
            if (idxPort != -1)
            {
                // localhost:2020
                this.Host = hostPort.Substring(0, idxPort);
                this.Port = Convert.ToInt32(hostPort.Substring(idxPort + 1));
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
