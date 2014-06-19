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
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Text.RegularExpressions;
using SuperPutty.Data;


namespace SuperPutty
{
    public enum RequestResult
    {
        RetryAuthentication,
        ListingFollows,
        UnknownError,
        SessionInvalid,
        InvalidArguments,
        CancelLogin
    }

    public delegate void TransferUpdateCallback(bool fileComplete, bool cancelAll, FileTransferStatus status);
    public delegate void DirListingCallback(RequestResult result, List<FileEntry> files);

    public struct FileTransferStatus
    {
        public string Filename;
        public int BytesTransferred;
        public float TransferRate;
        public string TimeLeft;
        public int PercentComplete;
        // 0 -> LICENSE.txt
        //1 -> 1 kB
        //2 -> 1.3 kB/s
        //3 -> ETA: 00:00:00
        //4 -> 100%
    }

    public struct FileEntry
    {
        // lrwxrwxrwx  1 jradford users       11 2009-08-02 08:33 lm -> landmachine
        private bool _IsFolder;

        public bool IsFolder
        {
            get { return _IsFolder; }
            private set { _IsFolder = value; }
        }
        private bool _IsSymLink;

        public bool IsSymLink
        {
            get { return _IsSymLink; }
            private set { _IsSymLink = value; }
        }
        private bool _IsFile;

        public bool IsFile
        {
            get { return _IsFile; }
            private set { _IsFile = value; }
        }

        private string _PermissionString;

        public string PermissionString
        {
            get { return _PermissionString; }
            set { _PermissionString = value; }
        }

        public int LinkCount; // second column
        public string OwnerName;
        public string GroupName;
        public int BlockCount;
        public DateTime TimeStamp;
        public string Name;
        
        public override string ToString()
        {
            return String.Format("Links: {0}, Owner: {1}, Group: {2}, Blocks: {3}, Time: {4}, Name: {5}, IsFile: {6}, IsLink: {7}, Perms: {8}",
                this.LinkCount, this.OwnerName, this.GroupName, this.BlockCount, this.TimeStamp, this.Name, this.IsFile, this.IsSymLink, this.PermissionString);
        }

        public void SetPropertiesFromPermissionString(string perms)
        {
            PermissionString = perms;
            char p = perms.ToCharArray(0, 1)[0];
            switch (p)
            {
                case '-':
                    this.IsFile = true;
                    break;
                case 'l':
                    this.IsSymLink = true;
                    break;
                case 'd':
                    this.IsFolder = true;
                    break;
            }
        }
    }

    public class PscpTransfer
    {
        /*
         * constant strings putty sends for various events, we use some of these to detect authentication
         * progress
         * 
         * These are from PuTTY Release 0.60. If these change significantly between PuTTY versions
         * we will need a better way of detection.
         * 
         * TODO: Need to verify whether or not some of these might be passed through putty from the SSH Server,
         * If they are we'll need to do some research to support the various ssh server implementations.
         */
        /// <summary>Putty is asking is to enter our username and or password, which means
        /// we've either not sent one, or the one we sent is invalid.</summary>
        private const string PUTTY_INTERACTIVE_AUTH = "Using keyboard-interactive authentication.";
        /// <summary>This is the fist line sent back from PuTTY if we sent invalid arguments</summary>
        private const string PUTTY_ARGUMENTS_HELP_HEADER = "PuTTY Secure Copy client";

