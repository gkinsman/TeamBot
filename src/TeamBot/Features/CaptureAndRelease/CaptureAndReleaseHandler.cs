using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamBot.Infrastructure.Messages;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Features.CaptureAndRelease
{
    public class CaptureAndReleaseHandler : MessageHandler
    {
        public override string[] Commands()
        {
            return new [] { "capture", "release" };
        }

        public override async Task<Message> Handle(IncomingMessage incomingMessage)
        {
            if (incomingMessage == null) 
                throw new ArgumentNullException("incomingMessage");

            var resource = incomingMessage.Text.ToLower();

            if (Command == "capture")
            {
                if (string.IsNullOrEmpty(resource))
                {
                    return new Message
                    {
                        Text = string.Format("@{0} what are you trying to capture?", incomingMessage.UserName),
                        Channel = string.Format("#{0} ", incomingMessage.ChannelName),
                        LinkNames = true
                    };
                }

                string text;
                object value;
                if (ViewBag.TryGetValue(resource, out value) == false)
                {
                    ViewBag[resource] = incomingMessage.UserName;

                    text = string.Format("@everyone {0} captured by @{1}", incomingMessage.Text, incomingMessage.UserName);
                }
                else
                {
                    text = string.Format("@{0} {1} is being held captive by @{2}", incomingMessage.UserName, incomingMessage.Text, value);
                }

                return new Message
                {
                    Text = text,
                    Channel = string.Format("#{0} ", incomingMessage.ChannelName),
                    LinkNames = true,

                };
            }
            else //release
            {
                if (string.IsNullOrEmpty(resource))
                {
                    var fields = new List<AttachmentField>();

                    foreach (var key in ViewBag.Keys)
                    {
                        fields.Add(new AttachmentField
                        {
                            Value = string.Format("{0} is being held captive by @{1}", key, ViewBag[key])
                        });
                    }

                    return new Attachment
                    {
                        Text = "Current inmates",
                        Fields = fields,
                        Channel = string.Format("#{0} ", incomingMessage.ChannelName),
                        LinkNames = true
                    };
                }

                string text;
                object value;
                if (ViewBag.TryGetValue(resource, out value))
                {
                    if (value.ToString() == incomingMessage.UserName)
                    {
                        ViewBag.Remove(resource);

                        text = string.Format("@everyone {0} was released by @{1}", incomingMessage.Text, incomingMessage.UserName);
                    }
                    else
                    {
                        text = string.Format("@{0} you can't release {1} can only be released by @{2}", incomingMessage.UserName, incomingMessage.Text, value);
                    }
                }
                else
                {
                    text = string.Format("@{0} {1} has not been captured", incomingMessage.UserName, incomingMessage.Text);
                }

                return new Message
                {
                    Text = text,
                    Channel = string.Format("#{0} ", incomingMessage.ChannelName),
                    LinkNames = true
                };
            }
        }
    }
}