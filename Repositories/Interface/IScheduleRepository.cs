﻿using Models.Entities;

namespace Repositories.Interface
{
	public interface IScheduleRepository
	{
		Task<IEnumerable<Schedule>> GetAllAsync();
		Task<Schedule?> GetByIDAsync(long id);
		Task<Schedule> InsertAsync(Schedule schedule);
		Task DeleteAsync(long id);
		Task DeleteAsync(Schedule schedule);
		Task UpdateAsync(Schedule schedule);
	}
}
