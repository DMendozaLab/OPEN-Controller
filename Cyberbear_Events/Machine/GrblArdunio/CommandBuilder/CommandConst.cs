using System.Collections.Generic;

namespace Cyberbear_Events.Machine.GrblArdunio.CommandBuilder
{
    /// <summary>
    /// Collection of the constants used by command builder for building commands
    /// Will include things like 'G' for g command and 'M' for marco commands.
    /// </summary>
    public static class CommandConst
    {
        public static char dollarSign = '$';
        public static char macro = 'M'; //const char of M for marco commands
        public static char gCommand = 'G'; //const char for G commands
        public static string gAbsolute = "90"; //for absolute motion
        public static string gIncremental = "91";
        public static char feedRate = 'F';
        public static int feedRateSpeed = 10000; //hardcoded will change later
        public static char xAxis = 'X'; //for x axis motion commands
        public static char yAxis = 'Y'; //for y axis motion commands
        public static char homeCmd = 'H'; //homing command with $
        public static string slpCmd = "SLP"; //for making stepper sleep
        public static char parameterCmd = '#'; //for seeing parameters
        public static char killAlarmLock = 'X';
        public static char cycleStart = '~'; //for restarting buffered gcode commands after feed hold (!)
        public static char currentStatus = '?'; //returns grbl state and current machine and work positions

        //will decided if should be stored this way or another
        private static IDictionary<string, string> grblCodes = new Dictionary<string, string>()
        {
            {"absolute", "90"},
            {"incremental", "91"},
        };
    }
}