using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SuperPutty.Data;

namespace SuperPutty.Scp
{
    public partial class PscpBrowserPanel : ToolWindowDocument
    {
        FileTransferPresenter fileTransferPresenter;
        IBrowserPresenter localBrowserPresenter;
        IBrowserPresenter remoteBrowserPresenter;

        public PscpBrowserPanel()
        {
            InitializeComponent();
        }

        public PscpBrowserPanel(SessionData session, PscpOptions options) :
            this(session, options, Environment.GetFolderPath(Environment.SpecialFolder.Desktop))
        { }

        public PscpBrowserPanel(SessionData session, PscpOptions options, string localStartingDir) : this()
        {
            this.Name = session.SessionName;
            this.TabText = session.SessionName;

            this.fileTransferPresenter = new FileTransferPresenter(options);
            this.localBrowserPresenter = new BrowserPresenter(
                "Local", new LocalBrowserModel(), session, fileTransferPresenter);
            this.remoteBrowserPresenter = new BrowserPresenter(
                "Remote", new RemoteBrowserModel(options), session, fileTransferPresenter);

            this.browserViewLocal.Initialize(this.localBrowserPresenter, new BrowserFileInfo(new DirectoryInfo(localStartingDir)));
            this.browserViewRemote.Initialize(this.remoteBrowserPresenter, RemoteBrowserModel.NewDirectory("/home/" + session.Username));
            this.fileTransferView.Initialize(this.fileTransferPresenter);
        }
    }
}
