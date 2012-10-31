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
using SuperPutty.Utils;
using SuperPutty.Gui;
using log4net;

namespace SuperPutty
{
    public partial class dlgEditSession : Form
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(dlgEditSession));

        public delegate bool SessionNameValidationHandler(string name, out string error);

        private SessionData Session;
        private String OldHostname;
        private bool isInitialized = false;
        private ImageListPopup imgPopup = null;

        public dlgEditSession(SessionData session, ImageList iconList)
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
                this.textBoxExtraArgs.Text = Session.ExtraArgs;
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
                    case ConnectionProtocol.Mintty:
                        radioButtonMintty.Checked = true;
                        break;
                    default:
                        radioButtonSSH.Checked = true;
                        break;
                }

                foreach(String settings in this.comboBoxPuttyProfile.Items){
                    if (settings == session.PuttySession)
                    {
                        this.comboBoxPuttyProfile.SelectedItem = settings;
                        break;
                    }
                }

                this.buttonSave.Enabled = true;
            }
            else
            {
                this.Text = "Create new session";
                radioButtonSSH.Checked = true;
                this.buttonSave.Enabled = false;
            }


            // Setup icon chooser
            this.buttonImageSelect.ImageList = iconList;
            this.buttonImageSelect.ImageKey = string.IsNullOrEmpty(Session.ImageKey)
                ? SessionTreeview.ImageKeySession
                : Session.ImageKey;
            this.toolTip.SetToolTip(this.buttonImageSelect, buttonImageSelect.ImageKey);

            this.isInitialized = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.BeginInvoke(new MethodInvoker(delegate { this.textBoxSessionName.Focus(); }));
        }

        private void PopulatePuttySettings()
        {
            foreach (String sessionName in PuttyDataHelper.GetSessionNames())
            {
                comboBoxPuttyProfile.Items.Add(sessionName);
            }
            comboBoxPuttyProfile.SelectedItem = PuttyDataHelper.SessionDefaultSettings;
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
            Session.ExtraArgs = textBoxExtraArgs.Text.Trim();
            Session.Port = int.Parse(textBoxPort.Text.Trim());
            Session.Username = textBoxUsername.Text.Trim();
            Session.SessionId = SessionData.CombineSessionIds(SessionData.GetSessionParentId(Session.SessionId), Session.SessionName);
            Session.ImageKey = buttonImageSelect.ImageKey;

            for (int i = 0; i < groupBox1.Controls.Count; i++)
            {
                RadioButton rb = (RadioButton)groupBox1.Controls[i];
                if (rb.Checked)
                {
                    Session.Proto = (ConnectionProtocol)rb.Tag;
                }
            }
            
            DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Special UI handling for cygterm or mintty sessions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButtonCygterm_CheckedChanged(object sender, EventArgs e)
        {
            string host = this.textBoxHostname.Text;
            bool isLocalShell = this.radioButtonCygterm.Checked || this.radioButtonMintty.Checked;
            this.textBoxPort.Enabled = !isLocalShell;
            this.textBoxExtraArgs.Enabled = !isLocalShell;
            this.textBoxUsername.Enabled = !isLocalShell;

            if (isLocalShell)
            {
                if (String.IsNullOrEmpty(host) || !host.StartsWith(CygtermStartInfo.LocalHost))
                {
                    OldHostname = this.textBoxHostname.Text;
                    this.textBoxHostname.Text = CygtermStartInfo.LocalHost;
                }
            }

        }

        private void radioButtonRaw_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButtonRaw.Checked && this.isInitialized)
            {
                if (!string.IsNullOrEmpty(OldHostname))
                {
                    this.textBoxHostname.Text = OldHostname;
                    OldHostname = null;
                }
            }
        }

        private void radioButtonTelnet_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButtonTelnet.Checked && this.isInitialized)
            {
                if (!string.IsNullOrEmpty(OldHostname))
                {
                    this.textBoxHostname.Text = OldHostname;
                    OldHostname = null;
                }
                this.textBoxPort.Text = "23";
            }
        }

        private void radioButtonRlogin_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButtonRlogin.Checked && this.isInitialized)
            {
                if (!string.IsNullOrEmpty(OldHostname))
                {
                    this.textBoxHostname.Text = OldHostname;
                    OldHostname = null;
                }
                this.textBoxPort.Text = "513";
            }
        }

        private void radioButtonSSH_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButtonSSH.Checked && this.isInitialized)
            {
                if (!string.IsNullOrEmpty(OldHostname))
                {
                    this.textBoxHostname.Text = OldHostname;
                    OldHostname = null;
                }
                this.textBoxPort.Text = "22";
            }
        }



        public static int GetDefaultPort(ConnectionProtocol protocol)
        {
            int port = 22;
            switch (protocol)
            {
                case ConnectionProtocol.Raw:
                    break;
                case ConnectionProtocol.Rlogin:
                    port = 513;
                    break;
                case ConnectionProtocol.Serial:
                    break;
                case ConnectionProtocol.Telnet:
                    port = 23;
                    break;
            }
            return port;
        }

        #region Icon
        private void buttonImageSelect_Click(object sender, EventArgs e)
        {
            if (this.imgPopup == null)
            {
                int n = buttonImageSelect.ImageList.Images.Count;
                int x = (int) Math.Floor(Math.Sqrt(n)) + 1;
                int cols = x;
                int rows = x;

                imgPopup = new ImageListPopup();
                imgPopup.BackgroundColor = Color.FromArgb(241, 241, 241);
                imgPopup.BackgroundOverColor = Color.FromArgb(102, 154, 204);
                imgPopup.Init(this.buttonImageSelect.ImageList, 8, 8, cols, rows);
                imgPopup.ItemClick += new ImageListPopupEventHandler(this.OnItemClicked);
            }

            Point pt = PointToScreen(new Point(buttonImageSelect.Left, buttonImageSelect.Bottom));
            imgPopup.Show(pt.X + 2, pt.Y);
        }


        private void OnItemClicked(object sender, ImageListPopupEventArgs e)
        {
            if (imgPopup == sender)
            {
                buttonImageSelect.ImageKey = e.SelectedItem;
                this.toolTip.SetToolTip(this.buttonImageSelect, buttonImageSelect.ImageKey);
            }
        } 
        #endregion

        #region Validation Logic

        public SessionNameValidationHandler SessionNameValidator { get; set; }

        private void textBoxSessionName_Validating(object sender, CancelEventArgs e)
        {
            if (this.SessionNameValidator != null)
            {
                string error;
                if (!this.SessionNameValidator(this.textBoxSessionName.Text, out error))
                {
                    e.Cancel = true;
                    this.SetError(this.textBoxSessionName, error ?? "Invalid Session Name");
                }
            }
        }

        private void textBoxSessionName_Validated(object sender, EventArgs e)
        {
            this.SetError(this.textBoxSessionName, String.Empty);
        }

        private void textBoxPort_Validating(object sender, CancelEventArgs e)
        {
            int val;
            if (!Int32.TryParse(this.textBoxPort.Text, out val))
            {
                e.Cancel = true;
                this.SetError(this.textBoxPort, "Invalid Port");
            }
        }

        private void textBoxPort_Validated(object sender, EventArgs e)
        {
            this.SetError(this.textBoxPort, String.Empty);
        }

        private void textBoxHostname_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty((string)this.comboBoxPuttyProfile.SelectedItem) &&
                string.IsNullOrEmpty(this.textBoxHostname.Text.Trim()))
            {
                if (sender == this.textBoxHostname)
                {
                    this.SetError(this.textBoxHostname, "A host name must be specified if a Putty Session Profile is not selected");
                }
                else if (sender == this.comboBoxPuttyProfile)
                {
                    this.SetError(this.comboBoxPuttyProfile, "A Putty Session Profile must be selected if a Host Name is not provided");
                }
            }
            else
            {
                this.SetError(this.textBoxHostname, String.Empty);
                this.SetError(this.comboBoxPuttyProfile, String.Empty);
            }
        }

        private void comboBoxPuttyProfile_Validating(object sender, CancelEventArgs e)
        {
            this.textBoxHostname_Validating(sender, e);
        }

        private void comboBoxPuttyProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ValidateChildren(ValidationConstraints.ImmediateChildren);    
        }

        void SetError(Control control, string error)
        {
            this.errorProvider.SetError(control, error);
            this.EnableDisableSaveButton();
        }

        void EnableDisableSaveButton()
        {
            this.buttonSave.Enabled = (
                this.errorProvider.GetError(this.textBoxSessionName) == String.Empty &&
                this.errorProvider.GetError(this.textBoxHostname) == String.Empty &&
                this.errorProvider.GetError(this.textBoxPort) == String.Empty &&
                this.errorProvider.GetError(this.comboBoxPuttyProfile) == String.Empty);
        }

        #endregion

    }
}
