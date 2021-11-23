using log4net;
using Renci.SshNet;
using SuperPutty.Data;
using SuperPutty.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuperPutty
{
    public partial class ctlSshNetPanel : ToolWindowDocument
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ctlSshNetPanel));

        public SessionData Session { get; set; }
        private PuttyStartInfo m_puttyStartInfo;
        private PuttyClosedCallback m_ApplicationExit;
        public string TextOverride { get; set; }

        public ctlSshNetPanel(SessionData session, PuttyClosedCallback callback)
        {
            Session = session;
            m_ApplicationExit = callback;
            m_puttyStartInfo = new PuttyStartInfo(session);

            InitializeComponent();

            this.Text = session.SessionName;
            this.TabText = session.SessionName;
            this.TextOverride = session.SessionName;

            CreatePanel();
            //AdjustMenu();           
        }

        private void CreatePanel()
        {
            Session.Password = "something";
            Renci.SshNet.SshClient sshClient = new SshClient(Session.Host, Session.Username, Session.Password);            
            try
            {
                sshClient.Connect();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            var shell = sshClient.CreateShellStream("Test", 80, 50, 800, 600, 1024);
            shell.DataReceived += Shell_DataReceived;

            shell.WriteAsync(Encoding.UTF8.GetBytes("ls"), 0, 2);
        }

        private void Shell_DataReceived(object sender, Renci.SshNet.Common.ShellDataEventArgs e)
        {
            Console.WriteLine("Line: {0} Data: {1}", e.Line, Encoding.UTF8.GetString(e.Data));
        }
    }
}
