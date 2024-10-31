using Microsoft.EntityFrameworkCore;
using Models;
using Models.Entities;
using Repositories.Interface;

namespace Repositories
{
	public class ScheduleRepository : IScheduleRepository, IDisposable
	{
		private readonly DBContext _context;

		public ScheduleRepository(DBContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Schedule>> GetAllAsync()
		{
			string sqlGetAllSchedules = "SELECT * FROM Schedules";
			string sqlGetShiftsByIds = "SELECT * FROM Shifts WHERE ShiftId IN ({0})";
			string sqlGetUsersByIds = "SELECT * FROM Users WHERE UserId IN ({0})";
			string sqlGetAttendancesByIds = "SELECT * FROM Attendances WHERE ScheId IN ({0})";

			var schedules = await _context.Schedules.FromSqlRaw(sqlGetAllSchedules).ToListAsync();

			//Include Shift
			var shiftIds = schedules.Select(s => s.ShiftId).Distinct().ToList();
			var shifts = await _context.Shifts.FromSqlRaw(string.Format(sqlGetShiftsByIds, string.Join(",", shiftIds))).ToDictionaryAsync(s => s.ShiftId);
			foreach (var schedule in schedules)
			{
				if (shifts.TryGetValue(schedule.ShiftId, out var shift))
				{
					schedule.Shift = shift;
				}
			}

			//Include User
			var userIds = schedules.Select(s => s.EmpId).Distinct().ToList();
			var users = await _context.Users.FromSqlRaw(string.Format(sqlGetUsersByIds, string.Join(",", userIds))).ToDictionaryAsync(u => u.UserId);
			foreach (var schedule in schedules)
			{
				if (users.TryGetValue(schedule.EmpId, out var user))
				{
					schedule.Employee = user;
				}
			}

			//Include Attendance

			var scheIds = schedules.Select(s => s.ScheId).Distinct().ToList();
			var scheOfAttendances = await _context.Attendances.FromSqlRaw(string.Format(sqlGetAttendancesByIds, string.Join(",", scheIds))).ToDictionaryAsync(s => s.ScheId);
			foreach (var schedule in schedules)
			{
				if (scheOfAttendances.TryGetValue(schedule.ScheId, out var attendance))
				{
					schedule.Attendance = attendance;
				}
			}

			return schedules;
		}

		public async Task<Schedule?> GetByIDAsync(long id)
		{
			string sqlGetScheduleById = "SELECT * FROM Schedules WHERE ScheId = {0}";
			string sqlGetShiftById = "SELECT * FROM Shifts WHERE ShiftId = {0}";
			string sqlGetUserById = "SELECT * FROM Users WHERE UserId = {0}";
			string sqlGetAttendanceById = "SELECT * FROM Attendances WHERE ScheId = {0}";

			// Get schedule by ID
			var schedule = await _context.Schedules.FromSqlRaw(sqlGetScheduleById, id).FirstOrDefaultAsync();
			if (schedule == null)
			{
				return null;
			}

			//Include Shift
			var shift = await _context.Shifts.FromSqlRaw(sqlGetShiftById, schedule.ShiftId).FirstOrDefaultAsync();
			if (shift != null)
			{
				schedule.Shift = shift;
			}

			//Include Employee
			var employee = await _context.Users.FromSqlRaw(sqlGetUserById, schedule.EmpId).FirstOrDefaultAsync();
			if (employee != null)
			{
				schedule.Employee = employee;
			}

			//Include Attendance
			var attendance = await _context.Attendances.FromSqlRaw(sqlGetAttendanceById, schedule.ScheId).FirstOrDefaultAsync();
			if (attendance != null)
			{
				schedule.Attendance = attendance;
			}

			return schedule;
		}

		public async Task<Schedule> InsertAsync(Schedule schedule)
		{
			string sqlInsertSchedule = @"INSERT INTO Schedules (ScheDate, EmpId, ShiftId)
									VALUES ({0}, {1}, {2});
									SELECT * FROM Schedules WHERE ScheId = SCOPE_IDENTITY();";

			var insertedSchedules = await _context.Schedules.FromSqlRaw(sqlInsertSchedule,
				schedule.ScheDate, schedule.EmpId, schedule.ShiftId).ToListAsync();

			var scheduleInserted = insertedSchedules.FirstOrDefault();

			if (scheduleInserted == null)
			{
				throw new Exception("Failed to insert schedule");
			}

			return scheduleInserted;
		}

		public async Task DeleteAsync(long id)
		{

			string sqlGetScheduleById = "SELECT * FROM Schedules WHERE ScheId = {0}";

			var schedule = await _context.Schedules.FromSqlRaw(sqlGetScheduleById, id).FirstOrDefaultAsync();

			if (schedule == null)
			{
				throw new Exception($"Schedule with id {id} not found");
			}
			await DeleteAsync(schedule);
		}

		public async Task DeleteAsync(Schedule schedule)
		{
			string sqlDeleteSchedule = "DELETE FROM Schedules WHERE ScheId = {0}";
			await _context.Database.ExecuteSqlRawAsync(sqlDeleteSchedule, schedule.ScheId);
		}

		public async Task UpdateAsync(Schedule schedule)
		{
			string sqlUpdateSchedule = @"UPDATE Schedules
									SET ScheDate = {0}, EmpId = {1}, ShiftId = {2}
									WHERE ScheId = {3}";

			await _context.Database.ExecuteSqlRawAsync(sqlUpdateSchedule, schedule.ScheDate, schedule.EmpId, schedule.ShiftId, schedule.ScheId);

		}

		public void Dispose()
		{
			_context.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
