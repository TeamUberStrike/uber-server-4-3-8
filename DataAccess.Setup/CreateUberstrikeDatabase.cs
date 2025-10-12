using System;
using System.Data.Linq;

class Program
{
    static void Main(string[] args)
    {
        // Replace with your actual connection string
        string connectionString = "Data Source=DESKTOP-LNSADFU;Initial Catalog=MvParadisePaintball4;User ID=sa;Password=cmune$1";
        var context = new UberStrike.DataCenter.DataAccess.UberstrikeDataContext(connectionString);
        
        Console.WriteLine("Checking if database exists...");
        if (context.DatabaseExists())
        {
            Console.WriteLine("Database already exists. Checking if schema needs to be created...");
            // You could add logic here to check for specific tables or drop/recreate if needed
            Console.WriteLine("✅ Database exists. Schema creation skipped.");
            Console.WriteLine("If you need to recreate the schema, drop the tables first or use a different database name.");
        }
        else
        {
            Console.WriteLine("Creating database and schema...");
            context.CreateDatabase();
            Console.WriteLine("✅ Database and schema created.");
        }
    }
}
