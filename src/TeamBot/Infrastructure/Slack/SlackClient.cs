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
        public async Task PostAsync(Message message)
        {
            if (message == null) 
                throw new ArgumentNullException("message");

            var uri = string.Format("https://{0}.slack.com/services/hooks/incoming-webhook?token={1}", SlackContext.Current.Company, SlackContext.Current.Token);
            using (var client = new HttpClient())
            {
                var settings = new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore };
                var json = JsonConvert.SerializeObject(message, settings);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, content);
                response.EnsureSuccessStatusCode();
            }
        }

        public async Task SendAsync(string replyTo, string text)
        {
            if (text == null) 
                throw new ArgumentNullException("text");

            if (replyTo == null)
                throw new ArgumentNullException("replyTo");

            var message = new Message
            {
                Channel = replyTo, 
                Text = text
            };

            await PostAsync(message);
        }
    }
}