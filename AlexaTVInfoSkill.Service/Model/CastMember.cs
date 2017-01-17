using System.Runtime.Serialization;

namespace AlexaTVInfoSkill.Service.Model
{
    [DataContract]
    public class CastMember
    {
        [DataMember(Name = "person")]
        public Person Person { get; set; }

        [DataMember(Name = "character")]
        public Person Character { get; set; }
    }
}
