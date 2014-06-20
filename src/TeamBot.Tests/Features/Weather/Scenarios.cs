using System.Collections.Generic;
using NSubstitute;
using TeamBot.Features.Weather;
using TeamBot.Infrastructure.Slack.Models;
using TeamBot.Tests.Specifications;

namespace TeamBot.Tests.Features.Weather
{
    public class Scenarios
    {
        public class WhenReceivingWeatherRequest : HandlerScenarioBase<WeatherHandler>
        {
            public override IDictionary<string, object> Brain
            {
                get
                {
                    return new Dictionary<string, object>
                    {
                        {WeatherHandler.ForcastIoApiKey, "aecabcef7974ee5c21f74e0a7943bc44"}
                    };
                }
            }

            public override string Request
            {
                get { return "weather brisbane"; }
            }

            [Then]
            public override void ShouldRespondCorrectly()
            {
                Subject.Slack.Received().SendAsync(IncomingMessage.ReplyTo(), Arg.Any<string>());
            }
        }
        
        public class WhenReceivingWeatherApiKey : HandlerScenarioBase<WeatherHandler>
        {
            public override string Request
            {
                get { return "forecast.io apikey aecabcef7974ee5c21f74e0a7943bc44"; }
            }

            public override string ExpectedResponse
            {
                get { return string.Format("@{0} Forcast.io ApiKey set.", IncomingMessage.UserName); }
            }
        }
    }
}