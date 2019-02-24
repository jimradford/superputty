using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPutty.Utils
{
    public static class SerialConnectionOptions
    {
        public readonly static string[] BaudRates =
        {
            "1200", "2400", "4800", "9600", "19200", "38400", "57600", "115200"
        };
        public readonly static string DefaultBaudRate = BaudRates[3];
        public readonly static string[] DataBits = { "7 bit", "8 bit" };
        public readonly static string DefaultDataBits = DataBits[1];
        public readonly static string[] Parity = { "None", "Odd", "Even", "Mark", "Space" };
        public readonly static string DefaultParity = Parity[0];
        public readonly static string[] StopBits = { "1 bit", "1.5 bit", "2 bit" };
        public readonly static string DefaultStopBits = StopBits[0];
        public readonly static string[] FlowControl = { "None", "XON/XOFF", "RTS/CTS", "DSR/DTR" };
        public readonly static string DefaultFlowControl = FlowControl[0];
    }
}
