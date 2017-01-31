using System;

namespace AlexaTVInfoSkill.Service.Scheduler
{
    public class DailySchedule : ISchedule
    {
        private readonly TimeSpan _timeOfDay;

        public DailySchedule(TimeSpan timeOfDay)
        {
            _timeOfDay = timeOfDay;
        }

        public DailySchedule(int hour, int minute = 0)
        {
            if (hour < 0 || hour > 23)
                throw new ArgumentOutOfRangeException("hour");

            if (minute < 0 || minute > 59)
                throw new ArgumentOutOfRangeException("minute");

            _timeOfDay = new TimeSpan(hour, minute, 0);
        }

        public TimeSpan TimeTillNext
        {
            get
            {
                var timeTillNext = _timeOfDay - SystemTime.Now.TimeOfDay;

                // next run is today
                if (timeTillNext > TimeSpan.Zero)
                    return timeTillNext;

                // scheduled time has past - next run is tomorrow
                return timeTillNext.Add(TimeSpan.FromDays(1));
            }
        }

        public override string ToString()
        {
            return "Daily at " + _timeOfDay.ToString(@"hh\:mm");
        }
    }
}
