using Models.Entities;

namespace WebApp.Models
{
	public class StorageWithLogViewModel : StorageViewModel
	{
		public IEnumerable<StorageLogViewModel> Logs { get; set; }

		public StorageWithLogViewModel(Storage storage) : base(storage)
		{
		}
	}
}
