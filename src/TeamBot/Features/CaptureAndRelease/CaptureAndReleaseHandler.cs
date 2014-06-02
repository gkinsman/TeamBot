using System;
using System.Threading;
using System.Threading.Tasks;
using TeamBot.Infrastructure.Messages;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Features.CaptureAndRelease
{
    //teambot capture frog | teambot release frog

    public class CaptureAndReleaseHandler : MessageHandler
    {
        public override string[] Commands()
        {
            return new[] {"capture","release"};
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
                    text = string.Format("@{0} {1} is being held by @{2}", incomingMessage.UserName, incomingMessage.Text, value);
                }

                return new Message
                {
                    Text = text,
                    Channel = string.Format("#{0} ", incomingMessage.ChannelName)
                };
            }
            else //release
            {
                string text;
                object value;
                if (ViewBag.TryGetValue(incomingMessage.Text, out value) == false)
                {
                    ViewBag.Remove(resource);

                    text = string.Format("@everyone {0} was released by @{1}", incomingMessage.Text, incomingMessage.UserName);
                }
                else
                {
                    text = string.Format("@{0} {1} has not been captured", incomingMessage.UserName, value);
                }

                return new Message
                {
                    Text = text,
                    Channel = string.Format("#{0} ", incomingMessage.ChannelName)
                };
            }
        }
    }
}