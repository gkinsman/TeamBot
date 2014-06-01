using System;
using System.Collections.Generic;

namespace TeamBot.Infrastructure.Match
{
    public class PatternMatch<TIn, TOut>
    {
        private readonly IList<PatternMatchCase> _cases = new List<PatternMatchCase>();
        private readonly TIn _value;
        private Func<TIn, TOut> _elseCase;

        internal PatternMatch(TIn value)
        {
            _value = value;
        }

        public PatternMatch<TIn, TOut> With(Predicate<TIn> condition, Func<TIn, TOut> result)
        {
            _cases.Add(new PatternMatchCase
            {
                Condition = condition,
                Result = result
            });

            return this;
        }

        public PatternMatch<TIn, TOut> With(Predicate<TIn> condition, TOut result)
        {
            return With(condition, x => result);
        }

        public PatternMatch<TIn, TOut> Else(Func<TIn, TOut> result)
        {
            if (_elseCase != null)
            {
                throw new InvalidOperationException("Cannot have multiple else cases");
            }

            _elseCase = result;

            return this;
        }

        public PatternMatch<TIn, TOut> Else(TOut result)
        {
            return Else(x => result);
        }

        public TOut Do()
        {
            if (_elseCase != null)
            {
                With(x => true, _elseCase);
            }

            foreach (var test in _cases)
            {
                if (test.Condition(_value))
                {
                    return test.Result(_value);
                }
            }

            throw new IncompletePatternMatchException();
        }

        public static implicit operator TOut(PatternMatch<TIn, TOut> pattern)
        {
            return pattern.Do();
        }

        private struct PatternMatchCase
        {
            public Predicate<TIn> Condition;
            public Func<TIn, TOut> Result;
        }
    }
}