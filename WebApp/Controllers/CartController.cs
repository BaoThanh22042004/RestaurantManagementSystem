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

		public IActionResult Index()
		{
			var cart = _cartManager.GetCartFromCookie(Request);
			return View("CartView", cart);
		}

		public async Task<IActionResult> AddToCart(int dishId, string returnUrl)
		{
			try
			{
				var dishes = await _dishRepository.GetAllAsync();
				var dish = dishes.Where(d => d.DishId.Equals(dishId)).FirstOrDefault();
				if (dish != null)
				{
					var cart = _cartManager.GetCartFromCookie(Request);
					bool isItemExist = cart.ItemList.Where(d => d.Id.Equals(dish.DishId)).Any();
					if (isItemExist)
					{
						cart.ItemList.Where(d => d.Id.Equals(dish.DishId)).First().Quantity++;
					}
					else
					{
						CartItemViewModel cartItemViewModel = new CartItemViewModel(dish);
						cart.ItemList.Add(cartItemViewModel);
					}
					_cartManager.SaveCartToCookie(cart, Response);
				}
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while adding dish to cart. Please try again later.";
				return View("DishView");
			}

			if (!string.IsNullOrEmpty(returnUrl))
			{
				return Redirect(returnUrl);
			}
			return RedirectToAction("Index", "Home");
		}

		public async Task<IActionResult> UpdateQuantity(int dishId, string returnUrl, bool isIncrease)
		{
			try
			{
				var dish = await _dishRepository.GetByIDAsync(dishId);
				if (dish != null)
				{
					var cart = _cartManager.GetCartFromCookie(Request);
					var cartItem = cart.ItemList.FirstOrDefault(d => d.Id == dish.DishId);
					if (cartItem != null)
					{
						if (isIncrease)
						{
							cartItem.Quantity++;
						}
						else if (cartItem.Quantity > 1 && !isIncrease)
						{
							cartItem.Quantity--;
						}
						else if (cartItem.Quantity == 1 && !isIncrease)
						{
							cart.ItemList.Remove(cartItem);
						}
						_cartManager.SaveCartToCookie(cart, Response);
					}
				}
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while updating dish quantity. Please try again later.";
				return View("CartView");
			}
			if (!string.IsNullOrEmpty(returnUrl))
			{
				return Redirect(returnUrl);
			}
			return RedirectToAction("Index", "Home");
		}

		public IActionResult ClearCart()
		{
			var cart = new CartViewModel();
			_cartManager.SaveCartToCookie(cart, Response);
			return RedirectToAction("Index", "Cart");
		}

	}
}
