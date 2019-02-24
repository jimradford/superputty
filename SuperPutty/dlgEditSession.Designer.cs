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
            this.labelSessionname = new System.Windows.Forms.Label();
            this.labelHostName = new System.Windows.Forms.Label();
            this.textBoxHostname = new System.Windows.Forms.TextBox();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.labelPort = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonVNC = new System.Windows.Forms.RadioButton();
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
            this.panelSerialConnSettings = new System.Windows.Forms.Panel();
            this.labelSerialLine = new System.Windows.Forms.Label();
            this.comboBoxSerialLine = new System.Windows.Forms.ComboBox();
            this.labelSerialSpeed = new System.Windows.Forms.Label();
            this.comboBoxSerialSpeed = new System.Windows.Forms.ComboBox();
            this.labelSerialDataBits = new System.Windows.Forms.Label();
            this.comboBoxSerialDataBits = new System.Windows.Forms.ComboBox();
            this.labelSerialStopBits = new System.Windows.Forms.Label();
            this.comboBoxSerialStopBits = new System.Windows.Forms.ComboBox();
            this.labelSerialParity = new System.Windows.Forms.Label();
            this.comboBoxSerialParity = new System.Windows.Forms.ComboBox();
            this.labelSerialFlowControl = new System.Windows.Forms.Label();
            this.comboBoxSerialFlowCtrl = new System.Windows.Forms.ComboBox();
            this.panelIpConnection = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.buttonImageSelect = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.textBoxSPSLScriptFile = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.buttonClearSPSLFile = new System.Windows.Forms.Button();
            this.groupBoxFileTransferOptions = new System.Windows.Forms.GroupBox();
            this.textBoxRemotePathSesion = new System.Windows.Forms.TextBox();
            this.lbRemotePath = new System.Windows.Forms.Label();
            this.buttonBrowseLocalPath = new System.Windows.Forms.Button();
            this.textBoxLocalPathSesion = new System.Windows.Forms.TextBox();
            this.lbLocalPath = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panelSerialConnSettings.SuspendLayout();
            this.panelIpConnection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.groupBoxFileTransferOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxSessionName
            // 
            this.textBoxSessionName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSessionName.Location = new System.Drawing.Point(6, 33);
            this.textBoxSessionName.Name = "textBoxSessionName";
            this.textBoxSessionName.Size = new System.Drawing.Size(464, 20);
            this.textBoxSessionName.TabIndex = 0;
            this.textBoxSessionName.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxSessionName_Validating);
            this.textBoxSessionName.Validated += new System.EventHandler(this.textBoxSessionName_Validated);
            // 
            // labelSessionname
            // 
            this.labelSessionname.AutoSize = true;
            this.labelSessionname.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSessionname.Location = new System.Drawing.Point(6, 16);
            this.labelSessionname.Name = "labelSessionname";
            this.labelSessionname.Size = new System.Drawing.Size(81, 15);
            this.labelSessionname.TabIndex = 1;
            this.labelSessionname.Text = "Session Name";
            // 
            // labelHostName
            // 
            this.labelHostName.AutoSize = true;
            this.labelHostName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHostName.Location = new System.Drawing.Point(6, 3);
            this.labelHostName.Name = "labelHostName";
            this.labelHostName.Size = new System.Drawing.Size(147, 15);
            this.labelHostName.TabIndex = 2;
            this.labelHostName.Text = "Host Name (or IP Address)";
            // 
            // textBoxHostname
            // 
            this.textBoxHostname.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxHostname.Location = new System.Drawing.Point(6, 24);
            this.textBoxHostname.Name = "textBoxHostname";
            this.textBoxHostname.Size = new System.Drawing.Size(352, 20);
            this.textBoxHostname.TabIndex = 1;
            this.textBoxHostname.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxHostname_Validating);
            // 
            // textBoxPort
            // 
            this.textBoxPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPort.Location = new System.Drawing.Point(381, 24);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(80, 20);
            this.textBoxPort.TabIndex = 2;
            this.textBoxPort.Text = "22";
            this.textBoxPort.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxPort_Validating);
            this.textBoxPort.Validated += new System.EventHandler(this.textBoxPort_Validated);
            // 
            // labelPort
            // 
            this.labelPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelPort.AutoSize = true;
            this.labelPort.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPort.Location = new System.Drawing.Point(378, 3);
            this.labelPort.Name = "labelPort";
            this.labelPort.Size = new System.Drawing.Size(53, 15);
            this.labelPort.TabIndex = 5;
            this.labelPort.Text = "TCP Port";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.radioButtonVNC);
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
            this.groupBox1.Size = new System.Drawing.Size(485, 49);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Connection type:";
            // 
            // radioButtonVNC
            // 
            this.radioButtonVNC.AutoSize = true;
            this.radioButtonVNC.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonVNC.Location = new System.Drawing.Point(432, 19);
            this.radioButtonVNC.Name = "radioButtonVNC";
            this.radioButtonVNC.Size = new System.Drawing.Size(49, 19);
            this.radioButtonVNC.TabIndex = 9;
            this.radioButtonVNC.Tag = SuperPutty.Data.ConnectionProtocol.VNC;
            this.radioButtonVNC.Text = "VNC";
            this.radioButtonVNC.UseVisualStyleBackColor = true;
            this.radioButtonVNC.CheckedChanged += new System.EventHandler(this.radioButtonVNC_CheckedChanged);
            // 
            // radioButtonMintty
            // 
            this.radioButtonMintty.AutoSize = true;
            this.radioButtonMintty.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonMintty.Location = new System.Drawing.Point(367, 19);
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
            this.radioButtonCygterm.Location = new System.Drawing.Point(292, 19);
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
            this.radioButtonSerial.Location = new System.Drawing.Point(235, 19);
            this.radioButtonSerial.Name = "radioButtonSerial";
            this.radioButtonSerial.Size = new System.Drawing.Size(53, 19);
            this.radioButtonSerial.TabIndex = 7;
            this.radioButtonSerial.Tag = SuperPutty.Data.ConnectionProtocol.Serial;
            this.radioButtonSerial.Text = "Serial";
            this.radioButtonSerial.UseVisualStyleBackColor = true;
            this.radioButtonSerial.CheckedChanged += new System.EventHandler(this.radioButtonSerial_CheckedChanged);
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
            this.radioButtonRlogin.Location = new System.Drawing.Point(119, 19);
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
            this.radioButtonTelnet.Location = new System.Drawing.Point(57, 19);
            this.radioButtonTelnet.Name = "radioButtonTelnet";
            this.radioButtonTelnet.Size = new System.Drawing.Size(57, 19);
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
            this.textBoxExtraArgs.Location = new System.Drawing.Point(150, 229);
            this.textBoxExtraArgs.Name = "textBoxExtraArgs";
            this.textBoxExtraArgs.Size = new System.Drawing.Size(349, 20);
            this.textBoxExtraArgs.TabIndex = 6;
            this.textBoxExtraArgs.TextChanged += new System.EventHandler(this.textBoxExtraArgs_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(14, 232);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(94, 15);
            this.label6.TabIndex = 14;
            this.label6.Text = "Extra Arguments";
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSave.Location = new System.Drawing.Point(343, 401);
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
            this.buttonCancel.Location = new System.Drawing.Point(424, 401);
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
            this.comboBoxPuttyProfile.Size = new System.Drawing.Size(349, 21);
            this.comboBoxPuttyProfile.TabIndex = 4;
            this.comboBoxPuttyProfile.SelectedIndexChanged += new System.EventHandler(this.comboBoxPuttyProfile_SelectedIndexChanged);
            this.comboBoxPuttyProfile.Validating += new System.ComponentModel.CancelEventHandler(this.comboBoxPuttyProfile_Validating);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.panelSerialConnSettings);
            this.groupBox2.Controls.Add(this.panelIpConnection);
            this.groupBox2.Controls.Add(this.labelSessionname);
            this.groupBox2.Controls.Add(this.textBoxSessionName);
            this.groupBox2.Location = new System.Drawing.Point(14, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(485, 109);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            // 
            // panelSerialConnSettings
            // 
            this.panelSerialConnSettings.Controls.Add(this.labelSerialLine);
            this.panelSerialConnSettings.Controls.Add(this.comboBoxSerialLine);
            this.panelSerialConnSettings.Controls.Add(this.labelSerialSpeed);
            this.panelSerialConnSettings.Controls.Add(this.comboBoxSerialSpeed);
            this.panelSerialConnSettings.Controls.Add(this.labelSerialDataBits);
            this.panelSerialConnSettings.Controls.Add(this.comboBoxSerialDataBits);
            this.panelSerialConnSettings.Controls.Add(this.labelSerialStopBits);
            this.panelSerialConnSettings.Controls.Add(this.comboBoxSerialStopBits);
            this.panelSerialConnSettings.Controls.Add(this.labelSerialParity);
            this.panelSerialConnSettings.Controls.Add(this.comboBoxSerialParity);
            this.panelSerialConnSettings.Controls.Add(this.labelSerialFlowControl);
            this.panelSerialConnSettings.Controls.Add(this.comboBoxSerialFlowCtrl);
            this.panelSerialConnSettings.Location = new System.Drawing.Point(3, 56);
            this.panelSerialConnSettings.Name = "panelSerialConnSettings";
            this.panelSerialConnSettings.Size = new System.Drawing.Size(479, 53);
            this.panelSerialConnSettings.TabIndex = 7;
            // 
            // labelSerialLine
            // 
            this.labelSerialLine.AutoSize = true;
            this.labelSerialLine.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSerialLine.Location = new System.Drawing.Point(6, 3);
            this.labelSerialLine.Name = "labelSerialLine";
            this.labelSerialLine.Size = new System.Drawing.Size(60, 15);
            this.labelSerialLine.TabIndex = 2;
            this.labelSerialLine.Text = "Serial Line";
            // 
            // comboBoxSerialLine
            // 
            this.comboBoxSerialLine.FormattingEnabled = true;
            this.comboBoxSerialLine.Location = new System.Drawing.Point(6, 24);
            this.comboBoxSerialLine.Name = "comboBoxSerialLine";
            this.comboBoxSerialLine.Size = new System.Drawing.Size(90, 21);
            this.comboBoxSerialLine.TabIndex = 6;
            // 
            // labelSerialSpeed
            // 
            this.labelSerialSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSerialSpeed.AutoSize = true;
            this.labelSerialSpeed.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSerialSpeed.Location = new System.Drawing.Point(100, 6);
            this.labelSerialSpeed.Name = "labelSerialSpeed";
            this.labelSerialSpeed.Size = new System.Drawing.Size(39, 15);
            this.labelSerialSpeed.TabIndex = 5;
            this.labelSerialSpeed.Text = "Speed";
            // 
            // comboBoxSerialSpeed
            // 
            this.comboBoxSerialSpeed.FormattingEnabled = true;
            this.comboBoxSerialSpeed.Location = new System.Drawing.Point(100, 24);
            this.comboBoxSerialSpeed.Name = "comboBoxSerialSpeed";
            this.comboBoxSerialSpeed.Size = new System.Drawing.Size(65, 21);
            this.comboBoxSerialSpeed.TabIndex = 17;
            // 
            // labelSerialDataBits
            // 
            this.labelSerialDataBits.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSerialDataBits.AutoSize = true;
            this.labelSerialDataBits.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSerialDataBits.Location = new System.Drawing.Point(170, 6);
            this.labelSerialDataBits.Name = "labelSerialDataBits";
            this.labelSerialDataBits.Size = new System.Drawing.Size(53, 15);
            this.labelSerialDataBits.TabIndex = 7;
            this.labelSerialDataBits.Text = "Data Bits";
            // 
            // comboBoxSerialDataBits
            // 
            this.comboBoxSerialDataBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSerialDataBits.FormattingEnabled = true;
            this.comboBoxSerialDataBits.Location = new System.Drawing.Point(170, 24);
            this.comboBoxSerialDataBits.Name = "comboBoxSerialDataBits";
            this.comboBoxSerialDataBits.Size = new System.Drawing.Size(65, 21);
            this.comboBoxSerialDataBits.TabIndex = 15;
            // 
            // labelSerialStopBits
            // 
            this.labelSerialStopBits.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSerialStopBits.AutoSize = true;
            this.labelSerialStopBits.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSerialStopBits.Location = new System.Drawing.Point(240, 6);
            this.labelSerialStopBits.Name = "labelSerialStopBits";
            this.labelSerialStopBits.Size = new System.Drawing.Size(53, 15);
            this.labelSerialStopBits.TabIndex = 8;
            this.labelSerialStopBits.Text = "Stop Bits";
            // 
            // comboBoxSerialStopBits
            // 
            this.comboBoxSerialStopBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSerialStopBits.FormattingEnabled = true;
            this.comboBoxSerialStopBits.Location = new System.Drawing.Point(240, 24);
            this.comboBoxSerialStopBits.Name = "comboBoxSerialStopBits";
            this.comboBoxSerialStopBits.Size = new System.Drawing.Size(65, 21);
            this.comboBoxSerialStopBits.TabIndex = 16;
            // 
            // labelSerialParity
            // 
            this.labelSerialParity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSerialParity.AutoSize = true;
            this.labelSerialParity.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSerialParity.Location = new System.Drawing.Point(310, 6);
            this.labelSerialParity.Name = "labelSerialParity";
            this.labelSerialParity.Size = new System.Drawing.Size(37, 15);
            this.labelSerialParity.TabIndex = 9;
            this.labelSerialParity.Text = "Parity";
            // 
            // comboBoxSerialParity
            // 
            this.comboBoxSerialParity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSerialParity.FormattingEnabled = true;
            this.comboBoxSerialParity.Location = new System.Drawing.Point(310, 24);
            this.comboBoxSerialParity.Name = "comboBoxSerialParity";
            this.comboBoxSerialParity.Size = new System.Drawing.Size(65, 21);
            this.comboBoxSerialParity.TabIndex = 13;
            // 
            // labelSerialFlowControl
            // 
            this.labelSerialFlowControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSerialFlowControl.AutoSize = true;
            this.labelSerialFlowControl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSerialFlowControl.Location = new System.Drawing.Point(380, 6);
            this.labelSerialFlowControl.Name = "labelSerialFlowControl";
            this.labelSerialFlowControl.Size = new System.Drawing.Size(75, 15);
            this.labelSerialFlowControl.TabIndex = 10;
            this.labelSerialFlowControl.Text = "Flow Control";
            // 
            // comboBoxSerialFlowCtrl
            // 
            this.comboBoxSerialFlowCtrl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSerialFlowCtrl.FormattingEnabled = true;
            this.comboBoxSerialFlowCtrl.Location = new System.Drawing.Point(380, 24);
            this.comboBoxSerialFlowCtrl.Name = "comboBoxSerialFlowCtrl";
            this.comboBoxSerialFlowCtrl.Size = new System.Drawing.Size(85, 21);
            this.comboBoxSerialFlowCtrl.TabIndex = 14;
            // 
            // panelIpConnection
            // 
            this.panelIpConnection.Controls.Add(this.labelHostName);
            this.panelIpConnection.Controls.Add(this.textBoxHostname);
            this.panelIpConnection.Controls.Add(this.labelPort);
            this.panelIpConnection.Controls.Add(this.textBoxPort);
            this.panelIpConnection.Location = new System.Drawing.Point(6, 56);
            this.panelIpConnection.Name = "panelIpConnection";
            this.panelIpConnection.Size = new System.Drawing.Size(476, 53);
            this.panelIpConnection.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(14, 205);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(93, 15);
            this.label5.TabIndex = 12;
            this.label5.Text = "Login Username";
            // 
            // textBoxUsername
            // 
            this.textBoxUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxUsername.Location = new System.Drawing.Point(150, 203);
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.Size = new System.Drawing.Size(349, 20);
            this.textBoxUsername.TabIndex = 5;
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            // 
            // buttonImageSelect
            // 
            this.buttonImageSelect.Location = new System.Drawing.Point(14, 395);
            this.buttonImageSelect.Name = "buttonImageSelect";
            this.buttonImageSelect.Size = new System.Drawing.Size(29, 29);
            this.buttonImageSelect.TabIndex = 7;
            this.buttonImageSelect.UseVisualStyleBackColor = true;
            this.buttonImageSelect.Click += new System.EventHandler(this.buttonImageSelect_Click);
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowse.Location = new System.Drawing.Point(424, 253);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowse.TabIndex = 15;
            this.buttonBrowse.Text = "Browse";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // textBoxSPSLScriptFile
            // 
            this.textBoxSPSLScriptFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSPSLScriptFile.Location = new System.Drawing.Point(150, 255);
            this.textBoxSPSLScriptFile.Name = "textBoxSPSLScriptFile";
            this.textBoxSPSLScriptFile.Size = new System.Drawing.Size(187, 20);
            this.textBoxSPSLScriptFile.TabIndex = 16;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(14, 259);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "SPSL Script";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "spsl";
            this.openFileDialog1.Filter = "script files (*.spsl)|*.spsl|txt files (*.txt)|*.txt|All files (*.*)|*.*";
            this.openFileDialog1.Title = "Open SPSL Script";
            // 
            // buttonClearSPSLFile
            // 
            this.buttonClearSPSLFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClearSPSLFile.Location = new System.Drawing.Point(343, 253);
            this.buttonClearSPSLFile.Name = "buttonClearSPSLFile";
            this.buttonClearSPSLFile.Size = new System.Drawing.Size(75, 23);
            this.buttonClearSPSLFile.TabIndex = 18;
            this.buttonClearSPSLFile.Text = "Clear";
            this.buttonClearSPSLFile.UseVisualStyleBackColor = true;
            this.buttonClearSPSLFile.Click += new System.EventHandler(this.buttonClearSPSLFile_Click);
            // 
            // groupBoxFileTransferOptions
            // 
            this.groupBoxFileTransferOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxFileTransferOptions.Controls.Add(this.textBoxRemotePathSesion);
            this.groupBoxFileTransferOptions.Controls.Add(this.lbRemotePath);
            this.groupBoxFileTransferOptions.Controls.Add(this.buttonBrowseLocalPath);
            this.groupBoxFileTransferOptions.Controls.Add(this.textBoxLocalPathSesion);
            this.groupBoxFileTransferOptions.Controls.Add(this.lbLocalPath);
            this.groupBoxFileTransferOptions.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.groupBoxFileTransferOptions.Location = new System.Drawing.Point(14, 289);
            this.groupBoxFileTransferOptions.Name = "groupBoxFileTransferOptions";
            this.groupBoxFileTransferOptions.Size = new System.Drawing.Size(485, 100);
            this.groupBoxFileTransferOptions.TabIndex = 19;
            this.groupBoxFileTransferOptions.TabStop = false;
            this.groupBoxFileTransferOptions.Text = "File Transfer Options";
            // 
            // textBoxRemotePathSesion
            // 
            this.textBoxRemotePathSesion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRemotePathSesion.Location = new System.Drawing.Point(91, 61);
            this.textBoxRemotePathSesion.Name = "textBoxRemotePathSesion";
            this.textBoxRemotePathSesion.Size = new System.Drawing.Size(379, 23);
            this.textBoxRemotePathSesion.TabIndex = 18;
            // 
            // lbRemotePath
            // 
            this.lbRemotePath.AutoSize = true;
            this.lbRemotePath.Location = new System.Drawing.Point(6, 64);
            this.lbRemotePath.Name = "lbRemotePath";
            this.lbRemotePath.Size = new System.Drawing.Size(75, 15);
            this.lbRemotePath.TabIndex = 17;
            this.lbRemotePath.Text = "Remote Path";
            // 
            // buttonBrowseLocalPath
            // 
            this.buttonBrowseLocalPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseLocalPath.Location = new System.Drawing.Point(395, 28);
            this.buttonBrowseLocalPath.Name = "buttonBrowseLocalPath";
            this.buttonBrowseLocalPath.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowseLocalPath.TabIndex = 16;
            this.buttonBrowseLocalPath.Text = "Browse";
            this.buttonBrowseLocalPath.UseVisualStyleBackColor = true;
            this.buttonBrowseLocalPath.Click += new System.EventHandler(this.buttonBrowseLocalPath_Click);
            // 
            // textBoxLocalPathSesion
            // 
            this.textBoxLocalPathSesion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxLocalPathSesion.Location = new System.Drawing.Point(90, 29);
            this.textBoxLocalPathSesion.Name = "textBoxLocalPathSesion";
            this.textBoxLocalPathSesion.Size = new System.Drawing.Size(294, 23);
            this.textBoxLocalPathSesion.TabIndex = 1;
            // 
            // lbLocalPath
            // 
            this.lbLocalPath.AutoSize = true;
            this.lbLocalPath.Location = new System.Drawing.Point(6, 32);
            this.lbLocalPath.Name = "lbLocalPath";
            this.lbLocalPath.Size = new System.Drawing.Size(62, 15);
            this.lbLocalPath.TabIndex = 0;
            this.lbLocalPath.Text = "Local Path";
            // 
            // dlgEditSession
            // 
            this.AcceptButton = this.buttonSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(515, 431);
            this.Controls.Add(this.groupBoxFileTransferOptions);
            this.Controls.Add(this.buttonClearSPSLFile);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBoxSPSLScriptFile);
            this.Controls.Add(this.buttonBrowse);
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
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "dlgEditSession";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create New Session";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panelSerialConnSettings.ResumeLayout(false);
            this.panelSerialConnSettings.PerformLayout();
            this.panelIpConnection.ResumeLayout(false);
            this.panelIpConnection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.groupBoxFileTransferOptions.ResumeLayout(false);
            this.groupBoxFileTransferOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxSessionName;
        private System.Windows.Forms.Label labelSessionname;
        private System.Windows.Forms.Label labelHostName;
        private System.Windows.Forms.TextBox textBoxHostname;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.Label labelPort;
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
        private System.Windows.Forms.RadioButton radioButtonVNC;
        private System.Windows.Forms.Button buttonImageSelect;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxSPSLScriptFile;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button buttonClearSPSLFile;
        private System.Windows.Forms.GroupBox groupBoxFileTransferOptions;
        private System.Windows.Forms.TextBox textBoxRemotePathSesion;
        private System.Windows.Forms.Label lbRemotePath;
        private System.Windows.Forms.Button buttonBrowseLocalPath;
        private System.Windows.Forms.TextBox textBoxLocalPathSesion;
        private System.Windows.Forms.Label lbLocalPath;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Panel panelIpConnection;
        private System.Windows.Forms.Panel panelSerialConnSettings;
        private System.Windows.Forms.Label labelSerialLine;
        private System.Windows.Forms.Label labelSerialSpeed;
        private System.Windows.Forms.ComboBox comboBoxSerialLine;
        private System.Windows.Forms.Label labelSerialDataBits;
        private System.Windows.Forms.Label labelSerialStopBits;
        private System.Windows.Forms.Label labelSerialParity;
        private System.Windows.Forms.Label labelSerialFlowControl;
        private System.Windows.Forms.ComboBox comboBoxSerialSpeed;
        private System.Windows.Forms.ComboBox comboBoxSerialDataBits;
        private System.Windows.Forms.ComboBox comboBoxSerialStopBits;
        private System.Windows.Forms.ComboBox comboBoxSerialParity;
        private System.Windows.Forms.ComboBox comboBoxSerialFlowCtrl;
    }
}
