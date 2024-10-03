using Models;

namespace TestDBContext
{
	internal class Program
	{
		static void Main(string[] args)
		{
			var context = new DBContext();

			context.Database.EnsureDeleted();
			Console.WriteLine("Database deleted.");
			context.Database.EnsureCreated();
			Console.WriteLine("Database created.");

			Console.WriteLine("Exiting...");
		}
	}
}
