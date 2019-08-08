using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeometRi;
using log4net;

namespace MANDRAKEware.Machine.GrblArdunio
{
    /// <summary>
    /// For connecting and disconnecting from grbl ardunio and other needed things, like sending lines
    /// </summary>
    class GRBLArdunio
    {
        //testing logger
        private static readonly ILog log = LogManager.GetLogger(typeof(GRBLArdunio));

        #region Properties
        private Vector3d machinePosition = new Vector3d(); //where machine thinks it is
        private Vector3d workPosition = new Vector3d(); //works position of machine (offsets and such)
        private string status; //status of machine

        public Vector3d WorkPosition { get => workPosition; set => workPosition = value; }
        public Vector3d MachinePosition { get => machinePosition; set => machinePosition = value; }
        public string Status { get => status; set => status = value; }
        #endregion

        #region Constructors
        static GRBLArdunio()
        {

        }
        //don't know if below is needed or not
        public static GRBLArdunio Instance
        {
            get
            {
                return Instance;
            }
        }

        
        #endregion

        #region Connection and Disconnect
        //TODO: for both of theses 
        public int Connect()
        {
            return 1;
        }

        public int Disconnect()
        {
            return 1;
        }

        #endregion


    }
}
