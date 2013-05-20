using System.Collections.Generic;
using NUnit.Framework;
using System.Diagnostics;
using log4net;
using System.Threading;
using System.Configuration;
using SuperPutty.Data;
using SuperPutty.Scp;
using System;
using System.IO;

namespace SuperPuttyUnitTests.Scp
{
    /// <summary>
    /// Basic SCP tests to test underlying process invocation limitations/quirks
    /// </summary>
    [TestFixture]
    class ScpTests
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ScpTests));

        static ScpTests()
        {
            Program.InitLoggingForUnitTests();
        }

        Process proc;
        List<string> outData;
        List<string> errData;
        int lineNum = 0;

        [SetUp]
        public void Setup()
        {
            Log.InfoFormat("Setup");

            if (proc != null)
            {
                try
                {
                    proc.Refresh();
                    if (!proc.HasExited)
                    {
                        proc.Kill();
                    }
                }
                catch(Exception ex)
                {
                    Log.Warn("Error in Setup", ex);
                }
            }
            proc = new Process();

            lineNum = 0;
            outData = new List<string>();
            errData = new List<string>();
        }

        void proc_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Log.InfoFormat("ERR {0:000} {1}", ++lineNum, e.Data);
            this.errData.Add(e.Data);
        }

        void proc_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Log.InfoFormat("OUT {0:000} {1}", ++lineNum, e.Data);
            this.outData.Add(e.Data);
        }

        void ListDirectory(string user, string password, string host)
        {
            string args = string.Format(
                "-ls -load \"Default Settings\" -P 22 -pw {0} {1}@{2}:.",
                password, user, host);
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = ScpConfig.PscpLocation,
                Arguments = args,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            proc.StartInfo = startInfo;
            proc.EnableRaisingEvents = true;

            Log.InfoFormat("Starting list: args={0}", args);
            if (proc.Start())
            {
                proc.OutputDataReceived += proc_OutputDataReceived;
                proc.ErrorDataReceived += proc_ErrorDataReceived;

                Log.Info("BeginErrorReadLine()");
                proc.BeginErrorReadLine();
                Log.Info("BeginOutputReadLine()");
                proc.BeginOutputReadLine();
                Log.Info("WaitForExit(5000)");
                proc.WaitForExit(5000);

                proc.CancelErrorRead();
                proc.CancelOutputRead();
                Log.InfoFormat("Done");
            }
            else
            {
                Assert.Fail("Process failed to start");
            }
        }


        /// <summary>
        /// Simple test to run scp and get a listing back.  
        /// </summary>
        [Test]
        public void ListDirSuccess()
        {
            ListDirectory(ScpConfig.UserName, ScpConfig.Password, ScpConfig.KnownHost);
            Assert.Greater(this.outData.Count, 0);
        }

        /// <summary>
        /// Bad password will hang with an auth failed messsage and prompt for password which blocks/locks the
        /// process
        /// </summary>
        [Test]
        public void ListDirBadPassword()
        {
            bool returned = false;
            Thread t = new Thread(x => 
            {
                ListDirectory(ScpConfig.UserName, ScpConfig.Password + "zzz", ScpConfig.KnownHost);
                lock (this)
                {
                    Monitor.Pulse(this);
                    returned = true;
                }
            });
            t.IsBackground = true;
            t.Start();

            lock (this)
            {
                Monitor.Wait(this, 5000);
            }
            Assert.False(returned);
            Assert.AreEqual(0, this.outData.Count);
            Assert.AreEqual(0, this.errData.Count);
        }

        /// <summary>
        /// Unknown host will hang asking to accept key on output stream
        /// </summary>
        [Test]
        public void ListDirHostNoKey()
        {
            bool returned = false;
            Thread t = new Thread(x =>
            {
                ListDirectory(ScpConfig.UserName, ScpConfig.Password, ScpConfig.UnKnownHost);
                lock (this)
                {
                    Monitor.Pulse(this);
                    returned = true;
                }
            });
            t.IsBackground = true;
            t.Start();

            lock (this)
            {
                Monitor.Wait(this, 5000);
            }
            Assert.False(returned);
            Assert.AreEqual(0, this.outData.Count);
            Assert.Greater(this.errData.Count, 1);
            Assert.IsTrue(this.errData[0].Contains("The server's host key is not cached "));
        }
    }

    [TestFixture]
    public class PscpClientListDirTests 
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PscpClientListDirTests));

        [Test]
        public void ListDirSuccess()
        {
            SessionData session = new SessionData
            {
                Username = ScpConfig.UserName, 
                Password = ScpConfig.Password, 
                Host = ScpConfig.KnownHost, 
                Port = 22
            };

            PscpClient client = new PscpClient(ScpConfig.PscpLocation, session);

            ListDirectoryResult res = client.ListDirectory(new BrowserFileInfo { Path = "." });

            Assert.AreEqual(ResultStatusCode.Success, res.StatusCode);
            Assert.Greater(res.Files.Count, 0);
            foreach (BrowserFileInfo file in res.Files)
            {
                Log.Info(file);
            }

            Log.InfoFormat("Result: {0}", res);
        }

        [Test]
        public void ListDirBadPassword()
        {
            SessionData session = new SessionData
            {
                Username = ScpConfig.UserName,
                Password = ScpConfig.Password + "xxx",
                Host = ScpConfig.KnownHost,
                Port = 22
            };

            PscpClient client = new PscpClient(ScpConfig.PscpLocation, session);

            ListDirectoryResult res = client.ListDirectory(new BrowserFileInfo { Path = "." });

            Assert.AreEqual(ResultStatusCode.RetryAuthentication, res.StatusCode);
            Assert.AreEqual(res.Files.Count, 0);

            Log.InfoFormat("Result: {0}", res);
        }

        [Test]
        public void ListDirHostNoKey()
        {
            SessionData session = new SessionData
            {
                Username = ScpConfig.UserName,
                Password = ScpConfig.Password,
                Host = ScpConfig.UnKnownHost,
                Port = 22
            };

            PscpClient client = new PscpClient(ScpConfig.PscpLocation, session);

            ListDirectoryResult res = client.ListDirectory(new BrowserFileInfo { Path = "." });

            Assert.AreEqual(ResultStatusCode.Error, res.StatusCode);
            Assert.AreEqual(res.Files.Count, 0);

            Log.InfoFormat("Result: {0}", res);
        }

        [Test]
        public void ListDirBadPath()
        {
            SessionData session = new SessionData
            {
                Username = ScpConfig.UserName,
                Password = ScpConfig.Password,
                Host = ScpConfig.KnownHost,
                Port = 22
            };

            PscpClient client = new PscpClient(ScpConfig.PscpLocation, session);

            ListDirectoryResult res = client.ListDirectory(new BrowserFileInfo { Path = "some_non_existant_dir" });

            Assert.AreEqual(ResultStatusCode.Error, res.StatusCode);
            Assert.AreEqual(0, res.Files.Count);
            Assert.IsTrue(res.ErrorMsg != null && res.ErrorMsg.StartsWith("Unable to open"));
            Log.InfoFormat("Result: {0}", res);
        }
    }

    [TestFixture]
    public class PscpClientTransferTests
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PscpClientTransferTests));

        static PscpClientTransferTests()
        {
            Program.InitLoggingForUnitTests();
        }

        [Test]
        public void LocalToRemote()
        {
            SessionData session = new SessionData
            {
                Username = ScpConfig.UserName,
                Password = ScpConfig.Password,
                Host = ScpConfig.KnownHost,
                Port = 22
            };

            List<BrowserFileInfo> sourceFiles = new List<BrowserFileInfo> 
            {
                //new BrowserFileInfo(new FileInfo(Path.GetTempFileName()))
                new BrowserFileInfo(new FileInfo( @"D:\Downloads\vs2012_winexp_enu.iso" ))
            };
            BrowserFileInfo target = new BrowserFileInfo
            { 
                Path = string.Format("/home/{0}/", session.Username), 
                Source = SourceType.Remote
            };

            PscpClient client = new PscpClient(ScpConfig.PscpLocation, session, 5000);
            PscpResult res = client.CopyFiles(
                sourceFiles, 
                target, 
                (complete, cancelAll, status) => 
                {
                    Log.InfoFormat(
                        "complete={0}, cancelAll={1}, fileName={2}, pctComplete={3}", 
                        complete, cancelAll, status.Filename, status.PercentComplete);
                });

            Log.InfoFormat("Result: {0}", res);

            /*
            ListDirectoryResult res = client.ListDirectory(new BrowserFileInfo { Path = "." });

            Assert.AreEqual(ResultStatusCode.Success, res.StatusCode);
            Assert.Greater(res.Files.Count, 0);
            foreach (BrowserFileInfo file in res.Files)
            {
                Log.Info(file);
            }

            Log.InfoFormat("Result: {0}", res);*/
        }
    }

    public class ScpConfig
    {
        public static readonly string PscpLocation = ConfigurationManager.AppSettings["SuperPuTTY.ScpTests.PscpLocation"];
        public static readonly string UserName = ConfigurationManager.AppSettings["SuperPuTTY.ScpTests.UserName"];
        public static readonly string Password = ConfigurationManager.AppSettings["SuperPuTTY.ScpTests.Password"];
        public static readonly string KnownHost = ConfigurationManager.AppSettings["SuperPuTTY.ScpTests.KnownHost"];
        public static readonly string UnKnownHost = ConfigurationManager.AppSettings["SuperPuTTY.ScpTests.UnKnownHost"];
    }
}
