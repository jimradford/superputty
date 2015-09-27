namespace SuperPutty
{
    partial class dlgScriptEditor
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
            this.buttonSendScript = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBoxSript = new System.Windows.Forms.TextBox();
            this.buttonLoadScript = new System.Windows.Forms.Button();
            this.buttonSaveScript = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonSendScript
            // 
            this.buttonSendScript.Location = new System.Drawing.Point(229, 226);
            this.buttonSendScript.Name = "buttonSendScript";
            this.buttonSendScript.Size = new System.Drawing.Size(43, 23);
            this.buttonSendScript.TabIndex = 0;
            this.buttonSendScript.Text = "Send";
            this.buttonSendScript.UseVisualStyleBackColor = true;
            this.buttonSendScript.Click += new System.EventHandler(this.buttonRunScript_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.panel1.Controls.Add(this.textBoxSript);
            this.panel1.Location = new System.Drawing.Point(13, 13);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(259, 207);
            this.panel1.TabIndex = 1;
            // 
            // textBoxSript
            // 
            this.textBoxSript.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.textBoxSript.Location = new System.Drawing.Point(3, 3);
            this.textBoxSript.Multiline = true;
            this.textBoxSript.Name = "textBoxSript";
            this.textBoxSript.Size = new System.Drawing.Size(253, 201);
            this.textBoxSript.TabIndex = 0;
            // 
            // buttonLoadScript
            // 
            this.buttonLoadScript.Location = new System.Drawing.Point(12, 226);
            this.buttonLoadScript.Name = "buttonLoadScript";
            this.buttonLoadScript.Size = new System.Drawing.Size(43, 23);
            this.buttonLoadScript.TabIndex = 2;
            this.buttonLoadScript.Text = "Load";
            this.buttonLoadScript.UseVisualStyleBackColor = true;
            this.buttonLoadScript.Click += new System.EventHandler(this.buttonLoadScript_Click);
            // 
            // buttonSaveScript
            // 
            this.buttonSaveScript.Location = new System.Drawing.Point(61, 226);
            this.buttonSaveScript.Name = "buttonSaveScript";
            this.buttonSaveScript.Size = new System.Drawing.Size(43, 23);
            this.buttonSaveScript.TabIndex = 3;
            this.buttonSaveScript.Text = "Save";
            this.buttonSaveScript.UseVisualStyleBackColor = true;
            this.buttonSaveScript.Click += new System.EventHandler(this.buttonSaveScript_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "scr";
            this.openFileDialog1.Filter = "script files (*.spsl)|*.spsl|txt files (*.txt)|*.txt|All files (*.*)|*.*";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "spsl";
            this.saveFileDialog1.Filter = "script files (*.spsl)|*.spsl|txt files (*.txt)|*.txt|All files (*.*)|*.*";
            // 
            // dlgScriptEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.buttonSaveScript);
            this.Controls.Add(this.buttonLoadScript);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.buttonSendScript);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "dlgScriptEditor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Script Editor";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonSendScript;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBoxSript;
        private System.Windows.Forms.Button buttonLoadScript;
        private System.Windows.Forms.Button buttonSaveScript;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}