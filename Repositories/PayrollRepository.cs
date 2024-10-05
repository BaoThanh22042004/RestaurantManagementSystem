using Microsoft.EntityFrameworkCore;
using Models;
using Models.Entities;
using Repositories.Interface;

namespace Repositories
{
	public class PayrollRepository : IPayrollRepository, IDisposable
	{
		private readonly DBContext _context;

		public PayrollRepository(DBContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Payroll>> GetAllAsync()
		{
			return await _context.Payrolls.AsNoTracking().ToListAsync();
		}

		public async Task<Payroll?> GetByIDAsync(long id)
		{
			return await _context.Payrolls.FindAsync(id);
		}

		public async Task InsertAsync(Payroll payroll)
		{
			await _context.Payrolls.AddAsync(payroll);
			await SaveAsync();
		}

		public async Task DeleteAsync(long id)
		{
			var payroll = await _context.Payrolls.FindAsync(id);
			if (payroll == null)
			{
				throw new Exception($"Payroll with id {id} not found");
			}
			await DeleteAsync(payroll);
		}

		public async Task DeleteAsync(Payroll payroll)
		{
			if (_context.Entry(payroll).State == EntityState.Detached)
			{
				_context.Payrolls.Attach(payroll);
			}
			_context.Payrolls.Remove(payroll);
			await SaveAsync();
		}

		public async Task UpdateAsync(Payroll payroll)
		{
			_context.Payrolls.Attach(payroll);
			_context.Entry(payroll).State = EntityState.Modified;
			await SaveAsync();
		}

		public async Task SaveAsync()
		{
			await _context.SaveChangesAsync();
		}

		public void Dispose()
		{
			_context.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
