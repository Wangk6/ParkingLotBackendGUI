using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleDetectionProject.Database
{
    public class DataAccess
    {
        //Used to query specific parking lot information 
        public List<ParkingLot> GetParkingLot(string lotName, string lotNum)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(SQLConnection.ConnString("ParkingLotDB")))
            {
                return connection.Query<ParkingLot>($"SELECT * FROM viewParkingLotInfo WHERE LotName = '" + lotName + "' AND (LotNumber = '" + lotNum + "' OR LotNumber IS NULL)").ToList();
            }
        }

        //Used to query all parking lot information 
        public List<ParkingLot> GetAllParkingLot()
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(SQLConnection.ConnString("ParkingLotDB")))
            {
                try
                {
                    return connection.Query<ParkingLot>($"SELECT * FROM viewParkingLotInfo").ToList();
                }
                catch(Exception ex)
                {

                }
                return null;
            }
        }

        //Used to query all parking lot status messages information 
        public List<ParkingLot> GetStatusMessage()
        {
            using (IDbConnection connection = new SqlConnection(SQLConnection.ConnString("ParkingLotDB")))
            {
                try
                {
                    return connection.Query<ParkingLot>($"SELECT * FROM viewParkingLotMessages").ToList();
                }
                catch (Exception ex)
                {

                }
                return null;
            }
        }

        //Used to update camera url for parking lot
        public void UpdateCameraURL(int parkingLotID, string cameraURL)
        {
            using (SqlConnection connection = new SqlConnection(SQLConnection.ConnString("ParkingLotDB")))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand("spUpdateCameraURL", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@ParkingLotID", parkingLotID));
                        command.Parameters.Add(new SqlParameter("@CameraURL", cameraURL));
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }        
        
        //Used to update number of cars parked for parking lot
        public void LotInfoParkedCars(int parkingLotID, int carsParked)
        {
            using (SqlConnection connection = new SqlConnection(SQLConnection.ConnString("ParkingLotDB")))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand("spLotInfoParkedCars", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@ParkingLotID", parkingLotID));
                        command.Parameters.Add(new SqlParameter("@CarsParked", carsParked));
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        //Used to update max capacity and permit type for parking lot
        public void ParkingLotInfo(int parkingLotID, int? maxCapacity, string permitType)
        {
            using (SqlConnection connection = new SqlConnection(SQLConnection.ConnString("ParkingLotDB")))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand("spParkingLotInfo", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@ParkingLotID", parkingLotID));
                        command.Parameters.Add(new SqlParameter("@MaxCapacity", maxCapacity));
                        command.Parameters.Add(new SqlParameter("@PermitType", permitType));
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
