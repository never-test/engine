namespace NeverTest;

public class ScenarioSet<T> where T: IState
{
    public string Name { get; internal set; } = null!;
    public required IEnumerable<Scenario<T>> Scenarios { get; init; }
}