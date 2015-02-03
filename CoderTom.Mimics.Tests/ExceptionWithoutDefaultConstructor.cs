using System;

namespace CoderTom.Mimics.Tests
{
    public class ExceptionWithoutDefaultConstructor : ApplicationException
    {
        public ExceptionWithoutDefaultConstructor(string message)
            : base(message)
        {

        }
    }
}