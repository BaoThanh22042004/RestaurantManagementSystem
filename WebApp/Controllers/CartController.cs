using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Newtonsoft.Json;
using Repositories;
using Repositories.Interface;
using WebApp.Models;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace WebApp.Controllers
{
    public class CartController : Controller
    {
        private readonly IDishRepository _dishRepository;

        public CartController(IDishRepository dishRepository)
        {
            _dishRepository = dishRepository;
        }

        public async Task<IActionResult> Index()
        {
			var cart = GetCartFromCookie();
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
                    var cart = GetCartFromCookie();
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
                    SaveCartToCookie(cart);
                }
            }
            catch (Exception)
            {
                throw;
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
                    var cart = GetCartFromCookie();
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
                        else if(cartItem.Quantity == 1 && !isIncrease)
                        {
                            cart.ItemList.Remove(cartItem);
                        }
                        SaveCartToCookie(cart);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> ClearCart()
        {
            var cart = GetCartFromCookie();
            cart.ItemList.Clear();
            SaveCartToCookie(cart);
            return RedirectToAction("Index", "Cart");
        }

        private const string CartCookieKey = "Cart";

        private CartViewModel GetCartFromCookie()
        {
            var cartCookie = Request.Cookies[CartCookieKey];
            if (string.IsNullOrEmpty(cartCookie))
            {
                return new CartViewModel();
            }
            return JsonConvert.DeserializeObject<CartViewModel>(cartCookie);
        }

        private void SaveCartToCookie(CartViewModel cart)
        {
            var cartJson = JsonConvert.SerializeObject(cart);
            CookieOptions options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(7),
                HttpOnly = true
            };
            Response.Cookies.Append(CartCookieKey, cartJson, options);
        }
    }
}
