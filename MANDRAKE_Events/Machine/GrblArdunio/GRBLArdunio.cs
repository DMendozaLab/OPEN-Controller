using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeometRi;
using log4net;
using System.Reflection;

namespace MANDRAKEware.Machine.GrblArdunio
{
    /// <summary>
    /// Connection type is serial for grbl ardunio
    /// </summary>
    enum ConnectionType
    {
        Serial
    }

    /// <summary>
    /// For connecting and disconnecting from grbl ardunio and other needed things, like sending lines
    /// </summary>
    class GRBLArdunio
    {
        #region Logger
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Properties
        /// <summary>
        /// OperatingMode is an enum that reflects the machine object's current Operating mode
        /// </summary>
        public enum OperatingMode
        {
            Manual, //0
            SendFile,//1
            Probe,//2
            Disconnected,//3
            SendMacro//4
        }

        //events and Actions


        private Vector3d machinePosition = new Vector3d(); //where machine thinks it is
        private Vector3d workPosition = new Vector3d(); //works position of machine (offsets and such)
        private string status; //status of machine
        private bool connected;

        public Vector3d WorkPosition { get => workPosition; set => workPosition = value; }
        public Vector3d MachinePosition { get => machinePosition; set => machinePosition = value; }
        public string Status { get => status; set => status = value; }
        public bool Connected { get => connected; set => connected = value; }
        #endregion

        #region Constructors
        static GRBLArdunio()
        {

        }
        
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
