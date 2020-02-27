using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyberbear_Events.MachineControl.CameraControl
{
    /// <summary>
    /// Consts for Camera Control Class
    /// </summary>
    /// May add methods to change the consts
    public class CameraConst
    {
        public string CameraState = "Not Ready"; //state of camera 
        public string CameraSettingsPath = @"C:\Users\sam998\Desktop\Cyberbear\Cyberbear\Cyberbear_Events\Machine\CameraControl\CameraSettings\wednesday_night_special.xml"; //where are camera settings saved
        public string SaveFolderPath = @"C:\Users\sam998\Desktop\TestPhotosFolder"; //save folder for photo capturing, hardcoded to desktop, remember to change when needed
        public string FileName = "NewImage"; //name of saved images
        public bool AddPositionNumbers; //added position numbers to end of file? true for yes, no for no
        public int positionNum = 0; //for position in cycle

        public CameraConst()
        {

        }
    }
}
