using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using SuperPutty.Data;

namespace SuperPutty.Scp
{
    public class RemoteBrowserModel : IBrowserModel
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(RemoteBrowserModel));

        public RemoteBrowserModel(string pscpLocation)
        {
            this.PscpLocation = pscpLocation;
        }

        /// <summary>
        /// Sync call to list directory contents
        /// </summary>
        /// <param name="session"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public ListDirectoryResult ListDirectory(SessionData session, BrowserFileInfo path)
        {
            ListDirectoryResult result;

            if (session == null || session.Username == null)
            {
                result = new ListDirectoryResult(path.Path);
                result.ErrorMsg = "Session invalid";
                result.StatusCode = ResultStatusCode.Error;
            }
            else
            {
                string targetPath = path.Path;
                if (targetPath == null || targetPath == ".")
                {
                    targetPath = string.Format("/home/{0}", session.Username);
                    Log.InfoFormat("Defaulting path: {0}->{1}", path.Path, targetPath);
                }

                PscpClient client = new PscpClient(this.PscpLocation);
                result = client.ListDirectory(session, targetPath);
            }

            return result;
        }

        public SessionData Session { get; private set; }
        public string PscpLocation { get; private set; }
    }

    public interface IPscpClient
    {
    }
}
