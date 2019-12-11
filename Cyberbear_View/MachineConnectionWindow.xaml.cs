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
using System.ComponentModel;

namespace Cyberbear_View
{
    /// <summary>
    /// Interaction logic for MachineConnectionWindow.xaml
    /// </summary>
    public partial class MachineConnectionWindow : Window
    {
        //logger
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Machine machine = new Machine();

        BackgroundWorker cycleWorker = new BackgroundWorker();
        BackgroundWorker timelapseControl = new BackgroundWorker();

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

               // BackgroundWorker cycleWorker = new BackgroundWorker();
                cycleWorker.WorkerReportsProgress = true;
                cycleWorker.DoWork += CycleWorker_DoWork; //does work
                cycleWorker.ProgressChanged += CycleWorker_ProgressChanged; //changes progress
                cycleWorker.RunWorkerCompleted += CycleWorker_RunWorkerCompleted; //wen cycle finished
                cycleWorker.WorkerSupportsCancellation = true;

               
                timelapseControl.DoWork += TimelapseControl_DoWork;
                timelapseControl.WorkerReportsProgress = true;
                timelapseControl.ProgressChanged += TimelapseControl_ProgressChanged;
                timelapseControl.RunWorkerCompleted += TimelapseControl_RunWorkerCompleted;
                timelapseControl.WorkerSupportsCancellation = true;
            }
            
            Task task = new Task(() => machine.CameraControl.StartVimba());
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
                ResetArdunioBtn.IsEnabled = true;
                HomeArdunioBtn.IsEnabled = true;

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
                machine.Connect(); //machine object

                setLightWhite(); //maybe need to remove
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
            machine.Disconnect();

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
            //int selectedIndex = 0;

            foreach (string port in ports)
            {
                //if (string.Equals(cb.Name, "LightsSerialComboBox"))
                //{
                //    selectedIndex = i;
                //}
                //if (string.Equals(cb.Name, "GrblSerialComboBox"))
                //{
                //    selectedIndex = i;
                //}

                cb.Items.Add(port);
                i++;
            }

            
                        

            log.Debug("Serial Ports Updated for Combo Box");

         
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

        private void GrblSerialComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectPort = GrblSerialComboBox.SelectedItem;
            string selectPortString = selectPort.ToString();//Todo fix
            UpdateSerialConst(selectPortString, GrblSerialComboBox);

