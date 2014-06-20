using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TeamBot.Infrastructure.Messages;
using TeamBot.Infrastructure.Slack;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Features.Giphy
{
    public class GiphyHandler : SlackMessageHandler
    {
        public const string GiphyApiKey = "GiphyApiKey";

        private readonly string _giphyUri;

        public GiphyHandler(ISlackClient slack)
            : base(slack)
        {   
            _giphyUri = "http://api.giphy.com/v1/gifs";
        }

        public override string Help()
        {
            return "{botname} (gif|giphy) {search}";
        }

        public override async Task Handle(IncomingMessage incomingMessage)
        {
            if (incomingMessage == null)
                throw new ArgumentNullException("incomingMessage");

            var patterns = new Dictionary<string, Func<IncomingMessage, Match, Task>>
		    {
                { "(giphy|gif)( me)? (.*)", async (message, match) => await GiphyAsync(message, match.Groups[3].Value) },
                { "giphyio( apikey)? (.*)", async (message, match) => await LoadApiKeyAsync(message, match.Groups[2].Value) },
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
            Brain[GiphyApiKey] = input;

            await Slack.SendAsync(incomingMessage.ReplyTo(), string.Format("@{0} Giphy ApiKey Set", incomingMessage.UserName));
        }

        private async Task GiphyAsync(IncomingMessage incomingMessage, string input)
        {
            Exception exception = null;
            try 
            {
                if (string.IsNullOrEmpty(input) || input.ToLower().StartsWith("random"))
                {
                    await Slack.SendAsync(incomingMessage.ReplyTo(), string.Format("@{0} {1}", incomingMessage.UserName, await Random()));
                    return;
                }

                var random = new Random();
                var offset = random.Next(3);
                var results = await Search(incomingMessage.Text, 10, offset);
                var max = results.Count();

                var image = max <= 0
                    ? "https://i.chzbgr.com/maxW500/6153751552/hC85366D2/"
                    : results[random.Next(max)];

                await Slack.SendAsync(incomingMessage.ReplyTo(), string.Format("@{0} {1}", incomingMessage.UserName, image));

            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                var response = string.Format("@{0} Umm... something went wrong  \"{1}\" {2}", incomingMessage.UserName, incomingMessage.Text, exception.Message);
                await Slack.SendAsync(incomingMessage.ReplyTo(), response);
            }
        }

        private async Task<string[]> Search(string input, int limit = 10, int offset = 0)
        {
            using (var client = new HttpClient())
            {
                var giphyApiKey = (string)Brain[GiphyApiKey];
                var uri = string.Format("{0}/search?api_key={1}&q={2}&limit={3}&offset={4}", _giphyUri, giphyApiKey, HttpUtility.UrlEncode(input), limit, offset);

                var response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var results = JsonConvert.DeserializeObject<dynamic>(content);

                int max = results.data.Count ?? 0;

                if (results.meta.status != 200 || max <= 0)
                    return Enumerable.Empty<string>().ToArray();

                var urls = new List<string>();
                foreach (var result in results.data)
                    urls.Add(string.Format("{0}", result.images.fixed_height.url));

                return urls.ToArray();
            }
        }

        private async Task<string> Random(string input = null)
        {
            using (var client = new HttpClient())
            {
                var giphyApiKey = (string)Brain[GiphyApiKey];
                var uri = input != null
                    ? string.Format("{0}/random?api_key={1}&tag={2}", _giphyUri, giphyApiKey, HttpUtility.UrlEncode(input))
                    : string.Format("{0}/random?api_key={1}", _giphyUri, giphyApiKey);

                var response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(content);

                if (result.meta.status != 200 || (result.data as JArray) != null)
                    return null;

                return (string)result.data.image_url;
            }
        }
    }
}