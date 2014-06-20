using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TeamBot.Infrastructure.Messages;
using TeamBot.Infrastructure.Slack;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Features.WhosTurn
{
    public class WhosTurnHandler : SlackMessageHandler
    {
        public const string UsersKey = "users";

        public WhosTurnHandler(ISlackClient slack) 
            : base(slack)
        {
        }

        public override string Help()
        {
            return "{botname} who (is|turn) {question}";
        }

        public override async Task Handle(IncomingMessage incomingMessage)
        {
            if (incomingMessage == null)
                throw new ArgumentNullException("incomingMessage");

            var patterns = new Dictionary<string, Func<IncomingMessage, Match, Task>>
		    {
                {  @"who (is|turn) @?([\w .\-]+)\?*$", async (message, match) => await TurnAsync(message, match.Groups[2].Value) },
                {  @"load users (.*)", async (message, match) => await LoadAsync(message, match.Groups[1].Value) },
		    };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(incomingMessage.Text, pattern.Key, RegexOptions.IgnoreCase);
                if (match.Length > 0)
                    await pattern.Value(incomingMessage, match);
            }
        }

        private async Task LoadAsync(IncomingMessage incomingMessage, string input)
        {
            var users = new List<string>();
            var names = input.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            
            users.AddRange(names.Where(name => name.ToLower() != incomingMessage.BotName));
            Brain[UsersKey] = string.Join(",", users);

            var response = string.Format("@{0} users loaded. [{1}]", incomingMessage.UserName, string.Join(", ", users));
            await Slack.SendAsync(incomingMessage.ReplyTo(), response);
        }

        private async Task TurnAsync(IncomingMessage incomingMessage, string input)
        {   
            var users = new List<string>();
            object value;
            if (Brain.TryGetValue(UsersKey, out value))
            {
                users.AddRange(value.ToString().Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries));
            }
            else
            {
                await Slack.SendAsync(incomingMessage.ReplyTo(), string.Format("@{0} no users loaded.", incomingMessage.UserName));
                return;
            }

            var random = new Random();
            var user = users[random.Next(users.Count())];

            var response = string.Format("@{0} is {1}", user, input);
            await Slack.SendAsync(incomingMessage.ReplyTo(), response);
        }
    }
}