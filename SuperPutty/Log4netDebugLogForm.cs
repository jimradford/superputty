using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using log4net.Appender;
using log4net;
using log4net.Repository.Hierarchy;
using log4net.Core;
using log4net.Layout;
using System.IO;

namespace SuperPutty
{
    public partial class Log4netLogViewer : ToolWindow
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Log4netLogViewer));

        private MemoryAppender memoryAppender;
        private Hierarchy repository;
        private StringWriter msgWriter;

        public Log4netLogViewer()
        {
            InitializeComponent();

            InitLogger();

            // start pulling log messages
            this.timerLogPull.Start();

            Log.Info("Viewer Ready");
        }

        /// <summary>
        /// Init log4net memoryAppender 
        /// http://dhvik.blogspot.com/2008/08/adding-appender-to-log4net-in-runtime.html
        /// </summary>
        void InitLogger()
        {
            //First create and configure the appender  
            this.memoryAppender = new MemoryAppender();
            this.memoryAppender.Name = this.GetType().Name + "MemoryAppender";

            PatternLayout layout = new PatternLayout();
            layout.ConversionPattern = "%date %-5level %20.20logger{1} - %message%newline";
            layout.ActivateOptions();
            this.memoryAppender.Layout = layout;

            //Notify the appender on the configuration changes  
            this.memoryAppender.ActivateOptions();

            //Get the logger repository hierarchy.  
            this.repository = (Hierarchy)LogManager.GetRepository();

            //and add the appender to the root level  
            //of the logging hierarchy  
            this.repository.Root.AddAppender(memoryAppender);

            //configure the logging at the root.  
            //this.repository.Root.Level = Level.Info;

            //mark repository as configured and notify that is has changed.  
            this.repository.Configured = true;
            this.repository.RaiseConfigurationChanged(EventArgs.Empty);

            this.msgWriter = new StringWriter();
        }

        void DisposeLogger()
        {
            // remove appender and drain events
            this.repository.Root.RemoveAppender(this.memoryAppender);
            this.memoryAppender.Clear();

            //mark repository as configured and notify that is has changed.  
            this.repository.Configured = true;
            this.repository.RaiseConfigurationChanged(EventArgs.Empty);
        }

        private void Log4netLogViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            Log.Info("Shutting down logViewer");
            this.timerLogPull.Stop();
            this.DisposeLogger();
        }

        private void timerLogPull_Tick(object sender, EventArgs e)
        {
            this.timerLogPull.Stop();
            try
            {
                LoggingEvent[] events = this.memoryAppender.GetEvents();
                this.memoryAppender.Clear();
                foreach (LoggingEvent logEvent in events)
                {
                    this.memoryAppender.Layout.Format(this.msgWriter, logEvent);
                    this.richTextBoxLogMessages.AppendText(this.msgWriter.GetStringBuilder().ToString());
                    this.msgWriter.GetStringBuilder().Clear(); // Remove(0, this.msgWriter.GetStringBuilder().Length);
                    //this.richTextBoxLogMessages.AppendText(Environment.NewLine);
                }
            }
            finally
            {
                this.timerLogPull.Start();
            }
        }

    }
}
