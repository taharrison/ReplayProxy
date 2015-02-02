using System;

namespace ReplayProxy.Tests
{
    public class ExceptionWithoutDefaultConstructor : ApplicationException
    {
        public ExceptionWithoutDefaultConstructor(string message)
            : base(message)
        {

        }
    }
}