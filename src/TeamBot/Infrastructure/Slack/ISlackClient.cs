using System.Threading.Tasks;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Infrastructure.Slack
{
    public interface ISlackClient
    {
        Task PostAsync(Message message);

        Task SendAsync(string replyTo, string text);
    }
}