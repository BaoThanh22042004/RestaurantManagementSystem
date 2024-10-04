using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entities
{
	public enum PayrollStatus
	{
		UnPaid,
		Paid
	}

	public class Payroll
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long PayrollId { get; set; }

		public required int CreatedBy { get; set; }

		public required DateTime CreatedAt { get; set; } = DateTime.Now;

		public required int EmpId { get; set; }

		public required byte Month { get; set; }

		public required short Year { get; set; }

		[Precision(6, 2)]
		public required decimal WorkingHours { get; set; }

		[Precision(10, 2)]
		public required decimal Salary { get; set; }

		public required PayrollStatus Status { get; set; }

		public DateOnly? PaymentDate { get; set; }

		// Navigation properties
		public User Creator { get; set; } = null!;

		public User Employee { get; set; } = null!;

	}
}
