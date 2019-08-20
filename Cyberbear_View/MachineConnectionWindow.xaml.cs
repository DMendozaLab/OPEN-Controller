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
using System.IO;
using MandrakeEvents.Machine.CameraControl;

namespace MANDRAKEware
{
    /// <summary>
    /// Interaction logic for MachineConnectionWindow.xaml
    /// </summary>
    public partial class MachineConnectionWindow : Window
    {
        //logger
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private GRBLArdunio gArdunio = GRBLArdunio.Instance;
        private LightsArdunio litArdunio = LightsArdunio.Instance;
        private CameraControl cameraControl = CameraControl.Instance;

        public BitmapImage bi; //image going to be captured



        public MachineConnectionWindow()
        {
            InitializeComponent();

            log.Info("Machine Connection Window Entered");

            //CAMERA UPDATE FUNCTIONS   
         //   updateCameraSettingsOptions();

            Task task = new Task(() => cameraControl.StartVimba());
            task.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
            task.Start();

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
                log.Info("GRBL Ardunio Connected");
                litArdunio.Connect();
                log.Info("Lights Ardunio Connected");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                log.Debug("Failure to connect to serial ports because: " + ex.Message);
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

        /// <summary>
        /// If drop down opened
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            ((ComboBox)sender).Items.Clear();

            foreach (string port in System.IO.Ports.SerialPort.GetPortNames())
                ((ComboBox)sender).Items.Add(port);
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
            gArdunio.Disconnect();
            log.Info("Grbl Ardunio Disconnected");
            litArdunio.Disconnect();
            log.Info("Lights Ardunio Disconnected");
            cameraControl.ShutdownVimba();
            log.Info("Camera Control shutdown");
        }
        /// <summary>
        /// Event for start button of MachineConnectionWindow, takes in a hard coded file
        /// and reads the commands line by line and sends to gArdunio property
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartManualCycleBtn_Click(object sender, RoutedEventArgs e)
        {
            log.Info("Starting a Manual Cycle");
            //BitmapImage bi; //image going to be captured
            string filePath = @"C:\Users\lsceedlings\Desktop\Lando's Folder\GRBLCommands.txt";

            log.Debug("Using the file: " + filePath);

            List<string> lines = File.ReadAllLines(filePath).ToList(); //putting all the lines in a list

            foreach (string line in lines)
            {
                gArdunio.SendLine(line); //sending line to ardunio
                log.Info("G Command Sent: " + line);

                bi = cameraControl.CapSaveImage().Clone(); //capture image
                bi.Freeze(); //freezes image to avoid need for copying to display and put in other threads
                //may need to raise event to work but idk
            }
        }

        /// <summary>
        /// Stops manual cycle when started
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopManualCycleBtn_Click(object sender, RoutedEventArgs e)
        {
           //TODO
        }

        /// <summary>
        /// After connecting machines, the start button is made able to click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartManualCycleBtn_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //if both ardunio connect then allow the button to be clicked
            //if(gArdunio.Connected == true && litArdunio.Connected == true)
            //{
            //    this.IsEnabled = true;
            //}
        }

        /// <summary>
        /// Exception Handler when expections happens
        /// </summary>
        /// <param name="task">The task were the expection occurred is passed to the method
        /// to log the error</param>
        static void ExceptionHandler(Task task)
        {
            var exception = task.Exception;
            log.Error(exception);
        }

        private void CameraList_cb_DropDownOpened(object sender, EventArgs e)
        {
            updateCameraSettingsOptions();
        }

        /// <summary>
        /// Updates Camera Settings Options for combo box, may change in future
        /// </summary>
        private void updateCameraSettingsOptions()
        {
            string csPath = CameraConst.CameraSettingsPath;
            CameraList_cb.Items.Clear();

            DirectoryInfo d = new DirectoryInfo("./Resources/CameraSettings/");//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.xml"); //Getting Text files with .xml at the end
                                                    //string str = "";

            int i = 0;
            int selectedIndex = i;
            foreach (FileInfo file in Files)
            {
                if (string.Equals(file.Name, csPath))
                {
                    selectedIndex = i;
                }
                CameraList_cb.Items.Add(file);
                i++;
            }
            //int index = Files.FindIndxx var match = Files.FirstOrDefault(file => file.Name.Contains(Properties.Settings.Default.CameraSettingsPath));
            Dispatcher.Invoke(() =>
            {//this refer to form in WPF application 
                CameraList_cb.SelectedIndex = selectedIndex;
                //if (match == null)
                //{
                //    //cameraSettingsCB.SelectedItem = 0;

                //}
                //else
                //{
                //    cameraSettingsCB.SelectedItem = Properties.Settings.Default.CameraSettingsPath;
                //}
            });

        }

        /// <summary>
        /// Updates Camera instance of new settings chosen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CameraList_cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Task task = new Task(() => cameraControl.loadCameraSettings());
            task.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
            task.Start();
        }
    }
}
