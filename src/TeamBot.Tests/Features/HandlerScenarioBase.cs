using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using Ploeh.AutoFixture;
using TeamBot.Infrastructure.Messages;
using TeamBot.Infrastructure.Slack.Models;
using TeamBot.Tests.Specifications;

namespace TeamBot.Tests.Features
{
    public abstract class HandlerScenarioBase<T> : AutoSpecificationForAsync<T>
        where T : IHandleMessage
    {
        public IncomingMessage IncomingMessage { get; private set; }

        public virtual IncomingMessage BuildIncomingMessage()
        {
            return Fixture.Build<IncomingMessage>()
                .Without(p => p.Command)
                .With(p => p.Text, Request)
                .Create();
        }

        protected override async Task<T> Given()
        {
            IncomingMessage = BuildIncomingMessage();

            return Fixture.Build<T>()
                .With(p => p.Brain, Brain)
                .Create();
        }

        public abstract string Request { get; }

        public virtual string ExpectedResponse { get { return string.Empty; } }

        public virtual IDictionary<string, object> Brain
        {
            get { return new Dictionary<string, object>(); }
        }

        protected override async Task When()
        {
            await Subject.Handle(IncomingMessage);
        }

        [Then]
        public virtual void ShouldRespondCorrectly()
        {
            Subject.Slack.Received().SendAsync(Arg.Is(IncomingMessage.ReplyTo()), Arg.Is(ExpectedResponse));
        }
    }
}