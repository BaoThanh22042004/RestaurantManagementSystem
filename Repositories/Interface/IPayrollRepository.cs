using Models.Entities;

namespace Repositories.Interface
{
	public interface IPayrollRepository
	{
		Task<IEnumerable<Payroll>> GetAllAsync();
		Task<Payroll?> GetByIDAsync(long id);
		Task InsertAsync(Payroll payroll);
		Task DeleteAsync(long id);
		Task DeleteAsync(Payroll payroll);
		Task UpdateAsync(Payroll payroll);
		Task SaveAsync();
	}
}
