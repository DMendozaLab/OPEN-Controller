using log4net;
using Cyberbear_Events.MachineControl.CameraControl;
using Cyberbear_Events.MachineControl.LightingControl;
using Cyberbear_Events.MachineControl.GrblArdunio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static Cyberbear_Events.MachineControl.LightingControl.LightsArdunio;
using System.Threading;
using Cyberbear_View.Consts;

namespace Cyberbear_Events.MachineControl
{
    /// <summary>
    /// Machine will be the class that contains everything needed for the
    /// machine to run and be connected
    /// </summary>
    public class Machine
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private GRBLArdunio grblArdunio;
        private LightsArdunio litArdunio;
        private Camera cameraControl;
        private TimelapseConst timelapseConst;
        private GRBLArdunio_Constants grblArdunio_Constants;
        private string name; //name of machine, may be user added or by front end automatically, or window
        private int numPlants;
        private CameraConst cameraConst;
        private bool growlightsOn;
        private bool dayNightCycleEnable = false; //day night cycle is not enabled unless checked box checked

        public GRBLArdunio GrblArdunio { get => grblArdunio; set => grblArdunio = value; }
        public LightsArdunio LitArdunio { get => litArdunio; set => litArdunio = value; }
        public Camera CameraControl { get => cameraControl; set => cameraControl = value; }
        public string Name { get => name; set => name = value; }
        public TimelapseConst TimelapseConst { get => timelapseConst; set => timelapseConst = value; }
        public int NumPlants { get => numPlants; set => numPlants = value; }
        public GRBLArdunio_Constants GrblArdunio_Constants { get => grblArdunio_Constants; set => grblArdunio_Constants = value; }
        public CameraConst CameraConst { get => cameraConst; set => cameraConst = value; }
        public bool GrowlightsOn { get => growlightsOn; set => growlightsOn = value; }
        public bool DayNightCycleEnable { get => dayNightCycleEnable; set => dayNightCycleEnable = value; }



        //constructor
        /// <summary>
        /// Will initalize a new machine object and subsaquent other things
        /// </summary>
        public Machine()
        {
            log.Info("New machine start named: " + name);

            GrblArdunio = new GRBLArdunio();
            log.Info("New Grbl Ardunio added");
            litArdunio = new LightsArdunio();
            log.Info("New Lights Ardunuio added");
            cameraControl = new Camera();
            log.Info("New Camera Control added");

            TimelapseConst = new TimelapseConst(); //for timelapses
            GrblArdunio_Constants = new GRBLArdunio_Constants();
            cameraConst = new CameraConst();
        }

