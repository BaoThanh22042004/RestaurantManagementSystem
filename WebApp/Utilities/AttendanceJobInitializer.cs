
using Repositories.Interface;

namespace WebApp.Utilities
{
	public class AttendanceJobInitializer : IHostedService
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly QuartzJobScheduler _scheduler;

		public AttendanceJobInitializer(IServiceProvider serviceProvider, QuartzJobScheduler scheduler)
		{
			_serviceProvider = serviceProvider;
			_scheduler = scheduler;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			using (var scope = _serviceProvider.CreateScope())
			{
				var scheduleRepository = scope.ServiceProvider.GetRequiredService<IScheduleRepository>();

				// Load today's shifts
				var today = DateOnly.FromDateTime(DateTime.Today);
				var schedules = (await scheduleRepository.GetAllAsync()).Where(s => s.ScheDate == today);

				foreach (var schedule in schedules)
				{
					await _scheduler.ScheduleCheckAttendanceJob(
						schedule.ShiftId,
						schedule.Shift.StartTime,
						schedule.Shift.EndTime
					);
				}
			}
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			// Nothing to do when stopping
			return Task.CompletedTask;
		}
	}
}
