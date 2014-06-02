using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
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

            var resource = incomingMessage.Text.ToUpper();

            if (string.IsNullOrEmpty(resource))
            {
                return new Message
                {
                    Text = string.Format("@{0} what are you trying to capture?", incomingMessage.UserName),
                    Channel = string.Format("#{0} ", incomingMessage.ChannelName)
                };
            }

            if (Command == "capture")
            {
                string text;
                object value;
                if (ViewBag.TryGetValue(resource, out value) == false)
                {
                    ViewBag[resource] = incomingMessage.UserName;

                    text = string.Format("@everyone {0} captured by @{1}", incomingMessage.Text, incomingMessage.UserName);
                }
                else
                {
                    text = string.Format("@{0} {1} was captured by @{2}", incomingMessage.UserName, incomingMessage.Text, value);
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
                            Title = key,
                            Value = string.Format("@{0}", ViewBag[key]),
                            Short = true,
                        });
                    }
                    
                    return new Attachment
                    {
                        PreText = "Current Inmates",
                        Fields = fields,
                        Channel = string.Format("#{0} ", incomingMessage.ChannelName),
                        LinkNames = true
                    };
                }
                
                string text;
                object value;
                if (ViewBag.TryGetValue(resource, out value) == false)
                {
                    ViewBag.Remove(resource);

                    text = string.Format("@everyone {0} was released by @{1}", incomingMessage.Text, incomingMessage.UserName);
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