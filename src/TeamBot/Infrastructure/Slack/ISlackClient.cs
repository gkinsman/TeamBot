using System.Threading.Tasks;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Infrastructure.Slack
{
    public interface ISlackClient
    {
        Task PostMessage(string company, string token, Message message);

        Task PostMessage(string company, string token, string text, string channel);
    }
}