using System.Threading.Tasks;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Infrastructure.Slack
{
    public interface ISlackClient
    {
        Task PostMessage(Message message);

        Task PostMessage(string text, string channel);
    }
}