using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.IO;
using System.Security.Principal;
using log4net;
using System.Security.AccessControl;
using SuperPutty.Data;
using System.Threading;

namespace SuperPutty.Scp
{
    #region IBrowserPresenter
    public interface IBrowserPresenter
    {
        event EventHandler<AuthEventArgs> AuthRequest;

        void LoadDirectory(BrowserFileInfo dir);
        void Refresh();

        bool CanTransferFile(BrowserFileInfo source, BrowserFileInfo target);
        void TransferFiles(FileTransferRequest fileTransfer);

        IBrowserViewModel ViewModel { get; }
        BrowserFileInfo CurrentPath { get; }
        SessionData Session { get; }
    }
    #endregion
    
    #region IBrowserViewModel
    public interface IBrowserViewModel : INotifyPropertyChanged
    {
        string Name { get; set; }
        string CurrentPath { get; set; }
        string Status { get; set; }

        BrowserState BrowserState { get; set; }
        BindingList<BrowserFileInfo> Files { get; }
        SynchronizationContext Context { get; }
    }
    #endregion

    #region IBrowserModel
    /// <summary>
    /// Core biz logic to login and get files from the datasource (local, scp)
    /// </summary>
    public interface IBrowserModel
    {
        ListDirectoryResult ListDirectory(SessionData session, BrowserFileInfo path);
    }
    #endregion

    #region PscpResult

    public class PscpResult
    {
        public void SetErrorFormat(string msgTemplate, params object[] args)
        {
            SetError(string.Format(msgTemplate, args), null);
        }

        public void SetError(string msg, Exception ex)
        {
            this.ErrorMsg = msg;
            this.Error = ex;
            this.StatusCode = ResultStatusCode.Error;
        }

        public ResultStatusCode StatusCode { get; set; }
        public Exception Error { get; set; }
        public string ErrorMsg { get; set; }
    }

    #endregion 

    #region ListDirectoryResult
    public class ListDirectoryResult : PscpResult
    {
        public ListDirectoryResult(BrowserFileInfo path)
        {
            this.Path = path;
            this.Files = new List<BrowserFileInfo>();
        }

        public void Add(BrowserFileInfo fileInfo)
        {
            this.Files.Add(fileInfo);
            if (fileInfo.Type == FileType.File)
            {
                this.FileCount++;
            }
            else if (fileInfo.Type == FileType.Directory)
            {
                this.DirCount++;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[").Append(GetType().Name);
            sb.Append(" Path=").Append(this.Path.Path);
            sb.Append(", StatusCode=").Append(this.StatusCode);
            sb.Append(", ErrorMsg=").Append(this.ErrorMsg);
            sb.Append("]");
            return sb.ToString();
        }

        public BrowserFileInfo Path { get; private set; }
        public List<BrowserFileInfo> Files { get; set; }

        public int FileCount { get; set; }
        public int DirCount { get; set; }
        public int MountCount { get; set; }

    } 
    #endregion

    #region FileTransferResult 
    public class FileTransferResult : PscpResult
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[").Append(GetType().Name);
            sb.Append(", StatusCode=").Append(this.StatusCode);
            sb.Append(", ErrorMsg=").Append(this.ErrorMsg);
            sb.Append("]");
            return sb.ToString();
        }
    }
    #endregion

    #region BrowserFileInfo
    public class BrowserFileInfo
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(BrowserFileInfo));

        public BrowserFileInfo() { }

        public BrowserFileInfo(FileInfo fi) 
        {
            Init(fi);
            Type = FileType.File;
            Size = fi.Length;
        }

        public BrowserFileInfo(DirectoryInfo di)
        {
            Init(di);
            Type = FileType.Directory;
        }

        public BrowserFileInfo(DriveInfo drive)
        {
            Path = drive.RootDirectory.FullName;
            Name = drive.Name;
            Type = FileType.Directory;
            CreateTime = drive.RootDirectory.CreationTime;
            LastModTime = drive.RootDirectory.LastWriteTime;
            Source = SourceType.Local;
        }

        void Init(FileSystemInfo fsi)
        {
            Path = fsi.FullName;
            Name = fsi.Name;
            CreateTime = fsi.CreationTime;
            LastModTime = fsi.LastWriteTime;
            Permissions = GetPermissions(fsi.Attributes);
            Source = SourceType.Local;
            try
            {
                FileSecurity fs = File.GetAccessControl(fsi.FullName);
                Owner = fs.GetOwner(typeof(NTAccount)).ToString();
                Group = fs.GetGroup(typeof(NTAccount)).ToString();
            }
            catch (Exception ex)
            {
                Log.DebugFormat("Unable to get owner/group.  dir={0}, err={1}", fsi.FullName, ex.Message);
            }
        }

        string GetPermissions(FileAttributes attribs)
        {
            StringBuilder sb = new StringBuilder();
            if ((attribs & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                sb.Append("ro ");
            }
            if ((attribs & FileAttributes.System) == FileAttributes.System)
            {
                sb.Append("sys ");
            }
            if ((attribs & FileAttributes.Hidden) == FileAttributes.Hidden)
            {
                sb.Append("hid ");
            }
            return sb.ToString();
        }

        public string Path { get; set; }
        public string Name { get; set; }
        public FileType Type { get; set; }
        public string Permissions { get; set; }
        public long Size { get; set; }
        public string Owner { get; set; }
        public string Group { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime LastModTime { get; set; }
        public SourceType Source { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("[BrowserFileInfo ");
            sb.Append("Path=").Append(this.Path);
            sb.Append(", Name=").Append(this.Name);
            sb.Append(", Type=").Append(this.Type);
            sb.Append(", Permissions=").Append(this.Permissions);
            sb.AppendFormat(", Size={0:0.0}KB", this.Size / 1024.0);
            sb.Append(", Owner=").Append(this.Owner);
            sb.Append(", Group=").Append(this.Group);
            sb.AppendFormat(", CreateTime={0:s}", this.CreateTime);
            sb.AppendFormat(", LastModTime={0:s}", this.LastModTime);
            sb.AppendFormat(", Source={0}", this.Source);
            sb.Append("]");
            return sb.ToString();
        }
    }
    #endregion

    #region AuthEventArgs
    public class AuthEventArgs : EventArgs
    {
        public bool Handled { get; set; }
        public SessionData Session { get; private set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    } 
    #endregion

    public enum FileType 
    {
        ParentDirectory, 
        Directory, 
        File
    }

    public enum SourceType
    {
        Local, 
        Remote
    }

    public enum BrowserState
    {
        Ready,
        Working
    }

    public enum ResultStatusCode
    {
        Success,
        RetryAuthentication,
        Error
    }
}
