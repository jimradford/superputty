/*
 * Copyright (c) 2009 - 2015 Jim Radford http://www.jimradford.com
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"}, to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions: 
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using log4net;

namespace SuperPutty.Utils
{
    /// <summary>Store and retrieve commands and keystrokes for sending to sessions</summary>
    public class CommandData
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CommandData));

        /// <summary>Get the command to send</summary>
        public string Command { get; private set; }
        /// <summary>Get the keystrokes to send</summary>
        public KeyEventArgs KeyData { get; private set; }

        public TimeSpan Delay { get; private set; }

        /// <summary>Construct a new <seealso cref="CommandData"/> object, specifying a command to send</summary>
        /// <param name="command">A string containing the command to send</param>
        public CommandData(string command)
        {
            this.Command = command;
        }

        /// <summary>Construct a new <seealso cref="CommandData"/> object, specifying keyboard keystrokes to send</summary>
        /// <param name="keys">A <seealso cref="KeyEventArgs"/> object containing the keyboard keystrokes</param>
        public CommandData(KeyEventArgs keys)
        {
            this.KeyData = keys;
        }

        /// <summary>Construct a new <seealso cref="CommandData"/> object, specifying both a command and keyboard keystrokes to send</summary>
        /// /// <param name="command">A string containing the command to send</param>
        /// <param name="keys">A <seealso cref="KeyEventArgs"/> object containing the keyboard keystrokes</param>
        public CommandData(string command, KeyEventArgs keys)
        {
            this.Command = command;
            this.KeyData = keys;
        }

        /// <summary>Construct a new <seealso cref="CommandData"/> object, specifying both a command and keyboard keystrokes to send</summary>
        /// /// <param name="command">A string containing the command to send</param>
        /// <param name="keys">A <seealso cref="KeyEventArgs"/> object containing the keyboard keystrokes</param>
        /// <param name="delay">How long we should wait before executing next command</param>
        public CommandData(string command, KeyEventArgs keys, TimeSpan delay)
        {
            this.Command = command;
            this.KeyData = keys;
            this.Delay = delay;
        }

        /// <summary>Send commands and keystrokes to the specified session</summary>
        /// <param name="handle">The Windows Handle to send to</param>
        public void SendToTerminal(int handle)
        {
            Log.InfoFormat("SendToTerminal: Handle={0}, Command=[{1}]", handle, this);
            if (!string.IsNullOrEmpty(this.Command))
            {
                // send normal string command
                foreach (Char c in this.Command)
                {
                    NativeMethods.SendMessage(handle, NativeMethods.WM_CHAR, (int)c, 0);
                }
            }

            if (this.KeyData != null)
            {
                // special keys
                if (this.KeyData.Control) { NativeMethods.PostMessage(handle, NativeMethods.WM_KEYDOWN, NativeMethods.VK_CONTROL, 0); }
                if (this.KeyData.Shift) { NativeMethods.PostMessage(handle, NativeMethods.WM_KEYDOWN, NativeMethods.VK_SHIFT, 0); }

                char charStr = Convert.ToChar(this.KeyData.KeyCode);
                NativeMethods.PostMessage(handle, NativeMethods.WM_KEYDOWN, (int)charStr, 0);

                if (this.KeyData.Shift) { NativeMethods.PostMessage(handle, NativeMethods.WM_KEYUP, NativeMethods.VK_SHIFT, 0); }
                if (this.KeyData.Control) { NativeMethods.PostMessage(handle, NativeMethods.WM_KEYUP, NativeMethods.VK_CONTROL, 0); }
            }

            if (this.Delay > TimeSpan.Zero)
            {
                Thread.Sleep(this.Delay);
            }
        }
        
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(this.Command))
            {
                sb.Append(this.Command);
            }

            if (this.KeyData != null)
            {
                sb.AppendFormat("({0})", this.KeyData.KeyData);
            }
            return sb.ToString();
        }

    }
}
