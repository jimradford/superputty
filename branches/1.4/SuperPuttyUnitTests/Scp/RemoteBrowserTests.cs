using System;
using NUnit.Framework;
using SuperPutty.Scp;
using System.Windows.Forms;
using System.Drawing;
using SuperPutty.Data;

namespace SuperPuttyUnitTests.Scp
{
    [TestFixture]
    public class RemoteBrowserTests
    {
        static RemoteBrowserTests()
        {
            Program.InitLoggingForUnitTests();
        }

        [Test]
        public void ParserTimestampYearMode()
        {
            // May 15 12:32, Mar  3 03:37, Nov 18 18:19, May 27  2012
            PscpClient.ScpLineParser parser = new PscpClient.ScpLineParser();
            DateTime dt;

            Assert.True(parser.TryParseTimestamp("May 15 2012", out dt));
            Assert.AreEqual(2012, dt.Year);
            Assert.AreEqual(5, dt.Month);
            Assert.AreEqual(15, dt.Day);
            Assert.AreEqual(0, dt.Hour);
            Assert.AreEqual(0, dt.Minute);
            Assert.AreEqual(DateTimeKind.Local, dt.Kind);

            Assert.True(parser.TryParseTimestamp("May  5 2012", out dt));
            Assert.AreEqual(2012, dt.Year);
            Assert.AreEqual(5, dt.Month);
            Assert.AreEqual(5, dt.Day);
            Assert.AreEqual(0, dt.Hour);
            Assert.AreEqual(0, dt.Minute);
            Assert.AreEqual(DateTimeKind.Local, dt.Kind);
        }

        [Test]
        public void ParserTimestampTimeMode()
        {
            PscpClient.ScpLineParser parser = new PscpClient.ScpLineParser();
            DateTime dt;

            Assert.True(parser.TryParseTimestamp("May 15 12:32", out dt));
            Assert.AreEqual(DateTime.Now.Year, dt.Year);
            Assert.AreEqual(5, dt.Month);
            Assert.AreEqual(15, dt.Day);
            Assert.AreEqual(12, dt.Hour);
            Assert.AreEqual(32, dt.Minute);
            Assert.AreEqual(DateTimeKind.Local, dt.Kind);

            Assert.True(parser.TryParseTimestamp("Mar  3 03:37", out dt));
            Assert.AreEqual(DateTime.Now.Year, dt.Year);
            Assert.AreEqual(3, dt.Month);
            Assert.AreEqual(3, dt.Day);
            Assert.AreEqual(3, dt.Hour);
            Assert.AreEqual(37, dt.Minute);
            Assert.AreEqual(DateTimeKind.Local, dt.Kind);

            Assert.True(parser.TryParseTimestamp("Nov 18 18:19", out dt));
            Assert.AreEqual(DateTime.Now.Year, dt.Year);
            Assert.AreEqual(11, dt.Month);
            Assert.AreEqual(18, dt.Day);
            Assert.AreEqual(18, dt.Hour);
            Assert.AreEqual(19, dt.Minute);
            Assert.AreEqual(DateTimeKind.Local, dt.Kind);
        }

        [TestView]
        public void TestGUI()
        {
            Form form = new Form();
            form.Size = new Size(600, 800);

            SessionData session = new SessionData
            {
                Username = ScpConfig.UserName,
                Password = ScpConfig.Password, 
                Host = ScpConfig.KnownHost, 
                Port = 22
            };

            BrowserPresenter presenter = new BrowserPresenter(
                new RemoteBrowserModel(ScpConfig.PscpLocation), 
                session, 
                new MockFileTransferPresenter());

            BrowserView view = new BrowserView(
                presenter,
                RemoteBrowserModel.NewDirectory("/home/" + ScpConfig.UserName));
            view.Dock = DockStyle.Fill;

            form.Controls.Add(view);
            form.ShowDialog();
        }

    }
}
