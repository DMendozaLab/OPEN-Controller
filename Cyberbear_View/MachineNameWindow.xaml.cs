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
using System.Xml.Linq;

namespace Cyberbear_View
{
    /// <summary>
    /// Interaction logic for MachineName.xaml
    /// </summary>
    public partial class MachineNameWindow : Window
    {
        public string TextBoxName
        {
            get
            {
                if (tbName == null)
                    return string.Empty;
                return tbName.Text;
            }
        }

        public MachineNameWindow()
        {
            InitializeComponent();
        }

        private void MachineNameEnterButton_Click(object sender, RoutedEventArgs e)
        {
            if(tbName.Text != "")
            {
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("Please enter a name for the window");
            }
        }

    }
}
