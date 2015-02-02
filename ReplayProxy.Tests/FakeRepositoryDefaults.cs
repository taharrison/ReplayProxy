namespace ReplayProxy.Tests
{
    public class FakeRepositoryDefaults : IFakeRepository
    {
        public string ParameterlessFn()
        {
            return null;
        }

        public int Function(int n)
        {
            return 0;
        }
    }
}