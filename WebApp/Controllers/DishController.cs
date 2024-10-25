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

		private async Task<List<DishCategoryViewModel>> GetCategoryListForFilter(List<DishViewModel> dishList)
		{
			var result = await _dishCategoryRepository.GetAllAsync();
			result = result.Where(category => dishList.Any(dish => dish.CategoryId.Equals(category.CatId)));
			return result.Select(category => new DishCategoryViewModel(category)).ToList();
		}

		private async Task<List<DishViewModel>> GetDishList()
		{
			var dishes = (await _dishRepository.GetAllAsync());
			return dishes.Select(d => new DishViewModel(d)).ToList();
		}

		public async Task<IActionResult> Index(MenuViewModel menuViewModel)
		{
			menuViewModel.Dishes ??= await GetDishList();
			menuViewModel.Categories ??= await GetCategoryListForFilter(menuViewModel.Dishes);
			menuViewModel.SelectedCategories ??= new List<int>();

			if (string.IsNullOrWhiteSpace(menuViewModel.Keyword) && menuViewModel.SelectedCategories.Count == 0)
			{
				return View("DishView", menuViewModel);
			}

			if (!string.IsNullOrWhiteSpace(menuViewModel.Keyword))
			{
				menuViewModel.Dishes = menuViewModel.Dishes.Where(d => d.DishName.Contains(menuViewModel.Keyword, StringComparison.OrdinalIgnoreCase)).ToList();
			}

			if (menuViewModel.SelectedCategories.Count != 0)
			{
				menuViewModel.SelectedCategories.ForEach(selected => menuViewModel.Categories.Find(c => c.CategoryId.Equals(selected)).IsSelected = true);
				menuViewModel.Dishes = menuViewModel.Dishes.Where(d => menuViewModel.SelectedCategories.Contains(d.CategoryId)).ToList();
			}

			return View("DishView", menuViewModel);
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
                dish.CategoryOptions = await GetCategoryList();
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
