namespace NeverTest.Tests.CustomState;

using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;

public class MyRunner : RunnerBase<MyScenarioState>
{
    protected override Task<MyScenarioState> CreateState(JToken? state)
    {
        var myState = state is null ? new MyScenarioState() : state.ToObject<MyScenarioState>()!;
        return Task.FromResult(myState);
    }
}
public class MySet : ScenarioSetAttribute<MyScenarioState>
{
    public MySet(string set) : base(set)
    {
        Builder
            .UseYaml()
            .UseDefaultEngine()
            .Verbosity(LogLevel.Debug)
            .Acts
            .Add((_, context) => context.State().Increment(), o => o.Name = "increment");
    }
}
