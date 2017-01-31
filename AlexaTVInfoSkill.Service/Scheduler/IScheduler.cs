namespace AlexaTVInfoSkill.Service.Scheduler
{
    public interface IScheduler
    {
        void Run(IJob job, ISchedule schedule);

        void Stop();
    }
}
