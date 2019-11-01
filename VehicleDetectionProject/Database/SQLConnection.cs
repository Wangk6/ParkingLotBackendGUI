using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleDetectionProject.Database
{
    public static class SQLConnection
    {
        public static string ConnString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        //https://docs.microsoft.com/en-us/azure/sql-database/sql-database-connect-query-dotnet-visual-studio

    }
}
