using Models.Entities;

namespace Repositories.Interface
{
	public interface IBillRepository
	{
		Task<IEnumerable<Bill>> GetAllAsync();
		Task<Bill?> GetByIDAsync(long id);
		Task InsertAsync(Bill bill);
		Task DeleteAsync(long id);
		Task DeleteAsync(Bill bill);
		Task UpdateAsync(Bill bill);
		Task SaveAsync();
	}
}
