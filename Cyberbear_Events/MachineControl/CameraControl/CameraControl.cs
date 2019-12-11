using log4net;
using Cyberbear_Events.Util;
using SynchronousGrab;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Threading;

namespace Cyberbear_Events.MachineControl.CameraControl
{
    /// <summary>
    /// Class for controlling the camera module of the machine
    /// </summary>
    public class Camera
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly Camera instance = new Camera();

        private CameraConst cameraConst = new CameraConst();

        #region Constructors
        /// <summary>
        /// Constructor for Camera Control Class
        /// </summary> 
        static Camera()
        {
           
        }
        public static Camera Instance //instance is the object's understanding at the specific point in time when called.
        {
            get
            {
                return instance;
            }
        }
        #endregion

        #region Properties
        public bool settingsLoaded = false; //are settings for the camera loaded or not?

        string previousSettingsDir;
        private VimbaHelper m_VimbaHelper = null;
        public Image m_PictureBox;
        //TODO Make real output
        private CameraInfo selectedItem;
        public VimbaHelper VimbaHelper { get => m_VimbaHelper; set => m_VimbaHelper = value; } //from VIMBAhelper.cs
        public CameraInfo SelectedItem { get => selectedItem; set => selectedItem = value; }
        public CameraConst CameraConst { get => cameraConst; set => cameraConst = value; }

        //Add log message to logging list box
        public delegate void ImageAcquired();
        public event EventHandler ImageAcquiredEvent;
        List<CameraInfo> cameras; //what is this?
        #endregion

        #region Start and Setting Methods
        /// <summary>
        /// Starts the Vimba camera and others
        /// </summary>
        public void StartVimba()
        {
            //cameraControl = new CameraControl();
            //TODO Refactor to raise event
            //updateCameraSettingsOptions();
            try
            {
                //Start up Vimba API
                VimbaHelper vimbaHelper = new VimbaHelper();
                vimbaHelper.Startup(OnCameraListChanged);
                //Text += String.Format(" Vimba .NET API Version {0}", vimbaHelper.GetVersion());
                m_VimbaHelper = vimbaHelper;
                VimbaHelper = m_VimbaHelper;

                VimbaHelper.ImageAcquiredEvent += OnImageAcquired;
                try
                {
                    UpdateCameraList();

                }
                catch (Exception exception)
                {
                    LogError("Could not update camera list. Reason: " + exception.Message);
                }
            }
            catch (Exception exception)
            {
                LogError("Could not startup Vimba API. Reason: " + exception.Message);
            }
        }

        public void OnImageAcquired(object sender, EventArgs e)
        {
            ImageAcquiredEvent.Raise(this, new EventArgs());
        }

        /// <summary>
        /// Shuts down Vimba camera
        /// </summary>
        public void ShutdownVimba()
        {
            if (null != m_VimbaHelper)
            {
                try
                {
                    //Shutdown Vimba API when application exits
                    m_VimbaHelper.Shutdown();

                    m_VimbaHelper = null;
                }
                catch (Exception exception)
                {
                    LogError("Could not shutdown Vimba API. Reason: " + exception.Message);
                }
            }
        }

        /// <summary>
        /// Logs message from camera in logger program
        /// </summary>
        /// <param name="message">The message (in string) passed to the method</param>
        public void LogMessage(string message)
        {
            if (null == message)
            {
                throw new ArgumentNullException("message");
            }

            _log.Info(message);
        }

        /// <summary>
        /// Logs error in logger program from camera
        /// </summary>
        /// <param name="message">The error passed into the method as a string</param>
        public void LogError(string message)
        {
            if (null == message)
            {
                throw new ArgumentNullException("message");
            }
            _log.Error(message);

        }

        public void UpdateCameraState(bool cameraReady)
        {
            if (cameraReady)
            {
                CameraConst.CameraState = "Ready";
            }
            else
            {
                CameraConst.CameraState = "Not Ready";
            }

            _log.Debug("Camera State is " + CameraConst.CameraState);
        }


        public void UpdateCameraList()
        {

            cameras = VimbaHelper.CameraList;

            if (cameras.Any())
            {
                //SelectedItem = cameras[0];
                UpdateCameraState(true);
                //_log.Info("New Selected Camera" + SelectedItem);
            }
            else
            {
                UpdateCameraState(false);
            }

        }

        /// <summary>
        /// Gets camera list from object and allows user to see all cameras available. Returns a list of string names for UI
        /// </summary>
        /// <returns>Returns list of camera infos available</returns>
        public List<string> GetCameraListNames()
        {
            List<string> cameraListNames = new List<string>();
            UpdateCameraList();

            foreach(CameraInfo cameraInfo in cameras)
            {
                string nameID;
                nameID = cameraInfo.Name + cameraInfo.ID;

                
                cameraListNames.Add(nameID); //make change to ID or make dictionary perhaps
            }

            return cameraListNames;
        }


