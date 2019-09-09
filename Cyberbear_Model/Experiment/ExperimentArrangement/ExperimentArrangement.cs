using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyberbear_Events.Experiment.ExperimentArrangement
{
    /// <summary>
    /// The arrangment of the entire experiment file
    /// </summary>
    class ExperimentArrangement
    {
        #region Properties
        /// <summary>
        /// The distance between each well of the experiment
        /// </summary>
        private int distanceBtnWells;
        /// <summary>
        /// The distance between each plate of the experiment
        /// </summary>
        private int distanceBtnPlates;
        /// <summary>
        /// The distance between each tray of the experiment
        /// </summary>
        private int distanceBtnTrays;

        public int DistanceBtnWells { get => distanceBtnWells; set => distanceBtnWells = value;}
        public int DistanceBtnPlates { get => distanceBtnPlates; set => distanceBtnPlates = value; }
        public int DistanceBtnTrays { get => distanceBtnTrays; set => distanceBtnTrays = value; }
        #endregion
    }
}
