using System;
using System.Threading.Tasks;
using TeamBot.Infrastructure.Messages;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Features.EightBall
{
    public class EightBallHandler : MessageHandler
    {
        public override string[] Commands()
        {
            return new [] { "eightball", "8ball" };
        }

        public override async Task<Message> Handle(IncomingMessage incomingMessage)
        {
            var response = new[]
            {
                "It is certain",
                "It is decidedly so",
                "Without a doubt",
                "Yes – definitely",
                "You may rely on it",
                "As I see it, yes",
                "Most likely",
                "Outlook good",
                "Signs point to yes",
                "Yes",
                "Reply hazy, try again",
                "Ask again later",
                "Better not tell you now",
                "Cannot predict now",
                "Concentrate and ask again",
                "Don't count on it",
                "My reply is no",
                "My sources say no",
                "Outlook not so good",
                "Very doubtful",
            };

            var random = new Random();

            return new Message
            {
                Text = string.Format("@{0} {1}", incomingMessage.UserName, response[random.Next(response.Length)]),
            };
        }
    }
}