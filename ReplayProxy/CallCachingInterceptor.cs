using System;
using System.Text;
using Castle.DynamicProxy;

namespace ReplayProxy
{
    public class CallCachingInterceptor : ReplayInterceptor
    {
        public CallCachingInterceptor(History history) : base(history)
        {
        }

        public override void ProcessIntercept(IInvocation invocation)
        {
            if (PreviousRunLog.GetMatchingCall(invocation) != null)
            {
                PreviousRunLog.ReplayMethodResult(invocation);
                return;
            }

            try
            {
                invocation.Proceed();
            }
            catch (Exception e)
            {
                PreviousRunLog.AddException(invocation, e);
                throw;
            }
        }
    }
}