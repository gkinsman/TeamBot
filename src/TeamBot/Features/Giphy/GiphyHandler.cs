using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TeamBot.Infrastructure.Messages;
using TeamBot.Infrastructure.Slack;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Features.Giphy
{
    public class GiphyHandler : SlackMessageHandler
    {
        private readonly IGiphyClient _giphy;

        public GiphyHandler(ISlackClient slack, IGiphyClient giphy)
            : base(slack)
        {
            if (giphy == null) 
                throw new ArgumentNullException("giphy");

            _giphy = giphy;
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
		    };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(incomingMessage.Text, pattern.Key, RegexOptions.IgnoreCase);
                if (match.Length > 0)
                    await pattern.Value(incomingMessage, match);
            }
        }

        private async Task GiphyAsync(IncomingMessage incomingMessage, string input)
        {
            Exception exception = null;
            try 
            {
                if (string.IsNullOrEmpty(input) || input.ToLower().StartsWith("random"))
                {
                    await Slack.SendAsync(incomingMessage.ReplyTo(), string.Format("@{0} {1}", incomingMessage.UserName, await _giphy.Random()));
                    return;
                }

                var random = new Random();
                var offset = random.Next(3);
                var results = await _giphy.Search(incomingMessage.Text, 10, offset);
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
    }
}