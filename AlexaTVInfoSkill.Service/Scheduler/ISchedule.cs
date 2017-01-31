using System;

namespace AlexaTVInfoSkill.Service.Scheduler
{
    public interface ISchedule
    {
        TimeSpan TimeTillNext { get; }
    }
}
