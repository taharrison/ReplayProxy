namespace CoderTom.Mimics
{
    public class UnexpectedCallException : MimicsException
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