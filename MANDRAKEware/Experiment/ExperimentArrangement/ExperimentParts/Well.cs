using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MANDRAKEware.Experiment.ExperimentArrangement.ExperimentParts
{
    /// <summary>
    /// The most basic part of the experiment. Insides plates.
    /// </summary>
    class Well
    {
        #region Properties
        /// <summary>
        /// X and Y position in the plate
        /// </summary>
        private int xPos;
        private int yPos;
        /// <summary>
        /// The X and Y offset from the plate. MAY PUT IN PLATE CLASS
        /// </summary>
        private int xOffset;
        private int yOffset;
        /// <summary>
        /// This is to tell if the well actually has something to experiment on in it
        /// </summary>
        private bool active;
        /// <summary>
        /// Description of what is in well
        /// </summary>
        private string description;

        public int XPos { get => xPos; set => xPos = value; }
        public int YPos { get => yPos; set => yPos = value; }
        public int XOffset { get => xOffset; set => xOffset = value; }
        public int YOffset { get => yOffset; set => yOffset = value; }
        public bool Active { get => active; set => active = value; }
        public string Description { get => description; set => description = value; }
        #endregion

        #region Constructor
        public Well()
        {
            //TODO: Constructor
        }
        #endregion

        #region Methods
        /// <summary>
        /// ActivateWell() changes the active property value to true to show that 
        /// the well is being experimented on.
        /// </summary>
        public void ActivateWell()
        {
            this.active = true;
        }
        #endregion 
    }
}
