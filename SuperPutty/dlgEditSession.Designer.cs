namespace SuperPutty
{
    partial class dlgEditSession
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.textBoxSessionName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxHostname = new System.Windows.Forms.TextBox();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonMintty = new System.Windows.Forms.RadioButton();
            this.radioButtonCygterm = new System.Windows.Forms.RadioButton();
            this.radioButtonSerial = new System.Windows.Forms.RadioButton();
            this.radioButtonSSH = new System.Windows.Forms.RadioButton();
            this.radioButtonRlogin = new System.Windows.Forms.RadioButton();
            this.radioButtonTelnet = new System.Windows.Forms.RadioButton();
            this.radioButtonRaw = new System.Windows.Forms.RadioButton();
            this.textBoxExtraArgs = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxPuttyProfile = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.buttonImageSelect = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxSessionName
            // 
            this.textBoxSessionName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSessionName.Location = new System.Drawing.Point(9, 33);
            this.textBoxSessionName.Name = "textBoxSessionName";
            this.textBoxSessionName.Size = new System.Drawing.Size(416, 20);
            this.textBoxSessionName.TabIndex = 0;
            this.textBoxSessionName.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxSessionName_Validating);
            this.textBoxSessionName.Validated += new System.EventHandler(this.textBoxSessionName_Validated);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Session Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(147, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Host Name (or IP Address)";
            // 
            // textBoxHostname
            // 
            this.textBoxHostname.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxHostname.Location = new System.Drawing.Point(9, 74);
            this.textBoxHostname.Name = "textBoxHostname";
            this.textBoxHostname.Size = new System.Drawing.Size(316, 20);
            this.textBoxHostname.TabIndex = 1;
            this.textBoxHostname.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxHostname_Validating);
            // 
            // textBoxPort
            // 
            this.textBoxPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPort.Location = new System.Drawing.Point(345, 74);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(80, 20);
            this.textBoxPort.TabIndex = 2;
            this.textBoxPort.Text = "22";
            this.textBoxPort.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxPort_Validating);
            this.textBoxPort.Validated += new System.EventHandler(this.textBoxPort_Validated);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(342, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "Port";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.radioButtonMintty);
            this.groupBox1.Controls.Add(this.radioButtonCygterm);
            this.groupBox1.Controls.Add(this.radioButtonSerial);
            this.groupBox1.Controls.Add(this.radioButtonSSH);
            this.groupBox1.Controls.Add(this.radioButtonRlogin);
            this.groupBox1.Controls.Add(this.radioButtonTelnet);
            this.groupBox1.Controls.Add(this.radioButtonRaw);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(14, 117);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(440, 49);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Connection type:";
            // 
            // radioButtonMintty
            // 
            this.radioButtonMintty.AutoSize = true;
            this.radioButtonMintty.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonMintty.Location = new System.Drawing.Point(368, 19);
            this.radioButtonMintty.Name = "radioButtonMintty";
            this.radioButtonMintty.Size = new System.Drawing.Size(60, 19);
            this.radioButtonMintty.TabIndex = 9;
            this.radioButtonMintty.Tag = SuperPutty.Data.ConnectionProtocol.Mintty;
            this.radioButtonMintty.Text = "Mintty";
            this.radioButtonMintty.UseVisualStyleBackColor = true;
            this.radioButtonMintty.CheckedChanged += new System.EventHandler(this.radioButtonCygterm_CheckedChanged);
            // 
            // radioButtonCygterm
            // 
            this.radioButtonCygterm.AutoSize = true;
            this.radioButtonCygterm.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonCygterm.Location = new System.Drawing.Point(297, 19);
            this.radioButtonCygterm.Name = "radioButtonCygterm";
            this.radioButtonCygterm.Size = new System.Drawing.Size(71, 19);
            this.radioButtonCygterm.TabIndex = 8;
            this.radioButtonCygterm.Tag = SuperPutty.Data.ConnectionProtocol.Cygterm;
            this.radioButtonCygterm.Text = "Cygterm";
            this.radioButtonCygterm.UseVisualStyleBackColor = true;
            this.radioButtonCygterm.CheckedChanged += new System.EventHandler(this.radioButtonCygterm_CheckedChanged);
            // 
            // radioButtonSerial
            // 
            this.radioButtonSerial.AutoSize = true;
            this.radioButtonSerial.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonSerial.Location = new System.Drawing.Point(238, 19);
            this.radioButtonSerial.Name = "radioButtonSerial";
            this.radioButtonSerial.Size = new System.Drawing.Size(53, 19);
            this.radioButtonSerial.TabIndex = 7;
            this.radioButtonSerial.Tag = SuperPutty.Data.ConnectionProtocol.Serial;
            this.radioButtonSerial.Text = "Serial";
            this.radioButtonSerial.UseVisualStyleBackColor = true;
            // 
            // radioButtonSSH
            // 
            this.radioButtonSSH.AutoSize = true;
            this.radioButtonSSH.Checked = true;
            this.radioButtonSSH.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonSSH.Location = new System.Drawing.Point(185, 19);
            this.radioButtonSSH.Name = "radioButtonSSH";
            this.radioButtonSSH.Size = new System.Drawing.Size(46, 19);
            this.radioButtonSSH.TabIndex = 6;
            this.radioButtonSSH.TabStop = true;
            this.radioButtonSSH.Tag = SuperPutty.Data.ConnectionProtocol.SSH;
            this.radioButtonSSH.Text = "SSH";
            this.radioButtonSSH.UseVisualStyleBackColor = true;
            this.radioButtonSSH.CheckedChanged += new System.EventHandler(this.radioButtonSSH_CheckedChanged);
            // 
            // radioButtonRlogin
            // 
            this.radioButtonRlogin.AutoSize = true;
            this.radioButtonRlogin.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonRlogin.Location = new System.Drawing.Point(120, 19);
            this.radioButtonRlogin.Name = "radioButtonRlogin";
            this.radioButtonRlogin.Size = new System.Drawing.Size(62, 19);
            this.radioButtonRlogin.TabIndex = 5;
            this.radioButtonRlogin.Tag = SuperPutty.Data.ConnectionProtocol.Rlogin;
            this.radioButtonRlogin.Text = "RLogin";
            this.radioButtonRlogin.UseVisualStyleBackColor = true;
            this.radioButtonRlogin.CheckedChanged += new System.EventHandler(this.radioButtonRlogin_CheckedChanged);
            // 
            // radioButtonTelnet
            // 
            this.radioButtonTelnet.AutoSize = true;
            this.radioButtonTelnet.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonTelnet.Location = new System.Drawing.Point(59, 19);
            this.radioButtonTelnet.Name = "radioButtonTelnet";
            this.radioButtonTelnet.Size = new System.Drawing.Size(58, 19);
            this.radioButtonTelnet.TabIndex = 4;
            this.radioButtonTelnet.Tag = SuperPutty.Data.ConnectionProtocol.Telnet;
            this.radioButtonTelnet.Text = "Telnet";
            this.radioButtonTelnet.UseVisualStyleBackColor = true;
            this.radioButtonTelnet.CheckedChanged += new System.EventHandler(this.radioButtonTelnet_CheckedChanged);
            // 
            // radioButtonRaw
            // 
            this.radioButtonRaw.AutoSize = true;
            this.radioButtonRaw.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonRaw.Location = new System.Drawing.Point(6, 19);
            this.radioButtonRaw.Name = "radioButtonRaw";
            this.radioButtonRaw.Size = new System.Drawing.Size(47, 19);
            this.radioButtonRaw.TabIndex = 3;
            this.radioButtonRaw.Tag = SuperPutty.Data.ConnectionProtocol.Raw;
            this.radioButtonRaw.Text = "Raw";
            this.radioButtonRaw.UseVisualStyleBackColor = true;
            this.radioButtonRaw.CheckedChanged += new System.EventHandler(this.radioButtonRaw_CheckedChanged);
            // 
            // textBoxExtraArgs
            // 
            this.textBoxExtraArgs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxExtraArgs.Location = new System.Drawing.Point(150, 228);
            this.textBoxExtraArgs.Name = "textBoxExtraArgs";
            this.textBoxExtraArgs.Size = new System.Drawing.Size(304, 20);
            this.textBoxExtraArgs.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(14, 230);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(132, 15);
            this.label6.TabIndex = 14;
            this.label6.Text = "Extra PuTTY Arguments";
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSave.Location = new System.Drawing.Point(298, 261);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 8;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.CausesValidation = false;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(379, 261);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(14, 178);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(121, 15);
            this.label4.TabIndex = 9;
            this.label4.Text = "PuTTY Session Profile";
            // 
            // comboBoxPuttyProfile
            // 
            this.comboBoxPuttyProfile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxPuttyProfile.FormattingEnabled = true;
            this.comboBoxPuttyProfile.Location = new System.Drawing.Point(150, 176);
            this.comboBoxPuttyProfile.Name = "comboBoxPuttyProfile";
            this.comboBoxPuttyProfile.Size = new System.Drawing.Size(304, 21);
            this.comboBoxPuttyProfile.TabIndex = 4;
            this.comboBoxPuttyProfile.SelectedIndexChanged += new System.EventHandler(this.comboBoxPuttyProfile_SelectedIndexChanged);
            this.comboBoxPuttyProfile.Validating += new System.ComponentModel.CancelEventHandler(this.comboBoxPuttyProfile_Validating);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.textBoxSessionName);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.textBoxHostname);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.textBoxPort);
            this.groupBox2.Location = new System.Drawing.Point(14, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(440, 109);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(14, 204);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(93, 15);
            this.label5.TabIndex = 12;
            this.label5.Text = "Login Username";
            // 
            // textBoxUsername
            // 
            this.textBoxUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxUsername.Location = new System.Drawing.Point(150, 202);
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.Size = new System.Drawing.Size(304, 20);
            this.textBoxUsername.TabIndex = 5;
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            // 
            // buttonImageSelect
            // 
            this.buttonImageSelect.Location = new System.Drawing.Point(14, 255);
            this.buttonImageSelect.Name = "buttonImageSelect";
            this.buttonImageSelect.Size = new System.Drawing.Size(29, 29);
            this.buttonImageSelect.TabIndex = 7;
            this.buttonImageSelect.UseVisualStyleBackColor = true;
            this.buttonImageSelect.Click += new System.EventHandler(this.buttonImageSelect_Click);
            // 
            // dlgEditSession
            // 
            this.AcceptButton = this.buttonSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(470, 291);
            this.Controls.Add(this.buttonImageSelect);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBoxExtraArgs);
            this.Controls.Add(this.textBoxUsername);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.comboBoxPuttyProfile);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "dlgEditSession";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create New Session";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxSessionName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxHostname;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonSerial;
        private System.Windows.Forms.RadioButton radioButtonSSH;
        private System.Windows.Forms.RadioButton radioButtonRlogin;
        private System.Windows.Forms.RadioButton radioButtonTelnet;
        private System.Windows.Forms.RadioButton radioButtonRaw;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxPuttyProfile;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.RadioButton radioButtonCygterm;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxExtraArgs;
        private System.Windows.Forms.RadioButton radioButtonMintty;
        private System.Windows.Forms.Button buttonImageSelect;
        private System.Windows.Forms.ToolTip toolTip;
    }
}