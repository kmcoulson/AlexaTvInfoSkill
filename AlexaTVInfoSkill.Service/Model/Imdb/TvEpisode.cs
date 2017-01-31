using System;
using System.Runtime.Serialization;

namespace AlexaTVInfoSkill.Service.Model.Imdb
{
    [DataContract]
    public class TvEpisode : ITvEpisode
    {
        [DataMember(Name = "idIMDB")]
        public string Id { get; set; }

        [DataMember(Name = "title")]
        public string Name { get; set; }

        public int Season { get; set; }

        [DataMember(Name = "episode")]
        public int Number { get; set; }

        [DataMember(Name = "date")]
        public string AirDate { get; set; }

        public DateTime? OriginalAirDate
        {
            get
            {
                DateTime value;
                if (DateTime.TryParse(AirDate.Replace(".", ""), out value))
                    return value;
                return null;
            }
            set
            {
                if (!value.HasValue) return;
                AirDate = value.Value.ToString("yyyy-MM-dd");
            }
        }

        public int Runtime { get; set; }
        public string Summary { get; set; }
    }
}