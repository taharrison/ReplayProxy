using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplayMocks
{
    public class Recorder<T> where T : class
    {
        public T Object { get; private set; }

        public History Log { get { return _interceptor.Log; }} 

        //public bool CompareToLog(string expectedLog, out string differenceReport)
        //{
        //    var myCalls = Log.Split('\n');
        //    var expectedCalls = expectedLog.Split('\n');
        //
        //    var areEqual = true;
        //    var differences = new StringBuilder();
        //    var numberOfCalls = Math.Min(myCalls.Count(), expectedCalls.Count());
        //
        //    if (myCalls.Count() != expectedCalls.Count())
        //    {
        //        areEqual = false;
        //        differences.AppendLine("Expected " + expectedCalls.Count() + " calls; actual number of calls: " + myCalls.Count());
        //        differences.AppendLine(" - next additional call: " + (expectedCalls.Count() > myCalls.Count() ? expectedCalls[numberOfCalls] : myCalls[numberOfCalls]));
        //    }
        //
        //    for (int i = 0; i < numberOfCalls; i++)
        //    {
        //        if (expectedCalls[i] != myCalls[i])
        //        {
        //            areEqual = false;
        //            differences.AppendLine("Expected: " + expectedCalls[i] + " Actual: " + myCalls[i]);
        //        }
        //    }
        //
        //    differenceReport = differences.ToString();
        //    return areEqual;
        //}

        private CallRecordingInterceptor _interceptor;

        public Recorder(T target)
        {
            var proxyGen = new Castle.DynamicProxy.ProxyGenerator();
            _interceptor = new CallRecordingInterceptor();
            Object = proxyGen.CreateInterfaceProxyWithTarget(target, _interceptor);
        }

        public History GetHistory()
        {
            return Log;
        }
    }
}
