using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		public required bool IsActive { get; set; }


		// Navigation properties
		public ICollection<Payroll> CreatedPayrolls { get; set; } = new List<Payroll>();
		public ICollection<Payroll> Payrolls { get; set; } = new List<Payroll>();

		public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
	}
}
