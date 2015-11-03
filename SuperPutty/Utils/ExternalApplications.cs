using System;
using log4net;
using SuperPutty.Data;
using System.Diagnostics;

namespace SuperPutty.Utils
{
    public static class ExternalApplications
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ExternalApplications));

        /// <summary>
        /// Open the filezilla program with the sesion data, for sftp connection. 
        /// </summary>
        /// <param name="session"></param>
        public static void openFileZilla(SessionData session)
        {
            if (!String.IsNullOrEmpty(session.Password) && !SuperPuTTY.Settings.AllowPlainTextPuttyPasswordArg)
                Log.Warn("SuperPuTTY is set to NOT allow the use of the -pw <password> argument, this can be overriden in Tools -> Options -> GUI");

            // open filezilla with the session info (https://wiki.filezilla-project.org/Command-line_arguments_%28Client%29)
            String pw = session.Password;
            String user = Uri.EscapeDataString(session.Username);
            String userPw =    !String.IsNullOrEmpty(user) ? (!String.IsNullOrEmpty(pw) && SuperPuTTY.Settings.AllowPlainTextPuttyPasswordArg ? user + ":" + pw      + "@" : user + "@") : "";
            String userPwLog = !String.IsNullOrEmpty(user) ? (!String.IsNullOrEmpty(pw) && SuperPuTTY.Settings.AllowPlainTextPuttyPasswordArg ? user + ":" + "XXXXX" + "@" : user + "@") : "";
            String rp = String.IsNullOrEmpty(session.RemotePath) ? "" : session.RemotePath;
            String lp = String.IsNullOrEmpty(session.LocalPath) ? "" : " --local=\"" + session.LocalPath + "\" ";
            String param = "sftp://" + userPw + session.Host + ":" + session.Port + rp + lp;
            
            Log.Debug("Send to FileZilla:" + SuperPuTTY.Settings.FileZillaExe + " params="+ "sftp://" + userPwLog + session.Host + ":" + session.Port + rp + lp);
            Process.Start(SuperPuTTY.Settings.FileZillaExe, param);
        }

        /// <summary>
        /// Open the WinSCP program with the sesion data, for sftp connection. 
        /// </summary>
        /// <param name="session"></param>
        public static void openWinSCP(SessionData session)
        {
            if (!String.IsNullOrEmpty(session.Password) && !SuperPuTTY.Settings.AllowPlainTextPuttyPasswordArg)
                Log.Warn("SuperPuTTY is set to NOT allow the use of the -pw <password> argument, this can be overriden in Tools -> Options -> GUI");

            // open WinSCP with the session info (https://winscp.net/eng/docs/commandline)           
            String pw = Uri.EscapeDataString(session.Password);
            String user = Uri.EscapeDataString(session.Username);
            String userPw =    !String.IsNullOrEmpty(user) ? (!String.IsNullOrEmpty(pw) && SuperPuTTY.Settings.AllowPlainTextPuttyPasswordArg ? user + ":" + pw      + "@" : user + "@") : "";
            String userPwLog = !String.IsNullOrEmpty(user) ? (!String.IsNullOrEmpty(pw) && SuperPuTTY.Settings.AllowPlainTextPuttyPasswordArg ? user + ":" + "XXXXX" + "@" : user + "@") : "";
            String rp = String.IsNullOrEmpty(session.RemotePath) ? "" : session.RemotePath;
            if (!rp.Substring(rp.Length).Equals("/"))
            {
                rp += "/";
            }
            String lp = String.IsNullOrEmpty(session.LocalPath) ? "" : " -rawsettings localDirectory=\"" + session.LocalPath + "\" ";
            String param = "sftp://" + userPw + session.Host + ":" + session.Port + rp + lp;
            Log.Debug("Send to WinSCP:" + SuperPuTTY.Settings.WinSCPExe + " params="+ "sftp://" + userPwLog + session.Host + ":" + session.Port + rp + lp);
            Process.Start(SuperPuTTY.Settings.WinSCPExe, param);
        }

    }
}
