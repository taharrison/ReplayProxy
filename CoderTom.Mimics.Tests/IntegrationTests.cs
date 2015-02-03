using CoderTom.Mimics.Utilities;
using NUnit.Framework;

namespace CoderTom.Mimics.Tests
{
    [TestFixture]
    public class IntegrationTests
    {
        [Test, ExpectedException(typeof(TypeArgumentMustBeAnInterfaceException))]
        public void RecorderShouldThrowWhenCreatedWithoutSpecifyingAnInterface()
        {
            var repo = new FakeRepositorySquares();
            var recorder = Mimic.Record(repo);
        }

        [Test, ExpectedException(typeof(TypeArgumentMustBeAnInterfaceException))]
        public void ReplayerShouldThrowWhenCreatedWithoutSpecifyingAnInterface()
        {
            var repo = new FakeRepositorySquares();
            var recorder = Mimic.Record<IFakeRepository>(repo);

            var replayer = Mimic.Stub<FakeRepositorySquares>(Mimic.GetHistory(recorder));
        }

        [Test, ExpectedException(typeof(UnexpectedCallException))]
        public void ReplayProxy_Throws_WhenNoMatchingCallIsPresent()
        {
            var repo = new FakeRepositorySquares();
            var recorder = Mimic.Record<IFakeRepository>(repo);

            var replayer = Mimic.Stub<IFakeRepository>(Mimic.GetHistory(recorder));

            int result = replayer.Function(3);
        }

        [Test]
        public void CacheProxyAddsUncachedCallsToTheHistory()
        {
            var repo = new FakeRepositorySquares();
            var recorder = Mimic.Cache<IFakeRepository>(repo, new History());
            recorder.Function(3);
            History history = Mimic.GetHistory(recorder);


            var replayer = Mimic.Stub<IFakeRepository>(history);
            int result = replayer.Function(3);

            Assert.AreEqual(9, result);
        }

        [Test]
        public void CacheProxyCombinesUncachedCallsAndHistoricalCalls()
        {
            var repo = new FakeRepositorySquares();
            var recorder = Mimic.Cache<IFakeRepository>(repo, new History());
            recorder.Function(3);
            History history1 = Mimic.GetHistory(recorder);


            var cacher = Mimic.Cache<IFakeRepository>(repo, history1);
            cacher.Function(4);

            History history2 = Mimic.GetCachedHistory(cacher);
            var replayer = Mimic.Stub<IFakeRepository>(history2);
            Assert.AreEqual(16, replayer.Function(4));
            Assert.AreEqual(9, replayer.Function(3));
        }

        [Test]
        public void CacheProxyPassesUncachedCallsToTheBaseType()
        {
            var repo = new FakeRepositorySquares();
            var recorder = Mimic.Record<IFakeRepository>(repo);
            recorder.Function(3);
            History history = Mimic.GetHistory(recorder);


            var replayer = Mimic.Cache<IFakeRepository>(new FakeRepositoryDoubles(),
                history);
            int result = replayer.Function(4);

            Assert.AreEqual(8, result);
        }

        [Test]
        public void CacheProxyReplaysCachedCalls()
        {
            var repo = new FakeRepositorySquares();
            var recorder = Mimic.Record<IFakeRepository>(repo);
            recorder.Function(3);
            History history = Mimic.GetHistory(recorder);


            var replayer = Mimic.Cache<IFakeRepository>(new FakeRepositoryDoubles(),
                history);
            int result = replayer.Function(3);

            Assert.AreEqual(9, result, "if result is 6 then this is because it was not cached");
        }

        [Test]
        public void CacheProxyReplaysCachedCallsForFnsWithNoParameters()
        {
            var repo = new FakeRepositorySquares();
            var recorder = Mimic.Record<IFakeRepository>(repo);
            recorder.ParameterlessFn();
            History history = Mimic.GetHistory(recorder);


            var replayer = Mimic.Cache<IFakeRepository>(new FakeRepositoryDoubles(),
                history);
            string result = replayer.ParameterlessFn();

            Assert.AreEqual("beep", result, "if result is not 'beep' then this is because it was not cached");
        }

