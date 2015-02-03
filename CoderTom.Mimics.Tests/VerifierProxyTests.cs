using System;
using NUnit.Framework;

namespace CoderTom.Mimics.Tests
{
    [TestFixture]
    public class VerifierProxyTests
    {
        [Test]
        public void VerifierProxy_AssertsPass_ForAnEmptyCallList()
        {
            var verifier = GivenIExpect_WhenIObserveBehaviour(
                expected: (repo) => { ; }, 
                observed: (repo) => { ; });

            var result = Mimic.ConfirmSameCallsWereMade(verifier);
            Assert.IsTrue(result);
        }

        private static IFakeRepository GivenIExpect_WhenIObserveBehaviour_WithCaching(
            Action<IFakeRepository> expected,
            Action<IFakeRepository> observed)
        {
            return GivenIExpect_WhenIObserveBehaviour(expected, observed, true);
        }

        private static IFakeRepository GivenIExpect_WhenIObserveBehaviour(Action<IFakeRepository> expected,
            Action<IFakeRepository> observed, bool caching = false)
        {
            var repo = new FakeRepositorySquares();
            var recorder = Mimic.Record<IFakeRepository>(repo);

            try
            {
                expected(recorder);
            }
            catch (Exception)
            {
            }

            string serialised = Mimic.SerialiseHistory(recorder);
            var log = Mimic.DeserialiseHistory(serialised);
            var verifier = caching ? 
                Mimic.Cache<IFakeRepository>(new FakeRepositorySquares(), log) 
                : Mimic.Stub<IFakeRepository>(log);

            try
            {
                observed(verifier);
            }
            catch (Exception)
            {
            }

            return verifier;
        }

        [Test]
        public void VerifierProxy_AssertsFail_WhenNoCallsAreExpectedAndOneIsMade()
        {
            var verifier = GivenIExpect_WhenIObserveBehaviour(
                expected: (repo) => { ; }, 
                observed: (repo) => { repo.Function(3); });
            
            var result = Mimic.ConfirmSameCallsWereMade(verifier);
            Assert.IsFalse(result);
        }

        [Test]
        public void VerifierProxy_AssertsFail_WhenOneCallsIsExpectedAndNoneIsMade()
        {
            var verifier = GivenIExpect_WhenIObserveBehaviour(
                expected: (repo) => { repo.Function(3); },
                observed: (repo) => { ; });

            var result = Mimic.ConfirmSameCallsWereMade(verifier);
            Assert.IsFalse(result);
        }

        [Test]
        public void VerifierProxy_AssertsPass_WhenTwoCallsAreExpectedAndAreMade()
        {
            var verifier = GivenIExpect_WhenIObserveBehaviour(
                expected: (repo) => { repo.Function(2); repo.Function(-1); },
                observed: (repo) => { repo.Function(2); repo.Function(-1); });

            var result = Mimic.ConfirmSameCallsWereMade(verifier);
            Assert.IsTrue(result);
        }

        [Test]
        public void VerifierProxy_AssertsPass_WhenTwoCallsAreExpectedAndAreMade_InEitherOrder()
        {
            var verifier = GivenIExpect_WhenIObserveBehaviour(
                expected: (repo) => { repo.Function(-1); repo.Function(2); },
                observed: (repo) => { repo.Function(2); repo.Function(-1); });

            var result = Mimic.ConfirmSameCallsWereMade(verifier);
            Assert.IsTrue(result);
        }

        [Test]
        public void VerifierProxy_AssertsPass_WhenSameCallIsMadeAndObservedThreeTimes()
        {
            var verifier = GivenIExpect_WhenIObserveBehaviour(
                expected: (repo) => { repo.Function(2); repo.Function(2); repo.Function(2); },
                observed: (repo) => { repo.Function(2); repo.Function(2); repo.Function(2); });

            var result = Mimic.ConfirmSameCallsWereMade(verifier);
            Assert.IsTrue(result);
        }

        [Test]
        public void VerifierProxy_AssertsFail_WhenSameCallIsMadeAndObservedFewerTimes()
        {
            var verifier = GivenIExpect_WhenIObserveBehaviour(
                expected: (repo) => { repo.Function(2); repo.Function(2); repo.Function(2); },
                observed: (repo) => { repo.Function(2); repo.Function(2); });

            var result = Mimic.ConfirmSameCallsWereMade(verifier);
            Assert.IsFalse(result);
        }

