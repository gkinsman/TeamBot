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

        public MessageProcessor(
            ISlackClient client,
            IEnumerable<IHandleMessage> messageHandlers)
        {
            if (client == null)
                throw new ArgumentNullException("client");

            if (messageHandlers == null)
                throw new ArgumentNullException("messageHandlers");

            _client = client;
            _messageHandlers = messageHandlers;
        }

        public async Task Process(IncomingMessage incomingMessage)
        {
            Log.Debug("Processing {@incomingMessage}", incomingMessage);

            if (incomingMessage == null)
                throw new ArgumentNullException("incomingMessage");

            var values = incomingMessage.Text.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

            var botName = incomingMessage.IsSlashCommand()
                ? incomingMessage.Command.Substring(1)
                : values[0].ToLower().Replace(":", "");

            incomingMessage.BotName = botName;

            var command = values[incomingMessage.Command == null ? 1 : 0].ToLower();
            incomingMessage.Text = string.Join(" ", values.Skip(incomingMessage.IsSlashCommand() ? 0 : 1));

            Exception exception = null;
            try
            {
                foreach (var handler in _messageHandlers)
                {
                    var handlerType = handler.GetType().FullName;
                    var company = SlackContext.Current.Company;

                    Log.Information("Handling message with {HandlerType}", handlerType);

                    await handler.Handle(incomingMessage);
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
    }
}