using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamBot.Infrastructure.Messages;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Features.WhosTurn
{
    public class WhosTurnHandler : MessageHandler
    {
        private const string UsersKey = "users";

        public override string[] Commands()
        {
            return new[] { "who", "users" };
        }

        public override async Task<Message> Handle(IncomingMessage incomingMessage)
        {
            var values = incomingMessage.Text.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);

            var users = new List<string>();

            if (Command == "users")
            {
                users.AddRange(values);
                ViewBag[UsersKey] = string.Join(",", users);
                
                return new Message
                {
                    Text = string.Format("@{0} users loaded. [{1}]", incomingMessage.UserName, string.Join(", ", users))
                };
            }

            object value;
            if (ViewBag.TryGetValue(UsersKey, out value))
            {
                users.AddRange(value.ToString().Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries));
            }
            else
            {
                return new Message
                {
                    Text = string.Format("@{0} no users loaded.", incomingMessage.UserName)
                };   
            } 
            
            var random = new Random();
            var user = users[random.Next(users.Count())];

            return new Message
            {
                Text = string.Format("@{0}'s {1}", user, incomingMessage.Text)
            };
        }
    }
}