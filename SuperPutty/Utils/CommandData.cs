using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SuperPutty.Utils
{
    public class CommandData
    {
        public CommandData(string command)
        {
            this.Command = command;
        }

        public CommandData(KeyEventArgs e)
        {
            this.KeyData = e;
        }

        public string Command { get; private set; }
        public KeyEventArgs KeyData { get; private set; }

        public void SendToTerminal(int handle)
        {
            if (!string.IsNullOrEmpty(this.Command))
            {
                // send normal string command
                foreach (Char c in this.Command)
                {
                    NativeMethods.SendMessage(handle, NativeMethods.WM_CHAR, (int)c, 0);
                }
                NativeMethods.SendMessage(handle, NativeMethods.WM_CHAR, (int)Keys.Enter, 0);
            }
            else if (this.KeyData != null)
            {
                // special keys
                if (this.KeyData.Control) { NativeMethods.PostMessage(handle, NativeMethods.WM_KEYDOWN, NativeMethods.VK_CONTROL, 0); }
                if (this.KeyData.Shift) { NativeMethods.PostMessage(handle, NativeMethods.WM_KEYDOWN, NativeMethods.VK_SHIFT, 0); }

                char charStr = Convert.ToChar(this.KeyData.KeyCode);
                NativeMethods.PostMessage(handle, NativeMethods.WM_KEYDOWN, (int)charStr, 0);

                if (this.KeyData.Shift) { NativeMethods.PostMessage(handle, NativeMethods.WM_KEYUP, NativeMethods.VK_SHIFT, 0); }
                if (this.KeyData.Control) { NativeMethods.PostMessage(handle, NativeMethods.WM_KEYUP, NativeMethods.VK_CONTROL, 0); }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(this.Command))
            {
                sb.Append(this.Command);
            }
            else if (this.KeyData != null)
            {
                sb.AppendFormat("({0})", this.KeyData.KeyData);
            }
            return sb.ToString();
        }

    }
}
