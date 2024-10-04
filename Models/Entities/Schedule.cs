using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entities
{
	public class Schedule
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long ScheId { get; set; }

		public required DateOnly ScheDate { get; set; }

		public required int EmpId { get; set; }

		public required int ShiftId { get; set; }

		// Navigation property
		public Shift Shift { get; set; } = null!;

		public Attendance? Attendance { get; set; }

		public User Employee { get; set; } = null!;
	}
}
