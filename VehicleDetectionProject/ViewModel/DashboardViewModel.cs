﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using VehicleDetectionProject.Database;

namespace VehicleDetectionProject.ViewModel
{
    public class DashboardViewModel
    {
        DataAccess db = new DataAccess();

        public bool IsServerConnected()
        {
            return db.IsServerConnected();
        }

        public List<ParkingLot> GetParkingLots()
        {
            return db.GetAllParkingLot();
        }

        public string ParkingLotStatusLongDisplay(char status)
        {
            return status == 'Y' ? "Open" : "Closed";
        }

        public void CarDidEnter(int parkingID)
        {
            db.CarParked(parkingID);
        }

        public void CarDidLeave(int parkingID)
        {
            db.CarLeft(parkingID);
        }
    }
}
