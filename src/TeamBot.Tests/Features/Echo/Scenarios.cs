using TeamBot.Features.Echo;

namespace TeamBot.Tests.Features.Echo
{
    public class WhenReceivingEchoRequest : HandlerScenarioBase<EchoHandler>
    {
        public override string Request
        {
            get { return "echo Hello World!"; }
        }

        public override string ExpectedResponse
        {
            get { return "Hello World!"; }
        }
    }
}