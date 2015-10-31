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
           // default value of localStartingDir moved to localPath in PscpBrowserPanel(SessionData session, PscpOptions options, string localStartingDir)            
           this(session, options, "")
        { }

        public PscpBrowserPanel(SessionData session, PscpOptions options, string localStartingDir) : this()
        {
            this.Name = session.SessionName;
            this.TabText = session.SessionName;

             //set the remote path
            String remotePath = "";            
            if (String.IsNullOrEmpty(session.RemotePath)){                
                remotePath = options.PscpHomePrefix + session.Username;
            }else{                
                remotePath = session.RemotePath;
            }

            //set the local path
            String localPath = "";
            if (String.IsNullOrEmpty(localStartingDir)){
                if (String.IsNullOrEmpty(session.LocalPath)){
                    localPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                }else{
                    localPath = session.LocalPath;
                }
            }else{
                localPath = localStartingDir;
            }
 		 

            this.fileTransferPresenter = new FileTransferPresenter(options);
            this.localBrowserPresenter = new BrowserPresenter(
                "Local", new LocalBrowserModel(), session, fileTransferPresenter);
            this.remoteBrowserPresenter = new BrowserPresenter(
                "Remote", new RemoteBrowserModel(options), session, fileTransferPresenter);

            this.browserViewLocal.Initialize(this.localBrowserPresenter, new BrowserFileInfo(new DirectoryInfo(localPath)));
            this.browserViewRemote.Initialize(this.remoteBrowserPresenter, RemoteBrowserModel.NewDirectory(remotePath));
            this.fileTransferView.Initialize(this.fileTransferPresenter);
        }
    }
}
