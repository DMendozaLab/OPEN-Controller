using log4net;
using Cyberbear_Events.Machine.GrblArdunio.CommandBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cyberbear_Events.Machine.GrblArdunio.CommandBuilder
{
    /// <summary>
    /// This class builds, figures out distance, and executes the commands for grbl
    /// </summary>
    class CommandBuilder
    {
        #region Logger
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion
        //Need to think of everything needed in this class more thoroughly
        #region Properties
        private string command; //completed command

        private CommandCalc calculator; //command calculator

        public string Command { get => command; set => command = value; }
        #endregion

        #region Constructors
        public CommandBuilder()
        {
            
        }
        #endregion

        #region Methods
        /// <summary>
        /// Command Builder function for getting to next position in cycle
        /// </summary>
        /// <param name="startingPt">Starting Point of command</param>
        /// <param name="endPt">Hopeful end point of command</param>
        /// <param name="axis">the Axis that is being moved on</param>
        /// <returns>Completed Command for cycle</returns>
        public string CycleCommand(int startingPt, int endPt, char axis)
        {
            StringBuilder sb = new StringBuilder();
            int distance = calculator.DistanceCalc(startingPt, endPt);
            sb.Append(CommandConst.gAbsolute); //hardcoded for now, will change in future
            sb.Append(axis);
            sb.Append(distance + "");
            sb.Append(CommandConst.feedRate + CommandConst.feedRateSpeed);

            log.Info("Generated Cycle Command: " + sb.ToString());


            return sb.ToString();
        }

        /// <summary>
        /// Builds offset command for moving machine to offset postion
        /// </summary>
        /// <param name="offset">How much is the offset</param>
        /// <param name="startingPt">Starting Point of the machine before command</param>
        /// <param name="axis">Axis command will tkae place on</param>
        /// <returns>Returns command to move offset</returns>
        public string OffsetCommand(int offset, int startingPt, char axis)
        {
            StringBuilder sb = new StringBuilder();
            int offsetPt = calculator.OffsetCalc(offset, startingPt);
            int distance = calculator.DistanceCalc(startingPt, offsetPt);
            sb.Append(CommandConst.gAbsolute);
            sb.Append(axis);
            sb.Append(distance + "");
            sb.Append(CommandConst.feedRate + CommandConst.feedRateSpeed);

            log.Info("Generated Offset Command: " + sb.ToString());


            return sb.ToString(); //will remove later
        }
        #endregion

    }
}
