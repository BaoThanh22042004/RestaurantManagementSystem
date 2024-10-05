using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IStorageLogRepository
    {
        Task<IEnumerable<StorageLog>> GetAllAsync();
        Task<StorageLog?> GetByIDAsync(long id); 
        Task InsertAsync(StorageLog storageLog);
        Task DeleteAsync(long id); 
        Task DeleteAsync(StorageLog storageLog);
        Task UpdateAsync(StorageLog storageLog);
        Task SaveAsync();
    }
}
