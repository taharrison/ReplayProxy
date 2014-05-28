using System;
using NUnit.Framework;

namespace ReplayMocks.Tests
{
    [TestFixture]
    public class ComplexParameterIntegrationTests
    {
        [Test]
        public void CacheProxyPassesUncachedCallsToTheBaseType()
        {
            var data = new DataStructure {Age = 21, Name = "Jimmy"};
            var data2 = new DataStructure {Age = 22, Name = "Janey"};
            var repo = new ComplexRepository();
            var recorder = Proxy.Record<IComplexRepository>(repo);
            DataStructure firstResult = recorder.GetRelated(data);
            History history = Proxy.GetHistory(recorder);

            var replayer = Proxy.Cache<IComplexRepository>(
                new ComplexRepository2(), history);
            DataStructure result = replayer.GetRelated(data2);

            Assert.AreNotEqual(firstResult.Name, result.Name, "otherwise this is because it was cached");
            Assert.AreNotEqual(firstResult.Age, result.Age, "otherwise this is because it was cached");
        }

        [Test]
        public void CacheProxyReplaysCachedCalls()
        {
            var data = new DataStructure {Age = 21, Name = "Jimmy"};
            var repo = new ComplexRepository();
            var recorder = Proxy.Record<IComplexRepository>(repo);
            DataStructure firstResult = recorder.GetRelated(data);
            History history = Proxy.GetHistory(recorder);


            var replayer = Proxy.Cache<IComplexRepository>(
                new ComplexRepository2(), history);
            DataStructure result = replayer.GetRelated(data);

            Assert.AreEqual(firstResult.Name, result.Name, "otherwise this is because it was not cached");
            Assert.AreEqual(firstResult.Age, result.Age, "otherwise this is because it was not cached");
        }

        [Test]
        public void DerivedReturnTypeCallIsRecordedWithoutLossOfInformationWhenReplayed()
        {
            var data = new DataStructure { Age = 21, Name = "Jimmy" };
            var repo = new ComplexRepositoryReturnsDerivedType();
            var recorder = Proxy.Record<IComplexRepository>(repo);
            DataStructure firstResult = recorder.GetRelated(data);

            string serialisedHistory = Proxy.SerialiseHistory(recorder);
            var history = Proxy.DeserialiseHistory(serialisedHistory);

            var replayer = Proxy.Cache<IComplexRepository>(
                new ComplexRepository2(), history);
            DataStructure result = replayer.GetRelated(data);

            Assert.AreEqual(firstResult.Name, result.Name, "otherwise this is because it was not cached");
            Assert.AreEqual(firstResult.Age, result.Age, "otherwise this is because it was not cached");
            Assert.IsInstanceOf(typeof(DerivedDataStructure), result);
            Assert.AreEqual(((DerivedDataStructure)firstResult).Balance, ((DerivedDataStructure)result).Balance);
        }

        [Test]
        public void DerivedParameterCallIsRecordedAndCanBeReplayed()
        {
            var data = new DerivedDataStructure { Age = 21, Name = "Jimmy", Balance = 123m };
            var repo = new ComplexRepositoryReturnsDerivedType();
            var recorder = Proxy.Record<IComplexRepository>(repo);
            DataStructure firstResult = recorder.GetRelated(data);

            string serialisedHistory = Proxy.SerialiseHistory(recorder);
            var history = Proxy.DeserialiseHistory(serialisedHistory);

            var replayer = Proxy.Cache<IComplexRepository>(
                new ComplexRepository2(), history);
            DataStructure result = replayer.GetRelated(data);

            Assert.AreEqual(firstResult.Name, result.Name, "otherwise this is because it was not cached");
            Assert.AreEqual(firstResult.Age, result.Age, "otherwise this is because it was not cached");
            Assert.IsInstanceOf(typeof(DerivedDataStructure), result);
            Assert.AreEqual(((DerivedDataStructure)firstResult).Balance, ((DerivedDataStructure)result).Balance);
        }

        [Test]
        public void DerivedParametersCanBeDistinguished()
        {
            var data = new DerivedDataStructure { Age = 21, Name = "Jimmy", Balance = 123m };
            var data2 = new DerivedDataStructure { Age = 21, Name = "Jimmy", Balance = 101m };
            var repo = new ComplexRepositoryReturnsDerivedType();
            var recorder = Proxy.Record<IComplexRepository>(repo);
            DataStructure firstResult = recorder.GetRelated(data);

            string serialisedHistory = Proxy.SerialiseHistory(recorder);
            var history = Proxy.DeserialiseHistory(serialisedHistory);

            var replayer = Proxy.Cache<IComplexRepository>(
                new ComplexRepositoryReturnsDerivedType(), history);
            DataStructure result = replayer.GetRelated(data2);

            Assert.IsInstanceOf(typeof(DerivedDataStructure), result);
            Assert.AreNotEqual(((DerivedDataStructure)firstResult).Balance, ((DerivedDataStructure)result).Balance, "otherwise this is because it was cached");
        }

