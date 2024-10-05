using Microsoft.EntityFrameworkCore;
using Models;
using Models.Entities;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return await _context.Attendances.AsNoTracking().ToListAsync();
        }

        public async Task<Attendance?> GetByIDAsync(long id)
        {
            return await _context.Attendances.FindAsync(id);
        }

        public async Task InsertAsync(Attendance attendance)
        {
            await _context.Attendances.AddAsync(attendance);
            await SaveAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null)
            {
                throw new Exception($"Attendance with id {id} not found");
            }
            await DeleteAsync(attendance);
        }

        public async Task DeleteAsync(Attendance attendance)
        {
            if (_context.Entry(attendance).State == EntityState.Detached)
            {
                _context.Attendances.Attach(attendance);
            }
            _context.Attendances.Remove(attendance);
            await SaveAsync();
        }

        public async Task UpdateAsync(Attendance attendance)
        {
            _context.Attendances.Attach(attendance);
            _context.Entry(attendance).State = EntityState.Modified;
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
