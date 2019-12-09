using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleDetectionProject.Database;

namespace VehicleDetectionProject.ViewModel
{
    public class ParkingDataViewModel
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

       public List<LotActivity> GetLotRecordDate(int parkingLot, string date)
        {
            return db.GetLotRecordDate(parkingLot, date);
        }

       public List<LotActivity> GetAllParkingRecords()
        {
            return db.GetAllParkingRecords();
        }
    }
}
