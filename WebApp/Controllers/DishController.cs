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
        private readonly IWebHostEnvironment _environment;

        private readonly string imageUploadsDir;

        public DishController(IDishRepository dishRepository, IDishCategoryRepository dishCategoryRepository, IWebHostEnvironment environment)
        {
            _dishRepository = dishRepository;
            _dishCategoryRepository = dishCategoryRepository;
            _environment = environment;
            imageUploadsDir = Path.Combine(_environment.WebRootPath, "Uploads/Images/Dishes");
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
                    await SaveImageFile(dishViewModel.UploadedImage, createdDish.DishId);
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

        private async Task SaveImageFile(IFormFile file, int dishId)
        {
            var imagePath = Path.Combine(imageUploadsDir, $"{dishId}.jpg");

            if (!Directory.Exists(imageUploadsDir))
            {
                Directory.CreateDirectory(imageUploadsDir);
            }

            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
        }

        private void DeleteImageFile(int dishId)
        {
            var imagePath = Path.Combine(imageUploadsDir, $"{dishId}.jpg");

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
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
                    await SaveImageFile(dish.UploadedImage, DishId);
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
                DeleteImageFile(DishId);
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
