namespace NeverTest;

internal class StepInstance
{
    public static readonly StepInstance NotApplicable = new()
    {
        Invocation = (_, _) => throw new InvalidOperationException("This step instance is not meant to be executed. " +
                                                                   "This exception most likely indicates a bug."),
        StepType = typeof(StepInstance)
    };

    public required Func<JToken?, IScenarioContext, Task<object?>> Invocation { get; init; }
    //public required Func<AssertKey, object?, JToken?, IScenarioContext, ValueTask> Assert { get; init; }
    public required Type StepType { get; init; }
}
