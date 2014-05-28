using System;
using NUnit.Framework;

namespace ReplayMocks.Tests
{
    public interface IFakeRepository
    {
        int GetIntSquared(int n);
    }
    public class FakeRepository : IFakeRepository
    {
        public int GetIntSquared(int n)
        {
            return n*n;
        }
    }

    [TestFixture]
    public class IntegrationTests
    {
        [Test]
        public void RecorderShouldActAsAProxy()
        {
            var repo = new FakeRepository();
            var testee = new Recorder<IFakeRepository>(repo);

            var result = testee.Object.GetIntSquared(3);

            Assert.AreEqual(9, result);
        }

        [Test]
        public void ShouldRecordInvokationResultsAsAProxy()
        {
            var repo = new FakeRepository();
            var recorder = new Recorder<IFakeRepository>(repo);
            recorder.Object.GetIntSquared(3);

            var history = recorder.GetHistory();

            var result = history.GetCall(0);

            Assert.AreEqual("GetIntSquared", result.MethodName);
        }
        
        [Test]
        public void ShouldRecordInvokationResultsAsAProxy_AndReplaySameResultsAsAMock()
        {
            var repo = new FakeRepository();
            var recorder = new Recorder<IFakeRepository>(repo);
            recorder.Object.GetIntSquared(3);

            var history = recorder.GetHistory();

            var replayer = new Replayer<IFakeRepository>(history);
            var result = replayer.Object.GetIntSquared(3);

            Assert.AreEqual(9, result);
        }

        [Test]
        public void ShouldRecordInvokationResultsAsAProxy_AndReplayResultsOfSameParameteredCallAsAMock()
        {
            var repo = new FakeRepository();
            var recorder = new Recorder<IFakeRepository>(repo);
            recorder.Object.GetIntSquared(2);
            recorder.Object.GetIntSquared(3);
            recorder.Object.GetIntSquared(4);

            var history = recorder.GetHistory();

            var replayer = new Replayer<IFakeRepository>(history);
            var result = replayer.Object.GetIntSquared(3);

            Assert.AreEqual(9, result);
        }

        [Test]
        public void CanSerialiseAndDeserialiseHistory_AndReplayResultsAsAMock()
        {
            var repo = new FakeRepository();
            var recorder = new Recorder<IFakeRepository>(repo);
            recorder.Object.GetIntSquared(2);
            recorder.Object.GetIntSquared(3);
            recorder.Object.GetIntSquared(4);

            var history = recorder.GetHistory();

            var serialised = XmlSerialiser.SerialiseWithoutNamespacesAndHeaderWithLinebreaks(history, typeof(History));

            var deserialised = (History)XmlSerialiser.Deserialise(serialised, typeof (History));

            var replayer = new Replayer<IFakeRepository>(deserialised);
            var result = replayer.Object.GetIntSquared(3);

            Assert.AreEqual(9, result);
        }
    }
}
