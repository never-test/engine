using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NeverTest.Acts;
using NeverTest.Arranges;
using NeverTest.Asserts;
using NeverTest.Yaml;

namespace NeverTest;

public class ScenarioEngine
{
    public required string EngineId { get; init; }

    /// <summary>
    /// Gets default provider that includes default services
    /// (added using <see cref="NeverTest.ScenarioBuilder&lt;T&gt;"/>)
    /// </summary>
    public required IServiceProvider DefaultProvider { get; init; }
    internal IServiceCollection DefaultServices { get; init; } = null!;
    internal IReadOnlyDictionary<ActKey, ActInstance> Acts { get; init; } = null!;
    internal IReadOnlyDictionary<AssertKey, AssertInstance> Asserts { get; init; } = null!;
    internal IReadOnlyDictionary<ArrangeKey, ArrangeInstance> Arranges { get; init; } = null!;

    public ScenarioSet<T> LoadSet<T>(string path) where T : IState
    {
        var loader = DefaultProvider.GetRequiredService<IScenarioSetLoader>();

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

    /// <summary>
    /// Creates service collection to be used per scenario,
    /// so it can be modified during arrange phase without
    /// affecting other scenarios.
    /// </summary>
    /// <returns>IServiceCollection</returns>
    public IServiceCollection CreateServiceCollection()
    {
        var clone = new ServiceCollection();

        foreach (var descriptor in DefaultServices)
        {
            clone.Add(descriptor);
        }

        return clone;
    }
}
