using System.Collections.Generic;
using NUnit.Framework;
using System.Diagnostics;
using log4net;
using System.Threading;
using System.Configuration;

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
            lineNum = 0;
            outData = new List<string>();
            errData = new List<string>();

            if (proc != null)
            {
                proc.Refresh();
                if (!proc.HasExited)
                {
                    proc.Kill();
                }
            }
            proc = new Process();
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
                FileName = ScpConfig.Default.PscpLocation,
                Arguments = args,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            proc.StartInfo = startInfo;
            proc.EnableRaisingEvents = true;

            Log.Info("Starting list");
            if (proc.Start())
            {
                proc.BeginErrorReadLine();
                proc.BeginOutputReadLine();
                proc.OutputDataReceived += proc_OutputDataReceived;
                proc.ErrorDataReceived += proc_ErrorDataReceived;
                proc.WaitForExit();
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
        public void DirectoryListingSuccess()
        {
            ListDirectory(ScpConfig.Default.UserName, ScpConfig.Default.Password, ScpConfig.Default.KnownHost);
            Assert.Greater(this.outData.Count, 0);
        }

        /// <summary>
        /// Bad password will hang with an auth failed messsage and prompt for password which blocks/locks the
        /// process
        /// </summary>
        [Test]
        public void DirectoryListingBadPassword()
        {
            bool returned = false;
            Thread t = new Thread(x => 
            {
                ListDirectory(ScpConfig.Default.UserName, ScpConfig.Default.Password + "zzz", ScpConfig.Default.KnownHost);
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
        public void DirectoryListingHostNoKey()
        {
            bool returned = false;
            Thread t = new Thread(x =>
            {
                ListDirectory(ScpConfig.Default.UserName, ScpConfig.Default.Password, ScpConfig.Default.UnKnownHost);
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

    public class ScpConfig
    {
        public static readonly ScpConfig Default = new ScpConfig();

        public ScpConfig()
        {
            this.PscpLocation = ConfigurationManager.AppSettings["SuperPuTTY.ScpTests.PscpLocation"];
            this.UserName = ConfigurationManager.AppSettings["SuperPuTTY.ScpTests.UserName"];
            this.Password = ConfigurationManager.AppSettings["SuperPuTTY.ScpTests.Password"];
            this.KnownHost = ConfigurationManager.AppSettings["SuperPuTTY.ScpTests.KnownHost"];
            this.UnKnownHost = ConfigurationManager.AppSettings["SuperPuTTY.ScpTests.UnKnownHost"];
        }

        public string PscpLocation { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string KnownHost { get; set; }
        public string UnKnownHost { get; set; }
    }

}
