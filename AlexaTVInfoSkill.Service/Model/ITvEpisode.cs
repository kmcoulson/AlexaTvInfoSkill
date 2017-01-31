using System;

namespace AlexaTVInfoSkill.Service.Model
{
    public interface ITvEpisode
    {
        string Id { get; set; }
        string Name { get; set; }
        int Season { get; set; }
        int Number { get; set; }
        DateTime? OriginalAirDate { get; set; }
        int Runtime { get; set; }
        string Summary { get; set; }
    }
}
