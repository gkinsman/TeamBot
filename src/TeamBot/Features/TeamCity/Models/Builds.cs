using System.Collections.Generic;
using Newtonsoft.Json;

namespace TeamBot.Features.TeamCity.Models
{
    public class Builds
    {
        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }

        [JsonProperty(PropertyName = "href")]
        public string Link { get; set; }

        [JsonProperty(PropertyName = "nextHref")]
        public string NextLink { get; set; }

        public IEnumerable<Build> Build { get; set; }
    }
}