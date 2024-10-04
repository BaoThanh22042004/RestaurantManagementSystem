using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entities
{
	public class DishCategory
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int CatId { get; set; }

		[StringLength(255)]
		public required string CatName { get; set; }

		// Navigation property
		public ICollection<Dish> Dishes { get; set; } = new List<Dish>();
	}
}
