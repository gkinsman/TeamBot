using Newtonsoft.Json;

namespace TeamBot.Infrastructure.Slack.Models
{
    public class AttachmentField
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        [JsonProperty(PropertyName = "short")]
        public bool Short { get; set; }
    }
}