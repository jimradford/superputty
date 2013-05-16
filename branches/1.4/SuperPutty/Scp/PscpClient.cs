using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using SuperPutty.Data;
using System.ComponentModel;
using System.IO;
using System.Globalization;

namespace SuperPutty.Scp
{
    /// <summary>
    /// Simplified version of PscpTransfer class
    /// - Movied LoginDialog calls outside
    /// - Made calls synchronous...move async outside
    /// - Work around issue in Process.StandardOuput not reading "userName's password:"
    /// </summary>
    public class PscpClient
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PscpClient));

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

        private const string PUTTY_UNABLE_TO_OPEN = "Unable to open ";
        /// <summary>
        /// This is the fist line sent back from PuTTY if we send no arguments.  
        /// </summary>
        private const string PUTTY_ARGUMENTS_HELP_HEADER = "PuTTY Secure Copy client";

        private const string PUTTY_HOST_DOES_NOT_EXIST = "ssh_init: Host does not exist";
        /// <summary>
        /// Returned when a bad arg is sent
        /// </summary>
        private const string PUTTY_BAD_ARGUMENT = "pscp: unknown option ";

        /// <summary>Start of the message indicating we have never connected to this host before</summary>
        private const string PUTTY_NO_KEY = "The server's host key is not cached in the registry";
        /* Network 
         * 10.1 ‘The server's host key is not cached in the registry’ 
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


        public PscpClient(string pscpLocation) 
        {
            this.PscpLocation = pscpLocation;
            this.TimeoutMs = 50000;
        }

        public ListDirectoryResult ListDirectory(SessionData session, string path)
        {
            ListDirectoryResult result = new ListDirectoryResult(path);

            if (!File.Exists(PscpLocation))
            {
                result.SetError(string.Format("Pscp missing, path={0}.", this.PscpLocation), null);
            }
            else if (session.Username == null)
            {
                result.SetError("UserName is null", null);
            }
            else if (session.Host == null)
            {
                result.SetError("Host is null", null);
            }
            else if (session.Port < 0)
            {
                result.SetError("Invalid port: " + session.Port, null);
            }
            else
            {
                // Setup the process list directory contents
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = this.PscpLocation,
                    WorkingDirectory = Path.GetDirectoryName(this.PscpLocation),
                    Arguments = ToArgs(session, session.Password, path),
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };

                // Start pscp
                string args = ToArgs(session, "XXXXX", path);
                Log.InfoFormat("Starting process: file={0}, args={1}", this.PscpLocation, args);
                Process proc = new Process
                {
                    StartInfo = startInfo,
                    EnableRaisingEvents = true
                };
                proc.Start();

                // Async read output/err.  Inline actions to quick kill the process when pscp prompts user.
                // NOTE: Using BeginReadOutput/ErrorReadLine doesn't work here.  Calls to read an empty stream
                //       will block (e.g. "user's password:" prompt will block on reading err stream).  
                AsyncStreamReader outReader = new AsyncStreamReader(
                    proc.StandardOutput,
                    strOut =>
                    {
                        if (strOut == PUTTY_INTERACTIVE_AUTH || strOut.Contains("password: "))
                        {
                            result.StatusCode = ResultStatusCode.RetryAuthentication;
                            Log.Debug("Username/Password invalid or not sent");
                            proc.Kill();
                        }
                    });
                AsyncStreamReader errReader = new AsyncStreamReader(
                    proc.StandardError,
                    strErr =>
                    {
                        if (strErr != null && strErr.Contains(PUTTY_NO_KEY))
                        {
                            result.SetError("Host key not cached.  Connect via putty to cache key then try again", null);
                            proc.Kill();
                        }
                    });

                // Wait for exit - Process finished ok, finished w/error (killed), or timeout
                if (proc.WaitForExit(this.TimeoutMs))
                {
                    Log.DebugFormat("Process exited normally, pid={0}, exitCode={1}", proc.Id, proc.ExitCode);

                    string output = outReader.StopAndGetData().Trim();
                    string err = errReader.StopAndGetData().Trim();

                    if (proc.ExitCode == 0 && output.Contains(PUTTY_UNABLE_TO_OPEN))
                    {
                        int idx = output.IndexOf(PUTTY_UNABLE_TO_OPEN);
                        result.SetErrorFormat(output.Substring(idx));
                    }
                    else if (proc.ExitCode == 0)
                    {
                        // successful list
                        ScpLineParser parser = new ScpLineParser();
                        foreach (string rawLine in output.Split('\n'))
                        {
                            string line = rawLine.TrimEnd();
                            BrowserFileInfo fileInfo;
                            if (parser.TryParseFileLine(line, out fileInfo))
                            {
                                if (fileInfo.Name != ".")
                                {
                                    fileInfo.Path = MakePath(path, fileInfo.Name);
                                    result.Add(fileInfo);
                                }
                            }
                        }
                    }
                    else
                    {
                        // some kind of error
                        if (result.StatusCode != ResultStatusCode.Success)
                        {
                            Log.Debug("Skipping output check since proactively killed process.");
                        }
                        else if (PUTTY_ARGUMENTS_HELP_HEADER == output)
                        {
                            result.SetErrorFormat("Invalid arguments sent to pscp, args={0}, output={1}", args, output);
                        }
                        else if (PUTTY_HOST_DOES_NOT_EXIST == err)
                        {
                            result.SetErrorFormat("Host does not exist.  {0}:{1}", session.Host, session.Port);
                        }
                        else
                        {
                            result.SetErrorFormat("Unknown error.  exitCode={0}, out={1}, err={2}", proc.ExitCode, output, err);
                        }
                    }
                }
                else
                {
                    // timeout
                    proc.Kill();
                    result.SetErrorFormat("Process timed out, path={0}", path);
                }
            }
            return result;
        }

        public static string MakePath(string parent, string child)
        {
            string path;
            if (child == "..")
            {
                int idx = parent.LastIndexOf('/');
                if (idx == 0)
                {
                    // /home/.. --> /
                    path = "/";
                }
                else if (idx > 0)
                {
                    // /home/scptest/..  --> /home
                    path = parent.Substring(0, idx);
                }
                else
                {
                    // foo/.. --> /
                    path = "";
                }
            }
            else
            {
                path = string.Format("{0}/{1}", parent == "/" ? string.Empty : parent, child);
            }

            //Log.DebugFormat("MakePath: parent={0}, child={1}, path={2}", parent, child, path);
            return path;
        }

        public static string ToArgs(SessionData session, string password, string path)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("-ls ");
            
            if (session.PuttySession != null)
            {
                sb.AppendFormat("-load \"{0}\" ", session.PuttySession);
            }
            if (!string.IsNullOrEmpty(session.Password))
            {
                sb.AppendFormat("-pw {0} ", password);
            }
            sb.AppendFormat("-P {0} ", session.Port);
            sb.AppendFormat("{0}@{1}:{2}", session.Username, session.Host, path);

            return sb.ToString();
        }

        #region AsyncStreamReader
        /// <summary>
        /// Utility class to read a bare stream.  Works around the issues with FileStream blocking when
        /// no data is available...even on peek!  
        /// http://zachsaw.blogspot.com/2011/07/streamreaderpeek-can-block-another-net.html
        /// </summary>
        public class AsyncStreamReader
        {
            private static readonly ILog Log = LogManager.GetLogger(typeof(AsyncStreamReader));

            StringBuilder sb = new StringBuilder();

            public AsyncStreamReader(StreamReader reader, Action<string> dataUpdated)
            {
                this.Reader = reader;
                this.DataUpdatedHandler = dataUpdated;

                this.Thread = new Thread(this.ReadAll);
                this.Thread.IsBackground = true;
                this.Thread.Start();
            }

            void ReadAll()
            {
                try
                {
                    int i = 0;
                    while (this.Reader.Peek() != -1)
                    {
                        char c = (char)this.Reader.Read();
                        lock (this)
                        {
                            sb.Append(c);
                            //Log.InfoFormat("[{0}]", c);

                            // scptest@localhost's password:
                            // Store key in cache? (y/n)
                            bool isPossibleHit =
                                (sb.Length > 10 && c == ' ' && sb[i - 1] == ':') ||
                                (sb.Length > 10 && c == ')' && sb[i - 2] == '/' && sb[i - 1] == 'n');
                            if (isPossibleHit && this.DataUpdatedHandler != null)
                            {
                                this.DataUpdatedHandler(sb.ToString());
                            }
                            i++;

                            if (i > 200)
                            {
                                // if we got this far, we probably have normal operation...just read-to-end
                                sb.Append(this.Reader.ReadToEnd());
                                break;
                            }
                        }
                    }
                    Log.DebugFormat("Read completed.  len={0}", sb.Length);
                }
                catch (ThreadAbortException)
                {
                    // signal to stop
                    Log.Debug("Thread aborted to stop read");
                }
                catch (Exception ex)
                {
                    Log.Error("Error reading stream", ex);
                }
            }

            public string StopAndGetData()
            {
                if (this.Thread.IsAlive)
                {
                    this.Thread.Abort();
                }

                lock (this)
                {
                    return this.sb.ToString();
                }
            }

            StreamReader Reader { get; set; }
            Action<string> DataUpdatedHandler { get; set; }
            Thread Thread { get; set; }
        } 
        #endregion

        #region ScpLineParser
        public class ScpLineParser
        {
            Regex regExPrimary = new Regex(@"^(?<Permissions>[cdrwx\-lSst]+)\s+(?<LinkCount>\d+)\s+(?<OwnerName>\w+)\s+(?<GroupName>\w+)\s+(?<BlockCount>\d+)\s+(?<Timestamp>(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec).{9})\s{1}(?<FileName>.*)$");
            public bool TryParseFileLine(string line, out BrowserFileInfo fileInfo)
            {
                bool success = false;

                if (line.StartsWith("Listing directory"))
                {
                    // ignore header line and current dir
                    fileInfo = null;
                }
                else
                {
                    Match match = regExPrimary.Match(line);
                    if (match.Success)
                    {
                        string name = match.Groups["FileName"].Value;

                        fileInfo = new BrowserFileInfo
                        {
                            Name = name,
                            Path = name,
                            Owner = match.Groups["OwnerName"].Value,
                            Group = match.Groups["GroupName"].Value,
                            Permissions = match.Groups["Permissions"].Value,
                        };
                        fileInfo.Type = fileInfo.Name == ".."
                            ? FileType.ParentDirectory
                            : fileInfo.Permissions != null && fileInfo.Permissions.StartsWith("d")
                                ? FileType.Directory : FileType.File;


                        DateTime lastMod;
                        if (TryParseTimestamp(match.Groups["Timestamp"].Value, out lastMod))
                        {
                            fileInfo.LastModTime = lastMod;
                        }
                        int blocks;
                        if (int.TryParse(match.Groups["BlockCount"].Value, out blocks))
                        {
                            fileInfo.Size = blocks;
                        }
                        success = true;
                    }
                    else
                    {
                        Log.WarnFormat("Could not parse directory listing entry: '{0}'", line);
                        fileInfo = null;
                    }
                }

                return success;
            }

            // May 15 12:32, Mar  3 03:37, Nov 18 18:19, May 27  2012
            public bool TryParseTimestamp(string timeStamp, out DateTime dateTime)
            {
                bool success = false;

                DateTimeStyles style = DateTimeStyles.AllowInnerWhite | DateTimeStyles.AssumeLocal;
                if (DateTime.TryParseExact(timeStamp, "MMM d yyyy", null, style, out dateTime))
                {
                    // May 27  2012 or Nov  1  2013
                    success = true;
                }
                else if (DateTime.TryParseExact(timeStamp, "MMM d H:mm", null, style, out dateTime))
                {
                    // Nov 18 18:19 or Mar  3 03:37
                    success = true;
                }

                return success;
            }
        } 
        #endregion

        public string PscpLocation { get; private set; }
        public int TimeoutMs { get; private set; }

        private dlgLogin m_Login;
        private SessionData m_Session;

        private PuttyClosedCallback m_PuttyClosed;

        private Thread m_PscpThread;
        public PuttyClosedCallback PuttyClosed
        {
            get { return m_PuttyClosed; }
            set { m_PuttyClosed = value; }
        }

        public PscpClient(SessionData session)
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
            if (String.IsNullOrEmpty(m_Session.Username)) // || String.IsNullOrEmpty(m_Session.Password))
            {
                if (m_Login.ShowDialog(SuperPuTTY.MainForm) == System.Windows.Forms.DialogResult.OK)
                {
                    m_Session.Username = m_Login.Username;
                    m_Session.Password = m_Login.Password;

                    if (m_Login.Remember)
                    {
                        //m_Session.SaveToRegistry(); // passwords are *never* saved and stored permanently
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
                m_processDir.StartInfo.WorkingDirectory = Path.GetDirectoryName(SuperPuTTY.Settings.PscpExe);
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
                            if (!m_processDir.HasExited)
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
                    bool started = m_processDir.Start();
                    Log.InfoFormat("Started={0}", started);
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
                    //args += "-l " + m_Session.Username + " ";
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
                    if (!processCopyToRemote.HasExited)
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
