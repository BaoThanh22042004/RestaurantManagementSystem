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
	public enum AttendanceStatus
	{
		Scheduled,
		ClockIn,
		ClockOut,
		Absent
	}

	public class Attendance
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long AttendId { get; set; }

		public required long ScheId { get; set; }
		public required TimeOnly CheckIn { get; set; }

		public TimeOnly? CheckOut { get; set; }

		[Precision(4, 2)]
		public decimal? WorkingHours { get; set; }

		public required AttendanceStatus Status { get; set; }

		// Navigation property
		public Schedule Schedule { get; set; } = null!;
	}
}