        /// <summary>Start of the message indicating we have never connected to this host before</summary>
        private const string PUTTY_NO_KEY = "The server's host key is not cached in the registry";
        /*           
             * 
             * Network 10.1 ‘The server's host key is not cached in the registry’ 
             * 10.2 ‘WARNING - POTENTIAL SECURITY BREACH!’ 
             * 10.3 ‘Out of space for port forwardings’ 
             * 10.4 ‘The first cipher supported by the server is ... below the configured warning threshold’ 
             * 10.5 ‘Server sent disconnect message type 2 (protocol error): "Too many authentication failures for root"’ 
             * 10.6 ‘Out of memory’ 
             * 10.7 ‘Internal error’, ‘Internal fault’, ‘Assertion failed’ 
             * 10.8 ‘Unable to use this private key file’, ‘Couldn't load private key’, ‘Key is of wrong type’ 
             * 10.9 ‘Server refused our public key’ or ‘Key refused’ 
             * 10.10 ‘Access denied’, ‘Authentication refused’ 
             * 10.11 ‘Incorrect CRC received on packet’ or ‘Incorrect MAC received on packet’ 
             * 10.12 ‘Incoming packet was garbled on decryption’ 
             * 10.13 ‘PuTTY X11 proxy: various errors’ 
             * 10.14 ‘Network error: Software caused connection abort’ 
             * 10.15 ‘Network error: Connection reset by peer’ 
             * 10.16 ‘Network error: Connection refused’ 
             * 10.17 ‘Network error: Connection timed out’ error: Connection timed out
             */

        private dlgLogin m_Login;
        private SessionData m_Session;

        private PuttyClosedCallback m_PuttyClosed;

        private Thread m_PscpThread;
        public PuttyClosedCallback PuttyClosed
        {
            get { return m_PuttyClosed; }
            set { m_PuttyClosed = value; }
        }

        public PscpTransfer(SessionData session)
        {
            m_Session = session;
            m_Login = new dlgLogin(m_Session);
        }

