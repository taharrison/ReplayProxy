namespace ReplayMocks.Tests
{
    public class FakeRepositoryDoubles : IFakeRepository
    {
        public string ParameterlessFn()
        {
            return "marp";
        }

        public int Function(int n)
        {
            return n*2;
        }
    }
}