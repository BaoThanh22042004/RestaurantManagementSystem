using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entities
{
	public enum PaymentMethod
	{
		Cash,
		Card,
		Online
	}

	public class Bill
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long BillId { get; set; }

		public required long OrderId { get; set; }

		public required int CreatedBy { get; set; }

		public required DateTime CreatedAt { get; set; } = DateTime.Now;

		[Precision(18, 2)]
		public required decimal TotalAmount { get; set; }

		public PaymentMethod? PaymentMethod { get; set; }

		public DateTime? PaymentTime { get; set; }

		// Navigation properties
		public Order Order { get; set; } = null!;

		public User Creator { get; set; } = null!;
	}
}
