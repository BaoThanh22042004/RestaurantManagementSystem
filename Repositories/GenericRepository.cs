using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
	public class GenericRepository<TEntity> : IDisposable where TEntity : class
	{
		protected readonly DBContext context;
		protected readonly DbSet<TEntity> dbSet;

		public GenericRepository(DBContext context)
		{
			this.context = context;
			dbSet = context.Set<TEntity>();
		}

		public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
		{
			return await dbSet.AsNoTracking().ToListAsync();
		}

		public virtual async Task<TEntity?> GetByIDAsync(object id)
		{
			return await dbSet.FindAsync(id);
		}

		public virtual async Task InsertAsync(TEntity entity)
		{
			await dbSet.AddAsync(entity);
		}

		public virtual async Task DeleteAsync(object id)
		{
			TEntity? entityToDelete = await dbSet.FindAsync(id);
			if (entityToDelete == null)
			{
				throw new Exception($"{nameof(TEntity)} entity with id {id} not found");
			}
			await DeleteAsync(entityToDelete);
		}

		public virtual async Task DeleteAsync(TEntity entityToDelete)
		{
			if (context.Entry(entityToDelete).State == EntityState.Detached)
			{
				dbSet.Attach(entityToDelete);
			}
			dbSet.Remove(entityToDelete);
			await SaveAsync();
		}

		public virtual async Task Update(TEntity entityToUpdate)
		{
			dbSet.Attach(entityToUpdate);
			context.Entry(entityToUpdate).State = EntityState.Modified;
			await SaveAsync();
		}

		public virtual async Task SaveAsync()
		{
			await context.SaveChangesAsync();
		}

		public void Dispose()
		{
			context.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
