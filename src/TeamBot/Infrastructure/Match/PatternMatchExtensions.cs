namespace TeamBot.Infrastructure.Match
{
    public static class PatternMatchExtensions
    {
        public static PatternMatchContext<TIn> Match<TIn>(this TIn value)
        {
            return new PatternMatchContext<TIn>(value);
        }
    }
}