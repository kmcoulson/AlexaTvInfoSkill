using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AlexaTVInfoSkill.Service.Model.Imdb
{
    [DataContract]
    public class ImdbResponse
    {
        [DataMember(Name = "data")]
        public ImdbData Data { get; set; }
    }

    [DataContract]
    public class ImdbData
    {
        [DataMember(Name = "movies")]
        public List<TvShow> Shows { get; set; }
    }
}
