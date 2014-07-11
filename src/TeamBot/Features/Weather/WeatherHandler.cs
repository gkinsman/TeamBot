using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using TeamBot.Infrastructure.Messages;
using TeamBot.Infrastructure.Slack;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Features.Weather
{
    public class WeatherHandler : SlackMessageHandler
    {
        public const string ForcastIoApiKey = "ForcastIoApiKey";

        public WeatherHandler(ISlackClient slack) 
            : base(slack)
        {
        }

        public override string Help()
        {
            return "weather {city}";
        }

        public override async Task Handle(IncomingMessage incomingMessage)
        {
            if (incomingMessage == null)
                throw new ArgumentNullException("incomingMessage");

            var patterns = new Dictionary<string, Func<IncomingMessage, Match, Task>>
		    {
                { "^weather (.*)", async (message, match) => await LookupAddressAsync(message, match.Groups[1].Value) },
                { "^forecastio( apikey)? (.*)", async (message, match) => await LoadApiKeyAsync(message, match.Groups[2].Value) },
		    };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(incomingMessage.Text, pattern.Key, RegexOptions.IgnoreCase);
                if (match.Length > 0)
                    await pattern.Value(incomingMessage, match);
            }
        }

        private async Task LoadApiKeyAsync(IncomingMessage incomingMessage, string input)
        {
            Brain[ForcastIoApiKey] = input;

            await Slack.SendAsync(incomingMessage.ReplyTo(), string.Format("@{0} Forcast.io ApiKey set.", incomingMessage.UserName));
        }

        private async Task LookupAddressAsync(IncomingMessage incomingMessage, string location)
        {
            if (incomingMessage == null) 
                throw new ArgumentNullException("incomingMessage");
            
            if (location == null) 
                throw new ArgumentNullException("location");

            const string googleMapUrl = "http://maps.googleapis.com/maps/api/geocode/json";

            using (var client = new HttpClient())
            {
                var uri = string.Format("{0}?address={1}&sensor={2}", googleMapUrl, HttpUtility.UrlEncode(location), true);
                var response = await client.GetAsync(uri);

                var body = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                var coords = body.results[0].geometry.location;

                var text = await LookupWeatherAsync((decimal)coords.lat, (decimal)coords.lng);

                await Slack.SendAsync(incomingMessage.ReplyTo(), string.Format("@{0} {1}", incomingMessage.UserName, text));
            }
        }

        private async Task<string> LookupWeatherAsync(decimal latitude, decimal longitude)
        {
            var forcastIoApi = (string)Brain[ForcastIoApiKey];
            const string forecastIoUrl = "https://api.forecast.io/forecast";

            using (var client = new HttpClient())
            {
                var uri = string.Format("{0}/{1}/{2},{3}?units={4}", forecastIoUrl, forcastIoApi, latitude, longitude, "ca");
                var response = await client.GetAsync(uri);

                var body = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                var current = body.currently;

                var humidity = (current.humidity * 100);
                var temperature = string.Format("{0} ºC", current.temperature);
                var text = string.Format("It is currently {0} {1}, {2}% humidity", temperature, current.summary, humidity);

                return text;
            }
        }
    }
}