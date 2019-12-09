﻿using System;
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

        private void buttonAddCamera_Click(object sender, RoutedEventArgs e)
        {
            if (!NoConnection.IsVisible) //There is a connection
            {
                try
                {
                    int index = comboBoxParkingLot.SelectedIndex + 1;
                    string date = textBoxDate.Text.Trim();
                    //Update parking lots camera url
                    la = svm.GetLotRecordDate(index, date);
                    RefreshDataASync();
                }
                catch (ArgumentOutOfRangeException ex) { };
            }
        }

        //Add Refresh when inserting/updating camera url to database is complete
        //Clears the information previously and adds up-to-date data
        private void FillInfoASync()
        {
            try
            {
                //Add to comboBoxParkingLot combobox
                comboBoxParkingLot.ItemsSource = pk;
                listViewParkingLot.ItemsSource = la;
            }
            catch(Exception e)
            {

            }
        }

        private void buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshDataASync();
        }

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

                FillInfoASync();
                LoadingData.Visibility = Visibility.Hidden;
            }
            else //No Connection
            {
                NoConnection.Visibility = Visibility.Visible;
                LoadingData.Visibility = Visibility.Hidden;
                Console.WriteLine("Not Connected");
            }
        }

        private async Task RefreshDataASync()
        {
            NoConnection.Visibility = Visibility.Hidden;
            RefreshDataIcon.Visibility = Visibility.Visible;
            svm = new ParkingDataViewModel();

            bool status = await Task.Run(() => svm.IsServerConnected());

            if (status == true) //Connection Established
            {
                pk = await Task.Run(() => svm.GetParkingLots());
                FillInfoASync();
                RefreshDataIcon.Visibility = Visibility.Hidden;
            }
            else //No Connection
            {
                NoConnection.Visibility = Visibility.Visible;
                RefreshDataIcon.Visibility = Visibility.Hidden;
                Console.WriteLine("Not Connected");
            }
        }

        //Select Date Click
        private void buttonAddDate_Click(object sender, RoutedEventArgs e)
        {
            //Set Calendar date to Date
            textBoxDate.Text = CalendarView.SelectedDate.Value.Date.ToShortDateString();
            //Hide Calendar
            CalendarGrid.Visibility = Visibility.Hidden;
        }

        private void Click_Date(object sender, MouseButtonEventArgs e)
        {
            CalendarGrid.Visibility = Visibility.Visible;
        }
    }
}
