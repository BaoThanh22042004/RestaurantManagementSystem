using Microsoft.AspNetCore.Mvc;
using Repositories.Interface;
using WebApp.Models;
using WebApp.Utilities;

namespace WebApp.Controllers
{
	public class CartController : Controller
	{
		private readonly IDishRepository _dishRepository;
		private readonly CartManager _cartManager;

		public CartController(IDishRepository dishRepository, CartManager cartManager)
		{
			_dishRepository = dishRepository;
			_cartManager = cartManager;
		}

		[HttpPost]
		public async Task<IActionResult> AddToCart(int dishId)
		{
			try
			{
				var dishes = await _dishRepository.GetAllAsync();
				var dish = dishes.FirstOrDefault(d => d.DishId.Equals(dishId)) ?? throw new Exception();

				var cart = _cartManager.GetCartFromCookie(Request);
				var cartItem = cart.ItemList.FirstOrDefault(d => d.Id == dish.DishId);

				if (cartItem != null)
				{
					cartItem.Quantity++;
				}
				else
				{
					CartItemViewModel cartItemViewModel = new CartItemViewModel(dish);
					cart.ItemList.Add(cartItemViewModel);
				}

				_cartManager.SaveCartToCookie(cart, Response);
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while adding dish to cart. Please try again later.";
				return Json(new { success = false });
			}

			return Json(new { success = true });
		}

		public IActionResult UpdateQuantity(int dishId, bool isIncrease)
		{
			try
			{
				var cart = _cartManager.GetCartFromCookie(Request);
				var cartItem = cart.ItemList.FirstOrDefault(d => d.Id == dishId);

				if (cartItem != null)
				{
					if (isIncrease)
					{
						cartItem.Quantity++;
					}
					else if (cartItem.Quantity > 1)
					{
						cartItem.Quantity--;
					}
					else
					{
						cart.ItemList.Remove(cartItem);
					}
				}

				_cartManager.SaveCartToCookie(cart, Response);
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while updating dish quantity. Please try again later.";
				return Json(new { success = false });
			}

			return Json(new { success = true });
		}

		[HttpPost]
		public IActionResult RemoveItem(int dishId)
		{
			try
			{
				var cart = _cartManager.GetCartFromCookie(Request);
				var cartItem = cart.ItemList.FirstOrDefault(d => d.Id == dishId);

				if (cartItem != null)
				{
					cart.ItemList.Remove(cartItem);
				}

				_cartManager.SaveCartToCookie(cart, Response);
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while removing the dish from the cart. Please try again later.";
				return Json(new { success = false });
			}

			return Json(new { success = true });
		}

		[HttpPost]
		public IActionResult ClearCart()
		{
			try
			{
				var cart = new CartViewModel();
				_cartManager.SaveCartToCookie(cart, Response);
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while clearing cart. Please try again later.";
				return Json(new { success = false });
			}

			return Json(new { success = true });
		}
	}
}
