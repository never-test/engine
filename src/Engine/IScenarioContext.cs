namespace NeverTest;

/// <summary>
/// Represents context of currently running scenario.
/// </summary>
public interface IScenarioContext
{
    /// <summary>
    /// Returns instance of the configured engine for current scenario.
    /// </summary>
    ScenarioEngine Engine { get; init; }

    /// <summary>
    /// Gets scenario instance.
    /// </summary>
    Scenario Scenario { get; init; }

    /// <summary>
    /// Gets scenario logger.
    /// </summary>
    public ILogger Log { get; }

    /// <summary>
    /// Gets string that represents current indentation.
    /// </summary>
    public string Indent { get; }

    public JsonSerializerSettings JsonSerializerSettings { get; }

    /// <summary>
    /// Returns original object instance of the act.
    /// </summary>
    /// <param name="actual">JToken</param>
    /// <typeparam name="TOutput">Object</typeparam>
    /// <returns></returns>
    public TOutput? GetOutput<TOutput>(JToken actual);

    /// <summary>
    /// Returns all outputs in cases when there multiple outputs
    /// for given act for the same node.
    /// e.g. Repeat.
    /// </summary>
    /// <param name="actual"></param>
    /// <typeparam name="TOutput"></typeparam>
    /// <returns></returns>
    public IEnumerable<TOutput?> GetOutputs<TOutput>(JToken actual);

    internal void TrackOutput(JToken actual, object? output);
}
