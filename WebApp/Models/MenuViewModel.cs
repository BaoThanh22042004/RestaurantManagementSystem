namespace WebApp.Models
{
	public class MenuViewModel
	{
		public List<DishViewModel> Dishes { get; set; }
		public List<DishCategoryViewModel> Categories { get; set; }
		public List<int> SelectedCategories { get; set; }
		public string? Keyword { get; set; }
	}
}
