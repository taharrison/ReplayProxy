using System;

namespace CoderTom.Mimics
{
    public class ObjectIsNotOfExpectedTypeException : MimicsException
    {
        public ObjectIsNotOfExpectedTypeException()
        {
        }

        public ObjectIsNotOfExpectedTypeException(Type actualType, Type expectedType)
            : base("The specified type, '" + actualType.Name + "' does not implement or derive from expected type '" + expectedType.Name + "'.")
        {
        }
    }
}