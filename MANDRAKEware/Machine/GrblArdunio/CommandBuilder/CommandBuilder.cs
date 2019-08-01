using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MANDRAKEware.Machine.GrblArdunio.CommandBuilder
{
    /// <summary>
    /// This class builds, figures out distance, and executes the commands for grbl
    /// </summary>
    class CommandBuilder
    {
        #region Properties
        private string command; //completed command
        private const char macro = 'M'; //const char of M for marco commands
        private const char gCommand = 'G'; //const char for G commands
        private const string gAbsolute = "90"; //for absolute motion
        private const string gIncremental = "91";
       
        private const char xAxis = 'X';
        private const char yAxis = 'Y';

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
        /// <returns></returns>
        public string CycleCommand(int startingPt, int endPt, char axis)
        {
            StringBuilder sb = new StringBuilder();
            int distance = calculator.DistanceCalc(startingPt, endPt);

            //TODO: rest of function where appends stuff

            return null; //will remove later
        }

        public string OffsetCommand(int offset, int startingPt)
        {
            StringBuilder sb = new StringBuilder();
            int offsetPt = calculator.OffsetCalc(offset, startingPt);
            //TODO

            return null;
        }
        #endregion

    }
}