        [Test]
        public void CanSerialiseAndDeserialiseHistory_AndReplayResultsAsAMock()
        {
            var repo = new FakeRepositorySquares();
            var recorder = Mimic.Record<IFakeRepository>(repo);
            recorder.Function(2);
            recorder.Function(3);
            recorder.Function(4);

            string serialised = Mimic.SerialiseHistory(recorder);

            var deserialised = Mimic.DeserialiseHistory(serialised);

            var replayer = Mimic.Stub<IFakeRepository>(deserialised);
            int result = replayer.Function(3);

            Assert.AreEqual(9, result);
        }

        [Test]
        public void CanSaveAndLoadHistory_AndReplayResultsAsAMock()
        {
            var repo = new FakeRepositorySquares();
            var recorder = Mimic.Record<IFakeRepository>(repo);
            recorder.Function(2);
            recorder.Function(3);
            recorder.Function(4);

            Mimic.SaveHistoryToFile(recorder, "test_commandhistory.xml");

            var replayer = Mimic.Stub<IFakeRepository>(Mimic.HistoryFromFile("test_commandhistory.xml"));
            int result = replayer.Function(3);

            Assert.AreEqual(9, result);
        }

        [Test]
        public void CanSerialiseObject()
        {
            var u = new Parameter
            {
                Name = "fred",
                ParameterActualType = "System.Int32",
                Value = 128
            };

            string result = XmlSerialiser.SerialiseWithoutNamespacesAndHeaderWithLinebreaks(u, typeof (Parameter));
            Assert.IsTrue(result.Contains("128"));
        }

        [Test]
        public void RecorderShouldActAsAProxy()
        {
            var repo = new FakeRepositorySquares();
            var testee = Mimic.Record<IFakeRepository>(repo);

            int result = testee.Function(3);

            Assert.AreEqual(9, result);
        }

        [Test]
        public void ShouldRecordInvokationResultsAsAProxy()
        {
            var repo = new FakeRepositorySquares();
            var recorder = Mimic.Record<IFakeRepository>(repo);
            recorder.Function(3);

            History history = Mimic.GetHistory(recorder);

            LoggedCall result = history.GetCall(0);

            Assert.AreEqual("Function", result.MethodName);
        }

        [Test]
        public void ShouldRecordInvokationResultsAsAProxy_AndReplayResultsOfSameParameteredCallAsAMock()
        {
            var repo = new FakeRepositorySquares();
            var recorder = Mimic.Record<IFakeRepository>(repo);
            recorder.Function(2);
            recorder.Function(3);
            recorder.Function(4);

            History history = Mimic.GetHistory(recorder);

            var replayer = Mimic.Stub<IFakeRepository>(history);
            int result = replayer.Function(3);

            Assert.AreEqual(9, result);
        }

        [Test]
        public void ShouldRecordInvokationResultsAsAProxy_AndReplaySameResultsAsAMock()
        {
            var repo = new FakeRepositorySquares();
            var recorder = Mimic.Record<IFakeRepository>(repo);
            recorder.Function(3);

            History history = Mimic.GetHistory(recorder);

            var replayer = Mimic.Stub<IFakeRepository>(history);
            int result = replayer.Function(3);

            Assert.AreEqual(9, result);
        }

        [Test]
        public void CanSerialiseAndDeserialiseHistory_ForRepositoryWithMethodOverloading_AndReplayResultsAsAMock()
        {
            var repo = new RepositoryWithMethodOverloading();
            var recorder = Mimic.Record<IRepositoryWithMethodOverloading>(repo);
            var returnvalue1 = recorder.OverloadedMethod(2);
            var returnvalue2 = recorder.OverloadedMethod("2");
            var returnvalue3 = recorder.OverloadedMethod(4, "s");
            var returnvalue4 = recorder.OverloadedMethod();

            string serialised = Mimic.SerialiseHistory(recorder);

            var deserialised = Mimic.DeserialiseHistory(serialised);

            var replayer = Mimic.Stub<IRepositoryWithMethodOverloading>(deserialised);
            Assert.AreEqual(returnvalue1, replayer.OverloadedMethod(2));
            Assert.AreEqual(returnvalue2, replayer.OverloadedMethod("2"));
            Assert.AreEqual(returnvalue3, replayer.OverloadedMethod(4, "s"));
            Assert.AreEqual(returnvalue4, replayer.OverloadedMethod());
        }

