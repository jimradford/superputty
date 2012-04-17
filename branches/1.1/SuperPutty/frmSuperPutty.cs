/*
 * Copyright (c) 2009 Jim Radford http://www.jimradford.com
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions: 
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using WeifenLuo.WinFormsUI.Docking;
using SuperPutty.Properties;
using SuperPutty.Data;
using log4net;
using System.Reflection;

namespace SuperPutty
{
    public partial class frmSuperPutty : Form
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(frmSuperPutty));

        public static string PuttyExe
        {
            get { return SuperPuTTY.Settings.PuttyExe; }
        }

        public static string PscpExe
        {
            get { return SuperPuTTY.Settings.PscpExe; }
        }

        public static bool IsScpEnabled
        {
            get { return File.Exists(PscpExe); }
        }

        private SessionTreeview m_Sessions;
        private LayoutsList m_Layouts;
        private Log4netLogViewer m_logViewer = null;

        public frmSuperPutty()
        {
            // Verify Putty is set; Prompt user if necessary; exit otherwise
            dlgFindPutty.PuttyCheck();
            
            InitializeComponent();

            /* 
             * Open the session treeview and dock it on the right
             */
            m_Sessions = new SessionTreeview(dockPanel1);
            m_Sessions.CloseButtonVisible = false;

            m_Layouts = new LayoutsList();
            m_Layouts.CloseButtonVisible = false;

            //m_Sessions.Show(dockPanel1, WeifenLuo.WinFormsUI.Docking.DockState.DockRight);

            // Hook into status
            SuperPuTTY.StatusEvent += new Action<string>(delegate(String msg) { this.toolStripStatusLabelMessage.Text = msg; });
            SuperPuTTY.ReportStatus("Ready");

            // Hook into LayoutChanging/Changed
            SuperPuTTY.LayoutChanging += new EventHandler<LayoutChangedEventArgs>(SuperPuTTY_LayoutChanging);

            this.toolStripStatusLabelVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }


        private void frmSuperPutty_Load(object sender, EventArgs e)
        {
            this.BeginInvoke(new Action(this.LoadLayout));
        }

        /// <summary>
        /// Handles focusing on tabs/windows which host PuTTY
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dockPanel1_ActiveDocumentChanged(object sender, EventArgs e)
        {
            if (dockPanel1.ActiveDocument is ctlPuttyPanel)
            {
                ctlPuttyPanel p = (ctlPuttyPanel)dockPanel1.ActiveDocument;
                p.SetFocusToChildApplication();
            }
        }


        private void frmSuperPutty_Activated(object sender, EventArgs e)
        {
            //dockPanel1_ActiveDocumentChanged(null, null);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "XML Files|*.xml";
            saveDialog.FileName = "Sessions.XML";
            saveDialog.InitialDirectory = Application.StartupPath;
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                SessionTreeview.ExportSessionsToXml(saveDialog.FileName);
            }
        }

        private void importSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "XML Files|*.xml";
            openDialog.FileName = "Sessions.XML";
            openDialog.CheckFileExists = true;
            openDialog.InitialDirectory = Application.StartupPath;
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                SessionTreeview.ImportSessionsFromXml(openDialog.FileName);
                m_Sessions.LoadSessions();
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmSuperPutty_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Exit SuperPuTTY?", "Confirm Exit", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        #region Layout

        void LoadLayout()
        {
            String dir = SuperPuTTY.LayoutsDir;
            if (Directory.Exists(dir))
            {
                this.openFileDialogLayout.InitialDirectory = dir;
                this.saveFileDialogLayout.InitialDirectory = dir;
            }

            if (SuperPuTTY.StartingLayout != null)
            {
                SuperPuTTY.LoadLayout(SuperPuTTY.StartingLayout);
            }
        }

        void SuperPuTTY_LayoutChanging(object sender, LayoutChangedEventArgs eventArgs)
        {
            if (eventArgs.IsNewLayoutAlreadyActive)
            {
                toolStripStatusLabelLayout.Text = eventArgs.New.Name;
            }
            else
            {
                // reset old layout (close old putty instances)
                foreach (DockContent dockContent in this.dockPanel1.DocumentsToArray())
                {
                    Log.Debug("Unhooking document: " + dockContent);
                    dockContent.DockPanel = null;
                    // close old putty
                    if (dockContent.CloseButtonVisible)
                    {
                        dockContent.Close();
                    }
                }
                List<DockContent> contents = new List<DockContent>();
                foreach (DockContent dockContent in this.dockPanel1.Contents)
                {
                    contents.Add(dockContent);
                }
                foreach (DockContent dockContent in contents)
                {
                    Log.Debug("Unhooking dock content: " + dockContent);
                    dockContent.DockPanel = null;
                    // close non-persistant windows
                    if (dockContent.CloseButtonVisible)
                    {
                        dockContent.Close();
                    }
                }


                if (eventArgs.New == null)
                {
                    // 1st time or reset
                    Log.Debug("Initializing default layout");
                    m_Sessions.Show(dockPanel1, WeifenLuo.WinFormsUI.Docking.DockState.DockRight);
                    m_Layouts.Show(m_Sessions.DockHandler.Pane, DockAlignment.Bottom, 0.5);
                    toolStripStatusLabelLayout.Text = "";
                    SuperPuTTY.ReportStatus("Initialized default layout");
                }
                else
                {
                    // load new one
                    Log.DebugFormat("Loading layout: {0}", eventArgs.New.FilePath);
                    this.dockPanel1.LoadFromXml(eventArgs.New.FilePath, RestoreLayoutFromPersistString);
                    toolStripStatusLabelLayout.Text = eventArgs.New.Name;
                    SuperPuTTY.ReportStatus("Loaded layout: {0}", eventArgs.New.FilePath);
                }

                // after all is done, cause a repaint to 
            }
        }

        private void saveLayoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SuperPuTTY.CurrentLayout != null)
            {
                String file = SuperPuTTY.CurrentLayout.FilePath;
                SuperPuTTY.ReportStatus("Saving layout: {0}", file);
                this.dockPanel1.SaveAsXml(file);
            }
            else
            {
                saveLayoutAsToolStripMenuItem_Click(sender, e);
            }
        }

        private void saveLayoutAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == this.saveFileDialogLayout.ShowDialog(this))
            {
                String file = this.saveFileDialogLayout.FileName;
                SuperPuTTY.ReportStatus("Saving layout as: {0}", file);
                this.dockPanel1.SaveAsXml(file);
                SuperPuTTY.AddLayout(file);
            } 
        }

        private IDockContent RestoreLayoutFromPersistString(String persistString)
        {
            if (typeof(SessionTreeview).FullName == persistString)
            {
                // session tree
                return this.m_Sessions;
            }
            else if (typeof(LayoutsList).FullName == persistString)
            {
                // session tree
                return this.m_Layouts;
            }
            else if (typeof(Log4netLogViewer).FullName == persistString)
            {
                InitLogViewer();
                return this.m_logViewer;
            }
            else
            {
                // putty session
                ctlPuttyPanel puttyPanel = ctlPuttyPanel.FromPersistString(m_Sessions, persistString);
                if (puttyPanel != null)
                {
                    return puttyPanel;
                }

                /*
                int idx = persistString.IndexOf(":");
                if (idx != -1)
                {
                    string sessionName = persistString.Substring(idx + 1);
                    Log.InfoFormat("Restoring putty session, {0}", sessionName);
                    ctlPuttyPanel puttyPanel = m_Sessions.NewPuttyPanel(sessionName);
                    return puttyPanel;
                }
                 */
            }
            return null;
        }


        #endregion

        #region Tools

        private void puTTYConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process p = new Process();
            p.StartInfo.FileName = PuttyExe;
            p.Start();

            SuperPuTTY.ReportStatus("Lauched Putty Configuration");
        }

        private void logViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //DebugLogViewer logView = new DebugLogViewer();
            //logView.Show(dockPanel1, WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide);
            if (this.m_logViewer == null)
            {
                InitLogViewer();
                this.m_logViewer.Show(dockPanel1, WeifenLuo.WinFormsUI.Docking.DockState.DockBottom);
                SuperPuTTY.ReportStatus("Showing Log Viewer");
            }
            else
            {
                this.m_logViewer.Show(dockPanel1);
                SuperPuTTY.ReportStatus("Bringing Log Viewer to Front");
            }

        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SuperPuTTY.ReportStatus("Editing Options");

            dlgFindPutty dialog = new dlgFindPutty();
            dialog.ShowDialog();

            SuperPuTTY.ReportStatus("Ready");
        }

        void InitLogViewer()
        {
            if (this.m_logViewer == null)
            {
                this.m_logViewer = new Log4netLogViewer();
                this.m_logViewer.FormClosed += delegate { this.m_logViewer = null; };
            }
        }

        #endregion

        #region Help Menu
        private void aboutSuperPuttyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 about = new AboutBox1();
            about.ShowDialog();
            about = null;
        }

        private void superPuttyWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://code.google.com/p/superputty/");
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (File.Exists(Application.StartupPath + @"\superputty.chm"))
            {
                Process.Start(Application.StartupPath + @"\superputty.chm");
            }
            else
            {
                DialogResult result = MessageBox.Show("Local documentation could not be found. Would you like to view the documentation online instead?", "Documentation Not Found", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    Process.Start("http://code.google.com/p/superputty/wiki/Documentation");
                }
            }
        }

        private void puTTYScpLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dlgFindPutty dialog = new dlgFindPutty();
            dialog.ShowDialog();
        }
        #endregion

    }
}
