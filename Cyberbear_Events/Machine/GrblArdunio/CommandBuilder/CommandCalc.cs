using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using GeometRi;

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
        /// Returns the distance between two points
        /// </summary>
        /// <param name="point1">The "starting" point of the distance measurement</param>
        /// <param name="point2">The "ending" point of hte distance measurement</param>
        /// <returns>A double of the calculated distance</returns>
        private double DistanceBtnPts(Point3d point1, Point3d point2)
        {
            return point1.DistanceTo(point2);
        }

        /// <summary>
        /// Adds two GeometRi 3D points in a method
        /// </summary>
        /// <param name="startPt">Starting 3D point</param>
        /// <param name="endPt">Ending 3D point</param>
        /// <returns></returns>
        private Point3d PointsAdd(Point3d startPt, Point3d endPt)
        {
            return (startPt.Add(endPt));
        }

        /// <summary>
        /// Subtracts two 3D Geomtri points
        /// </summary>
        /// <param name="startPt">Starting 3D point</param>
        /// <param name="endPt">Ending 3D point</param>
        /// <returns></returns>
        private Point3d PointsSub(Point3d startPt, Point3d endPt)
        {
            return (startPt.Subtract(endPt));
        }

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

        /// <summary>
        /// Finds an endpoint of motion from starting point and distance
        /// </summary>
        /// <param name="startPt">Starting point of command</param>
        /// <param name="distance">Distance to be travelled over command</param>
        /// <returns>End point position in absolute distance</returns>
        public int EndPtCalc(int startPt, int distance)
        {
            return Add(startPt, distance);
        }
        #endregion
    }
}
