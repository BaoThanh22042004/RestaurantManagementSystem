﻿using Models.Entities;

namespace Repositories.Interface
{
	internal interface IScheduleRepository
	{
		Task<IEnumerable<Schedule>> GetAllAsync();
		Task<Schedule?> GetByIDAsync(long id);
		Task InsertAsync(Schedule schedule);
		Task DeleteAsync(long id);
		Task DeleteAsync(Schedule schedule);
		Task UpdateAsync(Schedule schedule);
		Task SaveAsync();
	}
}