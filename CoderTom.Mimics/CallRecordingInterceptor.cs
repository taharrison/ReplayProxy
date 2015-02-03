using Castle.DynamicProxy;

namespace CoderTom.Mimics
{
    internal class CallRecordingInterceptor : MimeInterceptor
    {
        public override void ProcessIntercept(IInvocation invocation)
        {
            invocation.Proceed();
        }

    }
}