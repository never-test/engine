namespace NeverTest;

public class SetOptions
{
    /// <summary>
    /// Sets value that controls whether state
    /// is created per set or per scenario.
    /// </summary>
    public StateMode Mode { get; init; } = StateMode.Isolated;

    /// <summary>
    /// Allows referencing output using {{$.OutputName}} construct.
    /// Note that this has impact on performance.
    /// Sets options for all scenarios in a set.
    /// Can be overriden at scenario level.
    /// </summary>
    public bool Refs { get; set; }

    /// <summary>
    /// Enables folding.
    /// If output object has single value - simplifies by
    /// omitting act name.
    /// Does not fold named outputs.
    /// Applies to all scenarios in a set.
    /// Can be overriden at scenario level.
    /// </summary>
    public bool Folding { get; set; } = true;
}
