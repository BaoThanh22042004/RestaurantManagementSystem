using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		public required decimal WorkingHours { get; set; }

		public required decimal Salary { get; set; }

		public required PayrollStatus Status { get; set; }

		public DateOnly? PaymentDate { get; set; }

		public User Creator { get; set; } = null!;
		public User Employee { get; set; } = null!;

	}
}
