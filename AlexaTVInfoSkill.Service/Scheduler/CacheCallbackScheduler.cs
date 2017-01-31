namespace AlexaTVInfoSkill.Service.Scheduler
{
    public class CacheCallbackScheduler : IScheduler
    {
        private readonly ICache<IJob> _cache = new HttpRuntimeCacheAdapter<IJob>();

        public void Run(IJob job, ISchedule schedule)
        {
            ScheduleJob(job, schedule);
        }

        public void Stop()
        {
            // no op
        }

        private void ScheduleJob(IJob job, ISchedule schedule)
        {
            var key = MakeCacheKey(job);

            _cache.Put(key, job, schedule.TimeTillNext, (k, j) => RunJobAndReschedule(j, schedule));
        }

        private void RunJobAndReschedule(IJob job, ISchedule schedule)
        {
            job.Run();

            ScheduleJob(job, schedule);
        }

        private static string MakeCacheKey(IJob job)
        {
            return string.Concat(typeof(CacheCallbackScheduler).Name, "/", job.GetType().Name);
        }
    }
}