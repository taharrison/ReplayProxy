namespace ReplayProxy.Tests
{
    public class ComplexRepository : IComplexRepository
    {
        public DataStructure GetRelated(DataStructure parameter)
        {
            return new DataStructure {Age = parameter.Age + 30, Name = parameter.Name + " snr."};
        }
    }
}