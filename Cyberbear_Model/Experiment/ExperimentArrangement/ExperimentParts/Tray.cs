using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyberbear_Events.Experiment.ExperimentArrangement.ExperimentParts
{
    /// <summary>
    /// The boardest part of experiment. Contains array of plates
    /// </summary>
    class Tray
    {
        #region Properties
        private int xPos; //only one row of trays
        private bool active; //whole tray is active
        private Plate[,] plateArray;
        /// <summary>
        /// Numbers of rows and cols of plates in tray
        /// </summary>
        private int numRow;
        private int numCol;

        public int XPos { get => xPos; set => xPos = value; }
        public bool Active { get => active; set => active = value; }
        internal Plate[,] PlateArray { get => plateArray; set => plateArray = value; }
        public int NumRows { get => numRow; set => numRow = value; }
        public int NumCol { get => numCol; set => numCol = value; }
        #endregion

        #region Constructor
        public Tray()
        {
            //TODO
        }
        #endregion

        #region Methods
        /// <summary>
        /// Activates all the plates in the tray
        /// </summary>
        public void ActivateTray()
        {
            for(int i = 0; i < numRow; i++)
            {
                for(int j = 0; j < numCol; j++)
                {
                    plateArray[i, j].ActivatePlate(); //activates the plate
                }
            }
        }

        /// <summary>
        /// Checks to see if tray is active or not
        /// </summary>
        /// <returns>Returns 1 is success and 0 if not</returns>
        public int IsActive()
        {
            for (int i = 0; i < numRow; i++)
            {
                for (int j = 0; j < numCol; j++)
                {
                    int truthValue = plateArray[i, j].IsActive(); //checks to see if plate is active
                    if (truthValue == 0)
                        return 0; //failure
                }
            }

            return 1; //success
        }
        #endregion
    }
}
