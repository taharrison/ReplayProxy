using System;
using CoderTom.Mimics.Utilities;

namespace CoderTom.Mimics
{
    public class BehaviourLogItem
    {
        public bool Pass { get; internal set; }
        
        public LoggedCall Call { get; internal set; }
        public LoggedCall ActualCall { get; set; }

        public override string ToString()
        {
            return String.Format("Method: '{0}'\nParams: {1}\nOutcome: {2}\nReturn Value: {3}", 
                Call.MethodName, 
                XmlSerialiser.Serialise(Call.Parameters), 
                Pass ? "Pass" : "Fail",
                Pass ? SerialiseIfNotNull(Call.ReturnValue) : "Expected: " + ReturnValueOrException(Call.ReturnValue, Call.ThrownExceptionType)
                                                            + "\nActual: " + ReturnValueOrException(ActualCall.ReturnValue, ActualCall.ThrownExceptionType));
        }

        private string ReturnValueOrException(object returnValue, string thrownExceptionType)
        {
            if (thrownExceptionType != null)
            {
                return "exception of type <" + thrownExceptionType + ">";
            }
            return SerialiseIfNotNull(returnValue);
        }

        private string ReturnValueOrException(object returnValue, Exception exception)
        {
            if (exception != null)
            {
                return "exception of type <" + exception.GetType().Name + ">:" + Environment.NewLine + exception.ToString();
            }
            return SerialiseIfNotNull(returnValue);
        }

        private string SerialiseIfNotNull(object returnValue)
        {
            return returnValue != null ? XmlSerialiser.Serialise(returnValue) : "null";
        }
    }
}