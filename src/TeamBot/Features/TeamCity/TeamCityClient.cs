using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TeamBot.Features.TeamCity.Models;

namespace TeamBot.Features.TeamCity
{
    public class TeamCityClient : ITeamCityClient, IDisposable
    {
        private readonly string _teamCityUri;
        private readonly HttpClient _client;

        public TeamCityClient()
        {
            _teamCityUri = ConfigurationManager.AppSettings["TeamCityUri"];
            var teamCityUsername = ConfigurationManager.AppSettings["TeamCityUsername"];
            var teamCityPassword = ConfigurationManager.AppSettings["TeamCityPassword"];

            _client = new HttpClient();

            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(
                    System.Text.Encoding.ASCII.GetBytes(
                        string.Format("{0}:{1}", teamCityUsername, teamCityPassword))));
        }

        public async Task<Builds> GetBuilds()
        {
            var uri = string.Format("{0}/httpAuth/app/rest/builds", _teamCityUri);
         
            var response = await _client.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Builds>(content);
        }

        public async Task<BuildExtended> GetBuild(int id)
        {
            var uri = string.Format("{0}/httpAuth/app/rest/builds/id:{1}", _teamCityUri, id);
            
            var response = await _client.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<BuildExtended>(content);
        }

        public void Dispose()
        {
            var client = _client;
            if (client != null) client.Dispose();
            client = null;
        }
    }
}