using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Nancy.Security;

namespace TeamBot.Infrastructure.Security
{
    public class UserIdentity : IUserIdentity
    {
        public UserIdentity(ClaimsIdentity claimsIdentity)
        {
            if (claimsIdentity == null) 
                throw new ArgumentNullException("claimsIdentity");

            UserName = claimsIdentity.Name;
            ActualClaims = claimsIdentity.Claims;
        }

        public IEnumerable<Claim> ActualClaims { get; private set; }

        public string UserName { get; private set; }

        public IEnumerable<string> Claims
        {
            get { return ActualClaims.Select(c => c.ToString()); }
        }
    }

    public static class UserIdentityExtensions
    {
        public static IEnumerable<Claim> ActualClaims(this IUserIdentity identity)
        {
            if (identity == null) 
                throw new ArgumentNullException("identity");

            return ((UserIdentity)identity).ActualClaims;
        }

        public static string TokenUserId(this IUserIdentity identity)
        {
            if (identity == null) 
                throw new ArgumentNullException("identity");

            return identity.ActualClaims().Single(c => c.Type.Equals("user_id")).Value;
        }

        public static string TokenUserId(this ClaimsIdentity identity)
        {
            if (identity == null) 
                throw new ArgumentNullException("identity");

            return identity.Claims.Single(c => c.Type.Equals("user_id")).Value;
        }
    }
}