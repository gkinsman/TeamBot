namespace TeamBot.Infrastructure.Slack
{
    public class SlackContext
    {
        public SlackContext(string company, string token)
        {
            Company = company;
            Token = token;
        }

        public string Company { get; private set; }

        public string Token { get; private set; } 
    }
}