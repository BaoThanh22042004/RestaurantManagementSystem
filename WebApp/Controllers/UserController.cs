using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Repositories.Interface;
using WebApp.Models;
using WebApp.Utilities;

namespace WebApp.Controllers
{
	[Route("Dashboard/User")]
	[Authorize(Roles = $"{nameof(Role.Manager)}")]
	public class UserController : Controller
	{
		private readonly IUserRepository _userRepository;
		private readonly UserClaimManager _pendingUsersManager;

		public UserController(IUserRepository userRepository, UserClaimManager pendingUsersManager)
		{
			_userRepository = userRepository;
			_pendingUsersManager = pendingUsersManager;
		}

		public async Task<IActionResult> Index()
		{
			var users = await _userRepository.GetAllAsync();
			var userList = users.Select(user => new UserViewModel(user));
			return View("UserView", userList);
		}

		[HttpGet("Create")]
		public IActionResult Create()
		{
			return PartialView("_CreateUserModal");
		}

		[HttpPost("Create")]
		public async Task<IActionResult> Create(UserViewModel userViewModel)
		{
			if (string.IsNullOrWhiteSpace(userViewModel.Password))
			{
				ModelState.AddModelError("Password", "Password is required.");
			}

			if (!ModelState.IsValid)
			{
				return PartialView("_CreateUserModal", userViewModel);
			}

			try
			{
				var user = new User
				{
					Username = userViewModel.Username,
					Password = userViewModel.Password,
					FullName = userViewModel.FullName,
					Email = userViewModel.Email,
					Phone = userViewModel.Phone,
					Role = userViewModel.Role,
					Salary = userViewModel.Salary,
					IsActive = userViewModel.IsActive
				};

				await _userRepository.InsertAsync(user);
			}
			catch (ArgumentException e)
			{
				TempData["Error"] = e.Message;
				return PartialView("_CreateUserModal", userViewModel);
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while creating user. Please try again later.";
				return PartialView("_CreateUserModal", userViewModel);
			}

			return Json(new { success = true });
		}

		[HttpGet("Details/{id}")]
		public async Task<IActionResult> Details(int id)
		{
			var user = await _userRepository.GetByIDAsync(id);
			if (user == null)
			{
				return NotFound();
			}

			var userViewModel = new UserViewModel(user);
			return PartialView("_DetailsUserModal", userViewModel);
		}

		[Route("Edit/{id}")]
		public async Task<IActionResult> Edit(int id)
		{
			var user = await _userRepository.GetByIDAsync(id);
			if (user == null)
			{
				return NotFound();
			}

			var userViewModel = new UserViewModel(user);
			return PartialView("_EditUserModal", userViewModel);
		}

		[HttpPost]
		[Route("Edit/{id}")]
		public async Task<IActionResult> Edit(UserViewModel user)
		{
			if (!ModelState.IsValid)
			{
				return PartialView("_EditUserModal", user);
			}

			try
			{
				var id = user.UserId ?? throw new KeyNotFoundException();
				var userEntity = await _userRepository.GetByIDAsync(id);
				if (userEntity == null)
				{
					throw new KeyNotFoundException();
				}

				if (userEntity.Username == User.Identity.Name && user.Role != Role.Manager)
				{
					TempData["Error"] = "You cannot change your role to other than Manager.";
					return PartialView("_EditUserModal", user);
				}

				if (string.IsNullOrEmpty(user.Password))
				{
					user.Password = null;
				}
				userEntity.FullName = user.FullName;
				userEntity.Email = user.Email;
				userEntity.Phone = user.Phone;
				userEntity.Role = user.Role;
				userEntity.Salary = user.Salary;
				userEntity.IsActive = user.IsActive;

				await _userRepository.UpdateAsync(userEntity, user.Password);

				_pendingUsersManager.Add(userEntity.UserId.ToString(), userEntity);
			}
			catch (KeyNotFoundException)
			{
				return NotFound();
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while updating user. Please try again later.";
				return PartialView("_EditUserModal", user);
			}

			return Json(new { success = true });
		}

		[Route("Delete/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var user = await _userRepository.GetByIDAsync(id);
			if (user == null)
			{
				return NotFound();
			}

			var userViewModel = new UserViewModel(user);
			return PartialView("_DeleteUserModal", userViewModel);
		}

		[HttpPost]
		[Route("Delete/{UserId}")]
		public async Task<IActionResult> DeleteConfirmed(int UserId)
		{
			try
			{
				var userEntity = await _userRepository.GetByIDAsync(UserId);

				if (userEntity.Username == User.Identity.Name)
				{
					TempData["Error"] = "You cannot delete yourself.";
					return RedirectToAction("Delete", new { id = UserId });
				}

				await _userRepository.DeleteAsync(UserId);

				_pendingUsersManager.Add(UserId.ToString(), null);
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while deleting user. Please try again later.";
				return RedirectToAction("Delete", new { id = UserId });
			}

			return Json(new { success = true });
		}
	}
}
