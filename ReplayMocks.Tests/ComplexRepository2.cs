namespace ReplayMocks.Tests
{
    public class ComplexRepository2 : IComplexRepository
    {
        public DataStructure GetRelated(DataStructure parameter)
        {
            return new DataStructure {Age = parameter.Age - 16, Name = parameter.Name + " jr."};
        }
    }
}