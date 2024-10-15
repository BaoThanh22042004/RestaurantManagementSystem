using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using Repositories;
using Repositories.Interface;
using System.Linq;
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

        private async Task<IEnumerable<SelectListItem>> GetCategoryList()
        {
            var categories = (await _dishCategoryRepository.GetAllAsync()).Select(s => new SelectListItem
            {
                Value = s.CatId.ToString(),
                Text = s.CatName
            });
            return categories;
        }

        [Route("Search")]
        public async Task<IActionResult> Search(string keyword)
        {
            var dishes = await _dishCategoryRepository.GetAllAsync();

            var dishList = string.IsNullOrWhiteSpace(keyword)
                ? dishes.Select(dish => new DishCategoryViewModel(dish) { HasSearched = false }).ToList()
                : dishes
                    .Where(d => d.CatName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    .Select(dish => new DishCategoryViewModel(dish) { HasSearched = true }) // Set HasSearched to true
                    .ToList();

            var allCategories = await GetCategoryList();

            foreach (var DishCategoryViewModel in dishList)
            {
                DishCategoryViewModel.CategoryOptions = allCategories;
            }

            return View("DishCategoryView", dishList);
        }

        [Route("Create")]
        public IActionResult Create()
        {
            return View("CreateDishCategoryView");
        }

        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> Create(DishCategoryViewModel categoryViewModel)
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
            var categoryViewModel = new DishCategoryViewModel(category);
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

            var categoryViewModel = new DishCategoryViewModel(category);
            return View("EditDishCategoryView", categoryViewModel);
        }

        [HttpPost]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(DishCategoryViewModel categoryViewModel)
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

            var categoryViewModel = new DishCategoryViewModel(dishCategory);
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
