using System;

namespace CoderTom.Mimics.Tests
{
    public class CustomException : ApplicationException
    {
        public CustomException()
        {

        }

        public CustomException(string message)
            : base(message)
        {

        }
    }
}
