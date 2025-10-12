using System;
using System.Data.Linq;

class Program
{
    static void Main(string[] args)
    {
        var context = new UberStrike.DataCenter.DataAccess.UberstrikeDataContext();
        
        Console.WriteLine("Checking if database exists...");
        if (context.DatabaseExists())
        {
            Console.WriteLine("Database already exists...");
        }
        else
        {
            Console.WriteLine("Creating database and schema...");
            context.CreateDatabase();
            Console.WriteLine("Database and schema created.");
        }
    }
}
