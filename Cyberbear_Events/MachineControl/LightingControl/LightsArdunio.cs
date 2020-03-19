using log4net;
using System;
using System.IO.Ports;
using System.Reflection;
using System.Windows.Media;
using Cyberbear_Events;

namespace Cyberbear_Events.MachineControl.LightingControl
{
    /// <summary>
    /// Controls the lights of the machine, ripped from old SPIPware, should work the same
    /// </summary>
    public class LightsArdunio
    {
        //logger
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public event EventHandler PeripheralUpdate;

        ConnectionType connectionType;
        public bool Connected { get => connected; set => connected = value; }

        //false means off, true means on
        private bool lightStatus = false;
        public bool LightStatus { get => lightStatus; set => lightStatus = value; }

        SerialPort port;

        public enum Peripheral { Backlight = 6, GrowLight = 1 };

        private bool nightTime = false;
        public bool NightTime { get => nightTime; set => nightTime = value; }

        private bool connected = false;
        private static readonly LightsArdunio instance = new LightsArdunio();

        private DateTime startOfNight;
        private DateTime endOfNight;

        public DateTime StartOfNight { get => startOfNight; set => startOfNight = value; }
        public DateTime EndOfNight { get => endOfNight; set => endOfNight = value; }

        #region Constructors
        static LightsArdunio()
        {

        }
        public static LightsArdunio Instance
        {
            get
            {
                return instance;
            }
        }

      
        #endregion


        public void Connect()
        {
            switch (connectionType)
            {
                case ConnectionType.Serial:
                    _log.Info("Opening Peripheral Serial Port");
                    port = new SerialPort(SerialConsts.defaultComPortLights, SerialConsts.baudRateConst, Parity.None, 8, StopBits.One);
                    port.DataReceived += new SerialDataReceivedEventHandler(Peripheral_DataReceived);
                    port.Open();
                    //setLight(Peripheral.Backlight, true);
                    Connected = true;
                    break;
                default:
                    Connected = false;
                    throw new Exception("Invalid Connection Type");
            }

        }

        /// <summary>
        /// Method for finding an boolean statement to if the system is currently in nighttime
        /// and should have the growlights on or not
        /// </summary>
        /// <returns>Returns a true or false (boolean) statement for if nighttime or not. Will turn on Growlights
        /// on for Sunbear if accurate</returns>
        public bool IsNightTime()
        {
            DateTime now = DateTime.Now;

            //If current time is less than end of night and more than start of night, then it is night time 
            return ((now.TimeOfDay <= EndOfNight.TimeOfDay) & (now.TimeOfDay >= StartOfNight.TimeOfDay)) ? true : false;

        }


        public void SetLight(Peripheral peripheral, bool status, bool daytime)
        {
            if (status && daytime)
            {
                _log.Info(peripheral.ToString() + " lights on");
                SetLight(peripheral, status);
            }
            else
            {
                _log.Info(peripheral.ToString() + " lights off");
                SetLight(peripheral, false);
            }
        }

        public void SetLight(Peripheral peripheral, bool value)
        { 
           // PeripheralUpdate.Raise(this, new PeripheralEventArgs(peripheral, value));
            string cmdStr = "";
            if (peripheral == Peripheral.Backlight)
            {
                cmdStr = "S2P" + peripheral.ToString("D") + "V" + BtoI(value);
            }
            else if (peripheral == Peripheral.GrowLight)
            {
                cmdStr = "S1P" + peripheral.ToString("D") + "V" + BtoI(value);
            }
            SendCommand(cmdStr);

        }
        public void SetBacklightColor(Color color)
        {
            string cmdStr = "S3P0R" + color.R.ToString() + "G" + color.G.ToString() + "B" + color.B.ToString();
            SendCommand(cmdStr);
        }

        public void SetBacklightColorWhite()
        {
            string cmdStr = "S3P0R" + "255" + "G" + "255" + "B" + "255";
            SendCommand(cmdStr);
        }

        public void SetBrightness()
        {
            string cmdStr = "S4P0";
            SendCommand(cmdStr);
        }

        private int BtoI(bool value)
        {
            return (value == true) ? 1 : 0;
        }
        private void SendCommand(string commandString)
        {
            _log.Debug(commandString);
            if (port != null && port.IsOpen)
            {
                port.WriteLine(commandString);
                _log.Debug("Command" + commandString + "was written to lights ardunio port");
            }

        }
        private void Peripheral_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Show all the incoming data in the port's buffer
            _log.Debug("From Peripheral:" + port.ReadLine());
        }
        public void Disconnect()
        {
            try
            {
                port.Close();
            }
            catch(Exception ex)
            {
                _log.Error("Failure to close light arduino port because: " + ex.Message);
            }
        }
    }
}

