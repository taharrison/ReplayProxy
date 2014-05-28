namespace ReplayMocks
{
    public class UnexpectedCallException : ReplayMocksException
    {
        public UnexpectedCallException()
        {
        }

        public UnexpectedCallException(string message)
            : base(message)
        {
        }
    }
}