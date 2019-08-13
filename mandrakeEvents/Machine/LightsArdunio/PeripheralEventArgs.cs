using System;
using static MandrakeEvents.Machine.LightsArdunio.LightsArdunio;

namespace MandrakeEvents.Machine.LightsArdunio
{
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
