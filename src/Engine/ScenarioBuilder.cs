using NeverTest.Acts;
using NeverTest.Asserts;

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

    public ActBuilder<TState> Acts { get; }
    public AssertBuilder<TState> Asserts { get; }

    public ScenarioBuilder()
    {
        _serviceCollection
            .AddLogging(b => b
                .SetMinimumLevel(LogLevel.Information));

        _serviceCollection
            .AddOptions<JsonSerializerSettings>();

        Acts = new ActBuilder<TState>(this);
        Asserts = new AssertBuilder<TState>(this);
    }

    public IServiceCollection Services => _serviceCollection;

    public ScenarioEngine Build()
    {
        var id = GetType().FullName!;
        return Engines.GetOrAdd(id, engineId => new ScenarioEngine
        {
            EngineId = engineId,
            Provider = _serviceCollection.BuildServiceProvider(),
            Acts = Acts.Instances,
            Asserts = Asserts.Instances
        });
    }

    public ScenarioBuilder<TState> UseYaml()
    {
        _serviceCollection.AddOptions<YamlOptions>();
        _serviceCollection.AddSingleton<IScenarioSetLoader, YamlLoader>();

        return this;
    }
}

// todo: readonly struct? consider using vogen?
