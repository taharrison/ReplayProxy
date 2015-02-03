namespace CoderTom.Mimics
{
    public class TypeRequiresDefaultConstructorException : MimicsException
    {
        public TypeRequiresDefaultConstructorException()
        {
        }

        public TypeRequiresDefaultConstructorException(string message)
            : base(message)
        {
        }
    }
}
