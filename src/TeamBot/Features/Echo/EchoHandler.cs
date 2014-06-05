using System.Threading.Tasks;
using TeamBot.Infrastructure.Messages;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Features.Echo
{
    public class EchoHandler : MessageHandler
    {
        public override string[] Commands()
        {
            return new [] { "echo" };
        }

        public override async Task<Message> Handle(IncomingMessage incomingMessage)
        {
            if (incomingMessage.Text.StartsWith(BotName))
            {
                return new Message
                {
                    Text = string.Format("@{0} I speak for myself thank you very much!", incomingMessage.UserName),
                };
            }

            return new Message
            {
                Text = incomingMessage.Text,
            };
        }
    }
}