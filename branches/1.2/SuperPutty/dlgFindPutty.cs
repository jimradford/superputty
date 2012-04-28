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
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using log4net;
using SuperPutty.Data;

namespace SuperPutty
{
    public partial class dlgFindPutty : Form
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(dlgFindPutty));

        private string m_PuttyLocation;

        public string PuttyLocation
        {
            get { return m_PuttyLocation; }
            private set { m_PuttyLocation = value; }
        }
        private string m_PscpLocation;

        public string PscpLocation
        {
            get { return m_PscpLocation; }
            private set { m_PscpLocation = value; }
        }

        private string OrigSettingsFolder { get; set; }

        public dlgFindPutty()
        {
            InitializeComponent();

            string puttyExe = SuperPuTTY.Settings.PuttyExe;
            string pscpExe = SuperPuTTY.Settings.PscpExe;

            // check for location of putty/pscp
            if (!String.IsNullOrEmpty(puttyExe) && File.Exists(puttyExe))
            {
                textBoxPuttyLocation.Text = puttyExe;
                if (!String.IsNullOrEmpty(pscpExe) && File.Exists(pscpExe))
                {
                    textBoxPscpLocation.Text = pscpExe;
                }
            }
            else if(!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("ProgramFiles(x86)")))
            {
                if (File.Exists(Environment.GetEnvironmentVariable("ProgramFiles(x86)") + @"\PuTTY\putty.exe"))
                {
                    textBoxPuttyLocation.Text = Environment.GetEnvironmentVariable("ProgramFiles(x86)") + @"\PuTTY\putty.exe";
                    openFileDialog1.InitialDirectory = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
                }

                if (File.Exists(Environment.GetEnvironmentVariable("ProgramFiles(x86)") + @"\PuTTY\pscp.exe"))
                {

                    textBoxPscpLocation.Text = Environment.GetEnvironmentVariable("ProgramFiles(x86)") + @"\PuTTY\pscp.exe";
                }
            }
            else if (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("ProgramFiles")))
            {
                if (File.Exists(Environment.GetEnvironmentVariable("ProgramFiles") + @"\PuTTY\putty.exe"))
                {
                    textBoxPuttyLocation.Text = Environment.GetEnvironmentVariable("ProgramFiles") + @"\PuTTY\putty.exe";
                    openFileDialog1.InitialDirectory = Environment.GetEnvironmentVariable("ProgramFiles");
                }

                if (File.Exists(Environment.GetEnvironmentVariable("ProgramFiles") + @"\PuTTY\pscp.exe"))
                {
                    textBoxPscpLocation.Text = Environment.GetEnvironmentVariable("ProgramFiles") + @"\PuTTY\pscp.exe";
                }
            }            
            else
            {
                openFileDialog1.InitialDirectory = Application.StartupPath;
            } 
            
            // super putty settings (sessions and layouts)
            if (string.IsNullOrEmpty(SuperPuTTY.Settings.SettingsFolder))
            {
                // Set a default
                String dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SuperPuTTY");
                if (!Directory.Exists(dir))
                {
                    Log.InfoFormat("Creating default settings dir: {0}", dir);
                    Directory.CreateDirectory(dir);
                }
                this.textBoxSettingsFolder.Text = dir;
            }
            else
            {
                this.textBoxSettingsFolder.Text = SuperPuTTY.Settings.SettingsFolder;
            }
            this.OrigSettingsFolder = SuperPuTTY.Settings.SettingsFolder;

            // default layouts
            List<String> layouts = new List<string>();
            layouts.Add(String.Empty);
            foreach (LayoutData layout in SuperPuTTY.Layouts)
            {
                layouts.Add(layout.Name);
            }
            this.comboBoxLayouts.DataSource = layouts;
            this.comboBoxLayouts.SelectedItem = SuperPuTTY.Settings.DefaultLayoutName;

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.BeginInvoke(new MethodInvoker(delegate { this.textBoxPuttyLocation.Focus(); }));
        }
       
        private void buttonOk_Click(object sender, EventArgs e)
        {
            List<String> errors = new List<string>();
            if (!String.IsNullOrEmpty(textBoxPscpLocation.Text) && File.Exists(textBoxPscpLocation.Text))
            {
                SuperPuTTY.Settings.PscpExe = textBoxPscpLocation.Text;
            }

            string settingsDir = textBoxSettingsFolder.Text;
            if (String.IsNullOrEmpty(settingsDir) || !Directory.Exists(settingsDir))
            {
                errors.Add("Settings Folder must be set to valid directory");
            }
            else
            {
                SuperPuTTY.Settings.SettingsFolder = settingsDir;
            }

            if (this.comboBoxLayouts.SelectedValue != null)
            {
                SuperPuTTY.Settings.DefaultLayoutName = (string) comboBoxLayouts.SelectedValue;
            }

            if (!String.IsNullOrEmpty(textBoxPuttyLocation.Text) && File.Exists(textBoxPuttyLocation.Text))
            {
                SuperPuTTY.Settings.PuttyExe = textBoxPuttyLocation.Text;
            }
            else
            {
                errors.Insert(0, "PuTTY is required to properly use this application.");
            }

            SuperPuTTY.Settings.SingleInstanceMode = this.checkSingleInstanceMode.Checked;
            SuperPuTTY.Settings.RestrictPuttyTabDocking = this.checkConstrainPuttyDocking.Checked;
            SuperPuTTY.Settings.RestoreWindowLocation = this.checkRestoreWindow.Checked;

            if (errors.Count == 0)
            {
                SuperPuTTY.Settings.Save();

                // @TODO - move this to a better place...maybe event handler after opening
                if (OrigSettingsFolder != SuperPuTTY.Settings.SettingsFolder)
                {
                    SuperPuTTY.LoadLayouts();
                    SuperPuTTY.LoadSessions();
                }
                DialogResult = DialogResult.OK;
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (String s in errors)
                {
                    sb.Append(s).AppendLine().AppendLine();
                }
                if (MessageBox.Show(sb.ToString(), "Errors", MessageBoxButtons.RetryCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    DialogResult = DialogResult.Cancel;
                }
            }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "PuTTY|putty.exe|KiTTY|kitty*.exe";
            openFileDialog1.FileName = "putty.exe";
            if (File.Exists(textBoxPuttyLocation.Text))
            {
                openFileDialog1.FileName = Path.GetFileName(textBoxPuttyLocation.Text);
                openFileDialog1.InitialDirectory = Path.GetDirectoryName(textBoxPuttyLocation.Text);
                openFileDialog1.FilterIndex = openFileDialog1.FileName.ToLower().StartsWith("putty") ? 1 : 2;
            }
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (!String.IsNullOrEmpty(openFileDialog1.FileName))
                    textBoxPuttyLocation.Text = openFileDialog1.FileName;
            }            
        }

        private void buttonBrowsePscp_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "PScp|pscp.exe";
            openFileDialog1.FileName = "pscp.exe";

            if (File.Exists(textBoxPscpLocation.Text))
            {
                openFileDialog1.InitialDirectory = Path.GetDirectoryName(textBoxPscpLocation.Text);
            }
            openFileDialog1.ShowDialog();
            if (!String.IsNullOrEmpty(openFileDialog1.FileName))
                textBoxPscpLocation.Text = openFileDialog1.FileName;
        }

        /// <summary>
        /// Check that putty can be found.  If not, prompt the user
        /// </summary>
        public static void PuttyCheck()
        {
            if (String.IsNullOrEmpty(SuperPuTTY.Settings.PuttyExe) || SuperPuTTY.IsFirstRun)
            {
                dlgFindPutty dialog = new dlgFindPutty();
                if (dialog.ShowDialog() == DialogResult.Cancel)
                {
                    System.Environment.Exit(1);
                }
            }

            if (String.IsNullOrEmpty(SuperPuTTY.Settings.PuttyExe))
            {
                MessageBox.Show("Cannot find PuTTY installation. Please visit http://www.chiark.greenend.org.uk/~sgtatham/putty/download.html to download a copy",
                    "PuTTY Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                System.Environment.Exit(1);
            }
        }

        private void buttonBrowseLayoutsFolder_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog.ShowDialog(this) == DialogResult.OK) 
            {
                this.textBoxSettingsFolder.Text = this.folderBrowserDialog.SelectedPath;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
