using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Repositories.Interface;
using WebApp.Models;

namespace WebApp.Controllers
{
	[Route("Dashboard/Category")]
	[Authorize(Roles = $"{nameof(Role.Manager)}")]
	public class DishCategoryController : Controller
	{
		private readonly IDishCategoryRepository _dishCategoryRepository;

		public DishCategoryController(IDishCategoryRepository dishCategoryRepository)
		{
			_dishCategoryRepository = dishCategoryRepository;
		}

		public async Task<IActionResult> Index()
		{
			var dishes = await _dishCategoryRepository.GetAllAsync();
			var dishList = dishes.Select(dish => new DishCategoryViewModel(dish));
			return View("DishCategoryView", dishList);
		}

		[Route("Search")]
		public async Task<IActionResult> Search(string keyword)
		{
			if (string.IsNullOrWhiteSpace(keyword))
			{
				return RedirectToAction("Index");
			}

			var dishes = await _dishCategoryRepository.GetAllAsync();
			dishes = dishes.Where(d => d.CatName.Contains(keyword, StringComparison.OrdinalIgnoreCase));
			var dishList = dishes.Select(dish => new DishCategoryViewModel(dish)).ToList();

			return View("DishCategoryView", dishList);
		}

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return PartialView("_CreateDishCategoryModal");
        }

        [HttpPost("Create")]
		public async Task<IActionResult> Create(DishCategoryViewModel categoryViewModel)
		{

			if (!ModelState.IsValid)
			{
                return PartialView("_CreateDishCategoryModal", categoryViewModel);
            }

			try
			{
				var category = new DishCategory
				{
					CatName = categoryViewModel.CategoryName
				};

				await _dishCategoryRepository.InsertAsync(category);
			}
            catch (ArgumentException e)
            {
                TempData["Error"] = e.Message;
                return PartialView("_CreateDishCategoryModal", categoryViewModel);
            }
            catch (Exception)
			{
				TempData["Error"] = "An error occurred while creating the category. Please try again.";
                return PartialView("_CreateDishCategoryModal", categoryViewModel);
			}

            return Json(new { success = true });
        }

		[Route("Details/{id}")]
		public async Task<IActionResult> Details(int id)
		{
			var category = await _dishCategoryRepository.GetByIDAsync(id);
			if (category == null)
			{
				return NotFound();
			}
			var categoryViewModel = new DishCategoryViewModel(category);
            return PartialView("_DetailsDishCategoryModal", categoryViewModel);
		}

		[Route("Edit/{id}")]
		public async Task<IActionResult> Edit(int id)
		{
			var category = await _dishCategoryRepository.GetByIDAsync(id);
			if (category == null)
			{
				return NotFound();
			}

			var categoryViewModel = new DishCategoryViewModel(category);
            return PartialView("_EditDishCategoryModal", categoryViewModel);
        }

		[HttpPost]
		[Route("Edit/{id}")]
		public async Task<IActionResult> Edit(DishCategoryViewModel categoryViewModel)
		{
			if (!ModelState.IsValid)
			{
                return PartialView("_EditDishCategoryModal", categoryViewModel);
			}

			try
			{
				var id = categoryViewModel.CategoryId ?? throw new KeyNotFoundException();
				var categoryEntity = await _dishCategoryRepository.GetByIDAsync(id);
				if (categoryEntity == null)
				{
					throw new KeyNotFoundException();
				}

				categoryEntity.CatName = categoryViewModel.CategoryName;
				await _dishCategoryRepository.UpdateAsync(categoryEntity);
			}
			catch (KeyNotFoundException)
			{
				return NotFound();
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while updating the dish category. Please try again later.";
                return PartialView("_EditDishCategoryModal", categoryViewModel);
            }

            return Json(new { success = true });
        }

		[Route("Delete/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var dishCategory = await _dishCategoryRepository.GetByIDAsync(id);
			if (dishCategory == null)
			{
				return NotFound();
			}

			var categoryViewModel = new DishCategoryViewModel(dishCategory);
			return PartialView("_DeleteDishCategoryModal", categoryViewModel);
		}

		[HttpPost]
		[Route("Delete/{CategoryId}")]
		public async Task<IActionResult> DeleteConfirmed(int CategoryId)
		{
			try
			{
				await _dishCategoryRepository.DeleteAsync(CategoryId);
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while deleting the dish category. Please try again later.";
				return RedirectToAction("Delete", new { id = CategoryId });
			}

            return Json(new { success = true });
        }
	}
}
