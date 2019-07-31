using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MANDRAKEware.Machine.GrblArdunio.CommandBuilder
{
    /// <summary>
    /// This class is a calculator basically for figuring out distances and what not
    /// </summary>
    class CommandCalc
    {
        #region Consturctors
        public CommandCalc()
        {

        }
        #endregion

        //May add float versions of calc functions
        #region Calc Functions
        /// <summary>
        /// This functions takes in two numbers and returns the sum of the two
        /// </summary>
        /// <param name="num1">First number to be added</param>
        /// <param name="num2">Second number to be added</param>
        /// <returns>Returns the sum of the two numbers</returns>
        private int Add(int num1, int num2)
        {
            return (num1 + num2);
        }

        /// <summary>
        /// This numbers subtracts the first number by the second number
        /// </summary>
        /// <param name="num1">Number that will be subtracted from</param>
        /// <param name="num2">Number that will subtracted from num1</param>
        /// <returns>Returns the difference between num1 and num2</returns>
        private int Subtract(int num1, int num2)
        {
            return (num1 - num2);
        }

        /// <summary>
        /// Function for finding distance for grbl commands
        /// </summary>
        /// <param name="startPt">Starting point of command</param>
        /// <param name="endPt">Ending point of command</param>
        /// <returns>returns the distance betwen start pt and end pt</returns>
        public int DistanceCalc(int startPt, int endPt)
        {
            return Subtract(startPt, endPt);
        }

        /// <summary>
        /// Calcs new position from offset
        /// </summary>
        /// <param name="offset">Distance to offset</param>
        /// <param name="startingPt">Starting point before offset</param>
        /// <returns>The new distance with offset added</returns>
        public int OffsetCalc(int offset, int startingPt)
        {
            return Add(offset, startingPt);
        }
        #endregion
    }
}