        [Test]
        public void VerifierProxy_AssertsFailByDefault_WhenSameCallIsMadeAndObservedMoreTimes()
        {
            var verifier = GivenIExpect_WhenIObserveBehaviour(
                expected: (repo) => { repo.Function(2); repo.Function(2); },
                observed: (repo) => { repo.Function(2); repo.Function(2); repo.Function(2); });

            var result = Mimic.ConfirmSameCallsWereMade(verifier);
            Assert.IsFalse(result);
        }

        [Test]
        public void VerifierProxy_AssertsOk_WhenSameCallIsMadeAndObservedMoreTimes_AndNumberDoesNotMatter()
        {
            var verifier = GivenIExpect_WhenIObserveBehaviour(
                expected: (repo) => { repo.Function(2); repo.Function(2); },
                observed: (repo) => { repo.Function(2); repo.Function(2); repo.Function(2); });

            var result = Mimic.ConfirmSameCallsWereMade(verifier, MockOptions.Default_SameCallsSameNumberOfTimesInAnyOrder ^ MockOptions.ExpectedCallsMustBeObservedTheExactNumberOfTimes);
            Assert.IsTrue(result);
        }

        [Test]
        public void VerifierProxy_AssertsFail_WhenOrderIsSpecified_AndCallsAreObservedInTheWrongOrder()
        {
            var verifier = GivenIExpect_WhenIObserveBehaviour(
                expected: (repo) => { repo.Function(-1); repo.Function(2); },
                observed: (repo) => { repo.Function(2); repo.Function(-1); });

            var result = Mimic.ConfirmSameCallsWereMade(verifier, MockOptions.CallsMustOccurInTheExactSameOrder);
            Assert.IsFalse(result);
        }

        [Test]
        public void VerifierProxy_AssertsPass_WhenAdditionalCallsAreAllowedAndAreObserved()
        {
            var verifier = GivenIExpect_WhenIObserveBehaviour_WithCaching(
                expected: (repo) => { repo.Function(2); repo.Function(3); },
                observed: (repo) => { repo.Function(2); repo.Function(-1); repo.Function(3); });

            var result = Mimic.ConfirmSameCallsWereMade(verifier, MockOptions.AllExpectedCallsMustBeObserved | MockOptions.CallsMustOccurInTheExactSameOrder);
            Assert.IsTrue(result);
        }

        [Test]
        public void VerifierProxy_AssertsPass_WhenAdditionalCallsAreAllowedAndAreObservedIncludingOnesMatchingParams()
        {
            var verifier = GivenIExpect_WhenIObserveBehaviour_WithCaching(
                expected: (repo) => { repo.Function(2); repo.Function(3); },
                observed: (repo) => { repo.Function(3); repo.Function(2); repo.Function(3); });

            var result = Mimic.ConfirmSameCallsWereMade(verifier, MockOptions.AllExpectedCallsMustBeObserved | MockOptions.CallsMustOccurInTheExactSameOrder);
            Assert.IsTrue(result);
        }

        [Test]
        public void VerifierProxy_AssertsPass_WhenNumberOfCallsDoesNotMatterAndVariesHigher()
        {
            var verifier = GivenIExpect_WhenIObserveBehaviour_WithCaching(
                expected: (repo) => { repo.Function(2); repo.Function(3); },
                observed: (repo) => { repo.Function(3); repo.Function(2); repo.Function(3); });

            var result = Mimic.ConfirmSameCallsWereMade(verifier, MockOptions.AllExpectedCallsMustBeObserved | MockOptions.OnlyExpectedCallsAreAllowed);
            Assert.IsTrue(result);
        }

        [Test]
        public void VerifierProxy_AssertsPass_WhenNumberOfCallsDoesNotMatterAndVariesLower()
        {
            var verifier = GivenIExpect_WhenIObserveBehaviour_WithCaching(
                expected: (repo) => { repo.Function(2); repo.Function(2); repo.Function(3); repo.Function(2); repo.Function(3); },
                observed: (repo) => { repo.Function(3); repo.Function(2); });

            var result = Mimic.ConfirmSameCallsWereMade(verifier, MockOptions.AllExpectedCallsMustBeObserved | MockOptions.OnlyExpectedCallsAreAllowed);
            Assert.IsTrue(result);
        }
    }
}