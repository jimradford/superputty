using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperPutty.Data;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using log4net;
using System.Runtime.Remoting.Messaging;

namespace SuperPutty.Utils
{
    public class SingleInstanceHelper : MarshalByRefObject
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SingleInstanceHelper));

        public const string ChannelName = "SuperPuTTY";
        public const string SingleInstanceServiceName = "SingleInstance";

        public static void RegisterRemotingService()
        {
            try
            {
                IpcChannel ipcCh = new IpcChannel(ChannelName);
                ChannelServices.RegisterChannel(ipcCh, false);
                RemotingConfiguration.RegisterWellKnownServiceType(
                    typeof(SingleInstanceHelper), SingleInstanceServiceName, WellKnownObjectMode.SingleCall);

                Log.InfoFormat("Registered IpcChannel and Service: [ipc://{0}/{1}]", ChannelName, SingleInstanceServiceName);
            }
            catch (Exception ex)
            {
                Log.Warn("Unable to register ipcchannel for single instance support...feature unavailable", ex);
            }
        }

        public static void LaunchInExistingInstance(string[] args)
        {
            IpcChannel channel = new IpcChannel();
            ChannelServices.RegisterChannel(channel, false);
            string url = String.Format("ipc://{0}/{1}", ChannelName, SingleInstanceServiceName);

            // Register as client for remote object.
            WellKnownClientTypeEntry remoteType = new WellKnownClientTypeEntry(typeof(SingleInstanceHelper), url);
            RemotingConfiguration.RegisterWellKnownClientType(remoteType);

            // Create a message sink.
            string objectUri;
            IMessageSink messageSink = channel.CreateMessageSink(url, null, out objectUri);

            /*
            Console.WriteLine("The URI of the message sink is {0}.", objectUri);
            if (messageSink != null)
            {
                Console.WriteLine("The type of the message sink is {0}.", messageSink.GetType().ToString());
            }
            */

            SingleInstanceHelper helper = new SingleInstanceHelper();
            helper.Run(args);
        }


        public void Run(String[] args)
        {
            Log.InfoFormat("Received remote Run command: [{0}]", String.Join(" ", args));
            CommandLineOptions cmd = new CommandLineOptions(args);
            SessionDataStartInfo ssi = cmd.ToSessionStartInfo();
            SuperPuTTY.OpenSession(ssi);
        }
    }
}
