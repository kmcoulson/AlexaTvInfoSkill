namespace AlexaTVInfoSkill.Service.Model
{
    public interface ITvService
    {
        ITvShow GetShow(string id);
        ITvEpisode GetNextEpisode(string showId);
        ITvEpisode GetLastEpisode(string showId);
    }
}