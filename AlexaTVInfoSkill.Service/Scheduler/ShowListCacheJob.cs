namespace AlexaTVInfoSkill.Service.Scheduler
{
    public class ShowListCacheJob : IJob
    {
        public void Run()
        {
            TvInfoService.StoreShows();
            AppCache.Clear();
            TvInfoService.GetShows();
        }
    }
}
