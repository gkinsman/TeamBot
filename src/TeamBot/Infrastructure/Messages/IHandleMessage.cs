using System.Threading.Tasks;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Infrastructure.Messages
{
    public interface IHandleMessage
    {
        string[] Commands();
        
        bool CanHandle(string command);

        Task<Message> Handle(IncomingMessage incomingMessage);
    }
}