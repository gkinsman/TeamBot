using System;
using System.Linq;
using System.Threading.Tasks;
using Raven.Client;
using TeamBot.Infrastructure.Messages;
using TeamBot.Infrastructure.Slack;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Features.TeamCity
{
    public class TeamCityHandler : SlackMessageHandler
    {
        private readonly ITeamCityClient _client;

        public TeamCityHandler(ISlackClient slack, ITeamCityClient client)
            : base(slack)
        {
            if (client == null) 
                throw new ArgumentNullException("client");

            _client = client;
        }

        public override async Task Handle(IncomingMessage incomingMessage)
        {
            var values = incomingMessage.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var text = string.Join(" ", values.Skip(1));

            Message message;

            switch (values[0].ToLower())
            {
                case "buildstatus":
                    
                    message = await GetBuildStatus(text);
                    break;

                default:
                    
                    message = new Message { Text = string.Format("") };
                    break;
            }
        }

        private async Task<Message> GetBuildStatus(string filters)
        {
            var builds = await _client.GetBuilds();

            foreach (var buildInfo in builds.Build)
            {
                var build = await _client.GetBuild(buildInfo.Id);

                //attachment.Add(new Attachment
                //{
                //    Color = build.Status == "SUCCESSFUL" ? "good" : "danger",
                //    Text = string.Format("<{0} :: {1}|{2}>", build.BuildType, build.BuildType.Name, build.BuildType.WebUrl)
                //});
            }

            return null;
        }
    }
}