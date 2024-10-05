﻿using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IAttendanceRepository
    {
        Task<IEnumerable<Attendance>> GetAllAsync();
        Task<Attendance?> GetByIDAsync(long id);
        Task InsertAsync(Attendance attendance);
        Task DeleteAsync(long id);
        Task DeleteAsync(Attendance attendance);
        Task UpdateAsync(Attendance attendance);
        Task SaveAsync();
    }
}
