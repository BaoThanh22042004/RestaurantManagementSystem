using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IDishCategoryRepository
    {
        Task<IEnumerable<DishCategory>> GetAllAsync();
        Task<DishCategory?> GetByIDAsync(int id);
        Task InsertAsync(DishCategory dishCategory);
        Task DeleteAsync(int id);
        Task DeleteAsync(DishCategory dishCategory);
        Task UpdateAsync(DishCategory dishCategory);
        Task SaveAsync();
    }
}
