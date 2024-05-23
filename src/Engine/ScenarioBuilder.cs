using NeverTest.Building;

namespace NeverTest;

using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

using Yaml;

public class ScenarioBuilder<TState> where TState : IState
{
    public static readonly ConcurrentDictionary<string, Task<TState>> States = new();
    // ReSharper disable once StaticMemberInGenericType
    public static readonly ConcurrentDictionary<string, ScenarioEngine> Engines = new();
    private readonly IServiceCollection _serviceCollection = new ServiceCollection();
    private readonly ActBuilder<TState> _acts;

    public ActBuilder<TState> Acts => _acts;
    public ScenarioBuilder()
    {
        _serviceCollection
            .AddLogging(b => b
                .SetMinimumLevel(LogLevel.Information));

        _acts = new ActBuilder<TState>(this);
    }

    public IServiceCollection Services => _serviceCollection;

    public ScenarioEngine Build()
    {
        var id = GetType().FullName!;
        return Engines.GetOrAdd(id, engineId => new ScenarioEngine
        {
            EngineId = engineId,
            Provider = _serviceCollection.BuildServiceProvider(),
            Acts = Acts.Instances
        });
    }

    public ScenarioBuilder<TState> UseYaml()
    {
        _serviceCollection.AddOptions<YamlOptions>();
        _serviceCollection.AddSingleton<IScenarioSetLoader, YamlLoader>();

        return this;
    }
}

public record AssertKey(string Value);
public record ActKey(string Value);
