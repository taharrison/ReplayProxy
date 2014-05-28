using NUnit.Framework;

namespace ReplayMocks.Tests
{
    [TestFixture]
    public class BehaviourVerifierTests
    {
        [Test]
        public void EmptyHistoryVerifiesOK()
        {
            var history = new History();
            var testee = Proxy.GetBehaviourVerifier(history);
            var result = testee.ConfirmBehaviourHasNotChanged(new FakeRepositorySquares());
            Assert.IsTrue(result);
        }

        [Test]
        public void HistoryWithOneCallThatDoesNotMatchReturnValueVerifiesFail()
        {
            var repo = new FakeRepositorySquares();
            var recorder = Proxy.Record<IFakeRepository>(repo);
            recorder.Function(3);
            History history = Proxy.GetHistory(recorder);

            var testee = Proxy.GetBehaviourVerifier(history);
            var result = testee.ConfirmBehaviourHasNotChanged(new FakeRepositoryDoubles());
            Assert.IsFalse(result);
        }

        [Test]
        public void ComplexParameter_WhenMatchesExpectation()
        {
            var ds = new DataStructure() {Age = 13, Name = "jimbob"};
            var repo = new ComplexRepositoryReturnsWhateverItWasSetupWith(new DataStructure());
            var recorder = Proxy.Record<IComplexRepository>(repo);
            recorder.GetRelated(ds);
            History history = Proxy.GetHistory(recorder);

            var testee = Proxy.GetBehaviourVerifier(history);
            var result = testee.ConfirmBehaviourHasNotChanged(new ComplexRepositoryReturnsWhateverItWasSetupWith(new DataStructure()));
            Assert.IsTrue(result);
        }

        [Test]
        public void HistoryWithOneCallThatDoesNotMatchReturnValueVerifiesFail_WhenNullIsReturned()
        {
            var ds = new DataStructure() { Age = 13, Name = "jimbob" };
            var repo = new ComplexRepositoryReturnsWhateverItWasSetupWith(new DataStructure());
            var recorder = Proxy.Record<IComplexRepository>(repo);
            recorder.GetRelated(ds);
            History history = Proxy.GetHistory(recorder);

            var testee = Proxy.GetBehaviourVerifier(history);
            var result = testee.ConfirmBehaviourHasNotChanged(
                new ComplexRepositoryReturnsWhateverItWasSetupWith(null));
            Assert.IsFalse(result);
        }

        [Test]
        public void HistoryWithOneCallThatDoesNotMatchReturnValueVerifiesFail_WhenNullIsExpected()
        {
            var ds = new DataStructure() { Age = 13, Name = "jimbob" };
            var repo = new ComplexRepositoryReturnsWhateverItWasSetupWith(null);
            var recorder = Proxy.Record<IComplexRepository>(repo);
            recorder.GetRelated(ds);
            History history = Proxy.GetHistory(recorder);

            var testee = Proxy.GetBehaviourVerifier(history);
            var result = testee.ConfirmBehaviourHasNotChanged(
                new ComplexRepositoryReturnsWhateverItWasSetupWith(new DataStructure()));
            Assert.IsFalse(result);
        }

        [Test]
        public void NullParameter_WhenMatchesExpectation()
        {
            var repo = new ComplexRepositoryReturnsWhateverItWasSetupWith(new DataStructure());
            var recorder = Proxy.Record<IComplexRepository>(repo);
            recorder.GetRelated(null);
            History history = Proxy.GetHistory(recorder);

            var testee = Proxy.GetBehaviourVerifier(history);
            var result = testee.ConfirmBehaviourHasNotChanged(new ComplexRepositoryReturnsWhateverItWasSetupWith(new DataStructure()));
            Assert.IsTrue(result);
        }

        #region derived types

