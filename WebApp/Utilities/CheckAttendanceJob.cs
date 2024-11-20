using Models.Entities;
using Quartz;
using Repositories.Interface;

namespace WebApp.Utilities
{
	public class CheckAttendanceJob : IJob
	{
		private readonly IAttendanceRepository _attendanceRepository;
		private readonly IScheduleRepository _scheduleRepository;
		public CheckAttendanceJob(IAttendanceRepository attendanceRepository, IScheduleRepository scheduleRepository)
		{
			_attendanceRepository = attendanceRepository;
			_scheduleRepository = scheduleRepository;
		}

		public async Task Execute(IJobExecutionContext context)
		{
			var today = DateOnly.FromDateTime(DateTime.Now);
			var shiftId = context.JobDetail.JobDataMap.GetInt("ShiftId");
			var schedules = (await _scheduleRepository.GetAllAsync()).Where(s => s.ShiftId == shiftId && s.ScheDate == today);
			
			foreach (var schedule in schedules)
			{
				var attendance = (await _attendanceRepository.GetAllAsync()).FirstOrDefault(a => a.ScheId == schedule.ScheId);
				var currentTime = TimeOnly.FromDateTime(DateTime.Now);

				if (attendance != null)
				{
					if (currentTime > schedule.Shift.EndTime.AddMinutes(31) && attendance.Status != AttendanceStatus.ClockOut)
					{
						attendance.Status = AttendanceStatus.Absent;
						attendance.WorkingHours = 0;
						await _attendanceRepository.UpdateAsync(attendance);
					}
					
				}
				else if (currentTime > schedule.Shift.StartTime.AddMinutes(31))
				{
					attendance = new Attendance
					{
						CheckIn = DateTime.MinValue,
						ScheId = schedule.ScheId,
						WorkingHours = 0,
						Status = AttendanceStatus.Absent
					};
					await _attendanceRepository.InsertAsync(attendance);
				}

			}
		}
	}
}
