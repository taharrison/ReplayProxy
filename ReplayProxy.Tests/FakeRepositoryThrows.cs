using System;
using System.Runtime.InteropServices;

namespace ReplayProxy.Tests
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