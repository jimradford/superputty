namespace SuperPutty
{
    partial class dlgSelectPW
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
            this.rbUseMasterPassword = new System.Windows.Forms.RadioButton();
            this.rbOtherPassword = new System.Windows.Forms.RadioButton();
            this.tbOtherPassword = new System.Windows.Forms.TextBox();
            this.btSelectPasswordOK = new System.Windows.Forms.Button();
            this.btSelectPasswordCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbWPw = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rbUseMasterPassword
            // 
            this.rbUseMasterPassword.AutoSize = true;
            this.rbUseMasterPassword.Checked = true;
            this.rbUseMasterPassword.Location = new System.Drawing.Point(6, 42);
            this.rbUseMasterPassword.Name = "rbUseMasterPassword";
            this.rbUseMasterPassword.Size = new System.Drawing.Size(128, 17);
            this.rbUseMasterPassword.TabIndex = 0;
            this.rbUseMasterPassword.Text = "Use Master Password";
            this.rbUseMasterPassword.UseVisualStyleBackColor = true;
            // 
            // rbOtherPassword
            // 
            this.rbOtherPassword.AutoSize = true;
            this.rbOtherPassword.Location = new System.Drawing.Point(6, 65);
            this.rbOtherPassword.Name = "rbOtherPassword";
            this.rbOtherPassword.Size = new System.Drawing.Size(103, 17);
            this.rbOtherPassword.TabIndex = 1;
            this.rbOtherPassword.Text = "Select password";
            this.rbOtherPassword.UseVisualStyleBackColor = true;
            this.rbOtherPassword.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // tbOtherPassword
            // 
            this.tbOtherPassword.Location = new System.Drawing.Point(31, 88);
            this.tbOtherPassword.Name = "tbOtherPassword";
            this.tbOtherPassword.Size = new System.Drawing.Size(159, 20);
            this.tbOtherPassword.TabIndex = 2;
            // 
            // btSelectPasswordOK
            // 
            this.btSelectPasswordOK.Location = new System.Drawing.Point(84, 156);
            this.btSelectPasswordOK.Name = "btSelectPasswordOK";
            this.btSelectPasswordOK.Size = new System.Drawing.Size(75, 23);
            this.btSelectPasswordOK.TabIndex = 3;
            this.btSelectPasswordOK.Text = "Ok";
            this.btSelectPasswordOK.UseVisualStyleBackColor = true;
            this.btSelectPasswordOK.Click += new System.EventHandler(this.btSelectPasswordOK_Click);
            // 
            // btSelectPasswordCancel
            // 
            this.btSelectPasswordCancel.Location = new System.Drawing.Point(165, 155);
            this.btSelectPasswordCancel.Name = "btSelectPasswordCancel";
            this.btSelectPasswordCancel.Size = new System.Drawing.Size(75, 23);
            this.btSelectPasswordCancel.TabIndex = 5;
            this.btSelectPasswordCancel.Text = "Cancel";
            this.btSelectPasswordCancel.UseVisualStyleBackColor = true;
            this.btSelectPasswordCancel.Click += new System.EventHandler(this.btSelectPasswordCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbWPw);
            this.groupBox1.Controls.Add(this.rbUseMasterPassword);
            this.groupBox1.Controls.Add(this.rbOtherPassword);
            this.groupBox1.Controls.Add(this.tbOtherPassword);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(228, 137);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select a password for this operation";
            // 
            // rbWPw
            // 
            this.rbWPw.AutoSize = true;
            this.rbWPw.Location = new System.Drawing.Point(6, 19);
            this.rbWPw.Name = "rbWPw";
            this.rbWPw.Size = new System.Drawing.Size(110, 17);
            this.rbWPw.TabIndex = 3;
            this.rbWPw.Text = "Without password";
            this.rbWPw.UseVisualStyleBackColor = true;
            // 
            // frmSelectPW
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(253, 187);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btSelectPasswordCancel);
            this.Controls.Add(this.btSelectPasswordOK);
            this.Name = "frmSelectPW";
            this.Text = "Select password";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton rbUseMasterPassword;
        private System.Windows.Forms.RadioButton rbOtherPassword;
        private System.Windows.Forms.TextBox tbOtherPassword;
        private System.Windows.Forms.Button btSelectPasswordOK;
        private System.Windows.Forms.Button btSelectPasswordCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbWPw;
    }
}