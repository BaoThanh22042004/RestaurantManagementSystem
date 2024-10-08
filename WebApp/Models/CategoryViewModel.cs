using System.ComponentModel.DataAnnotations;
using Models.Entities;

namespace WebApp.Models
{
    public class CategoryViewModel
    {
        [Display(Name = "Category ID")]
        public int? CategoryId { get; set; } // Nullable for new entries

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(255, ErrorMessage = "Category name must not exceed 255 characters.")]
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; }

        // Default constructor
        public CategoryViewModel()
        {
        }

        // Constructor that maps from the Category model (entity)
        public CategoryViewModel(DishCategory category)
        {
            CategoryId = category.CatId;
            CategoryName = category.CatName;
        }

        // Method to convert back to Category entity (for database operations)
        public DishCategory ToEntity()
        {
            var categoryEntity = new DishCategory
            {
                CatId = this.CategoryId ?? 0, // Set to 0 for new entries
                CatName = this.CategoryName,
            };

            return categoryEntity;
        }
    }
}
