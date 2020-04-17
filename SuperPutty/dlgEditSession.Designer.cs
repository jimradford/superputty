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
            this.comboBoxProto = new System.Windows.Forms.ComboBox();
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
            this.textBoxHostname.Location = new System.Drawing.Point(6, 74);
            this.textBoxHostname.Name = "textBoxHostname";
            this.textBoxHostname.Size = new System.Drawing.Size(361, 20);
            this.textBoxHostname.TabIndex = 1;
            this.textBoxHostname.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxHostname_Validating);
            // 
            // textBoxPort
            // 
            this.textBoxPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPort.Location = new System.Drawing.Point(390, 74);
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
            this.label3.Location = new System.Drawing.Point(387, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "TCP Port";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.comboBoxProto);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(14, 117);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(485, 49);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Connection type:";
            // 
            // comboBoxProto
            // 
            this.comboBoxProto.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxProto.FormattingEnabled = true;
            this.comboBoxProto.Location = new System.Drawing.Point(136, 19);
            this.comboBoxProto.Name = "comboBoxProto";
            this.comboBoxProto.Size = new System.Drawing.Size(349, 21);
            this.comboBoxProto.TabIndex = 1;
            this.comboBoxProto.SelectedIndexChanged += new System.EventHandler(this.comboBoxPuttyProfile_SelectedIndexChanged);
            this.comboBoxProto.Validating += new System.ComponentModel.CancelEventHandler(this.comboBoxPuttyProfile_Validating);
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
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.textBoxSessionName);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.textBoxHostname);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.textBoxPort);
            this.groupBox2.Location = new System.Drawing.Point(14, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(485, 109);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
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
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.groupBoxFileTransferOptions.ResumeLayout(false);
            this.groupBoxFileTransferOptions.PerformLayout();
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
        private System.Windows.Forms.ComboBox comboBoxProto;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxPuttyProfile;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxExtraArgs;
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
    }
}