            Connect_Btn_CanEnable();
        }

        private void LightsSerialComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectPort = LightsSerialComboBox.SelectedItem;
            string selectPortString = selectPort.ToString();
            UpdateSerialConst(selectPortString, LightsSerialComboBox);

            Connect_Btn_CanEnable();
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
                machine.CameraSaveFolderPathChange(folderResult);

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
                machine.GRBLCommandFileChange(fileResult);

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
            machine.setLightWhiteMachine();
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
            machine.LightOn();
        }

        /// <summary>
        /// Turns off lighting for machine
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBackLightOff()
        {
            machine.LightOff();
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
            //may move to initializing window
            //BackgroundWorker cycleWorker = new BackgroundWorker();
            //cycleWorker.WorkerReportsProgress = true;
            //cycleWorker.DoWork += CycleWorker_DoWork; //does work
            //cycleWorker.ProgressChanged += CycleWorker_ProgressChanged; //changes progress
            //cycleWorker.RunWorkerCompleted += CycleWorker_RunWorkerCompleted; //wen cycle finished
            cycleWorker.RunWorkerAsync();

        }

        private void CycleWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //TODO
            //reenable certian buttons and some other shiz
            log.Info("Cycle Completed");
        }

        private void CycleWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
         //   CycleProgress.Value = e.ProgressPercentage;
        }

        private void CycleWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            SingleCycle();
        }

        /// <summary>
        /// Resets Ardunios with crtl x command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetArdunioBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                machine.SoftReset();
            }
            catch
            {
                //need to implement exception but may not need one
            }
        }

        /// <summary>
        /// Single Cycle of machine
        /// </summary>
        public void SingleCycle()
        {
            machine.SingleCycle();
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
            stopTimelapse();
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

            startTimelapse();
          //  timelapseControl.RunWorkerAsync();
        }

        private void TimelapseControl_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
         //   log.Info("Timelapse completed");
         //   View_Consts.runningTL = false;

          //  TimelapseCountTextBox.Clear();
          //  TimelapseEndTimeTextBox.Clear();
        }

        /// <summary>
        /// starts timelapse function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimelapseControl_DoWork(object sender, DoWorkEventArgs e)
        {
            log.Info("Timelapse Starting");

            runningTimeLapse = true;
            TimeSpan timeLapseInterval = TimeSpan.FromMilliseconds(machine.TimelapseConst.TlInterval * machine.TimelapseConst.TlIntervalType);
            log.Debug(machine.TimelapseConst.TlInterval * machine.TimelapseConst.TlIntervalType);
            log.Debug(timeLapseInterval.Seconds);

            machine.TimelapseConst.TlStartDate = DateTime.Now;

            Dispatcher.Invoke(() =>
            {
                TimelapseCountTextBox.Text = machine.TimelapseConst.TlStartDate.ToString(); //show start time of timelapse in textbox
            });

            double endTime = machine.TimelapseConst.TlEndInterval * machine.TimelapseConst.TlEndIntervalType;

            //finding end date of timelapse
            DateTime endDate = machine.TimelapseConst.TlStartDate.AddMilliseconds(endTime);
            tlEnd = endDate.ToString();

            Dispatcher.Invoke(() =>
            {
                TimelapseEndTimeTextBox.Text = tlEnd; //show ending time of timelapse in textbox
            });

            //if ending time of timelapse is smaller than the interval between timelapses
            if (endTime <= timeLapseInterval.TotalMilliseconds)
            {
                MessageBox.Show("Timelapse interval is larger than ending timelapse time, did you make a mistake?");
                return; //return to exit start method
            }

            tlCount = machine.TimelapseConst.TlStartDate.ToString(); //how long until next timelapse
            HandleTimelapseCalculations(timeLapseInterval, endTime);

        }

       
        private void TimelapseControl_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            TimelapseCountTextBox.Text = machine.TimelapseStartDate(); //TODO Fix thuis bug and figure out how to report
            TimelapseEndTimeTextBox.Text = machine.TimelapseEndDate();
        }

        /// <summary>
        /// Stops timelapse by canceling async thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopTimelapseCycleBtn_Click(object sender, RoutedEventArgs e)
        {
            if(View_Consts.runningTL == true)
            {
                stopTimelapse();
                //timelapseControl.CancelAsync();
            }
            else
            {
                MessageBox.Show("No timelapse is running at the moment");
            }
        }

        private CancellationTokenSource tokenSource;


        public bool runningTimeLapse = false;
        public bool runningSingleCycle = false;

        public string tlEnd;
        public string tlCount;
        public double totalMinutes;

        public void startTimelapse()
        {
            log.Info("Timelapse Starting");

            runningTimeLapse = true;
            TimeSpan timeLapseInterval = TimeSpan.FromMilliseconds(machine.TimelapseConst.TlInterval * machine.TimelapseConst.TlIntervalType);
            log.Debug(machine.TimelapseConst.TlInterval * machine.TimelapseConst.TlIntervalType);
            log.Debug(timeLapseInterval.Seconds);

            machine.TimelapseConst.TlStartDate = DateTime.Now;

            Dispatcher.Invoke(() =>
            {
                TimelapseCountTextBox.Text = machine.TimelapseConst.TlStartDate.ToString();
            });
           // TimelapseCountTextBox.Text = machine.TimelapseConst.TlStartDate.ToString();

            double endTime = machine.TimelapseConst.TlEndInterval * machine.TimelapseConst.TlEndIntervalType;

            DateTime endDate = machine.TimelapseConst.TlStartDate.AddMilliseconds(endTime);
            tlEnd = endDate.ToString();

            Dispatcher.Invoke(() =>
            {
                TimelapseEndTimeTextBox.Text = tlEnd;
            });
           

            if (endTime <= timeLapseInterval.TotalMilliseconds)
            {
                MessageBox.Show("Timelapse interval is larger than ending timelapse time, did you make a mistake?");
                return; //return to exit start method
            }

            tlCount = machine.TimelapseConst.TlStartDate.ToString();
            HandleTimelapseCalculations(timeLapseInterval, endTime);

        }

        async Task WaitForStartNow()
        {
            await Task.Delay(5000);
        }

        async Task RunSingleTimeLapse(TimeSpan duration, CancellationToken token)
        {
            log.Debug("Awaiting timelapse");
            while (duration.TotalSeconds > 0)
            {
                totalMinutes = duration.TotalMinutes;
                tlCount = duration.TotalMinutes.ToString() + " minute(s)";

                TimelapseCountTextBox.Dispatcher.Invoke(() =>
                {
                    TimelapseCountTextBox.Text = tlCount;
                });
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

        public async void HandleTimelapseCalculations(TimeSpan timeLapseInterval, Double endDuration)
        {

            if (((machine.TimelapseConst.StartNow || machine.TimelapseConst.TlStartDate <= DateTime.Now))
             && endDuration > 0)
            {
                log.Info("Running single timelapse cycle");
                tokenSource = new CancellationTokenSource();
                runningSingleCycle = true;
                log.Debug("TimeLapse Single Cycle Executed at: " + DateTime.Now);

                //may need await, we will see
                Task.Factory.StartNew(
                    () =>
                     {
                         SingleCycle();
                     });
                

                try
                {
                    await RunSingleTimeLapse(timeLapseInterval, tokenSource.Token);
                }
                catch (TaskCanceledException e)
                {
                    log.Error("TimeLapse Cancelled: " + e);
                    //runningTimeLapse = false;
                    stopTimelapse();
                    return;
                }
                catch (Exception e)
                {
                    log.Error("Unknown timelapse error: " + e);
                }



                HandleTimelapseCalculations(timeLapseInterval, endDuration - timeLapseInterval.TotalMilliseconds);
            }
            else if (machine.TimelapseConst.TlStartDate > DateTime.Now)
            {
                await WaitForStartNow();
                HandleTimelapseCalculations(timeLapseInterval, endDuration);
            }
            else
            {
                log.Info("TimeLapse Finished");
                runningTimeLapse = false;
                Dispatcher.Invoke(() =>
                {
                    TimelapseCountTextBox.Clear();
                    TimelapseEndTimeTextBox.Clear();
                });
                return;
            }

        }
        public void stopTimelapse()
        {

            // cycle.Stop();
            if (tokenSource != null)
            {
                tokenSource.Cancel();
            }
            Dispatcher.Invoke(() =>
            {
                TimelapseCountTextBox.Clear();
                TimelapseEndTimeTextBox.Clear();
            });
            
            runningTimeLapse = false;
            log.Debug("Timelapse stopped manually");

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
                return machine.TimelapseConst.TlEndIntervalType;
            }
            set
            {
                machine.TimelapseConst.TlEndIntervalType = value;
            }
        }

        /// <summary>
        /// accesor for tl interval of view consts
        /// </summary>
        public int TlInterval
        {
            get
            {
                return machine.TimelapseConst.TlInterval;
            }
            set
            {
                machine.TimelapseConst.TlInterval = value;
            }
        }

        public int TlEndInterval
        {
            get
            {
                return machine.TimelapseConst.TlEndInterval;
            }
            set
            {
                machine.TimelapseConst.TlEndInterval = value;
            }
        }

        public long TlIntervalType
        {
            get
            {
                return machine.TimelapseConst.TlIntervalType;
            }
            set
            {
                machine.TimelapseConst.TlIntervalType = value;
            }
        }

       
        #endregion

        private void CameraSettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            string fileResult = GetFileResult();


            if (fileResult != null && fileResult.Contains(".xml")) //if user chose something
            {
                machine.CameraSettingsPathChange(fileResult);

                CameraSettingsPath.Text = fileResult; //set text to folder path

                machine.loadCameraSettingsMachine();
            }
            else
            {
                log.Error("Wrong Camera Settinsg File selected");
                MessageBox.Show("Selected camera setttings file was incompatible with camera, please try again");
            }
        }

        /// <summary>
        /// For when combo box for list of cameras opened
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CameraSelectionCb_DropDownOpened(object sender, EventArgs e)
        {
            machine.CameraControl.UpdateCameraList();

            List<string> cameraNames = machine.CameraControl.GetCameraListNames();

            CameraSelectionCb.Items.Clear();

            foreach(string name in cameraNames)
            {
                CameraSelectionCb.Items.Add(name);
            }
            

        }

        /// <summary>
        /// when combo box for camera selection is changed, will set machine camera control to that
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CameraSelectionCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string name = (string)CameraSelectionCb.SelectedItem;

            machine.CameraControl.selectCameraName(name);

        }

        /// <summary>
        /// Turn on glowlight
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GrowlightOnBtn_Click(object sender, RoutedEventArgs e)
        {
            machine.GrowLightOn();
        }

        /// <summary>
        /// turn off glowlight
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GrowlightsOffBtn_Click(object sender, RoutedEventArgs e)
        {
            machine.GrowLightOff();
        }

        /// <summary>
        /// Homes machine
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HomeArdunioBtn_Click(object sender, RoutedEventArgs e)
        {
            machine.GrblArdunio.HomeMachine();
        }
    }


}
