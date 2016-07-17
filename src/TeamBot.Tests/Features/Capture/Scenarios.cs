using System.Collections.Generic;
using Ploeh.AutoFixture;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Tests.Features.Capture
{
    public class WhenCapturingAFreeResource : HandlerScenarioBase<CaptureHandler>
    {
        public override string Request
        {
            get { return "capture :frog:"; }
        }

        public override string ExpectedResponse
        {
            get { return string.Format("{0} captured by @{1}", ":frog:", IncomingMessage.UserName); }
        }
    }
    
    public class WhenCapturingACapturedResource : HandlerScenarioBase<CaptureHandler>
    {
        public override IDictionary<string, object> Brain
        {
            get
            {
                return new Dictionary<string, object>()
                {
                    {":frog:", "wilma"}
                };
            }
        }

        public override string Request
        {
            get { return "capture :frog:"; }
        }

        public override string ExpectedResponse
        {
            get
            {
                return string.Format("@{0} {1} is being held captive by @{2}", IncomingMessage.UserName, ":frog:", "wilma");
            } 
        }
    }

    public class WhenReleasingACapturedResource : HandlerScenarioBase<CaptureHandler>
    {
        public override IDictionary<string, object> Brain
        {
            get
            {
                return new Dictionary<string, object>()
                {
                    {":frog:", "fred"}
                };
            }
        }

        public override IncomingMessage BuildIncomingMessage()
        {
            return Fixture.Build<IncomingMessage>()
                .Without(p => p.Command)
                .With(p => p.UserName, "fred")
                .With(p => p.Text, Request)
                .Create(); 
        }

        public override string Request
        {
            get { return "release :frog:"; }
        }

        public override string ExpectedResponse
        {
            get { return string.Format("{0} was released by @{1}", ":frog:", IncomingMessage.UserName); } 
        }
    }

    public class WhenReleasingACapturedResourceHeldByAnotherUser : HandlerScenarioBase<CaptureHandler>
    {
        public override IDictionary<string, object> Brain
        {
            get
            {
                return new Dictionary<string, object>()
                {
                    {":frog:", "fred"}
                };
            }
        }

        public override IncomingMessage BuildIncomingMessage()
        {
            return Fixture.Build<IncomingMessage>()
                .Without(p => p.Command)
                .With(p => p.UserName, "wilma")
                .With(p => p.Text, Request)
                .Create();
        }

        public override string Request
        {
            get { return "release :frog:"; }
        }

        public override string ExpectedResponse
        {
            get { return string.Format("@{0} you can't release {1} can only be released by @{2}", IncomingMessage.UserName, ":frog:", "fred"); }
        }
    }
    
    public class WhenReleasingANoneCapturedResource : HandlerScenarioBase<CaptureHandler>
    {
        public override string Request
        {
            get { return "release :frog:"; }
        }

        public override string ExpectedResponse
        {
            get { return string.Format("@{0} {1} has not been captured", IncomingMessage.UserName, ":frog:"); }
        }
    }
}