namespace NeverTest.Acts;

internal class ActInstance
{
    public static readonly ActInstance NotApplicable = new()
    {
        Invocation = (_, _) => throw new InvalidOperationException("This step instance is not meant to be executed. " +
                                                                   "This exception most likely indicates a bug."),
        StepType = typeof(ActInstance)
    };

    public required Func<JToken?, IScenarioContext, Task<object?>> Invocation { get; init; }
    public required Type StepType { get; init; }
}
