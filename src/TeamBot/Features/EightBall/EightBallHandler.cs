using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TeamBot.Infrastructure.Messages;
using TeamBot.Infrastructure.Slack;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Features.EightBall
{
    public class EightBallHandler : SlackMessageHandler
    {
        public const string ResponsesKey = "Responses";

        public EightBallHandler(ISlackClient slack)
            : base(slack)
        {   
        }

        public override string Help()
        {
            return "(eightball|8ball) {question}";
        }

        public override async Task Handle(IncomingMessage incomingMessage)
        {
            if (incomingMessage == null) 
                throw new ArgumentNullException("incomingMessage");
            
            var patterns = new Dictionary<string, Func<IncomingMessage, Match, Task>> 
		    {
                { "^eightball (.*)", async (message, match) => await RespondAsync(message) },
			    { "^8ball (.*)", async (message, match) => await RespondAsync(message) },
		    };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(incomingMessage.Text, pattern.Key, RegexOptions.IgnoreCase);
                if (match.Length > 0)
                    await pattern.Value(incomingMessage, match);
            }
        }

        private string[] GetResponses()
        {
            object value;
            if (Brain.TryGetValue(ResponsesKey, out value) == false)
            {
                var responses = new[]
                {
                    "It is certain",
                    "It is decidedly so",
                    "Without a doubt",
                    "Yes – definitely",
                    "You may rely on it",
                    "As I see it, yes",
                    "Most likely",
                    "Outlook good",
                    "Signs point to yes",
                    "Yes",
                    "Reply hazy, try again",
                    "Ask again later",
                    "Better not tell you now",
                    "Cannot predict now",
                    "Concentrate and ask again",
                    "Don't count on it",
                    "My reply is no",
                    "My sources say no",
                    "Outlook not so good",
                    "Very doubtful",
                };

                Brain[ResponsesKey] = string.Join(",", responses);
                return responses;
            }
            else
            {
                return ((string) value).Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        private async Task RespondAsync(IncomingMessage incomingMessage)
        {   
            var random = new Random();
            var responses = GetResponses();
            var response = responses[random.Next(responses.Length)];

            await Slack.SendAsync(incomingMessage.ReplyTo(), string.Format("@{0} {1}", incomingMessage.UserName, response));
        }
    }
}