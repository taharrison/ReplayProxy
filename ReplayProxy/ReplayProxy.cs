using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace ReplayProxy
{
    public class ReplayProxy
    {
        protected ReplayInterceptor _interceptor;

        internal History CombineThisRunLogAndPastRunLog()
        {
            var thisRun = this._interceptor.ThisRunLog.Calls;
            var previousRun = this._interceptor.PreviousRunLog.Calls
                .Where(c => !thisRun.Any(a => History.Equals(a, c)));
            return new History()
            {
                Calls = previousRun.Union(thisRun).ToList()
            };
        }
        
        public ReplayProxy(ReplayInterceptor interceptor)
        {
            _interceptor = interceptor;
        }

        internal bool ConfirmSameCallsWereMade(VerifierOptions options = VerifierOptions.Default_SameCallsSameNumberOfTimesInAnyOrder)
        {
            var previous = _interceptor.PreviousRunLog.Calls;
            var current = _interceptor.ThisRunLog.Calls;

            VerifierOptions failedRules = VerifierOptions.None;

            failedRules |= EveryCallInFirstListExistsInSecond(VerifierOptions.AllExpectedCallsMustBeObserved, previous, current, sameNumber: options.HasFlag(VerifierOptions.ExpectedCallsMustBeObservedTheExactNumberOfTimes));

            failedRules |= VerifierOptions.OnlyExpectedCallsAreAllowed & EveryCallInFirstListExistsInSecond(VerifierOptions.OnlyExpectedCallsAreAllowed, current, previous, sameNumber: options.HasFlag(VerifierOptions.ExpectedCallsMustBeObservedTheExactNumberOfTimes));

            return (failedRules & options) == VerifierOptions.None;
        }

        private static VerifierOptions EveryCallInFirstListExistsInSecond(VerifierOptions whatRuleAreWeChecking, List<LoggedCall> expectedCalls, List<LoggedCall> compareCalls, bool sameNumber)
        {
            VerifierOptions failedRules = VerifierOptions.None;

            var copyOfCurrent = compareCalls.ToList();

            var lastMatchAt = -1;
            var inSameOrder = true;

            foreach (var call in expectedCalls)
            {
                var matchInOrder = copyOfCurrent.FindIndex(Math.Max(0, lastMatchAt), x => History.Equals(x, call));
                var match = matchInOrder > -1 ? matchInOrder : copyOfCurrent.FindIndex(x => History.Equals(x, call));
                if (match < 0)
                {
                    // missing call
                    failedRules |= whatRuleAreWeChecking;
                }
                else
                {
                    if (sameNumber)
                        copyOfCurrent.RemoveAt(match);

                    if (lastMatchAt > match)
                        failedRules |= VerifierOptions.CallsMustOccurInTheExactSameOrder;
                    lastMatchAt = match;
                }
                // add to report - expected
            }

            return failedRules;
        }

        public History GetThisRunLog()
        {
            return new History()
            {
                Calls = this._interceptor.ThisRunLog.Calls.ToList()
            };
        }
    }

    public class ReplayProxy<T> :ReplayProxy where T : class
    {
        public T Object { get; protected set; }

        public ReplayProxy(ReplayInterceptor interceptor) : base(interceptor)
        {
            var proxyGen = new Castle.DynamicProxy.ProxyGenerator();
            Object = proxyGen.CreateInterfaceProxyWithoutTarget<T>(_interceptor);
        }

        public ReplayProxy(ReplayInterceptor interceptor, T instance) : base(interceptor)
        {
            var proxyGen = new Castle.DynamicProxy.ProxyGenerator();
            Object = proxyGen.CreateInterfaceProxyWithTarget<T>(instance, _interceptor);
        }
    }
}