        [Test]
        public void DerivedParameter_WhenMatchesExpectation()
        {
            var ds = new DerivedDataStructure() { Balance = 2 };
            var repo = new ComplexRepositoryReturnsDerivedType();

            var recorder = Proxy.Record<IComplexRepository>(repo);
            recorder.GetRelated(ds);
            History history = Proxy.GetHistory(recorder);

            var testee = Proxy.GetBehaviourVerifier(history);
            var result = testee.ConfirmBehaviourHasNotChanged(
                new ComplexRepositoryReturnsDerivedType());
            Assert.IsTrue(result);
        }

        [Test]
        public void DerivedReturnValue_WhenMatchesExpectation()
        {
            var ds = new DataStructure() { Age = 13, Name = "jimbob" };
            var repo = new ComplexRepositoryReturnsWhateverItWasSetupWith(
                new DerivedDataStructure() { Balance = 2 });

            var recorder = Proxy.Record<IComplexRepository>(repo);
            recorder.GetRelated(ds);
            History history = Proxy.GetHistory(recorder);

            var testee = Proxy.GetBehaviourVerifier(history);
            var result = testee.ConfirmBehaviourHasNotChanged(
                new ComplexRepositoryReturnsWhateverItWasSetupWith(
                    new DerivedDataStructure() { Balance = 2}));
            Assert.IsTrue(result);
        }

        [Test]
        public void HistoryWithOneCallThatDoesNotMatchReturnValueVerifiesFail_WhenDerivedIsReturned()
        {
            var ds = new DataStructure() { Age = 13, Name = "jimbob" };
            var repo = new ComplexRepositoryReturnsWhateverItWasSetupWith(new DataStructure() { Name = "jim2" });
            var recorder = Proxy.Record<IComplexRepository>(repo);
            recorder.GetRelated(ds);
            History history = Proxy.GetHistory(recorder);

            var testee = Proxy.GetBehaviourVerifier(history);
            var result = testee.ConfirmBehaviourHasNotChanged(
                new ComplexRepositoryReturnsWhateverItWasSetupWith(new DerivedDataStructure() { Name = "jim2"}));
            Assert.IsFalse(result);
        }

        [Test]
        public void HistoryWithOneCallThatDoesNotMatchReturnValueVerifiesFail_WhenDerivedIsExpected()
        {
            var ds = new DataStructure() { Age = 13, Name = "jimbob" };
            var repo = new ComplexRepositoryReturnsWhateverItWasSetupWith(new DerivedDataStructure() { Name = "jim2"});
            var recorder = Proxy.Record<IComplexRepository>(repo);
            recorder.GetRelated(ds);
            History history = Proxy.GetHistory(recorder);

            var testee = Proxy.GetBehaviourVerifier(history);
            var result = testee.ConfirmBehaviourHasNotChanged(
                new ComplexRepositoryReturnsWhateverItWasSetupWith(new DataStructure() { Name = "jim2" }));
            Assert.IsFalse(result);
        }

        #endregion

        [Test]
        public void HistoryWithSecondCallThatDoesNotMatchReturnValueVerifiesFail()
        {
            var repo = new FakeRepositorySquares();
            var recorder = Proxy.Record<IFakeRepository>(repo);
            recorder.Function(2);
            recorder.Function(3);
            History history = Proxy.GetHistory(recorder);

            var testee = Proxy.GetBehaviourVerifier(history);
            var result = testee.ConfirmBehaviourHasNotChanged(new FakeRepositoryDoubles());
            Assert.IsFalse(result);
        }

        [Test]
        public void HistoryWithOneCallThatMatchesReturnValueVerifiesOK()
        {
            var repo = new FakeRepositorySquares();
            var recorder = Proxy.Record<IFakeRepository>(repo);
            recorder.Function(3);
            History history = Proxy.GetHistory(recorder);

            var testee = Proxy.GetBehaviourVerifier(history);
            var result = testee.ConfirmBehaviourHasNotChanged(new FakeRepositorySquares());
            Assert.IsTrue(result);
        }

