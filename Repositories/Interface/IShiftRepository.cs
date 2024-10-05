using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IShiftRepository
    {
        Task<IEnumerable<Shift>> GetAllAsync();
        Task<Shift?> GetByIDAsync(int id);
        Task InsertAsync(Shift shift);
        Task DeleteAsync(int id);
        Task DeleteAsync(Shift shift);
        Task UpdateAsync(Shift shift);
        Task SaveAsync();
    }
}
