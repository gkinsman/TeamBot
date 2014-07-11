using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using TeamBot.Features.WhosTurn;
using TeamBot.Infrastructure.Slack.Models;
using TeamBot.Tests.Specifications;

namespace TeamBot.Tests.Features.WhosTurn
{
    public class WhenReceivingWhoIsRequest : HandlerScenarioBase<WhosTurnHandler>
    {
        private readonly List<string> _users = new List<string> { "user1", "user2", "user3" };

        public override IDictionary<string, object> Brain
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { WhosTurnHandler.UsersKey, string.Join(",", _users) }
                };
            }
        }

        public override string Request
        {
            get { return "who is on support today?"; }
        }

        [Then]
        public override void ShouldRespondCorrectly()
        {
            Subject.Slack.Received().SendAsync(IncomingMessage.ReplyTo(), Arg.Is<string>(message => _users.Any(response => message == string.Format("@{0} is {1}", response, "on support today"))));
        }
    }
    
    public class WhenLoadingUsers : HandlerScenarioBase<WhosTurnHandler>
    {
        private readonly List<string> _users = new List<string> { "user1", "user2", "user3" };

        public override IDictionary<string, object> Brain
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { WhosTurnHandler.UsersKey, string.Join(",", _users) }
                };
            }
        }

        public override string Request
        {
            get { return string.Format("load users {0}", string.Join(",", _users)); }
        }

        public override string ExpectedResponse
        {
            get { return string.Format("@{0} users loaded. [{1}]", IncomingMessage.UserName, string.Join(", ", _users)); }
        }
    }
    
    public class WhenAskingWhosTurnIsIt : HandlerScenarioBase<WhosTurnHandler>
    {
        private readonly List<string> _users = new List<string> { "user1" };

        public override IDictionary<string, object> Brain
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { WhosTurnHandler.UsersKey, string.Join(",", _users) }
                };
            }
        }

        public override string Request
        {
            get { return string.Format("who is on beer o'clock next week?"); }
        }

        public override string ExpectedResponse
        {
            get { return string.Format("@{0} is on beer o'clock next week", "user1"); }
        }
    }
}