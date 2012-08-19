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
using SuperPutty.Utils;

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
        private string OrigDefaultLayoutName { get; set; }

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

            if (string.IsNullOrEmpty(SuperPuTTY.Settings.MinttyExe))
            {
                if (File.Exists(@"C:\cygwin\bin\mintty.exe"))
                {
                    this.textBoxMinttyLocation.Text = @"C:\cygwin\bin\mintty.exe";
                }
            }
            else
            {
                this.textBoxMinttyLocation.Text = SuperPuTTY.Settings.MinttyExe;
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

            // tab text
            foreach(String s in Enum.GetNames(typeof(frmSuperPutty.TabTextBehavior)))
            {
                this.comboBoxTabText.Items.Add(s);
            }
            this.comboBoxTabText.SelectedItem = SuperPuTTY.Settings.TabTextBehavior;

            // tab switcher
            ITabSwitchStrategy selectedItem = null;
            foreach (ITabSwitchStrategy strat in TabSwitcher.Strategies)
            {
                this.comboBoxTabSwitching.Items.Add(strat);
                if (strat.GetType().FullName == SuperPuTTY.Settings.TabSwitcher)
                {
                    selectedItem = strat;
                }
            }
            this.comboBoxTabSwitching.SelectedItem = selectedItem ?? TabSwitcher.Strategies[0];

            // activator types
            this.comboBoxActivatorType.Items.Add(typeof(KeyEventWindowActivator).FullName);
            this.comboBoxActivatorType.Items.Add(typeof(CombinedWindowActivator).FullName);
            this.comboBoxActivatorType.Items.Add(typeof(SetFGWindowActivator).FullName);
            this.comboBoxActivatorType.Items.Add(typeof(RestoreWindowActivator).FullName);
            this.comboBoxActivatorType.Items.Add(typeof(SetFGAttachThreadWindowActivator).FullName);
            this.comboBoxActivatorType.SelectedItem = SuperPuTTY.Settings.WindowActivator;

            // default layouts
            InitLayouts();

            this.checkSingleInstanceMode.Checked = SuperPuTTY.Settings.SingleInstanceMode;
            this.checkConstrainPuttyDocking.Checked = SuperPuTTY.Settings.RestrictContentToDocumentTabs;
            this.checkRestoreWindow.Checked = SuperPuTTY.Settings.RestoreWindowLocation;
            this.checkExitConfirmation.Checked = SuperPuTTY.Settings.ExitConfirmation;
            this.checkExpandTree.Checked = SuperPuTTY.Settings.ExpandSessionsTreeOnStartup;
            this.checkMinimizeToTray.Checked = SuperPuTTY.Settings.MinimizeToTray;
            this.checkSessionsTreeShowLines.Checked = SuperPuTTY.Settings.SessionsTreeShowLines;
            this.btnFont.Font = SuperPuTTY.Settings.SessionsTreeFont;
            this.btnFont.Text = ToShortString(SuperPuTTY.Settings.SessionsTreeFont);

            if (SuperPuTTY.IsFirstRun)
            {
                this.ShowIcon = true;
                this.ShowInTaskbar = true;
            }
        }

        private void InitLayouts()
        {
            String defaultLayout;
            List<String> layouts = new List<string>();
            if (SuperPuTTY.IsFirstRun)
            {
                layouts.Add(String.Empty);
                // HACK: first time so layouts directory not set yet so layouts don't exist...
                //       preload <AutoRestore> so we can set it as default
                layouts.Add(LayoutData.AutoRestore);

                defaultLayout = LayoutData.AutoRestore;
            }
            else
            {
                layouts.Add(String.Empty);
                // auto restore in inte layouts collection already
                foreach (LayoutData layout in SuperPuTTY.Layouts)
                {
                    layouts.Add(layout.Name);
                }

                defaultLayout = SuperPuTTY.Settings.DefaultLayoutName;
            }
            this.comboBoxLayouts.DataSource = layouts;
            this.comboBoxLayouts.SelectedItem = defaultLayout;
            this.OrigDefaultLayoutName = defaultLayout;
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

            string mintty = this.textBoxMinttyLocation.Text;
            if (!string.IsNullOrEmpty(mintty) && File.Exists(mintty))
            {
                SuperPuTTY.Settings.MinttyExe = mintty;
            }

            if (errors.Count == 0)
            {
                SuperPuTTY.Settings.SingleInstanceMode = this.checkSingleInstanceMode.Checked;
                SuperPuTTY.Settings.RestrictContentToDocumentTabs = this.checkConstrainPuttyDocking.Checked;
                SuperPuTTY.Settings.RestoreWindowLocation = this.checkRestoreWindow.Checked;
                SuperPuTTY.Settings.ExitConfirmation = this.checkExitConfirmation.Checked;
                SuperPuTTY.Settings.ExpandSessionsTreeOnStartup = this.checkExpandTree.Checked;
                SuperPuTTY.Settings.MinimizeToTray = this.checkMinimizeToTray.Checked;
                SuperPuTTY.Settings.TabTextBehavior = (string) this.comboBoxTabText.SelectedItem;
                SuperPuTTY.Settings.TabSwitcher = (string)this.comboBoxTabSwitching.SelectedItem.GetType().FullName;
                SuperPuTTY.Settings.SessionsTreeShowLines = this.checkSessionsTreeShowLines.Checked;
                SuperPuTTY.Settings.SessionsTreeFont = this.btnFont.Font;
                SuperPuTTY.Settings.WindowActivator = (string) this.comboBoxActivatorType.SelectedItem;

                SuperPuTTY.Settings.Save();

                // @TODO - move this to a better place...maybe event handler after opening
                if (OrigSettingsFolder != SuperPuTTY.Settings.SettingsFolder)
                {
                    SuperPuTTY.LoadLayouts();
                    SuperPuTTY.LoadSessions();
                }
                else if (OrigDefaultLayoutName != SuperPuTTY.Settings.DefaultLayoutName)
                {
                    SuperPuTTY.LoadLayouts();
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


        private void btnBrowseMintty_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "MinTTY|mintty.exe";
            openFileDialog1.FileName = "mintty.exe";

            if (File.Exists(textBoxMinttyLocation.Text))
            {
                openFileDialog1.InitialDirectory = Path.GetDirectoryName(textBoxMinttyLocation.Text);
            }
            openFileDialog1.ShowDialog();
            if (!String.IsNullOrEmpty(openFileDialog1.FileName))
                textBoxMinttyLocation.Text = openFileDialog1.FileName;
        }

        /// <summary>
        /// Check that putty can be found.  If not, prompt the user
        /// </summary>
        public static void PuttyCheck()
        {
            if (String.IsNullOrEmpty(SuperPuTTY.Settings.PuttyExe) || SuperPuTTY.IsFirstRun)
            {
                // first time, try to import old putty settings from registry
                SuperPuTTY.Settings.ImportFromRegistry();
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

            if (SuperPuTTY.IsFirstRun && SuperPuTTY.Sessions.Count == 0)
            {
                // first run, got nothing...try to import from registry
                SuperPuTTY.ImportSessionsFromSuperPutty1030();
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

        private void btnFont_Click(object sender, EventArgs e)
        {
            this.fontDialog.Font = this.btnFont.Font;
            if (this.fontDialog.ShowDialog() == DialogResult.OK)
            {
                this.btnFont.Font = this.fontDialog.Font;
                this.btnFont.Text = ToShortString(this.fontDialog.Font);
            }
        }


        static string ToShortString(Font font)
        {
            return String.Format("{0}, {1} pt, {2}", font.FontFamily.Name, font.Size, font.Style);
        }
    }

}
