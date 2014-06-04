using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Raven.Client;
using TeamBot.Infrastructure.Slack;
using TeamBot.Infrastructure.Slack.Models;
using TeamBot.Models;

namespace TeamBot.Infrastructure.Messages
{
    public class MessageProcessor : IMessageProcessor
    {
        private readonly ILifetimeScope _rootScope;
        private readonly IDocumentStore _documentStore;
        private readonly ISlackClient _client;

        public MessageProcessor(
            ILifetimeScope rootScope,
            IDocumentStore documentStore,
            ISlackClient client)
        {
            if (rootScope == null) 
                throw new ArgumentNullException("rootScope");
            
            if (documentStore == null) 
                throw new ArgumentNullException("documentStore");

            if (client == null) 
                throw new ArgumentNullException("client");

            _rootScope = rootScope;
            _documentStore = documentStore;
            _client = client;
        }

        public async Task Process(string company, string token, IncomingMessage incomingMessage)
        {
            if (incomingMessage == null)
                    throw new ArgumentNullException("incomingMessage");

            var values = incomingMessage.Text.Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var command = values[incomingMessage.Command == null ? 1 : 0].ToLower();
                incomingMessage.Text = string.Join(" ", values.Skip(incomingMessage.Command == null ? 2 : 1));

            var messages = new List<Message>();

            try
            {
                using (var scope = _rootScope.BeginLifetimeScope())
                {
                    var handlers = scope.Resolve<IEnumerable<IHandleMessage>>();

                    foreach (var handler in handlers.Where(handler => handler.CanHandle(command)))
                    {
                        using (var session = _documentStore.OpenSession())
                        {
                            var handlerType = handler.GetType().FullName;

                            var model = session.Query<ViewBagModel>()
                                .FirstOrDefault(c => c.Company == company && c.HandlerName == handlerType);

                            if (model == null)
                                model = new ViewBagModel(company, handlerType);

                            handler.ViewBag = model.ViewBag;

                            messages.Add(await handler.Handle(incomingMessage));

                            session.Store(model);
                            session.SaveChanges();
                        }
                    }

                    if (messages.Any() == false)
                    {
                        messages.Add(new Message
                        {
                            Text = string.Format("@{0} Umm, what do you mean by \"{1} {2}\"", incomingMessage.UserName, command, incomingMessage.Text)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                messages.Add(new Message
                {
                    Text = string.Format("@{0} Umm, something went wrong  \"{1} {2}\" {3}", incomingMessage.UserName, command, incomingMessage.Text, ex.Message)
                });
            }

            foreach (var message in messages)
            {
                if (string.IsNullOrEmpty(message.Channel) == false)
                {
                    message.Channel = (incomingMessage.IsSlashCommand() && command != "echo")
                        ? string.Format("@{0}", incomingMessage.UserName)
                        : string.Format("#{0}", incomingMessage.ChannelName);
                }

                message.LinkNames = true;
                message.UnfurlLinks = true;

                await _client.PostMessage(company, token, message);
            }
        }
    }

   
}