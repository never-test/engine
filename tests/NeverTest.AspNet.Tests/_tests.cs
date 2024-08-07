namespace NeverTest.AspNet.Tests;

[TestClass]
public class Tests : Runner<Program>
{
    [AspNetScenarioSet<Program>("basics.yaml", LogLevel.Debug)]
    public Task Basics(Scenario s) => Run(s);

    [AspNetScenarioSet<Program>("shared-mode.yaml")]
    public Task SharedMode(Scenario s) => Run(s);

    [CustomSet("custom-scenario.yaml")]
    public Task Custom(Scenario s) => Run(s);
}
