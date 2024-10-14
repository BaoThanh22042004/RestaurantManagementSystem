using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repositories;
using Repositories.Interface;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDishRepository _dishRepository;
        private readonly IDishCategoryRepository _dishCategoryRepository;

        public HomeController(IDishRepository dishRepository, IDishCategoryRepository categoryRepository)
        {
            _dishRepository = dishRepository;
            _dishCategoryRepository = categoryRepository;
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

		public IActionResult Index()
        {
            return View("HomeView");
        }

        public async Task<IActionResult> Menu()
        {
			var dishes = (await _dishRepository.GetAllAsync()).Where(d => d.Visible).ToList();

			var visibleDishes = dishes.Where(d => d.Visible).ToList();

            var dishList = visibleDishes.Select(dish => new DishViewModel(dish)).ToList();

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
			var dishes = (await _dishRepository.GetAllAsync()).Where(d => d.Visible).ToList();

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
			var dishes = (await _dishRepository.GetAllAsync()).Where(d => d.Visible).ToList();

			var allCategories = await GetCategoryList();

			if (selectedCategories == null || !selectedCategories.Any())
			{
				var dishList = dishes.Select(dish => new DishViewModel(dish)).ToList();

				foreach (var dishViewModel in dishList)
				{
					dishViewModel.CategoryOptions = allCategories;
				}

				return View("DishView", dishList);
			}

			var filteredDishes = dishes
				.Where(d => selectedCategories.Contains(d.CategoryId)).ToList();

			var filteredDishList = filteredDishes.Select(dish => new DishViewModel(dish)).ToList();

			foreach (var dishViewModel in filteredDishList)
			{
				dishViewModel.CategoryOptions = allCategories;
			}

			return View("DishView", filteredDishList);
		}



		public async Task<IActionResult> Details(int id)
        {
            var dish = await _dishRepository.GetByIDAsync(id);
            if (dish == null)
            {
                return NotFound();
            }

            var dishViewModel = new DishViewModel(dish)
            {
                CategoryOptions = await GetCategoryList()
            };
            return View("DetailsDishView", dishViewModel);
        }
    }
}
