namespace NeverTest.Tests.CustomState;

[TestClass]
public class MyScenarios: MyRunner
{

    [MySet("CustomState/usage.yaml")]
    public Task DefaultUsage(Scenario<MyScenarioState> scenario) => Run(scenario);

    [MySet("CustomState/shared-mode.yaml")]
    public Task ShareMode(Scenario<MyScenarioState> scenario) => Run(scenario);

    [MySet("CustomState/isolated-mode.yaml")]
    public Task IsolatedMode(Scenario<MyScenarioState> scenario) => Run(scenario);
}
