﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Repositories.Interface;
using System.Net.Mail;
using System.Net;
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

		public IActionResult Register()
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("RedirectBasedOnRole");
			}

			return View("RegisterView");
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel register)
		{
			if (!ModelState.IsValid)
			{
				return View("RegisterView", register);
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
				return View("RegisterView", register);
			}

			return RedirectToAction("Login", "Account");
		}

		public async Task<IActionResult> Login()
		{
			if (User.Identity.IsAuthenticated)
			{
				return await RedirectBasedOnRole();
			}

			return View("LoginView");
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel login)
		{
			if (!ModelState.IsValid)
			{
				return View("LoginView", login);
			}

			try
			{
				var user = await _userRepository.ValidateLoginAsync(login.Username, login.Password);
				if (user == null)
				{
					TempData["Error"] = "Invalid username or password.";
					return View("LoginView", login);
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
				return View("LoginView", login);
			}

			return RedirectToAction("RedirectBasedOnRole");
		}

		public IActionResult ForgotPassword()
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("RedirectBasedOnRole");
			}

			return View("ForgotPasswordView");
		}

		[HttpPost]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgotPassword)
		{
			User user;
			if (!ModelState.IsValid)
			{
				return View("ForgotPasswordView", forgotPassword);
			}

			try
			{
				user = await _userRepository.GetByUsernameAsync(forgotPassword.Username);
				if (user == null || user.Email != forgotPassword.Email)
				{
					TempData["Error"] = "Invalid username or email.";
					return View("ForgotPasswordView", forgotPassword);
				}
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while processing your request. Please try again later.";
				return View("ForgotPasswordView", forgotPassword);
			}

			var newPassword = Guid.NewGuid().ToString().Substring(0, 8);
			try
			{
				await _userRepository.UpdateAsync(user, newPassword);
				SendMail(user.Email, user.Username, newPassword);
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while processing your request. Please try again later.";
				return View("ForgotPasswordView", forgotPassword);
			}
			TempData["Success"] = "An email has been sent to your email address with your new password.";
			return RedirectToAction("Login", "Account");
		}

		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Index", "Home");
		}

		private void SendMail(string to, string username, string password)
		{
			var userSecret = new ConfigurationBuilder()
				  .AddUserSecrets<AccountController>()
				  .Build();

			var fromAddress = new MailAddress(userSecret["Email"]);
			var toAddress = new MailAddress(to);
			string fromPassword = userSecret["AppPassword"];
			string subject = $"[RMS] Reset Password for {username} account";
			string body = "This is an automated email. Please do not reply.\n\n" +
				$"Your new password is: {password}\n\n" +
				"Please change your password after logging in.\n\n" +
				"Thank you for using our service.";

			var smtp = new SmtpClient
			{
				Host = "smtp.gmail.com",
				Port = 587,
				EnableSsl = true,
				DeliveryMethod = SmtpDeliveryMethod.Network,
				UseDefaultCredentials = false,
				Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
			};
			using (var message = new MailMessage(fromAddress, toAddress)
			{
				Subject = subject,
				Body = body
			})
			{
				smtp.Send(message);
			}
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