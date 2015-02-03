using System;
using NUnit.Framework;

namespace CoderTom.Mimics.Tests
{
    [TestFixture]
    public class ExceptionHandlingTests
    {
        [Test, ExpectedException(typeof(ApplicationException))]
        public void ExampleFakeRepositoryThrowsBehaviour_OnParameterlessFunction()
        {
            var exception = new ApplicationException("message");
            var repo = new FakeRepositoryThrows(exception);
            repo.ParameterlessFn();
        }
        [Test, ExpectedException(typeof(ApplicationException))]
        public void ExampleFakeRepositoryThrowsBehaviour_OnParameterisedFunction()
        {
            var exception = new ApplicationException("message");
            var repo = new FakeRepositoryThrows(exception);
            repo.Function(1);
        }

        [Test, ExpectedException(typeof(CustomException))]
        public void RecorderProxyBubblesException()
        {
            var exception = new CustomException();
            var repo = new FakeRepositoryThrows(exception);

            var testee = Mimic.Record<IFakeRepository>(repo);

            testee.ParameterlessFn();
        }

        [Test]
        public void RecorderProxyRecordsException()
        {
            var exception = new CustomException();
            var repo = new FakeRepositoryThrows(exception);

            var source = Mimic.Record<IFakeRepository>(repo);
            try
            {
                source.ParameterlessFn();
                Assert.Fail("Expect CustomException");
            }
            catch (CustomException)
            {
            }

            var log = Mimic.GetHistory(source);

            var testee = Mimic.Stub<IFakeRepository>(log);

            try
            {
                testee.ParameterlessFn();
                Assert.Fail();
            }
            catch (CustomException e)
            {
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
        }

        [Test]
        public void CacheProxyRecordsException()
        {
            var exception = new CustomException();
            var repo = new FakeRepositoryThrows(exception);

            var source = Mimic.Cache<IFakeRepository>(repo, new History());
            try
            {
                source.ParameterlessFn();
                Assert.Fail("Expect CustomException");
            }
            catch (CustomException)
            {
            }

            var log = Mimic.GetHistory(source);

            var testee = Mimic.Stub<IFakeRepository>(log);

            try
            {
                testee.ParameterlessFn();
                Assert.Fail();
            }
            catch (CustomException e)
            {
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
        }

        [Test]
        public void CacheProxyReplaysException()
        {
            var exceptionMessage = "message";
            var exception = new CustomException(exceptionMessage);
            var repo = new FakeRepositoryThrows(exception);

            var source = Mimic.Record<IFakeRepository>(repo);
            try
            {
                source.ParameterlessFn();
                Assert.Fail("Expect CustomException");
            }
            catch (CustomException)
            {
            }

            var log = Mimic.GetHistory(source);

            var otherRepo = new FakeRepositoryDoubles();
            var testee = Mimic.Cache<IFakeRepository>(otherRepo, log);

            try
            {
                testee.ParameterlessFn();
                Assert.Fail();
            }
            catch (CustomException e)
            {
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
        }

        [Test]
        public void CanSerialiseExceptionInHistoryLog()
        {       
            var exception = new CustomException();
            var repo = new FakeRepositoryThrows(exception);

            var source = Mimic.Record<IFakeRepository>(repo);
            try
            {
                source.ParameterlessFn();
                Assert.Fail("Expect CustomException");
            }
            catch (CustomException)
            {
            }
            var logSerialised = Mimic.SerialiseHistory(source);

            var testee = Mimic.Stub<IFakeRepository>(Mimic.DeserialiseHistory(logSerialised));

            try
            {
                testee.ParameterlessFn();
                Assert.Fail();
            }
            catch (CustomException e)
            {
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
        }

        [Test]
        public void BehaviourVerifier_VerifiesExceptionThrown_PassesWhenExpectedAndDoesOccur()
        {
            var repo = new FakeRepositoryThrows(new CustomException());
            var recorder = Mimic.Record<IFakeRepository>(repo);
            try
            {
                recorder.Function(1);
                Assert.Fail();
            }
            catch (CustomException)
            {}

            History history = Mimic.GetHistory(recorder);

            var testee = Mimic.GetBehaviourVerifier(history);
            var result = testee.ConfirmBehaviourHasNotChanged(new FakeRepositoryThrows(new CustomException()));
            Assert.True(result);
        }

        [Test]
        public void BehaviourVerifier_VerifiesExceptionThrown_FailsWhenNotExpectedAndDoesOccur_WithNonNullReturnValue()
        {
            var repo = new FakeRepositoryDoubles();
            var recorder = Mimic.Record<IFakeRepository>(repo);
            recorder.Function(1);

            History history = Mimic.GetHistory(recorder);

            var testee = Mimic.GetBehaviourVerifier(history);
            var result = testee.ConfirmBehaviourHasNotChanged(new FakeRepositoryThrows(new CustomException()));
            Assert.False(result);
        }

        [Test]
        public void BehaviourVerifier_WhenUnexpectedExceptionIsThrown_ShouldIncludeExceptionDetailInTheLog()
        {
            var repo = new FakeRepositoryDoubles();
            var recorder = Mimic.Record<IFakeRepository>(repo);
            recorder.Function(1);

            History history = Mimic.GetHistory(recorder);

            var testee = Mimic.GetBehaviourVerifier(history);
            var result = testee.ConfirmBehaviourHasNotChanged(new FakeRepositoryThrows(new CustomException()));
            Assert.IsTrue(testee.VerificationLog.ToString().Contains("CustomException"));
        }

        [Test]
        public void BehaviourVerifier_VerifiesExceptionThrown_FailsWhenNotExpectedAndDoesOccur_WithNullReturnValue()
        {
            var repo = new FakeRepositoryDefaults();
            var recorder = Mimic.Record<IFakeRepository>(repo);
            recorder.ParameterlessFn();

            History history = Mimic.GetHistory(recorder);

            var testee = Mimic.GetBehaviourVerifier(history);
            var result = testee.ConfirmBehaviourHasNotChanged(new FakeRepositoryThrows(new CustomException()));
            Assert.False(result);
        }

        [Test]
        public void BehaviourVerifier_VerifiesExceptionThrown_FailsWhenExpectedAndDoesNotOccur()
        {
            var repo = new FakeRepositoryThrows(new CustomException());
            var recorder = Mimic.Record<IFakeRepository>(repo);
            try
            {
                recorder.ParameterlessFn();
                Assert.Fail();
            }
            catch (CustomException)
            {
                
            }

            History history = Mimic.GetHistory(recorder);

            var testee = Mimic.GetBehaviourVerifier(history);
            var result = testee.ConfirmBehaviourHasNotChanged(new FakeRepositoryDoubles());
            Assert.False(result);
        }

        [Test, ExpectedException(typeof(TypeRequiresDefaultConstructorException))]
        public void WhenLoadingHistoryIntoReplayer_IfThrownExceptionHasNoDefaultConstructor_ThenThrow()
        {
            var exception = new ExceptionWithoutDefaultConstructor("required parameter");
            var repo = new FakeRepositoryThrows(exception);

            var source = Mimic.Record<IFakeRepository>(repo);
            try
            {
                source.ParameterlessFn();
                Assert.Fail("Expect Exception");
            }
            catch (ExceptionWithoutDefaultConstructor)
            {
            }
            var log = Mimic.GetHistory(source);

            var replayer = Mimic.Stub<IFakeRepository>(log);
        }
    }
}
