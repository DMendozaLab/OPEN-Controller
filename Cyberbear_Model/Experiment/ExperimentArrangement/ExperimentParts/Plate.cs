using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyberbear_Events.Experiment.ExperimentArrangement.ExperimentParts
{
    /// <summary>
    /// A plate of wells for building experiment
    /// </summary>
    class Plate
    {

        #region Properties
        /// <summary>
        /// x and y pos of plate
        /// </summary>
        private int xPos;
        private int yPos;
        /// <summary>
        /// x and y offset of plate from tray
        /// </summary>
        private int xPlateOffset;
        private int yPlateOffset;
        /// <summary>
        /// Is the entire plate active? All wells within active?
        /// </summary>
        private bool active; //only if whole plate is active
        /// <summary>
        /// Numbers of rows and columns in the plate
        /// </summary>
        private int numRows;
        private int numCol;
        /// <summary>
        /// Array of Wells in the well
        /// </summary>
        private Well[,] wellArray;

        public int YPos { get => yPos; set => yPos = value; }
        public int XPos { get => xPos; set => xPos = value; }
        public int XPlateOffset { get => xPlateOffset; set => xPlateOffset = value; }
        public int YPlateOffset { get => yPlateOffset; set => yPlateOffset = value; }
        public bool Active { get => active; set => active = value; }
        public int NumRows { get => numRows; set => numRows = value; }
        public int NumCol { get => numCol; set => numCol = value; }
        public Well[,] WellArray { get => wellArray; set => wellArray = value; }
        #endregion

        #region Constructors
        public Plate()
        {
            //TODO: do constructors
        }
        #endregion

        #region Methods
        /// <summary>
        /// Actives all the wells in a plate to true boolean statement
        /// </summary>
        public void ActivatePlate()
        {
            for(int i =0; i < NumRows; i++)
            {
                for(int j = 0; j <numCol; j++)
                {
                    WellArray[i, j].Active = true;
                }
            }
        }

        /// <summary>
        /// Sees if Plate is active
        /// </summary>
        /// <returns>Returns 1 for success and 0 for failure</returns>
        public int IsActive()
        {
            for (int i = 0; i < NumRows; i++)
            {
                for (int j = 0; j < numCol; j++)
                {
                    int truthValue = WellArray[i, j].IsActive();

                    if (truthValue == 0) //if fails check then returns failure condition
                        return 0;
                }
            }

            return 1; //success
        }

        #endregion
    }
}
