using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
	public class DBContext : DbContext
	{
		public DbSet<User> Users { get; set; }

		public DbSet<Payroll> Payrolls { get; set; }

		public DbSet<OrderItem> OrderItems { get; set; }

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

			#endregion

			#region Dish

			#endregion

			#region Storage

			#endregion

			#region StorageLog

			#endregion

			#region Reservation

			#endregion

			#region Table

			#endregion

			#region Order

			#endregion

			#region OrderItem
			modelBuilder.Entity<OrderItem>()
						.HasOne(oi => oi.Creator)
						.WithMany(u => u.OrderItems)
						.HasForeignKey(oi => oi.CreatedBy)
						.OnDelete(DeleteBehavior.Restrict);
			#endregion

			#region Bill

			#endregion

			#region Shift

			#endregion

			#region Schedule

			#endregion

			#region Attendance

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

			base.OnModelCreating(modelBuilder);
		}
	}
}
