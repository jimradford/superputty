namespace SuperPutty
{
    partial class SessionDetail
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
            this.sessionDetailPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // sessionDetailPropertyGrid
            // 
            this.sessionDetailPropertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sessionDetailPropertyGrid.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.sessionDetailPropertyGrid.Location = new System.Drawing.Point(0, 1);
            this.sessionDetailPropertyGrid.Name = "sessionDetailPropertyGrid";
            this.sessionDetailPropertyGrid.Size = new System.Drawing.Size(290, 247);
            this.sessionDetailPropertyGrid.TabIndex = 0;
            this.sessionDetailPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.sessionDetailPropertyGrid_PropertyValueChanged);
            // 
            // SessionDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(289, 249);
            this.Controls.Add(this.sessionDetailPropertyGrid);
            this.Name = "SessionDetail";
            this.Text = "Session Detail";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid sessionDetailPropertyGrid;



    }
}