using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Repositories.Interface;
using System.Security.Claims;
using WebApp.Models;

namespace WebApp.Controllers
{
	public class AccountController : Controller
	{
		private readonly IUserRepository _userRepository;
		private readonly IAuthorizationService _authorizationService;
		public AccountController(IUserRepository userRepository, IAuthorizationService authorizationService)
		{
			_userRepository = userRepository;
			_authorizationService = authorizationService;
		}

		[HttpGet]
		public async Task<IActionResult> Register()
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("RedirectBasedOnRole");
			}

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel register)
		{
			if (!ModelState.IsValid)
			{
				return View(register);
			}

			try
			{
				var user = new User
				{
					Username = register.Username,
					Password = register.Password,
					FullName = register.FullName,
					Email = register.Email,
					Phone = register.Phone,
					Role = Role.Customer,
				};

				await _userRepository.InsertAsync(user);
			}
			catch (Exception e)
			{
				if (e is ArgumentException)
				{
					TempData["Error"] = e.Message;
				}
				else
				{
					TempData["Error"] = "An error occurred while registering user. Please try again later.";
				}
				return View(register);
			}

			return RedirectToAction("Login", "Account");
		}

		[HttpGet]
		public async Task<IActionResult> Login()
		{
			if (User.Identity.IsAuthenticated)
			{
				return await RedirectBasedOnRole();
			}

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel login)
		{
			if (!ModelState.IsValid)
			{
				return View(login);
			}

			try
			{
				var user = await _userRepository.ValidateLoginAsync(login.Username, login.Password);
				if (user == null)
				{
					TempData["Error"] = "Invalid username or password.";
					return View(login);
				}

				var claims = new List<Claim>
				{
					new Claim(ClaimTypes.Name, user.Username),
					new Claim("FullName", user.FullName),
					new Claim(ClaimTypes.Role, user.Role.ToString()),
				};

				var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

				var authProperties = new AuthenticationProperties
				{
					AllowRefresh = true,
					ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1),
					IsPersistent = true,
					IssuedUtc = DateTimeOffset.UtcNow
				};

				await HttpContext.SignInAsync(
					CookieAuthenticationDefaults.AuthenticationScheme,
					new ClaimsPrincipal(claimsIdentity),
					authProperties);
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while logging in. Please try again later.";
				return View(login);
			}

			return RedirectToAction("RedirectBasedOnRole");
		}

		[HttpGet]
		public async Task<IActionResult> ForgotPassword()
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("RedirectBasedOnRole");
			}

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgotPassword)
		{
			if (!ModelState.IsValid)
			{
				return View(forgotPassword);
			}

			try
			{
				var user = await _userRepository.GetByUsernameAsync(forgotPassword.Username);
				if (user == null || user.Email != forgotPassword.Email)
				{
					TempData["Error"] = "Invalid username or email.";
					return View(forgotPassword);
				}
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while processing your request. Please try again later.";
				return View(forgotPassword);
			}

			// Send email to user

			return RedirectToAction("Login", "Account");
		}

		[HttpGet]
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Index", "Home");
		}

		private void SendMail(string to, string subject, string body)
		{
			// Send email
		}

		public async Task<IActionResult> RedirectBasedOnRole()
		{
			var result = await _authorizationService.AuthorizeAsync(User, "Staff");
			if (result.Succeeded)
			{
				return RedirectToAction("Index", "Dashboard");
			}
			return RedirectToAction("Index", "Home");
		}
	}
}
