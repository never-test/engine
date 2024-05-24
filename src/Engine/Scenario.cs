using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace NeverTest;

public record ScenarioOptions
{
    /// <summary>
    /// Enables folding.
    /// If output object has single value - simplifies by
    /// omitting act name.
    /// Does not fold named outputs.
    /// </summary>
    public bool Folding { get; set; } = true;

    /// <summary>
    /// Allows referencing output using {{$.OutputName}} construct.
    /// Note that this has impact on performance.
    /// </summary>
    public bool Refs { get; set; } = false;

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
#nullable enable

    public required JToken When { get; init; }
    public JToken? Then { get; init; }
    public JToken? Output { get; init; }
}

// ReSharper disable once ClassNeverInstantiated.Global
public class Scenario<TState> : Scenario where TState : IState
{
    public async Task<ScenarioResult> Run(Func<Task<TState>> stateFactory)
    {
        var timer = Stopwatch.StartNew();
        if (!string.IsNullOrEmpty(Inconclusive))
        {
            return ScenarioResult.CreateInconclusive(Inconclusive, this);
        }
        var state = await ScenarioBuilder<TState>
            .States
            .GetOrAdd(StateKey, async (x) => await stateFactory())
            .ConfigureAwait(false);

        var engine = ScenarioBuilder<TState>.Engines[EngineId];

        await using var provider = engine.Provider.CreateAsyncScope();

        var verbosity = Verbosity ?? provider.ServiceProvider.GetRequiredService<IOptions<LoggerFilterOptions>>().Value.MinLevel;
        var log = new InMemoryLogger(verbosity);

        var context = new ScenarioContext<TState>()
        {
            Engine = engine,
            StateInstance = state,
            Provider = provider.ServiceProvider,
            Log = log,
            Scenario = this
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
        timer.Stop();
        return new ScenarioResult
        {
            Scenario = this,
            Logs = log.Logs,
            Exception = exception,
            Duration = timer.Elapsed
        };
    }
    protected virtual async Task Run(ScenarioContext<TState> context)
    {
        _ = new JObject
        {
            {nameof(When).ToLower(), When},
            {nameof(Then).ToLower(), Then},
        };

        await context.ExecuteActToken(When, "when");
        await context.ExecuteAssertToken(Output);
    }
}
