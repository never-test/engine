namespace NeverTest;

using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Logging;

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

        var state = await (SetOptions.Mode switch
        {
            StateMode.Isolated => stateFactory(State),
            StateMode.Shared => ScenarioBuilder<TState>
                .States
                .GetOrAdd(StateKey, async (x) => await stateFactory(State)),
            _ => throw new ArgumentOutOfRangeException()
        });

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

        return new ScenarioResult
        {
            Scenario = this,
            Logs = log.Logs,
            Exception = exception,
            Duration = Stopwatch.GetElapsedTime(start)
        };
    }
    protected virtual async Task Run(ScenarioContext<TState> context)
    {

        TimeSpan actDuration;
        TimeSpan asserDuration;
        TimeSpan outputDuration;

        _ = new JObject
        {
            {nameof(When).ToLower(), When},
            {nameof(Then).ToLower(), Then},
            {nameof(Exception).ToLower(), Exception},
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

            if (Then is not  null && Output is null)
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
