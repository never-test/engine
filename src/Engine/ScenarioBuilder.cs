namespace NeverTest;

using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

using Acts;
using Arranges;
using Asserts;
using Yaml;

/// <summary>
/// Allows building scenario engine by adding
/// arrange, act, assert steps and registering
/// dependencies in service collection.
/// </summary>
/// <typeparam name="TState"></typeparam>
public class ScenarioBuilder<TState> where TState : IState
{
    public static readonly ConcurrentDictionary<string, Task<TState>> States = new();
    // ReSharper disable once StaticMemberInGenericType
    public static readonly ConcurrentDictionary<string, ScenarioEngine> Engines = new();
    private readonly IServiceCollection _serviceCollection = new ServiceCollection();

    public ActBuilder<TState> Acts { get; }
    public AssertBuilder<TState> Asserts { get; }
    public ArrangeBuilder<TState> Arranges { get; }

    public ScenarioBuilder()
    {
        _serviceCollection
            .AddLogging(b => b
                .SetMinimumLevel(LogLevel.Information));

        _serviceCollection
            .AddOptions<JsonSerializerSettings>();

        Acts = new ActBuilder<TState>(this);
        Asserts = new AssertBuilder<TState>(this);
        Arranges = new ArrangeBuilder<TState>(this);
    }

    public IServiceCollection Services => _serviceCollection;

    public ScenarioEngine Build()
    {
        var id = GetType().FullName!;
        return Engines.GetOrAdd(id, engineId => new ScenarioEngine
        {
            EngineId = engineId,
            DefaultProvider = _serviceCollection.BuildServiceProvider(),
            DefaultServices = _serviceCollection,
            Arranges = Arranges.Instances,
            Acts = Acts.Instances,
            Asserts = Asserts.Instances,
        });
    }

    public ScenarioBuilder<TState> UseYaml()
    {
        _serviceCollection.AddOptions<YamlOptions>();
        _serviceCollection.AddSingleton<IScenarioSetLoader, YamlLoader>();

        return this;
    }
}
