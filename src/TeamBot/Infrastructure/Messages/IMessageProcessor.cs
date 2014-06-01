using System.Threading.Tasks;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Infrastructure.Messages
{
    public interface IMessageProcessor
    {
        Task Process(string company, string token, IncomingMessage incomingMessage);
    }
}