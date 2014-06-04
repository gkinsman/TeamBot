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
            if (incomingMessage.Text.StartsWith(incomingMessage.TriggerWord.Replace(":", "")))
            {
                return new Message
                {
                    Text = string.Format("@{0} I speak for myself thank very much!", incomingMessage.UserName),
                };
            }

            return new Message
            {
                Text = incomingMessage.Text,
            };
        }
    }
}