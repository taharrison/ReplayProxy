using System;
using System.Diagnostics;
using System.Text;
using Castle.DynamicProxy;

namespace ReplayProxy
{
    internal class CallRecordingInterceptor : ReplayInterceptor
    {
        public override void ProcessIntercept(IInvocation invocation)
        {
            invocation.Proceed();
        }

    }
}