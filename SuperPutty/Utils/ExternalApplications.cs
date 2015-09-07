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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void openFileZilla(SessionData session)
        {
            // open filezilla with the session info (https://wiki.filezilla-project.org/Command-line_arguments_%28Client%29)
            String pw = session.Password;
            String userPw = (!String.IsNullOrEmpty(session.Username)) ? ((!String.IsNullOrEmpty(pw)) ? session.Username + ":" + pw + "@" : session.Username + "@") : "";
            String rp = String.IsNullOrEmpty(session.RemotePath) ? "" : session.RemotePath;
            String lp = String.IsNullOrEmpty(session.LocalPath) ? "" : " --local=\"" + session.LocalPath + "\" ";
            String param = "sftp://" + userPw + session.Host + ":" + session.Port + rp + lp;
            Process.Start(SuperPuTTY.Settings.FileZillaExe, param);
        }
        /// <summary>
        /// Open the WinSCP program with the sesion data, for sftp connection. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void openWinSCP(SessionData session)
        {
            // open WinSCP with the session info (https://winscp.net/eng/docs/commandline)           
            String pw = Uri.EscapeDataString(session.Password);
            String user = Uri.EscapeDataString(session.Username);
            String userPw = (!String.IsNullOrEmpty(user)) ? ((!String.IsNullOrEmpty(pw)) ? user + ":" + pw + "@" : user + "@") : "";
            String rp = String.IsNullOrEmpty(session.RemotePath) ? "" : session.RemotePath;
            if (!rp.Substring(rp.Length).Equals("/"))
            {
                rp += "/";
            }
            String lp = String.IsNullOrEmpty(session.LocalPath) ? "" : " -rawsettings localDirectory=\"" + session.LocalPath + "\" ";
            String param = "sftp://" + userPw + session.Host + ":" + session.Port + rp + lp;
            Process.Start(SuperPuTTY.Settings.WinSCPExe, param);
        }
    }
}
