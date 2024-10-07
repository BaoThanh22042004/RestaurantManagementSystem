using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Repositories.Interface;
using WebApp.Models;

namespace WebApp.Controllers
{
	[Route("Dashboard")]
	[Authorize(Roles = "Manager")]
	public class UserController : Controller
	{
		private readonly IUserRepository _userRepository;

		public UserController(IUserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		[Route("User")]
		public async Task<IActionResult> Index()
		{
			var users = await _userRepository.GetAllAsync();
			var userList = users.Select(user => new UserViewModel(user));
			return View("UserView", userList);
		}

		[Route("User/Create")]
		public IActionResult Create()
		{
			return View("CreateUserView");
		}

		[Route("User/Create")]
		[HttpPost]
		public async Task<IActionResult> Create(UserViewModel userViewModel)
		{
			if (string.IsNullOrWhiteSpace(userViewModel.Password))
			{
				ModelState.AddModelError("Password", "Password is required.");
			}

			if (!ModelState.IsValid)
			{
				return View("CreateUserView", userViewModel);
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
				return View("CreateUserView", userViewModel);
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while creating user. Please try again later.";
				return View("CreateUserView", userViewModel);
			}

			return RedirectToAction("Index");
		}

		[Route("User/Details/{id}")]
		public async Task<IActionResult> Details(int id)
		{
			var user = await _userRepository.GetByIDAsync(id);
			if (user == null)
			{
				return NotFound();
			}

			var userViewModel = new UserViewModel(user);
			return View("DetailsUserView", userViewModel);
		}

		[Route("User/Edit/{id}")]
		public async Task<IActionResult> Edit(int id)
		{
			var user = await _userRepository.GetByIDAsync(id);
			if (user == null)
			{
				return NotFound();
			}

			var userViewModel = new UserViewModel(user);
			return View("EditUserView", userViewModel);
		}

		[HttpPost]
		[Route("User/Edit/{UserId}")]
		public async Task<IActionResult> Edit(UserViewModel user, int UserId)
		{
			if (!ModelState.IsValid)
			{
				return View("EditUserView", user);
			}

			try
			{
				var userEntity = await _userRepository.GetByIDAsync(UserId);
				if (userEntity == null)
				{
					return NotFound();
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
			}
			catch (InvalidOperationException)
			{
				return NotFound();
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while updating user. Please try again later.";
				return View("EditUserView", user);
			}

			return RedirectToAction("Index");
		}

		[Route("User/Delete/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var user = await _userRepository.GetByIDAsync(id);
			if (user == null)
			{
				return NotFound();
			}

			var userViewModel = new UserViewModel(user);
			return View("DeleteUserView", userViewModel);
		}

		[HttpPost]
		[Route("User/Delete/{UserId}")]
		public async Task<IActionResult> DeleteConfirmed(int UserId)
		{
			try
			{
				await _userRepository.DeleteAsync(UserId);
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while deleting user. Please try again later.";
				return RedirectToAction("Delete", new { UserId });
			}

			return RedirectToAction("Index");
		}
	}
}
