using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SuperPutty.Scp;
using log4net;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.ComponentModel;

namespace SuperPuttyUnitTests.Scp
{
    [TestFixture]
    public class LocalBrowserTests
    {
        static LocalBrowserTests()
        {
            Program.InitLoggingForUnitTests();
        }

        private static readonly ILog Log = LogManager.GetLogger(typeof(LocalBrowserTests));

        [Test]
        public void LocalViewModel()
        {
            BrowserViewModel viewModel = new BrowserViewModel();

            // test notify
            string updatedProp = null;
            viewModel.PropertyChanged += (s, e) =>  
            {
                Log.InfoFormat("PropertyChanged: {0}", e.PropertyName);
                updatedProp = e.PropertyName;
            };
            viewModel.Status = "foobar";
            Assert.AreEqual("Status", updatedProp);

            Assert.IsNotNull(viewModel.Files);
        }

        [Test]
        public void LocalListFiles()
        {
            BrowserPresenter presenter = new BrowserPresenter(new LocalBrowserModel(), null);
            IBrowserViewModel viewModel = presenter.ViewModel;

            // make mock dir
            String testDir = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
            File.Delete(testDir);
            Directory.CreateDirectory(Path.Combine(testDir, "A"));
            Directory.CreateDirectory(Path.Combine(testDir, "B"));
            Directory.CreateDirectory(Path.Combine(testDir, "C"));
            File.WriteAllText(Path.Combine(testDir, "file1"), "");
            File.WriteAllText(Path.Combine(testDir, "file2"), "");
            File.WriteAllText(Path.Combine(testDir, "file3"), "");
            File.WriteAllText(Path.Combine(testDir, "file4"), "1");

            // change to mode dir
            presenter.ViewModel.PropertyChanged+= (s, e) => 
            {
                if (presenter.ViewModel.BrowserState == BrowserState.Ready)
                {
                    lock (this) { Monitor.Pulse(this);  }
                }
            };
            presenter.LoadDirectory(testDir);
            lock (this)
            {
                Monitor.Wait(this, 1000);
            }

            foreach (BrowserFileInfo bfi in presenter.ViewModel.Files)
            {
                Log.InfoFormat("BFI: {0}", bfi);
            }
            Assert.IsNotNull(viewModel.Files);
            Assert.AreEqual(8, viewModel.Files.Count);
            Assert.AreEqual("..", viewModel.Files[0].Name);
            Assert.AreEqual(new DirectoryInfo(Path.GetTempPath()), new DirectoryInfo(viewModel.Files[0].Path));
            Assert.AreEqual(FileType.ParentDirectory, viewModel.Files[0].Type);
            Assert.AreEqual("A", viewModel.Files[1].Name);
            Assert.AreEqual(FileType.Directory, viewModel.Files[1].Type);
            Assert.AreEqual(0, viewModel.Files[1].Size);

            Assert.AreEqual("file1", viewModel.Files[4].Name, "file1");
            Assert.AreEqual(FileType.File, viewModel.Files[4].Type);
            Assert.AreEqual(0, viewModel.Files[4].Size);

            Assert.AreEqual(1, viewModel.Files[7].Size);
        }


        [TestView]
        public void TestGUI()
        {
            Log.Error("test", null);
            BrowserPresenter presenter = new BrowserPresenter(new LocalBrowserModel(), null);

            BrowserView view = new BrowserView(presenter, Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
            view.Dock = DockStyle.Fill;

            Form form = new Form();
            form.Size = new Size(600, 800);
            form.Controls.Add(view);
            form.ShowDialog();
        }

    }
}
