using System.Linq;
using System.Threading.Tasks;
using Autofac;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Shouldly;
using TeamBot.Infrastructure.Messages;
using TeamBot.Infrastructure.Slack.Models;
using TeamBot.Tests.Bot.Handlers;
using TeamBot.Tests.Specifications;

namespace TeamBot.Tests.Bot
{
    public class WhenProcessingIncomingMessage : AutoSpecificationForAsync<IMessageProcessor>
    {
        private IContainer _container;
        private Message _slackMessage;
        private string _userMessage;
        private IncomingMessage _incomingMessage;

        [SetUp]
        public override void SetUp()
        {
            var builder = new ContainerBuilder();
            
            builder.RegisterType(typeof(FooMessageHandler))
                .Named("foo", typeof(IHandleMessage))
                .InstancePerLifetimeScope();

            _container = builder.Build();

            base.SetUp();
        }

        protected override async Task<IMessageProcessor> Given()
        {
            return new MessageProcessor(_container, null);
        }

        protected override async Task When()
        {
            _userMessage = Fixture.Create<string>();

            _incomingMessage = Fixture.Build<IncomingMessage>()
                .With(p => p.Text, string.Format("teamcitybot: foo {0}", _userMessage))
                .With(p => p.TriggerWord, "teamcitybot:")
                .Without(p => p.Command)
                .Create();

            await Subject.Process(_incomingMessage);
        }

        [Then]
        public void Should()
        {
            _slackMessage.Text.ShouldBe(_userMessage);
        }

        [TearDown]
        public override void TearDown()
        {
            var container = _container;
            if (container != null) container.Dispose();
            _container = null;

            base.TearDown();
        }
    }
}