using System;

namespace ReplayMocks
{
    public class BehaviourLogItem
    {
        public bool Pass { get; set; }
        
        public LoggedCall Call { get; set; }

        public object ActualReturnValue { get; set; }

        public override string ToString()
        {
            return String.Format("Method: '{0}'\nParams: {1}\nOutcome: {2}\nReturn Value: {3}", 
                Call.MethodName, 
                XmlSerialiser.Serialise(Call.Parameters), 
                Pass ? "Pass" : "Fail",
                Pass ? SerialiseIfNotNull(Call.ReturnValue) : "Expected: " 
                                                                     + SerialiseIfNotNull(Call.ReturnValue) + "\nActual: "
                                                                     + SerialiseIfNotNull(ActualReturnValue));
        }

        private string SerialiseIfNotNull(object returnValue)
        {
            return returnValue != null ? XmlSerialiser.Serialise(returnValue) : "null";
        }
    }
}