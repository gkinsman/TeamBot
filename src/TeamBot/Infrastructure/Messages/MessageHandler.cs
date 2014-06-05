using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Infrastructure.Messages
{
    public abstract class MessageHandler : IHandleMessage
    {
        public abstract string[] Commands();

        public string Command { get; private set; }

        private string _botName;

        public string BotName
        {
            get { return _botName; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _botName = value;
            }
        }

        private IDictionary<string, object> _viewBag; 
        public IDictionary<string, object> ViewBag 
        { 
            get { return _viewBag; }
            set
            {   
                if (value == null)
                    throw new ArgumentNullException("value");

                _viewBag = value;
            } 
        }

        public virtual bool CanHandle(string command)
        {
            if (command == null) 
                throw new ArgumentNullException("command");

            var commands = Commands() ?? Enumerable.Empty<string>().ToArray();
            var contains = commands.Contains(command.ToLower());

            Command = contains 
                ? command 
                : null;

            return contains;
        }

        public abstract Task<Message> Handle(IncomingMessage incomingMessage);
    }
}