using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.IO;
using SuperPutty.Data;

namespace SuperPutty.Scp
{
    public class LocalBrowserModel : IBrowserModel
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LocalBrowserModel));

        public ListDirectoryResult ListDirectory(SessionData session, BrowserFileInfo path)
        {
            Log.InfoFormat("GetFilesForPath, path={0}", path.Path);

            ListDirectoryResult result = new ListDirectoryResult(path.Path);
            try
            {
                LoadDirectory(path, result);
            }
            catch (Exception ex)
            {
                result.SetError(null, ex);
                Log.ErrorFormat("Error loading directory: {0}", ex.Message);
            }

            return result;
        }

        void LoadDirectory(BrowserFileInfo path, ListDirectoryResult result)
        {
            if (path.Path == null)
            {
                // special case for drives/mounts
                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    BrowserFileInfo bfiDrive = new BrowserFileInfo(drive);
                    result.Add(bfiDrive);
                    result.MountCount++;
                }
            }
            else if (Directory.Exists(path.Path))
            {
                // get new files
                DirectoryInfo newDir = new DirectoryInfo(path.Path);

                // add back (..) entry, if relevant
                if (newDir.Parent != null)
                {
                    // has valid parent dir
                    BrowserFileInfo bfiParent = new BrowserFileInfo(newDir.Parent);
                    bfiParent.Name = "..";
                    bfiParent.Type = FileType.ParentDirectory;
                    result.Add(bfiParent);
                }
                else
                {
                    // hit top of root so list drives, special case of null path
                    BrowserFileInfo bfiListDrives = new BrowserFileInfo
                    {
                        Path = null,
                        Name = "..",
                        Type = FileType.ParentDirectory,
                    };
                    result.Add(bfiListDrives);
                }

                // dirs
                foreach (DirectoryInfo di in newDir.GetDirectories())
                {
                    BrowserFileInfo bfi = new BrowserFileInfo(di);
                    result.Add(bfi);
                }

                // files
                foreach (FileInfo fi in newDir.GetFiles())
                {
                    BrowserFileInfo bfi = new BrowserFileInfo(fi);
                    result.Add(bfi);
                }
            }
            else
            {
                result.SetError(string.Format("Directory doesn't exist: {0}", path.Path), null);
            }
        }

    }
}
