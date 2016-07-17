using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Serilog;
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
            return "echo {message}";
        }

        public override async Task<Message> Handle(IncomingMessage incomingMessage)
        {
            if (incomingMessage == null)
                throw new ArgumentNullException("incomingMessage");

            var patterns = new Dictionary<string, Func<IncomingMessage, Match, Task<Message>>>
            {
                {@"^echo (.*)", async (message, match) => await EchoAsync(message, match.Groups[1].Value)},
                {@"^send (.[\w]+) (.*)", async (message, match) => await SendAsync(message, match.Groups[1].Value, match.Groups[2].Value)},
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(incomingMessage.Text, pattern.Key, RegexOptions.IgnoreCase);
                if (match.Length > 0)
                    return await pattern.Value(incomingMessage, match);
            }

            return null;

        }

        private async Task<Message> EchoAsync(IncomingMessage incomingMessage, string input)
        {
            Log.Information("Echo handling {Message} with {Input}", incomingMessage, input);

            var response = incomingMessage.Text.StartsWith(incomingMessage.BotName)
                    ? string.Format("@{0} I speak for myself thank you very much!", incomingMessage.UserName)
                    : input;

            return new Message()
            {
                Text = response,
                UserName = incomingMessage.ReplyTo()
            };
        }

        private async Task<Message> SendAsync(IncomingMessage incomingMessage, string replyTo, string input)
        {
            var response = incomingMessage.Text.StartsWith(incomingMessage.BotName)
                    ? string.Format("@{0} I speak for myself thank you very much!", incomingMessage.UserName)
                    : input;

            return new Message()
            {
                Text = response,
                UserName = replyTo
            };
        }
    }
}