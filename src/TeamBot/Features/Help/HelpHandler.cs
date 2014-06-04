using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using TeamBot.Infrastructure.Messages;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Features.Help
{
    public class HelpHandler : MessageHandler
    {
        private readonly ILifetimeScope _scope;

        public HelpHandler(ILifetimeScope scope)
        {
            if (scope == null) 
                throw new ArgumentNullException("scope");

            _scope = scope;
        }

        public override string[] Commands()
        {
            return new[] { "help" };
        }

        public override async Task<Message> Handle(IncomingMessage incomingMessage)
        {
            var fields = _scope.Resolve<IEnumerable<IHandleMessage>>()
                .Where(t => t.GetType() != GetType())
                .OrderBy(t => t.GetType().Name)
                .Select(h => new AttachmentField { Value = string.Format("{0} ({1}) {{text}}", h.GetType().Name.Substring(0, h.GetType().Name.Length - 7), string.Join(" | ", h.Commands().OrderBy(c => c))) })
                .ToArray();

            return new Attachment
            {
                Text = "Commands",
                Fields = fields,
            };
        }
    }
}