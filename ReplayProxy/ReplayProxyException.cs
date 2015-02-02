using System;

namespace ReplayProxy
{
    public class ReplayProxyException : Exception
    {
        public ReplayProxyException()
        {
        }

        public ReplayProxyException(string message) : base(message)
        {
        }
    }
}