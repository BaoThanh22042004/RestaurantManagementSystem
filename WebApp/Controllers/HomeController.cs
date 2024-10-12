using Microsoft.AspNetCore.Mvc;
using Repositories.Interface;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDishRepository _dishRepository;

        public HomeController(IDishRepository dishRepository)
        {
            _dishRepository = dishRepository;
        }

        public IActionResult Index()
        {
            return View("HomeView");
        }

        public async Task<IActionResult> Menu()
        {
            var dishes = await _dishRepository.GetAllAsync();
            var dishList = dishes.Where(m => m.Visible).Select(dish => new DishViewModel(dish));
            return View("DishView", dishList);
        }

        [Route("Search")]
        public async Task<IActionResult> Search(string keyword)
        {
            var dishes = await _dishRepository.GetAllAsync();
            var dishList = dishes
            .Where(d => d.Visible && d.DishName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .Select(dish => new DishViewModel(dish));
            return View("DishView", dishList);
        }

        public async Task<IActionResult> Details(int id)
        {
            var dish = await _dishRepository.GetByIDAsync(id);
            if (dish == null)
            {
                return NotFound();
            }

            var dishViewModel = new DishViewModel(dish);
            return View("DetailsDishView", dishViewModel);
        }
    }
}
