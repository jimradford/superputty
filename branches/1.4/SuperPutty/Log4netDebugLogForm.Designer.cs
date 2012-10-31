namespace SuperPutty
{
    partial class Log4netLogViewer
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
            this.richTextBoxLogMessages = new System.Windows.Forms.RichTextBox();
            this.timerLogPull = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // richTextBoxLogMessages
            // 
            this.richTextBoxLogMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxLogMessages.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxLogMessages.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxLogMessages.Name = "richTextBoxLogMessages";
            this.richTextBoxLogMessages.Size = new System.Drawing.Size(740, 264);
            this.richTextBoxLogMessages.TabIndex = 0;
            this.richTextBoxLogMessages.Text = "";
            // 
            // timerLogPull
            // 
            this.timerLogPull.Interval = 500;
            this.timerLogPull.Tick += new System.EventHandler(this.timerLogPull_Tick);
            // 
            // Log4netLogViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(740, 264);
            this.Controls.Add(this.richTextBoxLogMessages);
            this.Name = "Log4netLogViewer";
            this.Text = "Log Viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Log4netLogViewer_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBoxLogMessages;
        private System.Windows.Forms.Timer timerLogPull;
    }
}