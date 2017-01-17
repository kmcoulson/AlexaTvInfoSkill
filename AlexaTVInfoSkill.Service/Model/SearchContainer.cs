using System.Runtime.Serialization;
using TVMazeAPI.Models;

namespace AlexaTVInfoSkill.Service.Model
{
    [DataContract]
    public class ShowSearchContainer
    {
        [DataMember(Name = "show")]
        public TvShow Show { get; set; }

        [DataMember(Name = "score")]
        public decimal Score { get; set; }
    }


    [DataContract]
    public class PersonSearchContainer
    {
        [DataMember(Name = "person")]
        public Person Person { get; set; }

        [DataMember(Name = "score")]
        public double Score { get; set; }
    }
}