        private bool m_DirIsBusy = false;
        private Process m_processDir;
        public void BeginGetDirectoryListing(string path, DirListingCallback callback)
        {
            
            if (m_Session == null)
            {
                callback(RequestResult.SessionInvalid, null);                
                return;
            }
            
            List<FileEntry> files = new List<FileEntry>();
            Stopwatch timeoutWatch = new Stopwatch();

            /* 
             * Check that we have a username either stored from previous sessions, or loaded
             * from the registry. If PPK Authentication is being used that will override
             * any values entered in the login dialog
             */
            if (String.IsNullOrEmpty(m_Session.Username))
            {
                if (m_Login.ShowDialog(SuperPuTTY.MainForm) == System.Windows.Forms.DialogResult.OK)
                {
                    m_Session.Username = m_Login.Username;
                    m_Session.Password = m_Login.Password;

                    if (m_Login.Remember)
                    {
                        //Session.SaveToRegistry(); // passwords are *never* saved and stored permanently
                        SuperPuTTY.SaveSessions();
                    }
                }
                else
                {
                    Logger.Log("Cancel connection");
                    callback(RequestResult.CancelLogin, null);
                }
            }

            Thread threadListFiles = new Thread(delegate()
            {
                m_processDir = new Process();
                
                m_processDir.EnableRaisingEvents = true;
                m_processDir.StartInfo.UseShellExecute = false;
                m_processDir.StartInfo.RedirectStandardError = true;
                //m_processDir.StartInfo.RedirectStandardInput = true;
                m_processDir.StartInfo.RedirectStandardOutput = true;
                m_processDir.StartInfo.CreateNoWindow = true;
                m_processDir.StartInfo.FileName = SuperPuTTY.Settings.PscpExe;                
                // process the various options from the session object and convert them into arguments pscp can understand
                string args = MakeArgs(m_Session, true, path);
                Logger.Log("Sending Command: '{0} {1}'", m_processDir.StartInfo.FileName, MakeArgs(m_Session, false, path));
                m_processDir.StartInfo.Arguments = args;                                
                /*
                 * Handle output from spawned pscp.exe process, handle any data received and parse
                 * any lines that look like a directory listing.
                 */
                m_processDir.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e)
                {
                    if (!String.IsNullOrEmpty(e.Data))
                    {
                        if (e.Data.Equals(PUTTY_ARGUMENTS_HELP_HEADER))
                        {                            
                            m_processDir.CancelOutputRead();
                            m_processDir.Kill();
                            return;
                        }
                        else if (e.Data.StartsWith("Listing directory "))
                        {
                            // This just tells us the current directory, however since we're the ones that requested it
                            // we already have this information. But this traps it so its not sent through the directory
                            // entry parser.                            
                        }
                        else if (e.Data.Equals(PUTTY_INTERACTIVE_AUTH) || e.Data.Contains("password: "))
                        {
                            m_processDir.CancelOutputRead();
                            if(!m_processDir.HasExited)
                                m_processDir.Kill();
                            Logger.Log("Username/Password invalid or not sent");
                            callback(RequestResult.RetryAuthentication, null);
                        }
                        else
                        {
                            timeoutWatch.Reset();
                            lock (files)
                            {
                                FileEntry file;
                                if (TryParseFileLine(e.Data, out file))
                                {
                                    files.Add(file);
                                }

                                if (files.Count > 0)
                                {
                                    callback(RequestResult.ListingFollows, files);                                    
                                }                                
                            }
                        }
                    }
                };

                m_processDir.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e)
                {
                    if (!String.IsNullOrEmpty(e.Data))
                    {
                        if (e.Data.Contains(PUTTY_NO_KEY))
                        {
                            m_processDir.CancelErrorRead();
                            m_processDir.Kill();
                            System.Windows.Forms.MessageBox.Show("The key of the host you are attempting to connect to has changed or is not cached \n" +
                                "You must connect to this host with with a PuTTY ssh terminal to accept the key and store it in the cache", "Host Key not found or changed", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
                        }
                        else
                        {
                            Logger.Log("Error Data:\n\t'{0}'", e.Data.TrimEnd());
                            // 'ssh_init: Host does not exist'
                        }
                    }
                };

                m_processDir.Exited += delegate(object sender, EventArgs e)
                {
                    if (m_processDir.ExitCode != 0)
                    {
                        Logger.Log("Process Exited (Failure): {0}", m_processDir.ExitCode);
                        callback(RequestResult.UnknownError, null);
                        if (m_PuttyClosed != null)
                            m_PuttyClosed(true);
                    }
                    else
                    {
                        Logger.Log("Process Exited: {0}", m_processDir.ExitCode);
                        if (m_PuttyClosed != null)
                            m_PuttyClosed(false);
                    }
                    m_DirIsBusy = false;
                };

                try
                {
                    m_processDir.Start();
                }
                catch (Win32Exception e)
                {
                    if (e.NativeErrorCode == 2) // File Not Found
                    {
                        Logger.Log(e);
                    }
                    else if (e.NativeErrorCode == 4) // Acess Denied
                    {
                        Logger.Log(e);
                    }
                }

                m_processDir.BeginErrorReadLine();
                m_processDir.BeginOutputReadLine();                
                m_processDir.WaitForExit();                
            });

            /* Only allow one directory list request at a time */
            if (!m_DirIsBusy)
            {
                m_DirIsBusy = true;
                threadListFiles.Name = "List Remote Directory";
                threadListFiles.IsBackground = true;
                threadListFiles.Start();
            }
            else
            {
                return;
            }
            
            Thread timeoutThread = new Thread(delegate()
            {
                while (m_DirIsBusy)
                {
                    /*
                     * if no data received in 5 seconds we'll stop the process,
                     * This allows us to capture any interactive prompts/messages
                     * sent to us by putty.
                     */
                    if (timeoutWatch.Elapsed.Seconds >= 5) 
                    {
                        Logger.Log("Timeout after {0} seconds", timeoutWatch.Elapsed.Seconds);                        
                                                
                        if (!m_processDir.HasExited)
                        {
                            m_processDir.Kill();                            
                        }
                        m_processDir.CancelErrorRead();
                        m_processDir.CancelOutputRead();
                        return;
                    }
                    Thread.Sleep(1000);
                }
            });
            timeoutThread.Name = "Timeout Watcher";
            timeoutThread.IsBackground = true;
            timeoutThread.Start();
            timeoutWatch.Start();                                    
        }

