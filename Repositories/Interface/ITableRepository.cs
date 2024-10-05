using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface ITableRepository
    {
        Task<IEnumerable<Table>> GetAllAsync();
        Task<Table?> GetByIDAsync(int id);
        Task InsertAsync(Table table);
        Task DeleteAsync(int id);
        Task DeleteAsync(Table table);
        Task UpdateAsync(Table table);
        Task SaveAsync();
    }
}
