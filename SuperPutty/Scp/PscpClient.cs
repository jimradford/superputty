using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using log4net;
using log4net.Core;
using SuperPutty.Data;

namespace SuperPutty.Scp
{
    /// <summary>
    /// Simplified version of PscpTransfer class
    /// - Movied LoginDialog calls outside
    /// - Made calls synchronous...move async outside
    /// - Make pieces unit-testable
    /// - Work around issue in Process.StandardOuput/StandardError blocking on calls
    /// Alternate workaround...native process start w/correct stream reading behavior (e.g. peek doens't block)
    /// http://stackoverflow.com/questions/6655613/why-does-standardoutput-read-block-when-startinfo-redirectstandardinput-is-set
    /// </summary>
    public class PscpClient
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PscpClient));

        #region Putty string constants
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
        
        #endregion


        public PscpClient(PscpOptions options, SessionData session) 
        {
            this.Options = options;
            this.Session = session;
        }

        #region ListDirectory (and helpers)

        public ListDirectoryResult ListDirectory(BrowserFileInfo path)
        {
            lock (this)
            {
                //return this.DoListDirectory(path);
                ListDirectoryResult result = new ListDirectoryResult(path);
                
                RunPscp(
                    result,
                    ToArgs(this.Session, this.Session.Password, path.Path),
                    ToArgs(this.Session, "XXXXX", path.Path), 
                    null, 
                    null,
                    (lines) =>
                    {
                        // successful list
                        ScpLineParser parser = new ScpLineParser();
                        foreach (string rawLine in lines)
                        {
                            string line = rawLine.TrimEnd();
                            BrowserFileInfo fileInfo;
                            if (parser.TryParseFileLine(line, out fileInfo))
                            {
                                if (fileInfo.Name != ".")
                                {
                                    fileInfo.Path = MakePath(path.Path, fileInfo.Name);
                                    result.Add(fileInfo);
                                }
                            }
                        }
                    });

                return result;
            }
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

        #endregion

        #region CopyFiles (and helpers)

        /// <summary>
        /// Copy files
        /// </summary>
        /// <param name="sourceFiles"></param>
        /// <param name="target"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public FileTransferResult CopyFiles(List<BrowserFileInfo> sourceFiles, BrowserFileInfo target, TransferUpdateCallback callback)
        {
            lock(this)
            {
                /// Known Issues:
                /// - If a large exe (or other file that the OS will virus scan) is tranfered, the operation will timeout.
                ///   After completion, the OS seems to block on the final write (while scanning).  During this time the process looks like it's
                ///   hanging and the timeout logic kicks in.  Hacked in "completed" logic into inlineOut/Err handlers but this is awkward
                
                FileTransferResult result = new FileTransferResult();

                string args = ToArgs(this.Session, this.Session.Password, sourceFiles, target);
                string argsToLog = ToArgs(this.Session, "XXXXX", sourceFiles, target);
                ScpLineParser parser = new ScpLineParser();
                RunPscp(
                    result, args, argsToLog, 
                    (line) => 
                    {
                        bool completed = false;
                        if (callback != null)
                        {
                            FileTransferStatus status;
                            if (parser.TryParseTransferStatus(line, out status))
                            {
                                completed = status.PercentComplete == 100;
                                callback(completed, false, status);
                            }
                        }
                        return completed;
                    }, 
                    null, null);

                return result;
            }
        }

        static string ToArgs(SessionData session, string password, List<BrowserFileInfo> source, BrowserFileInfo target)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("-r -agent ");  // default arguments
            if (!String.IsNullOrEmpty(session.PuttySession))
            {
                sb.Append("-load \"").Append(session.PuttySession).Append("\" ");
            }
            if (!String.IsNullOrEmpty(password))
            {
                sb.Append("-pw ").Append(password).Append(" ");
            }
            sb.AppendFormat("-P {0} ", session.Port);

            if (target.Source == SourceType.Remote)
            {
                // possible to send multiple files remotely at a time
                foreach(BrowserFileInfo file in source)
                {
                    sb.AppendFormat("\"{0}\" ", file.Path);
                }
                sb.AppendFormat(" {0}@{1}:\"{2}\"", session.Username, session.Host, EscapeForUnix(target.Path));
            }
            else
            {
                if (source.Count > 1)
                {
                    Log.WarnFormat("Not possible to transfer multiple remote files locally at one time.  Tranfering first only!");
                }
                sb.AppendFormat(" {0}@{1}:\"{2}\" ", session.Username, session.Host, EscapeForUnix(source[0].Path));
                sb.AppendFormat("\"{0}\"", target.Path);
            }

            return sb.ToString();
        }

        static string EscapeForUnix(string path)
        {
            return path.Replace(" ", @"\ "); 
        }

        #endregion

        #region RunPscp - Main work method
        /// <summary>
        /// Run Pscp synchronously
        /// </summary>
        /// <param name="result">Result object where results </param>
        /// <param name="args">The args to send to pscp</param>
        /// <param name="argsToLog">The args that are logged or can be returned in status messages</param>
        /// <param name="inlineOutHandler">Inline handler for output</param>
        /// <param name="inlineErrHandler">Inline handler for error</param>
        /// <param name="successOutHandler">Handler for output of successful operation</param>
        void RunPscp(
            PscpResult result, 
            string args, 
            string argsToLog, 
            Func<string, bool> inlineOutHandler, 
            Func<string, bool> inlineErrHandler, 
            Action<string[]> successOutHandler)
        {
            if (!File.Exists(this.Options.PscpLocation))
            {
                result.SetError(string.Format("Pscp missing, path={0}.", this.Options.PscpLocation), null);
            }
            else if (this.Session.Username == null)
            {
                result.SetError("UserName is null", null);
            }
            else if (this.Session.Host == null)
            {
                result.SetError("Host is null", null);
            }
            else if (this.Session.Port < 0)
            {
                result.SetError("Invalid port: " + this.Session.Port, null);
            }
            else
            {

                Process proc = NewProcess(this.Options.PscpLocation, args);
                Timer timeoutTimer = null;
                AsyncStreamReader outReader = null;
                AsyncStreamReader errReader = null;
                try
                {
                    // Start pscp
                    Log.InfoFormat("Starting process: file={0}, args={1}", this.Options.PscpLocation, argsToLog);
                    proc.Start();

                    // Timeout when no output is received
                    timeoutTimer = new Timer(
                        (x) => 
                        { 
                            // timeout
                            SafeKill(proc);
                            result.SetErrorFormat("Process timed out, args={0}", argsToLog);
                        }, 
                        null, this.Options.TimeoutMs, Timeout.Infinite);

                    // Async read output/err.  Inline actions to quick kill the process when pscp prompts user.
                    // NOTE: Using BeginReadOutput/ErrorReadLine doesn't work here.  Calls to read an empty stream
                    //       will block (e.g. "user's password:" prompt will block on reading err stream).  
                    outReader = new AsyncStreamReader(
                        "OUT",
                        proc.StandardOutput,
                        strOut =>
                        {
                            bool keepReading = true;
                            bool completed = false;
                            if (strOut == PUTTY_INTERACTIVE_AUTH || strOut.Contains("'s password:"))
                            {
                                result.StatusCode = ResultStatusCode.RetryAuthentication;
                                Log.Debug("Username/Password invalid or not sent");
                                SafeKill(proc);
                                keepReading = false;
                            }
                            else if (inlineOutHandler != null)
                            {
                                completed = inlineOutHandler(strOut);
                            }
                            timeoutTimer.Change(completed ? Timeout.Infinite : this.Options.TimeoutMs, Timeout.Infinite);
                            return keepReading;
                        });
                    errReader = new AsyncStreamReader(
                        "ERR",
                        proc.StandardError,
                        strErr =>
                        {
                            bool keepReading = true;
                            bool completed = false;
                            if (strErr != null && strErr.Contains(PUTTY_NO_KEY))
                            {
                                result.SetError("Host key not cached.  Connect via putty to cache key then try again", null);
                                SafeKill(proc);
                                keepReading = false;
                            }
                            else if (inlineErrHandler != null)
                            {
                                completed = inlineErrHandler(strErr);
                            }
                            timeoutTimer.Change(completed ? Timeout.Infinite : this.Options.TimeoutMs, Timeout.Infinite);
                            return keepReading;
                        });

                    // start process and wait for results
                    Log.DebugFormat("WaitingForExit");
                    proc.WaitForExit();

                    Log.InfoFormat("Process exited, pid={0}, exitCode={1}", proc.Id, proc.ExitCode);
                    timeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    string[] output = outReader.StopAndGetData();
                    string[] err = errReader.StopAndGetData();

                    string outputStr = String.Join("\r\n", output);
                    if (proc.ExitCode == 0 && outputStr.Contains(PUTTY_UNABLE_TO_OPEN))
                    {
                        // bad path
                        int idx = outputStr.IndexOf(PUTTY_UNABLE_TO_OPEN);
                        result.SetErrorFormat(outputStr.Substring(idx));
                    }
                    else if (proc.ExitCode == 0)
                    {
                        // successful operation
                        if (successOutHandler != null)
                        {
                            successOutHandler(output);
                        }
                    }
                    else
                    {
                        // some kind of error
                        if (result.StatusCode != ResultStatusCode.Success)
                        {
                            Log.Debug("Skipping output check since proactively killed process.");
                        }
                        else if (output.Contains(PUTTY_ARGUMENTS_HELP_HEADER))
                        {
                            result.SetErrorFormat("Invalid arguments sent to pscp, args={0}, output={1}", args, output);
                        }
                        else if (err.Contains(PUTTY_HOST_DOES_NOT_EXIST))
                        {
                            result.SetErrorFormat("Host does not exist.  {0}:{1}", this.Session.Host, this.Session.Port);
                        }
                        else
                        {
                            result.SetErrorFormat("Unknown error.  exitCode={0}, out={1}, err={2}", proc.ExitCode, output, err);
                        }
                    }
                }
                finally
                {
                    SafeKill(proc);
                    SafeDispose(timeoutTimer, proc, outReader, errReader);
                }

            }
        }

        #endregion

        #region Utility

        /// <summary>
        ///  Create a process for running pscp
        /// </summary>
        /// <param name="pscpLocation"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static Process NewProcess(string pscpLocation, string args)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = pscpLocation,
                WorkingDirectory = Path.GetDirectoryName(pscpLocation),
                Arguments = args,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            Process proc = new Process
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };
            return proc;
        }

        private static void SafeKill(Process proc)
        {
            if (proc == null) return;
            try
            {
                if (!proc.HasExited)
                {
                    proc.Kill();
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error killing proc, pid=" + proc.Id, ex);
            }
        }

        private static void SafeDispose(params IDisposable[] disposables)
        {
            foreach (IDisposable disposable in disposables)
            {
                if (disposable == null) return;
                try
                {
                    disposable.Dispose();
                }
                catch (Exception ex)
                {
                    Log.Error("Error disposing object: " + disposable, ex);
                }
            }
        }

        #endregion

        public PscpOptions Options { get; private set; }
        public SessionData Session {get; private set; }

        #region AsyncStreamReader
        /// <summary>
        /// Utility class to read a bare stream.  Works around the issues with FileStream blocking when
        /// no data is available...even on peek!  
        /// http://zachsaw.blogspot.com/2011/07/streamreaderpeek-can-block-another-net.html
        /// </summary>
        public class AsyncStreamReader : IDisposable
        {
            private static readonly ILog Log = LogManager.GetLogger(typeof(AsyncStreamReader));

            public AsyncStreamReader(string name, StreamReader reader, Func<string, bool> dataUpdated)
            {
                this.Name = name;
                this.Reader = reader;
                this.DataUpdatedHandler = dataUpdated;
                this.Lines = new List<string>();

                this.Thread = new Thread(this.ReadAll);
                this.Thread.IsBackground = true;
                this.Thread.Start();
            }

            void ReadAll()
            {
                try
                {
                    bool keepReading = true;

                    // read char-by-char at first 10 lines
                    int linesRead = 0;
                    StringBuilder sb = new StringBuilder();
                    while (keepReading && this.Reader.Peek() != -1)
                    {
                        char c = (char)this.Reader.Read();
                        sb.Append(c);

                        // add special case to fire readline if prompted with "user's password: "
                        bool isPossibleHit =
                            (c == '\n') ||
                            (c == ':' && sb.Length > 12 && sb.ToString().EndsWith("'s password:"));

                        //Log.InfoFormat("sb={0}, match={1}", sb, c == ':' && sb.Length > 12 && sb.ToString().EndsWith("'s password:"));
                        if (isPossibleHit)
                        {
                            keepReading = AppendLineAndNotify(sb.ToString());
                            if (++linesRead > 10)
                            {
                                break;
                            }
                            sb = new StringBuilder();
                        }
                    }

                    // after reading 1st line, assume we have a normal read and go by line
                    string line;
                    while (keepReading && (line = this.Reader.ReadLine()) != null)
                    {
                        keepReading = AppendLineAndNotify(line);
                    }
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

            bool AppendLineAndNotify(string line)
            {
                lock (this)
                {
                    bool keepReading = true;

                    string cleanLine = line.Trim('\r', '\n');
                    this.Lines.Add(cleanLine);

                    if (Log.Logger.IsEnabledFor(Level.Trace)) { Log.DebugFormat("[{0}] - {1}", Name, cleanLine); }

                    if (this.DataUpdatedHandler != null)
                    {
                        keepReading = this.DataUpdatedHandler(cleanLine);
                    }

                    return keepReading;
                }
            }

            public string[] StopAndGetData()
            {
                if (this.Thread.IsAlive)
                {
                    // consider better way to know operation is done reading output...timed out join is ok but not great.
                    this.Thread.Join(2000);
                    this.Thread.Abort();
                }

                lock (this)
                {
                    return this.Lines.ToArray();
                }
            }

            public string Name { get; set; }
            StreamReader Reader { get; set; }
            Func<string, bool> DataUpdatedHandler { get; set; }
            List<string> Lines { get; set; }
            Thread Thread { get; set; }

            public void Dispose()
            {
                if (this.Thread.IsAlive)
                {
                    this.Thread.Abort();
                }
            }
        }
        #endregion

        #region ScpLineParser
        public class ScpLineParser
        {
            Regex regExFileLine = new Regex(@"^(?<Permissions>[cdrwx\-lSst]+)\s+(?<LinkCount>\d+)\s+(?<OwnerName>\w+)\s+(?<GroupName>\w+)\s+(?<BlockCount>\d+)\s+(?<Timestamp>(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec).{9})\s{1}(?<FileName>.*)$");
            Regex regExStatus = new Regex(@".*|.*|.*|.*|.*");

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
                    Match match = regExFileLine.Match(line);
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
                            Source = SourceType.Remote
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

            public bool TryParseTransferStatus(string rawLine, out FileTransferStatus status)
            {
                bool success = false;
                status = new FileTransferStatus();

                if (!String.IsNullOrEmpty(rawLine))
                {
                    string line = rawLine.TrimEnd();
                    Match match = this.regExStatus.Match(line);
                    if (match.Success)
                    {
                        string[] update = line.Split('|');

                        status.Filename = update[0].Trim();
                        status.BytesTransferred = int.Parse(update[1].Replace("kB", "").Trim());
                        status.TransferRate = float.Parse(update[2].Replace("kB/s", "").Trim());
                        status.TimeLeft = update[3].Replace("ETA:", "").Trim();
                        status.PercentComplete = int.Parse(update[4].Replace("%", "").Trim());
                        success = true;
                    }
                    else
                    {
                        Log.WarnFormat("Unable to parse OutputData: {0}", line);
                    }
                }
                return success;
            }
        }
        #endregion
    }

    #region PscpOptions
    public class PscpOptions
    {
        public PscpOptions()
        {
            this.TimeoutMs = 10000;
        }

        public string PscpLocation { get; set; }
        public int TimeoutMs { get; set; }
    } 
    #endregion
}
