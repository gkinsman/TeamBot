using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Autofac;
using TeamBot.Features.TeamCity;
using TeamBot.Infrastructure.Messages;
using TeamBot.Infrastructure.Slack;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Features.Help
{
    public class HelpHandler : SlackMessageHandler
    {
        private readonly ILifetimeScope _scope;

        public HelpHandler(ISlackClient slack, ILifetimeScope scope)
            : base(slack)
        {
            if (scope == null) throw new ArgumentNullException("scope");

            _scope = scope;
        }

        public override async Task Handle(IncomingMessage incomingMessage)
        {
            if (incomingMessage == null)
                throw new ArgumentNullException("incomingMessage");

            var patterns = new Dictionary<string, Func<IncomingMessage, Match, Task>>
		    {
                { "help", async (message, match) => await ListFeaturesAsync(message) },
		    };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(incomingMessage.Text, pattern.Key, RegexOptions.IgnoreCase);
                if (match.Length > 0)
                    await pattern.Value(incomingMessage, match);
            }
        }

        private async Task ListFeaturesAsync(IncomingMessage incomingMessage)
        {
            var fields = _scope.Resolve<IEnumerable<IHandleMessage>>()
                .Where(t => t.GetType() != GetType() && t.GetType() != typeof(TeamCityHandler))
                .OrderBy(t => t.GetType().Name)
                .Select(h => new AttachmentField { Value = h.Help() })
                .ToArray();

            await Slack.PostAsync(new Attachment
            {
                Channel = incomingMessage.ReplyTo(),
                Text = "Availible Features",
                Fields = fields,
            });
        }
    }
}