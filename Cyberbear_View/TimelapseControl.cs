using Cyberbear_Events.Util;
using Cyberbear_View.Consts;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cyberbear_View
{
    class TimelapseControl
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private CancellationTokenSource tokenSource;
       // PeripheralControl peripheral = PeripheralControl.Instance;
       // CycleControl cycle = CycleControl.Instance;
       // Machine machine = Machine.Instance;

        public bool runningTimeLapse = false;
        public bool runningSingleCycle = false;
        //bool growLightsOn = false;

        public string tlEnd;
        public string tlCount;
        public double totalMinutes;
     //   private Experiment tempExperiment;
        private bool growLightsOn = false;

        public delegate void TimeLapseUpdate();
        public event EventHandler TimeLapseStatus;

        public delegate void ExperimentUpdate();
        public event EventHandler ExperimentStatus;

        MachineConnectionWindow MCW = new MachineConnectionWindow();

        public TimelapseControl()
        {
            //machine.StatusChanged += Machine_StatusChanged;
        //    cycle.StatusUpdate += CycleStatusUpdated;
        }
        public void Start()
        {
            growLightsOn = false;
            _log.Info("Timelapse Starting");

            runningTimeLapse = true;
            TimeSpan timeLapseInterval = TimeSpan.FromMilliseconds(View_Consts.tlInterval * View_Consts.tlEndIntervalType);
            _log.Debug(View_Consts.tlInterval * View_Consts.tlEndIntervalType);
            _log.Debug(timeLapseInterval.Seconds);

            View_Consts.tlStartDate = DateTime.Now;

            double endTime = View_Consts.tlEndInterval * View_Consts.tlEndIntervalType;

            DateTime endDate = View_Consts.tlStartDate.AddMilliseconds(endTime);
            tlEnd = endDate.ToString();

            tlCount = View_Consts.tlStartDate.ToString();
            TimeLapseStatus.Raise(this, new EventArgs());
            HandleTimelapseCalculations(timeLapseInterval, endTime);
            //timeLapseCount.Text = "Not Running";


        }
        //private void Machine_StatusChanged()
        //{

        //    if (machine.Status == "Alarm")
        //    {
        //        //("Timelapse canclled because hard limit encountered");
        //        _log.Error("Machine in Alarm State. Cancelling Timelapse");
        //        Stop();
        //    }

        //}
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
                TimeLapseStatus.Raise(this, new EventArgs());
               /* if (!cycle.runningCycle)
                {
                    if (!peripheral.IsNightTime() && !growLightsOn)
                    {
                        peripheral.SetLight(Peripheral.GrowLight, true, true);
                        growLightsOn = true;
                    }
                    else if (peripheral.IsNightTime() && growLightsOn)
                    {
                        peripheral.SetLight(Peripheral.GrowLight, false, false);
                        growLightsOn = false;
                    }
                }*/
                //_log.Debug("Waiting 1 Minute");
                await Task.Delay(60 * 1000, token);
                //_log.Debug("1 minute elapsed");
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
                //peripheral.SetLight(Peripheral.Backlight, true);
                //Thread.Sleep(300);
                runningSingleCycle = true;
                _log.Debug("TimeLapse Single Cycle Executed at: " + DateTime.Now);
                //single cycle here
                MCW.SingleCycle();

                try
                {
                    await RunSingleTimeLapse(timeLapseInterval, tokenSource.Token);
                }
                catch (TaskCanceledException e)
                {
                    _log.Error("TimeLapse Cancelled: " + e);
                    //runningTimeLapse = false;
                    Stop();
                    TimeLapseStatus.Raise(this, new EventArgs());
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
                TimeLapseStatus.Raise(this, new EventArgs());
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
            growLightsOn = true;
            runningTimeLapse = false;
            TimeLapseStatus.Raise(this, new EventArgs());

        }
    }
}
