using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace AlexaTVInfoSkill.Service.Model
{
    public class RequestLog
    {
        [JsonProperty(PropertyName = "requestId")]
        public string RequestId { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "intent")]
        public string Intent { get; set; }

        public string IntentName {
            get
            {
                switch (Intent)
                {
                    case "ShowSynopsisIntent":
                        return "Synopsis";
                    case "ShowStartIntent":
                        return "Show Start Date";
                    case "ShowNextEpisodeIntent":
                        return "Next Episode";
                    case "ShowCastIntent":
                    case "ShowPersonIntent":
                    case "ShowCharacterIntent":
                        return GetCastIntentName();
                    case "AMAZON.CancelIntent":
                        return "Cancel";
                    case "AMAZON.StopIntent":
                        return "Stop";
                    case "AMAZON.HelpIntent":
                        return "Help";
                    default:
                    {
                        switch (Type)
                        {
                            case "LaunchRequest":
                                return "Launch";
                            case "CancelRequest":
                                return "Cancel";
                            case "StopRequest":
                                return "Stop";
                            case "HelpRequest":
                                return "Help";
                                default:
                                return "Unknown Intent";
                        }
                    }
                }
            }
        }

        private string GetCastIntentName()
        {
            if (Slots == null || !Slots.Any()) return "Cast: Unknown";

            if (Slots.Any(x => x.Name == "person")) return "Get Character";

            if (Slots.Any(x => x.Name == "character")) return "Get Actor";

            return "Get Cast";
        }

        public string ShowName {
            get
            {
                if (Slots == null || !Slots.Any()) return "-";

                if (Slots.Any(x => x.Name == "showtitle")) return Slots.First(x => x.Name == "showtitle").Value;

                return "Unknown";
            }
        }

        [JsonProperty(PropertyName = "timeStamp")]
        public DateTime TimeStamp { get; set; }

        [JsonProperty(PropertyName = "responseText")]
        public string ResponseText { get; set; }

        [JsonProperty(PropertyName = "slots")]
        public virtual ICollection<RequestSlotLog> Slots { get; set; }
    }
}