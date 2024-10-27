using Newtonsoft.Json;
using WebApp.Models;

namespace WebApp.Utilities
{
	public class CartManager
	{
		private const string CartCookieKey = "Cart";

		public CartViewModel? GetCartFromCookie(HttpRequest request)
		{
			var cartCookie = request.Cookies[CartCookieKey];
			if (string.IsNullOrEmpty(cartCookie))
			{
				return new CartViewModel();
			}
			return JsonConvert.DeserializeObject<CartViewModel>(cartCookie);
		}

		public void SaveCartToCookie(CartViewModel cart, HttpResponse response)
		{
			var cartJson = JsonConvert.SerializeObject(cart);
			CookieOptions options = new CookieOptions
			{
				Expires = DateTime.Now.AddDays(7),
				HttpOnly = true
			};
			response.Cookies.Append(CartCookieKey, cartJson, options);
		}

		public string GetCartItemNumber(HttpRequest request)
		{
			var cart = GetCartFromCookie(request);
			if (cart == null) return "0";

			int count = 0;
			foreach (var item in cart.ItemList)
			{
				count += item.Quantity;
			}
			return count > 99 ? "99+" : count.ToString();
		}
	}
}
