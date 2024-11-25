﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entities
{
	public enum Role
	{
		Customer,
		Waitstaff,
		Chef,
		Accountant,
		Manager,
	}

	public class User
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int UserId { get; set; }

		[StringLength(255)]
		public required string Username { get; set; }

		[StringLength(32)]
		public required string Password { get; set; }

		[StringLength(255)]
		public required string FullName { get; set; }

		[StringLength(255)]
		public required string Email { get; set; }

		[StringLength(15)]
		public required string Phone { get; set; }

		public required Role Role { get; set; }

		[Precision(10, 2)]
		public decimal? Salary { get; set; }

		public bool IsActive { get; set; } = true;

		// Navigation properties
		public ICollection<Payroll> CreatedPayrolls { get; set; } = new List<Payroll>();

		public ICollection<Payroll> Payrolls { get; set; } = new List<Payroll>();

		public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

		public ICollection<OrderItem> CreatedOrderItems { get; set; } = new List<OrderItem>();

		public ICollection<StorageLog> CreatedStorageLogs { get; set; } = new List<StorageLog>();

		public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

		public ICollection<Bill> CreatedBills { get; set; } = new List<Bill>();
    }
}
