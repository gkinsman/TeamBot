using System.Collections.Generic;
using System.Threading.Tasks;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Infrastructure.Messages
{
    public interface IHandleMessage
    {
        string[] Commands();

        IDictionary<string, object> ViewBag { get; set; }

        string Command { get; }

        bool CanHandle(string command);

        Task<Message> Handle(IncomingMessage incomingMessage);
    }
}