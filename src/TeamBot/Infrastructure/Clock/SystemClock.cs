using System;

namespace TeamBot.Infrastructure.Clock
{
    internal class SystemClock : IClock
    {
        public DateTimeOffset UtcNow
        {
            get { return DateTimeOffset.UtcNow; }
        }
    }
}