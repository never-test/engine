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
    internal static readonly ConcurrentDictionary<string, Lazy<Task<TState>>> s_states = new();
    // ReSharper disable once StaticMemberInGenericType
    internal static readonly ConcurrentDictionary<string, Lazy<ScenarioEngine>> s_engines = new();

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

    public ScenarioEngine Build(string? id = null)
    {
        id ??= GetType().AssemblyQualifiedName!;
        return s_engines
            .GetOrAdd(
                id,
                engineId => new Lazy<ScenarioEngine>(() => new ScenarioEngine
                {
                    EngineId = engineId,
                    DefaultProvider = _serviceCollection.BuildServiceProvider(),
                    DefaultServices = _serviceCollection,
                    Arranges = Arranges.Instances,
                    Acts = Acts.Instances,
                    Asserts = Asserts.Instances,
                })).Value;
    }
    public ScenarioBuilder<TState> UseYaml()
    {
        _serviceCollection.AddOptions<YamlOptions>();
        _serviceCollection.AddSingleton<IScenarioSetLoader, YamlLoader>();

        return this;
    }
}
