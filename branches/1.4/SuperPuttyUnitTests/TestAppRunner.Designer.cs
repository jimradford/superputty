namespace SuperPuttyUnitTests
{
    partial class TestAppRunner
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
            this.groupBoxAutoStart = new System.Windows.Forms.GroupBox();
            this.comboAutoStart = new System.Windows.Forms.ComboBox();
            this.groupBoxViews = new System.Windows.Forms.GroupBox();
            this.groupBoxAutoStart.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxAutoStart
            // 
            this.groupBoxAutoStart.Controls.Add(this.comboAutoStart);
            this.groupBoxAutoStart.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBoxAutoStart.Location = new System.Drawing.Point(3, 597);
            this.groupBoxAutoStart.Name = "groupBoxAutoStart";
            this.groupBoxAutoStart.Size = new System.Drawing.Size(345, 42);
            this.groupBoxAutoStart.TabIndex = 0;
            this.groupBoxAutoStart.TabStop = false;
            this.groupBoxAutoStart.Text = "Auto-Start";
            // 
            // comboAutoStart
            // 
            this.comboAutoStart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboAutoStart.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboAutoStart.FormattingEnabled = true;
            this.comboAutoStart.Location = new System.Drawing.Point(3, 16);
            this.comboAutoStart.Name = "comboAutoStart";
            this.comboAutoStart.Size = new System.Drawing.Size(339, 21);
            this.comboAutoStart.TabIndex = 0;
            // 
            // groupBoxViews
            // 
            this.groupBoxViews.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxViews.Location = new System.Drawing.Point(3, 3);
            this.groupBoxViews.Name = "groupBoxViews";
            this.groupBoxViews.Size = new System.Drawing.Size(345, 594);
            this.groupBoxViews.TabIndex = 1;
            this.groupBoxViews.TabStop = false;
            this.groupBoxViews.Text = "Test Views";
            // 
            // TestAppRunner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(351, 642);
            this.Controls.Add(this.groupBoxViews);
            this.Controls.Add(this.groupBoxAutoStart);
            this.Name = "TestAppRunner";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "TestAppRunner";
            this.groupBoxAutoStart.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxAutoStart;
        private System.Windows.Forms.ComboBox comboAutoStart;
        private System.Windows.Forms.GroupBox groupBoxViews;
    }
}