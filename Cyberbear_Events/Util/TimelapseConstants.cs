using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyberbear_View.Consts
{
    //need to update for machine object and to have multiple instances
    /// <summary>
    /// consts for timelapse
    /// </summary>
    public class TimelapseConst
    {
        private bool runningTL;
        private string tlEnd;
        private int tlInterval = 0;
        private int tlEndInterval = 0;
        private long tlIntervalType = 60000;
        private long tlEndIntervalType = 60000;
        private DateTime tlStartDate;
        private bool startNow = true;

        public bool RunningTL { get => runningTL; set => runningTL = value; }
        public string TlEnd { get => tlEnd; set => tlEnd = value; }
        public int TlInterval { get => tlInterval; set => tlInterval = value; }
        public int TlEndInterval { get => tlEndInterval; set => tlEndInterval = value; }
        public long TlIntervalType { get => tlIntervalType; set => tlIntervalType = value; }
        public long TlEndIntervalType { get => tlEndIntervalType; set => tlEndIntervalType = value; }
        public DateTime TlStartDate { get => tlStartDate; set => tlStartDate = value; }
        public bool StartNow { get => startNow; set => startNow = value; }

        /// <summary>
        /// constructor
        /// </summary>
        public TimelapseConst()
        {
            TlInterval = 0;
            TlEndInterval = 0;
            TlIntervalType = 60000;
            TlEndIntervalType = 60000;
            StartNow = true;
        }
    }
}
