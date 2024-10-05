using Models;
using Models.Entities;
using Repositories;

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

			Console.WriteLine("Adding user data...");
			AddUserData(context).Wait();

			Console.WriteLine("Adding dish category data...");
			AddDishCategoryData(context).Wait();

			Console.WriteLine("Adding dish data...");
			AddDishData(context).Wait();

			Console.WriteLine("Adding table data...");
			AddTableData(context).Wait();

			Console.WriteLine("Exiting...");
		}

		static async Task AddUserData(DBContext context)
		{
			List<User> users = new List<User>
			{
				new User { Username = "user1", Password = "password1", FullName = "John Doe", Email = "JohnDoe@gmail.com", Phone = "0123456789", Role = Role.Manager, Salary = 2000},
				new User { Username = "user2", Password = "password2", FullName = "Jane Doe", Email = "JaneDoe@gmail.com", Phone = "0123456789", Role = Role.Chef, Salary = 1500},
				new User { Username = "user3", Password = "password3", FullName = "John Smith", Email = "JohnSmith@gmail.com", Phone = "0123456789", Role = Role.Waitstaff, Salary = 1200},
				new User { Username = "user4", Password = "password4", FullName = "Jane Smith", Email = "JaneSmith@gmail.com", Phone = "0123456789", Role = Role.Accountant, Salary= 1800},
				new User { Username = "user5", Password = "password5", FullName = "John Brown", Email = "JohnBrown@gmail.com", Phone = "0123456789", Role = Role.Customer},
			};

			UserRepository userRepository = new UserRepository(context);
			foreach (var user in users)
			{
				await userRepository.InsertAsync(user);
			}

			Console.WriteLine("User data added.");
		}

		static async Task AddDishCategoryData(DBContext context)
		{
			List<DishCategory> dishCategories = new List<DishCategory>
			{
				new DishCategory { CatName = "Appetizer" },
				new DishCategory { CatName = "Main Course" },
				new DishCategory { CatName = "Dessert" },
				new DishCategory { CatName = "Drink" },
			};

			DishCategoryRepository dishCategoryRepository = new DishCategoryRepository(context);
			foreach (var dishCategory in dishCategories)
			{
				await dishCategoryRepository.InsertAsync(dishCategory);
			}

			Console.WriteLine("Dish category data added.");
		}

		static async Task AddDishData(DBContext context)
		{
			List<Dish> dishes = new List<Dish>
			{
				new Dish { DishName = "Spring Rolls", Price = 5.99m, CategoryId = 1, Description = "Fried or fresh" },
				new Dish { DishName = "Pho", Price = 9.99m, CategoryId = 2, Description = "Beef or chicken" },
				new Dish { DishName = "Banh Mi", Price = 7.99m, CategoryId = 2, Description = "Pork or chicken" },
				new Dish { DishName = "Bun Bo Hue", Price = 10.99m, CategoryId = 2 },
				new Dish { DishName = "Banh Xeo", Price = 8.99m, CategoryId = 2 },
				new Dish { DishName = "Banh Canh", Price = 8.99m, CategoryId = 2 },
				new Dish { DishName = "Bun Rieu", Price = 9.99m, CategoryId = 2 },
				new Dish { DishName = "Bun Thit Nuong", Price = 9.99m, CategoryId = 2 },
				new Dish { DishName = "Bun Cha", Price = 9.99m, CategoryId = 2 },
				new Dish { DishName = "Bun Bo Nam Bo", Price = 9.99m, CategoryId = 2 },
				new Dish { DishName = "Bun Bo Xao", Price = 9.99m, CategoryId = 2 },
				new Dish { DishName = "Water", Price = 1.99m, CategoryId = 4, Description = "Bottled or tap" },
				new Dish { DishName = "Soda", Price = 2.99m, CategoryId = 4, Description = "Coke, Pepsi, Sprite" },
				new Dish { DishName = "Beer", Price = 3.99m, CategoryId = 4 },
				new Dish { DishName = "Wine", Price = 4.99m, CategoryId = 4 },
				new Dish { DishName = "Cheesecake", Price = 4.99m, CategoryId = 3, Description = "Strawberry or blueberry" },
				new Dish { DishName = "Tiramisu", Price = 4.99m, CategoryId = 3, Description = "Coffee or chocolate" },
				new Dish { DishName = "Creme Brulee", Price = 4.99m, CategoryId = 3 },
				new Dish { DishName = "Chocolate Cake", Price = 4.99m, CategoryId = 3 },
			};

			DishRepository dishRepository = new DishRepository(context);
			foreach (var dish in dishes)
			{
				await dishRepository.InsertAsync(dish);
			}

			Console.WriteLine("Dish data added.");
		}

		static async Task AddTableData(DBContext context)
		{
			List<Table> tables = new List<Table>
			{
				new Table { TableName = "Table 1", Capacity = 2, Status = TableStatus.Available },
				new Table { TableName = "Table 2", Capacity = 4, Status = TableStatus.Available },
				new Table { TableName = "Table 3", Capacity = 6, Status = TableStatus.Available },
				new Table { TableName = "Table 4", Capacity = 8, Status = TableStatus.Available },
				new Table { TableName = "Table 5", Capacity = 10, Status = TableStatus.Available },
				new Table { TableName = "Table 6", Capacity = 2, Status = TableStatus.Unavailable },
			};

			TableRepository tableRepository = new TableRepository(context);
			foreach (var table in tables)
			{
				await tableRepository.InsertAsync(table);
			}

			Console.WriteLine("Table data added.");
		}

		static async Task AddReservationData(DBContext context)
		{
			List<Reservation> reservations = new List<Reservation>
			{
				new Reservation { CustomerId = 5, PartySize = 3, ResDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)), ResTime = TimeOnly.FromDateTime(DateTime.Now.AddHours(2))},
				new Reservation { CustomerId = 5, PartySize = 5, ResDate = DateOnly.FromDateTime(DateTime.Now), ResTime = TimeOnly.FromDateTime(DateTime.Now.AddHours(3))},
				new Reservation { CustomerId = 5, PartySize = 7, ResDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)), ResTime = TimeOnly.FromDateTime(DateTime.Now.AddHours(4))},

			};

			ReservationRepository reservationRepository = new ReservationRepository(context);
			foreach (var reservation in reservations)
			{
				await reservationRepository.InsertAsync(reservation);
			}

			Console.WriteLine("Reservation data added.");
		}
	}
}