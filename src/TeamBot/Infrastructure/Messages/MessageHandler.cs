using System;
using System.Linq;
using System.Threading.Tasks;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Infrastructure.Messages
{
    public abstract class MessageHandler : IHandleMessage
    {
        public abstract string[] Commands();

        public virtual bool CanHandle(string command)
        {
            if (command == null) 
                throw new ArgumentNullException("command");

            var commands = Commands() ?? Enumerable.Empty<string>().ToArray();
            return commands.Contains(command.ToLower());
        }

        public abstract Task<Message> Handle(IncomingMessage incomingMessage);
    }
}