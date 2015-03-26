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
        private readonly IEnumerable<IHandleMessage> _messageHandlers;
        private readonly ISlackClient _client;
        private readonly IDocumentStore _documentStore;

        public MessageProcessor(
            ISlackClient client,
            IDocumentStore documentStore,
            IEnumerable<IHandleMessage> messageHandlers)
        {
            if (client == null) 
                throw new ArgumentNullException("client");
            
            if (documentStore == null) 
                throw new ArgumentNullException("documentStore");

            if (messageHandlers == null) 
                throw new ArgumentNullException("messageHandlers");

            _client = client;
            _documentStore = documentStore;
            _messageHandlers = messageHandlers;
        }

        public async Task Process(IncomingMessage incomingMessage)
        {
            Log.Debug("Processing {@incomingMessage}", incomingMessage);

            if (incomingMessage == null)
                throw new ArgumentNullException("incomingMessage");

            incomingMessage.BotName = ExtractBotName(incomingMessage);

            var values = incomingMessage.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var command = values[incomingMessage.Command == null ? 1 : 0].ToLower();
            incomingMessage.Text = string.Join(" ", values.Skip(incomingMessage.IsSlashCommand() ? 0 : 1));

            Exception exception = null;
            try
            {
                foreach (var handler in _messageHandlers)
                {
                    using (var session = _documentStore.OpenSession())
                    {
                        var handlerType = handler.GetType().FullName;
                        var company = SlackContext.Current.Company;
                        var models = session.Query<ViewBagModel>();

                        var model = models.FirstOrDefault(c => c.Company == company && c.HandlerName == handlerType)
                                    ?? new ViewBagModel(company, handlerType);

                        handler.Brain = model.ViewBag;

                        await handler.Handle(incomingMessage);

                        session.Store(model);
                        session.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                var response = string.Format("@{0} Umm, something went wrong  \"{1} {2}\" {3}", incomingMessage.UserName,
                    command, incomingMessage.Text, exception.Message);

                await _client.SendAsync(incomingMessage.ReplyTo(), response);
            }
        }

        private string ExtractBotName(IncomingMessage incomingMessage)
        {
            var values = incomingMessage.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var botName = incomingMessage.IsSlashCommand()
                ? incomingMessage.Command.Substring(1)
                : values[0].ToLower().Replace(":", "");

            return botName;
        }
    }
}