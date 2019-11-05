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
        List<ParkingLot> pk = new List<ParkingLot>();

        public ConfigureViewModel()
        {
            pk = db.GetAllParkingLot();
        }

        public List<ParkingLot> GetParkingLots()
        {
            return pk;
        }
    }
}
