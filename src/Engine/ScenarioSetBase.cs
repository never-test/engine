namespace NeverTest;

public class ScenarioSetBase<TState> where TState : IState
{
    public string? Name { get; internal set; }
    public SetOptions Options { get; init; } = new();
    public required Scenario<TState>[] Scenarios { get; init; }
    public JObject? Given { get; init; }
    public JToken? State { get; init; }
}
