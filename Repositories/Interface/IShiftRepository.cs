using Models.Entities;

namespace Repositories.Interface
{
	public interface IShiftRepository
	{
		Task<IEnumerable<Shift>> GetAllAsync();
		Task<Shift?> GetByIDAsync(int id);
		Task <Shift> InsertAsync(Shift shift);
		Task DeleteAsync(int id);
		Task DeleteAsync(Shift shift);
		Task UpdateAsync(Shift shift);
	}
}
