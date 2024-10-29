using Microsoft.EntityFrameworkCore;
using Models;
using Models.Entities;
using Repositories.Interface;

namespace Repositories
{
	public class AttendanceRepository : IAttendanceRepository, IDisposable
	{
		private readonly DBContext _context;

		public AttendanceRepository(DBContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Attendance>> GetAllAsync()
		{
			string sqlGetAllAttendances = "SELECT * FROM Attendances";
			return await _context.Attendances.FromSqlRaw(sqlGetAllAttendances)
				.Include(a => a.Schedule)
				.ToListAsync();
		}

		public async Task<Attendance?> GetByIDAsync(long id)
		{
			string sqlGetAttendanceById = "SELECT * FROM Attendances WHERE AttendId = {0}";
			return await _context.Attendances.FromSqlRaw(sqlGetAttendanceById,id).FirstOrDefaultAsync();
		}

		public async Task<Attendance> InsertAsync(Attendance attendance)
		{			

			string sqlInsertAttendance = @"INSERT INTO Attendances (ScheId,CheckIn,CheckOut,WorkingHours,Status)
											VALUES ({0}, {1}, {2}, {3}, {4});
											SELECT * FROM Attendances WHERE AttendId = SCOPE_IDENTITY();";

			var insertedAttendance = await _context.Attendances.FromSqlRaw(sqlInsertAttendance, attendance.ScheId,attendance.CheckIn,attendance.CheckOut,attendance.WorkingHours,attendance.Status).ToListAsync();

			var attendanceInserted = insertedAttendance.FirstOrDefault();

			if (attendanceInserted == null)
			{
				throw new Exception("Failed to insert dish category");
			}

			return attendanceInserted;
		}

		public async Task DeleteAsync(long id)
		{
			string sqlGetAttendanceById = "SELECT * FROM Attendances WHERE AttendId = {0}";

			var attendance = await _context.Attendances.FromSqlRaw(sqlGetAttendanceById, id).FirstOrDefaultAsync();
			if (attendance == null)
			{
				throw new Exception($"Attendance with id {id} not found");
			}
			await DeleteAsync(attendance);
		}

		public async Task DeleteAsync(Attendance attendance)
		{
			string sqlDeleteAttendance = "DELETE FROM Attendances WHERE AttendId = {0}";
			await _context.Database.ExecuteSqlRawAsync(sqlDeleteAttendance, attendance.AttendId);
		}

		public async Task UpdateAsync(Attendance attendance)
		{
			string sqlUpdateAttendance = @"UPDATE Attendances 
											SET ScheId = {0},
											CheckIn = {1},
											CheckOut = {2},
											WorkingHours = {3},
											Status = {4}
											WHERE AttendId = {5}";

			await _context.Database.ExecuteSqlRawAsync(sqlUpdateAttendance, attendance.ScheId, attendance.CheckIn, attendance.CheckOut,attendance.WorkingHours, attendance.Status, attendance.AttendId);
		}

		public void Dispose()
		{
			_context.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
