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
using System.Windows.Threading;
using VehicleDetectionProject.Database;
using VehicleDetectionProject.ViewModel;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using Carz;
using System.Reflection;
using Microsoft.Win32;
using System.IO;

namespace VehicleDetectionProject.Views
{
    /*
    * Class: DashboardView
    * Created By: Kevin Wang
    * Purpose: Controls for the DashboardView View Tab
    */
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        List<ParkingLot> pk = new List<ParkingLot>();
        DashboardViewModel dvm;
        Carz.VideoInterpreter vi;

        private static string videoFeed = System.IO.Path.GetFullPath(@"..\..\..\camera.mp4");
        private static string cvFile = System.IO.Path.GetFullPath(@"..\..\..\cars.xml");

    public DashboardView()
        {
            InitializeComponent();
        }

        private void DashboardView_Loaded(object sender, RoutedEventArgs e)
        {
            FillDataAsync();
        }

        #region View Controllers
        /*
         * Method: ParkingLot_SelectionChanged()
         * Input: User Changes Parking Lot
         * Output: None -
         * Purpose: User selects a parking lot and displays current info
         */
        private async void ParkingLot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (comboBoxParkingLot.SelectedIndex != -1)
                {
                    int index = comboBoxParkingLot.SelectedIndex;
                    //Status
                    string statusMsg = dvm.ParkingLotStatusLongDisplay(pk[index].Is_Lot_Open);
                    txtParkingLotStatus.Text = statusMsg;
                    //Parked
                    txtParkingLotCurrentParked.Text = pk[index].Num_Of_Cars_Parked.ToString();
                    //Max Capacity
                    txtParkingLotCurrentAvailable.Text = (pk[index].MaxCapacity - pk[index].Num_Of_Cars_Parked).ToString();
                    VideoDetection();
                }
                else
                {
                    mediaElementPlayer.Source = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + e);
            };
        }

        /*
         * Method: buttonRefresh_Click()
         * Input: User clicks Refresh Button
         * Output: None -
         * Purpose: Calls RefreshDataASync() to get updated database information
         */
        private void buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshDataAsync();
        }

        private void buttonVideoSample_Click(object sender, RoutedEventArgs e)
        {
            string projectDirectory = System.IO.Path.GetFullPath(@"..\..\..\");

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Video files (*.mp4)|*.mp4|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = projectDirectory;
            if (openFileDialog.ShowDialog() == true)
            {
                videoFeed = openFileDialog.FileName;
                Console.WriteLine("*******" + videoFeed);
                vehicleSampleStatus.Foreground = Brushes.Green;
            }
        }

        private void buttonVehicleDetection_Click(object sender, RoutedEventArgs e)
        {
            string projectDirectory = System.IO.Path.GetFullPath(@"..\..\..\");

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = projectDirectory;
            if (openFileDialog.ShowDialog() == true)
            {
                cvFile = openFileDialog.FileName;
                Console.WriteLine("*******" + cvFile);
                vehicleDetectionStatus.Foreground = Brushes.Green;
            }
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            //BindingOperations.GetBindingExpressionBase((ComboBox)sender, ComboBox.ItemsSourceProperty).UpdateTarget();
        }
        #endregion

        #region Car Detection
        void Play(Object o, EventArgs e)
        {
            vi.start();

        }

        public void VideoDetection()
        {
            if (vi != null)
            {
                vi.stop();
            }
            if (mediaElementPlayer.Source != null)
            {
                mediaElementPlayer.Source = null;
            }
            vi = new VideoInterpreter(videoFeed, cvFile, Dispatcher.CurrentDispatcher);
            //If tracking and streaming
            bool working = false;
            working = (vi != null) ? working = true : working = false;
            trackingStatus(working);
            working = (mediaElementPlayer != null) ? working = true : working = true;
            streamStatus(working);
            vi.setCarDidEnterDelegate(CarDidEnter);
            vi.setCarDidLeaveDelegate(CarDidLeave);
            vi.setCarProcessingDone(CarProcessingDone);
            vi.setShowWindow(true);
            vi.setfps(80);
            mediaElementPlayer.MediaOpened += Play;
            mediaElementPlayer.Source = new Uri(videoFeed);

            //vi.start();
        }

        public void CarDidEnter(VideoInterpreter vi)
        {
            dvm.CarDidEnter(comboBoxParkingLot.SelectedIndex + 1);
            CarParked();

            //System.Diagnostics.Debug.WriteLine("CAR DID ENTER CALLED");
        }

        public void CarDidLeave(VideoInterpreter vi)
        {
            dvm.CarDidLeave(comboBoxParkingLot.SelectedIndex + 1);
            CarLeft();

            //System.Diagnostics.Debug.WriteLine("CAR DID LEAVE CALLED");
        }

        public void CarParked()
        {
            int index = comboBoxParkingLot.SelectedIndex;
            if (index > -1)
            {
                //Used to increment/decrement. Usually would prefer to pull data from db but this saves queries
                int myCount = pk[index].Num_Of_Cars_Parked + 1;
                pk[index].Num_Of_Cars_Parked = myCount;
                Console.WriteLine("Parked: " + myCount);
                txtParkingLotCurrentParked.Text = myCount.ToString();
                if (!txtParkingLotCurrentAvailable.Text.Equals("0"))
                {
                    myCount = int.Parse(txtParkingLotCurrentAvailable.Text) - 1;
                    txtParkingLotCurrentAvailable.Text = myCount.ToString();
                }
                //Console.WriteLine("Available: " + myCount + "\n\n");
            }
        }
        public void CarLeft()
        {
            int index = comboBoxParkingLot.SelectedIndex;
            if (index > -1)
            {
                //Used to increment/decrement. Usually would prefer to pull data from db but this saves queries
                int myCount = pk[index].Num_Of_Cars_Parked - 1;
                pk[index].Num_Of_Cars_Parked = myCount;
                Console.WriteLine("Left: " + myCount);
                txtParkingLotCurrentParked.Text = myCount.ToString();
                myCount = int.Parse(txtParkingLotCurrentAvailable.Text) + 1;
                txtParkingLotCurrentAvailable.Text = myCount.ToString();
                Console.WriteLine("Available: " + myCount + "\n\n");
            }
        }

        //When video is done playing
        public void CarProcessingDone(VideoInterpreter vi)
        {
            streamStatus(false);
            trackingStatus(false);
            mediaElementPlayer.Source = null;
            System.Diagnostics.Debug.WriteLine("CarPricessingDone Called");
        }
        #endregion

        #region Getting/Setting/Clearing DB Data
        /*
         * Method: ClearInfo()
         * Input: None
         * Output: None
         * Purpose: Removes the status of parking lot and number of spaces available
         */
        private void ClearInfo()
        {
            //Status
            txtParkingLotStatus.Text = null;
            //Max Capacity
            txtParkingLotCurrentAvailable.Text = null;
        }

        /*
         * Method: FillInfo()
         * Input: None
         * Output: None
         * Purpose: Add Refresh when inserting/updating camera url to database is complete
         *          Clears the information previously and adds up-to-date data
         */
        private void FillInfo()
        {
            try
            {
                ClearInfo();
                comboBoxParkingLot.ItemsSource = pk;
            }
            catch (Exception e)
            {

            }
        }

        /*
         * Method: RefreshDataASync()
         * Input: User Clicks Refresh Database button
         * Output: None
         * Purpose: Checks for a connection before parsing the parking lots and refreshing info
         *          while sending user information on the pending status of refreshing the data
         */
        private async Task RefreshDataAsync()
        {
            NoConnection.Visibility = Visibility.Hidden;
            RefreshDataIcon.Visibility = Visibility.Visible;
            dvm = new DashboardViewModel();

            bool status = await Task.Run(() => dvm.IsServerConnected());

            if (status == true) //Connection Found
            {
                pk = await Task.Run(() => dvm.GetParkingLots());
                //FillInfo();
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

        /*
         * Method: FillDataAsync()
         * Input: User Clicks to update database
         * Output: None
         * Purpose: Checks for a connection before parsing the parking lots and filling info
         *          while sending user information on the pending status of refreshing the data
         */
        private async Task FillDataAsync()
        {
            NoConnection.Visibility = Visibility.Hidden;
            LoadingData.Visibility = Visibility.Visible;
            dvm = new DashboardViewModel();

            bool status = await Task.Run(() => dvm.IsServerConnected());

            if (status == true) //Connection Found
            {
                pk = await Task.Run(() => dvm.GetParkingLots());
                FillInfo();
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
        #endregion

        #region Status Display
        private void connectionStatus(bool status)
        {
            //On
            if (status == true)
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
            if (status == true)
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
            if (status == true)
            {
                trackingStatusIcon.Foreground = Brushes.Green;
            }
            else //Off
            {
                trackingStatusIcon.Foreground = Brushes.Red;
            }
        }
        #endregion
    }
}
