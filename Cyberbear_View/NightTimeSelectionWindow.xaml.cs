using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Cyberbear_View
{
    /// <summary>
    /// Interaction logic for NightTimeSelectionWindow.xaml
    /// </summary>
    public partial class NightTimeSelectionWindow : Window
    {
        public NightTimeSelectionWindow()
        {
            InitializeComponent();
        }

        private void DateTimeUpDown_InputValidationError(object sender, Xceed.Wpf.Toolkit.Core.Input.InputValidationErrorEventArgs e)
        {
            MessageBox.Show("The date inputed was formatted incorrently, please try again");
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
