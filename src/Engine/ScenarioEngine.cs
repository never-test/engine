using Microsoft.Extensions.DependencyInjection;
using NeverTest.Yaml;

namespace NeverTest;

public class ScenarioEngine
{
    public required string EngineId { get; init; }
    public required IServiceProvider Provider { get; init; }
    internal IReadOnlyDictionary<ActKey, StepInstance> Acts { get; init; } = null!;

    public IEnumerable<Scenario<T>> LoadScenarios<T>(string path) where T : IState
    {
        var loader = Provider.GetRequiredService<IScenarioSetLoader>();

        var set = loader.Load<T>(path);
        var key = Guid.NewGuid().ToString();

        var focused = set.Scenarios.Where(x => x.Focus).ToArray();

        foreach (var scenario in focused.Length > 0 ? focused : set.Scenarios)
        {
            scenario.StateKey = key;
            scenario.EngineId = EngineId;

            yield return scenario;
        }
    }
}
