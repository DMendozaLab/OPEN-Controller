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
using System.IO;
using Cyberbear_Events.MachineControl;
using Cyberbear_Events.MachineControl.GrblArdunio;
using Cyberbear_Events.MachineControl.LightingControl;
using Cyberbear_Events.MachineControl.CameraControl;
using Cyberbear_Events;
using Cyberbear_View.Consts;
using System.Threading;
using Cyberbear_Events.Util;
using System.Drawing;
using static Cyberbear_Events.MachineControl.LightingControl.LightsArdunio;

namespace Cyberbear_View
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
        private Camera cameraControl = Camera.Instance;

        private Machine machine = new Machine();


        /// <summary>
        /// Constructor for Machine Connection Window Class
        /// </summary>
        public MachineConnectionWindow()
        {
            InitializeComponent();

            log.Info("Machine Connection Window Entered");

            //setting name of window and machine
            var w = new MachineNameWindow();
            if (w.ShowDialog() == true) //gotta make sure no memory leak
            {
                machine.Name = w.TextBoxName;
                this.Title = machine.Name;
                
            }
            
            Task task = new Task(() => cameraControl.StartVimba());
            task.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
            task.Start();

            //for now will initalize one machine in start  
        }
        #region Window Closing
        /// <summary>
        /// For when Machine Window Closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Disconnect_Machine();

            log.Info("Machine Connection Window Closed");
        }
        #endregion

        #region Connection and Disconnection
        /// <summary>
        /// Connects ardunios to form machine
        /// </summary>
        private void Connect_Btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Connect_Machine();

                //start and stop buttons enabling to be pressed
                StartManualCycleBtn.IsEnabled = true;
                StopManualCycleBtn.IsEnabled = true;

                StartTimelapseCycleBtn.IsEnabled = true;
                StopTimelapseCycleBtn.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                log.Debug("Failure to connect to serial ports because: " + ex.Message);
            }
        }

        /// <summary>
        /// Connects Grbl and Lights Ardunio and logs the connections
        /// </summary>
        private void Connect_Machine()
        {
            try
            {
                //testing machine object
                if(machine.GrblArdunio.Connected == false)
                {
                    machine.GrblArdunio.Connect();
                    log.Info("GRBL Ardunio Connected");
                }
                if (litArdunio.Connected ==false)
                {
                    litArdunio.Connect();
                    log.Info("Lights Ardunio Connected");

                    Thread.Sleep(500);
                    setLightWhite();
                } 
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        /// <summary>
        /// Connection button is enabled after both combo boxes filled with serial ports. Method
        /// is raised after closing drop down box in combo box 
        /// </summary>
        private void Connect_Btn_CanEnable()
        {
            //if selected index greater than -1 then something was selected in the combo box
            if(GrblSerialComboBox.SelectedIndex > -1 && LightsSerialComboBox.SelectedIndex > -1)
            {
                Connect_Btn.IsEnabled = true;
                Disconnect_Btn.IsEnabled = true;

                log.Debug("Connection button is enabled");
            }
        }

        /// <summary>
        /// Disconnects Machines
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Disconnect_Btn_Click(object sender, RoutedEventArgs e)
        {
            Disconnect_Machine();

            Connect_Btn.IsEnabled = false;
            Disconnect_Btn.IsEnabled = false;

            log.Info("Disconnect Button Pressed");
        }

        /// <summary>
        /// When called, will disconnect gardunio and lit ardunio
        /// </summary>
        private void Disconnect_Machine()
        {
            if (gArdunio.Connected == true)
            {
                gArdunio.Disconnect();
                log.Info("Grbl Ardunio Disconnected");
            }
            else if (litArdunio.Connected == true)
            {
                ButtonBackLightOff(); //turn lights off before disconnecting

                litArdunio.Disconnect();
                log.Info("Lights Ardunio Disconnected");
            }
            else
            {
                cameraControl.ShutdownVimba();
                log.Info("Camera Control Shutdown");
            }

            log.Info("Machine Disconnected");
        }
        #endregion
 
        #region Serial Port Communication
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
                if (string.Equals(cb.Name, "LightsSerialComboBox"))
                {
                    selectedIndex = i;
                }
                if (string.Equals(cb.Name, "GrblSerialComboBox"))
                {
                    selectedIndex = i;
                }

                cb.Items.Add(port);
                i++;
            }

            string selectPort = ports[i-1];
            UpdateSerialConst(selectPort, cb);

            cb.SelectedIndex = selectedIndex;

            log.Debug("Serial Ports Updated for Combo Box");

            //sees if connection button can be enabled
            Connect_Btn_CanEnable();
        }

        /// <summary>
        /// Updates the Serial const class when port value is changed for connections
        /// </summary>
        /// <param name="comPort">The string name of the comPort selected in the combo box</param>
        /// <param name="cb">The name of the combo box that will have the const updated</param>
        private void UpdateSerialConst(string comPort, ComboBox cb)
        {
            if (string.Equals(cb.Name, "LightsSerialComboBox"))
            {
                SerialConsts.defaultComPortLights = comPort;
                log.Debug("Serial Port for lights is: " + comPort);
            }
            if (string.Equals(cb.Name, "GrblSerialComboBox"))
            {
                SerialConsts.defaultComPortGrbl = comPort;
                log.Debug("Serial Port for Grbl is: " + comPort);
            }
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
        #endregion

        #region Exception Handler
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
        #endregion
        
        #region Camera Settings 
        //private void CameraList_cb_DropDownOpened(object sender, EventArgs e)
        //{
        //    updateCameraSettingsOptions();
        //}

        /// <summary>
        /// Updates Camera Settings Options for combo box, may change in future
        /// </summary>
        /// Commenting out for now because not problem
        //private void updateCameraSettingsOptions()
        //{
        //    string csPath = CameraConst.CameraSettingsPath;
        //    CameraList_cb.Items.Clear();

        //    DirectoryInfo d = new DirectoryInfo(@"C:\Users\sam998\Desktop\Cyberbear\Cyberbear\Cyberbear_Events\Machine\CameraControl\CameraSettings");//Assuming Test is your Folder
        //    FileInfo[] Files = d.GetFiles("*.xml"); //Getting Text files with .xml at the end
        //                                            //string str = "";

        //    int i = 0;
        //    int selectedIndex = i;
        //    foreach (FileInfo file in Files)
        //    {
        //        if (string.Equals(file.Name, csPath))
        //        {
        //            selectedIndex = i;
        //        }
        //        CameraList_cb.Items.Add(file);
        //        i++;
        //    }
        //    Dispatcher.Invoke(() =>
        //    {//this refer to form in WPF application 
        //        CameraList_cb.SelectedIndex = selectedIndex;
        //    });

        //}

        /// <summary>
        /// Updates Camera instance of new settings chosen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void CameraList_cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    Task task = new Task(() => cameraControl.loadCameraSettings());
        //    task.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
        //    task.Start();
        //}
        #endregion
         
        #region Save Folder Location 
        /// <summary>
        /// Raises event that save folder button is clicked and will set the save path for photos to the folder selected by user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            string folderResult = GetFolderResult();

            if(folderResult != null) //if user chose something
            {
                CameraConst.SaveFolderPath = folderResult;

                SaveFolderPath.Text = folderResult; //set text to folder path
            }
        }

        /// <summary>
        /// Opens dialog window to search for window to save photos t0
        /// </summary>
        /// <returns>String name of Folder path for saving</returns>
        private string GetFolderResult()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                switch (result)
                {
                    case System.Windows.Forms.DialogResult.OK:
                        return dialog.SelectedPath;
                    case System.Windows.Forms.DialogResult.Cancel:
                    default:
                        return null;
                }
            }
        }
        #endregion
        
        #region GRBL command file
        /// <summary>
        /// Finds GRBL txt command doc
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GRBLCommandFileBtn_Click(object sender, RoutedEventArgs e)
        {
            string fileResult = GetFileResult();

            if (fileResult != null) //if user chose something
            {
                GRBLArdunio_Constants.GRBLFilePath = fileResult;

                GRBLCommandFilePath.Text = fileResult; //set text to folder path
            }
        }

        /// <summary>
        /// Finds file in file explorer browser and returns the filepath
        /// </summary>
        /// <returns>The filepath of selected .txt file</returns>
        private string GetFileResult()
        {
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                switch (result)
                {
                    case System.Windows.Forms.DialogResult.OK:
                        return dialog.FileName;
                    case System.Windows.Forms.DialogResult.Cancel:
                    default:
                        return null;
                }
            }
        }
        #endregion

        #region Light Control
        /// <summary>
        /// Sets lights to white
        /// </summary>
        private void setLightWhite()
        {
            litArdunio.SetBacklightColorWhite();
        }

        /// <summary>
        /// Raising event for when lights on button clicked. Should turn on lights
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LightsOnBtn_Click(object sender, RoutedEventArgs e)
        {
            ButtonBackLightOn();
            log.Info("Lights Turned On");
        }
        /// <summary>
        /// Raising event that lights off button clicked. Turns lights off
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LightsOffBtn_Click(object sender, RoutedEventArgs e)
        {
            ButtonBackLightOff();
            log.Info("Lights Turned Off");
        } 
        
        /// <summary>
        /// Turns on lighting for machine
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBackLightOn()
        {
            Task task = new Task(() => litArdunio.SetLight(Peripheral.Backlight, true));
            task.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
            task.Start();
        }

        /// <summary>
        /// Turns off lighting for machine
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBackLightOff()
        {
            Task task = new Task(() => litArdunio.SetLight(Peripheral.Backlight, false));
            task.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
            task.Start();
        }
        #endregion
        
        #region Cycle Control
        /// <summary>
        /// Event for start button of MachineConnectionWindow, takes in a hard coded file
        /// and reads the commands line by line and sends to gArdunio property
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartManualCycleBtn_Click(object sender, RoutedEventArgs e)
        {
            SingleCycle();
        }

        /// <summary>
        /// Single Cycle of machine
        /// </summary>
        public void SingleCycle()
        {
            cameraControl.ImageAcquiredEvent += CameraControl_ImageAcquiredEvent;

            log.Info("Starting a Manual Cycle");

            string filePath = GRBLArdunio_Constants.GRBLFilePath; //for second workstation testing

            log.Debug("Using the file: " + filePath);

            List<string> lines = File.ReadAllLines(filePath).ToList(); //putting all the lines in a list
            bool firstHome = true; //first time homing in cycle

            ButtonBackLightOn();
            setLightWhite();

            log.Debug("Backlights set to white");

            if(lines.Count == 0 || lines[0] != "$H")
            {
                MessageBox.Show("Please check that correct GRBLCommand file is selected.");
                return; // exit function
            }

            foreach (string line in lines)
            {
                gArdunio.SendLine(line); //sending line to ardunio
                log.Info("G Command Sent: " + line);

                if(line == "$H" && firstHome)
                {
                    System.Threading.Thread.Sleep(40000); //40 secs t0 home and not miss positions
                    firstHome = false; 
                }

                if(line.Contains('X'))
                {
                    System.Threading.Thread.Sleep(3000); 
                }
                if(line == "$HY")
                {
                    Thread.Sleep(6500);
                }

                //if line not homing command then take pics
                if(!line.Contains('H'))
                {
                    if(!line.Contains('X')) //if not moving y axis then take pics
                    {
                        cameraControl.CapSaveImage(); //capture image
                       
                      //  bi.Freeze(); //freezes image to avoid need for copying to display and put in other threads
                        //may need to raise event to work but idk
                   }
                }

                System.Threading.Thread.Sleep(1000); //sleep for 1 seconds
            }

            //turn lights off
            ButtonBackLightOff();
        }

        /// <summary>
        /// Camera Control registers when the Image Acquired Event is raised
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CameraControl_ImageAcquiredEvent(object sender, EventArgs e)
        {
            log.Debug("Photo taken by program");
        }

        /// <summary>
        /// Stops manual cycle when started
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopManualCycleBtn_Click(object sender, RoutedEventArgs e)
        {
            gArdunio.SoftReset();
        }
        #endregion

        #region Timelapse

        /// <summary>
        /// Starts timelapse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartTimelapseCycleBtn_Click(object sender, RoutedEventArgs e)
        {
            log.Info("Starting Timelapse");

            View_Consts.runningTL = true;

            Start(); //starting of timelapse
        }

        /// <summary>
        /// Stops timelapse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopTimelapseCycleBtn_Click(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        /// <summary>
        /// Value for combo box of timelapse things
        /// </summary>
        private static readonly KeyValuePair<long, string>[] intervalList = {
              //new KeyValuePair<long, string>(1000, "seconds(s)"),
              new KeyValuePair<long, string>(60000, "minute(s)"),
              new KeyValuePair<long, string>(3600000, "hour(s)"),
              new KeyValuePair<long, string>(86400000, "day(s)"),
              new KeyValuePair<long, string>(604800000, "week(s)")
          };

        /// <summary>
        /// Accessor for interval list
        /// </summary>
        public KeyValuePair<long, string>[] IntervalList
        {
            get
            {
                return intervalList;
            }
        }

        public long TlEndIntervalType
        {
            get
            {
                return View_Consts.tlEndIntervalType;
            }
            set
            {
                View_Consts.tlEndIntervalType = value;
            }
        }

        /// <summary>
        /// accesor for tl interval of view consts
        /// </summary>
        public int TlInterval
        {
            get
            {
                return View_Consts.tlInterval;
            }
            set
            {
                View_Consts.tlInterval = value;
            }
        }

        public int TlEndInterval
        {
            get
            {
                return View_Consts.tlEndInterval;
            }
            set
            {
                View_Consts.tlEndInterval = value;
            }
        }

        public long TlIntervalType
        {
            get
            {
                return View_Consts.tlIntervalType;
            }
            set
            {
                View_Consts.tlIntervalType = value;
            }
        }

        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private CancellationTokenSource tokenSource;

        public bool runningTimeLapse = false;
        public bool runningSingleCycle = false;

        public string tlEnd;
        public string tlCount;
        public double totalMinutes;

        public delegate void TimeLapseUpdate();
        public event EventHandler TimeLapseStatus;

        public delegate void ExperimentUpdate();
        //  public event EventHandler ExperimentStatus;

        public void Start()
        {
            _log.Info("Timelapse Starting");

            runningTimeLapse = true;
            TimeSpan timeLapseInterval = TimeSpan.FromMilliseconds(View_Consts.tlInterval * View_Consts.tlIntervalType);
            _log.Debug(View_Consts.tlInterval * View_Consts.tlIntervalType);
            _log.Debug(timeLapseInterval.Seconds);

            View_Consts.tlStartDate = DateTime.Now;
            TimelapseCountTextBox.Text = View_Consts.tlStartDate.ToString();

            double endTime = View_Consts.tlEndInterval * View_Consts.tlEndIntervalType;

            DateTime endDate = View_Consts.tlStartDate.AddMilliseconds(endTime);
            tlEnd = endDate.ToString();

            TimelapseEndTimeTextBox.Text = tlEnd;

            if(endTime <= timeLapseInterval.TotalMilliseconds)
            {
                MessageBox.Show("Timelapse interval is larger than ending timelapse time, did you make a mistake?");
                return; //return to exit start method
            }

            tlCount = View_Consts.tlStartDate.ToString();
            //  TimeLapseStatus.Raise(this, new EventArgs());
            HandleTimelapseCalculations(timeLapseInterval, endTime);

        }
        
        async Task WaitForStartNow()
        {
            await Task.Delay(5000);
        }

        async Task RunSingleTimeLapse(TimeSpan duration, CancellationToken token)
        {
            _log.Debug("Awaiting timelapse");
            while (duration.TotalSeconds > 0)
            {
                totalMinutes = duration.TotalMinutes;
                tlCount = duration.TotalMinutes.ToString() + " minute(s)";
                TimelapseCountTextBox.Text = tlCount;
               // TimeLapseStatus.Raise(this, new EventArgs());
                /* if (!cycle.runningCycle)
                  {
                      if (!litArdunio.IsNightTime() && !growLightsOn)
                      {
                          litArdunio.SetLight(litArdunio.GrowLight, true, true);
                          growLightsOn = true;
                      }
                      else if (litArdunio.IsNightTime() && growLightsOn)
                      {
                          litArdunio.SetLight(litArdunio.GrowLight, false, false);
                          growLightsOn = false;
                      }
                  }*/
                await Task.Delay(60 * 1000, token);
                duration = duration.Subtract(TimeSpan.FromMinutes(1));
            }

        }
        //public void CycleStatusUpdated(object sender, EventArgs e)
        //{
        //    if (!cycle.runningCycle && runningSingleCycle)
        //    {
        //        runningSingleCycle = false;
        //        tempExperiment.SaveExperimentToSettings();
        //        ExperimentStatus.Raise(this, new EventArgs());
        //    }
        //}

        public async void HandleTimelapseCalculations(TimeSpan timeLapseInterval, Double endDuration)
        {

            if (((View_Consts.startNow || View_Consts.tlStartDate <= DateTime.Now))
             && endDuration > 0)
            {
                _log.Info("Running single timelapse cycle");
                tokenSource = new CancellationTokenSource();

                //   tempExperiment = new Experiment();
                //   tempExperiment.LoadExperiment();


                //   Experiment experiment = Experiment.LoadExperimentAndSave(Properties.Settings.Default.tlExperimentPath);
                //experiment.SaveExperimentToSettings();
                //   ExperimentStatus.Raise(this, new EventArgs());
                // litArdunio.SetLight(litArdunio.Backlight, true);
                //Thread.Sleep(300);
                runningSingleCycle = true;
                _log.Debug("TimeLapse Single Cycle Executed at: " + DateTime.Now);
                //single cycle here

                
                
                SingleCycle();

                try
                {
                    await RunSingleTimeLapse(timeLapseInterval, tokenSource.Token);
                }
                catch (TaskCanceledException e)
                {
                    _log.Error("TimeLapse Cancelled: " + e);
                    //runningTimeLapse = false;
                    Stop();
                    //TimeLapseStatus.Raise(this, new EventArgs());
                    return;
                }
                catch (Exception e)
                {
                    _log.Error("Unknown timelapse error: " + e);
                }

                

                HandleTimelapseCalculations(timeLapseInterval, endDuration - timeLapseInterval.TotalMilliseconds);
            }
            else if (View_Consts.tlStartDate > DateTime.Now)
            {
                await WaitForStartNow();
                HandleTimelapseCalculations(timeLapseInterval, endDuration);
            }
            else
            {
                _log.Info("TimeLapse Finished");
                runningTimeLapse = false;
                //  TimeLapseStatus.Raise(this, new EventArgs());
                return;
            }

        }
        public void Stop()
        {

            // cycle.Stop();
            if (tokenSource != null)
            {
                tokenSource.Cancel();
            }
            runningTimeLapse = false;
            log.Debug("Timelapse stopped manually");

        }
        #endregion

        private void CameraSettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            string fileResult = GetFileResult();


            if (fileResult != null && fileResult.Contains(".xml")) //if user chose something
            {
                CameraConst.CameraSettingsPath = fileResult;

                CameraSettingsPath.Text = fileResult; //set text to folder path

                cameraControl.loadCameraSettings(); //load settings after finding file
            }
            else
            {
                log.Error("Wrong Camera Settinsg File selected");
                MessageBox.Show("Selected camera setttings file was incompatible with camera, please try again");
            }
        }
    }


}
