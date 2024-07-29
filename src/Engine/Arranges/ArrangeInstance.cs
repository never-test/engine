namespace NeverTest.Arranges;

internal class ArrangeInstance
{
    public static readonly ArrangeInstance NotApplicable = new()
    {
        Invocation = (_, _) => throw new InvalidOperationException("This step instance is not meant to be executed. " +
                                                                   "This exception most likely indicates a bug."),
        StepType = typeof(ArrangeInstance)
    };

    public required Func<JToken?, IArrangeScenarioContext, Task> Invocation { get; init; }
    public required Type StepType { get; init; }
}
