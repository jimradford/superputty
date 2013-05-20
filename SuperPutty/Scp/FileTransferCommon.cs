using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.IO;
using System.Security.Principal;
using log4net;
using System.Security.AccessControl;
using SuperPutty.Data;
using SuperPutty.Gui;
using SuperPutty.Utils;
using System.Threading;

namespace SuperPutty.Scp
{
    public interface IFileTransferPresenter
    {
        bool CanTransferFile(BrowserFileInfo source, BrowserFileInfo target);
        void TransferFiles(FileTransferRequest transfer);
        void Cancel(int id);
        void Remove(int id);
        void Restart(int id);
        FileTransferViewModel ViewModel { get; }
    }

    public class FileTransferRequest
    {
        public FileTransferRequest()
        {
            this.SourceFiles = new List<BrowserFileInfo>();
        }
        public SessionData Session { get; set; }
        public List<BrowserFileInfo> SourceFiles { get; set; }
        public BrowserFileInfo TargetFile { get; set; }
    }

    public class FileTransfer 
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FileTransfer));

        static int idSeed = 0;

        private Thread thread = null;

        public FileTransfer(FileTransferRequest request)
        {
            this.Id = Interlocked.Increment(ref idSeed);
            this.Request = request;
        }

        public void Start()
        {
            lock (this)
            {
                if (!this.IsActive)
                {
                    Log.InfoFormat("Starting transfer, id={0}", this.Id);
                    this.thread = new Thread(this.DoTransfer);
                    this.thread.IsBackground = false;
                    this.Start();
                }
                else
                {
                    Log.WarnFormat("Attempted to start active transfer, id={0}", this.Id);
                }
            }
        }

        public void Cancel()
        {
            lock (this)
            {
                if (this.IsActive)
                {
                    Log.InfoFormat("Canceling active transfer, id={0}", this.Id);
                    this.thread.Abort();
                    this.TransferStatus = Status.Canceled;
                }
                else
                {
                    Log.WarnFormat("Attempted to cancel inactive trandfer, id={0}", this.Id);
                }
            }
        }

        void DoTransfer()
        {
            lock (this)
            {
                this.TransferStatus = Status.Running;

                try
                {

                }
                catch (Exception ex)
                {
                    Log.Error("Error running transfer, id=" + this.Id, ex);
                    this.TransferStatus = Status.Error;
                }
                this.TransferStatus = Status.Complete;
            }
        }

        public bool IsActive
        {
            get
            {
                lock (this)
                {
                    Status status = this.TransferStatus;
                    return
                        status == FileTransfer.Status.Initializing ||
                        status == FileTransfer.Status.Running;
                }
            }
        }

        public int Id { get; private set; }
        public FileTransferRequest Request { get; private set; }

        public double PercentComplete { get; set; }
        public Status TransferStatus { get; set; }
        public int TotalSizeBytes { get; set; }

        public enum Status
        {
            Initializing,
            Running,
            Complete,
            Error,
            Canceled
        }
    }

    #region FileTransferViewModel
    public class FileTransferViewModel : BaseViewModel
    {
        public FileTransferViewModel()
        {
            this.FileTransfers = new SortableBindingList<FileTransferViewItem>();
        }

        public BindingList<FileTransferViewItem> FileTransfers { get; set; }
    } 
    #endregion

    #region FileTransferViewItem
    /// <summary>
    /// Converts FileTransfer
    /// </summary>
    public class FileTransferViewItem : BaseViewModel
    {
        public FileTransferViewItem()
        {
            this.Context = SynchronizationContext.Current;
        }

        public FileTransferViewItem(string session, string source, string target)
            : this()
        {
            this.Session = session;
            this.Source = source;
            this.Target = target;
        }

        public FileTransferViewItem(FileTransfer transfer)
            : this()
        {
            this.Id = transfer.Id;
            this.Session = transfer.Request.Session.SessionId;
            this.Source = ToString(transfer.Request.SourceFiles);
            this.Target = transfer.Request.TargetFile.Path;
        }

        public int Id { get; private set; }
        public string Session { get; private set; }
        public string Source { get; private set; }
        public string Target { get; private set; }

        private FileTransfer Transfer { get; set; }

        public DateTime? Start
        {
            get { return this.start; }
            set { SetField(ref this.start, value, () => this.Start); }
        }

        public DateTime? End
        {
            get { return this.end; }
            set { SetField(ref this.end, value, () => this.End); }
        }

        public int Progress
        {
            get { return this.progress; }
            set { SetField(ref this.progress, value, () => this.Progress); }
        }

        public FileTransfer.Status Status
        {
            get { return this.status; }
            set { SetField(ref this.status, value, () => this.Status); }
        }

        public string Message
        {
            get { return this.message; }
            set { SetField(ref this.message, value, () => this.Message); }
        }

        public bool IsActive
        {
            get { return this.isActive; }
            set { SetField(ref this.isActive, value, () => this.IsActive); }
        }

        static string ToString(List<BrowserFileInfo> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            string strSource;
            if (source.Count == 1)
            {
                strSource = source[0].Path;
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (BrowserFileInfo info in source)
                {
                    sb.AppendLine(info.Path);
                }
                strSource = sb.ToString();
            }
            return strSource;
        }

        DateTime? start;
        DateTime? end;
        int progress;
        FileTransfer.Status status;
        string message;
        bool isActive;
    }
    
    #endregion
}