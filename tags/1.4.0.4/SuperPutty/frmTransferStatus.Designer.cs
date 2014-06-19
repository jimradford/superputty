namespace SuperPutty
{
    partial class frmTransferStatus
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTransferStatus));
            this.button1 = new System.Windows.Forms.Button();
            this.progressBarCurrentFile = new System.Windows.Forms.ProgressBar();
            this.progressBarOverall = new System.Windows.Forms.ProgressBar();
            this.labelCurrentFile = new System.Windows.Forms.Label();
            this.labelOverall = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.labelOverallPct = new System.Windows.Forms.Label();
            this.labelCurrentPercent = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(415, 121);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // progressBarCurrentFile
            // 
            this.progressBarCurrentFile.Location = new System.Drawing.Point(15, 29);
            this.progressBarCurrentFile.Name = "progressBarCurrentFile";
            this.progressBarCurrentFile.Size = new System.Drawing.Size(471, 23);
            this.progressBarCurrentFile.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBarCurrentFile.TabIndex = 1;
            // 
            // progressBarOverall
            // 
            this.progressBarOverall.Location = new System.Drawing.Point(12, 84);
            this.progressBarOverall.Name = "progressBarOverall";
            this.progressBarOverall.Size = new System.Drawing.Size(474, 23);
            this.progressBarOverall.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBarOverall.TabIndex = 3;
            this.progressBarOverall.Visible = false;
            // 
            // labelCurrentFile
            // 
            this.labelCurrentFile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCurrentFile.AutoSize = true;
            this.labelCurrentFile.BackColor = System.Drawing.SystemColors.Control;
            this.labelCurrentFile.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCurrentFile.Location = new System.Drawing.Point(15, 7);
            this.labelCurrentFile.Name = "labelCurrentFile";
            this.labelCurrentFile.Size = new System.Drawing.Size(0, 17);
            this.labelCurrentFile.TabIndex = 4;
            this.labelCurrentFile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelOverall
            // 
            this.labelOverall.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelOverall.AutoSize = true;
            this.labelOverall.BackColor = System.Drawing.SystemColors.Control;
            this.labelOverall.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelOverall.Location = new System.Drawing.Point(15, 62);
            this.labelOverall.Name = "labelOverall";
            this.labelOverall.Size = new System.Drawing.Size(0, 17);
            this.labelOverall.TabIndex = 5;
            this.labelOverall.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelOverall.Visible = false;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(12, 125);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(131, 17);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.Text = "Close When Complete";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // labelOverallPct
            // 
            this.labelOverallPct.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelOverallPct.AutoSize = true;
            this.labelOverallPct.BackColor = System.Drawing.SystemColors.Control;
            this.labelOverallPct.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelOverallPct.Location = new System.Drawing.Point(492, 90);
            this.labelOverallPct.Name = "labelOverallPct";
            this.labelOverallPct.Size = new System.Drawing.Size(0, 17);
            this.labelOverallPct.TabIndex = 7;
            this.labelOverallPct.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelOverallPct.Visible = false;
            // 
            // labelCurrentPercent
            // 
            this.labelCurrentPercent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCurrentPercent.AutoSize = true;
            this.labelCurrentPercent.BackColor = System.Drawing.SystemColors.Control;
            this.labelCurrentPercent.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCurrentPercent.Location = new System.Drawing.Point(492, 29);
            this.labelCurrentPercent.Name = "labelCurrentPercent";
            this.labelCurrentPercent.Size = new System.Drawing.Size(0, 17);
            this.labelCurrentPercent.TabIndex = 8;
            this.labelCurrentPercent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // frmTransferStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 149);
            this.Controls.Add(this.labelCurrentPercent);
            this.Controls.Add(this.labelOverallPct);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.labelOverall);
            this.Controls.Add(this.labelCurrentFile);
            this.Controls.Add(this.progressBarOverall);
            this.Controls.Add(this.progressBarCurrentFile);
            this.Controls.Add(this.button1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmTransferStatus";
            this.Text = "File Transfer Progress";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ProgressBar progressBarCurrentFile;
        private System.Windows.Forms.ProgressBar progressBarOverall;
        private System.Windows.Forms.Label labelCurrentFile;
        private System.Windows.Forms.Label labelOverall;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label labelOverallPct;
        private System.Windows.Forms.Label labelCurrentPercent;
    }
}