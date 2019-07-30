using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MANDRAKEware.Experiment.ExperimentArrangement.ExperimentParts
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
        private int numCol

        public int XPos { get => xPos; set => xPos = value; }
        public bool Active { get => active; set => active = value; }
        internal Plate[,] PlateArray { get => plateArray; set => plateArray = value; }
        public int NumRows { get => numRow; set => numRow = value; }
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
            for(int i = 0; )
        }
        #endregion
    }
}
