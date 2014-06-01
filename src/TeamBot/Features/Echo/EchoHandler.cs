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
            return new Message
            {
                Text = incomingMessage.Text,
                Channel = string.Format("#{0}", incomingMessage.ChannelName)
            };
        }
    }
}