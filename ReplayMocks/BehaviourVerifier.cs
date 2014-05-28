using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReplayMocks
{
    public class BehaviourVerifier
    {
        private IEqualityComparer<object> comparer;
        private History history;

        public BehaviourLog VerificationLog { get; private set; }

        internal BehaviourVerifier(IEqualityComparer<object> comparer, History history)
        {
            this.comparer = comparer;
            this.history = history;
            VerificationLog = new BehaviourLog();
        }

        public bool ConfirmBehaviourHasNotChanged(object repoistory)
        {
            var t = repoistory.GetType();
            foreach (var call in history.Calls)
            {
                object returnValue = null;
                var targetType = Type.GetType(call.MethodDeclaringTypeName);

                var method = targetType.GetMethod(call.MethodName, call.Parameters.Select(p => Type.GetType(p.ParameterExpectedType)).ToArray());
                string thrownExceptionType = null;
                try
                {
                    returnValue = method.Invoke(repoistory, call.Parameters.Select(arg => arg.Value).ToArray());
                }
                catch (TargetInvocationException e)
                {
                    thrownExceptionType = e.InnerException.GetType().AssemblyQualifiedName;
                }
                
                var callReturnValue = call.ReturnValue;

                var isEqual = (thrownExceptionType == null && String.IsNullOrWhiteSpace(call.ThrownExceptionType)) &&
                              comparer.Equals(returnValue, callReturnValue)
                              ||
                              (!String.IsNullOrWhiteSpace(call.ThrownExceptionType) &&
                               thrownExceptionType == call.ThrownExceptionType);
                    
                VerificationLog.Add(new BehaviourLogItem()
                {
                    Pass = isEqual,
                    Call = call,
                    ActualReturnValue = returnValue,
                    //ActualThrownException = thrown
                });

                if (!isEqual)
                    return false;
            }
            return true;
        }
    }
}
