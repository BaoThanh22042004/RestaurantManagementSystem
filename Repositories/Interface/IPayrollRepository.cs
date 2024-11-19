using Models.Entities;

namespace Repositories.Interface
{
	public interface IPayrollRepository
	{
		Task<IEnumerable<Payroll>> GetAllAsync();
		Task<Payroll?> GetByIDAsync(long id);
		Task<Payroll?> GetByEmployeeIDAndYearMonthAsync(int empId, byte month, short year);
		Task<Payroll> InsertAsync(Payroll payroll);
		Task DeleteAsync(long id);
		Task DeleteAsync(Payroll payroll);
		Task UpdateAsync(Payroll payroll);
	}
}
