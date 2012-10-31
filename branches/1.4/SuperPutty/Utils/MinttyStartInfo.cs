using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using SuperPutty.Data;
using System.Text.RegularExpressions;
using System.IO;

namespace SuperPutty.Utils
{

    /// <summary>
    /// Helper class for MinTTY support
    /// </summary>
    public class MinttyStartInfo
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MinttyStartInfo));

        public const string LocalHost = "localhost";
        private SessionData session;

        public MinttyStartInfo(SessionData session)
        {
            this.session = session;

            // parse host args and starting dir
            Match m = Regex.Match(session.Host, LocalHost + ":(.*)");
            String dir = m.Success ? m.Groups[1].Value : null;
            bool exists = false;
            if (dir != null)
            {
                exists = Directory.Exists(dir);
                Log.DebugFormat("Parsed dir from host. Host={0}, Dir={1}, Exists={2}", session.Host, dir, exists);
            }
            if (dir != null && exists)
            {
                // start bash...will start in process start dir
                // >mintty.exe /bin/env CHERE_INVOKING=1 /bin/bash -l
                this.Args = "/bin/env CHERE_INVOKING=1 /bin/bash -l";
                this.StartingDir = dir;
            }
            else
            {
                // login shell
                // http://code.google.com/p/puttycyg/wiki/FAQ
                this.Args = "-";
            }
        }

        public string Args { get; set; }
        public string StartingDir { get; set; }

    }
}
