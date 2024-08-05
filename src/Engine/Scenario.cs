namespace NeverTest;

using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Logging;
using Arranges;

public record ScenarioOptions
{
    /// <summary>
    /// Enables folding.
    /// If output object has single value - simplifies by
    /// omitting act name.
    /// Does not fold named outputs.
    /// </summary>
    public bool? Folding { get; set; }

    /// <summary>
    /// Allows referencing output using {{$.OutputName}} construct.
    /// Note that this has impact on performance.
    /// </summary>
    public bool? Refs { get; set; }

}
public abstract class Scenario
{
    public required string Name { get; init; }
    public ScenarioOptions Options { get; init; } = new();
    public LogLevel? Verbosity { get; init; }
    public string? Describe { get; init; }
    public string? Inconclusive { get; init; }
    public bool Focus { get; init; }

#nullable disable
    public string StateKey { get; internal set; }
    public string EngineId { get; internal set; }
    public SetOptions SetOptions { get; internal set; }
    internal JToken State { get; set; }
#nullable enable

    internal JObject? SharedGiven { get; set; }
    public JObject? Given { get; init; }
    public required JToken When { get; init; }
    public JToken? Then { get; init; }
    public JToken? Output { get; init; }
    public JToken? Exception { get; init; }
}

// ReSharper disable once ClassNeverInstantiated.Global
public class Scenario<TState> : Scenario where TState : IState
{
    public async Task<ScenarioResult> Run(Func<JToken?, Task<TState>> stateFactory)
    {
        var start = Stopwatch.GetTimestamp();

        if (!string.IsNullOrEmpty(Inconclusive))
        {
            return ScenarioResult.CreateInconclusive(Inconclusive, this);
        }

        var (stateTask, newState) = ConstructState(stateFactory);
        var state = await stateTask;

        var engine = ScenarioBuilder<TState>.s_engines[EngineId].Value;
        var verbosity = Verbosity ?? engine.DefaultProvider.GetRequiredService<IOptions<LoggerFilterOptions>>().Value.MinLevel;
        var jsonSettings = engine.DefaultProvider.GetRequiredService<IOptions<JsonSerializerSettings>>();
        var log = new InMemoryLogger(verbosity);

        var arrangeContext = new ArrangeContext<TState>
        {
            Scenario = this,
            Engine = engine,
            Log = log,
            Services = engine.CreateServiceCollection(),
            StateInstance = state,
            JsonSerializerSettings = jsonSettings.Value
        };

        if (newState)
        {
            await RunGlobalArrangeStage(arrangeContext);
        }

        await RunArrangeStage(arrangeContext);

        await using var provider = arrangeContext
            .Services
            .BuildServiceProvider()
            .CreateAsyncScope();

        var context = new ScenarioContext<TState>
        {
            Engine = engine,
            StateInstance = state,
            Provider = provider.ServiceProvider,
            Log = log,
            Scenario = this,
            // load settings from scenario service provider
            // so there is opportunity to change during arrange phase
            JsonSerializerSettings = provider
                .ServiceProvider
                .GetRequiredService<IOptions<JsonSerializerSettings>>()
                .Value
        };

        Exception? exception = null;
        try
        {
            await Run(context);
        }

        catch (Exception ex)
        {
            exception = ex;
        }

        return new ScenarioResult
        {
            Scenario = this,
            Logs = log.Logs,
            Exception = exception,
            Duration = Stopwatch.GetElapsedTime(start)
        };
    }

    private (Task<TState>, bool) ConstructState(Func<JToken?, Task<TState>> stateFactory)
    {
        lock (stateFactory)
        {
            var isNew = !ScenarioBuilder<TState>.s_states.ContainsKey(StateKey);

            var state = SetOptions.Mode switch
            {
                StateMode.Isolated => stateFactory(State),
                StateMode.Shared => ScenarioBuilder<TState>
                    .s_states
                    .GetOrAdd(StateKey, (x) => new Lazy<Task<TState>>(() => stateFactory(State))).Value,
                _ => throw new ArgumentException($"{SetOptions.Mode} is not supported.")
            };
            return (state, isNew);
        }
    }

    private async Task RunGlobalArrangeStage(IArrangeScenarioContext<TState> context)
    {
        if (SharedGiven is not null)
        {
            _ = new JObject { { nameof(Given).ToLowerInvariant(), SharedGiven } };
            await ExecuteArrangeNode(context, SharedGiven);
        }
    }

    private async Task RunArrangeStage(IArrangeScenarioContext<TState> context)
    {
        if (Given is not null)
        {
            _ = new JObject { { nameof(Given).ToLowerInvariant(), Given } };
            await ExecuteArrangeNode(context, Given);
        }

        var always = context
            .Engine
            .DefaultProvider.GetServices<IArrangeStep<TState>>();

        foreach (var step in always)
        {
            context.Debug("Arranging {Step}", step.GetType().FullName);
            await step.Arrange(context);
        }
    }

    private static async Task ExecuteArrangeNode(IArrangeScenarioContext<TState> context, JObject gg)
    {
        foreach (var arrange in gg.Properties())
        {
            var key = ArrangeKey.FromString(arrange.Name);
            if (context.Engine.Arranges.TryGetValue(key, out var instance))
            {
                context.Info("{Path}", arrange.Path);
                context.Dump(arrange.Value);

                await instance.Invocation(arrange.Value, context);
            }
            else
            {
                var message = $"Arrange '{key.Value}' not found. Please make sure it is registered with the engine.";
                throw new InvalidOperationException(message);
            }
        }
    }

    protected virtual async Task Run(ScenarioContext<TState> context)
    {

        // TODO: report execution times outside
        TimeSpan actDuration;
        TimeSpan asserDuration;
        TimeSpan outputDuration;

        // todo: yuck
        _ = new JObject
        {
            {nameof(When).ToLowerInvariant(), When},
            {nameof(Then).ToLowerInvariant(), Then},
            {nameof(Exception).ToLowerInvariant(), Exception},
        };

        try
        {
            var start = Stopwatch.GetTimestamp();

            await context.ProcessActs(When);

            actDuration = Stopwatch.GetElapsedTime(start);
            context.Info("Act stage completed in {Duration}", actDuration);

            if (Output is not null)
            {
                start = Stopwatch.GetTimestamp();
                await context.ProcessOutputExpectations(Output);
                outputDuration = Stopwatch.GetElapsedTime(start);
                context.Info("Output verification stage completed in {Duration}", outputDuration);
            }

            if (Then is not null && Output is null)
            {
                start = Stopwatch.GetTimestamp();
                await context.ProcessAsserts(Then!);
                asserDuration = Stopwatch.GetElapsedTime(start);
                context.Info("Assert stage completed in {Duration}", asserDuration);

            } else if (Output is null)
            {
                throw new InvalidOperationException("There should be at least one assertion.");
            }

        }
        catch (Exception ex)
        {
            if (Exception is not null)
            {
                await context.ProcessExceptionExpectation(Exception, ex);
            }
            else
            {
                throw;
            }
        }
    }
}
