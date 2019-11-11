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
    /// Interaction logic for Controls.xaml
    /// </summary>
    public partial class ConfigureView : UserControl
    {
        List<ParkingLot> pk = new List<ParkingLot>();
        List<ParkingLot> msg = new List<ParkingLot>();
        ConfigureViewModel cvm;
        public ConfigureView()
        {
            InitializeComponent();
            //Query parking lot names and numbers
        }


        private void ConfigureView_Loaded(object sender, RoutedEventArgs e)
        {
            FillInfo();
        }

        //User selects a parking lot and displays existing camera URL
        private void ParkingLot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int index = comboBoxParkingLot.SelectedIndex;
                //URL
                textBoxCameraURL.Text = pk[index].CameraURL;
                //Status
                comboBoxStatus.Text = (pk[index].Is_Lot_Open == 'Y') ? "Open" : "Closed";
                //Message
                comboBoxMessage.Text = pk[index].Lot_Message;
                //Cars Parked
                textBoxCarsParked.Text = pk[index].Num_Of_Cars_Parked.ToString();
                //Max Capacity
                textBoxMaxCapacity.Text = pk[index].MaxCapacity.ToString();
                //Permit Type
                comboBoxPermitType.Text = pk[index].PermitType;
            }
            catch (ArgumentOutOfRangeException ex) { };
        }

        private void FillInfo()
        {
            ClearInfo();
            RefreshData();

            //Change to default visibility
            comboBoxMessage.Visibility = Visibility.Visible;
            textBoxMessage.Visibility = Visibility.Hidden;

            //Add to comboBoxParkingLot combobox
            foreach (ParkingLot i in pk)
            {
                comboBoxParkingLot.Items.Add(i.LotName + " " + i.LotNumber);
                //Parking Lot Name ListView
                listViewParkingLot.Items.Add(i);
            }

            //Add to comboBoxMessage combobox
            foreach(ParkingLot i in msg)
            {
                comboBoxMessage.Items.Add(i.Lot_Message);
            }
        }

        private void ClearInfo()
        {
            comboBoxParkingLot.Text = null;
            textBoxCameraURL.Text = null;
            comboBoxStatus.Text = null;
            comboBoxMessage.Text = null;
            textBoxCarsParked.Text = null;
            textBoxMaxCapacity.Text = null;
            comboBoxPermitType.Text = null;
            listViewParkingLot.Items.Clear();
        }

        private void AddNewMessage_Selected(object sender, RoutedEventArgs e)
        {
            comboBoxMessage.Visibility = Visibility.Hidden;
            textBoxMessage.Visibility = Visibility.Visible;
        }

        //Empty
        private void Modify_Click(object sender, RoutedEventArgs e)
        {
            int index = comboBoxParkingLot.SelectedIndex + 1;
            //If Camera Source is not Empty
            if(!textBoxCameraURL.Text.Trim().Equals(null))
            {
                string cameraURL = textBoxCameraURL.Text.Trim();
                cvm.UpdateCameraURL(index, cameraURL);
            }
            //If Status is not empty
            if (!comboBoxStatus.SelectedItem.Equals(null))
            {
                string status = comboBoxStatus.SelectedItem.ToString();
                //User wants to add a new message, we use the insert statement*******************************
                if(comboBoxMessage.Text.Equals("Add New Message"))
                {

                }
                //Message is not empty, get message index************************************
                else if(comboBoxMessage.SelectedItem.Equals(null))
                {
                    int message = comboBoxMessage.SelectedIndex + 1;


                }
            }

            //If cars parked is not null
            if (!textBoxCarsParked.Text.Trim().Equals(null))
            {
                int num;
                int.TryParse(textBoxCarsParked.Text.Trim(), out num);
                cvm.LotInfoParkedCars(index, num);
            }

            //If max capacity and Permit type is not null
            if (!textBoxMaxCapacity.Text.Trim().Equals(null) && !comboBoxPermitType.Text.Equals(null))
            {
                int capacity;
                int.TryParse(textBoxMaxCapacity.Text.Trim(), out capacity);
                cvm.ParkingLotInfo(index, capacity, comboBoxPermitType.Text);
            }
            //Else if max capacity is not null
            else if(!textBoxMaxCapacity.Text.Trim().Equals(null))
            {
                int capacity;
                int.TryParse(textBoxMaxCapacity.Text.Trim(), out capacity);
                cvm.ParkingLotInfo(index, capacity, null);
            }
            //Else permit type is not null and max capacity is null
            else
            {
                cvm.ParkingLotInfo(index, null, comboBoxPermitType.Text);
            }


        }

        private void RefreshData()
        {
            cvm = new ConfigureViewModel();
            pk = cvm.GetParkingLots();
            msg = cvm.GetStatusMessage();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            FillInfo();
        }
    }
}
