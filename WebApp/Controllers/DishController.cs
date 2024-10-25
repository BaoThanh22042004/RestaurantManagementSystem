using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using Repositories.Interface;
using WebApp.Models;
using WebApp.Utilities;

namespace WebApp.Controllers
{
	[Route("Dashboard/Dish")]
	[Authorize(Roles = $"{nameof(Role.Manager)}")]
	public class DishController : Controller
	{
		private readonly IDishRepository _dishRepository;
		private readonly IDishCategoryRepository _dishCategoryRepository;
		private readonly FileUploadManager _fileUploadManager;

		public DishController(IDishRepository dishRepository, IDishCategoryRepository dishCategoryRepository, FileUploadManager fileUploadManager)
		{
			_dishRepository = dishRepository;
			_dishCategoryRepository = dishCategoryRepository;
			_fileUploadManager = fileUploadManager;
		}

		private async Task<IEnumerable<SelectListItem>> GetCategoryList()
		{
			var categories = (await _dishCategoryRepository.GetAllAsync()).Select(c => new SelectListItem
			{
				Value = c.CatId.ToString(),
				Text = c.CatName
			});
			return categories;
		}

		public async Task<IActionResult> Index()
		{
			var dishes = await _dishRepository.GetAllAsync();
			var dishList = dishes.Select(dish => new DishViewModel(dish)).ToList();

			var allCategories = await GetCategoryList();
			foreach (var dishViewModel in dishList)
			{
				dishViewModel.CategoryOptions = allCategories;
			}
			return View("DishView", dishList);
		}

		[Route("Search")]
		public async Task<IActionResult> Search(string keyword)
		{
			var dishes = await _dishRepository.GetAllAsync();
			if (string.IsNullOrWhiteSpace(keyword))
			{
				var dishList = dishes.Select(dish => new DishViewModel(dish)).ToList();
				var allCategories = await GetCategoryList();
				foreach (var dishViewModel in dishList)
				{
					dishViewModel.CategoryOptions = allCategories;
				}

				return View("DishView", dishList);
			}
			var filteredDishes = dishes
				.Where(d => d.DishName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
				.ToList();
			var filteredDishList = filteredDishes.Select(dish => new DishViewModel(dish)).ToList();
			var allCategoriesForFilter = await GetCategoryList();
			foreach (var dishViewModel in filteredDishList)
			{
				dishViewModel.CategoryOptions = allCategoriesForFilter;
			}
			return View("DishView", filteredDishList);
		}

		[Route("Filter")]
		public async Task<IActionResult> Filter(List<int> selectedCategories)
		{
			var dishes = await _dishRepository.GetAllAsync();

			if (selectedCategories == null || !selectedCategories.Any())
			{
				var dishList = dishes.Select(dish => new DishViewModel(dish)).ToList();

				var allCategories = await GetCategoryList();
				foreach (var dishViewModel in dishList)
				{
					dishViewModel.CategoryOptions = allCategories;
				}

				return View("DishView", dishList);
			}

			var filteredDishes = dishes
				.Where(d => selectedCategories.Contains(d.CategoryId)).ToList();

			var filteredDishList = filteredDishes.Select(dish => new DishViewModel(dish)).ToList();

			var allCategoriesForFilter = await GetCategoryList();
			foreach (var dishViewModel in filteredDishList)
			{
				dishViewModel.CategoryOptions = allCategoriesForFilter;
			}

			return View("DishView", filteredDishList);
		}

		[Route("Create")]
		public async Task<IActionResult> Create()
		{
			var dishViewModel = new DishViewModel
			{
				CategoryOptions = await GetCategoryList()
			};
			return View("CreateDishView", dishViewModel);
		}

		[Route("Create")]
		[HttpPost]
		public async Task<IActionResult> Create(DishViewModel dishViewModel)
		{
			if (!ModelState.IsValid)
			{
				dishViewModel.CategoryOptions = await GetCategoryList();
				return View("CreateDishView", dishViewModel);
			}

			try
			{
				var dish = new Dish
				{
					DishName = dishViewModel.DishName,
					Price = dishViewModel.Price,
					Description = dishViewModel.Description,
					Visible = dishViewModel.Visible,
					CategoryId = dishViewModel.CategoryId,
				};

				var createdDish = await _dishRepository.InsertAsync(dish);

				if (dishViewModel.UploadedImage != null && dishViewModel.UploadedImage.Length > 0)
				{
					await _fileUploadManager.UploadImageAsync(dishViewModel.UploadedImage, "Dishes", $"{createdDish.DishId}.jpg");
				}
			}
			catch (ArgumentException e)
			{
				TempData["Error"] = e.Message;
				return View("CreateDishView", dishViewModel);
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while creating dish. Please try again later.";
				return View("CreateDishView", dishViewModel);
			}

			return RedirectToAction("Index");
		}

		[Route("Details/{id}")]
		public async Task<IActionResult> Details(int id)
		{
			var dish = await _dishRepository.GetByIDAsync(id);
			if (dish == null)
			{
				return NotFound();
			}

			var dishViewModel = new DishViewModel(dish);
			dishViewModel.CategoryOptions = await GetCategoryList();
			return View("DetailsDishView", dishViewModel);
		}

		[Route("Edit/{id}")]
		public async Task<IActionResult> Edit(int id)
		{
			var dish = await _dishRepository.GetByIDAsync(id);
			if (dish == null)
			{
				return NotFound();
			}

			var dishViewModel = new DishViewModel(dish);
			dishViewModel.CategoryOptions = await GetCategoryList();
			return View("EditDishView", dishViewModel);
		}

		[HttpPost]
		[Route("Edit/{DishId}")]
		public async Task<IActionResult> Edit(DishViewModel dish, int DishId)
		{
			if (!ModelState.IsValid)
			{
				dish.CategoryOptions = await GetCategoryList();
				return View("EditDishView", dish);
			}

			try
			{
				var dishEntity = await _dishRepository.GetByIDAsync(DishId);
				if (dishEntity == null)
				{
					return NotFound();
				}

				dishEntity.DishName = dish.DishName;
				dishEntity.Price = dish.Price;
				dishEntity.Description = dish.Description;
				dishEntity.Visible = dish.Visible;
				dishEntity.CategoryId = dish.CategoryId;

				if (dish.UploadedImage != null && dish.UploadedImage.Length > 0)
				{
					await _fileUploadManager.UploadImageAsync(dish.UploadedImage, "Dishes", $"{DishId}.jpg");
				}

				await _dishRepository.UpdateAsync(dishEntity);
			}
			catch (InvalidOperationException)
			{
				return NotFound();
			}
			catch (Exception e)
			{
				TempData["Error"] = "An error occurred while updating dish. Please try again later.";
				return View("EditDishView", dish);
			}

			return RedirectToAction("Index");
		}

		[Route("Delete/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var dish = await _dishRepository.GetByIDAsync(id);
			if (dish == null)
			{
				return NotFound();
			}

			var dishViewModel = new DishViewModel(dish);
			dishViewModel.CategoryOptions = await GetCategoryList();
			return View("DeleteDishView", dishViewModel);
		}

		[HttpPost]
		[Route("Delete/{DishId}")]
		public async Task<IActionResult> DeleteConfirmed(int DishId)
		{
			try
			{
				await _dishRepository.DeleteAsync(DishId);
				_fileUploadManager.DeleteImage("Dishes", $"{DishId}.jpg");
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while deleting dish. Please try again later.";
				return RedirectToAction("Delete", new { id = DishId });
			}

			return RedirectToAction("Index");
		}
	}
}
