using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyberbear_View.Consts
{
    /// <summary>
    /// consts for view, temporary
    /// </summary>
    public static class View_Consts
    {
        public static bool runningTL = false;
        public static string tlEnd;
        public static int tlInterval = 0;
        public static int tlEndInterval = 0;
        public static long tlIntervalType = 60000;
        public static long tlEndIntervalType = 60000;
        public static DateTime tlStartDate;
        public static bool startNow = true; //day night cycle is not enabled unless checked box checked
    }
}
