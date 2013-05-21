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
    #region FileTransferViewModel
    /// <summary>
    /// UI view adapter.
    /// Must be created on GUI thread to pickup proper context for notifications to be 
    /// auto-marshalled properly
    /// </summary>
    public class FileTransferViewModel : BaseViewModel
    {
        public FileTransferViewModel()
        {
            this.FileTransfers = new SortableBindingList<FileTransferViewItem>();
            this.Context = SynchronizationContext.Current;
        }

        public int FindIndexById(int id)
        {
            int idx = -1;
            for (int i = 0; i < this.FileTransfers.Count; i++)
            {
                FileTransferViewItem item = this.FileTransfers[i];
                if (item.Id == id)
                {
                    idx = i;
                    break;
                }
            }
            return idx;
        }

        public BindingList<FileTransferViewItem> FileTransfers { get; set; }
    } 
    #endregion

    #region FileTransferViewItem
    /// <summary>
    /// Converts FileTransfer
    /// </summary>
    public class FileTransferViewItem 
    {
        public FileTransferViewItem() 
        {
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
            this.Start = DateTime.Now;
        }

        public int Id { get; private set; }
        public string Session { get; private set; }
        public string Source { get; private set; }
        public string Target { get; private set; }

        private FileTransfer Transfer { get; set; }

        public DateTime? Start { get;set;}
        public DateTime? End {get;set;}
        public int Progress {get;set;}
        public FileTransfer.Status Status { get;set;}
        public string Message { get; set; }

        public bool CanRestart { get; set; }
        public bool CanCancel { get; set; }
        public bool CanDelete { get; set; }

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
    }
    
    #endregion
}