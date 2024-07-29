namespace NeverTest;

public interface IState;

/// <summary>
/// Represents default empty scenario set state.
/// Suitable for the majority of use cases.
/// </summary>
public sealed class Empty : IState
{
    /// <summary>
    /// Gets default instance.
    /// </summary>
    public static readonly Task<Empty> Instance = Task.FromResult(new Empty());
}
