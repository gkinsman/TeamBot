using System.Collections.Generic;
using System.Threading.Tasks;
using TeamBot.Infrastructure.Slack;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Infrastructure.Messages
{
    public interface IHandleMessage
    {
        IDictionary<string, object> Brain { get; set; }

        ISlackClient Slack { get; }

        Task Handle(IncomingMessage incomingMessage);
    }
}