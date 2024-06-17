namespace NeverTest.Asserts;

internal class AssertInstance
{
    public required Type StepType { get; init; }
    public required Func<JToken?,JToken?, IScenarioContext, ValueTask> Invocation { get; init; }
}
