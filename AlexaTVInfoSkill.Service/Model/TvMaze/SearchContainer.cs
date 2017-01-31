using System.Runtime.Serialization;

namespace AlexaTVInfoSkill.Service.Model.TvMaze
{
    [DataContract]
    public class SearchContainer
    {
        [DataMember(Name = "show")]
        public TvShow Show { get; set; }

        [DataMember(Name = "score")]
        public decimal Score { get; set; }
    }
}
