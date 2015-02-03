using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CoderTom.Mimics
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

        public bool TestGenerics<T>(T repository, LoggedCall call) where T : class
        {
            var t = repository.GetType();
            var targetType = typeof (T);

            var recorder = Mimic.Record(repository);

            var method = targetType.GetMethod(call.MethodName, call.Parameters.Select(p => Type.GetType(p.ParameterExpectedType)).ToArray());
            try
            {
                method.Invoke(recorder, call.Parameters.Select(arg => arg.Value).ToArray());
            }
            catch (TargetException e)
            {
                throw new ObjectIsNotOfExpectedTypeException(t, targetType);
            }
            catch (TargetInvocationException e)
            {
                // this is already logged by proxy recorder
            }

            var callReturnValue = call.ReturnValue;
            var recordedCall = Mimic.GetHistory(recorder).Calls[0];

            var isEqual = (recordedCall.ThrownExceptionType == null && String.IsNullOrWhiteSpace(call.ThrownExceptionType)) &&
                          comparer.Equals(recordedCall.ReturnValue, callReturnValue)
                          ||
                          (!String.IsNullOrWhiteSpace(call.ThrownExceptionType) &&
                           recordedCall.ThrownExceptionType == call.ThrownExceptionType);

            VerificationLog.Add(new BehaviourLogItem()
            {
                Pass = isEqual,
                Call = call,
                ActualCall = recordedCall
            });

            return isEqual;
        }

        public bool ConfirmBehaviourHasNotChanged(object repository, BehaviourVerifier.Options options = Options.None)
        {
            var t = repository.GetType();
            bool isEqual = true;
            foreach (var call in history.Calls)
            {
                var targetType = Type.GetType(call.MethodDeclaringTypeName);

                var methodRef = this.GetType().GetMethod("TestGenerics");
                var genericMethodRef = methodRef.MakeGenericMethod(targetType);
                try
                {
                    isEqual &= (bool)genericMethodRef.Invoke(this, new object[] { repository, call });
                    if (!isEqual && options.HasFlag(BehaviourVerifier.Options.AbortOnFail))
                        return isEqual;
                }
                catch (ArgumentException e)
                {
                    throw new ObjectIsNotOfExpectedTypeException(t, targetType);
                }
            }

            return isEqual;
        }

        [Flags]
        public enum Options
        {
            None = 0,
            AbortOnFail = 0x1
        }
    }
}
