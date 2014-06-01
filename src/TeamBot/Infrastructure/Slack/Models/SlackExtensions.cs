namespace TeamBot.Infrastructure.Slack.Models
{
    public static class SlackExtensions
    {
        public static bool IsSlashCommand(this IncomingMessage incomingMessage)
        {
            return incomingMessage.Command != null;
        }
    }
}