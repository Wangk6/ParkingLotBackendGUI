using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace VehicleDetectionProject.Database
{
    /*
    * Class: LotActivity
    * Created By: Kevin Wang
    * Purpose: Model utilizing Dapper to match column names in database
    */
    public class LotActivity
    {
        public string LotActivityTime { get; set; }

        public int ParkingLotID { get; set; }

        public char CarParked { get; set; }

        public string LotName { get; set; }

        public string LotNumber { get; set; }


        public override string ToString()
        {

            return string.Format("{0} {1}", LotName, LotNumber);
        }
    }
}
