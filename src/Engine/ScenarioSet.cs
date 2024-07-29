using System.Diagnostics;

namespace NeverTest;

public class ScenarioSetBase<TState> where TState : IState
{
    public string? Name { get; internal set; }
    public SetOptions Options { get; init; } = new();
    public required Scenario<TState>[] Scenarios { get; init; }

    public JToken? State { get; init; }
}

public class ScenarioSet<TState> : ScenarioSetBase<TState> where TState : IState
{
    public new required string Name { get; set; }

    public async Task<SetResult> Run(Func<JToken?, Task<TState>> stateFactory)
    {
        var start = Stopwatch.GetTimestamp();
        var results = new List<ScenarioResult>(Scenarios.Length);

        foreach (var scenario in Scenarios)
        {
            var result = await scenario.Run(stateFactory);
            results.Add(result);
        }

        return new SetResult
        {
            ScenarioResults = results.AsReadOnly(),
            Duration = Stopwatch.GetElapsedTime(start)
        };
    }
}


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
