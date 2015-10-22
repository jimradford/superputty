/*
 * Copyright (c) 2009-2015 Jim Radford http://www.jimradford.com
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
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
using System.Windows.Forms;
using System.IO;

namespace SuperPutty
{
    public partial class dlgScriptEditor : Form
    {
        /// <summary>Raised when a script is ready to execute.</summary>
        public event ExecuteScriptEventHandler ScriptReady;
        public delegate void ExecuteScriptEventHandler(object sender, ExecuteScriptEventArgs e);

        protected virtual void OnScriptReady(ExecuteScriptEventArgs e)
        {
            ExecuteScriptEventHandler handler = ScriptReady;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>A dialog that allows opening, saving, editing and executing multiline scripts</summary>
        public dlgScriptEditor()
        {
            InitializeComponent();
            this.ActiveControl = textBoxSript;         
        }

        private void buttonRunScript_Click(object sender, EventArgs e)
        {
            ExecuteScriptEventArgs args = new ExecuteScriptEventArgs { Script = textBoxSript.Text };
            OnScriptReady(args);
            this.Close();
            
        }

        private void buttonLoadScript_Click(object sender, EventArgs e)
        {
            DialogResult dlgResult = this.openFileDialog1.ShowDialog();
            if(dlgResult == DialogResult.OK)
            {
                string script = File.ReadAllText(this.openFileDialog1.FileName);
                textBoxSript.AppendText(script);
            }
        }

        private void buttonSaveScript_Click(object sender, EventArgs e)
        {
            DialogResult dlgResult = this.saveFileDialog1.ShowDialog();
            if(dlgResult == DialogResult.OK)
            {
                File.WriteAllText(this.saveFileDialog1.FileName, textBoxSript.Text);
            }
        }
    }

    /// <summary>Handles passing the script data to caller.</summary>
    public class ExecuteScriptEventArgs : EventArgs
    {
        /// <summary>A string containing a list of commands to be sent to open terminal sessions</summary>
        public string Script { get; set; }
        /// <summary>True if the script should be handled by script parser</summary>
        public bool IsSPSL {
            get
            {
                if (!string.IsNullOrEmpty(this.Script)
                    && this.Script.StartsWith("#!/bin/spsl"))
                    return true;
                else
                    return false;
            }
        }

        /// <summary>If set to the handle of a window, script will be restricted to the specified session only.</summary>
        public IntPtr Handle { get; set; }
    }
}
