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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VehicleDetectionProject.ViewModel;

namespace VehicleDetectionProject.Views
{
    /// <summary>
    /// Interaction logic for Controls.xaml
    /// </summary>
    public partial class ConfigureView : UserControl
    {
        //Establish connection
        //Query parking lot names and numbers
        ConfigureViewModel svm = new ConfigureViewModel();

        public ConfigureView()
        {
            InitializeComponent();
        }


        private void ConfigureView_Loaded(object sender, RoutedEventArgs e)
        {
            var lots = svm.GetParkingLots();
            //Add to comboBoxParkingLot combobox
            for (int i = 0; i < lots.Count; i++)
            {
                comboBoxParkingLot.Items.Add(lots[i].LotName + " " + lots[i].LotNumber);
            }
        }
    }
}
