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
using Microsoft.Win32;
using System.Web;
using SuperPutty.Data;

namespace SuperPutty
{
    public partial class dlgEditSession : Form
    {
        public delegate bool SessionNameValidationHandler(string name, out string error);

        private SessionData Session;
        public dlgEditSession(SessionData session)
        {
            Session = session;
            InitializeComponent();

            // get putty saved settings from the registry to populate
            // the dropdown
            PopulatePuttySettings();

            if (!String.IsNullOrEmpty(Session.SessionName))
            {
                this.Text = "Edit session: " + session.SessionName;
                this.textBoxSessionName.Text = Session.SessionName;
                this.textBoxHostname.Text = Session.Host;
                this.textBoxPort.Text = Session.Port.ToString();
                this.textBoxUsername.Text = Session.Username;

                switch (Session.Proto)
                {
                    case ConnectionProtocol.Raw:
                        radioButtonRaw.Checked = true;
                        break;
                    case ConnectionProtocol.Rlogin:
                        radioButtonRlogin.Checked = true;
                        break;
                    case ConnectionProtocol.Serial:
                        radioButtonSerial.Checked = true;
                        break;
                    case ConnectionProtocol.SSH:
                        radioButtonSSH.Checked = true;
                        break;
                    case ConnectionProtocol.Telnet:
                        radioButtonTelnet.Checked = true;
                        break;
                    case ConnectionProtocol.Cygterm:
                        radioButtonCygterm.Checked = true;
                        break;
                    default:
                        radioButtonSSH.Checked = true;
                        break;
                }
            }
            else
            {
                this.Text = "Create new session";
                radioButtonSSH.Checked = true;
            }
        }

        private void PopulatePuttySettings()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\SimonTatham\PuTTY\Sessions");
            if (key != null)
            {
                string[] savedSessionNames = key.GetSubKeyNames();

                for (int i = 0; i < savedSessionNames.Length; i++)
                    comboBoxPuttyProfile.Items.Add(HttpUtility.UrlDecode(savedSessionNames[i]));
            }
        }

        private void sessionForm_TextChanged(object sender, EventArgs e)
        {
            buttonSave.Enabled = (textBoxSessionName.Text.Length > 0
                && textBoxHostname.Text.Length > 0
                && textBoxPort.Text.Length > 0
                && comboBoxPuttyProfile.Text.Length > 0);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Session.SessionName = textBoxSessionName.Text.Trim();
            Session.PuttySession = comboBoxPuttyProfile.Text.Trim();
            Session.Host = textBoxHostname.Text.Trim();
            Session.Port = int.Parse(textBoxPort.Text.Trim());
            Session.Username = textBoxUsername.Text.Trim();

            for (int i = 0; i < groupBox1.Controls.Count; i++)
            {
                RadioButton rb = (RadioButton)groupBox1.Controls[i];
                if (rb.Checked)
                {
                    Session.Proto = (ConnectionProtocol)rb.Tag;
                }
            }
            
            //Session.SaveToRegistry();
            SuperPuTTY.SaveSessions();

            DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Special UI handling for cygterm sessions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButtonCygterm_CheckedChanged(object sender, EventArgs e)
        {
            string host = this.textBoxHostname.Text;
            bool isCygterm = this.radioButtonCygterm.Checked;
            this.textBoxPort.Enabled = !isCygterm;
            this.textBoxUsername.Enabled = !isCygterm;

            if (isCygterm)
            {
                if (String.IsNullOrEmpty(host) || !host.StartsWith(CygtermInfo.LocalHost))
                {
                    this.textBoxHostname.Text = CygtermInfo.LocalHost;
                }
            }

        }

        private void textBoxSessionName_Validating(object sender, CancelEventArgs e)
        {
            if (this.SessionNameValidator != null)
            {
                string error;
                if (!this.SessionNameValidator(this.textBoxSessionName.Text, out error))
                {
                    this.errorProvider.SetError(this.textBoxSessionName, error ?? "Invalid Session Name");
                    this.buttonSave.Enabled = false;
                }
                else
                {
                    this.errorProvider.SetError(this.textBoxSessionName, String.Empty);
                    this.buttonSave.Enabled = true;
                }
            }
        }

        public SessionNameValidationHandler SessionNameValidator { get; set; }
    }
}
