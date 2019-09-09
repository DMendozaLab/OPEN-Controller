using log4net;
using Cyberbear_Events.Machine.CameraControl;
using Cyberbear_Events.Machine.LightsArdunio;
using Cyberbear_Events.Machine.GrblArdunio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cyberbear_Events.Machine
{
    /// <summary>
    /// Machine will be the class that contains everything needed for the
    /// machine to run and be connected
    /// </summary>
    /// MAY NOT NEED SO DON'T HOLD BREATH
    class Machine
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private GRBLArdunio grblArdunio;
      //  private LightsArdunio lightsArdunio;
     //   private CameraControl cameraControl;
        private string name; //name of machine, may be user added or by front end automatically

        public GRBLArdunio GrblArdunio { get => grblArdunio; set => grblArdunio = value; }
     //   public LightsArdunio LightsArdunio { get => lightsArdunio; set => lightsArdunio = value; }
      //  public CameraControl CameraControl { get => cameraControl; set => cameraControl = value; }
        public string Name { get => name; set => name = value; }

        //constructor
        public Machine()
        {
            log.Info("New machine start named: " + name);

            GrblArdunio = new GRBLArdunio();
            log.Info("New Grbl Ardunio added");
        //    LightsArdunio = new LightsArdunio();
            log.Info("New Lights Ardunuio added");
          //  cameraControl = new CameraControl();
            log.Info("New Camera Control added");
        }
    }
}
