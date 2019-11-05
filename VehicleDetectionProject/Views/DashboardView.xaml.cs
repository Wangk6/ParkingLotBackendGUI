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
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        List<ParkingLot> pk = new List<ParkingLot>();
        DashboardViewModel dvm;

        public DashboardView()
        {
            InitializeComponent();
            RefreshData();
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
                //Name
                txtParkingLotName.Text = pk[index].LotName;
                //Number
                txtParkingLotNumber.Text = pk[index].LotNumber.ToString();
                //Category
                txtParkingLotCategory.Text = pk[index].PermitType;
                //Status
                txtParkingLotStatus.Text = (pk[index].Is_Lot_Open == 'Y') ? "Open" : "Closed";
                //Message
                txtMessage.Text = pk[index].Lot_Message;
                //Cars Parked
                txtParkingLotCurrentParked.Text = pk[index].Num_Of_Cars_Parked.ToString();
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
            //Add Refresh when inserting/updating camera url to database is complete
            //RefreshData()

            //Add to comboBoxParkingLot combobox
            foreach (ParkingLot i in pk)
            {
                comboBoxParkingLot.Items.Add(i.LotName + " " + i.LotNumber);
            }
        }

        private void ClearInfo()
        {
            //Name
            txtParkingLotName.Text = null;
            //Number
            txtParkingLotNumber.Text = null;
            //Category
            txtParkingLotCategory.Text = null;
            //Status
            txtParkingLotStatus.Text = null;
            //Message
            txtMessage.Text = null;
            //Cars Parked
            txtParkingLotCurrentParked.Text = null;
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
