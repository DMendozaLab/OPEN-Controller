using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using log4net;
using Cyberbear_Events.MachineControl.GrblArdunio;
using Cyberbear_Events.MachineControl;

namespace Cyberbear_View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //logger
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
       

        public MainWindow()
        {
            log4net.Config.XmlConfigurator.Configure();
            log.Info("==============Entering Program===============");

            //starting vimba, will start cameras individually for each, will look into for future 
            /*Task task = new Task(() => Machine.CameraControl.StartVimba());
            task.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
            task.Start();*/

            InitializeComponent();
        }

        private void ExperimentBuilder_Btn_Click(object sender, RoutedEventArgs e)
        {
           // ExperimentBuilderWindow experimentBuilderWindow = new ExperimentBuilderWindow();
            //experimentBuilderWindow.Show();
        }

        private void MachineWindow_Click(object sender, RoutedEventArgs e)
        {
            MachineConnectionWindow machineConnectionWindow = new MachineConnectionWindow(); //maybe make new one every time?

            machineConnectionWindow.Show();

            log.Info("Opened Machine Connection Window");
        }

        private void LoadExperimentBtn_Click(object sender, RoutedEventArgs e)
        {

        }


        /// <summary>
        /// Starts Machine cycle from text file (For now)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartMachineBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StopMachineBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {             
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Exception Handler when expections happens
        /// </summary>
        /// <param name="task">The task were the expection occurred is passed to the method
        /// to log the error</param>
        static void ExceptionHandler(Task task)
        {
            var exception = task.Exception;
            log.Error(exception);
        }
    }
}
