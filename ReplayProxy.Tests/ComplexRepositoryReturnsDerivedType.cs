namespace ReplayProxy.Tests
{
    public class ComplexRepositoryReturnsDerivedType : IComplexRepository
    {
        public DataStructure GetRelated(DataStructure parameter)
        {
            var asDerived = parameter as DerivedDataStructure;
            var balance = asDerived != null ? asDerived.Balance*2 : 0m;
            return new DerivedDataStructure { Age = parameter.Age - 16, Name = parameter.Name + " jr.", Balance = balance };
        }
    }
}