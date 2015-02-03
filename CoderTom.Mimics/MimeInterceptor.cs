using System;
using Castle.DynamicProxy;

namespace CoderTom.Mimics
{
    internal abstract class MimeInterceptor : IInterceptor
    {
        public History PreviousRunLog { get; set; }
        public History ThisRunLog { get; set; }

        public MimeInterceptor()
        {
            PreviousRunLog = new History();
            ThisRunLog = new History();
        }

        public MimeInterceptor(History previousRunLog)
        {
            PreviousRunLog = previousRunLog;
            ValidateLog();
            ThisRunLog = new History();
        }

        private void ValidateLog()
        {
            foreach (var call in PreviousRunLog.Calls)
            {
                if (!string.IsNullOrWhiteSpace(call.ThrownExceptionType))
                {
                    var ext = Type.GetType(call.ThrownExceptionType);
                    var defaultConstructor = ext.GetConstructor(new Type[] { });
                    if (defaultConstructor == null)
                    {
                        throw new TypeRequiresDefaultConstructorException("Exception '{0}' has no default constructor and cannot be thrown");
                    }
                }
            }
        }

        public abstract void ProcessIntercept(IInvocation invocation);

        public void Intercept(IInvocation invocation)
        {
            try
            {
                ProcessIntercept(invocation);
                ThisRunLog.Add(invocation);
            }
            catch (Exception e)
            {
                ThisRunLog.AddException(invocation, e);
                throw e;
            }
        }
    }
}