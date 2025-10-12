using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SqlClient;

class Program
{
    static void Main(string[] args)
    {
        var databases = new Dictionary<string, DataContext>
        {
            { "MvParadisePaintball", new UberStrike.DataCenter.DataAccess.UberstrikeDataContext() },
            { "MvParadisePaintballForum", new Cmune.DataCenter.Forum.DataAccess.ForumDataContext() },
            { "Cmune", new Cmune.DataCenter.DataAccess.CmuneDataContext() },
            { "CmuneMonitoring", new Cmune.Instrumentation.Monitoring.DataAccess.CmuneMonitoringDataContext() },
            { "Instrumentation", new Cmune.Instrumentation.DataAccess.InstrumentationDataContext() },
            { "aspnetdb", new Cmune.Instrumentation.DataAccess.InstrumentationDataContext() }
        };

        foreach (var eachDatabase in databases)
        {
            string databaseName = eachDatabase.Key;
            var database = eachDatabase.Value;

            Console.WriteLine($"Creating database and schema for {databaseName}...");
            try
            {
                database.CreateDatabase();
                Console.WriteLine($"Database creation is successful for {databaseName}.");
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Database creation failed for {databaseName}");
            }
        }
    }
}
