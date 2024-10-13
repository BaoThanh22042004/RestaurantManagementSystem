using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
	public class CartViewModel
	{
		public List<CartItemViewModel> ItemList { get; set; } = new List<CartItemViewModel>();

	}
}
