using System.Text.Json;
using WebApp.Models;

namespace WebApp.Utilities
{
	public class InformationManager
	{
		public InformationViewModel Information { get; private set; }
		public string RestaurantName => Information.RestaurantName;

		public InformationManager()
		{
			Information = LoadInformation();
		}

		public async Task SaveInformation()
		{
			var jsonString = JsonSerializer.Serialize(Information);
			await File.WriteAllTextAsync("restaurant-information.json", jsonString);
		}

		private static InformationViewModel LoadInformation()
		{
			try
			{
				var jsonString = File.ReadAllText("restaurant-information.json");
				return JsonSerializer.Deserialize<InformationViewModel>(jsonString);
			}
			catch
			{
				return new InformationViewModel();
			}
		}
	}
}
