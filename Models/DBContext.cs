using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.Entities;

namespace Models
{
	public class DBContext : DbContext
	{
		public DbSet<User> Users { get; set; }

		public DbSet<DishCategory> DishCategories { get; set; }

		public DbSet<Dish> Dishes { get; set; }

		public DbSet<Storage> Storages { get; set; }

		public DbSet<StorageLog> StorageLogs { get; set; }

		public DbSet<Reservation> Reservations { get; set; }

		public DbSet<Table> Tables { get; set; }

		public DbSet<Order> Orders { get; set; }

		public DbSet<OrderItem> OrderItems { get; set; }

		public DbSet<Bill> Bills { get; set; }

		public DbSet<Shift> Shifts { get; set; }

		public DbSet<Schedule> Schedules { get; set; }

		public DbSet<Attendance> Attendances { get; set; }

		public DbSet<Payroll> Payrolls { get; set; }

		public DbSet<FeedBack> Feedbacks { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			var connectionString = new ConfigurationBuilder()
				  .AddUserSecrets<DBContext>()
				  .Build()
				  ["ConnectionString"];

			optionsBuilder.UseSqlServer(connectionString);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			#region User
			modelBuilder.Entity<User>()
						.HasIndex(u => u.Username)
						.IsUnique();
			#endregion

			#region DishCategory
			modelBuilder.Entity<DishCategory>()
						.HasIndex(dc => dc.CatName)
						.IsUnique();
			#endregion

			#region Dish
			modelBuilder.Entity<Dish>()
						.HasIndex(d => d.DishName)
						.IsUnique();

			modelBuilder.Entity<Dish>()
						.HasOne(d => d.Category)
						.WithMany(dc => dc.Dishes)
						.HasForeignKey(d => d.CategoryId)
						.IsRequired();
			#endregion

			#region Storage
			modelBuilder.Entity<Storage>()
						.HasIndex(s => s.ItemName)
						.IsUnique();
			#endregion

			#region StorageLog
			modelBuilder.Entity<StorageLog>()
						.HasOne(sl => sl.StorageItem)
						.WithMany(s => s.StorageLogs)
						.HasForeignKey(sl => sl.ItemId)
						.IsRequired();

			modelBuilder.Entity<StorageLog>()
						.HasOne(sl => sl.Creator)
						.WithMany(u => u.CreatedStorageLogs)
						.HasForeignKey(sl => sl.CreatedBy)
						.IsRequired();
			#endregion

			#region Reservation
			modelBuilder.Entity<Reservation>()
						.HasOne(r => r.Customer)
						.WithMany(u => u.Reservations)
						.HasForeignKey(r => r.CustomerId)
						.IsRequired();
			#endregion

			#region Table

			#endregion

			#region Order
			modelBuilder.Entity<Order>()
						.HasOne(o => o.Reservation)
						.WithOne(r => r.Order)
						.HasForeignKey<Order>(o => o.ResId)
						.IsRequired(false);

			modelBuilder.Entity<Order>()
						.HasOne(o => o.Table)
						.WithMany(t => t.Orders)
						.HasForeignKey(o => o.TableId)
						.IsRequired(false);
			#endregion

			#region OrderItem
			modelBuilder.Entity<OrderItem>()
						.HasOne(oi => oi.Creator)
						.WithMany(u => u.CreatedOrderItems)
						.HasForeignKey(oi => oi.CreatedBy)
						.IsRequired();

			modelBuilder.Entity<OrderItem>()
						.HasOne(oi => oi.Dish)
						.WithMany(d => d.OrderItems)
						.HasForeignKey(oi => oi.DishId);

			modelBuilder.Entity<OrderItem>()
						.HasOne(oi => oi.Order)
						.WithMany(o => o.OrderItems)
						.HasForeignKey(oi => oi.OrderId)
						.IsRequired();
			#endregion

			#region Bill
			modelBuilder.Entity<Bill>()
						.HasOne(b => b.Creator)
						.WithMany(u => u.CreatedBills)
						.HasForeignKey(b => b.CreatedBy)
						.IsRequired();

			modelBuilder.Entity<Bill>()
						.HasOne(b => b.Order)
						.WithOne(o => o.Bill)
						.HasForeignKey<Bill>(b => b.OrderId)
						.IsRequired();
			#endregion

			#region Shift

			#endregion

			#region Schedule
			modelBuilder.Entity<Schedule>()
						.HasOne(sch => sch.Shift)
						.WithMany(s => s.Schedules)
						.HasForeignKey(sch => sch.ShiftId)
						.IsRequired();

			modelBuilder.Entity<Schedule>()
						.HasOne(sch => sch.Employee)
						.WithMany(u => u.Schedules)
						.HasForeignKey(sch => sch.EmpId)
						.IsRequired();
			#endregion

			#region Attendance
			modelBuilder.Entity<Attendance>()
						.HasOne(a => a.Schedule)
						.WithOne(s => s.Attendance)
						.HasForeignKey<Attendance>(a => a.ScheId)
						.IsRequired();
			#endregion

			#region Payroll
			modelBuilder.Entity<Payroll>()
						.HasOne(p => p.Creator)
						.WithMany(u => u.CreatedPayrolls)
						.HasForeignKey(p => p.CreatedBy)
						.OnDelete(DeleteBehavior.Restrict);


			modelBuilder.Entity<Payroll>()
						.HasOne(p => p.Employee)
						.WithMany(u => u.Payrolls)
						.HasForeignKey(p => p.EmpId)
						.OnDelete(DeleteBehavior.Restrict);
			#endregion

			#region Feedback
			#endregion

			base.OnModelCreating(modelBuilder);
		}
	}
}
