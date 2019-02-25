using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        internal static void InitializeSerialPortCombo(Component uiComponent, string value="")
        {
            initializeSerialCombo(uiComponent, SerialPort.GetPortNames(), "", value);
        }

        internal static void InitializeSerialSpeedCombo(Component uiComponent, string value = "")
        {
            initializeSerialCombo(uiComponent, BaudRates, DefaultBaudRate, value);
        }

        internal static void InitializeSerialStopBitsCombo(Component uiComponent, string value = "")
        {
            initializeSerialCombo(uiComponent, StopBits, DefaultStopBits, value);
        }

        internal static void InitializeSerialDataBitsCombo(Component uiComponent, string value = "")
        {
            initializeSerialCombo(uiComponent, DataBits, DefaultDataBits, value);
        }

        internal static void InitializeSerialParityCombo(Component uiComponent, string value = "")
        {
            initializeSerialCombo(uiComponent, Parity, DefaultParity, value);
        }

        internal static void InitializeSerialFlowCtrlCombo(Component uiComponent, string value = "")
        {
            initializeSerialCombo(uiComponent, FlowControl, DefaultFlowControl, value);
        }

        /// <summary>
        /// Local utility method for setting up combo boxes in the GUI.
        /// 
        /// Will set the combo box items to the listValues.
        /// If 'value' is a non-empty string, will make sure it is an option in the combo box. 
        ///     If its not, will add it to the list of options.
        /// If the 'value' is in the listValues, will set the current selection to value. 
        /// If 'value' is an empty string, will set the current selection to defaultSelection.
        /// </summary>
        /// <param name="uiComponent">ComboBox or ToolStripComboBox object to configure</param>
        /// <param name="listValues">List of options that are available in the combo box</param>
        /// <param name="defaultSelection">If 'value' is not specified, the combobox will be set to this</param>
        /// <param name="value">The ComboBox will be set to this value as the selected item.</param>
        private static void initializeSerialCombo(Component uiComponent, string[] listValues, string defaultSelection, string value)
        {
            ComboBox thisComboBox = getComboBoxObject(uiComponent);
            thisComboBox.Items.Clear();
            thisComboBox.Items.AddRange(listValues);
            if ((value != null) && (value.Length > 0))
            {
                if (!thisComboBox.Items.Contains(value))
                    thisComboBox.Items.Add(value);
                thisComboBox.SelectedItem = value;
            }
            else if (thisComboBox.Items.Count > 0)
            {
                if (defaultSelection.Length > 0)
                {
                    thisComboBox.SelectedItem = defaultSelection;
                }
                else { thisComboBox.SelectedIndex = 0; }
            }
        }


        /// <summary>
        /// The utilities in this class can be used to initialize Tool Strip combo boxes, or
        /// normal combo boxes. This method will check to see if the component is a Combo
        /// Box or Tool Strip Combo. If its a tool strip combo box, it will return the embedded
        /// combo box object.
        /// </summary>
        /// <param name="parentComponent"></param>
        /// <returns></returns>
        private static ComboBox getComboBoxObject(Component parentComponent)
        {
            if (parentComponent is ComboBox)
                return (ComboBox)parentComponent;
            else if (parentComponent is ToolStripComboBox)
                return ((ToolStripComboBox)parentComponent).ComboBox;
            else
                return null;
        }


        /// <summary>
        /// The -SERCFG option that is passed into putty uses different codes than the human-readable
        /// options specified in this class. This method will convert them to what putty is expecting
        /// 
        /// From the putty documentation, options are:
        ///         A single lower-case letter specifies the parity: ‘n’ for none, ‘o’ for odd, 
        ///         ‘e’ for even, ‘m’ for mark and ‘s’ for space.
        /// </summary>
        /// <param name="parity">Human-readable parity string</param>
        /// <returns></returns>
        public static string ParityStrToPuttyCode(string parity)
        {
            return parity.ToLower().Substring(0, 1);
        }

        /// <summary>
        /// The -SERCFG option that is passed into putty uses different codes than the human-readable
        /// options specified in this class. This method will convert them to what putty is expecting
        /// 
        /// From the putty documentation, options are:
        ///         A single upper-case letter specifies the flow control: ‘N’ for none, 
        ///         ‘X’ for XON/XOFF, ‘R’ for RTS/CTS and ‘D’ for DSR/DTR.
        /// </summary>
        /// <param name="flow">Flow Control String</param>
        /// <returns></returns>
        public static string FlowControlToPuttyCode(string flow)
        {
            return flow.ToUpper().Substring(0, 1);
        }

        /// <summary>
        /// The -SERCFG option that is passed into putty uses different codes than the human-readable
        /// options specified in this class. This method will convert them to what putty is expecting
        /// 
        /// From the putty documentation, options are:
        ///         ‘1’, ‘1.5’ or ‘2’ sets the number of stop bits.
        /// </summary>
        /// <param name="stopBits"></param>
        /// <returns></returns>
        public static string StopBitsToPuttyCode(string stopBits)
        {
            switch(stopBits.ToLower())
            {
                case "1 bit":
                    return "1";
                case "1.5 bit":
                    return "1.5";
                case "2 bit":
                    return "2";
                default:
                    return "1";
            }

        }


        /// <summary>
        /// The -SERCFG option that is passed into putty uses different codes than the human-readable
        /// options specified in this class. This method will convert them to what putty is expecting
        /// 
        /// From the putty documentation, options are:
        ///         Any single digit from 5 to 9 sets the number of data bits.
        /// </summary>
        /// <param name="flow">Flow Control String</param>
        /// <returns></returns>
        public static string DataBitsToPuttyCode(string dataBits)
        {
            string retString = "8";
            int result = -1;
            if (Int32.TryParse(dataBits.Substring(0, 1),out result))
            {
                retString = result.ToString();
            }
            return retString;
        }

        
    }
}
