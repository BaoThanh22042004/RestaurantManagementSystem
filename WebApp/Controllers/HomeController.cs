using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repositories.Interface;
using WebApp.Models;
using WebApp.Utilities;

namespace WebApp.Controllers
{
	public class HomeController : Controller
	{
		private readonly IDishRepository _dishRepository;
		private readonly IDishCategoryRepository _dishCategoryRepository;
		private readonly InformationManager _informationManager;

		public HomeController(IDishRepository dishRepository, IDishCategoryRepository categoryRepository, InformationManager informationManager)
		{
			_dishRepository = dishRepository;
			_dishCategoryRepository = categoryRepository;
			_informationManager = informationManager;
		}

		public IActionResult Index()
		{
			var informationViewModel = _informationManager.Information;

			return View("HomeView", informationViewModel);
		}

		private async Task<List<DishCategoryViewModel>> GetCategoryList(List<DishViewModel> dishList)
		{
			var result = await _dishCategoryRepository.GetAllAsync();
			result = result.Where(category => dishList.Any(dish => dish.CategoryId.Equals(category.CatId)));
			return result.Select(category => new DishCategoryViewModel(category)).ToList();
		}

		private async Task<List<DishViewModel>> GetDishList()
		{
			var dishes = (await _dishRepository.GetAllAsync()).Where(d => d.Visible);
			return dishes.Select(d => new DishViewModel(d)).ToList();
		}

		public async Task<IActionResult> Menu(MenuViewModel menuViewModel)
		{
			menuViewModel.Dishes ??= await GetDishList();
			menuViewModel.Categories ??= await GetCategoryList(menuViewModel.Dishes);
			menuViewModel.SelectedCategories ??= new List<int>();

			if (string.IsNullOrWhiteSpace(menuViewModel.Keyword) && menuViewModel.SelectedCategories.Count == 0)
			{
				return View("MenuView", menuViewModel);
			}

			if (!string.IsNullOrWhiteSpace(menuViewModel.Keyword))
			{
				menuViewModel.Dishes = menuViewModel.Dishes.Where(d => d.DishName.Contains(menuViewModel.Keyword, StringComparison.OrdinalIgnoreCase)).ToList();
			}

			if (menuViewModel.SelectedCategories.Count != 0)
			{
				menuViewModel.SelectedCategories.ForEach(selected => menuViewModel.Categories.Find(c => c.CategoryId.Equals(selected)).IsSelected =true);
				menuViewModel.Dishes = menuViewModel.Dishes.Where(d => menuViewModel.SelectedCategories.Contains(d.CategoryId)).ToList();
			}

			return View("MenuView", menuViewModel);
		}

		public async Task<IActionResult> Details(int id)
		{
			var dish = await _dishRepository.GetByIDAsync(id);
			if (dish == null)
			{
				return NotFound();
			}

			var dishViewModel = new DishViewModel(dish);
			dishViewModel.CategoryOptions = new List<SelectListItem>
			{
				new SelectListItem
				{
					Text = dishViewModel.CategoryName,
					Value = dishViewModel.CategoryId.ToString(),
				}
			};


			return View("DetailsDishView", dishViewModel);
		}
	}
}
