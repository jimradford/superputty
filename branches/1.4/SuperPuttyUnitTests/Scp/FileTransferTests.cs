using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SuperPutty.Scp;
using log4net;
using System.Threading;
using System.Windows.Forms;

namespace SuperPuttyUnitTests.Scp
{
    [TestFixture]
    public class FileTransferTests
    {

    }

    public class MockFileTransferPresenter : IFileTransferPresenter
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MockFileTransferPresenter));

        FileTransferPresenter filePresenter = new FileTransferPresenter();

        public MockFileTransferPresenter()
        {
            this.ViewModel = new FileTransferViewModel();
        }

        public bool CanTransferFile(BrowserFileInfo source, BrowserFileInfo target)
        {
            return filePresenter.CanTransferFile(source, target);
        }

        public void TransferFiles(FileTransferRequest transfer)
        {
            Log.InfoFormat("TransferFiles: {0}", transfer);
            this.LastRequest = transfer;
        }

        public FileTransferRequest LastRequest { get; set; }
        public FileTransferViewModel ViewModel { get; set; }


        public void Cancel(int id)
        {
            this.filePresenter.Cancel(id);
        }

        public void Remove(int id)
        {
            this.filePresenter.Remove(id);
        }

        public void Restart(int id)
        {
            this.filePresenter.Restart(id);
        }
    }

    public class FileTransferTestView
    {
        [TestView]
        public void RunView()
        {
            Form form = new Form();

            MockFileTransferPresenter presenter = new MockFileTransferPresenter();
            presenter.ViewModel.FileTransfers.Add(
                new FileTransferViewItem("Running", "Source", "Target")
                {
                    Progress = 50,
                    Start = DateTime.Now,
                    Message = "Chugging...",
                    Status = FileTransfer.Status.Running
                });
            presenter.ViewModel.FileTransfers.Add(
                new FileTransferViewItem("Done", "Source", "Target")
                {
                    Progress = 100,
                    Start = DateTime.Now,
                    Message = "Completed in 23.3s",
                    Status = FileTransfer.Status.Complete
                });
            presenter.ViewModel.FileTransfers.Add(
                new FileTransferViewItem("Done", "Source", "Target")
                {
                    Progress = 10,
                    Start = DateTime.Now,
                    Message = "Canceled",
                    Status = FileTransfer.Status.Canceled
                });

            FileTransferView view = new FileTransferView(presenter);
            view.Show();

            Thread thread = new Thread(() =>
            {
                int increment = 1;
                while (true)
                {
                    Thread.Sleep(100);
                    FileTransferViewItem item = presenter.ViewModel.FileTransfers[0];
                    if (item.Progress == 99 || item.Progress == 1)
                    {
                        increment *= -1;
                    }
                    item.Progress += increment;
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }
    }
}
