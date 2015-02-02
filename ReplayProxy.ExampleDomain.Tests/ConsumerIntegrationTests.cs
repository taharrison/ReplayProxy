using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ReplayProxy.ExampleDomain.Tests
{
    [TestFixture]
    public class ConsumerIntegrationTests
    {
        /// <summary>
        /// An integration test on the example domain,
        /// end to end, without using ReplayProxy at all.
        /// </summary>
        /// <remarks>
        /// Consumer ---CALLS---→ Repository
        /// Consumer ←---DATA---- Repository
        /// </remarks>
        [Test]
        public void CustomerReportShouldContainAllCustomersMissingAddresses()
        {
            var repository = new Repository();
            var consumer = new Consumer(repository); // using inversion of control

            var result = consumer.GetMissingAddressesReport();

            AssertReportMatchesExpectations(result); // for the purpose of this example we don't need to know what kinds of assertions we are running on the report.
        }

        /// <summary>
        /// This method is /not actually intended as a test/ but is based on 
        /// the test "CustomerReportShouldContainAllCustomersMissingAddresses"
        /// This method records the behaviour at the service boundary, so that
        /// the log of that behaviour "MissingAddressReport_CallHistory_out.xml"
        /// can be used in other tests such as
        /// "CustomerReportShouldContainAllCustomersMissingAddresses_IsolationTest"
        /// </summary>
        /// <remarks>
        /// Consumer ---CALLS---→ Proxy Recorder ---CALLS---→ Repository
        /// Consumer ←---DATA---- Proxy Recorder ←---DATA---- Repository
        /// 
        /// Proxy recorder logs all traffic passing through it.
        /// </remarks>
        [Test]
        public void CustomerReportShouldContainAllCustomersMissingAddresses_RecordBehaviour()
        {
            var repository = new Repository();
            var recorderProxy = Proxy.Record<IRepository>(repository);
            var consumer = new Consumer(recorderProxy);

            var result = consumer.GetMissingAddressesReport();

            AssertReportMatchesExpectations(result);

            Proxy.SaveHistoryToFile(recorderProxy, "MissingAddressReport_CallHistory_out.xml");
        }

        /// <summary>
        /// this kind of test is ideal for quickly isolating units in legacy code, so their overcomplicated behaviour
        /// can be massaged into a better, more understandable shape
        /// </summary>
        /// <remarks>
        /// Consumer ---CALLS---→ Replayer -X        Repository
        /// Consumer ←---DATA---- Replayer -X        Repository
        /// 
        /// Replayer returns previously recorded values.
        /// </remarks>
        [Test]
        public void CustomerReportShouldContainAllCustomersMissingAddresses_IsolationTest()
        {
            // *** before running this test for the first time, the behaviour of the dependent layer needs to be serialised
            // using something like the temporary test CustomerReportShouldContainAllCustomersMissingAddresses_RecordBehaviour
            // or the cached test CustomerReportShouldContainAllCustomersMissingAddresses_CachedIntegrationTest

            // we are using a replayer proxy.  this kind of proxy has no object behind it; it pretends to be the required
            // object but only replays the values that have been previously recorded as correct behaviour for each method call
            // if called with unfamiliar method names or parameters, an exception will be thrown.
            var history = Proxy.HistoryFromFile("MissingAddressReport_CallHistory.xml");
            var replayerProxy = Proxy.Replay<IRepository>(history);

            var testee = new Consumer(replayerProxy);

            var result = testee.GetMissingAddressesReport();

            AssertReportMatchesExpectations(result);
        }

        /// <summary>
        /// this kind of test is a hybrid and requires maintenance.
        /// we cache the behaviour of a slow-responding dependency and can then heavily refactor the consuming layer with fast
        /// test feedback. because we are caching, the tests won't fail if we take methods from the consuming layer and move 
        /// them into the dependency layer
        /// for example we may begin with a complicated service which mixes logic and multiple data access calls; we can use 
        /// this kind of proxy to maintain test coverage while refactoring each data access call into a new repository; 
        /// eventually isolating the two concerns completely
        /// </summary>
        /// <remarks>
        /// Consumer ---CALLS---→ Cache Proxy ---CALLS---→ Repository
        /// Consumer ←---DATA---- Cache Proxy ←---DATA---- Repository
        /// 
        /// Consumer ---CALLS---→ Cache Proxy -X           Repository
        /// Consumer ←---DATA---- Cache Proxy -X           Repository
        ///  
        /// Cache Proxy logs calls it hasn't seen before, and returns previously recorded values.
        /// </remarks>
        [Test]
        public void CustomerReportShouldContainAllCustomersMissingAddresses_CachedIntegrationTest()
        {
            // by using the CacheProxy here, calls to a potentially expensive or unavailable resource can be calculated once
            // and then reused for future test runs.

            // the first time you run this test, you can use "new History()" for the history parameter instead of a filename.
            // then take the history xml from the end of the test and use that as your initialiser from then on
            var repository = new Repository();
            History history = Proxy.HistoryFromFile("MissingAddressReport_CallHistory.xml");
            var cacheProxy = Proxy.Cache<IRepository>(repository, history);

            var consumer = new Consumer(cacheProxy);

            var result = consumer.GetMissingAddressesReport();

            // after using the CacheProxy, if anything has changed you can read the new serialised cache out again
            // you may want to completely recalculate the cached calls, which you can do by passing a new History instance 
            // into the GetCacheProxy method

            Proxy.SaveCachedHistoryToFile(cacheProxy, "MissingAddressReport_CallHistory_out.xml");

            AssertReportMatchesExpectations(result);
        }

        private static void AssertReportMatchesExpectations(string result)
        {
            Assert.IsTrue(result.Contains("Total contacts missing addresses: 1"), "Wrong number of results");
            Assert.IsTrue(result.Contains("12343"), "Contact Ids should be included in report");
        }

        /// <summary>
        /// this kind of test is ideal for hard to understand / hard to test repositories
        /// for example, a repository that loads data from a hard to understand SQL stored procedure
        /// by covering the repository adequately with this kind of test, the data layer can be refactored without fear.
        /// </summary>
        /// <remarks>
        /// Consumer           X- Verifier ---CALLS---→ Repository
        /// Consumer           X- Verifier ←---DATA---- Repository
        /// 
        /// Verifier executes all the recorded calls against the Repository and enables regression testing against
        /// previously recorded values.
        /// </remarks>
        [Test]
        public void ConfirmRepositoryBehviourForSampleTrafficHasNotChanged_GoldStandardBehaviourRegression_IsolationTest()
        {
            // having captured traffic that characterises the behaviour of the repository,
            // load the log of that traffic, and by replaying requests, confirm that the repository still works as expected
            History history = Proxy.HistoryFromFile("MissingAddressReport_CallHistory.xml");
            var verifier = Proxy.GetBehaviourVerifier(history);

            var testee = new Repository();

            var result = verifier.ConfirmBehaviourHasNotChanged(testee);

            Assert.IsTrue(result, verifier.VerificationLog.ToString());
        }
    }
}
