using System.Threading.Tasks;
using NUnit.Framework;
using Ploeh.AutoFixture;
using TeamBot.Features.Echo;
using TeamBot.Infrastructure.Slack.Models;
using TeamBot.Tests.Specifications;

namespace TeamBot.Tests.Features.Echo
{
    [Ignore]
    public class WhenReceivingEchoRequest : AutoSpecificationForAsync<EchoHandler>
    {
        protected IncomingMessage IncomingMessage;
        protected Message Response;

        protected override async Task<EchoHandler> Given()
        {
            IncomingMessage = Fixture.Build<IncomingMessage>()
                .Without(p => p.Command)
                .Create();

            return Fixture.Create<EchoHandler>();
        }

        protected override async Task When()
        {
            Response = await Subject.Handle(IncomingMessage);
        }

        [Then]
        public void ShouldRespondCorrectly()
        {
            Response.Text = IncomingMessage.Text;
        }
    }
}