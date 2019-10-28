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
        private string name; //name of machine, may be user added or by front end automatically, or window

        public GRBLArdunio GrblArdunio { get => grblArdunio; set => grblArdunio = value; }
        public LightsArdunio LitArdunio { get => litArdunio; set => litArdunio = value; }
        public Camera CameraControl { get => cameraControl; set => cameraControl = value; }
        public string Name { get => name; set => name = value; }

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


        /// <summary>
        /// Sets lights to white on lights ardunio
        /// </summary>
        public void setLightWhiteMachine()
        {
            litArdunio.SetBacklightColorWhite();
        }

        public void LightOn()
        {
            Task task = new Task(() => LitArdunio.SetLight(Peripheral.Backlight, true));    //may chnage task onto object
            task.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
            task.Start();
        }

        public void LightOff()
        {
            Task task = new Task(() => LitArdunio.SetLight(Peripheral.Backlight, false));
            task.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
            task.Start();
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
                GrblArdunio.SendLine(line); //sending line to ardunio
                log.Info("G Command Sent: " + line);

                if (line == "$H" && firstHome)
                {
                    System.Threading.Thread.Sleep(40000); //40 secs t0 home and not miss positions
                    firstHome = false;
                }

                if (line.Contains('X'))
                {
                    System.Threading.Thread.Sleep(3000);
                }
                if (line == "$HY")
                {
                    Thread.Sleep(6500);
                }

                //if line not homing command then take pics
                if (!line.Contains('H'))
                {
                    if (!line.Contains('X')) //if not moving y axis then take pics
                    {
                        cameraControl.CapSaveImage(); //capture image

                        //  bi.Freeze(); //freezes image to avoid need for copying to display and put in other threads
                        //may need to raise event to work but idk
                    }
                }

                System.Threading.Thread.Sleep(1000); //sleep for 1 seconds
            }

            //turn lights off
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
            TimeSpan timeLapseInterval = TimeSpan.FromMilliseconds(TimelapseConst.tlInterval * TimelapseConst.tlIntervalType);
            log.Debug(TimelapseConst.tlInterval * TimelapseConst.tlIntervalType);
            log.Debug(timeLapseInterval.Seconds);

            TimelapseConst.tlStartDate = DateTime.Now;

            double endTime = TimelapseConst.tlEndInterval * TimelapseConst.tlEndIntervalType;

            DateTime endDate = TimelapseConst.tlStartDate.AddMilliseconds(endTime);
            tlEnd = endDate.ToString();

            if (endTime <= timeLapseInterval.TotalMilliseconds)
            {
                MessageBox.Show("Timelapse interval is larger than ending timelapse time, did you make a mistake?");
                return; //return to exit start method
            }

            tlCount = TimelapseConst.tlStartDate.ToString();
            //  TimeLapseStatus.Raise(this, new EventArgs());
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

        public async void HandleTimelapseCalculations(TimeSpan timeLapseInterval, Double endDuration)
        {

            if (((TimelapseConst.startNow || TimelapseConst.tlStartDate <= DateTime.Now))
             && endDuration > 0)
            {
                log.Info("Running single timelapse cycle");
                tokenSource = new CancellationTokenSource();

                //   tempExperiment = new Experiment();
                //   tempExperiment.LoadExperiment();


                //   Experiment experiment = Experiment.LoadExperimentAndSave(Properties.Settings.Default.tlExperimentPath);
                //experiment.SaveExperimentToSettings();
                //   ExperimentStatus.Raise(this, new EventArgs());
                // litArdunio.SetLight(litArdunio.Backlight, true);
                //Thread.Sleep(300);
                runningSingleCycle = true;
                log.Debug("TimeLapse Single Cycle Executed at: " + DateTime.Now);
                //single cycle here



                SingleCycle();

                try
                {
                    await RunSingleTimeLapse(timeLapseInterval, tokenSource.Token);
                }
                catch (TaskCanceledException e)
                {
                    log.Error("TimeLapse Cancelled: " + e);
                    //runningTimeLapse = false;
                    stopTimelapse();
                    //TimeLapseStatus.Raise(this, new EventArgs());
                    return;
                }
                catch (Exception e)
                {
                    log.Error("Unknown timelapse error: " + e);
                }



                HandleTimelapseCalculations(timeLapseInterval, endDuration - timeLapseInterval.TotalMilliseconds);
            }
            else if (TimelapseConst.tlStartDate > DateTime.Now)
            {
                await WaitForStartNow();
                HandleTimelapseCalculations(timeLapseInterval, endDuration);
            }
            else
            {
                log.Info("TimeLapse Finished");
                runningTimeLapse = false;
                //  TimeLapseStatus.Raise(this, new EventArgs());
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
            runningTimeLapse = false;
            log.Debug("Timelapse stopped manually");

        }

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
        #endregion
    }
}
