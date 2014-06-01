using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Infrastructure.Slack
{
    public class SlackClient : ISlackClient
    {
        public async Task PostMessage(string company, string token, Message message)
        {
            if (message == null) 
                throw new ArgumentNullException("message");

            var uri = string.Format("https://{0}.slack.com/services/hooks/incoming-webhook?token={1}", company, token);
            using (var client = new HttpClient())
            {
                var settings = new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore };
                var json = JsonConvert.SerializeObject(message, settings);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, content);
                response.EnsureSuccessStatusCode();
            }
        }

        public async Task PostMessage(string company, string token, string text, string channel)
        {
            if (text == null) 
                throw new ArgumentNullException("text");
            
            if (channel == null) 
                throw new ArgumentNullException("channel");

            await PostMessage(company, token, new Message {Text = text, Channel = channel});
        }
    }
}