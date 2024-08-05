using NeverTest.Aspire;

namespace NeverTests.AspireApp.Tests;

using NeverTest;

[TestClass]
public class Tests: Runner
{
    [AspireSet("basics.yaml")]
    public Task Weather(Scenario<AspireState> s) => Run(s);
}
