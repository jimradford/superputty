namespace SuperPutty
{
    partial class dlgFindPutty
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(dlgFindPutty));
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonBrowsePutty = new System.Windows.Forms.Button();
            this.buttonBrowsePscp = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxPuttyLocation = new System.Windows.Forms.TextBox();
            this.textBoxPscpLocation = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.textBoxSettingsFolder = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonBrowseLayoutsFolder = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxLayouts = new System.Windows.Forms.ComboBox();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.checkRestoreWindow = new System.Windows.Forms.CheckBox();
            this.checkConstrainPuttyDocking = new System.Windows.Forms.CheckBox();
            this.checkSingleInstanceMode = new System.Windows.Forms.CheckBox();
            this.checkExitConfirmation = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.checkExpandTree = new System.Windows.Forms.CheckBox();
            this.checkMinimizeToTray = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(388, 403);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 20;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonBrowsePutty
            // 
            this.buttonBrowsePutty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowsePutty.Location = new System.Drawing.Point(469, 111);
            this.buttonBrowsePutty.Name = "buttonBrowsePutty";
            this.buttonBrowsePutty.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowsePutty.TabIndex = 1;
            this.buttonBrowsePutty.Text = "Browse";
            this.buttonBrowsePutty.UseVisualStyleBackColor = true;
            this.buttonBrowsePutty.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // buttonBrowsePscp
            // 
            this.buttonBrowsePscp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowsePscp.Location = new System.Drawing.Point(469, 140);
            this.buttonBrowsePscp.Name = "buttonBrowsePscp";
            this.buttonBrowsePscp.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowsePscp.TabIndex = 3;
            this.buttonBrowsePscp.Text = "Browse";
            this.buttonBrowsePscp.UseVisualStyleBackColor = true;
            this.buttonBrowsePscp.Click += new System.EventHandler(this.buttonBrowsePscp_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 116);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(162, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "putty.exe Location (Required)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 144);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "pscp.exe Location";
            // 
            // textBoxPuttyLocation
            // 
            this.textBoxPuttyLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPuttyLocation.Location = new System.Drawing.Point(180, 113);
            this.textBoxPuttyLocation.Name = "textBoxPuttyLocation";
            this.textBoxPuttyLocation.Size = new System.Drawing.Size(283, 20);
            this.textBoxPuttyLocation.TabIndex = 0;
            // 
            // textBoxPscpLocation
            // 
            this.textBoxPscpLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPscpLocation.Location = new System.Drawing.Point(180, 142);
            this.textBoxPscpLocation.Name = "textBoxPscpLocation";
            this.textBoxPscpLocation.Size = new System.Drawing.Size(283, 20);
            this.textBoxPscpLocation.TabIndex = 2;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // richTextBox1
            // 
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Cursor = System.Windows.Forms.Cursors.Default;
            this.richTextBox1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.Location = new System.Drawing.Point(12, 12);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(485, 95);
            this.richTextBox1.TabIndex = 8;
            this.richTextBox1.TabStop = false;
            this.richTextBox1.Text = resources.GetString("richTextBox1.Text");
            // 
            // textBoxSettingsFolder
            // 
            this.textBoxSettingsFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSettingsFolder.Location = new System.Drawing.Point(180, 172);
            this.textBoxSettingsFolder.Name = "textBoxSettingsFolder";
            this.textBoxSettingsFolder.Size = new System.Drawing.Size(283, 20);
            this.textBoxSettingsFolder.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 173);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(143, 15);
            this.label3.TabIndex = 9;
            this.label3.Text = "Settings Folder (Required)";
            // 
            // buttonBrowseLayoutsFolder
            // 
            this.buttonBrowseLayoutsFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseLayoutsFolder.Location = new System.Drawing.Point(469, 169);
            this.buttonBrowseLayoutsFolder.Name = "buttonBrowseLayoutsFolder";
            this.buttonBrowseLayoutsFolder.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowseLayoutsFolder.TabIndex = 5;
            this.buttonBrowseLayoutsFolder.Text = "Browse";
            this.buttonBrowseLayoutsFolder.UseVisualStyleBackColor = true;
            this.buttonBrowseLayoutsFolder.Click += new System.EventHandler(this.buttonBrowseLayoutsFolder_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 201);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 15);
            this.label4.TabIndex = 12;
            this.label4.Text = "Default Layout";
            // 
            // comboBoxLayouts
            // 
            this.comboBoxLayouts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxLayouts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLayouts.FormattingEnabled = true;
            this.comboBoxLayouts.Location = new System.Drawing.Point(180, 199);
            this.comboBoxLayouts.Name = "comboBoxLayouts";
            this.comboBoxLayouts.Size = new System.Drawing.Size(283, 21);
            this.comboBoxLayouts.TabIndex = 6;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(469, 403);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 21;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(12, 232);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(94, 15);
            this.label5.TabIndex = 23;
            this.label5.Text = "User Preferences";
            // 
            // checkRestoreWindow
            // 
            this.checkRestoreWindow.AutoSize = true;
            this.checkRestoreWindow.Location = new System.Drawing.Point(180, 255);
            this.checkRestoreWindow.Name = "checkRestoreWindow";
            this.checkRestoreWindow.Size = new System.Drawing.Size(193, 17);
            this.checkRestoreWindow.TabIndex = 26;
            this.checkRestoreWindow.Text = "Restore Window Size and Location";
            this.checkRestoreWindow.UseVisualStyleBackColor = true;
            // 
            // checkConstrainPuttyDocking
            // 
            this.checkConstrainPuttyDocking.AutoSize = true;
            this.checkConstrainPuttyDocking.Location = new System.Drawing.Point(180, 318);
            this.checkConstrainPuttyDocking.Name = "checkConstrainPuttyDocking";
            this.checkConstrainPuttyDocking.Size = new System.Drawing.Size(193, 17);
            this.checkConstrainPuttyDocking.TabIndex = 25;
            this.checkConstrainPuttyDocking.Text = "Restrict Content to Document Tabs";
            this.checkConstrainPuttyDocking.UseVisualStyleBackColor = true;
            // 
            // checkSingleInstanceMode
            // 
            this.checkSingleInstanceMode.AutoSize = true;
            this.checkSingleInstanceMode.Location = new System.Drawing.Point(180, 232);
            this.checkSingleInstanceMode.Name = "checkSingleInstanceMode";
            this.checkSingleInstanceMode.Size = new System.Drawing.Size(254, 17);
            this.checkSingleInstanceMode.TabIndex = 24;
            this.checkSingleInstanceMode.Text = "Only allow single instance of SuperPuTTY to run";
            this.checkSingleInstanceMode.UseVisualStyleBackColor = true;
            // 
            // checkExitConfirmation
            // 
            this.checkExitConfirmation.AutoSize = true;
            this.checkExitConfirmation.Location = new System.Drawing.Point(180, 278);
            this.checkExitConfirmation.Name = "checkExitConfirmation";
            this.checkExitConfirmation.Size = new System.Drawing.Size(104, 17);
            this.checkExitConfirmation.TabIndex = 27;
            this.checkExitConfirmation.Text = "Exit Confirmation";
            this.checkExitConfirmation.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(12, 320);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(90, 15);
            this.label6.TabIndex = 28;
            this.label6.Text = "GUI Preferences";
            // 
            // checkExpandTree
            // 
            this.checkExpandTree.AutoSize = true;
            this.checkExpandTree.Location = new System.Drawing.Point(180, 341);
            this.checkExpandTree.Name = "checkExpandTree";
            this.checkExpandTree.Size = new System.Drawing.Size(187, 17);
            this.checkExpandTree.TabIndex = 29;
            this.checkExpandTree.Text = "Expand Sessions Tree on Start up";
            this.checkExpandTree.UseVisualStyleBackColor = true;
            // 
            // checkMinimizeToTray
            // 
            this.checkMinimizeToTray.AutoSize = true;
            this.checkMinimizeToTray.Location = new System.Drawing.Point(180, 364);
            this.checkMinimizeToTray.Name = "checkMinimizeToTray";
            this.checkMinimizeToTray.Size = new System.Drawing.Size(106, 17);
            this.checkMinimizeToTray.TabIndex = 30;
            this.checkMinimizeToTray.Text = "Minimize To Tray";
            this.checkMinimizeToTray.UseVisualStyleBackColor = true;
            // 
            // dlgFindPutty
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(556, 438);
            this.Controls.Add(this.checkMinimizeToTray);
            this.Controls.Add(this.checkExpandTree);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.checkExitConfirmation);
            this.Controls.Add(this.checkRestoreWindow);
            this.Controls.Add(this.checkConstrainPuttyDocking);
            this.Controls.Add(this.checkSingleInstanceMode);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.comboBoxLayouts);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.buttonBrowseLayoutsFolder);
            this.Controls.Add(this.textBoxSettingsFolder);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.textBoxPscpLocation);
            this.Controls.Add(this.textBoxPuttyLocation);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonBrowsePscp);
            this.Controls.Add(this.buttonBrowsePutty);
            this.Controls.Add(this.buttonOk);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "dlgFindPutty";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SuperPuTTY Options";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonBrowsePutty;
        private System.Windows.Forms.Button buttonBrowsePscp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxPuttyLocation;
        private System.Windows.Forms.TextBox textBoxPscpLocation;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.TextBox textBoxSettingsFolder;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonBrowseLayoutsFolder;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxLayouts;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox checkRestoreWindow;
        private System.Windows.Forms.CheckBox checkConstrainPuttyDocking;
        private System.Windows.Forms.CheckBox checkSingleInstanceMode;
        private System.Windows.Forms.CheckBox checkExitConfirmation;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox checkExpandTree;
        private System.Windows.Forms.CheckBox checkMinimizeToTray;
    }
}