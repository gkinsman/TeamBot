using System.Threading.Tasks;
using Autofac;
using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Raven.Client;
using Raven.Client.Embedded;
using TeamBot.Infrastructure.Messages;
using TeamBot.Infrastructure.Slack;
using TeamBot.Infrastructure.Slack.Models;
using TeamBot.Tests.Messages.Handlers;
using TeamBot.Tests.Specifications;

namespace TeamBot.Tests.Messages
{
    public class WhenProcessingIncomingMessage : AutoSpecificationForAsync<IMessageProcessor>
    {
        private IContainer _container;
        private string _userMessage;
        private IncomingMessage _incomingMessage;
        private ISlackClient _client;
        private IDocumentStore _documentStore;

        protected override async Task<IMessageProcessor> Given()
        {
            SlackContext.Current = Fixture.Create<SlackContext>();

            _client = Fixture.Freeze<ISlackClient>();
            _documentStore = new EmbeddableDocumentStore { RunInMemory = true };
            _documentStore.Initialize();

            return new MessageProcessor(_client, _documentStore, new[] { Fixture.Create<FooHandler>() });
        }

        protected override async Task When()
        {
            _userMessage = Fixture.Create<string>();

            _incomingMessage = Fixture.Build<IncomingMessage>()
                .With(p => p.Text, string.Format("teambot: foo {0}", _userMessage))
                .With(p => p.TriggerWord, "teambot:")
                .Without(p => p.Command)
                .Create();

            await Subject.Process(_incomingMessage);
        }

        [Then]
        public void ShouldProccesTheMessageSuccessfully()
        {
            _client.Received(1).SendAsync(Arg.Is(_incomingMessage.ReplyTo()), Arg.Is(_userMessage));
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