using System;
using System.Data.Linq;
using System.Data.SqlClient;

class Program
{
    static void Main(string[] args)
    {
        var context = new UberStrike.DataCenter.DataAccess.UberstrikeDataContext();
        
        Console.WriteLine("Creating database and schema...");
        try
        {
            context.CreateDatabase();
            Console.WriteLine("Database and schema created successfully.");
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"Database creation failed: {ex.Message}");
        }
    }
}
