using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Repositories.Interface;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using WebApp.Models;
using WebApp.Utilities;

namespace WebApp.Controllers
{
	public class AccountController : Controller
	{
		private readonly IUserRepository _userRepository;
		private readonly IAuthorizationService _authorizationService;
		private readonly UserClaimManager _userClaimManager;

		public AccountController(IUserRepository userRepository, IAuthorizationService authorizationService, UserClaimManager userClaimManager)
		{
			_userRepository = userRepository;
			_authorizationService = authorizationService;
			_userClaimManager = userClaimManager;
		}

		public IActionResult Register()
		{
			if (User.Identity?.IsAuthenticated == true)
			{
				return RedirectToAction(nameof(RedirectBasedOnRole));
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
			catch (ArgumentException e)
			{
				TempData["Error"] = e.Message;
				return View("RegisterView", register);
			}
			catch
			{
				TempData["Error"] = "An error occurred while registering user. Please try again later.";
				return View("RegisterView", register);
			}

			TempData["Success"] = "User registered successfully. Please login to continue.";
			return RedirectToAction(nameof(Login));
		}

		public async Task<IActionResult> Login()
		{
			if (User.Identity?.IsAuthenticated == true)
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

				await SignInAsync(user);
			}
			catch
			{
				TempData["Error"] = "An error occurred while logging in. Please try again later.";
				return View("LoginView", login);
			}

			return RedirectToAction(nameof(RedirectBasedOnRole));
		}

		public IActionResult ForgotPassword()
		{
			if (User.Identity?.IsAuthenticated == true)
			{
				return RedirectToAction(nameof(RedirectBasedOnRole));
			}

			return View("ForgotPasswordView");
		}

		[HttpPost]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgotPassword)
		{
			if (!ModelState.IsValid)
			{
				return View("ForgotPasswordView", forgotPassword);
			}

			try
			{
				var user = await _userRepository.GetByUsernameAsync(forgotPassword.Username);
				if (user == null || user.Email != forgotPassword.Email)
				{
					TempData["Error"] = "Invalid username or email.";
					return View("ForgotPasswordView", forgotPassword);
				}

				var newPassword = Guid.NewGuid().ToString().Substring(0, 8);
				await _userRepository.UpdateAsync(user, newPassword);
				SendMail(user.Email, user.Username, newPassword);
				TempData["Success"] = "An email has been sent to your email address with your new password.";
			}
			catch
			{
				TempData["Error"] = "An error occurred while processing your request. Please try again later.";
				return View("ForgotPasswordView", forgotPassword);
			}

			return RedirectToAction(nameof(Login));
		}

		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Index", "Home");
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

		[Authorize]
		public async Task<IActionResult> Profile()
		{
			var user = await _userRepository.GetByUsernameAsync(User.Identity.Name);
			var userViewModel = new ProfileViewModel(user);
			return View("ProfileView", userViewModel);
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> Profile(ProfileViewModel profile)
		{
			try
			{
				if (!string.IsNullOrWhiteSpace(profile.OldPassword))
				{
					var isOldPasswordCorrect = await _userRepository.ValidateLoginAsync(User.Identity.Name, profile.OldPassword) != null;

					if (!isOldPasswordCorrect)
					{
						ModelState.AddModelError("OldPassword", "Old password is incorrect.");
					}
					else if (!string.IsNullOrWhiteSpace(profile.NewPassword) && profile.OldPassword == profile.NewPassword)
					{
						ModelState.AddModelError("NewPassword", "New password must be different from old password.");
					}
					else if (string.IsNullOrWhiteSpace(profile.NewPassword))
					{
						ModelState.AddModelError("NewPassword", "New password is required.");
					}
				}

				if (!ModelState.IsValid)
				{
					return View("ProfileView", profile);
				}

				var userEntity = await _userRepository.GetByUsernameAsync(User.Identity.Name);
				userEntity.FullName = profile.FullName;
				userEntity.Email = profile.Email;
				userEntity.Phone = profile.Phone;
				await _userRepository.UpdateAsync(userEntity, profile.NewPassword);

				await SignInAsync(userEntity);

				TempData["Success"] = "Profile updated successfully.";
			}
			catch
			{
				TempData["Error"] = "An error occurred while updating your profile. Please try again later.";
				return View("ProfileView", profile);
			}

			return RedirectToAction(nameof(Profile));
		}

		private async Task SignInAsync(User userEntity)
		{
			var claimsPrincipal = _userClaimManager.CreateClaimsPrincipal(
				userEntity.UserId.ToString(),
				userEntity.Username,
				userEntity.FullName,
				userEntity.Role.ToString()
			);

			var claimsIdentity = (ClaimsIdentity)claimsPrincipal.Identity;
			claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, userEntity.Email));
			claimsIdentity.AddClaim(new Claim(ClaimTypes.MobilePhone, userEntity.Phone));

			var authenticationProperties = _userClaimManager.authenticationProperties;

			await HttpContext.SignInAsync(
				CookieAuthenticationDefaults.AuthenticationScheme,
				claimsPrincipal,
				authenticationProperties);
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

			using var message = new MailMessage(fromAddress, toAddress)
			{
				Subject = subject,
				Body = body
			};

			smtp.Send(message);
		}
	}
}