        private static bool TryParseFileLine(string line, out FileEntry FileNode)
        {
            Match match;
            // 'drwxr-xr-x    6 jradford users        4096 Mar 28 22:07 legend'
            match = Regex.Match(line.TrimEnd(), @"^(?<Permissions>[drwx\-lSs]+)\s+(?<LinkCount>\d{1,})\s+(?<OwnerName>\w+)\s+(?<GroupName>\w+)\s+(?<BlockCount>\d+)\s+(?<FileMonth>Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\s{1,2}(?<FileDay>\d{1,2})\s(?<FileHour>[0-9]{2}):(?<FileMinute>[0-9]{2})\s(?<FileName>.*)$");
            if (!match.Success) // '-rw-r--r--   1 jradford jradford     1157 Jan 15  2002 localwww.gif'
                match = Regex.Match(line.TrimEnd(), @"^(?<Permissions>[drwx\-lSs]+)\s+(?<LinkCount>\d+)\s+(?<OwnerName>\w+)\s+(?<GroupName>\w+)\s+(?<BlockCount>\d+)\s+(?<FileMonth>Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\s{1,2}(?<FileDay>\d{1,2})\s+(?<FileYear>[0-9]{4})\s(?<FileName>.*)$");

            if (match.Success)
            {
                FileEntry node = new FileEntry();
                node.SetPropertiesFromPermissionString(match.Groups["Permissions"].Value);

                int.TryParse(match.Groups["LinkCount"].Value, out node.LinkCount);
                node.OwnerName = match.Groups["OwnerName"].Value;
                node.GroupName = match.Groups["GroupName"].Value;
                int.TryParse(match.Groups["BlockCount"].Value, out node.BlockCount);

                string dateStr = String.Format("{0} {1} {2} {3}:{4}:{5}",
                    match.Groups["FileMonth"].Value,
                    match.Groups["FileDay"].Value,
                    String.IsNullOrEmpty(match.Groups["FileYear"].Value) ? DateTime.UtcNow.Year.ToString() : match.Groups["FileYear"].Value,
                    String.IsNullOrEmpty(match.Groups["FileHour"].Value) ? "23" : match.Groups["FileHour"].Value,
                    String.IsNullOrEmpty(match.Groups["FileMinute"].Value) ? "59" : match.Groups["FileMinute"].Value,
                    String.IsNullOrEmpty(match.Groups["FileSeconds"].Value) ? "59" : match.Groups["FileSeconds"].Value);

                DateTime.TryParse(dateStr, out node.TimeStamp);

                node.Name = match.Groups["FileName"].Value;

                FileNode = node;
                return true;
            }
            else
            {
                Logger.Log("Could not parse directory listing entry: \n\t'{0}'", line.TrimEnd());
                FileNode = new FileEntry();
                return false;
            }
        }

        static string MakeArgs(SessionData session, bool includePassword, string path)
        {
            string args = "-ls "; // default arguments
            args += (!String.IsNullOrEmpty(session.PuttySession)) ? "-load \"" + session.PuttySession + "\" " : "";
            args += (!String.IsNullOrEmpty(session.Password) && session.Password.Length > 0) 
                ? "-pw " + (includePassword ? session.Password : "XXXXX") + " " 
                : "";
            args += "-P " + session.Port + " ";
            args += (!String.IsNullOrEmpty(session.Username)) ? session.Username + "@" : "";
            args += session.Host + ":" + path;

            return args;
        }

