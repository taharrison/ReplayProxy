using System;
using System.Text;
using Castle.DynamicProxy;

namespace ReplayProxy
{
    internal class CallCachingInterceptor : ReplayInterceptor
    {
        internal CallCachingInterceptor(History history) : base(history)
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