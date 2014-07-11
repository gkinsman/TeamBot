using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TeamBot.Infrastructure.Messages;
using TeamBot.Infrastructure.Slack;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Features.Capture
{
    public class CaptureHandler : SlackMessageHandler
    {
        public CaptureHandler(ISlackClient slack) 
            : base(slack)
        {
        }

        public override string Help()
        {
            return "(capture|release) {resource}";
        }

        public override async Task Handle(IncomingMessage incomingMessage)
        {
            if (incomingMessage == null) 
                throw new ArgumentNullException("incomingMessage");

            var patterns = new Dictionary<string, Func<IncomingMessage, Match, Task>> 
		    {
                { "^capture (.*)", async (message, match) => await CaptureAsync(message, match.Groups[1].Value) },
                { "^lock (.*)", async (message, match) => await CaptureAsync(message, match.Groups[1].Value) },
			    { "^release ?(.*)", async (message, match) => await ReleaseAsync(message, match.Groups[1].Value) },
			    { "^unlock ?(.*)", async (message, match) => await ReleaseAsync(message, match.Groups[1].Value) },
		    };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(incomingMessage.Text, pattern.Key, RegexOptions.IgnoreCase);
                if (match.Length > 0)
                    await pattern.Value(incomingMessage, match);
            }
        }

        private async Task CaptureAsync(IncomingMessage incomingMessage, string resource)
        {
            if (string.IsNullOrEmpty(resource))
            {
                await Slack.SendAsync(incomingMessage.ReplyTo(), string.Format("@{0} what are you trying to capture?", incomingMessage.UserName));
                return;
            }

            string text;
            object value;
            if (Brain.TryGetValue(resource, out value) == false)
            {
                Brain[resource] = incomingMessage.UserName;

                text = string.Format("{0} captured by @{1}", resource, incomingMessage.UserName);
            }
            else
            {
                text = string.Format("@{0} {1} is being held captive by @{2}", incomingMessage.UserName, resource, value);
            }

            await Slack.SendAsync(incomingMessage.ReplyTo(), text);
        }

        private async Task ReleaseAsync(IncomingMessage incomingMessage, string resource)
        {
            if (string.IsNullOrEmpty(resource))
            {
                var fields = new List<AttachmentField>();

                foreach (var key in Brain.Keys)
                {
                    fields.Add(new AttachmentField
                    {
                        Value = string.Format("{0} is being held captive by @{1}", key, Brain[key])
                    });
                }

                await Slack.PostAsync(new Attachment
                {
                    Channel = incomingMessage.ReplyTo(),
                    Text = "Current inmates",
                    Fields = fields,
                });
                return;
            }

            string text;
            object value;
            if (Brain.TryGetValue(resource, out value))
            {
                if (value.ToString() == incomingMessage.UserName)
                {
                    Brain.Remove(resource);

                    text = string.Format("{0} was released by @{1}", resource, incomingMessage.UserName);
                }
                else
                {
                    text = string.Format("@{0} you can't release {1} can only be released by @{2}", incomingMessage.UserName, resource, value);
                }
            }
            else
            {
                text = string.Format("@{0} {1} has not been captured", incomingMessage.UserName, resource);
            }

            await Slack.SendAsync(incomingMessage.ReplyTo(), text);
        }
    }
}