        [Test]
        public void DerivedParametersCanRecogised()
        {
            var data = new DerivedDataStructure { Age = 21, Name = "Jimmy", Balance = 123m };
            var repo = new ComplexRepositoryReturnsDerivedType();
            var recorder = Proxy.Record<IComplexRepository>(repo);
            DataStructure firstResult = recorder.GetRelated(data);

            string serialisedHistory = Proxy.SerialiseHistory(recorder);
            var history = Proxy.DeserialiseHistory(serialisedHistory);

            var replayer = Proxy.Cache<IComplexRepository>(
                new ComplexRepository2(), history);
            DataStructure result = replayer.GetRelated(data);

            Assert.AreEqual(firstResult.Name, result.Name, "otherwise this is because it was not cached");
            Assert.AreEqual(firstResult.Age, result.Age, "otherwise this is because it was not cached");
            Assert.IsInstanceOf(typeof(DerivedDataStructure), result);
            Assert.AreEqual(((DerivedDataStructure)firstResult).Balance, ((DerivedDataStructure)result).Balance, "otherwise this is because it was cached");
        }



        // NULL checking


        [Test]
        public void CacheProxyPassesUncachedCallsToTheBaseType_WhenCheckingNULL()
        {
            var data = new DataStructure { Age = 21, Name = "Jimmy" };
            var returnValue = new DataStructure { Age = 22, Name = "Janey" };
            var returnValue2 = new DataStructure { Age = 11, Name = "Jimbo" };
            var repo = new ComplexRepositoryReturnsWhateverItWasSetupWith(returnValue);

            var recorder = Proxy.Record<IComplexRepository>(repo);
            DataStructure firstResult = recorder.GetRelated(data);

            string serialisedHistory = Proxy.SerialiseHistory(recorder);
            var history = Proxy.DeserialiseHistory(serialisedHistory);

            var replayer = Proxy.Cache<IComplexRepository>(
                new ComplexRepositoryReturnsWhateverItWasSetupWith(returnValue2), history);
            DataStructure result = replayer.GetRelated(null);

            Assert.AreEqual(returnValue2.Name, result.Name, "otherwise this is because it was cached");
            Assert.AreEqual(returnValue2.Age, result.Age, "otherwise this is because it was cached");
        }

        [Test]
        public void CacheProxyReplaysCachedCalls_WhenParameterIsNULL()
        {
            var response = new DataStructure { Age = 21, Name = "Jimmy" };
            var repo = new ComplexRepositoryReturnsWhateverItWasSetupWith(response);
            var recorder = Proxy.Record<IComplexRepository>(repo);
            DataStructure firstResult = recorder.GetRelated(null);

            string serialisedHistory = Proxy.SerialiseHistory(recorder);
            var history = Proxy.DeserialiseHistory(serialisedHistory);


            var replayer = Proxy.Cache<IComplexRepository>(
                new ComplexRepository2(), history);
            DataStructure result = replayer.GetRelated(null);

            Assert.AreEqual(firstResult.Name, result.Name, "otherwise this is because it was not cached");
            Assert.AreEqual(firstResult.Age, result.Age, "otherwise this is because it was not cached");
        }

        [Test]
        public void ReplaySavesCachedPastHistory()
        {
            var response = new DataStructure { Age = 21, Name = "Jimmy" };
            var repo = new ComplexRepositoryReturnsWhateverItWasSetupWith(response);
            var recorder = Proxy.Record<IComplexRepository>(repo);
            DataStructure firstResult = recorder.GetRelated(null);

            var pastHistory = Proxy.GetHistory(recorder);
            var cacher = Proxy.Cache<IComplexRepository>(repo, pastHistory);

            var cachedHistory = Proxy.GetCachedHistory(cacher);

            var replayer = Proxy.Replay<IComplexRepository>(cachedHistory);
            DataStructure result = replayer.GetRelated(null);

            Assert.AreEqual(firstResult.Name, result.Name, "otherwise this is because it was not cached");
            Assert.AreEqual(firstResult.Age, result.Age, "otherwise this is because it was not cached");
        }

        [Test]
        public void ReplaySavesCachedCombinedHistory()
        {
            var response = new DataStructure { Age = 21, Name = "Jimmy" };
            var repo = new ComplexRepositoryReturnsWhateverItWasSetupWith(response);
            var recorder = Proxy.Record<IComplexRepository>(repo);

            var pastHistory = Proxy.GetHistory(recorder);
            var cacher = Proxy.Cache<IComplexRepository>(repo, pastHistory);
            DataStructure firstResult = cacher.GetRelated(null);

            var cachedHistory = Proxy.GetCachedHistory(cacher);

            var replayer = Proxy.Replay<IComplexRepository>(cachedHistory);
            DataStructure result = replayer.GetRelated(null);

            Assert.AreEqual(firstResult.Name, result.Name, "otherwise this is because it was not cached");
            Assert.AreEqual(firstResult.Age, result.Age, "otherwise this is because it was not cached");
        }


