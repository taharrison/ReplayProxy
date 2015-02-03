using System;

namespace CoderTom.Mimics
{
    public class MimicsException : Exception
    {
        public MimicsException()
        {
        }

        public MimicsException(string message) : base(message)
        {
        }
    }
}