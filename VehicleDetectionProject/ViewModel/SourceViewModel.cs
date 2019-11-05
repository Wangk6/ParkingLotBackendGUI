using System;
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

        public List<ParkingLot> GetParkingLots()
        {
            return db.GetAllParkingLot();
        }
    }
}
