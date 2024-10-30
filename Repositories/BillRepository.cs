using Microsoft.EntityFrameworkCore;
using Models;
using Models.Entities;
using Repositories.Interface;

namespace Repositories
{
	public class BillRepository : IBillRepository, IDisposable
	{
		private readonly DBContext _context;

		public BillRepository(DBContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Bill>> GetAllAsync()
		{
			string sqlGetAllBills = "SELECT * FROM Bills";

			return await _context.Bills.FromSqlRaw(sqlGetAllBills).ToListAsync();
		}

		public async Task<Bill?> GetByIDAsync(long id)
		{
			string sqlGetBillById = "SELECT * FROM Bills WHERE BillId = {0}";

			return await _context.Bills.FromSqlRaw(sqlGetBillById, id).FirstOrDefaultAsync();
		}

		public async Task<Bill> InsertAsync(Bill bill)
		{
			string sqlInsertBill = @"INSERT INTO Bills ([OrderId] ,[CreatedBy] ,[CreatedAt] ,[TotalAmount] ,[PaymentMethod] ,[PaymentTime])
											VALUES ({0}, {1}, {2}, {3}, {4}, {5});
											SELECT * FROM Bills WHERE BillId = SCOPE_IDENTITY();";

			var insertedBill = await _context.Bills.FromSqlRaw(sqlInsertBill, bill.OrderId, bill.CreatedBy, bill.CreatedAt, bill.TotalAmount, bill.PaymentMethod, bill.PaymentTime).ToListAsync();

			var billInserted = insertedBill.FirstOrDefault();

			if (billInserted == null)
			{
				throw new Exception("Failed to insert bill");
			}

			return billInserted;
		}

		public async Task DeleteAsync(long id)
		{
			string sqlGetBillById = "SELECT * FROM Bills WHERE BillId = {0}";

			var bill = await _context.Bills.FromSqlRaw(sqlGetBillById, id).FirstOrDefaultAsync();

			if (bill == null)
			{
				throw new Exception($"Bill with id {id} not found");
			}

			await DeleteAsync(bill);
		}

		public async Task DeleteAsync(Bill bill)
		{
			string sqlDeleteBill = "DELETE FROM Bills WHERE BillId = {0}";
			await _context.Database.ExecuteSqlRawAsync(sqlDeleteBill, bill.BillId);
		}

		public async Task UpdateAsync(Bill bill)
		{
			string sqlUpdateBill = @"UPDATE Bills SET [OrderId] = {0}, [CreatedBy] = {1}, 
													[CreatedAt] = {2}, [TotalAmount] = {3}, 
													[PaymentMethod] = {4}, [PaymentTime] = {5} 
													WHERE BillId = {6}";
			await _context.Database.ExecuteSqlRawAsync(sqlUpdateBill, bill.OrderId, bill.CreatedBy,
																		bill.CreatedAt, bill.TotalAmount,
																		bill.PaymentMethod, bill.PaymentTime,
																		bill.BillId);
		}

		public void Dispose()
		{
			_context.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
