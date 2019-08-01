using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MANDRAKEware.Machine.GrblArdunio
{
    /// <summary>
    /// For connecting and disconnecting from grbl ardunio and other needed things, like sending lines
    /// </summary>
    class GRBLArdunio
    {

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
        public int Connect()
        {

        }


        #endregion


    }
}
