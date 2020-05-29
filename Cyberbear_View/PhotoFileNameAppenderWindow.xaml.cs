using Cyberbear_Events.MachineControl.CameraControl;
using System.Windows;

namespace Cyberbear_View
{
    /// <summary>
    /// Interaction logic for PhotoFileNameAppenderWindow.xaml
    /// </summary>
    public partial class PhotoFileNameAppenderWindow : Window
    {

        public PhotoFileNameAppenderWindow()
        {
            InitializeComponent();

            PhotoFileNameTextbox.Text = "NewImage"; //default name in camera settings
        }

        //returns new files naming scheme
        private void ConfirmationButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
