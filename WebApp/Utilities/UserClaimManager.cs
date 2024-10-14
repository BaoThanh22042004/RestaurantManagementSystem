using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Models.Entities;
using System.Security.Claims;

namespace WebApp.Utilities
{
	public class UserClaimManager
	{
		private Dictionary<string, User?> _pendingUsers = new Dictionary<string, User?>();

		public bool IsEmpty { get; private set; }

		public readonly AuthenticationProperties authenticationProperties = new AuthenticationProperties
		{
			AllowRefresh = true,
			ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1),
			IsPersistent = true,
			IssuedUtc = DateTimeOffset.UtcNow
		};

		public ClaimsPrincipal CreateClaimsPrincipal(string userId, string username, string fullName, string role)
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, userId),
				new Claim(ClaimTypes.Name, username),
				new Claim(ClaimTypes.GivenName, fullName),
				new Claim(ClaimTypes.Role, role)
			};
			var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			return new ClaimsPrincipal(identity);
		}

		public void Add(string userId, User? user)
		{
			_pendingUsers[userId] = user;
			IsEmpty = false;
		}

		public ClaimsPrincipal? Get(string userId)
		{
			var user = _pendingUsers[userId];
			if (user == null)
			{
				return null;
			}
			if (user.IsActive)
			{
				return CreateClaimsPrincipal(user.UserId.ToString(), user.Username, user.FullName, user.Role.ToString());
			}
			return null;
		}

		public void Remove(string userID)
		{
			_pendingUsers.Remove(userID);
			if (_pendingUsers.Count == 0)
			{
				IsEmpty = true;
			}
		}

		public bool ContainsKey(string userId)
		{
			return _pendingUsers.ContainsKey(userId);
		}
	}
}
