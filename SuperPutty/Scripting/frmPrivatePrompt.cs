using System;
using System.Drawing;
using System.Windows.Forms;

namespace SuperPutty
{
    public class frmPrivatePrompt : Form
    {
        private Label displayMessage;
        private TextBox textBox1;

        public frmPrivatePrompt(String arg)
        {
            InitializeComponent(arg);
        }

        private void InitializeComponent(String arg)
        {
            textBox1 = new TextBox();
            displayMessage = new Label();
            SuspendLayout();
            
            ClientSize = new Size(590, 185);

            displayMessage.Width = 310;
            displayMessage.Location = new Point(30,30);
            displayMessage.Text = arg;
            
            textBox1.UseSystemPasswordChar = true;
            textBox1.Width = ClientSize.Width - 50;
            textBox1.Location = new Point(20,ClientSize.Height - 55);

            Button okButton = new Button();
            okButton.DialogResult = DialogResult.OK;
            okButton.Name = "okButton";
            okButton.Size = new Size(90, 30);
            okButton.Text = "&OK";
            okButton.Anchor = AnchorStyles.Right;
            okButton.Location = new Point(ClientSize.Width - 95, 20);

            Button cancelButton = new Button();
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(90, 30);
            cancelButton.Text = "&Cancel";
            cancelButton.Anchor = AnchorStyles.Right;
            cancelButton.Location = new Point(ClientSize.Width - 95, 60);

            Controls.Add(displayMessage);
            Controls.Add(textBox1);
            Controls.Add(cancelButton);
            Controls.Add(okButton);

            FormBorderStyle = FormBorderStyle.FixedSingle;
            AcceptButton = okButton;
            CancelButton = cancelButton;
            Text = "SuperPutty";
            ResumeLayout(false);
            PerformLayout();
        }

        public String GetResult()
        {
            return textBox1.Text;
        }
    }
}