        public interface IRepositoryWithVoidMethod
        {
            void VoidMethod();
            string ParameterlessString();
        }

        public class RepositoryWithVoidMethod : IRepositoryWithVoidMethod
        {
            public void VoidMethod()
            {
                
            }

            public string ParameterlessString()
            {
                return "ParameterlessString";
            }
        }

        [Test]
        public void CanSerialiseAndDeserialiseHistory_WhichIncludesVoidMethods()
        {
            var recorder = Mimic.Record<IRepositoryWithVoidMethod>(new RepositoryWithVoidMethod());
            recorder.VoidMethod();
            var expected = recorder.ParameterlessString();

            var history = Mimic.SerialiseHistory(recorder);

            var replayer = Mimic.Stub<IRepositoryWithVoidMethod>(Mimic.DeserialiseHistory(history));

            replayer.VoidMethod();
            Assert.AreEqual(expected, replayer.ParameterlessString());

            Assert.Pass("we are expecting no exceptions");
        }

        [Test]
        public void CanConfirmBehviourOfRepository_WithHistoryWhichIncludesVoidMethods()
        {
            var recorder = Mimic.Record<IRepositoryWithVoidMethod>(new RepositoryWithVoidMethod());
            recorder.VoidMethod();
            var expected = recorder.ParameterlessString();

            var history = Mimic.SerialiseHistory(recorder);

            var verifier = Mimic.GetBehaviourVerifier(Mimic.DeserialiseHistory(history));
            verifier.ConfirmBehaviourHasNotChanged(new RepositoryWithVoidMethod());

            var stringOfLog = verifier.VerificationLog.ToString();

            Assert.Pass("we are expecting no exceptions");
        }
        
        public interface IHiddenBaseMultipliesByY
        {
            int Function(int x);
        }

        public interface IHidingDerivedTriples : IHiddenBaseMultipliesByY
        {
            int Function(int x);
        }

        public class HiddenBaseMultipliesByY : IHiddenBaseMultipliesByY
        {
            public int Y { get; set; }
            public virtual int Function(int x)
            {
                return x*Y;
            }
        }

        public class HidingDerivedMultipliesByX : HiddenBaseMultipliesByY, IHidingDerivedTriples
        {
            public int X { get; set; }
            int IHidingDerivedTriples.Function(int x)
            {
                return x * X;
            }
        }

        [Test]
        public void HiddenMethodsAndHidingMethodsShouldBeCorrectlyDistinguished()
        {
            var repo = new HidingDerivedMultipliesByX() { X = 3, Y = 1 };
            var proxy = Mimic.Record<IHidingDerivedTriples>(repo);

            var answer1 = proxy.Function(3);
            var answer2 = ((IHiddenBaseMultipliesByY)proxy).Function(3);

            var history = Mimic.GetHistory(proxy);

            var replayer = Mimic.Stub<IHidingDerivedTriples>(history);

            Assert.AreEqual(answer1, replayer.Function(3));
            Assert.AreEqual(answer2, ((IHiddenBaseMultipliesByY)replayer).Function(3));
        }

        [Test]
        public void HiddenMethodsAndHidingMethodsShouldHaveTheirBehaviourCorrectlyVerified_WhenBehaviourIsAsExpected()
        {
            var repo = new HidingDerivedMultipliesByX() { X = 3, Y = 1 };
            var proxy = Mimic.Record<IHidingDerivedTriples>(repo);

            var answer1 = proxy.Function(3);
            var answer2 = ((IHiddenBaseMultipliesByY)proxy).Function(3);

            var history = Mimic.GetHistory(proxy);


            var repoBehavesAsExpected = new HidingDerivedMultipliesByX() { X = 3, Y = 1 };
            var verifier = Mimic.GetBehaviourVerifier(history);
            var result = verifier.ConfirmBehaviourHasNotChanged(repoBehavesAsExpected);
            Assert.IsTrue(result, verifier.VerificationLog.ToString());
        }

