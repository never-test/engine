namespace NeverTest;

public enum StateMode
{
    /// <summary>
    /// State is created per scenario.
    /// </summary>
    Isolated = 1,

    /// <summary>
    /// State is created per set.
    /// </summary>
    Shared = 2
}
