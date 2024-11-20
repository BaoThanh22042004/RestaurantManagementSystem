using Microsoft.EntityFrameworkCore;
using Models;
using Models.Entities;
using Repositories.Interface;

namespace Repositories
{
	public class FinancialReportRepository : IFinancialReportRepository, IDisposable
	{
		private readonly DBContext _context;

		public FinancialReportRepository(DBContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<FinancialReport>> GetAllAsync()
		{
			string sql = "SELECT * FROM FinancialReports";
			return await _context.FinancialReports.FromSqlRaw(sql).ToListAsync();
		}

		public void Dispose()
		{
			_context.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
