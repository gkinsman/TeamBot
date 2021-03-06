using System;
using System.Security.Claims;
using Nancy.Security;

namespace TeamBot.Infrastructure.Security
{
    public class UserIdentityEnricher : IUserIdentityEnricher
    {
        public IUserIdentity GetEnrichedUser(ClaimsIdentity identity)
        {
            if (identity == null) 
                throw new ArgumentNullException("identity");

            // Potentially go to the database and enrich the user identity
            var user = new UserIdentity(identity);

            return user;
        }
    }
}