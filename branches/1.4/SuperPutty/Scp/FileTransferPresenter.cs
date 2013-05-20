using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using log4net;

namespace SuperPutty.Scp
{
    public class FileTransferPresenter : IFileTransferPresenter
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FileTransferPresenter));

        private IDictionary<int, FileTransfer> fileTranfers = new Dictionary<int, FileTransfer>();

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
            else if (target.Path == source.Path)
            {
                // can't drop on self
                canTransfer = false;
            }
            else if (source.Source == SourceType.Local)
            {
                // can't drop into own folder...no op
                string sourceDir = Path.GetFullPath(Path.GetDirectoryName(source.Path));
                string targetDir = Path.GetFullPath(target.Path);
                canTransfer = sourceDir != targetDir;
            }

            return canTransfer;
        }

        public void TransferFiles(FileTransferRequest transfer)
        {
        }

        public void Cancel(int id)
        {
            FileTransfer transfer = GetById(id);
            if (transfer != null)
            {
                if (!transfer.IsActive)
                {
                    throw new InvalidOperationException("Can not cancel inactive transfer, id=" + id);
                }
                transfer.Start();
            }
        }

        public void Remove(int id)
        {
            FileTransfer transfer = GetById(id);
            if (transfer != null)
            {
                if (transfer.IsActive)
                {
                    this.fileTranfers.Remove(id);
                }
                else
                {
                    Log.WarnFormat("Could not remove active FileTransfer, id={0}", id);
                }
            }
        }

        public void Restart(int id)
        {
            FileTransfer transfer = GetById(id);
            if (transfer != null)
            {
                if (transfer.IsActive)
                {
                    throw new InvalidOperationException("Can not restart active transfer, id=" + id);
                }
                transfer.Start();
            }
        }

        FileTransfer GetById(int id)
        {
            FileTransfer transfer;
            if (this.fileTranfers.TryGetValue(id, out transfer))
            {
                Log.WarnFormat("Could not get FileTransfer: id={0}", id);
            }
            return transfer;
        }

        public FileTransferViewModel ViewModel { get; private set; }


    }
}
