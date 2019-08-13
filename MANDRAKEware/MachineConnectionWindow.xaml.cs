using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using log4net;
using MANDRAKEware_Events.Machine.GrblArdunio;
using MANDRAKEware_Events.Machine;
using MandrakeEvents;
using MandrakeEvents.Machine.LightsArdunio;

namespace MANDRAKEware
{
    /// <summary>
    /// Interaction logic for MachineConnectionWindow.xaml
    /// </summary>
    public partial class MachineConnectionWindow : Window
    {
        //logger
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        // private int NumMachines; //testing for machines
        private GRBLArdunio gArdunio = GRBLArdunio.Instance;
        private LightsArdunio litArdunio = LightsArdunio.Instance;

        public MachineConnectionWindow()
        {
            InitializeComponent();

            log.Info("Machine Connection Window Entered");

            //for now will initalize one machien in start  
        }

        /// <summary>
        /// Connects ardunios to form machine
        /// </summary>
        private void Connect_Btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                gArdunio.Connect();
                litArdunio.Connect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        //Couldn't figure out how to make more modular so will try in future, works for now
        /// <summary>
        /// Opens all available serial ports for grbl serial port combo box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GrblSerialComboBox_DropDownOpened(object sender, EventArgs e)
        {
            log.Info("Grbl Serial Port Combo Box Opened");

            updateSerialPortComboBox(GrblSerialComboBox);
        }
        /// <summary>
        /// Opens all available serial ports for lights ardunio serial port combo box
        /// </summary>
        private void LightsSerialComboBox_DropDownOpened(object sender, EventArgs e)
        {
            log.Info("Lights Ardunio Serial Port Combo Box Opened");

            updateSerialPortComboBox(LightsSerialComboBox);
        }

        /// <summary>
        /// Updates Combo Box Selection of Serial ports for connecting ardunios
        /// </summary>
        /// <param name="cb">The combobox that is being updated is being passed to the function to get serial ports</param>
        private void updateSerialPortComboBox(ComboBox cb)
        {
            cb.Items.Clear();
            String[] ports = System.IO.Ports.SerialPort.GetPortNames();
            int i = 0;
            int selectedIndex = 0;

            foreach (string port in ports)
            {
                if (string.Equals(cb.Name, "PeripheralSerialPortSelect"))
                {
                    selectedIndex = i;
                }
                if (string.Equals(cb.Name, "SerialPortSelect"))
                {
                    selectedIndex = i;
                }

                cb.Items.Add(port);
                i++;
            }
            cb.SelectedIndex = selectedIndex;

            log.Debug("Serial Ports Updated for Combo Box");
        }

        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Method for event of number of machine ticker changing
        /// </summary>
        private void NumberOfMachines_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //Will add when adding multi machines
        }

        //closing window method
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //TODO
        }
    }
}
