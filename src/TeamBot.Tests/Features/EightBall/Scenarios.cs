using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using TeamBot.Features.EightBall;
using TeamBot.Infrastructure.Slack.Models;
using TeamBot.Tests.Specifications;

namespace TeamBot.Tests.Features.EightBall
{
    public class Scenarios
    {
        public class WhenReceivingEightBallRequest : HandlerScenarioBase<EightBallHandler>
        {
            private readonly List<string> _responses = new List<string> {"1", "2", "3"};

            public override IDictionary<string, object> Brain
            {
                get
                {
                    return new Dictionary<string, object>
                    {
                        {EightBallHandler.ResponsesKey, string.Join(",", _responses)}
                    };
                }
            }

            public override string Request
            {
                get { return "eightball will it rain today?"; }
            }

            [Then]
            public override void ShouldRespondCorrectly()
            {
                Subject.Slack.Received().SendAsync(IncomingMessage.ReplyTo(), Arg.Is<string>(message => _responses.Any(response => message == string.Format("@{0} {1}", IncomingMessage.UserName, response))));
            }
        }
    }
}