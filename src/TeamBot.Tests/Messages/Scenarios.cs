using System.Threading.Tasks;
using Autofac;
using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Raven.Client;
using TeamBot.Infrastructure.Messages;
using TeamBot.Infrastructure.Slack;
using TeamBot.Infrastructure.Slack.Models;
using TeamBot.Tests.Messages.Handlers;
using TeamBot.Tests.Specifications;

namespace TeamBot.Tests.Messages
{
    [Ignore]
    public class WhenProcessingIncomingMessage : AutoSpecificationForAsync<IMessageProcessor>
    {
        private IContainer _container;
        private string _userMessage;
        private IncomingMessage _incomingMessage;
        private ISlackClient _client;
        private IDocumentStore _documentStore;
        private string _company;
        private string _token;
        private IDocumentSession _session;

        [SetUp]
        public override void SetUp()
        {
            var builder = new ContainerBuilder();
            
            builder.RegisterType(typeof(FooMessageHandler))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            _container = builder.Build();

            base.SetUp();
        }

        protected override async Task<IMessageProcessor> Given()
        {
            _company = string.Format("comapy{0}", Fixture.Create<string>());
            _token = string.Format("token{0}", Fixture.Create<string>());
            _client = Fixture.Freeze<ISlackClient>();
            _session = Fixture.Freeze<IDocumentSession>();
            _documentStore = Fixture.Freeze<IDocumentStore>();
            _documentStore.OpenSession().Returns(_session);

            return new MessageProcessor(_container, _documentStore, _client);
        }

        protected override async Task When()
        {
            _userMessage = Fixture.Create<string>();

            _incomingMessage = Fixture.Build<IncomingMessage>()
                .With(p => p.Text, string.Format("teambot: foo {0}", _userMessage))
                .With(p => p.TriggerWord, "teambot:")
                .Without(p => p.Command)
                .Create();

            await Subject.Process(_company, _token, _incomingMessage);
        }

        [Then]
        public void Should()
        {
            _client.Received(1).PostMessage(Arg.Is(_company), Arg.Is(_token), Arg.Is<Message>(m => m.Text == _userMessage));
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