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
        public BrowserPresenter(IBrowserModel model, SessionData session)
        {
            this.Model = model;
            this.Session = session;

            this.BackgroundWorker = new BackgroundWorker();
            this.BackgroundWorker.WorkerReportsProgress = true;
            this.BackgroundWorker.WorkerSupportsCancellation = true;
            this.BackgroundWorker.DoWork += (BackgroundWorker_DoWork);
            this.BackgroundWorker.ProgressChanged += (BackgroundWorker_ProgressChanged);
            this.BackgroundWorker.RunWorkerCompleted += (BackgroundWorker_RunWorkerCompleted);

            this.ViewModel = new BrowserViewModel();
            this.ViewModel.BrowserState = BrowserState.Ready;
        }

        #region Async Work

        void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.ViewModel.Status = (string) e.UserState;
        }

        void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string path = (string) e.Argument;

            this.BackgroundWorker.ReportProgress(5, "Requesting files...");
            ListDirectoryResult result = this.Model.ListDirectory(this.Session, new BrowserFileInfo { Path = path });
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
                        this.ViewModel.CurrentPath = result.Path;
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

        public void LoadDirectory(string path)
        {
            if (this.BackgroundWorker.IsBusy)
            {
                this.ViewModel.Status = "Busying loading directory";
            }
            else
            {
                this.ViewModel.BrowserState = BrowserState.Working;
                Log.InfoFormat("LoadDirectory, path={0}", path);
                this.BackgroundWorker.RunWorkerAsync(path);
            }
        }

        public void Refresh()
        {
            // refresh current directory
            this.LoadDirectory(this.ViewModel.CurrentPath);
        }

        /*
        public void LoadDirectorySync(string path)
        {
            Log.InfoFormat("LoadDirectory, path={0}", path);

            ListDirectoryResult result = this.Model.ListDirectory(new BrowserFileInfo { Path = path });

            if (result.StatusCode == ResultStatusCode.Success)
            {   
                string msg = result.MountCount > 0
                    ? string.Format("{0} items", result.MountCount)
                    : string.Format("{0} files {1} directories", result.FileCount, result.DirCount);
                
                this.ViewModel.Status = string.Format("{0} @ {1}", msg, DateTime.Now);
                this.ViewModel.CurrentPath = path;
                BrowserViewModel.UpdateList(this.ViewModel.Files, result.Files);
            }
            else if (result.ErrorMsg != null)
            {
                Log.Error(result.ErrorMsg);
                this.ViewModel.Status = result.ErrorMsg;
            }
            else if (result.Error != null)
            {
                string msg = string.Format("Error loading directory: {0}", result.Error.Message);
                Log.Error(msg, result.Error);
                this.ViewModel.Status = msg;
            }
        }*/


        protected void OnAuthRequest(AuthEventArgs evt)
        {
            if (this.AuthRequest != null)
            {
                this.AuthRequest(this, evt);
            }
        }

        IBrowserModel Model { get; set; }
        SessionData Session { get; set; }
        BackgroundWorker BackgroundWorker { get; set; }
        public IBrowserViewModel ViewModel { get; protected set; }
    } 

    #endregion
}
