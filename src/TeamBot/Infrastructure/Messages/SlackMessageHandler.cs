using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamBot.Infrastructure.Slack;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Infrastructure.Messages
{
    public abstract class SlackMessageHandler : IHandleMessage
    {
        private readonly ISlackClient _slack;
        
        private IDictionary<string, object> _brain;

        public SlackMessageHandler(ISlackClient slack)
        {
            if (slack == null) 
                throw new ArgumentNullException("slack");

            _slack = slack;
        }

        public ISlackClient Slack { get { return _slack; } }

        public virtual string Help()
        {
            return string.Empty;
        }
        
        public abstract Task Handle(IncomingMessage incomingMessage);

        public IDictionary<string, object> Brain
        {
            get
            {
                return _brain;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _brain = value;
            }
        }
    }
}