        [Test]
        public void HistoryWithOneCallThatMatchesReturnHasLogWhichVerifiesOK()
        {
            var repo = new FakeRepositorySquares();
            var recorder = Proxy.Record<IFakeRepository>(repo);
            recorder.Function(3);
            History history = Proxy.GetHistory(recorder);

            var testee = Proxy.GetBehaviourVerifier(history);
            testee.ConfirmBehaviourHasNotChanged(new FakeRepositorySquares());

            var result = testee.VerificationLog[0];

            Assert.IsTrue(result.Pass);
            Assert.IsTrue(result.ToString().Contains("Function"));
            Assert.IsTrue(result.ToString().Contains("3"));
            Assert.IsTrue(result.ToString().Contains("9"));
            Assert.IsTrue(result.ToString().ToLower().Contains("pass"));
        }

        [Test]
        public void HistoryWithOneCallThatMatchesReturnHasLogWhichVerifiesFail()
        {
            var repo = new FakeRepositorySquares();
            var recorder = Proxy.Record<IFakeRepository>(repo);
            recorder.Function(3);
            History history = Proxy.GetHistory(recorder);

            var testee = Proxy.GetBehaviourVerifier(history);
            testee.ConfirmBehaviourHasNotChanged(new FakeRepositoryDoubles());

            var result = testee.VerificationLog[0];

            Assert.IsFalse(result.Pass);
            Assert.IsTrue(result.ToString().Contains("Function"));
            Assert.IsTrue(result.ToString().Contains("3"));
            Assert.IsTrue(result.ToString().Contains("9"));
            Assert.IsTrue(result.ToString().Contains("6"));
            Assert.IsTrue(result.ToString().ToLower().Contains("fail"));
        }

        [Test]
        public void HistoryWithTwoCallsHasDebugLogWhichOutputsAll()
        {
            var repo = new FakeRepositorySquares();
            var recorder = Proxy.Record<IFakeRepository>(repo);
            recorder.Function(2);
            recorder.ParameterlessFn();
            History history = Proxy.GetHistory(recorder);

            var testee = Proxy.GetBehaviourVerifier(history);
            testee.ConfirmBehaviourHasNotChanged(new FakeRepositoryDoubles());

            var result = testee.VerificationLog.ToString();

            Assert.IsTrue(result.Contains("Function"));
            Assert.IsTrue(result.Contains("2"));
            Assert.IsTrue(result.Contains("4"));
            Assert.IsTrue(result.ToLower().Contains("pass"));
        }

        [Test]
        public void HistoryComplexParameterStillVerifiesOk()
        {
            var tony = new DataStructure() { Name = "tony", Age = 21 };

            var repo = new ComplexRepository();
            var recorder = Proxy.Record<IComplexRepository>(repo);
            recorder.GetRelated(tony);
            History history = Proxy.GetHistory(recorder);

            var testee = Proxy.GetBehaviourVerifier(history);
            testee.ConfirmBehaviourHasNotChanged(new ComplexRepository());

            var result = testee.VerificationLog.ToString();

            Assert.IsTrue(result.Contains("GetRelated"));
            Assert.IsTrue(result.Contains("tony"));
            Assert.IsTrue(result.Contains("snr"));
            Assert.IsTrue(result.ToLower().Contains("pass"));
        }

        [Test]
        public void HistoryMismatchedComplexParameterStillFailsOk()
        {
            var tony = new DataStructure() { Name = "tony", Age = 21 };

            var repo = new ComplexRepository();
            var recorder = Proxy.Record<IComplexRepository>(repo);
            recorder.GetRelated(tony);
            History history = Proxy.GetHistory(recorder);

            var testee = Proxy.GetBehaviourVerifier(history);
            testee.ConfirmBehaviourHasNotChanged(new ComplexRepository2());

            var result = testee.VerificationLog.ToString();

            Assert.IsTrue(result.Contains("GetRelated"));
            Assert.IsTrue(result.Contains("<Name>tony jr.</Name>"));
            Assert.IsTrue(result.Contains("<Name>tony snr.</Name>"));
            Assert.IsTrue(result.ToLower().Contains("fail"));
        }
    }
}