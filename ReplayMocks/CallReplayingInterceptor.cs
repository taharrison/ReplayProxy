using System.Text;
using Castle.DynamicProxy;

namespace ReplayMocks
{
    public class CallReplayingInterceptor : ReplayInterceptor
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