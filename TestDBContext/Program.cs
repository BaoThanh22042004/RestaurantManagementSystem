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

            context.ResetDatabase();
            Console.WriteLine("Database reset.");

            Console.WriteLine("Adding user data...");
            AddUserData(context).Wait();

            Console.WriteLine("Adding dish category data...");
            AddDishCategoryData(context).Wait();

            Console.WriteLine("Adding dish data...");
            AddDishData(context).Wait();

            Console.WriteLine("Adding storage data...");
            AddStorageData(context).Wait();

            Console.WriteLine("Adding storage log data...");
            AddStorageLogData(context).Wait();

            Console.WriteLine("Adding table data...");
            AddTableData(context).Wait();

            Console.WriteLine("Adding shift data...");
            AddShiftData(context).Wait();

            Console.WriteLine("Adding schedule data...");
            AddScheduleData(context).Wait();

            Console.WriteLine("Adding attendance data...");
            AddAttendanceData(context).Wait();

            Console.WriteLine("Adding reservation data...");
            AddReservationData(context).Wait();

            Console.WriteLine("Adding feedback data...");
            AddFeedbackData(context).Wait();

            Console.WriteLine("Adding payroll data...");
            AddPayrollData(context).Wait();

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
                new DishCategory { CatName = "Baked Dishes" },
                new DishCategory { CatName = "Beverages" },
                new DishCategory { CatName = "Desserts" },
                new DishCategory { CatName = "Main Dishes" },
                new DishCategory { CatName = "Pizza" },
                new DishCategory { CatName = "Side Dishes"}
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
             new Dish { DishName = "Bruschetta", Price = 8.00m, CategoryId = 1, Description = "Grilled bread topped with fresh tomatoes, garlic, and olive oil, often garnished with basil." },
             new Dish { DishName = "Caesar Salad", Price = 10.00m, CategoryId = 1, Description = "Romaine lettuce with Caesar dressing, Parmesan cheese, and crunchy croutons." },
             new Dish { DishName = "Stuffed Mushrooms", Price = 9.00m, CategoryId = 1, Description = "Mushrooms stuffed with ground meat and cheese, baked until golden." },
             new Dish { DishName = "Onion Rings", Price = 7.00m, CategoryId = 1, Description = "Sliced onions battered and deep-fried, typically served with ketchup or mayo." },
             new Dish { DishName = "Spinach Artichoke Dip", Price = 11.00m, CategoryId = 1, Description = "A creamy dip made with spinach and artichokes, served with bread or tortilla chips." },
             new Dish { DishName = "Steak", Price = 25.00m, CategoryId = 3, Description = "Grilled beef cooked to your liking, usually served with mashed potatoes and vegetables." },
             new Dish { DishName = "Pasta Carbonara", Price = 15.00m, CategoryId = 3, Description = "Spaghetti with a creamy sauce made from eggs, bacon, and Parmesan cheese." },
             new Dish { DishName = "Chicken Alfredo", Price = 14.00m, CategoryId = 3, Description = "Fettuccine pasta in a creamy sauce with grilled chicken, often topped with Parmesan cheese." },
             new Dish { DishName = "Seafood Paella", Price = 20.00m, CategoryId = 3, Description = "Rice cooked with fresh seafood and saffron, a signature dish from Spain." },
             new Dish { DishName = "Ratatouille", Price = 12.00m, CategoryId = 3, Description = "A French vegetable stew made with zucchini, tomatoes, and bell peppers, often served with bread." },
             new Dish { DishName = "Margherita Pizza", Price = 12.00m, CategoryId = 4, Description = "Pizza topped with tomato sauce, mozzarella cheese, and fresh basil." },
             new Dish { DishName = "Pepperoni Pizza", Price = 14.00m, CategoryId = 4, Description = "Pizza topped with tomato sauce, mozzarella, and spicy pepperoni sausage." },
             new Dish { DishName = "Vegetarian Pizza", Price = 13.00m, CategoryId = 4, Description = "Pizza topped with fresh vegetables such as bell peppers, mushrooms, and olives, without meat." },
             new Dish { DishName = "Lasagna", Price = 16.00m, CategoryId = 5, Description = "Layered pasta with meat, cheese, and tomato sauce, baked until bubbly." },
             new Dish { DishName = "Beef Wellington", Price = 28.00m, CategoryId = 5, Description = "Beef wrapped in pastry dough with mushrooms, a luxurious dish perfect for special occasions." },
             new Dish { DishName = "Quiche Lorraine", Price = 10.00m, CategoryId = 5, Description = "A French egg pie filled with bacon, cheese, and onions." },
             new Dish { DishName = "Tiramisu", Price = 7.00m, CategoryId = 2, Description = "An Italian dessert made from coffee-soaked ladyfingers layered with mascarpone cheese and cocoa." },
             new Dish { DishName = "Cheesecake", Price = 6.00m, CategoryId = 2, Description = "A sweet dessert made with cream cheese, often served with a crust made from cookies or graham crackers." },
             new Dish { DishName = "Crème Brûlée", Price = 8.00m, CategoryId = 2, Description = "A vanilla custard dessert topped with a hard caramelized sugar layer." },
             new Dish { DishName = "Brownies", Price = 5.00m, CategoryId = 2, Description = "Rich and fudgy chocolate dessert bars, often served with ice cream on top." },
             new Dish { DishName = "Margarita", Price = 10.00m, CategoryId = 6, Description = "A cocktail made with tequila, lime juice, and triple sec, usually served with a salted rim." },
             new Dish { DishName = "Mojito", Price = 9.00m, CategoryId = 6, Description = "A refreshing cocktail made with rum, lime juice, sugar, and mint." },
             new Dish { DishName = "Pina Colada", Price = 11.00m, CategoryId = 6, Description = "A tropical cocktail made with rum, pineapple juice, and coconut cream." },
             new Dish { DishName = "Wine", Price = 8.00m, CategoryId = 6, Description = "Choice of red, white, or rosé wine, to complement your meal." },
             new Dish { DishName = "Coffee", Price = 3.00m, CategoryId = 6, Description = "Espresso, cappuccino, or latte, a popular beverage to enjoy with any meal." },
             new Dish { DishName = "Garlic Bread", Price = 5.00m, CategoryId = 7, Description = "Bread topped with garlic and butter, baked until crispy, often served with pasta or main dishes." },
             new Dish { DishName = "Coleslaw", Price = 4.00m, CategoryId = 7, Description = "Shredded cabbage salad mixed with mayonnaise dressing, a popular side dish." },
             new Dish { DishName = "French Fries", Price = 4.00m, CategoryId = 7, Description = "Crispy deep-fried potatoes, usually served with ketchup or mayo." },
             new Dish { DishName = "Mashed Potatoes", Price = 5.00m, CategoryId = 7, Description = "Creamy mashed potatoes made with butter and cream, a perfect side for any main dish." },
             new Dish { DishName = "Vegetable Medley", Price = 6.00m, CategoryId = 7, Description = "A mix of sautéed or steamed vegetables, a healthy complement to your meal." }
            };


            DishRepository dishRepository = new DishRepository(context);
            foreach (var dish in dishes)
            {
                await dishRepository.InsertAsync(dish);
            }

            Console.WriteLine("Dish data added.");
        }

        static async Task AddStorageData(DBContext context)
        {
            List<Storage> items = new List<Storage>
            {
            new Storage { ItemName = "Bruschetta", Unit = "piece", Quantity = 40 },
            new Storage { ItemName = "Caesar Salad", Unit = "bowl", Quantity = 25 },
            new Storage { ItemName = "Stuffed Mushrooms", Unit = "piece", Quantity = 25 },
            new Storage { ItemName = "Onion Rings", Unit = "piece", Quantity = 40 },
            new Storage { ItemName = "Spinach Artichoke Dip", Unit = "bowl", Quantity = 15 },
            new Storage { ItemName = "Steak", Unit = "kg", Quantity = 75 },
            new Storage { ItemName = "Pasta Carbonara", Unit = "kg", Quantity = 60 },
            new Storage { ItemName = "Chicken Alfredo", Unit = "kg", Quantity = 60 },
            new Storage { ItemName = "Seafood Paella", Unit = "kg", Quantity = 50 },
            new Storage { ItemName = "Ratatouille", Unit = "kg", Quantity = 40 },
            };

            StorageRepository storageRepository = new StorageRepository(context);
            foreach (var item in items)
            {
                await storageRepository.InsertAsync(item);
            }

            Console.WriteLine("Item data added.");
        }

        static async Task AddStorageLogData(DBContext context)
        {
            int currentUserId = 1;

            List<StorageLog> logs = new List<StorageLog>
    {
        new StorageLog
        {
            CreatedBy = currentUserId,
            CreatedAt = DateTime.Now,
            ItemId = 1,
            ChangeQuantity = 50,
            RemainQuantity = 50,
            Action = Models.Entities.Action.Import,
            Cost = 200.00m,
            Description = "Imported 50 Bruschetta"
        },
        new StorageLog
        {
            CreatedBy = currentUserId,
            CreatedAt = DateTime.Now,
            ItemId = 1,
            ChangeQuantity = 10,
            RemainQuantity = 40,
            Action = Models.Entities.Action.Export,
            Description = "Exported 10 Bruschetta for an event"
        },
        new StorageLog
        {
            CreatedBy = currentUserId,
            CreatedAt = DateTime.Now,
            ItemId = 2,
            ChangeQuantity = 30,
            RemainQuantity = 30,
            Action = Models.Entities.Action.Import,
            Cost = 90.00m,
            Description = "Imported 30 Caesar Salads"
        },
        new StorageLog
        {
            CreatedBy = currentUserId,
            CreatedAt = DateTime.Now,
            ItemId = 2,
            ChangeQuantity = 5,
            RemainQuantity = 25,
            Action = Models.Entities.Action.Export,
            Description = "Exported 5 Caesar Salads for delivery"
        },
        new StorageLog
        {
            CreatedBy = currentUserId,
            CreatedAt = DateTime.Now,
            ItemId = 3,
            ChangeQuantity = 40,
            RemainQuantity = 40,
            Action = Models.Entities.Action.Import,
            Cost = 120.00m,
            Description = "Imported 40 Stuffed Mushrooms"
        },
        new StorageLog
        {
            CreatedBy = currentUserId,
            CreatedAt = DateTime.Now,
            ItemId = 3,
            ChangeQuantity = 15,
            RemainQuantity = 25,
            Action = Models.Entities.Action.Export,
            Description = "Exported 15 Stuffed Mushrooms to the kitchen"
        },
        new StorageLog
        {
            CreatedBy = currentUserId,
            CreatedAt = DateTime.Now,
            ItemId = 4,
            ChangeQuantity = 60,
            RemainQuantity = 60,
            Action = Models.Entities.Action.Import,
            Cost = 90.00m,
            Description = "Imported 60 Onion Rings"
        },
        new StorageLog
        {
            CreatedBy = currentUserId,
            CreatedAt = DateTime.Now,
            ItemId = 4,
            ChangeQuantity = 20,
            RemainQuantity = 40,
            Action = Models.Entities.Action.Export,
            Description = "Exported 20 Onion Rings for a large order"
        },
        new StorageLog
        {
            CreatedBy = currentUserId,
            CreatedAt = DateTime.Now,
            ItemId = 5,
            ChangeQuantity = 25,
            RemainQuantity = 25,
            Action = Models.Entities.Action.Import,
            Cost = 80.00m,
            Description = "Imported 25 Spinach Artichoke Dips"
        },
        new StorageLog
        {
            CreatedBy = currentUserId,
            CreatedAt = DateTime.Now,
            ItemId = 5,
            ChangeQuantity = 10,
            RemainQuantity = 15,
            Action = Models.Entities.Action.Export,
            Description = "Exported 10 Spinach Artichoke Dips for an event"
        },
        new StorageLog
        {
            CreatedBy = currentUserId,
            CreatedAt = DateTime.Now,
            ItemId = 6,
            ChangeQuantity = 100,
            RemainQuantity = 100,
            Action = Models.Entities.Action.Import,
            Cost = 300.00m,
            Description = "Imported 100 kg of Steak"
        },
        new StorageLog
        {
            CreatedBy = currentUserId,
            CreatedAt = DateTime.Now,
            ItemId = 6,
            ChangeQuantity = 25,
            RemainQuantity = 75,
            Action = Models.Entities.Action.Export,
            Description = "Exported 25 kg of Steak for a special order"
        },
        new StorageLog
        {
            CreatedBy = currentUserId,
            CreatedAt = DateTime.Now,
            ItemId = 7,
            ChangeQuantity = 80,
            RemainQuantity = 80,
            Action = Models.Entities.Action.Import,
            Cost = 160.00m,
            Description = "Imported 80 kg of Pasta Carbonara"
        },
        new StorageLog
        {
            CreatedBy = currentUserId,
            CreatedAt = DateTime.Now,
            ItemId = 7,
            ChangeQuantity = 20,
            RemainQuantity = 60,
            Action = Models.Entities.Action.Export,
            Description = "Exported 20 kg of Pasta Carbonara for a catering event"
        },
        new StorageLog
        {
            CreatedBy = currentUserId,
            CreatedAt = DateTime.Now,
            ItemId = 8,
            ChangeQuantity = 90,
            RemainQuantity = 90,
            Action = Models.Entities.Action.Import,
            Cost = 270.00m,
            Description = "Imported 90 kg of Chicken Alfredo"
        },
        new StorageLog
        {
            CreatedBy = currentUserId,
            CreatedAt = DateTime.Now,
            ItemId = 8,
            ChangeQuantity = 30,
            RemainQuantity = 60,
            Action = Models.Entities.Action.Export,
            Description = "Exported 30 kg of Chicken Alfredo for banquet"
        },
        new StorageLog
        {
            CreatedBy = currentUserId,
            CreatedAt = DateTime.Now,
            ItemId = 9,
            ChangeQuantity = 75,
            RemainQuantity = 75,
            Action = Models.Entities.Action.Import,
            Cost = 225.00m,
            Description = "Imported 75 kg of Seafood Paella"
        },
        new StorageLog
        {
            CreatedBy = currentUserId,
            CreatedAt = DateTime.Now,
            ItemId = 9,
            ChangeQuantity = 25,
            RemainQuantity = 50,
            Action = Models.Entities.Action.Export,
            Description = "Exported 25 kg of Seafood Paella for a special menu"
        },
        new StorageLog
        {
            CreatedBy = currentUserId,
            CreatedAt = DateTime.Now,
            ItemId = 10,
            ChangeQuantity = 60,
            RemainQuantity = 60,
            Action = Models.Entities.Action.Import,
            Cost = 180.00m,
            Description = "Imported 60 kg of Ratatouille"
        },
        new StorageLog
        {
            CreatedBy = currentUserId,
            CreatedAt = DateTime.Now,
            ItemId = 10,
            ChangeQuantity = 20,
            RemainQuantity = 40,
            Action = Models.Entities.Action.Export,
            Description = "Exported 20 kg of Ratatouille for a customer order"
        },
    };

            StorageLogRepository storageLogRepository = new StorageLogRepository(context);
            foreach (var log in logs)
            {
                await storageLogRepository.InsertAsync(log);
            }

            Console.WriteLine("Item log data added.");
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

        public static async Task AddShiftData(DBContext context)
        {
            List<Shift> shifts = new List<Shift>
            {
                new Shift { ShiftName = "Morning Shift", StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(12, 0) },
                new Shift { ShiftName = "Afternoon Shift", StartTime = new TimeOnly(13, 0), EndTime = new TimeOnly(17, 0) },
                new Shift { ShiftName = "Evening Shift", StartTime = new TimeOnly(18, 0), EndTime = new TimeOnly(22, 0) }
            };

            ShiftRepository shiftRepository = new ShiftRepository(context);
            foreach (var shift in shifts)
            {
                await shiftRepository.InsertAsync(shift);
            }

            Console.WriteLine("Shift data added.");
        }

        public static async Task AddScheduleData(DBContext context)
        {
            List<int> empIds = new List<int> { 1, 2, 3 };
            List<int> shiftIds = new List<int> { 1, 2, 3 };

            List<Schedule> schedules = new List<Schedule>
            {
                new Schedule { ScheDate = DateOnly.FromDateTime(DateTime.Now), EmpId = empIds[0], ShiftId = shiftIds[0] },
                new Schedule { ScheDate = DateOnly.FromDateTime(DateTime.Now).AddDays(-1), EmpId = empIds[1], ShiftId = shiftIds[1] },
                new Schedule { ScheDate = DateOnly.FromDateTime(DateTime.Now).AddDays(-2), EmpId = empIds[2], ShiftId = shiftIds[2] },
                new Schedule { ScheDate = DateOnly.FromDateTime(DateTime.Now), EmpId = empIds[0], ShiftId = shiftIds[1] },
                new Schedule { ScheDate = DateOnly.FromDateTime(DateTime.Now), EmpId = empIds[1], ShiftId = shiftIds[2] },
                new Schedule { ScheDate = DateOnly.FromDateTime(DateTime.Now), EmpId = empIds[2], ShiftId = shiftIds[0] }
            };

            ScheduleRepository scheduleRepository = new ScheduleRepository(context);
            foreach (var schedule in schedules)
            {
                await scheduleRepository.InsertAsync(schedule);
            }

            Console.WriteLine("Schedule data added.");
        }

        public static async Task AddAttendanceData(DBContext context)
        {
            List<Attendance> attendances = new List<Attendance>
            {
                new Attendance { ScheId = 1, Status = AttendanceStatus.ClockIn, CheckIn = DateTime.Today.AddHours(8) },
                new Attendance { ScheId = 2, Status = AttendanceStatus.ClockOut, CheckIn = DateTime.Today.AddHours(13).AddMinutes(-10), CheckOut = DateTime.Today.AddHours(17).AddMinutes(5), WorkingHours = new decimal(4.9) },
                new Attendance { ScheId = 3, Status = AttendanceStatus.ClockOut, CheckIn = DateTime.Today.AddHours(18).AddMinutes(5), CheckOut = DateTime.Today.AddHours(22).AddMinutes(10), WorkingHours = new decimal(4.1) },
            };

            AttendanceRepository attendanceRepository = new AttendanceRepository(context);
            foreach (var attendance in attendances)
            {
                await attendanceRepository.InsertAsync(attendance);
            }

            Console.WriteLine("Attendance data added.");
        }

        public static async Task AddFeedbackData(DBContext context)
        {
            List<FeedBack> feedbacks = new List<FeedBack>
            {
                new FeedBack { FullName = "John Doe", Email = "JohnDoe@gmail.com", Phone = "0123456789", Subject = "Great Service", Body = "I had a wonderful experience at your restaurant. The food was delicious, and the staff was friendly and attentive. I will definitely be back!", CreateAt = DateTime.Now.AddDays(-3), Status = FeedBackStatus.Resolved },
                new FeedBack { FullName = "Jane Doe", Email = "JaneDoe@gmail.com", Phone = "0123456789", Subject = "Excellent Food", Body = "The food at your restaurant is amazing! I especially loved the pasta carbonara and tiramisu. Keep up the good work!", CreateAt = DateTime.Now.AddDays(-2), Status = FeedBackStatus.Reviewed },
                new FeedBack { FullName = "John Smith", Email = "JohnSmith@gmail.com", Phone = "0123456789", Subject = "Outstanding Service", Body = "I had a fantastic dining experience at your restaurant. The service was top-notch, and the food was delicious. I will definitely recommend your restaurant to my friends and family.", CreateAt = DateTime.Now.AddDays(0), Status = FeedBackStatus.Pending },
                new FeedBack { FullName = "Jane Smith", Email = "JaneSmith@gmail.com", Phone = "0123456789", Subject ="Test", Body = "Test", CreateAt = DateTime.Now.AddDays(-1), Status = FeedBackStatus.Dismissed },
            };

            FeedBackRepository feedbackRepository = new FeedBackRepository(context);
            foreach (var feedback in feedbacks)
            {
                await feedbackRepository.InsertAsync(feedback);
            }

            Console.WriteLine("Feedback data added.");
        }

        public static async Task AddPayrollData(DBContext context)
        {

            List<Payroll> payrolls = new List<Payroll>
        {
            new Payroll
            {
                CreatedBy = 1,
                CreatedAt = new DateTime(2022, 1, 1),
                EmpId = 1,
                Month = 1,
                Year = 2022,
                WorkingHours = 160,
                Salary = 2000,
                Status = PayrollStatus.Paid,
                PaymentDate = new DateOnly(2022, 1, 31)
            },
            new Payroll
            {
                CreatedBy = 1,
                CreatedAt = new DateTime(2022, 1, 2),
                EmpId = 2,
                Month = 1,
                Year = 2022,
                WorkingHours = 160,
                Salary = 1500,
                Status = PayrollStatus.UnPaid,
                PaymentDate = new DateOnly(2022, 2, 5)
            },
            new Payroll
            {
                CreatedBy = 1,
                CreatedAt = new DateTime(2022, 1, 3),
                EmpId = 3,
                Month = 1,
                Year = 2022,
                WorkingHours = 160,
                Salary = 1200,
                Status = PayrollStatus.UnPaid,
                PaymentDate = new DateOnly(2022, 2, 5)
            },
            new Payroll
            {
                CreatedBy = 1,
                CreatedAt = new DateTime(2022, 1, 4),
                EmpId = 4,
                Month = 1,
                Year = 2022,
                WorkingHours = 160,
                Salary = 1800,
                Status = PayrollStatus.Paid,
                PaymentDate = new DateOnly(2022, 2, 5)
            }
        };
            PayrollRepository payrollRepository = new PayrollRepository(context);
            foreach (var payroll in payrolls)
            {
                await payrollRepository.InsertAsync(payroll);
            }

            Console.WriteLine("Payroll data added.");

        }
    }
}