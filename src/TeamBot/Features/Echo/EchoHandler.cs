using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TeamBot.Infrastructure.Messages;
using TeamBot.Infrastructure.Slack;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Features.Echo
{
    public class EchoHandler : SlackMessageHandler
    {
        public EchoHandler(ISlackClient slack) 
            : base(slack)
        {
        }

        public override string Help()
        {
            return "{botname} echo {message}";
        }

        public override async Task Handle(IncomingMessage incomingMessage)
        {
            if (incomingMessage == null)
                throw new ArgumentNullException("incomingMessage");

            var patterns = new Dictionary<string, Func<IncomingMessage, Match, Task>>
            {
                {"echo (.*)", async (message, match) => await EchoAsync(message, match.Groups[1].Value)},
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(incomingMessage.Text, pattern.Key, RegexOptions.IgnoreCase);
                if (match.Length > 0)
                    await pattern.Value(incomingMessage, match);
            }
        }

        private async Task EchoAsync(IncomingMessage incomingMessage, string input)
        {
            var response = incomingMessage.Text.StartsWith(incomingMessage.BotName)
                    ? string.Format("@{0} I speak for myself thank you very much!", incomingMessage.UserName)
                    : input;

            await Slack.SendAsync(incomingMessage.ReplyTo(), response);
        }
    }
}