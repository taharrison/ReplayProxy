using System;
using System.Linq;
using System.Text;

namespace ReplayProxy
{
    internal class InvokationLoggingMock<T> where T : class
    {
        public T Object { get; private set; }

        public string Log { get { return _interceptor.Log; }} 

        public bool CompareToLog(string expectedLog, out string differenceReport)
        {
            var myCalls = Log.Split('\n');
            var expectedCalls = expectedLog.Split('\n');

            var areEqual = true;
            var differences = new StringBuilder();
            var numberOfCalls = Math.Min(myCalls.Count(), expectedCalls.Count());

            if (myCalls.Count() != expectedCalls.Count())
            {
                areEqual = false;
                differences.AppendLine("Expected " + expectedCalls.Count() + " calls; actual number of calls: " + myCalls.Count());
                differences.AppendLine(" - next additional call: " + (expectedCalls.Count() > myCalls.Count() ? expectedCalls[numberOfCalls] : myCalls[numberOfCalls]));
            }

            for (int i = 0; i < numberOfCalls; i++)
            {
                if (expectedCalls[i] != myCalls[i])
                {
                    areEqual = false;
                    differences.AppendLine("Expected: " + expectedCalls[i] + @" Actual: " + myCalls[i]);
                }
            }

            differenceReport = differences.ToString();
            return areEqual;
        }

        private CallLoggingInterceptor _interceptor;

        public InvokationLoggingMock()
        {
            var proxyGen = new Castle.DynamicProxy.ProxyGenerator();
            _interceptor = new CallLoggingInterceptor();
            Object = proxyGen.CreateInterfaceProxyWithoutTarget<T>(_interceptor);
        }
    }
}
