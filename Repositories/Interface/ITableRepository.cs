using Models.Entities;

namespace Repositories.Interface
{
	public interface ITableRepository
	{
		Task<IEnumerable<Table>> GetAllAsync();
		Task<Table?> GetByIDAsync(int id);
		Task<Table> InsertAsync(Table table);
		Task DeleteAsync(int id);
		Task DeleteAsync(Table table);
		Task UpdateAsync(Table table);
	}
}
