using System.ComponentModel.DataAnnotations;
using Models.Entities;

namespace WebApp.Models
{
    public class DishCategoryViewModel
    {
        [Display(Name = "Category ID")]
        public int? CategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(255, ErrorMessage = "Category name must not exceed 255 characters.")]
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; }

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
