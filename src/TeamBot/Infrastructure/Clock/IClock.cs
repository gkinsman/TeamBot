using System;

namespace TeamBot.Infrastructure.Clock
{
    public interface IClock
    {
        DateTimeOffset UtcNow { get; }
    }
}