        /// <summary>
        /// Connects Machine object by connecting GRBL ardunio, lights ardunio, and camera control. Returns boolean for success or failure
        /// </summary>
        /// <returns>Returns 1 if success, 0 if connection failed</returns>
        public void Connect()
        {
            try
            {
                //testing machine object
                if (GrblArdunio.Connected == false)
                {
                    GrblArdunio.Connect();
                    log.Info("GRBL Ardunio Connected");
                }
                if (litArdunio.Connected == false)
                {
                    litArdunio.Connect();
                    log.Info("Lights Ardunio Connected");

                    //TODO set lights to white when turned on
                    //setLightWhite();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        /// <summary>
        /// Disconnects Machine object from window when called
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (GrblArdunio.Connected == true)
                {
                    GrblArdunio.Disconnect();
                    log.Info("Grbl Ardunio Disconnected");
                }
                else if (litArdunio.Connected == true)
                {
                    LightOff(); //turn lights off before disconnecting

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
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }


        /// <summary>
        /// Single Cycle of machine
        /// </summary>
        public void SingleCycle()
        {
            cameraControl.ImageAcquiredEvent += CameraControl_ImageAcquiredEvent;

            log.Info("Starting a Manual Cycle");

            string filePath = GrblArdunio_Constants.GRBLFilePath; //for second workstation testing

            log.Debug("Using the file: " + filePath);

            List<string> lines = File.ReadAllLines(filePath).ToList(); //putting all the lines in a list
            bool firstHome = true; //first time homing in cycle

            
            LightOn();
            setLightWhiteMachine();

            log.Debug("Backlights set to white");

            if (lines.Count == 0 || lines[0] != "$H")
            {
                MessageBox.Show("Please check that correct GRBLCommand file is selected.");
                return; // exit function
            }

            foreach (string line in lines)
            {
                //TODO something about the timing
                GrblArdunio.SendLine(line); //sending line to ardunio
                log.Info("G Command Sent: " + line);

                if (line == "$H" && firstHome)
                {
                    System.Threading.Thread.Sleep(6000); //6 secs t0 home and not miss positions
                    firstHome = false;
                }

                if (line.Contains('X'))
                {
                   System.Threading.Thread.Sleep(4000);
                }
                if (line == "$HY")
                {
                    Thread.Sleep(12000);
                }

                //if line not homing command then take pics
                if (!line.Contains('H'))
                {
                    if (!line.Contains('X')) //if not moving y axis then take pics
                    {
                        Thread.Sleep(1000); //presleep for capture to adjust

                        cameraControl.CapSaveImage(); //capture image

                        Thread.Sleep(1500); //sleep for 1 seconds

                    }
                }

                
            }

            //turn lights off (and growlights if night)
            LightOff();

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

        public void loadCameraSettingsMachine()
        {
            cameraControl.loadCameraSettings();
        }

        #region Timelapse
        //private CancellationTokenSource tokenSource;

        //public delegate void SingleCycleCompleted();

        //public bool runningTimeLapse = false;
        //public bool runningSingleCycle = false;

        //public string tlEnd;
        //public string tlCount;
        //public double totalMinutes;

        //public void startTimelapse()
        //{
        //    log.Info("Timelapse Starting");

        //    runningTimeLapse = true;
        //    TimeSpan timeLapseInterval = TimeSpan.FromMilliseconds(TimelapseConst.TlInterval * TimelapseConst.TlIntervalType);
        //    log.Debug(TimelapseConst.TlInterval * TimelapseConst.TlIntervalType);
        //    log.Debug(timeLapseInterval.Seconds);

        //    TimelapseConst.TlStartDate = DateTime.Now;

        //    double endTime = TimelapseConst.TlEndInterval * TimelapseConst.TlEndIntervalType;

        //    DateTime endDate = TimelapseConst.TlStartDate.AddMilliseconds(endTime);
        //    tlEnd = endDate.ToString();

        //    if (endTime <= timeLapseInterval.TotalMilliseconds)
        //    {
        //        MessageBox.Show("Timelapse interval is larger than ending timelapse time, did you make a mistake?");
        //        return; //return to exit start method
        //    }

        //    tlCount = TimelapseConst.TlStartDate.ToString();
        //    //  TimeLapseStatus.Raise(this, new EventArgs());
        //    HandleTimelapseCalculations(timeLapseInterval, endTime);

        //}

        //async Task WaitForStartNow()
        //{
        //    await Task.Delay(5000);
        //}

        //async Task RunSingleTimeLapse(TimeSpan duration, CancellationToken token)
        //{
        //    log.Debug("Awaiting timelapse");
        //    while (duration.TotalSeconds > 0)
        //    {
        //        totalMinutes = duration.TotalMinutes;
        //        tlCount = duration.TotalMinutes.ToString() + " minute(s)";
        //        // TimeLapseStatus.Raise(this, new EventArgs());
        //        /* if (!cycle.runningCycle)
        //          {
        //              if (!litArdunio.IsNightTime() && !growLightsOn)
        //              {
        //                  litArdunio.SetLight(litArdunio.GrowLight, true, true);
        //                  growLightsOn = true;
        //              }
        //              else if (litArdunio.IsNightTime() && growLightsOn)
        //              {
        //                  litArdunio.SetLight(litArdunio.GrowLight, false, false);
        //                  growLightsOn = false;
        //              }
        //          }*/
        //        await Task.Delay(60 * 1000, token);
        //        duration = duration.Subtract(TimeSpan.FromMinutes(1));
        //    }

        //}

        //public async void HandleTimelapseCalculations(TimeSpan timeLapseInterval, Double endDuration)
        //{

        //    if (((TimelapseConst.StartNow || TimelapseConst.TlStartDate <= DateTime.Now))
        //     && endDuration > 0)
        //    {
        //        log.Info("Running single timelapse cycle");
        //        tokenSource = new CancellationTokenSource();
        //        runningSingleCycle = true;
        //        log.Debug("TimeLapse Single Cycle Executed at: " + DateTime.Now);
        //        //single cycle here

                

        //        SingleCycle();

        //        try
        //        {
        //            await RunSingleTimeLapse(timeLapseInterval, tokenSource.Token);
        //        }
        //        catch (TaskCanceledException e)
        //        {
        //            log.Error("TimeLapse Cancelled: " + e);
        //            //runningTimeLapse = false;
        //            stopTimelapse();
        //            //TimeLapseStatus.Raise(this, new EventArgs());
        //            return;
        //        }
        //        catch (Exception e)
        //        {
        //            log.Error("Unknown timelapse error: " + e);
        //        }



        //        HandleTimelapseCalculations(timeLapseInterval, endDuration - timeLapseInterval.TotalMilliseconds);
        //    }
        //    else if (TimelapseConst.TlStartDate > DateTime.Now)
        //    {
        //        await WaitForStartNow();
        //        HandleTimelapseCalculations(timeLapseInterval, endDuration);
        //    }
        //    else
        //    {
        //        log.Info("TimeLapse Finished");
        //        runningTimeLapse = false;
        //        //  TimeLapseStatus.Raise(this, new EventArgs());
        //        return;
        //    }

        //}
        //public void stopTimelapse()
        //{

        //    // cycle.Stop();
        //    if (tokenSource != null)
        //    {
        //        tokenSource.Cancel();
        //    }
        //    runningTimeLapse = false;
        //    log.Debug("Timelapse stopped manually");

        //}

        /// <summary>
        /// returns next timelapse starting time
        /// </summary>
        /// <returns></returns>
        public string UpdateNextTimelapse()
        {
            return TimelapseConst.TlStartDate.ToString();
        }

        //find way to report timelapse timing and shiz 

        //legacy code, don't known if needed or not
        //public void CycleStatusUpdated(object sender, EventArgs e)
        //{
        //    if (!cycle.runningCycle && runningSingleCycle)
        //    {
        //        runningSingleCycle = false;
        //        tempExperiment.SaveExperimentToSettings();
        //        ExperimentStatus.Raise(this, new EventArgs());
        //    }
        //}
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

        /// <summary>
        /// To accesss the machine's timelapse start date and make it into a string for a textbox
        /// </summary>
        /// <returns>A string version of the timelapse starting date</returns>
        public string TimelapseStartDate()
        {
            return TimelapseConst.TlStartDate.ToString();
        }

        /// <summary>
        /// Accesses the machine's end date for the timelapse
        /// </summary>
        /// <returns>Returns string version of ending time for timelapse</returns>
        public string TimelapseEndDate()
        {
            return timelapseConst.TlEnd;
        }
        #endregion

        #region Lighting Control
        /// <summary>
        /// When called, turns Growlights on in machine object
        /// </summary>
        public void GrowLightOn()
        {
            Task task = new Task(() => LitArdunio.SetLight(Peripheral.GrowLight, true));
            task.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
            task.Start();

            growlightsOn = true;
        }
        /// <summary>
        /// When called, turns growlights off
        /// </summary>
        public void GrowLightOff()
        {
            Task task = new Task(() => LitArdunio.SetLight(Peripheral.GrowLight, false));
            task.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
            task.Start();

            growlightsOn = false;
        }

        /// <summary>
        /// Sets lights to white on lights ardunio
        /// </summary>
        public void setLightWhiteMachine()
        {
            Task task = new Task(() => LitArdunio.SetBacklightColorWhite());
            task.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
            task.Start();
        }

        public void LightOn()
        {
            if(litArdunio.LightStatus == false)
            {
                Task task = new Task(() => LitArdunio.SetLight(Peripheral.Backlight, true));
                task.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
                task.Start();

                litArdunio.LightStatus = true; //lights are on

            }
            else
            {
                return;
            }
        }

        public void LightOff()
        {
            if(litArdunio.LightStatus == true)
            {
                Task task = new Task(() => LitArdunio.SetLight(Peripheral.Backlight, false));
                task.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
                task.Start();

                Task task2 = new Task(() => LitArdunio.SetLight(Peripheral.Backlight, false));
                task2.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
                task2.Start();

                litArdunio.LightStatus = false; //lights are off 
                if(dayNightCycleEnable == true)
                {
                    NightTimeLightOff();
                }
               
            }
            else
            {
                return;
            }
            
        }

        /// <summary>
        /// Checks if nightime then turns growlights off
        /// </summary>
        public void NightTimeLightOff()
        {
            if (litArdunio.IsNightTime() == true)
            {
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(500);
                    GrowLightOff();
                });
            }
        }

        /// <summary>
        /// soft reset Grbl Ardunio for machine
        /// </summary>
        public void SoftReset()
        {
            GrblArdunio.SoftReset();
        }
        #endregion

        //add camera const changes
        public void CameraSettingsPathChange(string filepath)
        {
            CameraControl.CameraConst.CameraSettingsPath = filepath;
        }

        public void CameraSaveFolderPathChange(string folderResult)
        {
            CameraControl.CameraConst.SaveFolderPath = folderResult;
        }

        public void GRBLCommandFileChange(string fileResult)
        {
            GrblArdunio_Constants.GRBLFilePath = fileResult;
        }
    }
}
