using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entities
{
	public enum OrderItemStatus
	{
		Reservation,
		Pending,
		Preparing,
		Ready,
		Served,
	}

	public class OrderItem
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long OrItemId { get; set; }

		public required long OrderId { get; set; }

		public required int DishId { get; set; }

		public required int CreatedBy { get; set; }

		public required short Quantity { get; set; }

		[Precision(10, 2)]
		public required decimal Price { get; set; }

		public required OrderItemStatus Status { get; set; }

		[Column(TypeName = "text")]
		public string? Notes { get; set; }

		// Navigation properties
		public User Creator { get; set; } = null!;

		public Order Order { get; set; } = null!;

		public Dish Dish { get; set; } = null!;
	}
}
