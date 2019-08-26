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
using MANDRAKEware_Events.Machine.GrblArdunio;

namespace MANDRAKEware
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //logger
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //windows
        ExperimentBuilderWindow experimentBuilderWindow = new ExperimentBuilderWindow();
        MachineConnectionWindow machineConnectionWindow = new MachineConnectionWindow();

        //testing
       

        public MainWindow()
        {
            log4net.Config.XmlConfigurator.Configure();
            log.Info("==============Entering Program===============");

            InitializeComponent();
        }

        private void ExperimentBuilder_Btn_Click(object sender, RoutedEventArgs e)
        {
           // ExperimentBuilderWindow experimentBuilderWindow = new ExperimentBuilderWindow();
            experimentBuilderWindow.Show();
        }

        //ADD THE UPDATED SERIAL BOX FUNCTION TO THIS
        private void MachineWindow_Click(object sender, RoutedEventArgs e)
        {
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
        //    string filePath = @"C:\Users\lsceedlings\Desktop\Lando's Folder\GBRLCommands.txt";

        //    List<string> lines = File.ReadAllLines(filePath).ToList(); //putting all the lines in a list

        //    foreach(string line in lines)
        //    {
        //     //   GRBLArdunio.SendLine(line);
        //    }
        }

        private void StopMachineBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            machineConnectionWindow.Close();
            experimentBuilderWindow.Close();
                       
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
