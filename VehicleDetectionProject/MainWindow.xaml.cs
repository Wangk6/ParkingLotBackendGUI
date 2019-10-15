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
using VehicleDetectionProject.ViewModel;

namespace VehicleDetectionProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Change status, do [name.Foreground] for pack icons
    

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            btnOpenMenu.Visibility = Visibility.Collapsed;
            btnCloseMenu.Visibility = Visibility.Visible;
        }

        private void btnCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            btnOpenMenu.Visibility = Visibility.Visible;
            btnCloseMenu.Visibility = Visibility.Collapsed;
        }

        private void DashboardView_Clicked(object sender, RoutedEventArgs e)
        {
            DataContext = new DashboardViewModel();
        }
        
        private void ControlView_Clicked(object sender, RoutedEventArgs e)
        {
            DataContext = new ControlsViewModel();
        }

        private void ConfigureView_Clicked(object sender, RoutedEventArgs e)
        {
            DataContext = new ConfigureViewModel();

        }

        private void ListViewItem_Selected(object sender, RoutedEventArgs e)
        {

        }
    }
}