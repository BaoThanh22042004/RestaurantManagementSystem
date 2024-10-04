using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entities
{
	public class Shift
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ShiftId { get; set; }

		[StringLength(255)]
		public required string ShiftName { get; set; }

		public required TimeOnly StartTime { get; set; }

		public required TimeOnly EndTime { get; set; }

		// Navigation property
		public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
	}
}
