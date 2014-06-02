using System;

namespace TeamBot.Infrastructure.Match
{
    public class PatternMatchContext<TIn>
    {
        private readonly TIn _value;

        internal PatternMatchContext(TIn value)
        {
            _value = value;
        }

        public PatternMatch<TIn, TOut> WithOutputType<TOut>()
        {
            return new PatternMatch<TIn, TOut>(_value);
        }

        public PatternMatch<TIn, TOut> With<TOut>(Predicate<TIn> condition, TOut result)
        {
            return new PatternMatch<TIn, TOut>(_value)
                .With(condition, result);
        }

        public PatternMatch<TIn, TOut> With<TOut>(Predicate<TIn> condition, Func<TIn, TOut> result)
        {
            return new PatternMatch<TIn, TOut>(_value)
                .With(condition, result);
        }
    }
}