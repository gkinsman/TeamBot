using System.Collections.Generic;
using System.Threading.Tasks;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Infrastructure.Messages
{
    public interface IHandleMessage
    {
        string[] Commands();

        string Command { get; }
        
        string BotName { get; set; }
        
        IDictionary<string, object> ViewBag { get; set; }

        bool CanHandle(string command);

        Task<Message> Handle(IncomingMessage incomingMessage);
    }
}