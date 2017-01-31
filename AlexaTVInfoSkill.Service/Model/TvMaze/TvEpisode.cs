using System;
using System.Runtime.Serialization;

namespace AlexaTVInfoSkill.Service.Model.TvMaze
{
    [DataContract]
    public class TvEpisode : ITvEpisode
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "url")]
        public Uri Url { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "season")]
        public int Season { get; set; }

        [DataMember(Name = "number")]
        public int Number { get; set; }

        [DataMember(Name = "airdate")]
        public string AirDate { get; set; }

        [DataMember(Name = "airtime")]
        public string AirTime { get; set; }

        public DateTime? OriginalAirDate
        {
            get
            {
                DateTime value;
                if (DateTime.TryParse($"{AirDate} {AirTime}", out value))
                    return value;
                return null;
            }
            set
            {
                if (!value.HasValue) return;
                AirDate = value.Value.ToString("yyyy-MM-dd");
                AirTime = value.Value.ToString("HH:mm");
            }
        }

        [DataMember(Name = "runtime")]
        public int Runtime { get; set; }

        [DataMember(Name = "summary")]
        public string Summary { get; set; }
    }
}
