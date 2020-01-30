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
    /*
    * Class: ParkingDataView
    * Created By: Kevin Wang
    * Purpose: Controls for the ParkingDataView View Tab
    */
    /// <summary>
    /// Interaction logic for Configure.xaml
    /// </summary>
    public partial class ParkingDataView : UserControl
    {
        List<ParkingLot> pk = new List<ParkingLot>();
        List<LotActivity> la = new List<LotActivity>();
        ParkingDataViewModel svm;

        public ParkingDataView()
        {
            InitializeComponent();
        }

        private void ParkingDataView_Loaded(object sender, RoutedEventArgs e)
        {
            FillDataAsync();
        }

        #region View Controller
        /*
         * Method: buttonViewHistory_Click()
         * Input: User Clicks Confirm
         * Output: None -
         * Purpose: Gets the history of selected date by user
         */
        private void buttonViewHistory_Click(object sender, RoutedEventArgs e)
        {
            if (!NoConnection.IsVisible) //There is a connection
            {
                try
                {
                    if(textBoxDate.Text == "") //Date is empty
                    {
                        int index = comboBoxParkingLot.SelectedIndex + 1;
                        string date = textBoxDate.Text.Trim();
                        //Update parking lots camera url
                        la = svm.GetLotRecordDate(index, date);
                        RefreshDataASync();
                    }
                    else if(comboBoxParkingLot.Text == "")
                    {
                        string date = textBoxDate.Text.Trim();
                        //Update parking lots camera url
                        la = svm.GetLotRecordDate(-1, date);
                        textBoxDate.Clear();
                        RefreshDataASync();
                    }else if(textBoxDate.Text != "" && comboBoxParkingLot.Text != "")
                    {
                        int index = comboBoxParkingLot.SelectedIndex + 1;
                        string date = textBoxDate.Text.Trim();
                        //Update parking lots camera url
                        la = svm.GetLotRecordDate(index, date);
                        textBoxDate.Clear();
                        RefreshDataASync();
                    }
                }
                catch (ArgumentOutOfRangeException ex) { };
            }
        }

        /*
         * Method: buttonAddDate_Click()
         * Input: User Clicks Date Confirm
         * Output: None -
         * Purpose: Gets the selected calendar date, puts in textbox and closes calendar view
         */
        private void buttonAddDate_Click(object sender, RoutedEventArgs e)
        {
            //Set Calendar date to Date
            try
            {
                textBoxDate.Text = CalendarView.SelectedDate.Value.Date.ToShortDateString();
            }
            catch(Exception)
            {
                //Text empty
            }
            //Hide Calendar
            CalendarGrid.Visibility = Visibility.Hidden;
        }

        /*
         * Method: Click_Date()
         * Input: User Clicks Date textBox
         * Output: None -
         * Purpose: Opens up a Calendar for user to click and select a date
         */
        private void Click_Date(object sender, MouseButtonEventArgs e)
        {
            CalendarGrid.Visibility = Visibility.Visible;
        }

        /*
         * Method: buttonRefresh_Click()
         * Input: User clicks Refresh Button
         * Output: None -
         * Purpose: Calls RefreshDataASync() to get updated database information
         */
        private void buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
            comboBoxParkingLot.Text = "";
            textBoxDate.Text = "";
            FillDataAsync();
        }
        #endregion

        #region Getting/Setting/Clearing DB Data
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
                //Add to comboBoxParkingLot combobox
                comboBoxParkingLot.ItemsSource = pk;
                listViewParkingLot.ItemsSource = la;
            }
            catch (Exception e)
            {

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
            svm = new ParkingDataViewModel();

            bool status = await Task.Run(() => svm.IsServerConnected());

            if (status == true) //Connection Established
            {
                pk = svm.GetParkingLots();
                la = svm.GetAllParkingRecords();

                FillInfo();
                LoadingData.Visibility = Visibility.Hidden;
            }
            else //No Connection
            {
                NoConnection.Visibility = Visibility.Visible;
                LoadingData.Visibility = Visibility.Hidden;
                Console.WriteLine("Not Connected");
            }
        }

        /*
         * Method: RefreshDataASync()
         * Input: User Clicks Refresh Database button
         * Output: None
         * Purpose: Checks for a connection before parsing the parking lots and refreshing info
         *          while sending user information on the pending status of refreshing the data
         */
        private async Task RefreshDataASync()
        {
            NoConnection.Visibility = Visibility.Hidden;
            RefreshDataIcon.Visibility = Visibility.Visible;
            //svm = new ParkingDataViewModel();

            bool status = await Task.Run(() => svm.IsServerConnected());

            if (status == true) //Connection Established
            {
                pk = await Task.Run(() => svm.GetParkingLots());
                FillInfo();
                RefreshDataIcon.Visibility = Visibility.Hidden;
            }
            else //No Connection
            {
                NoConnection.Visibility = Visibility.Visible;
                RefreshDataIcon.Visibility = Visibility.Hidden;
                Console.WriteLine("Not Connected");
            }
        }
        #endregion
    }
}
