using System.Threading.Tasks;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Infrastructure.Messages
{
    public interface IMessageProcessor
    {
        Task Process(IncomingMessage incomingMessage);
    }
}