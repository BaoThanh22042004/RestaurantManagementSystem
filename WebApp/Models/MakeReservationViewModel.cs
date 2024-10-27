using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
	public class MakeReservationViewModel
	{
		public CartViewModel? Cart { get; set; }

		[Display(Name = "Party Size")]
		[Required(ErrorMessage = "Party size is required.")]
		[Range(1, 100, ErrorMessage = "Party size must be between 1 and 100.")]
		public short PartySize { get; set; }

		[Display(Name = "Reservation Date")]
		[Required(ErrorMessage = "Reservation date is required.")]
		public DateOnly ResDate { get; set; }

		[Display(Name = "Reservation Time")]
		[Required(ErrorMessage = "Reservation time is required.")]
		public TimeOnly ResTime { get; set; }

		[Display(Name = "Notes")]
		[DataType(DataType.MultilineText)]
		public string? Notes { get; set; }
	}
}
