using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using TeamBot.Infrastructure.Slack;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Infrastructure.Messages
{
    public class MessageProcessor : IMessageProcessor
    {
        private readonly ILifetimeScope _rootScope;
        private readonly ISlackClient _client;

        public MessageProcessor(
            ILifetimeScope rootScope, 
            ISlackClient client)
        {
            if (rootScope == null) 
                throw new ArgumentNullException("rootScope");
            
            if (client == null) 
                throw new ArgumentNullException("client");

            _rootScope = rootScope;
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
                        messages.Add(await handler.Handle(incomingMessage));
                    }

                    if (messages.Any() == false)
                    {
                        messages.Add(new Message
                        {
                            Text = string.Format("@{0} Umm, what do you mean by \"{1} {2}\"", incomingMessage.UserName, command, incomingMessage.Text),
                            Channel = string.Format("#{0}", incomingMessage.ChannelName)
                        });
                    }
                }
            }
            catch(Exception ex)
            {
                messages.Add(new Message
                {
                    Text = string.Format("@{0} Umm, something went wrong  \"{1} {2}\" {3}", incomingMessage.UserName, command, incomingMessage.Text, ex.Message),
                    Channel = string.Format("#{0}", incomingMessage.ChannelName)
                });
            }

            if (incomingMessage.IsSlashCommand() && command != "echo")
            {
                foreach (var message in messages)
                {
                    message.Channel = string.Format("@{0}", incomingMessage.UserName);
                }
            }

            foreach (var message in messages)
                await _client.PostMessage(company, token, message);
        }
    }
}