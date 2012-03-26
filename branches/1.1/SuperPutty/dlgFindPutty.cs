/*
 * Copyright (c) 2009 Jim Radford http://www.jimradford.com
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
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SuperPutty
{
    public partial class dlgFindPutty : Form
    {
        private string m_PuttyLocation;

        public string PuttyLocation
        {
            get { return m_PuttyLocation; }
            private set { m_PuttyLocation = value; }
        }
        private string m_PscpLocation;

        public string PscpLocation
        {
            get { return m_PscpLocation; }
            private set { m_PscpLocation = value; }
        }

        public dlgFindPutty()
        {
            InitializeComponent();


            // check for location of putty/pscp
            if (!String.IsNullOrEmpty(frmSuperPutty.PuttyExe) && File.Exists(frmSuperPutty.PuttyExe))
            {
                textBoxPuttyLocation.Text = frmSuperPutty.PuttyExe;
                if (!String.IsNullOrEmpty(frmSuperPutty.PscpExe) && File.Exists(frmSuperPutty.PscpExe))
                {
                    textBoxPscpLocation.Text = frmSuperPutty.PscpExe;
                }
            }
            else if(!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("ProgramFiles(x86)")))
            {
                if (File.Exists(Environment.GetEnvironmentVariable("ProgramFiles(x86)") + @"\PuTTY\putty.exe"))
                {
                    textBoxPuttyLocation.Text = Environment.GetEnvironmentVariable("ProgramFiles(x86)") + @"\PuTTY\putty.exe";
                    openFileDialog1.InitialDirectory = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
                }

                if (File.Exists(Environment.GetEnvironmentVariable("ProgramFiles(x86)") + @"\PuTTY\pscp.exe"))
                {

                    textBoxPscpLocation.Text = Environment.GetEnvironmentVariable("ProgramFiles(x86)") + @"\PuTTY\pscp.exe";
                }
            }
            else if (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("ProgramFiles")))
            {
                if (File.Exists(Environment.GetEnvironmentVariable("ProgramFiles") + @"\PuTTY\putty.exe"))
                {
                    textBoxPuttyLocation.Text = Environment.GetEnvironmentVariable("ProgramFiles") + @"\PuTTY\putty.exe";
                    openFileDialog1.InitialDirectory = Environment.GetEnvironmentVariable("ProgramFiles");
                }

                if (File.Exists(Environment.GetEnvironmentVariable("ProgramFiles") + @"\PuTTY\pscp.exe"))
                {
                    textBoxPscpLocation.Text = Environment.GetEnvironmentVariable("ProgramFiles") + @"\PuTTY\pscp.exe";
                }
            }            
            else
            {
                openFileDialog1.InitialDirectory = Application.StartupPath;
            }                
        }
       
        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxPscpLocation.Text) && File.Exists(textBoxPscpLocation.Text))
            {
                PscpLocation = textBoxPscpLocation.Text;
            }

            if (!String.IsNullOrEmpty(textBoxPuttyLocation.Text) && File.Exists(textBoxPuttyLocation.Text))
            {
                PuttyLocation = textBoxPuttyLocation.Text;
                DialogResult = DialogResult.OK;
            }
            else
            {
                if (MessageBox.Show("PuTTY is required to properly use this application.", "PuTTY Required", MessageBoxButtons.RetryCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    System.Environment.Exit(1);
                }
            }                        
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "PuTTY|putty.exe";
            openFileDialog1.FileName = "putty.exe";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (!String.IsNullOrEmpty(openFileDialog1.FileName))
                    textBoxPuttyLocation.Text = openFileDialog1.FileName;
            }            
        }

        private void buttonBrowsePscp_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "PScp|pscp.exe";
            openFileDialog1.FileName = "pscp.exe";
            openFileDialog1.ShowDialog();
            if (!String.IsNullOrEmpty(openFileDialog1.FileName))
                textBoxPscpLocation.Text = openFileDialog1.FileName;
        }
    }
}
