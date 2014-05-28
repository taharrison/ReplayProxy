using System;

namespace ReplayMocks
{
    public class ReplayMocksException : Exception
    {
        public ReplayMocksException()
        {
        }

        public ReplayMocksException(string message) : base(message)
        {
        }
    }
}