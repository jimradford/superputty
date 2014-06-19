using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperPutty.Data;
using log4net;
using System.Threading;

namespace SuperPutty.Scp
{
    #region FileTransfer
    public class FileTransfer
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FileTransfer));
        private static int idSeed = 0;

        public event EventHandler Update;

        private Thread thread = null;
        Status status = Status.Initializing;

        public FileTransfer(PscpOptions options, FileTransferRequest request)
        {
            this.Options = options;
            this.Request = request;

            this.Id = Interlocked.Increment(ref idSeed);
        }

        public void Start()
        {
            lock (this)
            {
                if (this.TransferStatus == Status.Initializing || CanRestart(this.TransferStatus))
                {
                    Log.InfoFormat("Starting transfer, id={0}", this.Id);

                    this.StartTime = DateTime.Now;

                    this.thread = new Thread(this.DoTransfer);
                    this.thread.IsBackground = false;
                    this.thread.Start();

                    this.UpdateStatus(0, Status.Running, "Started transfer");
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
                if (CanCancel(this.TransferStatus))
                {
                    Log.InfoFormat("Canceling active transfer, id={0}", this.Id);
                    this.thread.Abort();
                    Log.InfoFormat("Canceled active transfer, id={0}", this.Id);
                    this.UpdateStatus(this.PercentComplete, Status.Canceled, "Canceled");
                }
                else
                {
                    Log.WarnFormat("Attempted to cancel inactive transfer, id={0}", this.Id);
                }
            }
        }

        void DoTransfer()
        {
            try
            {
                PscpClient client = new PscpClient(this.Options, this.Request.Session);

                int estSizeKB = Int32.MaxValue;
                FileTransferResult res = client.CopyFiles(
                    this.Request.SourceFiles,
                    this.Request.TargetFile,
                    (complete, cancelAll, s) =>
                    {
                        string msg;
                        if (s.PercentComplete > 0)
                        {
                            estSizeKB = Math.Min(estSizeKB, s.BytesTransferred * 100 / s.PercentComplete);
                            string units = estSizeKB > 1024 * 10 ? "MB" : "KB";
                            int divisor = units == "MB" ? 1024 : 1;
                            msg = string.Format(
                                "{0}, ({1} of {2} {3}, {4})",
                                s.Filename,
                                s.BytesTransferred / divisor,
                                estSizeKB / divisor,
                                units,
                                s.TimeLeft);
                        }
                        else
                        {
                            // < 1% completed
                            msg = string.Format("{0}, ({1} KB, {2})", s.Filename, s.BytesTransferred, s.TimeLeft);
                        }
                        this.UpdateStatus(s.PercentComplete, Status.Running, msg);
                    });

                this.EndTime = DateTime.Now;
                switch (res.StatusCode)
                {
                    case ResultStatusCode.Success:
                        double duration = (EndTime.Value - StartTime.Value).TotalSeconds;
                        this.UpdateStatus(100, Status.Complete, String.Format("Duration {0:#,###} s", duration));
                        break;
                    case ResultStatusCode.RetryAuthentication:
                    case ResultStatusCode.Error:
                        this.UpdateStatus(this.PercentComplete, Status.Error, res.ErrorMsg);
                        break;
                }
            }
            catch (ThreadAbortException)
            {
                this.UpdateStatus(this.PercentComplete, Status.Canceled, "");
            }
            catch (Exception ex)
            {
                Log.Error("Error running transfer, id=" + this.Id, ex);
                this.UpdateStatus(0, Status.Error, ex.Message);
            }
        }

        void UpdateStatus(int percentageComplete, Status status, string message)
        {
            this.PercentComplete = percentageComplete;
            this.TransferStatus = status;
            this.TransferStatusMsg = message;
            if (this.Update != null)
            {
                this.Update(this, EventArgs.Empty);
            }
        }

        public static bool CanRestart(Status status)
        {
            return status == Status.Complete || status == Status.Canceled || status == Status.Error;
        }

        public static bool CanCancel(Status status)
        {
            return status == Status.Running;
        }

        public PscpOptions Options { get; private set; }
        public FileTransferRequest Request { get; private set; }
        public int Id { get; private set; }

        public Status TransferStatus
        {
            get { lock (this) { return this.status; } }
            private set { lock (this) { this.status = value; } }
        }

        public int PercentComplete { get; private set; }
        public string TransferStatusMsg { get; private set; }
        public DateTime? StartTime { get; private set; }
        public DateTime? EndTime { get; private set; }

        public enum Status
        {
            Initializing,
            Running,
            Complete,
            Error,
            Canceled
        }
    } 
    #endregion

    #region FileTransferRequest
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
    #endregion
}
