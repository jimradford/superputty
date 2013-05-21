using System;
using log4net;
using System.ComponentModel;
using SuperPutty.Data;

namespace SuperPutty.Scp
{
    #region LocalBrowserPresenter
    /// <summary>
    /// Start in last directory...preference
    /// </summary>
    public class BrowserPresenter : IBrowserPresenter
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(BrowserPresenter));

        public event EventHandler<AuthEventArgs> AuthRequest;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model">The pluggable biz logic model</param>
        /// <param name="session"></param>
        public BrowserPresenter(IBrowserModel model, SessionData session, IFileTransferPresenter fileTransferPresenter)
        {
            this.Model = model;
            this.Session = session;

            this.FileTransferPresenter = fileTransferPresenter;
            this.FileTransferPresenter.ViewModel.FileTransfers.ListChanged += (FileTransfers_ListChanged);

            this.BackgroundWorker = new BackgroundWorker();
            this.BackgroundWorker.WorkerReportsProgress = true;
            this.BackgroundWorker.WorkerSupportsCancellation = true;
            this.BackgroundWorker.DoWork += (BackgroundWorker_DoWork);
            this.BackgroundWorker.ProgressChanged += (BackgroundWorker_ProgressChanged);
            this.BackgroundWorker.RunWorkerCompleted += (BackgroundWorker_RunWorkerCompleted);

            this.ViewModel = new BrowserViewModel();
            this.ViewModel.BrowserState = BrowserState.Ready;
        }

        void FileTransfers_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemChanged)
            {
                BindingList<FileTransferViewItem> list = (BindingList<FileTransferViewItem>)sender;
                FileTransferViewItem item = list[e.NewIndex];
                if (item.Status == FileTransfer.Status.Complete &&
                    item.Target == this.CurrentPath.Path)
                {
                    Log.InfoFormat("Refreshing for FileTransferUpdate, path={0}", this.CurrentPath.Path);

                    this.ViewModel.Context.Post((x) => { Log.Info("####"); this.Refresh(); }, null);
                }
            }
        }

        #region Async Work

        void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.ViewModel.Status = (string) e.UserState;
        }

        void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BrowserFileInfo targetPath = (BrowserFileInfo)e.Argument;
            this.BackgroundWorker.ReportProgress(5, "Requesting files for " + targetPath.Path);
            
            ListDirectoryResult result = this.Model.ListDirectory(this.Session, targetPath);
            
            this.BackgroundWorker.ReportProgress(80, "Remote call complete: " + result.StatusCode);
            e.Result = result;
        }

        void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                string msg = string.Format("System error while loading directory: {0}", e.Error.Message);
                Log.Error(msg, e.Error);
                this.ViewModel.Status = msg;
            }
            else
            {
                ListDirectoryResult result = (ListDirectoryResult)e.Result;
                switch (result.StatusCode)
                {
                    case ResultStatusCode.RetryAuthentication:
                        // Request login- first login will be initiated from here after failing 1st call to pscp which attemps key based auth
                        AuthEventArgs authEvent = new AuthEventArgs
                        { 
                            UserName = this.Session.Username, 
                            Password = this.Session.Password 
                        };
                        this.OnAuthRequest(authEvent);
                        if (authEvent.Handled)
                        {
                            // retry listing
                            this.Session.Username = authEvent.UserName;
                            this.Session.Password = authEvent.Password;
                            this.LoadDirectory(result.Path);
                        }
                        break;
                    case ResultStatusCode.Success:
                        // list files
                        string msg = result.MountCount > 0
                            ? string.Format("{0} items", result.MountCount)
                            : string.Format("{0} files {1} directories", result.FileCount, result.DirCount);
                        this.ViewModel.Status = string.Format("{0} @ {1}", msg, DateTime.Now);
                        this.CurrentPath = result.Path;
                        this.ViewModel.CurrentPath = result.Path.Path;
                        BrowserViewModel.UpdateList(this.ViewModel.Files, result.Files);
                        break;
                    case ResultStatusCode.Error:
                        string errMsg = result.ErrorMsg != null
                            ? result.ErrorMsg 
                            : result.Error != null 
                                ? string.Format("Error listing directory, {0}", result.Error.Message) 
                                : "Unknown Error listing directory";
                        Log.Error(errMsg, result.Error);
                        this.ViewModel.Status = errMsg;
                        break;
                    default:
                        Log.InfoFormat("Unknown result '{0}'", result.StatusCode);
                        break;
                }
            }
            this.ViewModel.BrowserState = BrowserState.Ready;
        }

        #endregion

        public void LoadDirectory(BrowserFileInfo dir)
        {
            if (this.BackgroundWorker.IsBusy)
            {
                this.ViewModel.Status = "Busying loading directory";
            }
            else
            {
                this.ViewModel.BrowserState = BrowserState.Working;
                Log.InfoFormat("LoadDirectory, path={0}", dir);
                this.BackgroundWorker.RunWorkerAsync(dir);
            }
        }

        public void Refresh()
        {
            // refresh current directory
            Log.InfoFormat("Refresh");
            this.LoadDirectory(this.CurrentPath);
        }

        public bool CanTransferFile(BrowserFileInfo source, BrowserFileInfo target)
        {
            return this.FileTransferPresenter.CanTransferFile(source, target);
        }

        public void TransferFiles(FileTransferRequest fileTransferReqeust)
        {
            this.FileTransferPresenter.TransferFiles(fileTransferReqeust);
        }

        protected void OnAuthRequest(AuthEventArgs evt)
        {
            if (this.AuthRequest != null)
            {
                this.AuthRequest(this, evt);
            }
        }

        IBrowserModel Model { get; set; }
        IFileTransferPresenter FileTransferPresenter { get; set; }
        
        BackgroundWorker BackgroundWorker { get; set; }

        public IBrowserViewModel ViewModel { get; protected set; }
        public BrowserFileInfo CurrentPath { get; protected set; }
        public SessionData Session { get; protected set; }
    } 

    #endregion
}
