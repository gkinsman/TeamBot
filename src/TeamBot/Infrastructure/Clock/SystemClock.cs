using System;

namespace TeamBot.Infrastructure.Clock
{
    internal class SystemClock : IClock
    {
        private readonly Func<DateTimeOffset> _factory;

        public SystemClock()
            : this(() => DateTimeOffset.UtcNow)
        {   
        }

        public SystemClock(Func<DateTimeOffset> factory)
        {
            _factory = factory;
        }

        public DateTimeOffset UtcNow
        {
            get { return _factory(); }
        }
    }
}