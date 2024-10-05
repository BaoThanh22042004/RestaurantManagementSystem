using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IDishRepository
    {
        Task<IEnumerable<Dish>> GetAllAsync();
        Task<Dish?> GetByIDAsync(int id);
        Task InsertAsync(Dish dish);
        Task DeleteAsync(int id);
        Task DeleteAsync(Dish dish);
        Task UpdateAsync(Dish dish);
        Task SaveAsync();
    }
}
