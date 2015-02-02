namespace ReplayProxy
{
    public class UnexpectedCallException : ReplayProxyException
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