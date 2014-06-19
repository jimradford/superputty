using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SuperPutty.Utils
{
    /// <summary>
    /// Make a text box focus and select all on mouse click, tab in, etc.
    /// http://stackoverflow.com/questions/97459/automatically-select-all-text-on-focus-in-winforms-textbox
    /// </summary>
    public class TextBoxFocusHelper : IDisposable
    {
        public TextBoxFocusHelper(TextBox txt)
        {
            this.TextBox = txt;
            this.TextBox.GotFocus += TextBox_GotFocus;
            this.TextBox.MouseUp += TextBox_MouseUp;
            this.TextBox.Leave += TextBox_Leave;
        }

        void TextBox_MouseUp(object sender, MouseEventArgs e)
        {
            // Web browsers like Google Chrome select the text on mouse up.
            // They only do it if the textbox isn't already focused,
            // and if the user hasn't selected all text.
            if (!alreadyFocused && this.TextBox.SelectionLength == 0)
            {
                alreadyFocused = true;
                this.TextBox.SelectAll();
            }
        }

        void TextBox_GotFocus(object sender, EventArgs e)
        {
            // Select all text only if the mouse isn't down.
            // This makes tabbing to the textbox give focus.
            if (Control.MouseButtons == MouseButtons.None)
            {
                this.TextBox.SelectAll();
                alreadyFocused = true;
            }
        }

        void TextBox_Leave(object sender, EventArgs e)
        {
            alreadyFocused = false;
        }

        public void Dispose()
        {
            this.TextBox.GotFocus -= TextBox_GotFocus;
            this.TextBox.MouseUp -= TextBox_MouseUp;
            this.TextBox.Leave -= TextBox_Leave;
            this.TextBox = null;
        }

        bool alreadyFocused;
        public TextBox TextBox { get; private set; }

    }
}
