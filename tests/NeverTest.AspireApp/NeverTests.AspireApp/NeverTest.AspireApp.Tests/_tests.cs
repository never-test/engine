using NeverTest.Aspire;

namespace NeverTests.AspireApp.Tests;

using NeverTest;

[TestClass]
public class Tests: AspireRunner
{
    [AspireSet("basics.yaml")]
    public Task Weather(Scenario<AspireState> s) => Run(s);
}
