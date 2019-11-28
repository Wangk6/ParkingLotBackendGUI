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
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;

namespace VehicleDetectionProject.Views
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        List<ParkingLot> pk = new List<ParkingLot>();
        DashboardViewModel dvm;

        public DashboardView()
        {
            InitializeComponent();
        }

        private void DashboardView_Loaded(object sender, RoutedEventArgs e)
        {
            FillInfo();
        }

        //User selects a parking lot and displays existing camera URL
        private void ParkingLot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                connectionStatus(true);
                int index = comboBoxParkingLot.SelectedIndex;
                //Status
                txtParkingLotStatus.Text = (pk[index].Is_Lot_Open == 'Y') ? "Open" : "Closed";
                //Max Capacity
                txtParkingLotCurrentAvailable.Text = (pk[index].MaxCapacity - pk[index].Num_Of_Cars_Parked).ToString();
            }
            catch (ArgumentOutOfRangeException ex) {
                connectionStatus(false);
            };
        }

        private void FillInfo()
        {
            ClearInfo();

            try
            {
                //Add Refresh when inserting/updating camera url to database is complete
                RefreshData();
                comboBoxParkingLot.ItemsSource = pk;
            }
            catch(Exception e)
            {
                connectionStatus(false);
            }
        }

        private void ClearInfo()
        {
            //Status
            txtParkingLotStatus.Text = null;
            //Max Capacity
            txtParkingLotCurrentAvailable.Text = null;
        }

        private void connectionStatus(bool status)
        {
            //On
            if(status == true)
            {
                connectionStatusIcon.Foreground = Brushes.Green;
            }
            else //Off
            {
                connectionStatusIcon.Foreground = Brushes.Red;
            }
        }
        private void streamStatus(bool status)
        {
            //On
            if(status == true)
            {
                streamStatusIcon.Foreground = Brushes.Green;
            }
            else //Off
            {
                streamStatusIcon.Foreground = Brushes.Red;
            }
        }
        private void trackingStatus(bool status)
        {
            //On
            if(status == true)
            {
                trackingStatusIcon.Foreground = Brushes.Green;
            }
            else //Off
            {
                trackingStatusIcon.Foreground = Brushes.Red;
            }
        }

        private void RefreshData()
        {
            dvm = new DashboardViewModel();
            pk = dvm.GetParkingLots();
        }

    }
}
