using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Nancy.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TeamBot.Features.Giphy
{
    public class GiphyClient : IGiphyClient
    {
        private readonly string _giphyUri;
        private readonly string _giphyApiKey;

        public GiphyClient()
        {
            _giphyUri = ConfigurationManager.AppSettings["GiphyUri"];
            _giphyApiKey = ConfigurationManager.AppSettings["GiphyApiKey"];
        }

        public async Task<string[]> Search(string input, int limit = 10, int offset = 0)
        {
            using (var client = new HttpClient())
            {
                var uri = string.Format("{0}/search?api_key={1}&q={2}&limit={3}&offset={4}", _giphyUri, _giphyApiKey, HttpUtility.UrlEncode(input), limit, offset);

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

        public async Task<string> Random(string input = null)
        {
            using (var client = new HttpClient())
            {
                var uri = input != null
                    ? string.Format("{0}/random?api_key={1}&tag={2}", _giphyUri, _giphyApiKey, HttpUtility.UrlEncode(input))
                    : string.Format("{0}/random?api_key={1}", _giphyUri, _giphyApiKey);

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