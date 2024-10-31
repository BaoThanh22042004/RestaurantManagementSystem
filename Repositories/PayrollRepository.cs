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
			string sqlGetAllPayrolls = "SELECT * FROM Payrolls";
			string sqlGetUsersByIds = "SELECT * FROM Users WHERE UserId IN ({0})";
			var payrolls = await _context.Payrolls.FromSqlRaw(sqlGetAllPayrolls).ToListAsync();

			// Include employee
			var employeeIds = payrolls.Select(p => p.EmpId).Distinct().ToList();
			if (employeeIds.Count != 0)
			{
				var employees = await _context.Users.FromSqlRaw(string.Format(sqlGetUsersByIds, string.Join(",", employeeIds))).ToDictionaryAsync(e => e.UserId);
				foreach (var payroll in payrolls)
				{
					if (employees.TryGetValue(payroll.EmpId, out var employee))
					{
						payroll.Employee = employee;
					}
				}
			}
			return payrolls;
		}

		public async Task<Payroll?> GetByIDAsync(long id)
		{
			string sqlGetPayrollById = "SELECT * FROM Payrolls WHERE PayrollId = {0}";
			string sqlGetUserById = "SELECT * FROM Users WHERE UserId = {0}";

			var payroll = await _context.Payrolls.FromSqlRaw(sqlGetPayrollById, id).FirstOrDefaultAsync();

			if (payroll == null)
			{
				return null;
			}

			var employee = await _context.Users.FromSqlRaw(string.Format(sqlGetUserById, payroll.EmpId)).FirstOrDefaultAsync();

			if (employee != null)
			{
				payroll.Employee = employee;
			}

			return payroll;
		}

		public async Task<Payroll> InsertAsync(Payroll payroll)
		{
			string sqlPayrollInsert = @"INSERT INTO Payrolls ([CreatedBy] ,[CreatedAt] ,[EmpId] ,[Month] ,[Year] ,[WorkingHours] ,[Salary] ,[Status] ,[PaymentDate])
											VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8});
											SELECT * FROM Payrolls WHERE PayrollId = SCOPE_IDENTITY();";
			var insertedPayrolls = await _context.Payrolls.FromSqlRaw(sqlPayrollInsert, payroll.CreatedBy, payroll.CreatedAt, payroll.EmpId, payroll.Month, payroll.Year, payroll.WorkingHours, payroll.Salary, payroll.Status, payroll.PaymentDate).ToListAsync();
			var insertedPayroll = insertedPayrolls.FirstOrDefault();


			if (insertedPayroll == null)
			{
				throw new Exception("Failed to insert payroll");
			}

			return insertedPayroll;
		}

		public async Task DeleteAsync(long id)
		{
			string sqlGetPayrollById = "SELECT * FROM Payrolls WHERE PayrollId = {0}";

			var payroll = await _context.Payrolls.FromSqlRaw(sqlGetPayrollById, id).FirstOrDefaultAsync();

			if (payroll == null)
			{
				throw new Exception($"Payroll with id {id} not found");
			}

			await DeleteAsync(payroll);
		}

		public async Task DeleteAsync(Payroll payroll)
		{
			string sqlDeletePayroll = "DELETE FROM Payrolls WHERE PayrollId = {0}";
			await _context.Database.ExecuteSqlRawAsync(sqlDeletePayroll, payroll.PayrollId);
		}

		public async Task UpdateAsync(Payroll payroll)
		{
			string sqlUpdatePayroll = @"UPDATE Payrolls SET [CreatedBy] = {0}, [CreatedAt] = {1}, [EmpId] = {2}, [Month] = {3}, [Year] = {4}, [WorkingHours] = {5}, [Salary] = {6}, [Status] = {7}, [PaymentDate] = {8}
										WHERE PayrollId = {9}";

			await _context.Database.ExecuteSqlRawAsync(sqlUpdatePayroll, payroll.CreatedBy, payroll.CreatedAt, payroll.EmpId, payroll.Month, payroll.Year, payroll.WorkingHours, payroll.Salary, payroll.Status, payroll.PaymentDate, payroll.PayrollId);
		}

		public void Dispose()
		{
			_context.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
