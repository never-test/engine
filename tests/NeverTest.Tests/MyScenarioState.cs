namespace NeverTest.Tests;

public class MyScenarioState(TestContext context) : IState
{
    public Guid InstanceId { get; } = Guid.NewGuid();
}
