using System;

namespace CoderTom.Mimics.Tests
{
    public class FakeRepositoryThrows : IFakeRepository
    {
        private Exception _exception;

        public FakeRepositoryThrows(Exception e)
        {
            _exception = e;
        }
        public string ParameterlessFn()
        {
            throw _exception;
        }

        public int Function(int n)
        {
            throw _exception;
        }
    }
}