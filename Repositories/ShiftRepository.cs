using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Models;
using Models.Entities;
using Repositories.Interface;

namespace Repositories
{
	public class ShiftRepository : IShiftRepository, IDisposable
	{
		private readonly DBContext _context;

		public ShiftRepository(DBContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Shift>> GetAllAsync()
		{
			string sqlGetAllShifts = "SELECT * FROM Shifts";

			return await _context.Shifts.FromSqlRaw(sqlGetAllShifts).ToListAsync();
			
		}

		public async Task<Shift?> GetByIDAsync(int id)
		{
			string sqlGetShiftById = "SELECT * FROM Shifts WHERE ShiftId = {0}";
			return await _context.Shifts.FromSqlRaw(sqlGetShiftById, id).FirstOrDefaultAsync();
		}

		public async Task<Shift> InsertAsync(Shift shift)
		{
			string sqlShiftInsert = @"INSERT INTO Shifts (ShiftName,StartTime,EndTime)
											VALUES ({0}, {1}, {2});
											SELECT * FROM Shifts WHERE ShiftId = SCOPE_IDENTITY();";

			var insertedShift = await _context.Shifts.FromSqlRaw(sqlShiftInsert, shift.ShiftName, shift.StartTime,shift.EndTime).ToListAsync();

			var shiftInserted = insertedShift.FirstOrDefault();

			if (shiftInserted == null)
			{
				throw new Exception("Failed to insert shift");
			}

			return shiftInserted;
		}

		public async Task DeleteAsync(int id)
		{
			string sqlGetShiftById = "SELECT * FROM Shifts WHERE ShiftId = {0}";

			var shift = await _context.Shifts.FromSqlRaw(sqlGetShiftById,id).FirstOrDefaultAsync();
			if (shift == null)
			{
				throw new Exception($"Shift with id {id} not found");
			}
			await DeleteAsync(shift);
		}

		public async Task DeleteAsync(Shift shift)
		{
			string sqlDeleteShift = "DELETE FROM Shifts WHERE ShiftId = {0}";
			await _context.Database.ExecuteSqlRawAsync(sqlDeleteShift, shift.ShiftId);
		}

		public async Task UpdateAsync(Shift shift)
		{
			string sqlUpdateShift = @"UPDATE Shifts 
											SET 
											ShiftName = {0},
											StartTime = {1},
											EndTime = {2}
											WHERE ShiftId = {3}";
			await _context.Database.ExecuteSqlRawAsync(sqlUpdateShift, shift.ShiftName, shift.StartTime, shift.EndTime, shift.ShiftId);
		}

		public void Dispose()
		{
			_context.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
