using System;

namespace CoderTom.Mimics
{
    public class TypeArgumentMustBeAnInterfaceException : MimicsException
    {
        public TypeArgumentMustBeAnInterfaceException()
        {
        }

        public TypeArgumentMustBeAnInterfaceException(Type type) : base("The specified type, '" + type.Name + "' is not an interface.  Try specifying the type parameter explicitly.")
        {
        }
    }
}