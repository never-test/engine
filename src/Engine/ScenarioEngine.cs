using Microsoft.Extensions.DependencyInjection;
using NeverTest.Yaml;

namespace NeverTest;

public class ScenarioEngine
{
    public required string EngineId { get; init; }
    public required IServiceProvider Provider { get; init; }
    internal IReadOnlyDictionary<ActKey, StepInstance> Acts { get; init; } = null!;


    public ScenarioSet<T> LoadSet<T>(string path) where T : IState
    {
        var loader = Provider.GetRequiredService<IScenarioSetLoader>();

        var set = loader.Load<T>(path);
        var key = Guid.NewGuid().ToString();

        var focused = set.Scenarios.Where(x => x.Focus).ToArray();

        var scenarios = new List<Scenario<T>>();
        foreach (var scenario in focused.Length > 0 ? focused : set.Scenarios)
        {
            scenario.StateKey = key;
            scenario.EngineId = EngineId;
            scenario.SetOptions = set.Options;
            scenario.State = set.State;

            scenarios.Add(scenario);
        }

        return new ScenarioSet<T>
        {
            Name = set.Name ?? path,
            Scenarios = scenarios.ToArray()
        };
    }
}
