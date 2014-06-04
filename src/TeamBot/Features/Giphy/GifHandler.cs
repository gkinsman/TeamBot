using System;
using System.Linq;
using System.Threading.Tasks;
using TeamBot.Infrastructure.Messages;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Features.Giphy
{
    public class GiphyHandler : MessageHandler
    {
        private readonly IGiphyClient _client;

        public GiphyHandler(IGiphyClient client)
        {
            if (client == null) 
                throw new ArgumentNullException("client");

            _client = client;
        }

        public override string[] Commands()
        {
            return new[] { "giphy", "gif" };
        }

        public override async Task<Message> Handle(IncomingMessage incomingMessage)
        {
            try
            {
                if (string.IsNullOrEmpty(incomingMessage.Text))
                {
                    return new Message
                    {
                        Text = await _client.Random(),
                    };
                }

                if (incomingMessage.Text.ToLower().StartsWith("random"))
                {
                    return new Message
                    {
                        Text = await _client.Random(),
                    };
                }

                var random = new Random();
                var offset = random.Next(3);
                var results = await _client.Search(incomingMessage.Text, 10, offset);
                var max = results.Count();

                if (max > 0)
                {
                    int index = random.Next(max);
                
                    return new Message
                    {
                        Text = results[index],
                    };
                }
                else
                {
                    return new Message
                    {
                        Text = string.Format("@{0} {1}", incomingMessage.UserName, "https://i.chzbgr.com/maxW500/6153751552/hC85366D2/"),
                    };
                }
            }
            catch (Exception ex)
            {
                return new Message
                {
                    Text = string.Format("@{0} Umm... something went wrong  \"gif {1}\" {2}", incomingMessage.UserName, incomingMessage.Text, ex.Message),
                };
            }
        }
    }
}