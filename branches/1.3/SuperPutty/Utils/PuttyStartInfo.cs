using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperPutty.Data;
using log4net;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace SuperPutty.Utils
{
    public class PuttyStartInfo
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PuttyStartInfo));

        private static readonly Regex regExEnvVars = new Regex(@"(%\w+%)");

        public PuttyStartInfo(SessionData session)
        {
            string argsToLog = null;

            this.Executable = SuperPuTTY.Settings.PuttyExe;

            if (session.Proto == ConnectionProtocol.Cygterm)
            {
                CygtermStartInfo cyg = new CygtermStartInfo(session);
                this.Args = cyg.Args;
                this.WorkingDir = cyg.StartingDir;
            }
            else if (session.Proto == ConnectionProtocol.Mintty)
            {
                MinttyStartInfo mintty = new MinttyStartInfo(session);
                this.Args = mintty.Args;
                this.WorkingDir = mintty.StartingDir;
                this.Executable = SuperPuTTY.Settings.MinttyExe;
            }
            else
            {
                this.Args = MakeArgs(session, true);
                argsToLog = MakeArgs(session, false);
            }

            // attempt to parse env vars
            this.Args = this.Args.Contains('%') ? TryParseEnvVars(this.Args) : this.Args;

            Log.InfoFormat("Putty Args: '{0}'", argsToLog ?? this.Args);
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
            args += (!String.IsNullOrEmpty(session.Username) && session.Username.Length > 0) ? " -l " + session.Username + " " : "";
            args += session.Host;

            return args;
        }

        static string TryParseEnvVars(string args)
        {
            string result = args;
            try
            {
                result = regExEnvVars.Replace(
                    args,
                    m =>
                    {
                        string name = m.Value.Trim('%');
                        return Environment.GetEnvironmentVariable(name) ?? m.Value;
                    });
            }
            catch(Exception ex)
            {
                Log.Warn("Could not parse env vars in args", ex);
            }
            return result;
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
