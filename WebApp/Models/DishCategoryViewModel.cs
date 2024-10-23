using Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
	public class DishCategoryViewModel
	{
		[Display(Name = "Category ID")]
		public int? CategoryId { get; set; }

		[Required(ErrorMessage = "Category name is required.")]
		[StringLength(255, ErrorMessage = "Category name must not exceed 255 characters.")]
		[Display(Name = "Category Name")]
		public string? CategoryName { get; set; }

		// For the checkbox in the view
		public bool IsSelected { get; set; }

		// Default constructor
		public DishCategoryViewModel()
		{
		}

		// Constructor that maps from the Category model (entity)
		public DishCategoryViewModel(DishCategory category)
		{
			CategoryId = category.CatId;
			CategoryName = category.CatName;
		}
	}
}
