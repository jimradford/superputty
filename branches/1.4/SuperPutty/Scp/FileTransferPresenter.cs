using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using log4net;

namespace SuperPutty.Scp
{
    #region IFileTransferPresenter
    public interface IFileTransferPresenter
    {
        bool CanTransferFile(BrowserFileInfo source, BrowserFileInfo target);
        void TransferFiles(FileTransferRequest transfer);
        void Cancel(int id);
        void Remove(int id);
        void Restart(int id);
        FileTransferViewModel ViewModel { get; }
        PscpOptions Options { get; }
    }
    #endregion

    #region FileTransferPresenter
    public class FileTransferPresenter : IFileTransferPresenter
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FileTransferPresenter));

        private IDictionary<int, FileTransfer> fileTranfers = new Dictionary<int, FileTransfer>();

        public FileTransferPresenter(PscpOptions options)
        {
            this.Options = options;
            this.ViewModel = new FileTransferViewModel();
        }

        public bool CanTransferFile(BrowserFileInfo source, BrowserFileInfo target)
        {
            //    Source        Target   Result
            // 1) WindowsFile   Local    if not the same, File.Copy
            // 2) Local         Local    File.Copy
            // 3) Remote        Local    RemoteCopy
            bool canTransfer = true;

            if (target.Type == FileType.File || target.Type == FileType.ParentDirectory)
            {
                // can't drop on file or parent dir (..)
                canTransfer = false;
            }
            else if (target.Source == source.Source)
            {
                // can't drop local-on-local or remote-on-remote
                canTransfer = false;
            }
            else if (target.Path == source.Path)
            {
                // can't drop on self
                canTransfer = false;
            }
            /*  Future...if/when local-on-local allowed
            else if (source.Source == SourceType.Local)
            {
                // can't drop into own folder...no op
                string sourceDir = Path.GetFullPath(Path.GetDirectoryName(source.Path));
                string targetDir = Path.GetFullPath(target.Path);
                canTransfer = sourceDir != targetDir;
            }*/
            return canTransfer;
        }

        public void TransferFiles(FileTransferRequest request)
        {
            // Expand requests, if needed
            List<FileTransfer> transfers = new List<FileTransfer>();
            if (request.TargetFile.Source == SourceType.Local)
            {
                // remote-to-local: Create 1 transfer per source as pscp only allows 1 remote file per op
                foreach (BrowserFileInfo source in request.SourceFiles)
                {
                    FileTransferRequest req = new FileTransferRequest
                    {
                        TargetFile = request.TargetFile,
                        Session = request.Session
                    };
                    req.SourceFiles.Add(source);
                    transfers.Add(new FileTransfer(this.Options, req));
                }
            }
            else
            {
                // local-to-remote: Create 1 transfer for all
                transfers.Add(new FileTransfer(this.Options, request));
            }

            // Add and start each transfer
            foreach (FileTransfer transfer in transfers)
            {
                AddTransfer(transfer);
                transfer.Start();
            }
        }

        void AddTransfer(FileTransfer transfer)
        {
            // store
            this.fileTranfers.Add(transfer.Id, transfer);

            // notify
            this.ViewModel.FileTransfers.Add(new FileTransferViewItem(transfer));

            // hook for updates
            transfer.Update += (transfer_Update);
        }

        void transfer_Update(object sender, EventArgs e)
        {
            FileTransfer transfer = (FileTransfer)sender;
            if (this.ViewModel.Context != null)
            {
                this.ViewModel.Context.Post((x) => ProcessTransferUpdate(transfer), e);
            }
            else
            {
                ProcessTransferUpdate(transfer);
            }
        }

        void ProcessTransferUpdate(FileTransfer transfer)
        {

            int idx = this.ViewModel.FindIndexById(transfer.Id);
            if (idx == -1)
            {
                Log.WarnFormat("Could not notify ViewModel, id={0}", transfer.Id);
            }
            else
            {
                // update items
                FileTransferViewItem viewItem = this.ViewModel.FileTransfers[idx];
                viewItem.Progress = transfer.PercentComplete;
                viewItem.Status = transfer.TransferStatus;
                viewItem.Message = transfer.TransferStatusMsg;
                viewItem.Start = transfer.StartTime;
                viewItem.End = transfer.EndTime;
                viewItem.CanCancel = CanCancel(viewItem.Status);
                viewItem.CanRestart = CanRestart(viewItem.Status);
                viewItem.CanDelete = CanRestart(viewItem.Status);

                // notify on update
                this.ViewModel.FileTransfers.ResetItem(idx);
            }
        }

        bool CanRestart(FileTransfer.Status status)
        {
            return status == FileTransfer.Status.Complete || status == FileTransfer.Status.Canceled;
        }

        bool CanCancel(FileTransfer.Status status)
        {
            return status == FileTransfer.Status.Running;
        }

        public void Remove(int id)
        {
            FileTransfer transfer = GetById(id);
            if (transfer != null)
            {
                if (CanRestart(transfer.TransferStatus))
                {
                    this.fileTranfers.Remove(id);
                    transfer.Update -= (transfer_Update);
                    int idx = this.ViewModel.FindIndexById(id);
                    if (idx != -1)
                    {
                        this.ViewModel.FileTransfers.RemoveAt(idx);
                    }
                }
                else
                {
                    Log.WarnFormat("Could not remove active FileTransfer, id={0}", id);
                }
            }
        }

        public void Cancel(int id)
        {
            FileTransfer transfer = GetById(id);
            if (transfer != null)
            {
                if (CanCancel(transfer.TransferStatus))
                {
                    transfer.Cancel();
                }
                else
                {
                    throw new InvalidOperationException("Can not cancel inactive transfer, id=" + id);
                }
            }
        }

        public void Restart(int id)
        {
            FileTransfer transfer = GetById(id);
            if (transfer != null)
            {
                if (CanRestart(transfer.TransferStatus))
                {
                    transfer.Start();
                }
                else
                {
                    throw new InvalidOperationException("Can not restart active transfer, id=" + id);
                }
            }
        }

        FileTransfer GetById(int id)
        {
            FileTransfer transfer;
            if (!this.fileTranfers.TryGetValue(id, out transfer))
            {
                Log.WarnFormat("Could not get FileTransfer: id={0}", id);
            }
            return transfer;
        }

        public FileTransferViewModel ViewModel { get; private set; }
        public PscpOptions Options { get; private set; }

    } 
    #endregion
}
