using System;
using static Cyberbear_Events.MachineControl.LightingControl.LightsArdunio;

namespace Cyberbear_Events.MachineControl.LightingControl
{
    /// <summary>
    /// Event Args for lights event raisers
    /// </summary>
    public class PeripheralEventArgs : EventArgs
    {
        private Peripheral periperal;
        private bool status;

        public PeripheralEventArgs(Peripheral periperal, bool peripheralStatus)
        {
            this.Periperal = periperal;
            this.Status = peripheralStatus;
        }

        public Peripheral Periperal { get => periperal; set => periperal = value; }
        public bool Status { get => status; set => status = value; }
    }
}
