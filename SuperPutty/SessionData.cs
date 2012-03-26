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
using System.Collections.Generic;
using System.Text;
using System.Net;
using Microsoft.Win32;
using WeifenLuo.WinFormsUI.Docking;

namespace SuperPutty
{
    public enum ConnectionProtocol
    {
        SSH,
        Telnet,
        Rlogin,
        Raw,
        Serial,
        Cygterm
    }

    public class SessionData
    {
        private string _OldName;

        public string OldName
        {
            get { return _OldName; }
            set { _OldName = value; }
        }

        private string _SessionName;
        public string SessionName
        {
            get { return _SessionName; }
            set { OldName = _SessionName; 
                _SessionName = value; }
        }

        private string _Host;
        public string Host
        {
            get { return _Host; }
            set { _Host = value; }
        }

        private int _Port;
        public int Port
        {
            get { return _Port; }
            set { _Port = value; }
        }

        private ConnectionProtocol _Proto;
        public ConnectionProtocol Proto
        {
            get { return _Proto; }
            set { _Proto = value; }
        }

        private string _PuttySession;
        public string PuttySession
        {
            get { return _PuttySession; }
            set { _PuttySession = value; }
        }

        private string _Username;
        public string Username
        {
            get { return _Username; }
            set { _Username = value; }
        }

        private string _Password;
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        private string _LastPath = ".";
        public string LastPath
        {
            get { return _LastPath; }
            set { _LastPath = value; }
        }

        private DockState m_LastDockstate = DockState.Document;
        public DockState LastDockstate
        {
            get { return m_LastDockstate; }
            set { m_LastDockstate = value; }
        }

        private bool m_AutoStartSession = false;
        public bool AutoStartSession
        {
            get { return m_AutoStartSession; }
            set { m_AutoStartSession = value; }
        }

        public SessionData(string sessionName, string hostName, int port, ConnectionProtocol protocol, string sessionConfig)
        {
            SessionName = sessionName;
            Host = hostName;
            Port = port;
            Proto = protocol;
            PuttySession = sessionConfig;
        }
        
        public SessionData()
        {

        }

        public void SaveToRegistry()
        {
            if (!String.IsNullOrEmpty(this.SessionName)
                && !String.IsNullOrEmpty(this.Host)
                && this.Port >= 0)
            {

                // detect if session was renamed
                if (!String.IsNullOrEmpty(this.OldName) && this.OldName != this.SessionName)
                {
                    RegistryRemove(this.OldName);
                }

                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Jim Radford\SuperPuTTY\Sessions\" + this.SessionName, true);
                if (key == null)
                {
                    key = Registry.CurrentUser.CreateSubKey(@"Software\Jim Radford\SuperPuTTY\Sessions\" + this.SessionName);
                }

                if (key != null)
                {                   
                    key.SetValue("Host", this.Host);
                    key.SetValue("Port", this.Port);
                    key.SetValue("Proto", this.Proto);
                    key.SetValue("PuttySession", this.PuttySession);
                    
                    if(!String.IsNullOrEmpty(this.Username))
                        key.SetValue("Login", this.Username);

                    key.SetValue("Last Path", this.LastPath);

                    if(this.LastDockstate != DockState.Hidden && this.LastDockstate != DockState.Unknown)
                        key.SetValue("Last Dock", (int)this.LastDockstate);                    

                    key.SetValue("Auto Start", this.AutoStartSession);
                    key.Close();
                }
                else
                {
                    Logger.Log("Unable to create registry key for " + this.SessionName);
                }
            }
        }

        internal void RegistryRemove(string sessionName)
        {
            if (!String.IsNullOrEmpty(sessionName))
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Jim Radford\SuperPuTTY\Sessions", true);
                try
                {
                    key.OpenSubKey(sessionName);
                    key.DeleteSubKeyTree(sessionName);                    
                    key.Close();
                }
                catch (UnauthorizedAccessException e)
                {
                    Logger.Log(e);
                }
            }
        }
    }
}
