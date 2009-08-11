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

namespace SuperPutty
{
    public partial class frmSuperPutty : Form
    {
        private static string _PuttyExe;

        public static string PuttyExe
        {
            get { return _PuttyExe; }
            set
            {
                _PuttyExe = value;

                if (File.Exists(value))
                {
                    RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Jim Radford\SuperPuTTY\Settings");
                    key.SetValue("PuTTYExe", value);
                }
            }
        }

        private static string _PscpExe;

        public static string PscpExe
        {
            get { return _PscpExe; }
            set
            {
                _PscpExe = value;

                if (File.Exists(value))
                {
                    RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Jim Radford\SuperPuTTY\Settings");
                    key.SetValue("PscpExe", value);
                }
            }
        }

        public static bool IsScpEnabled
        {
            get { return File.Exists(PscpExe); }
        }

        private SessionTreeview m_Sessions;

        public frmSuperPutty()
        {
            // Get Registry Entry for Putty Exe
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Jim Radford\SuperPuTTY\Settings");
            if (key != null)
            {
                string puttyExe = key.GetValue("PuTTYExe", "").ToString();
                if (File.Exists(puttyExe))
                {
                    PuttyExe = puttyExe;
                }

                string pscpExe = key.GetValue("PscpExe", "").ToString();
                if (File.Exists(pscpExe))
                {
                    PscpExe = pscpExe;
                }
            }

            if (String.IsNullOrEmpty(PuttyExe))
            {
                dlgFindPutty dialog = new dlgFindPutty();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    PuttyExe = dialog.PuttyLocation;
                    PscpExe = dialog.PscpLocation;
                }
            }

            if (String.IsNullOrEmpty(PuttyExe))
            {
                MessageBox.Show("Cannot find PuTTY installation. Please visit http://www.chiark.greenend.org.uk/~sgtatham/putty/download.html to download a copy",
                    "PuTTY Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                System.Environment.Exit(1);
            }

            InitializeComponent();
            dockPanel1.ActiveDocumentChanged += new EventHandler(dockPanel1_ActiveDocumentChanged);
            m_Sessions = new SessionTreeview(dockPanel1);
            m_Sessions.Show(dockPanel1, WeifenLuo.WinFormsUI.Docking.DockState.DockRight);

        }

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
            dockPanel1_ActiveDocumentChanged(null, null);
        }

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

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                PuttyExe = dialog.PuttyLocation;
                PscpExe = dialog.PscpLocation;
            }
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
            if (MessageBox.Show("Exit SuperPuTTY?", "Confirm Exit", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                System.Environment.Exit(0);
            }
        }

        private void puTTYConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process p = new Process();
            p.StartInfo.FileName = PuttyExe;
            p.Start();
        }

    }
}
