using Newtonsoft.Json;

namespace AlexaTVInfoSkill.Service.Model
{
    public class RequestSlotLog
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }
}