using System;
using System.Runtime.Serialization;

namespace AlexaTVInfoSkill.Service.Model
{
    [DataContract]
    public class Character
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "url")]
        public Uri Url { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}
