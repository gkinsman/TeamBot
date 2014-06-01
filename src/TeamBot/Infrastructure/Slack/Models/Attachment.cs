using System.Collections.Generic;
using Newtonsoft.Json;

namespace TeamBot.Infrastructure.Slack.Models
{
    public class Attachment : Message
    {
        [JsonProperty(PropertyName = "fallback")]
        public string Fallback { get; set; }

        [JsonProperty(PropertyName = "pretext")]
        public string PreText { get; set; }

        [JsonProperty(PropertyName = "color")]
        public string Color { get; set; }

        [JsonProperty(PropertyName = "fields")]
        public IEnumerable<AttachmentField> Fields { get; set; }
    }
}