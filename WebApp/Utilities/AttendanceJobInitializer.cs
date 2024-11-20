
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
				var shiftRepository = scope.ServiceProvider.GetRequiredService<IShiftRepository>();

				var shifts = await shiftRepository.GetAllAsync();

				foreach (var shift in shifts)
				{
					await _scheduler.ScheduleCheckAttendanceJob(
						shift.ShiftId,
						shift.StartTime,
						shift.EndTime
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
