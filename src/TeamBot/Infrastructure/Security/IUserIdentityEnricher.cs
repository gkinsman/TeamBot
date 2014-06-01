using System.Security.Claims;
using Nancy.Security;

namespace TeamBot.Infrastructure.Security
{
    public interface IUserIdentityEnricher
    {
        IUserIdentity GetEnrichedUser(ClaimsIdentity identity);
    }
}