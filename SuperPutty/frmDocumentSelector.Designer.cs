namespace SuperPutty
{
    partial class frmDocumentSelector
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkSendToVisible = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.listViewDocs = new System.Windows.Forms.ListView();
            this.colTab = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSessionId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colNum = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.checkSendToVisible);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 223);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(404, 31);
            this.panel1.TabIndex = 0;
            // 
            // checkSendToVisible
            // 
            this.checkSendToVisible.AutoSize = true;
            this.checkSendToVisible.Location = new System.Drawing.Point(7, 9);
            this.checkSendToVisible.Name = "checkSendToVisible";
            this.checkSendToVisible.Size = new System.Drawing.Size(151, 17);
            this.checkSendToVisible.TabIndex = 2;
            this.checkSendToVisible.Text = "Send To Visible Tabs Only";
            this.checkSendToVisible.UseVisualStyleBackColor = true;
            this.checkSendToVisible.CheckedChanged += new System.EventHandler(this.checkSendToVisible_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(326, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(245, 5);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // listViewDocs
            // 
            this.listViewDocs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colNum,
            this.colTab,
            this.colSessionId});
            this.listViewDocs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewDocs.FullRowSelect = true;
            this.listViewDocs.Location = new System.Drawing.Point(0, 0);
            this.listViewDocs.Name = "listViewDocs";
            this.listViewDocs.Size = new System.Drawing.Size(404, 223);
            this.listViewDocs.TabIndex = 1;
            this.listViewDocs.UseCompatibleStateImageBehavior = false;
            this.listViewDocs.View = System.Windows.Forms.View.Details;
            // 
            // colTab
            // 
            this.colTab.Text = "Tab";
            this.colTab.Width = 90;
            // 
            // colSessionId
            // 
            this.colSessionId.Text = "Session Id";
            this.colSessionId.Width = 285;
            // 
            // colNum
            // 
            this.colNum.Text = "#";
            this.colNum.Width = 25;
            // 
            // frmDocumentSelector
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(404, 254);
            this.Controls.Add(this.listViewDocs);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "frmDocumentSelector";
            this.ShowInTaskbar = false;
            this.Text = "Select Documents";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ListView listViewDocs;
        private System.Windows.Forms.ColumnHeader colTab;
        private System.Windows.Forms.ColumnHeader colSessionId;
        private System.Windows.Forms.CheckBox checkSendToVisible;
        private System.Windows.Forms.ColumnHeader colNum;

    }
}