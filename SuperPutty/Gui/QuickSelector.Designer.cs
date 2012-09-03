namespace SuperPutty.Gui
{
    partial class QuickSelector
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
            this.comboBoxData = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // comboBoxData
            // 
            this.comboBoxData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBoxData.FormattingEnabled = true;
            this.comboBoxData.Location = new System.Drawing.Point(0, 0);
            this.comboBoxData.Name = "comboBoxData";
            this.comboBoxData.Size = new System.Drawing.Size(345, 21);
            this.comboBoxData.TabIndex = 0;
            this.comboBoxData.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboBoxData_KeyDown);
            // 
            // QuickSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(345, 23);
            this.Controls.Add(this.comboBoxData);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "QuickSelector";
            this.Text = "QuickSelector";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxData;
    }
}