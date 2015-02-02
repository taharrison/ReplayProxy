# ReplayProxy
*A new way to isolation test legacy .NET code - record existing behaviour and assert nothing has changed as you refactor.*

Repeat ad infinitum.
* I can't **refactor** this code until it is **tested**!
* I can't **test** this code until I **understand** it!
* I can't **understand** this code until I **refactor** it!

Sound familiar?

Often the only easy tests we can write are integration tests; outside in.  We start with a service which depends on a repository, which depends on a database, which has its own ugly logic hidden in obscure stored procedures.  

If we are able to write regression integration tests around the service-repository-database layers, then in theory we can start refactoring... but in practice the cost of running this test may be too high to make effective progress.
I've worked on regression tests that compare a generated report to a previously established gold standard, but which took 2 minutes to run.  Progress was slow.

**ReplayProxy** is a project aimed at reducing the costs of the first isolation tests in a legacy codebase.  If you can *understand* an end to end test, you no longer need to understand the *isolation* test conditions in order to test in isolation.

Our first integration test looks like this:

```
// Consumer ---CALLS---→ Repository
// Consumer ←---DATA---- Repository
[Test] public void CustomerReportShouldContainAllCustomersMissingAddresses()
{
    var repository = new Repository();
    var consumer = new Consumer(repository); // using inversion of control

    var result = consumer.GetMissingAddressesReport();

    AssertReportMatchesExpectations(result);
}
```
It passes but takes several minutes to run, is dependent on specific data in a shared test database, and if it fails we don't know if the data, the stored procedures, the Repository, or the Consumer is at fault.
We have no isolation.

Ideally we would like to isolate the logic in the ``Consumer`` service and refactor that.  But at the moment we don't **understand** the many calls it makes to the repository, and setting up a Stub would involve dozens of lines of initialisation code if we could somehow figure out what data was being returned.

Using ReplayProxy we can shortcut this process.

First we record the current behaviour.
```
/// Consumer ---CALLS---→ Proxy Recorder ---CALLS---→ Repository
/// Consumer ←---DATA---- Proxy Recorder ←---DATA---- Repository

var repository = new Repository();
var recorderProxy = Proxy.Record<IRepository>(repository);
var consumer = new Consumer(recorderProxy);

var result = consumer.GetMissingAddressesReport();

Proxy.SaveHistoryToFile(recorderProxy, "MissingAddressReport_CallHistory.xml");
```

Now we can include this xml history file in our test project and create an isolation test for the consumer.

```
// Consumer ---CALLS---→ Replayer -X        Repository
// Consumer ←---DATA---- Replayer -X        Repository
[Test]
public void CustomerReportShouldContainAllCustomersMissingAddresses_IsolationTest()
{
    var history = Proxy.HistoryFromFile("MissingAddressReport_CallHistory.xml");
    var replayerProxy = Proxy.Replay<IRepository>(history);

    var testee = new Consumer(replayerProxy);

    var result = testee.GetMissingAddressesReport();

    AssertReportMatchesExpectations(result);
}
```

Next, to ensure we don't break the data layer either, we can use the ``Repository`` method calls we recorded to drive data layer isolation tests.

```
// Consumer           X- Verifier ---CALLS---→ Repository
// Consumer           X- Verifier ←---DATA---- Repository
[Test]
public void ConfirmRepositoryBehviourForSampleTrafficHasNotChanged_GoldStandardBehaviourRegression_IsolationTest()
{
    History history = Proxy.HistoryFromFile("MissingAddressReport_CallHistory.xml");
    var verifier = Proxy.GetBehaviourVerifier(history);

    var testee = new Repository();

    var result = verifier.ConfirmBehaviourHasNotChanged(testee);

    Assert.IsTrue(result, verifier.VerificationLog.ToString());
}
```

Now that the tests have been split in two, you can run the fast tests as often as you like, and you still have complete confidence because you have the slower data tests to cover the rest of the story.

## Case study - Refactoring the String-Concatenated XML Report
I wrote ReplayProxy to help tame a legacy xml report generated by a dozen calls to a repository with many more (o(N+1) efficiency!) calls to a database.  The report was built by iterating through the data result sets and performing an unreadable, unmaintainable, ugly as sin series of string concatenations, which looked something like this:
```
var resultTable = _repository.GetCustomer(custId);
reportBuilder.AppendLine("<customer><Name>" + resultTable[0].ToString() + "</Name>");
reportBuilder.AppendLine(GetCustomerDetailsXml());
// etc
reportBuilder.AppendLine("</Customer>");
```
And it had magic functions that corrected errors in the concatenation process...
```
report = FixInvalidXmlCharacters(reportBuilder.ToString());
```

My first integration test took 2 minutes to execute.

By introducing an IRepository interface and then using a ReplayProxy to act as a Stub data store, the execution time of the report builder tests dropped to 2 seconds.

Powered with much faster feedback, I could refactor confidently, bit by bit shuffling the responsibilities of loading data into one logical place, replacing string concatenation with automatic Xml serialisation of an object graph.

After I had refactored the report builder I better understood what it was doing, and could start refactoring and optimising the way the repository and stored procedures worked, with a safety net provided by the ``BehvaiourVerifier`` tests I got for free.

## Limitations

At present, the ``ReplayProxy`` classes work for a large but not exhaustive set of circumstances.

The typical scenario will have
* a service/repository(s) with an interface(s).
* a consumer that depends only on the interface(s) of the service/repository(s).
* integration test(s) covering the behaviour of the consumer and service(s).

You will also need
* all the methods on the interface must depend on Xml serialisable parameters, and must have Xml serialisable return values.
* xml serialisation should be deterministic and unique.
* the interface should be stateless - the order the calls are made in should not matter.

- Happily, if you are dealing with POCOs or DataTables, Xml serialisation should not require any additional effort on your part.

Currently the following are not supported
* Stateful interfaces; e.g. ``repo.Foo(); repo.Bar();`` must behave the same as ``repo.Bar(); repo.Foo();`` 
* Mocks - the ProxyReplayer is a Stub, not a Mock, so if you need to verify that certain calls are made, this feature is not available in this version.
* Custom equality comparison.  If you record ``repo.Foo(x);``, then any time you call ``replayer.Foo(y)`` with a ``y`` that serialises to the same xml as ``x``, then the replayer will return the recorded result.  However if the serialisation of ``y`` includes irrelevant fields - like a datestamp, for example, then this is considered a different method call.  Customising how objects are compared is a feature of an upcoming version.  

## Disclaimer
These tests are **easy** to write and will get you productive **fast**.  But they should not be your first choice of solution for a problem.  Aim to clean up the code under test to the point that you can reduce or remove your reliance on the ReplayProxy tests - otherwise in the long term you'll pay for them.