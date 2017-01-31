using System;

namespace AlexaTVInfoSkill.Service.Scheduler
{
    public class IntervalSchedule : ISchedule
    {
        public TimeSpan TimeTillNext { get; }

        public IntervalSchedule(TimeSpan interval)
        {
            TimeTillNext = interval;
        }

        public override string ToString()
        {
            return "Interval: " + TimeTillNext;
        }
    }
}
