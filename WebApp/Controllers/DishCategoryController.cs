using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Repositories.Interface;
using WebApp.Models;

namespace WebApp.Controllers
{
	[Route("Dashboard/Category")]
	[Authorize(Roles = "Manager")]
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
			var dishList = dishes.Select(dish => new CategoryViewModel(dish));
			return View("DishCategoryView", dishList);
		}

		[Route("Create")]
		public IActionResult Create()
		{
			return View("CreateDishCategoryView");
		}

		[Route("Create")]
		[HttpPost]
		public async Task<IActionResult> Create(CategoryViewModel categoryViewModel)
		{
			if (!ModelState.IsValid)
			{
				return View("CreateDishCategoryView", categoryViewModel);
			}

			try
			{
				var category = new DishCategory
				{
					CatName = categoryViewModel.CategoryName
				};

				await _dishCategoryRepository.InsertAsync(category);
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while creating the category. Please try again.";
				return View("CreateDishCategoryView", categoryViewModel);
			}

			return RedirectToAction("Index");
		}

		[Route("Details/{id}")]
		public async Task<IActionResult> Details(int id)
		{
			var category = await _dishCategoryRepository.GetByIDAsync(id);
			if (category == null)
			{
				return NotFound();
			}
			var categoryViewModel = new CategoryViewModel(category);
			return View("DetailsDishCategoryView", categoryViewModel);
		}

		[Route("Edit/{id}")]
		public async Task<IActionResult> Edit(int id)
		{
			var category = await _dishCategoryRepository.GetByIDAsync(id);
			if (category == null)
			{
				return NotFound();
			}

			var categoryViewModel = new CategoryViewModel(category);
			return View("EditDishCategoryView", categoryViewModel);
		}

		[HttpPost]
		[Route("Edit/{id}")]
		public async Task<IActionResult> Edit(CategoryViewModel categoryViewModel)
		{
			if (!ModelState.IsValid)
			{
				return View("EditDishCategoryView", categoryViewModel);
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
				return View("EditDishCategoryView", categoryViewModel);
			}

			return RedirectToAction("Index");
		}

		[Route("Delete/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var dishCategory = await _dishCategoryRepository.GetByIDAsync(id);
			if (dishCategory == null)
			{
				return NotFound();
			}

			var categoryViewModel = new CategoryViewModel(dishCategory);
			return View("DeleteDishCategoryView", categoryViewModel);
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

			return RedirectToAction("Index");
		}
	}
}