        /// <summary>
        /// Attempts to copy local files from the local filesystem to the selected remote target path
        /// </summary>
        /// <param name="files">An array containing full paths to files and or folders to copy</param>
        /// <param name="target">The target path on the remote system</param>
        /// <param name="callback">A callback to fire on success or error. On failure the files parameter will be null</param>
        public void BeginCopyFiles(string[] files, string target, TransferUpdateCallback callback)
        {
            if (String.IsNullOrEmpty(m_Session.Username))
            {
                if (m_Login.ShowDialog(SuperPuTTY.MainForm) == System.Windows.Forms.DialogResult.OK)
                {
                    m_Session.Username = m_Login.Username;
                    m_Session.Password = m_Login.Password;
                }
            }

            // put the copy operation in the background since it could take a long long time
            m_PscpThread = new Thread(delegate()
            {
                Process processCopyToRemote = new Process();
                try
                {
                    processCopyToRemote.EnableRaisingEvents = true;
                    processCopyToRemote.StartInfo.RedirectStandardError = true;                    
                    processCopyToRemote.StartInfo.RedirectStandardInput = true;
                    processCopyToRemote.StartInfo.RedirectStandardOutput = true;
                    processCopyToRemote.StartInfo.FileName = SuperPuTTY.Settings.PscpExe;
                    processCopyToRemote.StartInfo.CreateNoWindow = true;
                    // process the various options from the session object and convert them into arguments pscp can understand
                    string args = "-r -agent "; // default arguments
                    args += (!String.IsNullOrEmpty(m_Session.PuttySession)) ? "-load \"" + m_Session.PuttySession + "\" " : "";
                    //args += "-l " + Session.Username + " ";
                    args += (!String.IsNullOrEmpty(m_Session.Password) && m_Session.Password.Length > 0) ? "-pw " + m_Session.Password + " " : "";
                    args += "-P " + m_Session.Port + " ";
                    args += "\"" + files[0] + "\" ";
                    args += (!String.IsNullOrEmpty(m_Session.Username)) ? m_Session.Username + "@" : "";
                    args += m_Session.Host + ":" + target;
                    Logger.Log("Args: '{0} {1}'", processCopyToRemote.StartInfo.FileName, args);
                    processCopyToRemote.StartInfo.Arguments = args;
                    processCopyToRemote.StartInfo.UseShellExecute = false;

                    processCopyToRemote.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e)
                    {
                        if (!String.IsNullOrEmpty(e.Data))
                        {
                            Match match = Regex.Match(e.Data.TrimEnd(), ".*|.*|.*|.*|.*");
                            if (match.Success)
                            {
                                if (callback != null)
                                {
                                    FileTransferStatus status = new FileTransferStatus();
                                    string[] update = e.Data.TrimEnd().Split('|');
                                    status.Filename = update[0].Trim();
                                    status.BytesTransferred = int.Parse(update[1].Replace("kB", "").Trim());
                                    status.TransferRate = float.Parse(update[2].Replace("kB/s", "").Trim());
                                    status.TimeLeft = update[3].Replace("ETA:", "").Trim();
                                    status.PercentComplete = int.Parse(update[4].Replace("%", "").Trim());
                                    //Logger.Log("File Transfer Data: " + e.Data);
                                    callback(status.PercentComplete.Equals(100), false, status);
                                }
                            }
                            else
                            {
                                Logger.Log("Unable to parse OutputData: {0}", e.Data.TrimEnd());
                            }
                        }
                    };

                    processCopyToRemote.Start();
                    processCopyToRemote.BeginOutputReadLine();
                    processCopyToRemote.WaitForExit();                    
                }
                catch (ThreadAbortException)
                {
                    if(!processCopyToRemote.HasExited)
                        processCopyToRemote.Kill();
                }                
            });

            m_PscpThread.IsBackground = true;
            m_PscpThread.Name = "File Upload";
            m_PscpThread.Start();
        }

        internal void CancelTransfers()
        {
            Logger.Log("Aborting Transfer in CancelTransfer");
            m_PscpThread.Abort();

        }
    }
}
