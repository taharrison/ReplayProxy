using System;

namespace ReplayMocks.Tests
{
    public class ExceptionWithoutDefaultConstructor : ApplicationException
    {
        public ExceptionWithoutDefaultConstructor(string message)
            : base(message)
        {

        }
    }
}