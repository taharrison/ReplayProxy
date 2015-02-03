using Castle.DynamicProxy;

namespace CoderTom.Mimics
{
    internal class CallReplayingInterceptor : MimeInterceptor
    {
        public CallReplayingInterceptor(History history) : base(history)
        {
        }

        public override void ProcessIntercept(IInvocation invocation)
        {
            PreviousRunLog.ReplayMethodResult(invocation);
        }
    }
}