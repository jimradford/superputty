using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SuperPutty.Scp;
using log4net;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using SuperPutty.Data;
using System.IO;

namespace SuperPuttyUnitTests.Scp
{
    [TestFixture]
    public class FileTransferTests
    {

    }

    public class MockFileTransferPresenter : IFileTransferPresenter
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MockFileTransferPresenter));

        FileTransferPresenter filePresenter = new FileTransferPresenter(ScpConfig.DefaultOptions);

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
        public PscpOptions Options { get; set; }


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
        public void RunMockView()
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

            FileTransferView view = new FileTransferView(presenter) { Dock = DockStyle.Fill };
            form.Controls.Add(view);
            form.Size = new Size(1024, 250);
            form.Show();

            Thread thread = new Thread(() =>
            {
                Thread.Sleep(1000);
                // simulate updates
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

                    if (view.IsDisposed) break;

                    view.BeginInvoke(new Action(() => presenter.ViewModel.FileTransfers.ResetItem(0)), null);
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }

        [TestView]
        public void RunCombinedView()
        {
            Form form = new Form();

            SessionData session = new SessionData
            {
                SessionId = "Test/SessionId",
                SessionName = "Test SessionName",
                Username = ScpConfig.UserName,
                Password = ScpConfig.Password, 
                Host = ScpConfig.KnownHost, 
                Port = 22
            };

            FileTransferPresenter fileTransferPresenter = new FileTransferPresenter(ScpConfig.DefaultOptions);

            FileTransferView fileTransferView = new FileTransferView(fileTransferPresenter) { Dock = DockStyle.Bottom };
            BrowserView localBrowserView = new BrowserView(
                new BrowserPresenter("Local", new LocalBrowserModel(), session, fileTransferPresenter), 
                new BrowserFileInfo(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop))));
            localBrowserView.Dock = DockStyle.Fill;
            
            BrowserView remoteBrowserView = new BrowserView(
                new BrowserPresenter("Remote", new RemoteBrowserModel(ScpConfig.DefaultOptions), session, fileTransferPresenter),
                RemoteBrowserModel.NewDirectory("/home/" + ScpConfig.UserName));
            remoteBrowserView.Dock = DockStyle.Fill;

            SplitContainer browserPanel = new SplitContainer() 
            { 
                Dock = DockStyle.Fill, 
                SplitterDistance = 75, 
            };
            browserPanel.Panel1.Controls.Add(localBrowserView);
            browserPanel.Panel2.Controls.Add(remoteBrowserView);

            form.Controls.Add(browserPanel);
            form.Controls.Add(fileTransferView);
            form.Size = new Size(1024, 768);
            form.Show();
        }

        [TestView]
        public void RunPscpBrowserPanel()
        {
            SessionData session = new SessionData
            {
                SessionId = "Test/SessionId",
                SessionName = "Test SessionName",
                Username = ScpConfig.UserName,
                Password = ScpConfig.Password,
                Host = ScpConfig.KnownHost,
                Port = 22
            };

            PscpBrowserPanel panel = new PscpBrowserPanel(session, ScpConfig.DefaultOptions);
            panel.Size = new Size(1024, 768);
            panel.Show();
        }
    }
}
