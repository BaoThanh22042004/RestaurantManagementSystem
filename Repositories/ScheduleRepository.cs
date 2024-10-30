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
			return await _context.Schedules.FromSqlRaw(sqlGetAllSchedules)
				.Include(s => s.Shift)
				.Include(s => s.Employee)
				.Include(s => s.Attendance)
				.ToListAsync();
		}

		public async Task<Schedule?> GetByIDAsync(long id)
		{
			return await _context.Schedules
				.Include(s => s.Shift)
                .Include(s => s.Employee)
                .Include(s => s.Attendance)
				.FirstOrDefaultAsync(s => s.ScheId == id);
		}

		public async Task<Schedule> InsertAsync(Schedule schedule)
		{
			await _context.Schedules.AddAsync(schedule);
			await SaveAsync();
			return schedule;
		}

		public async Task DeleteAsync(long id)
		{
			var schedule = await _context.Schedules.FindAsync(id);
			if (schedule == null)
			{
				throw new Exception($"Schedule with id {id} not found");
			}
			await DeleteAsync(schedule);
		}

		public async Task DeleteAsync(Schedule schedule)
		{
			if (_context.Entry(schedule).State == EntityState.Detached)
			{
				_context.Schedules.Attach(schedule);
			}
			_context.Schedules.Remove(schedule);
			await SaveAsync();
		}

		public async Task UpdateAsync(Schedule schedule)
		{
			_context.Schedules.Attach(schedule);
			_context.Entry(schedule).State = EntityState.Modified;
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
