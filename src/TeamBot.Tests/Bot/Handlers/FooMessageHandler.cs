using System.Threading.Tasks;
using TeamBot.Infrastructure.Messages;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Tests.Bot.Handlers
{
    public class FooMessageHandler : MessageHandler
    {
        public override string[] Commands()
        {
            return new [] { "foo" };
        }

        public override async Task<Message> Handle(IncomingMessage incomingMessage)
        {
            return new Message
            {
                Text = incomingMessage.Text
            };
        }
    }
}