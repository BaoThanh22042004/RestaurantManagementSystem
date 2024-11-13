using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entities
{
	public enum OrderStatus
	{
		Reservation,
		Serving,
		Paid,
		Cancelled
	}

	public class Order
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long OrderId { get; set; }

		public int? TableId { get; set; }

		public required DateTime CreatedAt { get; set; }

		public required OrderStatus Status { get; set; }

		public long? ResId { get; set; }

		// Navigation property
		public Reservation? Reservation { get; set; }

		public Table? Table { get; set; }

		public Bill? Bill { get; set; }

		public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
	}
}
