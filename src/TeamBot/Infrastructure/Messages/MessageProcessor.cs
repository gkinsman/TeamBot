using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Raven.Client;
using Serilog;
using TeamBot.Infrastructure.Slack;
using TeamBot.Infrastructure.Slack.Models;
using TeamBot.Models;

namespace TeamBot.Infrastructure.Messages
{
    public class MessageProcessor : IMessageProcessor
    {
        private readonly IDocumentStore _documentStore;
        private readonly IEnumerable<IHandleMessage> _messageHandlers;
        private readonly ISlackClient _client;

        public MessageProcessor(
            IDocumentStore documentStore,
            ISlackClient client,
            IEnumerable<IHandleMessage> messageHandlers)
        {
            if (documentStore == null) 
                throw new ArgumentNullException("documentStore");

            if (client == null) 
                throw new ArgumentNullException("client");
            
            if (messageHandlers == null) 
                throw new ArgumentNullException("messageHandlers");

            _documentStore = documentStore;
            _client = client;
            _messageHandlers = messageHandlers;
        }

        public async Task Process(string company, string token, IncomingMessage incomingMessage)
        {
            Log.Debug("Processing {@incomingMessage}",  incomingMessage);

            if (incomingMessage == null)
                    throw new ArgumentNullException("incomingMessage");

            var values = incomingMessage.Text.Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var botName = incomingMessage.IsSlashCommand()
                ? incomingMessage.Command.Substring(1)
                : values[0].ToLower().Replace(":", "");
            
            var command = values[incomingMessage.Command == null ? 1 : 0].ToLower();
                incomingMessage.Text = string.Join(" ", values.Skip(incomingMessage.Command == null ? 2 : 1));

            var messages = new List<Message>();

            try
            {
                foreach (var handler in _messageHandlers.Where(handler => handler.CanHandle(command)))
                {
                    using (var session = _documentStore.OpenSession())
                    {
                        var handlerType = handler.GetType().FullName;

                        var model = session.Query<ViewBagModel>()
                            .FirstOrDefault(c => c.Company == company && c.HandlerName == handlerType);

                        if (model == null)
                            model = new ViewBagModel(company, handlerType);

                        handler.BotName = botName;
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
            catch (Exception ex)
            {
                messages.Add(new Message
                {
                    Text = string.Format("@{0} Umm, something went wrong  \"{1} {2}\" {3}", incomingMessage.UserName, command, incomingMessage.Text, ex.Message)
                });
            }

            foreach (var message in messages)
            {
                message.Channel = (incomingMessage.IsSlashCommand() )
                    ? incomingMessage.UserId
                    : incomingMessage.ChannelId;

                message.LinkNames = true;
                message.UnfurlLinks = true;

                await _client.PostMessage(company, token, message);
            }
        }
    }

   
}