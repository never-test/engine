namespace NeverTest.Tests;

public class MyScenarioState(TestContext context) : IState
{
    public TestContext Fooo { get; } = context;
    public Guid InstanceId { get; } = Guid.NewGuid();
}