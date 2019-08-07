using System;
using System.Collections.Generic;
using System.Linq;
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
using GodSharp.SerialPort;

namespace MANDRAKEware
{
    /// <summary>
    /// Interaction logic for MachineConnectionWindow.xaml
    /// </summary>
    public partial class MachineConnectionWindow : Window
    {


        public MachineConnectionWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Connects ardunios to form machine
        /// </summary>
        private void Connect_Btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //will need to update after this
            //    machine.Connect();
             //   peripheral.Connect();
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
            updateSerialPortComboBox(GrblSerialComboBox);
        }
        /// <summary>
        /// Opens all available serial ports for lights ardunio serial port combo box
        /// </summary>
        private void LightsSerialComboBox_DropDownOpened(object sender, EventArgs e)
        {
            updateSerialPortComboBox(LightsSerialComboBox);
        }

        /// <summary>
        /// Updates Combo Box Selection of Serial ports for connecting ardunios
        /// </summary>
        /// <param name="cb">The combobox that is being updated is being passed to the function to get serial ports</param>
        private void updateSerialPortComboBox(ComboBox cb)
        {
            // string sp = Properties.Settings.Default.P;
            // string psp = Properties.Settings.Default.PeripheralSP;
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
        }

        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {

        }
    }
}
