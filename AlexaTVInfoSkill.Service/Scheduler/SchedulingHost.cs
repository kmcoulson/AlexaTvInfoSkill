namespace AlexaTVInfoSkill.Service.Scheduler
{
    public static class SchedulingHost
    {
        private static IScheduler _scheduler;

        public static void Start()
        {
            _scheduler = new CacheCallbackScheduler();

            ScheduleShowListCacheJob();
        }

        public static void Stop()
        {
            _scheduler?.Stop();
        }

        private static void ScheduleShowListCacheJob()
        {
            var schedule = new DailySchedule(1);
            var job = new ShowListCacheJob();

            Schedule(job, schedule);
        }

        private static void Schedule(IJob job, ISchedule schedule)
        {
            _scheduler.Run(job, schedule);
        }
    }
}