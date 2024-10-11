using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using Repositories;
using Repositories.Interface;
using System.Collections;
using WebApp.Models;

namespace WebApp.Controllers
{
	[Route("Dashboard/Dish")]
	[Authorize(Roles = $"{nameof(Role.Manager)}")]
	public class DishController : Controller
	{
		private readonly IDishRepository _dishRepository;
		private readonly IDishCategoryRepository _dishCategoryRepository;

		public DishController(IDishRepository dishRepository, IDishCategoryRepository dishCategoryRepository)
		{
			_dishRepository = dishRepository;
			_dishCategoryRepository = dishCategoryRepository;
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
			var dishList = dishes.Select(dish => new DishViewModel(dish));
			return View("DishView", dishList);
		}

        [Route("Search")]
        public async Task<IActionResult> Search(string keyword)
        {
            var dishes = await _dishRepository.GetAllAsync();
            var dishList = dishes.Where(d => d.DishName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .Select(dish => new DishViewModel(dish));
            return View("DishView", dishList);
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

				await _dishRepository.InsertAsync(dish);
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

				await _dishRepository.UpdateAsync(dishEntity);
			}
			catch (InvalidOperationException)
			{
				return NotFound();
			}
			catch (Exception)
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
