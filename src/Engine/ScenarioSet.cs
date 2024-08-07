using System.Diagnostics;

namespace NeverTest;

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