        [Test]
        public void NullReturnedCallIsRecordedWithoutLossOfInformationWhenReplayed()
        {
            var data = new DataStructure { Age = 21, Name = "Jimmy" };
            var repo = new ComplexRepositoryReturnsWhateverItWasSetupWith(null);
            var recorder = Proxy.Record<IComplexRepository>(repo);
            DataStructure firstResult = recorder.GetRelated(data);

            string serialisedHistory = Proxy.SerialiseHistory(recorder);
            var history = Proxy.DeserialiseHistory(serialisedHistory);


            var replayer = Proxy.Cache<IComplexRepository>(
                new ComplexRepository2(), history);
            DataStructure result = replayer.GetRelated(data);

            Assert.AreEqual(firstResult, result, "otherwise this is because it was not cached");
        }

        [Test]
        public void WhenIModifyAParameterAfterLoggingIt_TheOriginalVerionShouldBeLogged()
        {
            var data = new DataStructure { Age = 21, Name = "Jimmy" };
            var repo = new ComplexRepositoryReturnsWhateverItWasSetupWith(new DataStructure());
            var recorder = Proxy.Record<IComplexRepository>(repo);
            DataStructure firstResult = recorder.GetRelated(data);

            data.Age++;

            string serialisedHistory = Proxy.SerialiseHistory(recorder);
            var history = Proxy.DeserialiseHistory(serialisedHistory);

            var replayer = Proxy.Replay<IComplexRepository>(history);
            DataStructure result = replayer.GetRelated(new DataStructure { Age = 21, Name = "Jimmy" });

            Assert.Pass("no exceptions (there will be an exception if the logged call does not have age 21");
        }

        [Test]
        public void WhenIModifyAReturnValueAfterLoggingIt_TheOriginalVerionShouldBeLogged()
        {
            var data = new DataStructure { Age = 21, Name = "Jimmy" };
            var repo = new ComplexRepositoryReturnsWhateverItWasSetupWith(data);
            var recorder = Proxy.Record<IComplexRepository>(repo);
            DataStructure firstResult = recorder.GetRelated(null);

            data.Age++;

            string serialisedHistory = Proxy.SerialiseHistory(recorder);
            var history = Proxy.DeserialiseHistory(serialisedHistory);

            var replayer = Proxy.Replay<IComplexRepository>(history);
            DataStructure result = replayer.GetRelated(null);
            Assert.AreEqual(21, result.Age);
        }

        public interface IStatefulBuilderWithPassByReference
        {
            DataStructure GetStructure();
            DataStructure SetAge(int age);
            DataStructure SetName(string name);
        }

        public class StatefulBuilderWithPassByReference : IStatefulBuilderWithPassByReference
        {
            private DataStructure _structure = new DataStructure();

            public DataStructure GetStructure()
            {
                return this._structure;
            }

            public DataStructure SetAge(int age)
            {
                _structure.Age = age;
                return this._structure;
            }

            public DataStructure SetName(string name)
            {
                _structure.Name = name;
                return this._structure;
            }
        }

        [Test]
        public void WhenIReplayAndRecordTwoFunctions_AndTheSecondModifiesTheFirstFunctionsReturnValueByReference()
        {
            var repo = new StatefulBuilderWithPassByReference();
            var recorder = Proxy.Record<IStatefulBuilderWithPassByReference>(repo);
            recorder.GetStructure();
            recorder.SetAge(14);
            recorder.SetName("Wesley");
            
            string serialisedHistory = Proxy.SerialiseHistory(recorder);
            var history = Proxy.DeserialiseHistory(serialisedHistory);

            var replayer = Proxy.Replay<IStatefulBuilderWithPassByReference>(history);
            DataStructure firstResult = replayer.GetStructure();
            DataStructure secondResult = replayer.SetAge(14);
            DataStructure thirdResult = replayer.SetName("Wesley");

            Assert.AreEqual(0, firstResult.Age);
            Assert.AreEqual(null, firstResult.Name);
            Assert.AreEqual(14, secondResult.Age);
            Assert.AreEqual(null, secondResult.Name);
            Assert.AreEqual(14, thirdResult.Age);
            Assert.AreEqual("Wesley", thirdResult.Name);
        }

        [Test]
        public void WhenIVerifyBehaviourOfTwoFunctions_AndTheSecondModifiesTheFirstFunctionsReturnValueByReference_WhenAsExpectedShouldPass()
        {
            var repo = new StatefulBuilderWithPassByReference();
            var recorder = Proxy.Record<IStatefulBuilderWithPassByReference>(repo);
            recorder.GetStructure();
            recorder.SetAge(14);
            recorder.SetName("Wesley");

            string serialisedHistory = Proxy.SerialiseHistory(recorder);
            var history = Proxy.DeserialiseHistory(serialisedHistory);

            var replayer = Proxy.GetBehaviourVerifier(history);
            var result = replayer.ConfirmBehaviourHasNotChanged(new StatefulBuilderWithPassByReference());
            Assert.IsTrue(result);
        }
    }
}