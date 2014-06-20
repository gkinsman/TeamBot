using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TeamBot.Infrastructure.Messages;
using TeamBot.Infrastructure.Slack;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Tests.Messages.Handlers
{
    public class FooHandler : SlackMessageHandler
    {
        public FooHandler(ISlackClient slack) 
            : base(slack)
        {
        }

        public override async Task Handle(IncomingMessage incomingMessage)
        {
            var match = Regex.Match(incomingMessage.Text, "foo (.*)", RegexOptions.IgnoreCase);

            if (match.Length > 0)
                await Slack.SendAsync(incomingMessage.ReplyTo(), match.Groups[1].Value);
        }
    }
}