        [Test]
        public void HiddenMethodsAndHidingMethodsShouldHaveTheirBehaviourCorrectlyVerified_WhenBehaviourIsNotAsExpected()
        {
            var repo = new HidingDerivedMultipliesByX() { X = 3, Y = 1 };
            var proxy = Mimic.Record<IHidingDerivedTriples>(repo);

            var answer1 = proxy.Function(3);
            var answer2 = ((IHiddenBaseMultipliesByY)proxy).Function(3);

            var history = Mimic.GetHistory(proxy);


            var repoBehavesAsExpected = new HidingDerivedMultipliesByX() { X = 2, Y = 1 };
            var verifier = Mimic.GetBehaviourVerifier(history);
            var result = verifier.ConfirmBehaviourHasNotChanged(repoBehavesAsExpected);
            Assert.IsFalse(result);

            var repoBehavesAsExpected2 = new HidingDerivedMultipliesByX() { X = 3, Y = 2 };
            var verifier2 = Mimic.GetBehaviourVerifier(history);
            var result2 = verifier.ConfirmBehaviourHasNotChanged(repoBehavesAsExpected2);
            Assert.IsFalse(result2);
        }

        public interface IOverridingRepository
        {
            string Function(object x);
            string Function(string x);
        }

        public class OverridingRepository : IOverridingRepository
        {
            public OverridingRepository(string objectName, string stringName)
            {
                ObjectName = objectName;
                StringName = stringName;
            }

            public string ObjectName { get; set; }

            public string StringName { get; set; }

            public string Function(object x)
            {
                return ObjectName + " : " +  x.ToString();
            }

            public string Function(string x)
            {
                return StringName + " : " + x;
            }
        }

        [Test]
        public void ShouldCorrectlyDistinguishOMethodOverrides_WhenReplaying()
        {
            var repo = new OverridingRepository("object", "string");
            var recorder = Mimic.Record<IOverridingRepository>(repo);

            var objExpected = recorder.Function((object)"3");
            var strExpected = recorder.Function("3");

            var serialisedHistory = Mimic.SerialiseHistory(recorder);

            var history = Mimic.DeserialiseHistory(serialisedHistory);

            var replayer = Mimic.Stub<IOverridingRepository>(history);


            Assert.AreEqual(objExpected, replayer.Function((object)"3"));
            Assert.AreEqual(strExpected, replayer.Function("3"));
        }
        [Test]
        public void BehaviourVerifierShouldCorrectlyVerifyBehaviourOfMethodOverrides_whenIsAsExpected()
        {
            var repo = new OverridingRepository("object", "string");
            var recorder = Mimic.Record<IOverridingRepository>(repo);

            recorder.Function((object)"3");
            recorder.Function("3");

            var serialisedHistory = Mimic.SerialiseHistory(recorder);

            var history = Mimic.DeserialiseHistory(serialisedHistory);

            var verifier = Mimic.GetBehaviourVerifier(history);
            var result = verifier.ConfirmBehaviourHasNotChanged(repo);
            Assert.IsTrue(result, verifier.VerificationLog.ToString());
        }
        [Test]
        public void BehaviourVerifierShouldCorrectlyVerifyBehaviourOfMethodOverrides_whenIsNotAsExpected()
        {
            var repo = new OverridingRepository("object", "object");
            var recorder = Mimic.Record<IOverridingRepository>(repo);

            recorder.Function((object)"3");
            recorder.Function("3");

            var serialisedHistory = Mimic.SerialiseHistory(recorder);

            var history = Mimic.DeserialiseHistory(serialisedHistory);

            var verifier = Mimic.GetBehaviourVerifier(history);

            var repo2 = new OverridingRepository("object", "string");
            var result = verifier.ConfirmBehaviourHasNotChanged(repo2);
            Assert.IsFalse(result, verifier.VerificationLog.ToString());
            var repo3 = new OverridingRepository("string", "object");
            var result2 = verifier.ConfirmBehaviourHasNotChanged(repo3);
            Assert.IsFalse(result2, verifier.VerificationLog.ToString());
        }
    }
}