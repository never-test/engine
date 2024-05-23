namespace NeverTest;

using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

using Yaml;

public class ScenarioBuilder<TState>  where TState: IState
{
    public static readonly ConcurrentDictionary<string, Task<TState>> States = new();
    // ReSharper disable once StaticMemberInGenericType
    public static readonly ConcurrentDictionary<string, ScenarioEngine> Engines = new();
    private readonly IServiceCollection _serviceCollection = new ServiceCollection();
    private readonly Dictionary<ActKey, StepInstance> _acts = new();

    public ScenarioBuilder()
    {
        _serviceCollection
            .AddLogging(b => b
                .SetMinimumLevel(LogLevel.Information));
    }
    public ScenarioBuilder<TState> AddArrange<TArrange, TOptions>()
        where TArrange : IArrangeStep<TOptions, TState>
    {
        return this;
    }

    public IServiceCollection Services => _serviceCollection;
    public ScenarioBuilder<TState> AddAct<TAct>(string key) where TAct : class, IActStep<JToken, object?, TState> => AddAct<TAct, JToken, object?>( key);
    
    public ScenarioBuilder<TState> AddAct<TAct, TInput>(string key) where TAct : class,
        IActStep<TInput, object?, TState> where TInput : class => AddAct<TAct, TInput, object?>( key);
    public ScenarioBuilder<TState> AddAct<TAct, TInput, TOutput>(string key) where TAct : class, IActStep<TInput, TOutput, TState> where TInput : class
    {
        _serviceCollection.AddTransient<TAct>();

        var step = new StepInstance
        {
            Invocation = async (input, sc) => await sc
                .ScenarioEngine
                .Provider
                .GetRequiredService<TAct>()
                // TODO: !
                .Act(input != null ? Convert<TInput>(input) : default!, (IScenarioContext<TState>) sc),
            StepType = typeof(TAct),
           
        };
        
        _acts.Add(new(key), step);
        return this;
    }

    private Dictionary<AssertKey, Type> _asserts = new();
    
    public void AddAssert<TAssert>(string name) where TAssert: class, IAssertStep<TState>
    {
        var key = new AssertKey(name);
        _asserts.Add(key, typeof(TAssert));
        
        _serviceCollection.AddTransient<TAssert>();
    }

    private static TInput Convert<TInput>(JToken input) where TInput : class
    {
        if (typeof(TInput) == typeof(JObject) &&  input is JObject jObject)
        {
            return (jObject as TInput)!;
        }
        
        return input.ToObject<TInput>()!;
    }

    public ScenarioEngine Build()
    {
        var id = GetType().FullName!;
        
   
        return Engines.GetOrAdd(id, engineId => new ScenarioEngine
        {
            Provider = _serviceCollection.BuildServiceProvider(),
            EngineId = engineId,
            Acts = _acts
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

public class StepInstance
{
    public static readonly StepInstance NotApplicable = new()
    {
        Invocation = (_, _) => throw new InvalidOperationException("This step instance is not meant to be executed. " +
                                                                    "This exception most likely indicates a bug."),
        StepType = typeof(StepInstance)
    };

    public required Func<JToken?, IScenarioContext, Task<object?>> Invocation { get; init; }
    //public required Func<AssertKey, object?, JToken?, IScenarioContext, ValueTask> Assert { get; init; }
    public required Type StepType { get; init; }
}