using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Models.Entities
{
	public enum ReservationStatus
	{
		Confirmed,
		Completed,
		Cancelled
	}

	public class Reservation
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long ResId { get; set; }

		public required int CustomerId { get; set; }


		public required short PartySize { get; set; }


		public required DateOnly ResDate { get; set; }


		public required TimeSpan ResTime { get; set; }


		public required ReservationStatus Status { get; set; }

		[Precision(10, 2)]
		public decimal? DepositAmount { get; set; }

		[Column(TypeName = "text")]
		public string? Notes { get; set; }

		// Navigation property
		public Order? Order { get; set; }

		public User Customer { get; set; } = null!;
	}
}