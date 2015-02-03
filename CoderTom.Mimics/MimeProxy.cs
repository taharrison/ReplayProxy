using System;
using System.Collections.Generic;
using System.Linq;

namespace CoderTom.Mimics
{
    internal class MimeProxy
    {
        protected MimeInterceptor _interceptor;

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
        
        public MimeProxy(MimeInterceptor interceptor)
        {
            _interceptor = interceptor;
        }

        internal bool ConfirmSameCallsWereMade(MockOptions options = MockOptions.Default_SameCallsSameNumberOfTimesInAnyOrder)
        {
            var previous = _interceptor.PreviousRunLog.Calls;
            var current = _interceptor.ThisRunLog.Calls;

            MockOptions failedRules = MockOptions.None;

            failedRules |= EveryCallInFirstListExistsInSecond(MockOptions.AllExpectedCallsMustBeObserved, previous, current, sameNumber: options.HasFlag(MockOptions.ExpectedCallsMustBeObservedTheExactNumberOfTimes));

            failedRules |= MockOptions.OnlyExpectedCallsAreAllowed & EveryCallInFirstListExistsInSecond(MockOptions.OnlyExpectedCallsAreAllowed, current, previous, sameNumber: options.HasFlag(MockOptions.ExpectedCallsMustBeObservedTheExactNumberOfTimes));

            return (failedRules & options) == MockOptions.None;
        }

        private static MockOptions EveryCallInFirstListExistsInSecond(MockOptions whatRuleAreWeChecking, List<LoggedCall> expectedCalls, List<LoggedCall> compareCalls, bool sameNumber)
        {
            MockOptions failedRules = MockOptions.None;

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
                        failedRules |= MockOptions.CallsMustOccurInTheExactSameOrder;
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

    internal class MimeProxy<T> :MimeProxy where T : class
    {
        public T Object { get; protected set; }

        public MimeProxy(MimeInterceptor interceptor) : base(interceptor)
        {
            var proxyGen = new Castle.DynamicProxy.ProxyGenerator();
            Object = proxyGen.CreateInterfaceProxyWithoutTarget<T>(_interceptor);
        }

        public MimeProxy(MimeInterceptor interceptor, T instance) : base(interceptor)
        {
            var proxyGen = new Castle.DynamicProxy.ProxyGenerator();
            Object = proxyGen.CreateInterfaceProxyWithTarget<T>(instance, _interceptor);
        }
    }
}
