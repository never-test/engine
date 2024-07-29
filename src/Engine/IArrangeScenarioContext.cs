namespace NeverTest;

using Microsoft.Extensions.DependencyInjection;

public interface IArrangeScenarioContext : IScenarioContextBase
{
    public IServiceCollection Services { get; }
}
public interface IArrangeScenarioContext<out TState>: IArrangeScenarioContext
{
    public TState State();
}

public class ArrangeContext<TState> : IArrangeScenarioContext<TState>
{
    public TState State() => StateInstance;
    public required Scenario Scenario { get; init; }
    public required ScenarioEngine Engine { get; init; }
    public required ILogger Log { get; init; }
    public string Indent => string.Empty;
    public required JsonSerializerSettings JsonSerializerSettings { get; init; }
    public required IServiceCollection Services { get; init; }
    public required TState StateInstance { get; init; }
}
