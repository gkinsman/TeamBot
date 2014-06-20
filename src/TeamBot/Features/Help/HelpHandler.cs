using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
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
            if (scope == null) 
                throw new ArgumentNullException("scope");

            _scope = scope;
        }

        public override async Task Handle(IncomingMessage incomingMessage)
        {
            var fields = _scope.Resolve<IEnumerable<IHandleMessage>>()
                .Where(t => t.GetType() != GetType())
                .OrderBy(t => t.GetType().Name)
                .Select(h => new AttachmentField { Value = h.Help() })
                .ToArray();

            await Slack.PostAsync(new Attachment
            {
                Text = "Availible Commands",
                Fields = fields,
            });
        }
    }
}