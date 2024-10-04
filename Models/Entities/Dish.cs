using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entities
{
	public class Dish
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int DishId { get; set; }

		[StringLength(255)]
		public required string DishName { get; set; }

		[Precision(10, 2)]
		public required decimal Price { get; set; }

		[Column(TypeName = "text")]
		public string? Description { get; set; }

		public required bool Visible { get; set; }

		public required int CategoryId { get; set; }

		// Navigation property
		public DishCategory Category { get; set; } = null!;

		public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
	}
}
