using System;
using System.Collections.Generic;
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

namespace MANDRAKEware
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
            log.Info("Entering Program");

            InitializeComponent();
        }

        private void ExperimentBuilder_Btn_Click(object sender, RoutedEventArgs e)
        {
            ExperimentBuilderWindow experimentBuilderWindow = new ExperimentBuilderWindow();
            experimentBuilderWindow.Show();
        }

        private void MachineWindow_Click(object sender, RoutedEventArgs e)
        {
            MachineConnectionWindow machineConnectionWindow = new MachineConnectionWindow();
            machineConnectionWindow.Show();

            log.Info("Opened Machine Connection Window");
        }

        private void LoadExperimentBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StartMachineBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StopMachineBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
