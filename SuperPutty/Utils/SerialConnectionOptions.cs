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

        public static void initializeSerialCombo(Component uiComponent, string[] listValues, string defaultSelection, string value)
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
    }
}
