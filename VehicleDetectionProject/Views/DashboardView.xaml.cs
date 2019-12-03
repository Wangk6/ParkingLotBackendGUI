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
            FillDataAsync();
        }

        //User selects a parking lot and displays existing camera URL
        private void ParkingLot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int index = comboBoxParkingLot.SelectedIndex;
                //Status
                txtParkingLotStatus.Text = (pk[index].Is_Lot_Open == 'Y') ? "Open" : "Closed";
                //Max Capacity
                txtParkingLotCurrentAvailable.Text = (pk[index].MaxCapacity - pk[index].Num_Of_Cars_Parked).ToString();
            }
            catch (Exception ex) {

            };
        }

        private void FillInfo()
        {
            try
            {
                ClearInfo();
                comboBoxParkingLot.ItemsSource = pk;
            }
            catch(Exception e)
            {

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

        private async Task RefreshDataAsync()
        {
            NoConnection.Visibility = Visibility.Hidden;
            RefreshDataIcon.Visibility = Visibility.Visible;
            dvm = new DashboardViewModel();

            bool status = await Task.Run(() => dvm.IsServerConnected());

            if (status == true) //Connection Found
            {
                pk = await Task.Run(() => dvm.GetParkingLots());
                connectionStatus(true);
                RefreshDataIcon.Visibility = Visibility.Hidden;
            }
            else //No Connection
            {
                RefreshDataIcon.Visibility = Visibility.Hidden;
                NoConnection.Visibility = Visibility.Visible;
                connectionStatus(false);
            }
        }

        private async Task FillDataAsync()
        {
            NoConnection.Visibility = Visibility.Hidden;
            LoadingData.Visibility = Visibility.Visible;
            dvm = new DashboardViewModel();

            bool status = await Task.Run(() => dvm.IsServerConnected());

            if (status == true) //Connection Found
            {
                pk = dvm.GetParkingLots();
                await Task.Run(() => FillInfo());
                connectionStatus(true);
                LoadingData.Visibility = Visibility.Hidden;
            }
            else //No Connection
            {
                LoadingData.Visibility = Visibility.Hidden;
                NoConnection.Visibility = Visibility.Visible;
                connectionStatus(false);
            }
        }

        private void buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshDataAsync();
        }
    }
}
