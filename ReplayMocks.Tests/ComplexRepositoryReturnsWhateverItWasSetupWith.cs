namespace ReplayMocks.Tests
{
    public class ComplexRepositoryReturnsWhateverItWasSetupWith : IComplexRepository
    {
        private DataStructure _returnValue;

        public ComplexRepositoryReturnsWhateverItWasSetupWith(DataStructure returnValue)
        {
            _returnValue = returnValue;
        }

        public DataStructure GetRelated(DataStructure parameter)
        {
            return _returnValue;
        }
    }
}