﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleDetectionProject.Database;

namespace VehicleDetectionProject.ViewModel
{
    public class SourceViewModel
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

        public void UpdateCameraURL(int parkingLotID, string cameraURL)
        {
            db.UpdateCameraURL(parkingLotID, cameraURL);
        }
    }
}
