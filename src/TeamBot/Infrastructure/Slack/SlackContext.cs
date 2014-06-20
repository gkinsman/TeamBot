using System;

namespace TeamBot.Infrastructure.Slack
{
    public class SlackContext
    {
        public SlackContext(string company, string token)
        {
            if (company == null) 
                throw new ArgumentNullException("company");
            
            if (token == null) 
                throw new ArgumentNullException("token");

            Company = company;
            Token = token;
        }

        public string Company { get; private set; }
        public string Token { get; private set; }

        private static SlackContext _current;
        public static SlackContext Current
        {
            get { return _current; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _current = value;
            }
        }
    }
}