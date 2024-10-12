using Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
	public class CartItemViewModel
	{
		public int Id { get; set; }

		[Display(Name = "Dish Name")]
		public string DishName { get; set; }

		[Display(Name = "Quantity")]
		public int Quantity {  get; set; }

		[Display(Name = "Price")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

		public CartItemViewModel() { }

		public CartItemViewModel(Dish dish)
		{
			Id = dish.DishId;
			DishName = dish.DishName;
			Quantity = 1;
			Price = dish.Price;
		}
	}
}
