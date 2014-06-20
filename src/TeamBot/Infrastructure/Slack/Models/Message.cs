using Newtonsoft.Json;

namespace TeamBot.Infrastructure.Slack.Models
{
    public class Message
    {
        public Message()
        {
            UnfurlLinks = true;
            LinkNames = true;
        }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "icon_url")]
        public string IconUrl { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "channel")]
        public string Channel { get; set; }

        [JsonProperty(PropertyName = "unfurl_links")]
        public bool UnfurlLinks { get; set; }

        [JsonProperty(PropertyName = "link_names")]
        public bool LinkNames { get; set; }
    }
}