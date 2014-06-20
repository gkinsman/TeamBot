namespace TeamBot.Infrastructure.Slack.Models
{
    public static class SlackExtensions
    {
        public static bool IsSlashCommand(this IncomingMessage incomingMessage)
        {
            return incomingMessage.Command != null;
        }

        public static string ReplyTo(this IncomingMessage incomingMessage)
        {
            return incomingMessage.IsSlashCommand()
                ? incomingMessage.UserId
                : incomingMessage.ChannelId;
        }
    }
}