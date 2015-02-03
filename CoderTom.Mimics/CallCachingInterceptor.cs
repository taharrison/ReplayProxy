using System;
using Castle.DynamicProxy;

namespace CoderTom.Mimics
{
    internal class CallCachingInterceptor : MimeInterceptor
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