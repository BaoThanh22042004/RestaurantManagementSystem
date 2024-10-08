using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Repositories;
using Repositories.Interface;
using WebApp.Models;

namespace WebApp.Controllers
{
	[Route("Dashboard")]
	[Authorize(Roles = "Manager")]
	public class DishCategoryController : Controller
	{
		private readonly IDishCategoryRepository _dishCategoryRepository;

		public DishCategoryController(IDishCategoryRepository dishCategoryRepository)
		{
			_dishCategoryRepository = dishCategoryRepository;
		}

		[Route("Category")]
		public async Task<IActionResult> Index()
		{
			var dishes = await _dishCategoryRepository.GetAllAsync();
			var dishList = dishes.Select(dish => new CategoryViewModel(dish));
			return View("DishCategoryView", dishList);
		}
		[Route("Category/Create")]
		public IActionResult Create()
		{
			return View("CreateDishCategoryView");
		}

		[Route("Category/Create")]
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
		[Route("Category/Details/{id}")]
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
		[Route("Category/Edit/{id}")]
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
		[Route("Category/Edit/{CategoryId}")]
		public async Task<IActionResult> Edit(CategoryViewModel categoryViewModel, int CategoryId)
		{
			if (!ModelState.IsValid)
			{
				return View("EditDishCategoryView", categoryViewModel);
			}

			try
			{
				var categoryEntity = await _dishCategoryRepository.GetByIDAsync(CategoryId);
				if (categoryEntity == null)
				{
					return NotFound();
				}

				categoryEntity.CatName = categoryViewModel.CategoryName;
				await _dishCategoryRepository.UpdateAsync(categoryEntity);
			}
			catch (InvalidOperationException)
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
		[Route("Category/Delete/{id}")]
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
		[Route("Category/Delete/{CategoryId}")]
		public async Task<IActionResult> DeleteConfirmed(int CategoryId)
		{
			try
			{
				await _dishCategoryRepository.DeleteAsync(CategoryId);
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while deleting the dish category. Please try again later.";
				return RedirectToAction("Delete", new { CategoryId });
			}

			return RedirectToAction("Index");
		}
	}
}
