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
using VehicleDetectionProject.Database;
using VehicleDetectionProject.ViewModel;

namespace VehicleDetectionProject.Views
{
    /// <summary>
    /// Interaction logic for Configure.xaml
    /// </summary>
    public partial class SourceView : UserControl
    {
        List<ParkingLot> pk = new List<ParkingLot>();
        SourceViewModel svm;
        public SourceView()
        {
            InitializeComponent();
        }

        private void SourceView_Loaded(object sender, RoutedEventArgs e)
        {
            FillInfo();
        }

        //Placeholder, need to add update sql statement ****************************************
        private void buttonAddCamera_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = comboBoxParkingLot.SelectedIndex + 1;
                string url = textBoxCameraURL.Text.Trim();
                //Update parking lots camera url
                svm.UpdateCameraURL(index, url);
                FillInfo();
            }
            catch (ArgumentOutOfRangeException ex) { };
        }


        //Clears the information previously and adds up-to-date data
        private void FillInfo()
        {
            //Add Refresh when inserting/updating camera url to database is complete
            RefreshData();

            try
            {
                //Add to comboBoxParkingLot combobox
                comboBoxParkingLot.ItemsSource = pk;
                listViewParkingLot.ItemsSource = pk;
            }
            catch(Exception e)
            {

            }
        }

        private void buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
            FillInfo();
        }

        private void RefreshData()
        {
            svm = new SourceViewModel();
            pk = svm.GetParkingLots();
        }
    }
}
