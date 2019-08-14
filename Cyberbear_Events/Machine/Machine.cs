using MANDRAKEware.Machine.GrblArdunio;
using MANDRAKEware_Events.Machine.GrblArdunio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mandrake_Events.Machine
{
    /// <summary>
    /// Machine will be the class that contains everything needed for the
    /// machine to run and be connected
    /// </summary>
    /// MAY NOT NEED SO DON'T HOLD BREATH
    class Machine
    {
        private GRBLArdunio grblArdunio;
        //  public LightsArdunio lightArdunio;
        //  public CameraControl camera;

        public GRBLArdunio GrblArdunio { get => grblArdunio; set => grblArdunio = value; }

        //constructor
        public Machine()
        {
            GrblArdunio = new GRBLArdunio();
        }
    }
}
