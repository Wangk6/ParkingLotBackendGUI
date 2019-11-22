using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleDetectionProject.Database;

namespace VehicleDetectionProject.ViewModel
{
    public class ConfigureViewModel
    {
        DataAccess db = new DataAccess();

        public List<ParkingLot> GetParkingLots()
        {
            return db.GetAllParkingLot();
        }

        public List<ParkingLot> GetStatusMessage()
        {
            return db.GetStatusMessage();
        }

        public void UpdateCameraURL(int parkingLotID, string cameraURL)
        {
            db.UpdateCameraURL(parkingLotID, cameraURL);
        }

        public void LotInfoParkedCars(int parkingLotID, int carsParked)
        {
            db.LotInfoParkedCars(parkingLotID, carsParked);
        }

        public void ParkingLotInfo(int parkingLotID, int? maxCapacity, string permitType)
        {       
         db.ParkingLotInfo(parkingLotID, maxCapacity, permitType);
        }
    }
}
