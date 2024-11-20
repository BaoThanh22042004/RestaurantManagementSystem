using Models.Entities;
using Quartz;

namespace WebApp.Utilities
{
	public class QuartzJobScheduler
	{
		private readonly ISchedulerFactory _schedulerFactory;
		private readonly IScheduler _scheduler;

		public QuartzJobScheduler(ISchedulerFactory schedulerFactory)
		{
			_schedulerFactory = schedulerFactory;
			_scheduler = _schedulerFactory.GetScheduler().Result;
		}

		public async Task ScheduleCheckAttendanceJob(int shiftId, TimeOnly startTime, TimeOnly endTime)
		{
			var jobKey = new JobKey($"ShiftCheck_{shiftId}");

			// Remove existing job if rescheduling
			if (await _scheduler.CheckExists(jobKey))
			{
				await _scheduler.DeleteJob(jobKey);
			}

			var job = JobBuilder.Create<CheckAttendanceJob>()
				.WithIdentity(jobKey)
				.UsingJobData("ShiftId", shiftId)
				.Build();

			var startTimeCheckTime = startTime.AddMinutes(31);
			var triggerStartTime = TriggerBuilder.Create()
				.WithIdentity($"ShiftStartTimeTrigger_{shiftId}")
				.StartAt(DateBuilder.TodayAt(startTimeCheckTime.Hour, startTimeCheckTime.Minute, 0))
				.Build();

			var endTimeCheckTime = endTime.AddMinutes(31);
			var triggerEndTime = TriggerBuilder.Create()
				.WithIdentity($"ShiftEndTimeTrigger_{shiftId}")
				.StartAt(DateBuilder.TodayAt(endTimeCheckTime.Hour, endTimeCheckTime.Minute, 0))
				.Build();

			var triggers = new List<ITrigger> { triggerStartTime, triggerEndTime };

			await _scheduler.ScheduleJob(job, triggers, true);
		}

		public async Task UnScheduleJob(int shiftId)
		{
			var jobKey = new JobKey($"ShiftCheck_{shiftId}");
			if (await _scheduler.CheckExists(jobKey))
			{
				await _scheduler.DeleteJob(jobKey);
			}
		}

	}
}