        public void selectCameraName(string name)
        {
            int position = 0;

            foreach(CameraInfo cameraInfo in cameras)
            {
                if (name == (cameraInfo.Name + cameraInfo.ID))
                {
                    SelectCamera(position);
                }
                else
                    position++;
            }
        }

        /// <summary>
        /// For selecting camera from camera list
        /// </summary>
        public void SelectCamera(int listPosition)
        {
            selectedItem = cameras[listPosition]; //TODO make it so camera don't overlap with machines

            _log.Info("New Selected Camera" + SelectedItem);
        }

        public void OnCameraListChanged(object sender, EventArgs args)
        {

            if (null != VimbaHelper)
            {
                try
                {
                    UpdateCameraList();

                    LogMessage("Camera list updated.");
                }
                catch (Exception exception)
                {
                    LogError("Could not update camera list. Reason: " + exception.Message);
                }
            }
        }
        public void loadCameraSettings()
        {
            //     string cameraSettingsFileName = Directory.GetCurrentDirectory() + @".\Machine\CameraControl\CameraSettings\" + CameraConst.CameraSettingsPath;
            string cameraSettingsFileName = CameraConst.CameraSettingsPath;
            if (!String.Equals(cameraSettingsFileName, previousSettingsDir))
            {
                previousSettingsDir = cameraSettingsFileName;
                forceSettingsReload();
            }

        }

        public void forceSettingsReload()
        {
            string cameraSettingsFileName = CameraConst.CameraSettingsPath;
            if (File.Exists(cameraSettingsFileName))
            {
                LogMessage("Loading camera settings");
                if (VimbaHelper != null && SelectedItem != null)
                {
                    _log.Debug(SelectedItem.ID);
                    VimbaHelper.loadCamSettings(cameraSettingsFileName, SelectedItem.ID);
                    LogMessage("Loaded Camera Settings");
                }
                else
                {
                    LogError("Settings not loaded because camera is disconnected");
                }

            }
            else
            {
                LogError("CameraSettings Filename: " + cameraSettingsFileName);
                LogError("Invalid file name for Camera Settings");
            }
            settingsLoaded = true;


        }
        #endregion

        #region Image Capturing
        public BitmapImage bi;
        public BitmapImage UpdateImageBox(System.Drawing.Image image)
        {

            using (var ms = new MemoryStream())
            {
                bi = new BitmapImage();
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                Thread.Sleep(500);
                ms.Position = 0;


                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.StreamSource = ms;
                bi.EndInit();
            }


            return bi;

        }
        public Task WriteBitmapToFile(BitmapImage image, string filePath)
        {
            Task task = new Task(() =>
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));

                using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
            });
            return task;
        }

        ImageFormat fileType = ImageFormat.Png;

        //Will need to changed this function in the future
        //Will need to address the settings that are commented out
        public String createFilePath()
        {
            StringBuilder sb = new StringBuilder();
            
            
            
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd--H-mm-ss");

            sb.Append(currentDate + "_");
            sb.Append(CameraConst.FileName);
            sb.Append("." + fileType.ToString().ToLower()); //putting the .png 

            String filePath = Path.Combine(CameraConst.SaveFolderPath, sb.ToString());
            return filePath;
        }

        public Task WriteImageToFile(System.Drawing.Image image)
        {
            Task task = new Task(() => {

                String filePath = createFilePath();
                _log.Info("Image written to: " + filePath);
                
                image.Save(filePath, fileType);

                LogMessage("Image acquired synchonously."); //TODO: fix current plate setting when taking pictures
                                                            // if (Properties.Settings.Default.CurrentPlate[0] < Properties.Settings.Default.TotalRows)
                image.Dispose(); //to dispose of pic after saving
            });

            return task;
        }
        //changed return value from BitmapImage to void because of memory leak issues
        public void CapSaveImage()
        {
            try
            {
                //Determine selected camera
                if (null == SelectedItem)
                {
                    throw new NullReferenceException("No camera selected.");
                }


                //Acquire an image synchronously (snap) from selected camera
                Image image = VimbaHelper.AcquireSingleImage(SelectedItem.ID);
                System.Drawing.Image imageCopy = (System.Drawing.Image)image.Clone();
              //  BitmapImage img = UpdateImageBox(imageCopy);
              //  BitmapImage imgCopy = img;


                String filePath = createFilePath();
                if (Directory.Exists(CameraConst.SaveFolderPath))
                {


                    Task witf = WriteImageToFile(imageCopy);
                    witf.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
                    witf.Start();

                }
                else
                {
                    LogError("Invalid directory selected");
                }

              //  return imgCopy;

            }
            catch (Exception exception)
            {
                LogError("Could not acquire image. Reason: " + exception.Message);
                //return null;
            }
        }
        static void ExceptionHandler(Task task)
        {
            var exception = task.Exception;
            _log.Error(exception);
        }
    }
    #endregion
}
