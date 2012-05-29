using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperPutty.Data;
using log4net;
using System.Diagnostics;
using System.IO;

namespace SuperPutty.Utils
{
    public class PuttyStartInfo
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PuttyStartInfo));

        public PuttyStartInfo(SessionData session)
        {

            this.Executable = SuperPuTTY.Settings.PuttyExe;

            if (session.Proto == ConnectionProtocol.Cygterm)
            {
                CygtermInfo cyg = new CygtermInfo(session);
                this.Args = cyg.Args;
                this.WorkingDir = cyg.StartingDir;
            }
            else
            {
                this.Args = MakeArgs(session, true);
            }

            Log.InfoFormat("Putty Args: '{0}'", MakeArgs(session, false));
        }

        static string MakeArgs(SessionData session, bool includePassword)
        {
            string args = "-" + session.Proto.ToString().ToLower() + " ";
            args += (!String.IsNullOrEmpty(session.Password) && session.Password.Length > 0) 
                ? "-pw " + (includePassword ? session.Password : "XXXXX") + " " 
                : "";
            args += "-P " + session.Port + " ";
            args += (!String.IsNullOrEmpty(session.PuttySession)) ? "-load \"" + session.PuttySession + "\" " : "";
            args += (!String.IsNullOrEmpty(session.ExtraArgs) ? session.ExtraArgs + " " : "");
            args += (!String.IsNullOrEmpty(session.Username) && session.Username.Length > 0) ? session.Username + "@" : "";
            args += session.Host;

            return args;
        }

        /// <summary>
        /// Start of standalone putty process
        /// </summary>
        public void StartStandalone()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = this.Executable;
            startInfo.Arguments = this.Args;
            if (this.WorkingDir != null && Directory.Exists(this.WorkingDir))
            {
                startInfo.WorkingDirectory = this.WorkingDir;
            }
            Process.Start(startInfo);
        }

        public SessionData Session { get; private set; }

        public string Args { get; private set; }
        public string WorkingDir { get; private set; }
        public string Executable { get; private set; }

    }
}
