using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class DishViewModel
    {

        public int? DishId { get; set; }

        [Required(ErrorMessage = "Dish name is required.")]
        [StringLength(255, ErrorMessage = "Dish name cannot be longer than 255 characters.")]
        [Display(Name = "Dish Name")]
        public string DishName { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, 10000000, ErrorMessage = "Price must be between 0 and 10000000.")]
        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Visible")]
        public bool Visible { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        // Dropdown list
        public IEnumerable<SelectListItem>? CategoryOptions { get; set; }

        // Display only
        [Display(Name = "Category")]
        public string? CategoryName { get; set; }

        public string? ImagePath { get; set; }

		[Display(Name = "Upload Image")]
		public IFormFile? UploadedImage { get; set; }

		public DishViewModel()
        {
        }

        public DishViewModel(Dish dish)
        {
            DishId = dish.DishId;
            DishName = dish.DishName;
            Price = dish.Price;
            Description = dish.Description;
            Visible = dish.Visible;
            CategoryId = dish.CategoryId;
            CategoryName = dish.Category?.CatName;
            ImagePath = $"/Uploads/Images/Dishes/{dish.DishId}.jpg"; ;
        }
    }
}
