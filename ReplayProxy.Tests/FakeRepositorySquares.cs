namespace ReplayProxy.Tests
{
    public class FakeRepositorySquares : IFakeRepository
    {
        public string ParameterlessFn()
        {
            return "beep";
        }

        public int Function(int n)
        {
            return n*n;
        }
    }
}